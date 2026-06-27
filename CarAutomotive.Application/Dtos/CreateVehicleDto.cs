using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarAutomotive.Application.Dtos
{
    public class CreateVehicleDto
    {
        public string Make { get; set; } = null!;
        public string Model { get; set; } = null!;
        public int Year { get; set; }
        public string PlateCode { get; set; } = null!;
        public string PlateNumber { get; set; } = null!;
        public int Mileage { get; set; }
        public string? ImageUrl { get; set; }
    }
}
