using BLL.Services;
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

namespace ThaiDQ_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string role;
        private CustomerService _customerService;

        public MainWindow(string userRole)
        {
            InitializeComponent();
            role = userRole;
            SetupRoleUI();
            LoadCustomer();
        }

        private void SetupRoleUI()
        {
            // Ẩn toàn bộ panel
            HideAllPanels();
            panelDashboard.Visibility = Visibility.Visible;

            if (role.ToLower() == "admin")
            {
                MessageBox.Show($"Đang chạy MainWindow, Role: {role}");
                // Ẩn chức năng chỉ dành cho customer
                btnProfile.Visibility = Visibility.Collapsed;
                btnHistory.Visibility = Visibility.Collapsed;
            }
            else if (role.ToLower() == "customer")
            {
                // Ẩn chức năng admin
                btnCustomer.Visibility = Visibility.Collapsed;
                btnBooking.Visibility = Visibility.Collapsed;
                btnReport.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadCustomer()
        {
            dgCustomer.ItemsSource = _customerService.GetCustomers();
        }


        private void HideAllPanels()
        {
            panelRoom.Visibility = Visibility.Collapsed;
            panelCustomer.Visibility = Visibility.Collapsed;
            panelBooking.Visibility = Visibility.Collapsed;
            panelReport.Visibility = Visibility.Collapsed;
            panelProfile.Visibility = Visibility.Collapsed;
            panelHistory.Visibility = Visibility.Collapsed;
            panelDashboard.Visibility = Visibility.Collapsed;
        }

        private void btnRoom_Click(object sender, RoutedEventArgs e)
        {
            HideAllPanels();
            panelRoom.Visibility = Visibility.Visible;
        }

        private void btnCustomer_Click(object sender, RoutedEventArgs e)
        {
            HideAllPanels();
            panelCustomer.Visibility = Visibility.Visible;
        }

        private void btnBooking_Click(object sender, RoutedEventArgs e)
        {
            HideAllPanels();
            panelBooking.Visibility = Visibility.Visible;
        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            HideAllPanels();
            panelReport.Visibility = Visibility.Visible;
        }

        private void btnProfile_Click(object sender, RoutedEventArgs e)
        {
            HideAllPanels();
            panelProfile.Visibility = Visibility.Visible;
        }

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            HideAllPanels();
            panelHistory.Visibility = Visibility.Visible;
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Logout successful!");
            Close();
        }
    }
}