namespace CarAutomotive.Core.Entities
{
    public class Merchants : BaseEntity
    {
        public int Id { get; set; }
        public string ShopName { get; set; } 
        public string CommercialRegister { get; set; } 
        public string Status { get; set; } = "PENDING VETTING"; 
        public Guid AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
