using Conwy_Cafe_Web_API.Data;
using ConwyCafe.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Conwy_Cafe_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {

        private readonly AppDbContext _context;

        // Constructor to inject the database context (setting the context)
        public ItemsController(AppDbContext context) { _context = context; }

        // Getting the baskets (api/baskets)
        // A parameter is used to differentiate between active and all baskets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Item>>> GetItems()
        {
            var response = await _context.Items.Where(i => i.IsActive).ToListAsync(); // Only getting active items
            return Ok(response);
        }

        // Adding a new item (api/items)
        [HttpPost]
        public async Task<ActionResult<Item>> AddItem(Item item)
        {
            _context.Items.Add(item); // Adds the new item to the database context.
            await _context.SaveChangesAsync(); // Saves the changes to the database asynchronously.
            return Ok(item); // Returns an HTTP 200 OK response with the added item in the response body.
        }

        [HttpPut("{id}")]
        // Updating an existing item (api/items/{id})
        public async Task<ActionResult<Item>> UpdateItem(Item updatedItem)
        {
            var existingItem = await _context.Items.FindAsync(updatedItem.Id); // Finds the existing item in the database using the provided ID.
            if (existingItem == null) { return NotFound(); } // If the item is not found, returns an HTTP 404 Not Found response.

            // Updates the properties of the existing item with the values from the updated item.
            // Try catch used in case of any issues
            try
            {
                existingItem.Name = updatedItem.Name;
                existingItem.Description = updatedItem.Description;
                existingItem.ItemType = updatedItem.ItemType;
                await _context.SaveChangesAsync(); // Saves the changes to the database asynchronously.
                return Ok(existingItem); // Returns an HTTP 200 OK response with the updated item in the response body.

            }
            catch (Exception ex) { return BadRequest(ex.Message); } // If an exception occurs during the update process, returns an HTTP 400 Bad Request response with the exception message in the response body.
        }

        // To deactivate an item (api/items/{id}/deactivate)
        [HttpPut("delete/{id}")]
        public async Task<ActionResult<Item>> DeactivateItem(int id)
        {
            var existingItem = await _context.Items.FindAsync(id); // Finds the existing item in the database using the provided ID.
            if (existingItem == null) { return NotFound(); } // If the item is not found, returns an HTTP 404 Not Found response.
            try
            {
                existingItem.IsActive = false; // Sets the IsActive property of the existing item to false, effectively deactivating it.
                await _context.SaveChangesAsync(); // Saves the changes to the database asynchronously.
                return Ok(existingItem); // Returns an HTTP 200 OK response with the deactivated item in the response body.
            }
            catch (Exception ex) { return BadRequest(ex.Message); } // If an exception occurs during the deactivation process, returns an HTTP 400 Bad Request response with the exception message in the response body.
        }
    }
}
