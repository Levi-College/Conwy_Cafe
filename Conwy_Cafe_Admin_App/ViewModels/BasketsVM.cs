using Conwy_Cafe_Admin_App.Utilities;
using ConwyCafe.Shared.Models;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Windows;

namespace Conwy_Cafe_Admin_App.ViewModels
{
    public class BasketsVM : ViewModelBase
    {
        // Constructor
        public BasketsVM()
        {
            // Loading the data
            LoadData();
        }


        // Declaring variables
        private Basket _selectedBasket;
        private Item _selectedItem;
        private Order _selectedOrder;

        public ObservableCollection<Basket> AllBaskets { get; set; } = new ObservableCollection<Basket>();
        public ObservableCollection<Item> AllItems { get; set; } = new ObservableCollection<Item>();
        public ObservableCollection<Order> AllOrder { get; set; } = new ObservableCollection<Order>();
        public ObservableCollection<Categories> AllCategories { get; set; } = new ObservableCollection<Categories>();



        // Properties
        public Basket SelectedBasket
        {
            get { return _selectedBasket; }
            set
            {
                _selectedBasket = value;
                OnPropertyChanged(nameof(SelectedBasket));
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


        // Methods
        public async void LoadData()
        {
            // Waits for the data of baskets and categories to be loaded before proceeding. This ensures that the necessary data is available for the view model to function properly, such as populating the UI with the retrieved baskets and categories.
            await GetAllBaskets();
            await GetAllCategories();
        }
        public void RefreshPage()
        {
            // This method would contain logic to refresh the page, such as re-fetching data from the data source and updating the relevant properties or collections in the view model. 
            // For example, it might look like this:
            // GetAllBaskets();
        }

        public async Task GetAllBaskets()
        {
            RefreshPage();
            // This method would contain logic to retrieve all baskets from the data source (e.g., database, API) and populate the relevant properties or collections in the view model. 
            // For example, it might look like this:
            // Baskets = BasketService.GetAllBaskets();

            try
            {
                // calling the api to get all baskets and adding them to the observable collection
                var response = await App.Http.GetFromJsonAsync<List<Basket>>("/api/basket"); // Make a synchronous GET request to the specified API endpoint to retrieve all baskets. The result is stored in the 'response' variable.

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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //Getting all the basket categories
        public async Task GetAllCategories()
        {
            try
            {
                var response = await App.Http.GetFromJsonAsync<List<Categories>>("/api/categories"); // Make a synchronous GET request to the specified API endpoint to retrieve all categories. The result is stored in the 'response' variable.
                if (response != null)
                {
                    AllCategories.Clear(); // Clear the existing categories in the observable collection before adding new ones to avoid duplicates.
                    foreach (var category in response) // Iterate through each category in the response (using .Result to get the result of the asynchronous operation).
                    {
                        AllCategories.Add(category); // Add each category to the 'AllCategories' observable collection.
                    }
                    MessageBox.Show("Categories retrieved successfully!"); // Show a message box to indicate that the categories were retrieved successfully.
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
