using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Marvin.StreamExtensions;
using Movies.Client.Models;
using Newtonsoft.Json;


namespace Movies.Client.Services
{
    public class DealingWithErrorsAndFaultsService : IIntegrationService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly CancellationTokenSource cancellationTokenSource;

        public DealingWithErrorsAndFaultsService(IHttpClientFactory factory)
        {
            httpClientFactory = factory;
            cancellationTokenSource = new CancellationTokenSource();
        }


        public async Task Run()
        {
            // await GetMovieAndDealWithInvalidResponses(cancellationTokenSource.Token);
            await PostAndHandleValidationErrors(cancellationTokenSource.Token);
        }

        private async Task GetMovieAndDealWithInvalidResponses(CancellationToken token)
        {
            HttpClient httpClient = httpClientFactory.CreateClient("MoviesClient");

            HttpRequestMessage request = new(
                HttpMethod.Get,
                "api/movies/030a43b0-f9a5-405a-811c-bf342524b2be");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            using (var response = await httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                token))
            {
                // response.EnsureSuccessStatusCode();

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        Console.WriteLine("Unable to find requested object");
                        return;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        Console.WriteLine("Unauthorized 401");
                        return;
                    }

                    response.EnsureSuccessStatusCode();
                }

                var stream = await response.Content.ReadAsStreamAsync();
                var movie = stream.ReadAndDeserializeFromJson<Movie>();
            }
        }

        private async Task PostAndHandleValidationErrors(CancellationToken cancellationToken)
        {
            HttpClient httpClient = httpClientFactory.CreateClient("MoviesClient");

            MovieForCreation movieForCreation = new()
            {
                Title = "Pulp Fiction",
                Description = "Too short",
                DirectorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                ReleaseDate = new DateTimeOffset(new DateTime(1992, 9, 2)),
                Genre = "Crime, Drama",
            };
            var serialized = JsonConvert.SerializeObject(movieForCreation);

            using (HttpRequestMessage request = new(
                HttpMethod.Post,
                "api/movies"))
            {
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                request.Content = new StringContent(serialized);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using (var response = await httpClient.SendAsync(
                    request,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
                        {
                            var errorStream = await response.Content.ReadAsStreamAsync();
                            var errors = errorStream.ReadAndDeserializeFromJson();
                            Console.WriteLine(errors);
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
}
