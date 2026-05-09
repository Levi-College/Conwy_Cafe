namespace ConwyCafe.Shared.Models
{
    public enum BasketCategory // (0,1,2) for Breakfast, Lunch, Dinner
    {
        Meat,
        Vegetarian,
        ChildrenMeat,
        ChildrenVegetarian,
    }

    public class Basket
    {
        public int Id { get; set; } // Basket ID
        public BasketCategory Category { get; set; } // Navigation property to Category
        public string Name { get; set; } = string.Empty; // Default value to avoid null reference issues
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public decimal? ExtraPricePerPerson { get; set; } // Price for additional person
        public string? ImagePath { get; set; }
        public bool IsActive { get; set; } // Checks if the basket is currentlty active and can be ordered
        public List<BasketItems> BasketItems { get; set; } = new List<BasketItems>(); // Collection of items in the basket
    }
}
