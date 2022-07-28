using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Movies.API.Entities;


namespace Movies.API.Services
{
    public interface IMoviesRepository
    {
        Task<Movie> GetMovieAsync(Guid movieId);

        Task<IEnumerable<Movie>> GetMoviesAsync();

        void UpdateMovie(Movie movieToUpdate);

        void AddMovie(Movie movieToAdd);

        void DeleteMovie(Movie movieToDelete);

        Task<bool> SaveChangesAsync();
    }
}
