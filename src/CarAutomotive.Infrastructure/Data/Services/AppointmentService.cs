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
        public async Task<AppointmentDto> GetAppointmentByIdAsync(Guid appointmentId, Guid requestingUserId, bool isAdmin = false)
        {
            var spec = new AppointmentByIdSpecification(appointmentId);
            var appointment = await _unitOfWork.Repository<Appointment>().GetByIdWithSpecAsync(spec);

            if (appointment == null)
            {
                throw new KeyNotFoundException("The appointment does not exist.");
            }

            EnsureAppointmentAccess(appointment, requestingUserId, isAdmin);

            return _mapper.Map<AppointmentDto>(appointment);
        }

        public async Task<IReadOnlyList<AppointmentDto>> GetMechanicAppointmentsAsync(Guid mechanicId, Guid requestingUserId, bool isAdmin = false)
        {
            if (!isAdmin && mechanicId != requestingUserId)
            {
                var mechanic = await _unitOfWork.Repository<MechanicProfile>().GetById(mechanicId);
                if (mechanic == null || mechanic.UserId != requestingUserId)
                {
                    throw new UnauthorizedAccessException("You are not authorized to view this mechanic's appointments.");
                }
            }

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

        public async Task<AppointmentDto> UpdateAppointmentStatusAsync(Guid appointmentId, AppointmentStatus newStatus, Guid requestingUserId, bool isAdmin = false)
        {
            var spec = new AppointmentByIdSpecification(appointmentId);
            var appointment = await _unitOfWork.Repository<Appointment>().GetByIdWithSpecAsync(spec);

            if (appointment == null)
            {
                throw new KeyNotFoundException("The appointment does not exist.");
            }

            EnsureAppointmentAccess(appointment, requestingUserId, isAdmin);

            appointment.Status = newStatus;

            _unitOfWork.Repository<Appointment>().Update(appointment);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<AppointmentDto>(appointment);
        }

        private static void EnsureAppointmentAccess(Appointment appointment, Guid requestingUserId, bool isAdmin)
        {
            if (isAdmin)
                return;

            var isOwner = appointment.UserId == requestingUserId;
            var isAssignedMechanic = appointment.MechanicId == requestingUserId
                || (appointment.Mechanic != null && appointment.Mechanic.UserId == requestingUserId);

            if (!isOwner && !isAssignedMechanic)
            {
                throw new UnauthorizedAccessException("You are not authorized to access this appointment.");
            }
        }
    }
}
