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


namespace Movies.Client
{
    public class MoviesClient
    {
        private readonly HttpClient client;


        public MoviesClient(HttpClient c)
        {
            client = c;
            client.BaseAddress = new Uri("http://localhost:35087");
            client.Timeout = new TimeSpan(0, 0, 30);
            client.DefaultRequestHeaders.Clear();
        }


        public async Task<IEnumerable<Movie>> FindAll(CancellationToken cancellationToken)
        {
            HttpRequestMessage request = new(HttpMethod.Get, "api/movies");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            using (var response = await client.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken))
            {
                var stream = await response.Content.ReadAsStreamAsync();
                response.EnsureSuccessStatusCode();

                return stream.ReadAndDeserializeFromJson<List<Movie>>();
            }
        }
    }
}
