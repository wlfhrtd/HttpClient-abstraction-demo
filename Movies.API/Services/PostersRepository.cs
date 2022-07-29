using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Movies.API.Contexts;
using Movies.API.InternalModels;


namespace Movies.API.Services
{
    public class PostersRepository : IPostersRepository, IDisposable
    {
        private MoviesContext context;


        public PostersRepository(MoviesContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<Poster> GetPosterAsync(Guid movieId, Guid posterId)
        {
            // name gen from movie title
            var movie = await context.Movies.FirstOrDefaultAsync(m => m.Id == movieId);

            if (movie == null) throw new Exception($"Movie with id {movieId} not found.");
            // gen 500KB poster 
            var random = new Random();
            var generatedBytes = new byte[524288];
            random.NextBytes(generatedBytes);

            return new Poster()
            {
                Bytes = generatedBytes,
                Id = posterId,
                MovieId = movieId,
                Name = $"{movie.Title} poster number {DateTime.UtcNow.Ticks}",
            };
        }

        public async Task<Poster> AddPoster(Guid movieId, Poster posterToAdd)
        {
            // fake; set id and return
            posterToAdd.MovieId = movieId;
            posterToAdd.Id = Guid.NewGuid();
            return await Task.FromResult(posterToAdd);
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
