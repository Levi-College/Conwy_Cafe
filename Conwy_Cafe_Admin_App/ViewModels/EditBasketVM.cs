using Conwy_Cafe_Admin_App.Utilities;
using ConwyCafe.Shared.Models;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Input;

namespace Conwy_Cafe_Admin_App.ViewModels
{
    public class EditBasketVM : ViewModelBase
    {
        // Constructor
        public EditBasketVM(Basket SelectedBasket, List<Item> Items)
        {
            // Work on a copy so we can "Cancel" without ruining the main list
            EditBasket = new Basket
            {
                Id = SelectedBasket.Id,
                Name = SelectedBasket.Name,
                Description = SelectedBasket.Description,
                Price = SelectedBasket.Price,
                ExtraPricePerPerson = SelectedBasket.ExtraPricePerPerson,
                ImagePath = SelectedBasket.ImagePath,
                IsActive = SelectedBasket.IsActive,
                Category = SelectedBasket.Category,
                BasketItems = SelectedBasket.BasketItems.ToList()
            };

            OnPropertyChanged(nameof(FullImagePath));

            AllItems = Items;
            LoadData();

            CancelUpdateCommand = new RelayCommand(execute: obj => CancelUpdate(obj as Window));
            SaveUpdateCommand = new RelayCommand(execute: obj => SaveUpdate(obj as Window));
            ChangeImageCommand = new RelayCommand(ChangeImage);
        }

        // Declaring variables and collections
        private Basket _editBasket;

        // Storing the items in separate lists based on their type (main, side, drink) for easier access and display in the UI. The AllItems list contains all the items, while the MainItems, SideItems, and DrinkItems lists contain only the items of their respective types.
        // This categorisation allows for easier filtering and display of items in the UI, such as populating combo boxes or lists with the appropriate items based on their type.
        public List<Item> AllItems { get; set; } = new List<Item>();
        public List<Item> MainItems { get; set; } = new List<Item>();
        public List<Item> SideItems { get; set; } = new List<Item>();
        public List<Item> DrinkItems { get; set; } = new List<Item>();


        // Dictionaries to map enum values to display names for the UI. These dictionaries are used to provide user-friendly names for the BasketCategory and ItemType enums when displaying them in the UI, such as in combo boxes or labels.
        // The keys of the dictionaries are the enum values, and the values are the corresponding display names that will be shown to the user.
        public Dictionary<BasketCategory, string> CategoryDisplayNames { get; } = new()
        {
            { BasketCategory.Meat, "Meat" },
            { BasketCategory.Vegetarian, "Vegetarian" },
            { BasketCategory.ChildrenMeat, "Children Meat" },
            { BasketCategory.ChildrenVegetarian, "Children Vegetarian" }
        };

        public Dictionary<ItemType, string> ItemDisplayNames { get; } = new()
        {
            { ItemType.Main, "Main" },
            { ItemType.Side, "Side" },
            { ItemType.Drink, "Drink" }
        };


        public ICommand CancelUpdateCommand { get; set; }
        public ICommand SaveUpdateCommand { get; set; }
        public ICommand ChangeImageCommand { get; set; }

        // Properties
        public Basket EditBasket
        {
            get { return _editBasket; }
            set
            {
                _editBasket = value;
                OnPropertyChanged(nameof(EditBasket));
            }
        }

        // Creating slots for the 1-3-1 UI layout. The main slot is for the main item, the three side slots are for the side items, and the drink slot is for the drink item. These properties will be used to bind the UI elements to the corresponding items in the basket, allowing for easy display and interaction with the items in the basket.
        public BasketItems MainSlot
        {
            get
            {
                // This goes through the basket items of the basket, checks the item (null check) and checks the item type.
                // If the item type is main, it returns that item as the main slot. If no main item is found, it returns null.
                foreach (var item in EditBasket.BasketItems) { if (item.Item?.ItemType == ItemType.Main) { return item; } }
                return null;
            }
            set
            {
                // This goes through the basket items of the basket, checks the item and type.
                // If the item type is main, it updates that item with the new value. If no main item is found, it adds a new BasketItem with the main item to the basket.
                foreach (var items in EditBasket.BasketItems)
                {
                    if (items.Item.ItemType == ItemType.Main)
                    {
                        items.ItemId = value.ItemId;
                        items.Item = value.Item;
                        break;
                    }
                }
            }
        }

        public BasketItems DrinkSlot
        {
            get
            {
                // This goes through the basket items of the basket, checks the item (null check) and checks the item type.
                // If the item type is drink, it returns that item as the drink slot. If no drink item is found, it returns null.
                foreach (var items in EditBasket.BasketItems) { if (items.Item != null && items.Item.ItemType == ItemType.Drink) { return items; } }
                return null;
            }
            set
            {
                // This goes through the basket items of the basket, checks the item and type.
                // If the item type is drink, it updates that item with the new value. If no drink item is found, it adds a new BasketItem with the drink item to the basket.
                foreach (var item in EditBasket.BasketItems)
                {
                    if (item.Item.ItemType == ItemType.Drink)
                    {
                        item.ItemId = value.ItemId;
                        item.Item = value.Item;
                        break;
                    }
                }
            }
        }

        // Although the logic is similar to the main slots, there are 3 sides and thus a list is used to store the side slots instead of a single variable.
        // The method goes through the basket items of the basket, checks the item (null check) and checks the item type.
        // If the item type is side, it adds that item to a list of side slots. It returns the list of side slots, which may contain 0 to 3 items depending on how many side items are in the basket.
        public IEnumerable<BasketItems> SideSlots
        {
            get
            {
                // This goes through the basket items of the basket, checks the item (null check) and checks the item type.
                // If the item type is side, it adds that item to a list of side slots. It returns the list of side slots, which may contain 0 to 3 items depending on how many side items are in the basket.
                List<BasketItems> sideSlots = new List<BasketItems>();
                foreach (var item in EditBasket.BasketItems) { if (item.Item != null && item.Item.ItemType == ItemType.Side) { sideSlots.Add(item); } }
                return sideSlots;
            }

        }

