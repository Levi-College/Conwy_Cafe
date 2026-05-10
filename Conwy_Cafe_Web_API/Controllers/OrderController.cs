using Conwy_Cafe_Web_API.Data;
using ConwyCafe.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Conwy_Cafe_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        // Constructor to inject the database context (setting the context)
        public OrderController(AppDbContext context) { _context = context; }

        // Used to receive the order information from the webpage when the user clicks "Place Order".
        // The webpage will send a CheckoutModel object containing the customer information and the list of cart items (baskets) in the order.
        public class CheckoutModel
        {
            public string CustomerName { get; set; }
            public string CustomerEmail { get; set; }
            public string CustomerPhone { get; set; }
            public decimal TotalPrice { get; set; }
            public DateTime OrderDate { get { return DateTime.Today; } }

            public List<CartItem> CartItems { get; set; }
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> CreateOrder([FromBody] CheckoutModel request)
        {
            // 1. Create the Main Order record
            var order = new Order
            {
                OrderDate = request.OrderDate,

                // Calculating the total amount by summing the total price of each cart item (which is calculated as BasePrice + ExtraPrice * PeopleCount, multiplied by Quantity)
                TotalAmount = request.TotalPrice, // Calculated when place order is clicked in the carts page
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var b in request.CartItems)
            {
                // 2. Create the OrderBasket (The middle link)
                var orderBasket = new OrderBaskets
                {
                    OrderId = order.Id,
                    BasketId = b.BasketId,
                    BasketName = b.Name,
                    BasketPrice = b.BasePrice,
                    Quantity = b.Quantity, // This is the number of times the user wants to order this basket (e.g. 2x Family Feast)

                    NumberOfPeople = b.PeopleCount // This is the total number of people
                };

                // Adding the OrderBasket to the table
                _context.OrderBaskets.Add(orderBasket);
                await _context.SaveChangesAsync();

                // Fetch the original Basket to find which items belong in it
                var basketTemplate = await _context.Baskets
                    .Include(x => x.BasketItems) // Gets all the items related to the baskets 
                    .ThenInclude(bi => bi.Item) // Gets the Item properties
                    .FirstOrDefaultAsync(x => x.Id == b.BasketId); // WHERE ID = BasketID

                if (basketTemplate != null)
                {
                    // Getting the items in the basket items list
                    foreach (var bi in basketTemplate.BasketItems)
                    {
                        // Logic: 1-3-1 rule. Sides (3), Main/Drink (1).
                        // A variable is used to set the number of items (if side qty = 3 else 1)
                        //int qty = 0;
                        //if (bi.Item?.ItemType != ItemType.Side) { qty = 1; }
                        //else { qty = 3; }
                        //int baseQtyPerPerson = bi.Item.ItemType == ItemType.Side ? 3 : 1;

                        var orderItem = new OrderItems
                        {
                            OrderBasketId = orderBasket.Id,
                            ItemId = bi.ItemId,
                            // Quantity is based on the number of people in the order. Already calculated when the order is sent from the website
                            Quantity = b.PeopleCount
                        };
                        
                        _context.OrderItems.Add(orderItem);
                    }
                }

            }

            await _context.SaveChangesAsync();

            // Return the ID so the webpage can show "Order #1234 Placed!"
            return Ok(order.Id.ToString());

        }
    }
}
