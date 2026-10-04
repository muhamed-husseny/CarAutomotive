namespace CarAutomotive.Application.MappingProfiles
{
    public class AppointmenMappingProfile : Profile
    {
        public AppointmenMappingProfile()
        {
            CreateMap<Appointment, AppointmentDto>()
                .ForMember(d => d.MechanicName, o => o.MapFrom(s => s.Mechanic.Name))
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.User.UserName))         
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));
        }
    }
}
