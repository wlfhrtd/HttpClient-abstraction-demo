using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Marvin.StreamExtensions;
using Movies.Client.Handlers;
using Movies.Client.Models;

namespace Movies.Client.Services
{
    public class HttpHandlersService : IIntegrationService
    {
        private readonly IHttpClientFactory httpClientFactory;

        private readonly CancellationTokenSource cancellationTokenSource;

        private static HttpClient oldStyleInstantianatedHttpClient =
            new HttpClient(
                new RetryPolicyDelegatingHandler(
                    new HttpClientHandler()
                    { AutomaticDecompression = System.Net.DecompressionMethods.GZip }, 2));


        public HttpHandlersService(IHttpClientFactory factory)
        {
            httpClientFactory = factory;
            cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task Run()
        {
            await GetMovieWithRetryPolicy(cancellationTokenSource.Token);
        }

        private async Task GetMovieWithRetryPolicy(CancellationToken token)
        {
            HttpClient httpClient = httpClientFactory.CreateClient("MoviesClient");

            HttpRequestMessage request = new(
                HttpMethod.Get,
                "api/movies/5b1c2b4d-48c7-402a-80c3-cc796ad49c6b");
                // "api/movies/030a43b0-f9a5-405a-811c-bf342524b2be");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            using (var response = await httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                token))
            {
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        Console.WriteLine("Unable to find requested object.");
                        return;
                    }
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        Console.WriteLine("Unauthorized request 401");
                        return;
                    }
                    response.EnsureSuccessStatusCode();
                }

                var stream = await response.Content.ReadAsStreamAsync();
                var movie = stream.ReadAndDeserializeFromJson<Movie>();
            }
        }
    }
}
