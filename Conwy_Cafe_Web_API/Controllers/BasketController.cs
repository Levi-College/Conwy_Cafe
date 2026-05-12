using Conwy_Cafe_Web_API.Data;
using ConwyCafe.Shared.Models; // Where your Basket class lives
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Conwy_Cafe_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly AppDbContext _context;

        // Constructor to inject the database context (setting the context)
        public BasketController(AppDbContext context) { _context = context; }

        // Getting the baskets (api/baskets)
        // A parameter is used to differentiate between active and all baskets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Basket>>> GetBaskets([FromQuery] bool showAll = false)
        {
            // Setting up the query 
            var query = _context.Baskets
                .Include(b => b.BasketItems) // Include related BasketItems (joining the baskets and the baskets items)
                .ThenInclude((bi => bi.Item)) // Include the related Item for each BasketItem (joining the baskets items and the items)
                .AsQueryable();

            if (!showAll)
            {
                // If showAll is false, filter to only active baskets
                query = query.Where(b => b.IsActive);
            }

            // Gets baskets depending on the query
            return await query.ToListAsync();
        }

        // To get the details of an individual basket, the id of the basket is sent (api/baskets/1)
        [HttpGet("{id}")]
        public async Task<ActionResult<Basket>> GetBasket(int id)
        {
            // Getting the basket with the given id from the database, including its related BasketItems and their related Items to get all the details
            var basket = await _context.Baskets
                .Include(b => b.BasketItems) // Include related BasketItems
                .ThenInclude(bi => bi.Item) // Include the related Item for each BasketItem
                .FirstOrDefaultAsync(b => b.Id == id); // Get the first basket that matches the given id or return null if no match is found
            if (basket == null) { return NotFound("Basket not found."); } // Check if the basket exists, if it does not exist, return a not found error
            return basket; // Return the basket details
        }

        //Updating/Saving a basket of which the id is sent
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBasket(int id, [FromBody] Basket updatedBasket)
        {
            //return null;
            // Check if the id in the URL matches the id of the basket being updated
            if (id != updatedBasket.Id)
            {
                return BadRequest("Basket ID mismatch.");
            }

            // Getting the basket with the given id from the database, including its related BasketItems to update the details
            // Context is used to access the database, Baskets is the table, Include is used to include the related BasketItems, FirstOrDefaultAsync is used to get the first basket that matches the given id or return null if no match is found
            var existingBasket = await _context.Baskets
                .Include(b => b.BasketItems) // Include related BasketItems
                .FirstOrDefaultAsync(b => b.Id == id);

            // Checks if the basket exists, if it does not exist, return a not found error
            if (existingBasket == null) { return NotFound("Basket not found."); }

            // Update the existing basket's properties with the values from the updated basket
            // This only updates the properties of the existing basket, it does not update the related BasketItems, you would need to handle that separately if needed
            _context.Entry(existingBasket).CurrentValues.SetValues(updatedBasket);

            //Updating the basket items table (b)
            _context.BasketItems.RemoveRange(existingBasket.BasketItems); // Remove existing BasketItems

            //Add the new BasketItems from the updated basket
            foreach (var item in updatedBasket.BasketItems)
            {
                // Adding the basket item using the model's properties, this is done to ensure that the BasketId is set correctly for each BasketItem, and to avoid any issues with tracking entities in the context
                existingBasket.BasketItems.Add(new BasketItems
                {
                    BasketId = existingBasket.Id, // Set the BasketId for the new BasketItem
                    ItemId = item.ItemId,
                    Quantity = 1, // Default quantity to 1
                    IsDeleted = item.IsDeleted
                }); // Add the new BasketItem to the existing Basket's BasketItems collection
            }

            try
            {
                await _context.SaveChangesAsync(); // Save the changes to the database
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the basket: {ex.Message}"); // Return a 500 Internal Server Error with the exception message if an error occurs
            }
            //return BadRequest("Please retry");
            return Ok();
        }


        // Updating the image of a basket, the id of the basket and the file are sent
        [HttpPut("{id}/update-image")]
        public async Task<IActionResult> UpdateImage(int id, IFormFile file)
        {
            var basket = await _context.Baskets.FindAsync(id); // Find the basket with the given id in the database
            if (basket == null || file == null) return NotFound(); // Null check

            // Setup Folders (wwwroot/ Images/Baskets). Gets the current directory of the application, combines it with "wwwroot" to get the path to the wwwroot folder, then combines that with "images/baskets" to get the path to the folder where the basket images will be stored (e.g., C:\Projects\ConwyCafe\wwwroot\images\baskets)
            string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string folderPath = Path.Combine(rootPath, "images", "baskets");

            // If it does not exist, create one (in the directory)
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            // Capture Old Path to delete later (stored in the database)
            string oldRelativePath = basket.ImagePath;

            // .Ticks is used to create a unqique name for the file, this is because if two files have the same name, they will overwrite each other, so using the current time in ticks ensures that each file has a unique name
            string extension = Path.GetExtension(file.FileName); // Get the file extension from the original file name
            string newFileName = $"basket_{id}_{DateTime.Now.Ticks}{extension}"; // A default value for the image
            string newRelativePath = $"images/baskets/{newFileName}"; // The relative path to be stored in the database, this is the path that will be used to access the image from the web application (e.g., /images/baskets/basket_1_637654321012345678.jpg)
            string newFullPath = Path.Combine(rootPath, newRelativePath); // The full path to save the file on the server, this is the actual location on the server's file system where the image will be stored (e.g., C:\Projects\ConwyCafe\wwwroot\images\baskets\basket_1_637654321012345678.jpg)

            // Save New File
            //
            using (var stream = new FileStream(newFullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream); // Copy the uploaded file to the new location on the server (newFullPath) (async used to avoid blocking the thread while the file is being saved)
            }

            // Changing the value in the database and saving it
            basket.ImagePath = newRelativePath;
            await _context.SaveChangesAsync();

            // Deleting the old file if it exists (if there is an old image, delete it from the directory)
            if (!string.IsNullOrEmpty(oldRelativePath))
            {
                string oldFullPath = Path.Combine(rootPath, oldRelativePath.TrimStart('/'));
                if (System.IO.File.Exists(oldFullPath))
                {
                    try { System.IO.File.Delete(oldFullPath); }
                    catch (Exception ex) { Console.WriteLine($"Failed to delete old image: {ex.Message}"); } 
                }
            }

            return Ok(new { NewPath = newRelativePath });
        }

    }
}
