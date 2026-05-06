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
            var response = await _context.Items.ToListAsync();
            return Ok(response);
        }
    }
}
