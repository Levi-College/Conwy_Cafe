using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConwyCafe.Shared.Models
{
    public class Basket
    {
        public int Id { get; set; } // Basket ID
        public int CategoryId { get; set; } // Foreign key to Category
        public string Name { get; set; } = string.Empty; // Default value to avoid null reference issues
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public decimal? ExtraPricePerPerson { get; set; } // Price for additional person
        public string? ImagePath { get; set; }
        public bool IsActive { get; set; } // Checks if the basket is currentlty active and can be ordered
    }
}
