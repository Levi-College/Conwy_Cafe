namespace ConwyCafe.Shared.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int ItemId { get; set; } // Foreign key to Item
        public int OrderBasketId { get; set; } // Foreign key to OrderBaskets
        public string ItemName { get; set; } // Name of the item at the time of order

        // Quantity of the item in the order. Based on the number of people from the OrderBaskets table,
        // for example if there are 6 people in the basket, and the item is "Bread", the quantity of "Bread" in the OrderItems table will be 6 (1 bread per person).
        public int Quantity { get; set; } 
    }
}
