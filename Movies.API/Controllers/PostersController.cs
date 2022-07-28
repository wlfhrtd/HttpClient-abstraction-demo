using Microsoft.AspNetCore.Mvc;
using System;
using AutoMapper;
using Movies.API.InternalModels;
using Movies.API.Services;
using System.Threading.Tasks;


namespace Movies.API.Controllers
{
    [Route("api/movies/{movieId}/posters")]
    [ApiController]
    public class PostersController : ControllerBase
    {
        private readonly IPostersRepository postersRepository;

        private readonly IMapper mapper;


        public PostersController(IPostersRepository postersRepository, IMapper mapper)
        {
            this.postersRepository = postersRepository ?? throw new ArgumentNullException(nameof(postersRepository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }


        [HttpGet("{posterId}", Name = "GetPoster")]
        public async Task<ActionResult<Models.Poster>> GetPoster(Guid movieId, Guid posterId)
        {
            var poster = await postersRepository.GetPosterAsync(movieId, posterId);
            if (poster == null) return NotFound();
            return Ok(mapper.Map<Models.Poster>(poster));
        }

        [HttpPost]
        public async Task<IActionResult> CreatePoster(Guid movieId,
            [FromBody] Models.PosterForCreation posterForCreation)
        {
            if (posterForCreation == null) return BadRequest();

            if (!ModelState.IsValid)
            {
                // 422 - Unprocessable Entity
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var poster = mapper.Map<Poster>(posterForCreation);
            var createdPoster = await postersRepository.AddPoster(movieId, poster);

            // no need to save since immediately persisted

            // map the poster from the repository to a shared model poster
            return CreatedAtRoute("GetPoster",
                new { movieId, posterId = createdPoster.Id },
                mapper.Map<Models.Poster>(createdPoster));
        }
    }
}
