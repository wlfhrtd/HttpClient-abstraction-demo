using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Movies.API.Entities;
using Movies.API.Services;
using Microsoft.AspNetCore.JsonPatch;


namespace Movies.API.Controllers
{
    [Route("api/movies")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMoviesRepository moviesRepository;

        private readonly IMapper mapper;


        public MoviesController(IMoviesRepository moviesRepository, IMapper mapper)
        {
            this.moviesRepository = moviesRepository ?? throw new ArgumentNullException(nameof(moviesRepository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.Movie>>> GetMovies()
        {
            var movieEntities = await moviesRepository.GetMoviesAsync();
            return Ok(mapper.Map<IEnumerable<Models.Movie>>(movieEntities));
        }

        [HttpGet("{movieId}", Name = "GetMovie")]
        public async Task<ActionResult<Models.Movie>> GetMovie(Guid movieId)
        {
            var movieEntity = await moviesRepository.GetMovieAsync(movieId);
            if (movieEntity == null) return NotFound();
            return Ok(mapper.Map<Models.Movie>(movieEntity));
        }

        [HttpPost]
        public async Task<IActionResult> CreateMovie(
            [FromBody] Models.MovieForCreation movieForCreation)
        {
            if (movieForCreation == null) return BadRequest();

            if (!ModelState.IsValid)
            {
                // 422 - Unprocessable Entity
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var movieEntity = mapper.Map<Movie>(movieForCreation);
            moviesRepository.AddMovie(movieEntity);

            await moviesRepository.SaveChangesAsync();

            // Fetch the movie from the data store so the director is included
            await moviesRepository.GetMovieAsync(movieEntity.Id);

            return CreatedAtRoute("GetMovie",
                new { movieId = movieEntity.Id },
                mapper.Map<Models.Movie>(movieEntity));
        }

        [HttpPut("{movieId}")]
        public async Task<IActionResult> UpdateMovie(Guid movieId,
            [FromBody] Models.MovieForUpdate movieForUpdate)
        {
            // if (movieForUpdate == null) return BadRequest();

            if (!ModelState.IsValid)
            {
                // 422 - Unprocessable Entity
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var movieEntity = await moviesRepository.GetMovieAsync(movieId);

            if (movieEntity == null) return NotFound();

            // map submitted object into movie entity to ensure properties update
            mapper.Map(movieForUpdate, movieEntity);

            // for compatibility with tests
            moviesRepository.UpdateMovie(movieEntity);

            await moviesRepository.SaveChangesAsync();

            // map and returned updated entity
            return Ok(mapper.Map<Models.Movie>(movieEntity));
        }

        [HttpPatch("{movieId}")]
        public async Task<IActionResult> PartiallyUpdateMovie(Guid movieId,
            [FromBody] JsonPatchDocument<Models.MovieForUpdate> patchDoc)
        {
            var movieEntity = await moviesRepository.GetMovieAsync(movieId);

            if (movieEntity == null) return NotFound();

            // patch DTO not movie entity
            var movieToPatch = Mapper.Map<Models.MovieForUpdate>(movieEntity);

            patchDoc.ApplyTo(movieToPatch, ModelState);

            if (!ModelState.IsValid) return new UnprocessableEntityObjectResult(ModelState);

            // map back to the entity, and save
            Mapper.Map(movieToPatch, movieEntity);

            // for compatibility with tests
            moviesRepository.UpdateMovie(movieEntity);

            await moviesRepository.SaveChangesAsync();

            // map and return updated entity
            return Ok(mapper.Map<Models.Movie>(movieEntity));
        }

        [HttpDelete("{movieid}")]
        public async Task<IActionResult> DeleteMovie(Guid movieId)
        {
            var movieEntity = await moviesRepository.GetMovieAsync(movieId);

            if (movieEntity == null) return NotFound();

            moviesRepository.DeleteMovie(movieEntity);
            await moviesRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}
