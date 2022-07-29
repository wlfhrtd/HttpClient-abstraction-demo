using Movies.Client.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Marvin.StreamExtensions;


namespace Movies.Client.Services
{
    public class StreamService : IIntegrationService
    {
        // private static HttpClient httpClient = new();

        private static HttpClient httpClient =
            new(new HttpClientHandler() { AutomaticDecompression = System.Net.DecompressionMethods.GZip});


        public StreamService()
        {
            httpClient.BaseAddress = new Uri("http://localhost:35087");
            httpClient.Timeout = new TimeSpan(0, 0, 30);
            httpClient.DefaultRequestHeaders.Clear();
        }


        public async Task Run()
        {
            // await GetPosterWithStream();
            // await GetPosterWithStreamAndCompletionMode();
            // await TestGetPosterWithoutStream();
            // await TestGetPosterWithStream();
            // await TestGetPosterWithStreamAndCompletionMode();
            // await PostWithStream();
            // await PostAndReadWithStream();
            // await TestPostWithoutStream();
            // await TestPostWithStream();
            // await TestPostAndReadWithStream();
            await GetPosterWithCompression();
        }
        // starts reading content when it fully loaded
        private async Task GetPosterWithStream()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters/{Guid.NewGuid()}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync();
                var poster = stream.ReadAndDeserializeFromJson<Poster>();

