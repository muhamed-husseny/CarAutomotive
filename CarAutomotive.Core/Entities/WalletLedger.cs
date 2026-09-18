using CarAutomotive.Core.Entities.Identity;
using CarAutomotive.Core.Enums;

namespace CarAutomotive.Core.Entities
{
    public class WalletLedger : BaseEntity<Guid>
    {
        public Guid UserId { get; set; }
        public AppUser User { get; set; } = null!;

        public string ReferenceType { get; set; } = null!; // 'ORDER' or 'APPOINTMENT'
        public Guid ReferenceId { get; set; }

        public TransactionType Type { get; set; }
        public decimal AmountEgp { get; set; }
        public TransactionStatus Status { get; set; } = TransactionStatus.Completed;

        public string Description { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
