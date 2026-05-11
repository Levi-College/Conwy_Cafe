namespace ConwyCafe.Shared.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime OrderDate { get; set; } = default(DateTime);
        public decimal TotalAmount { get; set; }

        // Holds the list of all baskets in the order
        public List<OrderBasket> OrderBaskets { get; set; } = new List<OrderBasket>();     
    }
}
