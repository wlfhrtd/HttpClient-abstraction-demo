using Microsoft.AspNetCore.Mvc;
using System;
using AutoMapper;
using Movies.API.InternalModels;
using Movies.API.Services;
using System.Threading.Tasks;


namespace Movies.API.Controllers
{
    [Route("api/movies/{movieId}/trailers")]
    [ApiController]
    public class TrailersController : ControllerBase
    {
        private readonly ITrailersRepository trailersRepository;

        private readonly IMapper mapper;


        public TrailersController(ITrailersRepository trailersRepository, IMapper mapper)
        {
            this.trailersRepository = trailersRepository ?? throw new ArgumentNullException(nameof(trailersRepository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }


        [HttpGet("{trailerId}", Name = "GetTrailer")]
        public async Task<ActionResult<Models.Trailer>> GetTrailer(Guid movieId, Guid trailerId)
        {
            var trailer = await trailersRepository.GetTrailerAsync(movieId, trailerId);
            if (trailer == null) return NotFound();
            return Ok(mapper.Map<Models.Trailer>(trailer));
        }

        [HttpPost]
        public async Task<IActionResult> CreateTrailer(Guid movieId,
            [FromBody] Models.TrailerForCreation trailerForCreation)
        {
            if (trailerForCreation == null) return BadRequest();

            if (!ModelState.IsValid)
            {
                // 422 - Unprocessable Entity
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var trailer = mapper.Map<Trailer>(trailerForCreation);
            var createdTrailer = await trailersRepository.AddTrailer(movieId, trailer);

            // no need to save since immediately persisted

            // map the trailer from the repository to a shared model trailer
            return CreatedAtRoute("GetTrailer",
                new { movieId, trailerId = createdTrailer.Id },
                mapper.Map<Models.Trailer>(createdTrailer));
        }
    }
}
