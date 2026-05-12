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

        // Getting one order by ID (api/order/{id})
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderBaskets) // Include related OrderBaskets (joining the orders and the order baskets)
                .ThenInclude(ob => ob.OrderItems) // Include the related OrderItems for each OrderBasket (joining the order baskets and the order items)
                .ThenInclude(oi => oi.Item) // Include the related Item for each OrderItem (joining the order items and the items)
                .FirstOrDefaultAsync(o => o.Id == id); // WHERE ID = id
            if (order == null) { return NotFound(); }
            return Ok(order);
        }

        // HTTP get method to retrieve all orders from the database.Used by the admin app to show the list of orders.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderBaskets) // Include related OrderBaskets (joining the orders and the order baskets)
                .ThenInclude(ob => ob.OrderItems) // Include the related OrderItems for each OrderBasket (joining the order baskets and the order items)
                .ThenInclude(oi => oi.Item)
                .ToListAsync();
            return Ok(orders);
        }

        // This is the HTTPPost method that the webpage will call to create the order in the database. It receives a CheckoutModel object in the request body.
        [HttpPost("checkout")]
        public async Task<IActionResult> CreateOrder([FromBody] CheckoutModel request)
        {
            // 1. Create the Main Order record
            var order = new Order
            {
                OrderDate = request.OrderDate,
                CustomerName = request.CustomerName,
                CustomerEmail = request.CustomerEmail,
                PhoneNumber = request.CustomerPhone,
                // Calculating the total amount by summing the total price of each cart item (which is calculated as BasePrice + ExtraPrice * PeopleCount, multiplied by Quantity)
                TotalAmount = request.TotalPrice, // Calculated when place order is clicked in the carts page
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var b in request.CartItems)
            {
                // 2. Create the OrderBasket (The middle link)
                var orderBasket = new OrderBasket
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
                        var orderItem = new OrderItem
                        {

                            OrderBasketId = orderBasket.Id,
                            ItemId = bi.ItemId,
                            ItemName = bi.Item.Name,
                            // Quantity is based on the number of people in the order and the number of baskets ordered.
                            // For example, if the user orders 2x Family Feast for 4 people, then the quantity of each item in the Family Feast will be 8 (4 people x 2 baskets).
                            Quantity = b.PeopleCount * b.Quantity
                        };

                        _context.OrderItems.Add(orderItem);
                    }
                }
            }

            await _context.SaveChangesAsync();

            // Return the ID so the webpage can show "Order #1234 Placed!"
            return Ok(order.Id.ToString());

        }

        // Archive an order (api/order/archive/{id})
        [HttpPut("archive/{id}")]
        public async Task<IActionResult> ArchiveOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) { return NotFound(); }
            order.Archived = true;
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
