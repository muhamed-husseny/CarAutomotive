namespace CarAutomotive.Core.Entities.Mechanic
{
    public class MechanicService
    {
        public int Id { get; set; }

        public string ServiceName { get; set; }
        public string Description { get; set; } 

        public decimal EstimatedPrice { get; set; } 

      
        public Guid MechanicProfileId { get; set; }
        public MechanicProfile MechanicProfile { get; set; }
    }
}
