using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Conwy_Cafe_Admin_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new ViewModels.AdminVM(); // Set the DataContext of the MainWindow to an instance of the AdminVM view model. This allows the UI elements in the MainWindow to bind to properties and commands defined in the AdminVM, enabling the MVVM pattern for data binding and separation of concerns between the view and view model.
        }
    }
}