using AutoMapper;


namespace Movies.API.Profiles
{
    public class MoviesProfile : Profile
    {
        public MoviesProfile()
        {
            CreateMap<Entities.Movie, Models.Movie>()
                .ForMember(dest => dest.Director, opt => opt.MapFrom(src =>
                   $"{src.Director.FirstName} {src.Director.LastName}"));

            CreateMap<Models.MovieForCreation, Entities.Movie>();

            CreateMap<Models.MovieForUpdate, Entities.Movie>().ReverseMap();
        }
    }
}
