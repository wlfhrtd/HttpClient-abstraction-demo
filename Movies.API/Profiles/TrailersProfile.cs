using AutoMapper;


namespace Movies.API.Profiles
{
    public class TrailersProfile : Profile
    {
        public TrailersProfile()
        {
            CreateMap<InternalModels.Trailer, Models.Trailer>();
            CreateMap<Models.TrailerForCreation, InternalModels.Trailer>();
        }
    }
}
