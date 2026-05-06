using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Conwy_Cafe_Web_API.Data;
using ConwyCafe.Shared.Models; // Where your Basket class lives

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

            //return await _context.Baskets.Where(b => b.IsActive).ToListAsync();

            // Gets baskets depending on the query
            return await query.ToListAsync();
        }

    }
}
