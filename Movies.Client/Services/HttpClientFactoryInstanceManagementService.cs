using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Marvin.StreamExtensions;
using Movies.Client.Models;

namespace Movies.Client.Services
{
    public class HttpClientFactoryInstanceManagementService : IIntegrationService
    {
        private readonly CancellationTokenSource cancellationTokenSource = new();

        private readonly IHttpClientFactory httpClientFactory;

        private readonly MoviesClient moviesClient;


        public HttpClientFactoryInstanceManagementService(IHttpClientFactory httpClientFactory, MoviesClient moviesClient)
        {
            this.httpClientFactory = httpClientFactory;
            this.moviesClient = moviesClient;
        }

        public async Task Run()
        {
            // await TestDisposeHttpClient(cancellationTokenSource.Token);
            // await TestReuseHttpClient(cancellationTokenSource.Token);
            // await GetMoviesWithHttpClientFromFactory(cancellationTokenSource.Token);
            // await GetMoviesWithNamedHttpClientFromFactory(cancellationTokenSource.Token);
            // await GetMoviesWithTypedHttpClientFromFactory(cancellationTokenSource.Token);
            await GetMoviesViaMoviesClient(cancellationTokenSource.Token);
        }

        private async Task TestDisposeHttpClient(CancellationToken cancellationToken)
        {
            for (int i = 0; i < 10; i++)
            {
                using (HttpClient httpClient = new())
                {
                    HttpRequestMessage request = new(
                        HttpMethod.Get,
                        "https://www.google.com");

                    using (var response = await httpClient.SendAsync(
                        request,
                        HttpCompletionOption.ResponseHeadersRead,
                        cancellationToken))
                    {
                        var stream = await response.Content.ReadAsStreamAsync();
                        response.EnsureSuccessStatusCode();
                        Console.WriteLine($"Request completed with status code: {response.StatusCode}");
                    }
                }
            }

            Console.ReadKey();
        }

        private async Task TestReuseHttpClient(CancellationToken cancellationToken)
        {
            HttpClient httpClient = new();

            for (int i = 0; i < 10; i++)
            {
                HttpRequestMessage request = new(
                    HttpMethod.Get,
                    "https://www.google.com");

                using (var response = await httpClient.SendAsync(
                    request,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken))
                {
                    var stream = response.Content.ReadAsStreamAsync();
                    response.EnsureSuccessStatusCode();
                    Console.WriteLine($"Request completed with status code: {response.StatusCode}");
                }
            }

            Console.ReadKey();
        }

        private async Task GetMoviesWithHttpClientFromFactory(CancellationToken cancellationToken)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();

            HttpRequestMessage request = new(
                HttpMethod.Get,
                "http://localhost:35087/api/movies");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken))
            {
                var stream = await response.Content.ReadAsStreamAsync();
                response.EnsureSuccessStatusCode();
                var movies = stream.ReadAndDeserializeFromJson<List<Movie>>();
            }
        }

        private async Task GetMoviesWithNamedHttpClientFromFactory(CancellationToken cancellationToken)
        {
            HttpClient httpClient = httpClientFactory.CreateClient("MoviesClient");

            HttpRequestMessage request = new(
                HttpMethod.Get,
                "api/movies");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            using (var response = await httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken))
            {
                var stream = await response.Content.ReadAsStreamAsync();
                response.EnsureSuccessStatusCode();
                var movies = stream.ReadAndDeserializeFromJson<List<Movie>>();
            }
        }

        //private async Task GetMoviesWithTypedHttpClientFromFactory(CancellationToken cancellationToken)
        //{
        //    HttpRequestMessage request = new(
        //        HttpMethod.Get,
        //        "api/movies");
        //    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //    request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

        //    using (var response = await moviesClient.Client.SendAsync(
        //        request,
        //        HttpCompletionOption.ResponseHeadersRead,
        //        cancellationToken))
        //    {
        //        var stream = await response.Content.ReadAsStreamAsync();
        //        response.EnsureSuccessStatusCode();
        //        var movies = stream.ReadAndDeserializeFromJson<List<Movie>>();
        //    }
        //}

        private async Task GetMoviesViaMoviesClient(CancellationToken cancellationToken)
        {
            var movies = await moviesClient.FindAll(cancellationToken);
        }
    }
}
