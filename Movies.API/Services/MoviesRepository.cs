using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Movies.API.Contexts;
using Movies.API.Entities;


namespace Movies.API.Services
{
    public class MoviesRepository : IMoviesRepository, IDisposable
    {
        private MoviesContext context;


        public MoviesRepository(MoviesContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<Movie> GetMovieAsync(Guid movieId)
        {
            return await context.Movies.Include(m => m.Director).FirstOrDefaultAsync(m => m.Id == movieId);
        }

        public async Task<IEnumerable<Movie>> GetMoviesAsync()
        {
            return await context.Movies.Include(m => m.Director).ToListAsync();
        }

        public void UpdateMovie(Movie movieToUpdate)
        {
            // tracked by context
        }

        public void AddMovie(Movie movieToAdd)
        {
            if (movieToAdd == null) throw new ArgumentNullException(nameof(movieToAdd));
            context.Add(movieToAdd);
        }

        public void DeleteMovie(Movie movieToDelete)
        {
            if (movieToDelete == null) throw new ArgumentNullException(nameof(movieToDelete));
            context.Remove(movieToDelete);
        }

        public async Task<bool> SaveChangesAsync()
        {
            // return true if 1 or more entities were changed
            return (await context.SaveChangesAsync() > 0);
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
