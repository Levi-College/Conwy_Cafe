using ConwyCafe.Shared.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Conwy_Cafe_Webpage.Pages
{
    public class IndexModel : PageModel
    {
        // HttpClient to call the API
        private readonly HttpClient _http;

        public IndexModel(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("CafeAPI");
        }

        // Holds the list (to loop through in the Razor page)
        // The model used by the cshtml page is the Baskets.
        public List<Basket> Baskets { get; set; } = new List<Basket>();

        public async Task OnGetAsync()
        {
            try
            {
                // Call the API to get the baskets
                var response = await _http.GetFromJsonAsync<List<Basket>>("api/basket");

                if (response != null) { Baskets = response; }

            }
            catch (Exception ex) { ex.Message.ToString(); }
        }
    }
}
