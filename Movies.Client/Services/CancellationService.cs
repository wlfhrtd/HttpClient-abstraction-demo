using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Marvin.StreamExtensions;
using Movies.Client.Models;


namespace Movies.Client.Services
{
    public class CancellationService : IIntegrationService
    {
        private static HttpClient httpClient =
            new HttpClient(new HttpClientHandler() { AutomaticDecompression = System.Net.DecompressionMethods.GZip });

        private CancellationTokenSource cancellationTokenSource = new();


        public CancellationService()
        {
            httpClient.BaseAddress = new Uri("http://localhost:35087");
            httpClient.Timeout = new TimeSpan(0, 0, 5);
            httpClient.DefaultRequestHeaders.Clear();
        }


        public async Task Run()
        {
            // cancellationTokenSource.CancelAfter(2000);
            // await GetTrailerAndCancel(cancellationTokenSource.Token);
            await GetTrailerAndHandleTimeout();
        }

        private async Task GetTrailerAndCancel(CancellationToken cancellationToken)
        {
            HttpRequestMessage request = new(
                HttpMethod.Get,
                $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/trailers/{Guid.NewGuid()}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            //CancellationTokenSource cancellationTokenSource = new();
            //cancellationTokenSource.CancelAfter(2000);

            try
            {
                using (var response = await httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken))
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    response.EnsureSuccessStatusCode();
                    var trailer = stream.ReadAndDeserializeFromJson<Trailer>();
                }
            }
            catch (OperationCanceledException oc)
            {
                Console.WriteLine($"Operation was canceled: {oc.Message}");
            }
        }

        private async Task GetTrailerAndHandleTimeout()
        {
            HttpRequestMessage request = new(
                HttpMethod.Get,
                $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/trailers/{Guid.NewGuid()}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            try
            {
                using (var response = await httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead))
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    response.EnsureSuccessStatusCode();
                    var trailer = stream.ReadAndDeserializeFromJson<Trailer>();
                }
            }
            catch (OperationCanceledException oc)
            {
                Console.WriteLine($"Operation was canceled: {oc.Message}");
            }
        }
    }
}
