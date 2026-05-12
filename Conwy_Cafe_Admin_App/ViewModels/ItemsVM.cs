using Conwy_Cafe_Admin_App.Utilities;
using ConwyCafe.Shared.Models;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Input;

namespace Conwy_Cafe_Admin_App.ViewModels
{
    public class ItemsVM : ViewModelBase
    {
        public ItemsVM()
        {
            LoadData();
            RefreshItemsCommand = new RelayCommand(RefreshPage);
            NewItemCommand = new RelayCommand(NewItem);

            // Add item command checks if all the details are entered before allowing the user to add the item, if any of the required details (name, item type, description) are missing, it displays a message box prompting the user to enter all the details and prevents the addition of the item until all required information is provided.
            AddItemCommand = new RelayCommand(async (obj) =>
            {
            // Setting up the item
            if (SelectedItem == null || SelectedItem.Name == null || SelectedItem.ItemType == null || SelectedItem.Description == null) { MessageBox.Show("Please enter all the details."); return; }
                await AddItem(SelectedItem);

            });

            // Asyn (obj) is used to allow the command to execute an asynchronous method (UpdateItem) when the command is triggered. The lambda expression (async (obj) => { ... }) defines an asynchronous anonymous function that takes an object parameter (obj) and contains the logic to check if an item is selected and then calls the UpdateItem method with the selected item as an argument.
            UpdateItemCommand = new RelayCommand(async (obj) =>
            {
                if (SelectedItem == null) { MessageBox.Show("Please select an item to update."); return; }
                await UpdateItem(SelectedItem);
            });

            DeleteItemCommand = new RelayCommand(async (obj) =>
            {
                if (SelectedItem == null) { MessageBox.Show("Please select an item to delete."); return; }
                var result = MessageBox.Show($"Are you sure you want to delete the item '{SelectedItem.Name}'?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    await DeleteItem(SelectedItem.Id);
                }
            });
        }



        // Declaring variables
        private Item _selectedItem;
        private string _selectedItemType = "All"; // Default value for the item type filter
        private bool _enableElementsForNewItem = false;
        private bool _disableElementsForNewItem = false;

        public ICommand RefreshItemsCommand { get; }
        public ICommand NewItemCommand { get; }
        public ICommand AddItemCommand { get; }
        public ICommand UpdateItemCommand { get; }
        public ICommand DeleteItemCommand { get; }
        //public List<Item> AllItems { get; set; } = new List<Item>();
        public ObservableCollection<Item> AllItems { get; set; } = new ObservableCollection<Item>();

        // The Mapping Dictionary (for the item type
        public Dictionary<ItemType, string> ItemDisplayNames { get; } = new()
        {
            { ItemType.Main, "Main" },
            { ItemType.Side, "Side" },
            { ItemType.Drink, "Drink" },
        };

        public List<ItemType> ItemTypes => ItemDisplayNames.Keys.ToList();

        public List<string> ItemsFilterList { get; } = new List<string> { "All", "Main", "Side", "Drink" };



        // Properties

        // This property represents the currently selected item in the UI. When the value of this property changes, it raises the PropertyChanged event to notify the UI that it needs to update any bindings that are bound to this property.
        public Item SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
                EnableElementsForNewItem = false;

