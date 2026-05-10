namespace ConwyCafe.Shared.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedDate { get; set; } = default(DateTime);
        public decimal TotalAmount { get; set; }
    }
}
