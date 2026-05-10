using Conwy_Cafe_Webpage.Utilities;
using ConwyCafe.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Conwy_Cafe_Webpage.Pages
{
    public class CartPageModel : PageModel
    {
        public List<CartItem> CartItems { get; set; } = new();
        public decimal CartTotal => CartItems.Sum(x => x.TotalPrice);

        public void OnGet()
        {
            CartItems = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new();
        }

        // This method is called when the user clicks the "Checkout" button for an item in the cart. It removes the item from the cart and updates the session.
        public async Task<IActionResult> OnPostCheckoutAsync(int itemId)
        {
            var cartItems = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new();
            var itemToRemove = cartItems.FirstOrDefault(x => x.Id == itemId);
            if (itemToRemove != null)
            {
                cartItems.Remove(itemToRemove);
                HttpContext.Session.SetObjectAsJson("Cart", cartItems);
            }
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
