using Conwy_Cafe_Admin_App.Utilities;
using ConwyCafe.Shared.Models;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Windows;

namespace Conwy_Cafe_Admin_App.ViewModels
{
    public class OrdersVM : ViewModelBase
    {
        public OrdersVM()
        {
            LoadData();

            RefreshOrdersCommand = new RelayCommand(RefreshPage);
            ArchiveOrderCommand = new RelayCommand(ArchiveOrder);
        }

        // Variables
        private Order _selectedOrder;
        private string _selectedOrderType = "All"; // Default to "All" to show all orders initially
        private bool _enableArchiveButton = false; // Archive button is disabled by default. Only enabled if a non-archived order is selected.

        public ObservableCollection<Order> AllOrders { get; set; } = new ObservableCollection<Order>();

        // Properties
        public Order SelectedOrder
        {
            get { return _selectedOrder; }
            set { 
                _selectedOrder = value; 
                OnPropertyChanged(nameof(SelectedOrder)); }
        }


        // Methods
        public void RefreshPage()
        {
            // This method would contain logic to refresh the page, such as re-fetching data from the data source and updating the relevant properties or collections in the view model. 
            // For example, it might look like this:
            // GetAllBaskets();
        }
        public async void LoadData()
        {
            await GetAllOrders();
        }

        public async Task GetAllOrders()
        {
            RefreshPage();

            try
            {
                var response = await App.Http.GetFromJsonAsync<List<Order>>("/api/order");
                if (response != null)
                {
                    AllOrders.Clear();
                    foreach (var order in response) { AllOrders.Add(order); }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

        }


    }
}
