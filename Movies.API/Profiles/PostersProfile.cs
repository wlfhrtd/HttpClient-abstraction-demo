using AutoMapper;


namespace Movies.API.Profiles
{
    public class PostersProfile : Profile
    {
        public PostersProfile()
        {
            CreateMap<InternalModels.Poster, Models.Poster>();
            CreateMap<Models.PosterForCreation, InternalModels.Poster>();
        }
    }
}
