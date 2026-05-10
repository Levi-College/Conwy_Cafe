using ConwyCafe.Shared.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Conwy_Cafe_Webpage.Pages
{
    public class IndexModel : PageModel
    {
        // Used for logging (optional, but good for debugging)
        private readonly ILogger<IndexModel> _logger;
        private readonly HttpClient _http;

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory factory)
        {
            _http = factory.CreateClient("CafeAPI");
            _logger = logger;
        }

        // Holds the list (to loop through in the Razor page)
        public List<Basket> Baskets { get; set; } = new List<Basket>();

        public async Task OnGetAsync()
        {
            try
            {
                // Call the API to get the baskets
                var response = await _http.GetFromJsonAsync<List<Basket>>("api/basket");


                if (response != null)
                {
                    Baskets = response;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching baskets from API");
                // Optionally, you could set an error message to display on the page
            }
        }
    }
}
