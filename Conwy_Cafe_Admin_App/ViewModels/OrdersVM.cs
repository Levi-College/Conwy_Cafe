using Conwy_Cafe_Admin_App.Utilities;
using ConwyCafe.Shared.Models;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Input;

namespace Conwy_Cafe_Admin_App.ViewModels
{
    public class OrdersVM : ViewModelBase
    {
        public OrdersVM()
        {
            LoadData();

            RefreshOrdersCommand = new RelayCommand(RefreshPage);
            ArchiveOrderCommand = new RelayCommand(async (obj)=> await ArchiveOrder(obj));
        }

        // Variables
        private Order _selectedOrder;
        private string _selectedOrderType = "All"; // Default to "All" to show all orders initially
        private bool _enableArchiveButton = false; // Archive button is disabled by default. Only enabled if a non-archived order is selected.

        public ICommand RefreshOrdersCommand { get; }
        public ICommand ArchiveOrderCommand { get; }

        public ObservableCollection<Order> AllOrders { get; set; } = new ObservableCollection<Order>();
        public List<string> OrderFilterList { get; set; } = new List<string> { "All", "Active", "Archived" };
        // Properties
        public Order SelectedOrder
        {
            get { return _selectedOrder; }
            set
            {
                _selectedOrder = value;
                OnPropertyChanged(nameof(SelectedOrder));

                // Setting the archive button to enabled or disabled depending on the archived status of the selected order. If the order is archived, the button is disabled. If the order is not archived, the button is enabled.
                if (SelectedOrder != null)
                {
                    if (SelectedOrder.Archived) { EnableArchiveButton = false; }
                    else { EnableArchiveButton = true; }
                }
            }
        }

        public string SelectedOrderType
        {
            get { return _selectedOrderType; }
            set
            {
                _selectedOrderType = value;
                OnPropertyChanged(nameof(SelectedOrderType));
                FilterOrdersByType(value);
            }
        }

        
        public bool EnableArchiveButton
        {
            get { return _enableArchiveButton; }
            set
            {
                _enableArchiveButton = value;
                OnPropertyChanged(nameof(EnableArchiveButton));
            }
        }

        // Methods
        public void RefreshPage(object? obj)
        {
            SelectedOrder = null; // Clear the selected order when refreshing the page
            GetAllOrders();
        }
        public async void LoadData()
        {
            await GetAllOrders();
        }

        public async Task GetAllOrders()
        {
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

        private async Task FilterOrdersByType(string value)
        {
            if (value == "All") { GetAllOrders(); }
            else if (value == "Active")
            {
                await GetAllOrders(); // Refresh the orders list to ensure we have the latest data before filtering
                var activeOrders = AllOrders.Where(o => !o.Archived).ToList(); // Filter the orders to only include active (non-archived) orders
                AllOrders.Clear();
                foreach (var order in activeOrders) { AllOrders.Add(order); }
            }
            else if (value == "Archived")
            {
                await GetAllOrders(); // Refresh the orders list to ensure we have the latest data before filtering
                var archivedOrders = AllOrders.Where(o => o.Archived).ToList();
                AllOrders.Clear();
                foreach (var order in archivedOrders) { AllOrders.Add(order); }
            }
        }

        public async Task ArchiveOrder(object? obj)
        {
            if (SelectedOrder == null) { MessageBox.Show("Please select an order to archive."); return; }
            if (SelectedOrder.Archived) { MessageBox.Show("This order is already archived."); return; }
            try
            {
                var response = await App.Http.PutAsJsonAsync($"/api/order/archive/{SelectedOrder.Id}", SelectedOrder);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Order archived successfully.");
                    await GetAllOrders(); // Refresh the orders list after archiving
                }
                else
                {
                    MessageBox.Show("Failed to archive the order. Please try again.");
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

    }
}
