using CarAutomotive.Core.DTOs.ReviewsDto;

namespace CarAutomotive.Application.MappingProfiles
{
    public class ReviewMappingProfile : Profile
    {
        public ReviewMappingProfile()
        {
            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.User.DisplayName));
        }
    }
}
