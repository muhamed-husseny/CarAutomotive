using CarAutomotive.Core.Dtos;
using NetTopologySuite.Geometries; 

namespace CarAutomotive.Application.MappingProfiles 
{
    public class MechanicMappingProfile : Profile
    {
        public MechanicMappingProfile()
        {
            CreateMap<MechanicProfile, MechanicProfileDto>()
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Location.X))
                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Location.Y));

            CreateMap<CreateMechanicProfileDto, MechanicProfile>()
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src =>
                    new Point(src.Longitude, src.Latitude) { SRID = 4326 }));

            CreateMap<MechanicProfile, NearbyMechanicDto>()
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Location.X))
                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Location.Y))
                .ForMember(dest => dest.DistanceInMeters, opt => opt.Ignore());

            CreateMap<Vehicle, VehicleDto>()
               .ForMember(dest => dest.Images, opt => opt.MapFrom(src =>
               string.IsNullOrEmpty(src.ImageUrl) ? new List<string>() : new List<string> { src.ImageUrl }
    ));
        }
    }
}