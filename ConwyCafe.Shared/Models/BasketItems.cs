using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConwyCafe.Shared.Models
{
    public class BasketItems
    {
        public int BasketId { get; set; } // Basket ID (foreign key to Basket)
        public int ItemId { get; set; } // Item ID (foreign key to Item) (composite key with BasketId)
        public int Quantity { get; set; } // Quantity of the item in the basket
        public bool IsDeleted { get; set; } // Checks if the basket is currentlty active and can be ordered

        public Item Item { get; set; } // Navigation property to Item
    }
}
