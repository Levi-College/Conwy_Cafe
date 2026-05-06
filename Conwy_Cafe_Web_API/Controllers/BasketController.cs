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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Basket>>> GetBaskets()
        {
            return await _context.Baskets.Where(b => b.IsActive).ToListAsync();
        }
    }
}
