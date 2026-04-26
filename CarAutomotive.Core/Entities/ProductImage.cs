using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarAutomotive.Core.Entities
{
    public class ProductImage:BaseEntity
    {
        public string ImageUrl { get; set; } = null!; 
        public int ProductId { get; set; } // Fk TO Product
        public Product Product { get; set; } = null!; // Navigation property for related Product
    }
}
