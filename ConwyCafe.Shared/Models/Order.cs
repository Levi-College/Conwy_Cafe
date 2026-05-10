namespace ConwyCafe.Shared.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = default(DateTime);
        public decimal TotalAmount { get; set; }
    }
}
