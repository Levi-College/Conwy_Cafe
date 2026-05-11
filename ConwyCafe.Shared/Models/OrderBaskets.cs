using System.Buffers.Text;
using System.Reflection.Metadata;

namespace ConwyCafe.Shared.Models
{
    public class OrderBasket
    {
        public int Id { get; set; }
        public int OrderId { get; set; } // Foreign key to Order
        public int BasketId { get; set; } // Foreign key to Basket
        public string BasketName { get; set; } // Name of the basket at the time of order
        public decimal BasketPrice { get; set; } // Price of the basket at the time of order
       
        public int Quantity { get; set; } // Quantity of this basket in the order

        /// <summary>
        /// This is the total number of people for the basket. This number will be used in the OrderItems table to calculate the quanitity of each item in the basket
        /// The value is automatically calculated when the order is placed.
        /// For example, the cutomer places an order with 2 "Meat" baskets, each with 3 people (each). The Quantity for the "Meat" basket in the OrderBaskets table will be 2, and the NumberOfPeople will be 6 (2 baskets * 3 people each). 
        /// This allows us to calculate the total quantity of each item in the basket based on the number of people, which is important for inventory management and preparation.
        /// </summary>
        public int NumberOfPeople { get; set; }

        // Used to hold the order basket items for this order basket
        // This is not stored in the database, it is used to transfer the data from the webpage to the backend when the order is placed, and to transfer the data from the backend to the webpage when the order details are retrieved.
        // It is populated based on the BasketId when the order is placed, and it is used to calculate the quantity of each item in the OrderItems table based on the NumberOfPeople.
        public List<OrderItem> OrderItems { get; set; }

    }
}
