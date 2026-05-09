using Conwy_Cafe_Admin_App.Utilities;
using Conwy_Cafe_Admin_App.Views;
using ConwyCafe.Shared.Models;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Input;

namespace Conwy_Cafe_Admin_App.ViewModels
{
    public class BasketsVM : ViewModelBase
    {
        // Constructor
        public BasketsVM()
        {
            // Loading the data
            LoadData();

            EditBasketWindowCommand = new RelayCommand(OpenEditBasketWindow);
        }

        // Declaring variables
        private Basket _selectedBasket;
        private Item _selectedItem;
        private Order _selectedOrder;
        //private string _fullImageUri;


        public ICommand EditBasketWindowCommand { get; }
        public ICommand NewBasketCommand { get; }

        public ObservableCollection<Basket> AllBaskets { get; set; } = new ObservableCollection<Basket>();
        public List<Item> AllItems { get; set; } = new List<Item>();
        public ObservableCollection<Order> AllOrders { get; set; } = new ObservableCollection<Order>();
        //public ObservableCollection<Categories> AllCategories { get; set; } = new ObservableCollection<Categories>();

        // The Mapping Dictionary
        public Dictionary<BasketCategory, string> CategoryDisplayNames { get; } = new()
        {
            { BasketCategory.Meat, "Meat" },
            { BasketCategory.Vegetarian, "Vegetarian" },
            { BasketCategory.ChildrenMeat, "Children Meat" },
            { BasketCategory.ChildrenVegetarian, "Children Vegetarian" }
        };

        // This is what the ComboBox will bind to
        // Converting the keys of the CategoryDisplayNames dictionary to a list, which will be used as the options for the ComboBox in the UI.
        // This allows the ComboBox to display the different basket categories (e.g., Meat, Vegetarian, Children Meat, Children Vegetarian) as selectable options for the user.
        public List<BasketCategory> CategoryOptions => CategoryDisplayNames.Keys.ToList();

        // Properties
        public Basket SelectedBasket
        {
            get { return _selectedBasket; }
            set
            {
                _selectedBasket = value;
                // Update the FullImagePath property whenever a new basket is selected.
                // This will trigger the UI to update the displayed image accordingly.
                //if (value.ImagePath != null) { FullImagePath = value.ImagePath; }
                OnPropertyChanged(nameof(SelectedBasket));
                OnPropertyChanged(nameof(FullImagePath)); // Notify that the FullImagePath property has changed, which will trigger the UI to update the displayed image based on the new selected basket's ImagePath.
            }
        }

        public Item SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; OnPropertyChanged(nameof(SelectedItem)); }
        }

        public Order SelectedOrder
        {
            get { return _selectedOrder; }
            set { _selectedOrder = value; OnPropertyChanged(nameof(SelectedOrder)); }
        }


        public string FullImagePath
        {
            get
            {
                // Use the null-conditional ?. to safely check if a basket exists
                if (string.IsNullOrEmpty(SelectedBasket?.ImagePath)) return null;

                //Old
                //var path = $"pack://application:,,,/ConwyCafe.Shared;component/{SelectedBasket.ImagePath}";

                // New (moved to API)
                var path = $"https://localhost:7008/{SelectedBasket.ImagePath}";


                return path;

                //return new Uri(path);
                //return new Uri($"pack://application:,,,/ConwyCafe.Shared;component/{SelectedBasket.ImagePath}"); 
            }
            //set
            //{
            //    // This constructs the full image path by combining the base path (pack URI) with the relative image path provided by the selected basket's ImagePath property.
            //    // Goes to the ConwyCafe.Shared project, looks for the image in the path specified by value (which is the ImagePath of the selected basket) and constructs a pack URI that can be used to display the image in the UI.
            //    // Value will be Images/Baskets/Basket1.png for example, so the full path will be pack://application:,,,/ConwyCafe.Shared;component/Images/Baskets/Basket1.png
            //    _fullImagePath = $"pack://application:,,,/ConwyCafe.Shared;component/{value}";
            //    OnPropertyChanged(nameof(FullImagePath));
            //}
        }



        // Methods
        public async void LoadData()
        {
            // Waits for the data of baskets and categories to be loaded before proceeding. This ensures that the necessary data is available for the view model to function properly, such as populating the UI with the retrieved baskets and categories.
            await GetAllBaskets();
            await GetAllItems();
            //await GetAllCategories();
        }


        public void RefreshPage()
        {
            // This method would contain logic to refresh the page, such as re-fetching data from the data source and updating the relevant properties or collections in the view model. 
            // For example, it might look like this:
            // GetAllBaskets();
        }

        public void OpenEditBasketWindow(object? obj)
        {
            if (SelectedBasket == null)
            {
                MessageBox.Show("Please select a basket to edit.");
                return;
            }
            //Setting the datacontext
            var EditBasketVM = new EditBasketVM(SelectedBasket, AllItems);
            // This method would contain logic to open a new window for editing a basket. 
            EditBasketWindow editWindow = new EditBasketWindow();
            editWindow.DataContext = EditBasketVM;
            editWindow.ShowDialog();
        }

        public async Task GetAllBaskets()
        {
            //RefreshPage(); // Can be used to refresh the pages
            try
            {
                // calling the api to get all baskets and adding them to the observable collection
                var response = await App.Http.GetFromJsonAsync<List<Basket>>("/api/basket?showAll=true"); // Make a synchronous GET request to the specified API endpoint to retrieve all baskets. The result is stored in the 'response' variable.

                // If the response is not null, it means that the API call was successful and we have received a list of baskets. We then iterate through each basket in the response and add it to the 'AllBaskets' observable collection, which is used to bind the data to the UI.
                if (response != null)
                {
                    AllBaskets.Clear(); // Clear the existing baskets in the observable collection before adding new ones to avoid duplicates.
                    foreach (var basket in response) // Iterate through each basket in the response (using .Result to get the result of the asynchronous operation).
                    {
                        AllBaskets.Add(basket); // Add each basket to the 'AllBaskets' observable collection.
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
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
    }
}
