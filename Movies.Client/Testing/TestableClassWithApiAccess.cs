using Movies.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Marvin.StreamExtensions;


namespace Movies.Client.Testing
{
    public class TestableClassWithApiAccess
    {
        private readonly HttpClient httpClient;

        public TestableClassWithApiAccess(HttpClient client)
        {
            httpClient = client;
        }

        public async Task<Movie> FindOne(CancellationToken cancellationToken)
        {
            HttpRequestMessage request = new(
                HttpMethod.Get,
                "api/movies/030a43b0-f9a5-405a-811c-bf342524b2be");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            using (var response = await httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken))
            {
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        Console.WriteLine("Unable to find requested object.");
                        return null;
                    }
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {

                    }

                    response.EnsureSuccessStatusCode();
                }

                var stream = await response.Content.ReadAsStreamAsync();
                return stream.ReadAndDeserializeFromJson<Movie>();
            }
        }
    }
}
