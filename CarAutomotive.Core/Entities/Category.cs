using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarAutomotive.Core.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = null!; 
        public string? Description { get; set; } 
        public ICollection<Product> Products { get; set; } = new HashSet<Product>(); // Navigation property for related Products

    }
}