                if (value != null)
                {
                    DisableElementsForNewItem = true;
                }
                else { DisableElementsForNewItem = false; }
            }
        }

        public string SelectedItemType
        {
            get { return _selectedItemType; }
            set
            {
                if (_selectedItemType != value) // Check if the new value is different from the current value to avoid unnecessary updates and filtering when the same item type is selected again.
                {
                    _selectedItemType = value;
                    OnPropertyChanged(nameof(SelectedItemType));
                    FilterItemsByType(value); // Call the method to filter items based on the newly selected item type.
                }
            }
        }

        // Used to enable and disable the update and add buttons for items
        public bool EnableElementsForNewItem
        {
            get => _enableElementsForNewItem;
            set { _enableElementsForNewItem = value; OnPropertyChanged(); }
        }

        public bool DisableElementsForNewItem
        {
            get => _disableElementsForNewItem;
            set { _disableElementsForNewItem = value; OnPropertyChanged(); }
        }



        // Methods
        public async void LoadData()
        {
            // Getting all the items from the API
            await GetAllItems();
        }


        // This method would contain logic to refresh the page, such as re-fetching data from the data source and updating the relevant properties or collections in the view model. 
        public async void RefreshPage(object? obj)
        {
            // 1. Clearing the list and selected item
            AllItems.Clear();
            SelectedItem = null;
            SelectedItemType = "All"; // Reset the item type filter to "All" when refreshing the page

            // 2. Getting all the details
            await GetAllItems();
        }

        private void NewItem(object? obj)
        {
            // Setting a blueprint for the new item
            SelectedItem = new Item
            {
                Name = "Add Name",
                Description = "Add description",
                ItemType = ItemType.Main, // Default to Main, can be changed by the user
                IsActive = true // Set the new item as active by default
            };

            // Enabling the add button
            EnableElementsForNewItem = true;
            DisableElementsForNewItem = false;
        }

        // Getting all the items
        public async Task GetAllItems()
        {
            try
            {
                // A GET request to the API to get list of items and stores it in response (can loop through)
                var response = await App.Http.GetFromJsonAsync<List<Item>>("/api/items");
                if (response != null)
                {
                    AllItems.Clear(); // Clear the existing items in the observable collection before adding new ones to avoid duplicates.
                    foreach (var item in response) // Iterate through each item in the response (using .Result to get the result of the asynchronous operation).
                    {
                        AllItems.Add(item); // Add each item to the 'AllItems' observable collection.
                    }
                }

            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

        }

        public void OpenEditItemWindow(object? obj)
        {
            if (SelectedItem == null) { MessageBox.Show("Please select a basket to edit."); return; }
            //Setting the datacontext
            //var EditBasketVM = new EditBasketVM(SelectedBasket, AllItems);
            //// This method would contain logic to open a new window for editing a basket. 
            //EditBasketWindow editWindow = new EditBasketWindow();
            //editWindow.DataContext = EditBasketVM;
            //editWindow.ShowDialog();
        }

        public async Task FilterItemsByType(string itemType)
        {
            // If "All" is selected, show all items
            if (itemType == "All") { LoadData(); }
            else
            {
                // Getting the updated list of items from the database (all items)
                await GetAllItems();

                // Using switch statement to filter items based on the selected item type. The Enum.TryParse method is used to convert the string representation of the item type to its corresponding enum value. If the parsing is successful, it filters the items in the AllItems collection based on the parsed item type and updates the collection accordingly.
                switch (itemType)
                {
                    case "Main":
                        var mainItems = AllItems.Where(item => item.ItemType == ItemType.Main).ToList();
                        AllItems.Clear();
                        foreach (var item in mainItems) { AllItems.Add(item); }
                        break;

                    case "Side":
                        var sideItems = AllItems.Where(item => item.ItemType == ItemType.Side).ToList();
                        AllItems.Clear();
                        foreach (var item in sideItems) { AllItems.Add(item); }
                        break;

                    case "Drink":
                        var drinkItems = AllItems.Where(item => item.ItemType == ItemType.Drink).ToList();
                        AllItems.Clear();
                        foreach (var item in drinkItems) { AllItems.Add(item); }
                        break;
                }
            }
        }

        // Method to add a new item to the database
        public async Task AddItem(Item newItem)
        {
            try
            {
                var response = await App.Http.PostAsJsonAsync("/api/items", newItem);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Item added successfully!");
                    RefreshPage(null); // Refresh the page to show the newly added item
                }
                else { MessageBox.Show("Failed to add item. Please try again."); }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        // Updated the item details, the item id is used to identify the item to be updated, and the updated item object contains the new details for the item. 
        public async Task UpdateItem(Item updatedItem)
        {
            try
            {
                var response = await App.Http.PutAsJsonAsync($"/api/items/{updatedItem.Id}", updatedItem);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Item updated successfully!");
                    RefreshPage(null); // Refresh the page to show the updated item
                }
                else { MessageBox.Show("Failed to update item. Please try again."); }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        // Method to delete an item from the database, the item id is used to identify the item to be deleted.
        public async Task DeleteItem(int itemId)
        {
            try
            {
                var response = await App.Http.PutAsync($"/api/items/delete/{itemId}", null);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Item deleted successfully!");
                    RefreshPage(null); // Refresh the page to remove the deleted item from the list
                }
                else { MessageBox.Show("Failed to delete item. Please try again."); }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}
