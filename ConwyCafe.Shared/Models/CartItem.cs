namespace ConwyCafe.Shared.Models
{
    public class CartItem
    {
        public int Id { get; set; } // Just to idnetify the cart item, not related to the database
        public int BasketId { get; set; }
        public string Name { get; set; }
        public int PeopleCount { get; set; } // 1 or 2
        public int Quantity { get; set; }    // Number of baskets
        public decimal BasePrice { get; set; } // Holds the base price of the basket
        public decimal ExtraPrice { get; set; } // Holds the cost of extra person

        // Base price covers the cost of 2 people, if there are more than 2 people, we add the extra price for each additional person
        public decimal ItemPrice // Cart Item Price 
        {
            get
            {
                if (PeopleCount <= 2)
                {
                    return BasePrice;
                }
                else
                {
                    return BasePrice + ExtraPrice * (PeopleCount - 2);
                }
            }
        }

        // The total price for this cart item is the price of one item multiplied by the quantity
        // The price of one (item basket in cart) is calculated based on the number of people (base price + extra price for additional people)
        public decimal TotalPrice { 
            get { return ItemPrice * Quantity; } 
        }


    }
}
