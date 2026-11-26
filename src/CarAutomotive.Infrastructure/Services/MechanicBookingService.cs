using AutoMapper;
using CarAutomotive.Core.DTOs;
using CarAutomotive.Core.Entities;
using CarAutomotive.Core.Entities.Appointment;
using CarAutomotive.Core.Enums;
using CarAutomotive.Core.Interfaces;
using CarAutomotive.Core.Specifications;

namespace CarAutomotive.Infrastructure.Services
{
    public class MechanicBookingService : IMechanicBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MechanicBookingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<MechanicBookingDto>> GetMechanicBookingsAsync(Guid mechanicId)
        {
            var spec = new MechanicBookingsSpecification(mechanicId);
            var appointments = await _unitOfWork.Repository<Appointment>().GetAllWithSpecAsync(spec);

            var result = new List<MechanicBookingDto>();

            foreach (var a in appointments)
            {
                // Resolve Vehicle
                var vehicleInfo = "N/A";
                if (a.Vehicle != null)
                {
                    vehicleInfo = $"{a.Vehicle.Make} {a.Vehicle.Model} {a.Vehicle.Year}".Trim();
                }
                else
                {
                    // Fallback to lookup client's vehicle
                    var clientVehicles = await _unitOfWork.Repository<Vehicle>()
                        .GetAllWithSpecAsync(new VehiclesByClientSpec(a.UserId));
                    var firstVehicle = clientVehicles.FirstOrDefault();
                    if (firstVehicle != null)
                    {
                        vehicleInfo = $"{firstVehicle.Make} {firstVehicle.Model} {firstVehicle.Year}".Trim();
                    }
                }

                var clientName = a.User != null 
                    ? (a.User.DisplayName ?? a.User.UserName ?? "Client") 
                    : "Client";

                var bookingDto = new MechanicBookingDto
                {
                    Id = a.AppointmentId != Guid.Empty ? a.AppointmentId : Guid.NewGuid(),
                    ClientUserId = a.UserId,
                    ClientName = clientName,
                    Vehicle = vehicleInfo,
                    ServiceType = !string.IsNullOrEmpty(a.ServiceType) ? a.ServiceType : (!string.IsNullOrEmpty(a.Notes) ? a.Notes : "General Repair"),
                    ScheduledAt = a.AppointmentDate,
                    Status = MapStatusToString(a.Status),
                    Invoice = a.Invoice != null ? new InvoiceSummaryDto
                    {
                        Id = a.Invoice.Id,
                        LaborTotalEgp = a.Invoice.LaborTotalEgp,
                        PartsTotalEgp = a.Invoice.PartsTotalEgp,
                        TotalAmountEgp = a.Invoice.TotalAmountEgp,
                        IsPaid = a.Invoice.IsPaid,
                        Parts = a.Invoice.Parts.Select(p => new InvoicePartDto
                        {
                            Name = p.Name,
                            Price = p.Price
                        }).ToList()
                    } : null
                };

                result.Add(bookingDto);
            }

            return result;
        }

        public async Task<bool> UpdateBookingStatusAsync(Guid mechanicId, Guid appointmentId, string status)
        {
            var spec = new MechanicBookingsSpecification(mechanicId, appointmentId);
            var appointment = await _unitOfWork.Repository<Appointment>().GetEntityWithSpec(spec);

            if (appointment == null)
            {
                return false;
            }

            appointment.Status = ParseStatus(status);
            _unitOfWork.Repository<Appointment>().Update(appointment);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> GenerateInvoiceAsync(Guid mechanicId, Guid appointmentId, CreateInvoiceDto dto)
        {
            if (dto == null)
            {
                return false;
            }

            var spec = new MechanicBookingsSpecification(mechanicId, appointmentId);
            var appointment = await _unitOfWork.Repository<Appointment>().GetEntityWithSpec(spec);

            if (appointment == null)
            {
                return false;
            }

            decimal partsTotal = dto.Parts != null ? dto.Parts.Sum(p => p.Price) : 0m;
            decimal totalAmount = dto.LaborTotal + partsTotal;

            if (appointment.Invoice == null)
            {
                var invoice = new Invoice
                {
                    AppointmentId = appointment.Id,
                    AppointmentGuid = appointment.AppointmentId,
                    LaborTotalEgp = dto.LaborTotal,
                    PartsTotalEgp = partsTotal,
                    TotalAmountEgp = totalAmount,
                    IsPaid = dto.IsPaid,
                    PaidAt = dto.IsPaid ? DateTime.UtcNow : null,
                    Parts = dto.Parts?.Select(p => new InvoicePart
                    {
                        Name = p.Name,
                        Price = p.Price
                    }).ToList() ?? new List<InvoicePart>()
                };

                _unitOfWork.Repository<Invoice>().Add(invoice);
            }
            else
            {
                appointment.Invoice.LaborTotalEgp = dto.LaborTotal;
                appointment.Invoice.PartsTotalEgp = partsTotal;
                appointment.Invoice.TotalAmountEgp = totalAmount;
                appointment.Invoice.IsPaid = dto.IsPaid;
                if (dto.IsPaid && appointment.Invoice.PaidAt == null)
                {
                    appointment.Invoice.PaidAt = DateTime.UtcNow;
                }

                // Replace parts
                appointment.Invoice.Parts.Clear();
                if (dto.Parts != null)
                {
                    foreach (var part in dto.Parts)
                    {
                        appointment.Invoice.Parts.Add(new InvoicePart
                        {
                            InvoiceId = appointment.Invoice.Id,
                            Name = part.Name,
                            Price = part.Price
                        });
                    }
                }

                _unitOfWork.Repository<Invoice>().Update(appointment.Invoice);
            }

            await _unitOfWork.CompleteAsync();
            return true;
        }

        private static string MapStatusToString(AppointmentStatus status)
        {
            return status switch
            {
                AppointmentStatus.Pending => "PENDING",
                AppointmentStatus.WaitingForRepair => "WAITING_FOR_REPAIR",
                AppointmentStatus.UnderRepair => "UNDER_REPAIR",
                AppointmentStatus.ReadyForPickup => "READY_FOR_PICKUP",
                AppointmentStatus.Completed => "COMPLETED",
                AppointmentStatus.Cancelled => "CANCELLED",
                AppointmentStatus.Confirmed => "CONFIRMED",
                _ => status.ToString().ToUpperInvariant()
            };
        }

        private static AppointmentStatus ParseStatus(string status)
        {
            var normalized = status?.Trim().ToUpperInvariant() ?? string.Empty;

            return normalized switch
            {
                "PENDING" => AppointmentStatus.Pending,
                "WAITING_FOR_REPAIR" => AppointmentStatus.WaitingForRepair,
                "UNDER_REPAIR" => AppointmentStatus.UnderRepair,
                "READY_FOR_PICKUP" => AppointmentStatus.ReadyForPickup,
                "COMPLETED" => AppointmentStatus.Completed,
                "CANCELLED" => AppointmentStatus.Cancelled,
                "CONFIRMED" => AppointmentStatus.Confirmed,
                _ => Enum.TryParse<AppointmentStatus>(status, true, out var parsed) 
                    ? parsed 
                    : AppointmentStatus.Pending
            };
        }
    }
}
