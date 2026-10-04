namespace CarAutomotive.Core.DTOs.AppointmentsDto
{
    public class AppointmentDto
    {
        public int Id { get; set; }
        public Guid MechanicId { get; set; }
        public string MechanicName { get; set; } 
        public string UserName { get; set; }     
        public DateTime AppointmentDate { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; }
    }
}
