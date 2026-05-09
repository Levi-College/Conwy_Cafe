using Conwy_Cafe_Admin_App.Utilities;
using System.Windows.Input;


namespace Conwy_Cafe_Admin_App.ViewModels
{

    public class AdminVM : ViewModelBase
    {
        // Class constructor, which is called when an instance of the AdminVM class is created. It initializes the view model and sets up any necessary data or commands. In this case, it is currently empty, but it can be used to set up initial values or commands for the view model.
        public AdminVM()
        {
            // Page commands
            BasketsCommand = new RelayCommand(BasketsPage);
            ItemsCommand = new RelayCommand(ItemsPage);
            OrdersCommand = new RelayCommand(OrdersPage);

            CurrentView = new BasketsVM(); // Set the default view to the baskets page when the application starts. This means that when the admin interface is first loaded, it will display the baskets page by default.
        }

        // Declaring variables
        private object _currentView = null;
        public ICommand BasketsCommand { get; } //Command for baskets page
        public ICommand ItemsCommand { get; } // Command for items page
        public ICommand OrdersCommand { get; } // Command for orders page


        // Properties
        public object CurrentView
        {
            get { return _currentView; }
            // Notify the view that the CurrentView property has changed, allowing the UI to update accordingly (e.g., displaying the new view content)
            set { _currentView = value; OnPropertyChanged(nameof(CurrentView)); }
        }

        private void BasketsPage(object? obj)
        {
            // The data template file will assign the UI since BasketsVM is the view model for the baskets page. This method will be called when the BasketsCommand is executed. It will set the CurrentView property to a new instance of the BasketsVM, which will be the view model for the baskets page.
            CurrentView = new BasketsVM();
        }

        private void ItemsPage(object? obj) { CurrentView = new ItemsVM(); }
        private void OrdersPage(object? obj) { CurrentView = new OrdersVM(); }
    }
}