                /*
                using (StreamReader streamReader = new(stream))
                {
                    using (JsonTextReader jsonTextReader = new(streamReader))
                    {
                        JsonSerializer jsonSerializer = new();
                        Poster poster = jsonSerializer.Deserialize<Poster>(jsonTextReader);
                    }
                }
                */
            }
        }
        // starts reading content when headers received
        private async Task GetPosterWithStreamAndCompletionMode()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters/{Guid.NewGuid()}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)) // magic enum
            {
                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync();
                var poster = stream.ReadAndDeserializeFromJson<Poster>();

                /*
                using (StreamReader streamReader = new(stream))
                {
                    using (JsonTextReader jsonTextReader = new(streamReader))
                    {
                        JsonSerializer jsonSerializer = new();
                        Poster poster = jsonSerializer.Deserialize<Poster>(jsonTextReader);
                    }
                }
                */
            }
        }
        // for benchmark
        private async Task GetPosterWithoutStream()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters/{Guid.NewGuid()}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var poster = JsonConvert.DeserializeObject<Poster>(content);
        }
        
        private async Task TestGetPosterWithoutStream()
        {
            // warm up
            await GetPosterWithoutStream();

            var stopWatch = Stopwatch.StartNew();
            for (int i = 0; i < 1000; i++) await GetPosterWithoutStream();
            stopWatch.Stop();

            long res = stopWatch.ElapsedMilliseconds / 1000L;
            Console.WriteLine($"Elapsed seconds WITHOUT stream: {res}\n" +
                $"Average time per request in millis: {res}\n");
        }

        private async Task TestGetPosterWithStream()
        {
            // warm up
            await GetPosterWithStream();

            var stopWatch = Stopwatch.StartNew();
            for (int i = 0; i < 1000; i++) await GetPosterWithStream();
            stopWatch.Stop();

            long res = stopWatch.ElapsedMilliseconds / 1000L;
            Console.WriteLine($"Elapsed seconds WITH stream: {res}\n" +
                $"Average time per request in millis: {res}\n");
        }

        private async Task TestGetPosterWithStreamAndCompletionMode()
        {
            // warm up
            await GetPosterWithStreamAndCompletionMode();

            var stopWatch = Stopwatch.StartNew();
            for (int i = 0; i < 1000; i++) await GetPosterWithStreamAndCompletionMode();
            stopWatch.Stop();

            long res = stopWatch.ElapsedMilliseconds / 1000L;
            Console.WriteLine($"Elapsed seconds with stream and completion mode: {res}\n" +
                $"Average time per request in millis: {res}\n");
        }

        private async Task PostWithStream()
        {
            var generatedBytes = new byte[524288];
            new Random().NextBytes(generatedBytes);

            PosterForCreation posterForCreation = new()
            {
                Name = "New poster",
                Bytes = generatedBytes,
            };

            MemoryStream memoryStream = new();
            memoryStream.SerializeToJsonAndWrite(posterForCreation, new UTF8Encoding(), 1024, true);

            /*
            using (StreamWriter streamWriter = new(memoryStream, new UTF8Encoding(), 1024, true))
            {
                using (JsonTextWriter jsonTextWriter = new(streamWriter))
                {
                    new JsonSerializer().Serialize(jsonTextWriter, posterForCreation);
                    jsonTextWriter.Flush();
                }
            }
            */

            memoryStream.Seek(0, SeekOrigin.Begin);
            using (HttpRequestMessage request = new(
                HttpMethod.Post,
                $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters"))
            {
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (StreamContent streamContent = new(memoryStream))
                {
                    request.Content = streamContent;
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var response = await httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    var createdContent = await response.Content.ReadAsStringAsync();
                    var createdPoster = JsonConvert.DeserializeObject<Poster>(createdContent);
                }
            }
        }

        private async Task PostAndReadWithStream()
        {
            var generatedBytes = new byte[524288];
            new Random().NextBytes(generatedBytes);

            PosterForCreation posterForCreation = new()
            {
                Name = "New poster",
                Bytes = generatedBytes,
            };

            MemoryStream memoryStream = new();
            memoryStream.SerializeToJsonAndWrite(posterForCreation, new UTF8Encoding(), 1024, true);

            memoryStream.Seek(0, SeekOrigin.Begin);
            using (HttpRequestMessage request = new(
                HttpMethod.Post,
                $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters"))
            {
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (StreamContent streamContent = new(memoryStream))
                {
                    request.Content = streamContent;
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                    {
                        response.EnsureSuccessStatusCode();
                        var stream = await response.Content.ReadAsStreamAsync();
                        var createdPoster = stream.ReadAndDeserializeFromJson<Poster>();
                    }
                }
            }
        }
        // for benchmark
        private async Task PostWithoutStream()
        {
            var generated = new byte[524288];
            new Random().NextBytes(generated);
            PosterForCreation poster = new() { Name = "New poster", Bytes = generated };

            var serialized = JsonConvert.SerializeObject(poster);

            HttpRequestMessage request = new(
                HttpMethod.Post,
                "api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(serialized);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var created = JsonConvert.DeserializeObject<Poster>(content);
        }

        private async Task TestPostWithoutStream()
        {
            await PostWithoutStream();

            var stopWatch = Stopwatch.StartNew();
            for (int i = 0; i < 1000; i++) await PostWithoutStream();
            stopWatch.Stop();
            long res = stopWatch.ElapsedMilliseconds / 1000L;
            Console.WriteLine($"Post without stream - Elapsed time in sec / millis per request: {res}");
        }

        private async Task TestPostWithStream()
        {
            await PostWithStream();

            var stopWatch = Stopwatch.StartNew();
            for (int i = 0; i < 1000; i++) await PostWithStream();
            stopWatch.Stop();
            long res = stopWatch.ElapsedMilliseconds / 1000L;
            Console.WriteLine($"Post with stream - Elapsed time in sec / millis per request: {res}");
        }

        private async Task TestPostAndReadWithStream()
        {
            await PostAndReadWithStream();

            var stopWatch = Stopwatch.StartNew();
            for (int i = 0; i < 1000; i++) await PostAndReadWithStream();
            stopWatch.Stop();
            long res = stopWatch.ElapsedMilliseconds / 1000L;
            Console.WriteLine($"Post with streams for post and read - Elapsed time in sec / millis per request: {res}");
        }

        private async Task GetPosterWithCompression()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters/{Guid.NewGuid()}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            using (var response = await httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync();
                var poster = stream.ReadAndDeserializeFromJson<Poster>();
            }
        }
    }
}
