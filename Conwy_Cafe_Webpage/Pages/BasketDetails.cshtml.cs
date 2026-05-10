using ConwyCafe.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Conwy_Cafe_Webpage.Pages
{
    //public class BasketDetailsModel : PageModel
    //{
    //    public void OnGet()
    //    {
    //    }
    //}

    public class BasketDetailsModel : PageModel
    {
        private readonly HttpClient _http;
        public BasketDetailsModel(IHttpClientFactory factory) => _http = factory.CreateClient("ConwyApi");

        public Basket Basket { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Fetch the specific basket from your API
            // Make sure your API endpoint supports /api/basket/{id}
            var response = await _http.GetFromJsonAsync<Basket>($"/api/basket/{id}");

            if (response == null) return NotFound();

            Basket = response;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            // We will build the "Add to Cart" logic next!
            return RedirectToPage("/Cart");
        }
    }
}
