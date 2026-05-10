using Conwy_Cafe_Webpage.Utilities;
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
            _http = factory.CreateClient("CafeAPI");
        }

        public Basket Basket { get; set; }

        // Runs when the page is accessed with a GET request, e.g., /BasketDetails?id=1
        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Fetch the specific basket from your API
            // Make sure your API endpoint supports /api/basket/{id}
            var response = await _http.GetFromJsonAsync<Basket>($"api/basket/{id}");

            if (response == null) return NotFound();

            Basket = response;
            return Page();
        }

        // Called when the form on the page is submitted (POST request)
        public async Task<IActionResult> OnPostAsync(int id, int peopleCount, int quantity)
        {
            // Getting the basket details
            var basket = await _http.GetFromJsonAsync<Basket>($"api/basket/{id}");
            if (basket == null) return NotFound();

            // Get existing cart from Session or create new list (if it doesn't exist) (??)
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            // 3. Calculate Price: Base price + (Extra * (PeopleCount - 1))
            // This handles 1 person (Base) or 2 people (Base + Extra)
            decimal unitPrice = (decimal)basket.Price;
            if (peopleCount > 1)
            {
                unitPrice += (decimal)basket.ExtraPricePerPerson;
            }

            // 4. Create and add the item
            cart.Add(new CartItem
            {
                Id = cart.Count + 1, // Simple ID generation (only untill the session remains)
                BasketId = id,
                Name = basket.Name,
                Quantity = quantity,
                PeopleCount = peopleCount,
                BasePrice = (decimal)basket.Price,
                ExtraPrice = (decimal)basket.ExtraPricePerPerson,
            });

            // Saving the list back to Session as JSON
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            // Redirect to the Cart page to show the updated cart
            return RedirectToPage("/CartPage");
        }
    }
}
