using Conwy_Cafe_Webpage.Utilities;
using ConwyCafe.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Conwy_Cafe_Webpage.Pages
{
    public class CartPageModel : PageModel
    {
        private readonly HttpClient _http;
        public List<CartItem> CartItems { get; set; } = new();
        public decimal CartTotal => CartItems.Sum(x => x.TotalPrice);

        public CartPageModel(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("CafeAPI");
        }
        // This is used to send the information of the entire order to the API when place order is pressed.
        // The API will then unpack the order information and save it to the database.
        public class CheckoutModel
        {
            public string CustomerName { get; set; }
            public string CustomerEmail { get; set; }
            public string CustomerPhone { get; set; }
            public DateTime OrderDate {
                get { 
                    return DateTime.Today; 
                }
                set { } 
            }
            public decimal TotalPrice { get; set; } 
            public List<CartItem> CartItems { get; set; }
        }
        public void OnGet()
        {
            CartItems = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new();
        }

        // This method is called when the user clicks the "Checkout" button for an item in the cart. It removes the item from the cart and updates the session.
        public async Task<IActionResult> OnPostCheckoutAsync(string custName, string custEmail, string custPhone)
        {
            // Placing the order logic here (e.g., saving the order to the database, calling an API, etc.)

            // Getting the cart items from the session
            var cartItems = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new();

            // Checking if the cart is empty
            if (cartItems.Count == 0)
            {
                // If the cart is empty, redirect back to the cart page with a message or simply redirect
                return RedirectToPage("/CartPage");
            }

            // Placing the order
            // 1. Create an order object and populate it with the cart items and other necessary details (e.g., customer info, order date, etc.)
            CheckoutModel model = new CheckoutModel()
            {
                CustomerName = "Trial",
                CustomerEmail = "Trial",
                CustomerPhone = "Trial",
                TotalPrice = cartItems.Sum(x => x.TotalPrice),
                CartItems = cartItems
            };

            // 2. Save the order to the database or call an API to process the order
            var result = await _http.PostAsJsonAsync("api/Order/checkout", model); // Assuming you have an API endpoint to handle order placement

            if (!result.IsSuccessStatusCode)
            {
                var errorMessage = await result.Content.ReadAsStringAsync(); // Read the error message from the API response
                Console.WriteLine(errorMessage);
                // Handle the error (e.g., log it, show an error message to the user, etc.)
                // For simplicity, we will just redirect back to the cart page with an error message
                TempData["ErrorMessage"] = "There was an issue placing your order. Please try again.";
                return RedirectToPage("/CartPage");
            }
            //result.EnsureSuccessStatusCode(); // Ensure the API call was successful, you can also handle errors here if needed
            


            // 3. After successfully placing the order, clear the cart
            cartItems.Clear(); // Clear the cart items
            HttpContext.Session.SetObjectAsJson("Cart", cartItems); // Update the session with the empty cart
            return RedirectToPage("/CartPage");
        }

        // This method is called when the user clicks the "Remove" button for an item in the cart. It removes the item from the cart and updates the session.
        public async Task<IActionResult> OnPostRemoveAsync(int itemId)
        {
            // Get the current cart from the session, or create a new list if it doesn't exist
            var cartItems = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new();
            var cartItemToRemove = cartItems.FirstOrDefault(x => x.Id == (itemId + 1)); // Getting x where x.Id == itemID passed as the parameter
            if (cartItemToRemove != null)
            {
                cartItems.Remove(cartItemToRemove); // Remove the item from the cart
                //Update the session with the new cart
                HttpContext.Session.SetObjectAsJson("Cart", cartItems);
            }
            // Redirect back to the CartPage to show the updated cart
            return RedirectToPage("/CartPage");
        }
    }
}
