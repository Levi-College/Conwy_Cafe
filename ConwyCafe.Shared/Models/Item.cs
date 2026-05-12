namespace ConwyCafe.Shared.Models
{
    public enum ItemType // (0,1,2) for Main, Side, Drink
    {
        Main,
        Side,
        Drink
    }
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ItemType ItemType { get; set; }
        //public decimal BasePrice { get; set; }

        public bool IsActive { get; set; } // Checks if item is active or not
    }
}
