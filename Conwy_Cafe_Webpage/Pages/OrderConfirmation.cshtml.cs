using ConwyCafe.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Conwy_Cafe_Webpage.Pages
{
    public class OrderConfirmationModel : PageModel
    {
        
        private readonly HttpClient _http;

        public Order Order { get; set; }

        public OrderConfirmationModel(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("CafeAPI");
        }

        public async Task OnGetAsync(int id)
        {
            Order = await _http.GetFromJsonAsync<Order>($"api/Order/{id}");
        }
    }

    
}
