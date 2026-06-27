using CarAutomotive.Core.DTOs.MerchantDtos;

namespace CarAutomotive.Application.MappingProfiles
{
    public class MerchantMappingProfile : Profile
    {
        public MerchantMappingProfile()
        {
            CreateMap<Merchants, MerchantDto>();

            CreateMap<CreateMerchantDto, Merchants>();
        }
    }
}
