using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Movies.API.Contexts;
using Movies.API.InternalModels;


namespace Movies.API.Services
{
    public class TrailersRepository : ITrailersRepository, IDisposable
    {
        private MoviesContext context;


        public TrailersRepository(MoviesContext context)
        {
            context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<Trailer> GetTrailerAsync(Guid movieId, Guid trailerId)
        {
            // name gen from movie title
            var movie = await context.Movies.FirstOrDefaultAsync(m => m.Id == movieId);

            if (movie == null) throw new Exception($"Movie with id {movieId} not found.");
            // gen 50-100MB trailer
            var random = new Random();
            var generatedByteLength = random.Next(52428800, 104857600);
            var generatedBytes = new byte[generatedByteLength];
            random.NextBytes(generatedBytes);

            return new Trailer()
            {
                Bytes = generatedBytes,
                Id = trailerId,
                MovieId = movieId,
                Name = $"{movie.Title} trailer number {DateTime.UtcNow.Ticks}",
                Description = $"{movie.Title} trailer description {DateTime.UtcNow.Ticks}"
            };
        }

        public async Task<Trailer> AddTrailer(Guid movieId, Trailer trailerToAdd)
        {
            // fake; set id and return
            trailerToAdd.MovieId = movieId;
            trailerToAdd.Id = Guid.NewGuid();
            return await Task.FromResult(trailerToAdd);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (context != null)
                {
                    context.Dispose();
                    context = null;
                }
            }
        }
    }
}
