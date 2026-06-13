namespace CarAutomotive.Core.MappingProfiles
{
    public class MechanicMappingProfile : Profile
    {
        public MechanicMappingProfile()
        {
            
            CreateMap<CreateMechanicProfileDto, MechanicProfile>()
                .ForMember(dest => dest.Location, opt => opt.Ignore());

           
            CreateMap<MechanicProfile, MechanicProfileDto>()
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Location.X))
                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Location.Y));

           
            CreateMap<MechanicProfile, NearbyMechanicDto>()
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Location.X))
                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Location.Y))
                .ForMember(dest => dest.DistanceInMeters, opt => opt.Ignore());
        }
    }
}