using ConwyCafe.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Conwy_Cafe_Webpage.Pages
{
    public class BasketDetailsModel : PageModel
    {
        // This HttpClient will be used to call the API to get the basket details
        private readonly HttpClient _http;

        public BasketDetailsModel(IHttpClientFactory factory)
        {
            _http = factory.CreateClient();
        }

        public Basket Basket { get; set; }

        // Runs when the page is accessed with a GET request, e.g., /BasketDetails?id=1
        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Fetch the specific basket from your API
            // Make sure your API endpoint supports /api/basket/{id}
            var response = await _http.GetFromJsonAsync<Basket>($"https://localhost:7008/api/basket/{id}");

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