        // This method is used to retrieve the list of side items from the basket. It goes through the basket items of the basket, checks the item (null check) and checks the item type.
        // IEnumerable means that it returns a collection of items that can be enumerated (e.g., using a foreach loop). It returns a list of side items, which may contain 0 to 3 items depending on how many side items are in the basket.
        public IEnumerable<BasketItems> GetSideSlots()
        {
            List<BasketItems> sideSlots = new List<BasketItems>();
            foreach (var item in EditBasket.BasketItems)
            {
                if (item.Item?.ItemType == ItemType.Side) { sideSlots.Add(item); }
            }
            return sideSlots;
        }

        // These properties are used to bind the three side slots in the UI to the corresponding items in the basket.
        // They use the GetSideSlots method to retrieve the list of side items and return the item at the corresponding index (0 for the first side slot, 1 for the second, and 2 for the third).
        // If there are not enough side items in the basket, it returns null for the remaining slots.
        public BasketItems SideSlot1 => GetSideSlots().ElementAtOrDefault(0);
        public BasketItems SideSlot2 => GetSideSlots().ElementAtOrDefault(1);
        public BasketItems SideSlot3 => GetSideSlots().ElementAtOrDefault(2);

        public string FullImagePath
        {
            get
            {
                // Checks if the imagepath is empty
                if (string.IsNullOrEmpty(EditBasket?.ImagePath)) { return null; }
                // Gets the updated (actual basket image path)
                else { return $"https://localhost:7008/{EditBasket.ImagePath}"; }
            }
        }

        // Methods
        // Simple helper class for the JSON response
        public class UploadResponse { public string NewPath { get; set; } }
        public async void LoadData()
        {
            // Waits for the data of baskets and categories to be loaded before proceeding. This ensures that the necessary data is available for the view model to function properly, such as populating the UI with the retrieved baskets and categories.
            await FilterAllItems();
        }

        // Method to filter all items
        // Loops through all the items in the AllItems list and categorizes them based on their ItemType. It checks the ItemType of each item and adds it to the corresponding list (MainItems, SideItems, or DrinkItems) based on whether it is a Main, Side, or Drink item.
        private async Task FilterAllItems()
        {
            // Clear existing categorized lists to avoid duplicates
            MainItems.Clear();
            SideItems.Clear();
            DrinkItems.Clear();

            // Populate categorized lists based on ItemType (used for the combo boxes in the item selector)
            foreach (var item in AllItems)
            {
                switch (item.ItemType)
                {
                    case ItemType.Main: MainItems.Add(item); break;
                    case ItemType.Side: SideItems.Add(item); break;
                    case ItemType.Drink: DrinkItems.Add(item); break;
                }
            }
        }

        // To save the changes made.
        private async Task SaveUpdate(Window editWindow)
        {
            // Adding the items (id) to a list to check if they are unique
            List<int> itemIds = new List<int>();

            foreach (var basketItem in EditBasket.BasketItems)
            {
                // Checks if it is in the list
                if (itemIds.Contains(basketItem.ItemId))
                { MessageBox.Show("Duplicate Items found"); return; } // stops the method if it is not unique
                // If not add to the basket item ids list
                itemIds.Add(basketItem.ItemId);
            }

            // Make a PUT request to the specified API endpoint to update the basket with the changes made in the EditBasket object. The response from the API is stored in the 'response' variable.
            var response = await App.Http.PutAsJsonAsync($"/api/basket/{EditBasket.Id}", EditBasket);

            if (response.IsSuccessStatusCode) // Checks if the response indicates a successful status code (e.g., 200 OK). If the update was successful, it proceeds to close the edit window.
            {
                if (editWindow != null)
                {
                    editWindow.DialogResult = true; // Set DialogResult to true to indicate success
                    editWindow.Close(); // Close the edit window
                }
            }
            // If the update was not successful, it displays an error message with the reason for the failure.
            else { MessageBox.Show("Failed to save: " + response.ReasonPhrase); }
        }

        // To cancel the changes made and revert to the original state.
        private void CancelUpdate(Window editWindow)
        {
            // Close the window without saving changes. This allows the user to exit the edit mode and discard any modifications made to the basket, effectively reverting to the original state of the basket before any edits were made.
            if (editWindow != null)
            {
                editWindow.DialogResult = false; // Set DialogResult to false to indicate cancellation
                editWindow.Close(); // Close the edit window
            }
        }

        private async void ChangeImage(object? obj)
        {
            await ExecuteChangeImage();
        }

        private async Task ExecuteChangeImage()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.webp",
                Title = "Select New Basket Image"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    using var content = new MultipartFormDataContent();
                    var fileStream = File.OpenRead(openFileDialog.FileName);
                    var fileContent = new StreamContent(fileStream);

                    // "file" name must match the API parameter name
                    content.Add(fileContent, "file", Path.GetFileName(openFileDialog.FileName));

                    // Use PUT to signify an update/replacement
                    var response = await App.Http.PutAsync($"/api/basket/{EditBasket.Id}/update-image", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<UploadResponse>();

                        // Update the model path (e.g., Images/Baskets/basket_5_6385.jpg)
                        EditBasket.ImagePath = result.NewPath;

                        // Refresh the UI preview
                        OnPropertyChanged(nameof(FullImagePath));
                    }
                    else { MessageBox.Show("Failed to upload image to server."); }
                }
                catch (Exception ex) { MessageBox.Show($"Error: {ex.Message}"); }
            }
        }

        


    }
}
