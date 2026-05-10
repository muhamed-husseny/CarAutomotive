namespace CarAutomotive.Infrastructure.Data.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AppointmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<AppointmentDto> CreateAppointmentAsync(Guid userId, CreateAppointmentDto dto)
        {

            dto.AppointmentDate = dto.AppointmentDate.ToUniversalTime();

          
            if (dto.AppointmentDate <= DateTime.UtcNow)
            {
                throw new Exception("Cannot schedule an appointment in the past.");
            }


            var conflictSpec = new AppointmentConflictSpecification(dto.MechanicId, dto.AppointmentDate);
            var existingAppointments = await _unitOfWork.Repository<Appointment>().ListAsync(conflictSpec);

            if (existingAppointments.Any())
            {
                throw new Exception("The mechanic is already booked for this specific time. Please choose another time.");
            }

            
            var appointment = new Appointment
            {
                UserId = userId,
                MechanicId = dto.MechanicId,
                AppointmentDate = dto.AppointmentDate,
                Notes = dto.Notes,
                Status = AppointmentStatus.Pending
            };

           
            _unitOfWork.Repository<Appointment>().Add(appointment);
            await _unitOfWork.CompleteAsync();

           
            return _mapper.Map<AppointmentDto>(appointment);
        }
        public async Task<AppointmentDto> GetAppointmentByIdAsync(Guid appointmentId)
        {
           
            var appointment = await _unitOfWork.Repository<Appointment>().GetById(appointmentId);

            if (appointment == null)
            {
                throw new Exception("The appointment does not exist!");
            }

            return _mapper.Map<AppointmentDto>(appointment);
        }

        public async Task<IReadOnlyList<AppointmentDto>> GetMechanicAppointmentsAsync(Guid mechanicId)
        {
            var spec = new AppointmentByMechanicSpecification(mechanicId);
            var appointments = await _unitOfWork.Repository<Appointment>().ListAsync(spec);

            return _mapper.Map<IReadOnlyList<AppointmentDto>>(appointments);
        }

        public async Task<IReadOnlyList<AppointmentDto>> GetUserAppointmentsAsync(Guid userId)
        {
            var spec = new AppointmentByUserSpecification(userId);
            var appointments = await _unitOfWork.Repository<Appointment>().ListAsync(spec);

            return _mapper.Map<IReadOnlyList<AppointmentDto>>(appointments);
        }

        public async Task<AppointmentDto> UpdateAppointmentStatusAsync(Guid appointmentId, AppointmentStatus newStatus, Guid requestingUserId)
        {
            var appointment = await _unitOfWork.Repository<Appointment>().GetById(appointmentId);

            if (appointment == null)
            {
                throw new Exception("The reservation does not exist!");
            }

            appointment.Status = newStatus;

            _unitOfWork.Repository<Appointment>().Update(appointment);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<AppointmentDto>(appointment);
        }
    }
}
