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
        private int? customerId; // Lưu customerId nếu là customer
        private CustomerService _customerService;
        private RoomService _roomService;
        private BookingService _bookingService;
        
        public MainWindow(string userRole, int? customerIdParam = null)
        {
            InitializeComponent();
            role = userRole;
            customerId = customerIdParam;
            _customerService = new CustomerService();
            _roomService = new RoomService();
            _bookingService = new BookingService();
            SetupRoleUI();
            LoadCustomer();
            LoadRoom();
            LoadBooking();
        }

        private void SetupRoleUI()
        {
            // Ẩn toàn bộ panel
            HideAllPanels();
            panelDashboard.Visibility = Visibility.Visible;

            if (role.ToLower() == "admin")
            {
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

        private void LoadRoom()
        {
            dgRoom.ItemsSource = _roomService.GetRooms();
        }
        private void LoadBooking()
        {
            dgBookReservation.ItemsSource = _bookingService.GetBookingReservations();
        }
        private void LoadStatisticReport()
        {
            //dgReport.ItemsSource = _bookingService.GetStatisticReport();
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
            LoadProfile();
        }

        private void LoadProfile()
        {
            if (customerId == null)
            {
                MessageBox.Show("Customer ID not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var customer = _customerService.GetCustomerById(customerId.Value);
            if (customer != null)
            {
                txtProfileId.Text = customer.CustomerId.ToString();
                txtProfileName.Text = customer.CustomerFullName;
                txtProfileEmail.Text = customer.EmailAddress;
                txtProfilePhone.Text = customer.Telephone;
                dpProfileBirthday.SelectedDate = customer.CustomerBirthday?.ToDateTime(TimeOnly.MinValue);
                txtProfileStatus.Text = customer.CustomerStatus == 1 ? "Active" : "Inactive";
            }
        }

        private void btnSaveProfile_Click(object sender, RoutedEventArgs e)
        {
            if (customerId == null) return;

            // Validation
            if (string.IsNullOrWhiteSpace(txtProfileName.Text))
            {
                MessageBox.Show("Full Name is required!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var customer = _customerService.GetCustomerById(customerId.Value);
            if (customer != null)
            {
                // Update thông tin
                customer.CustomerFullName = txtProfileName.Text.Trim();
                customer.Telephone = txtProfilePhone.Text.Trim();
                customer.CustomerBirthday = dpProfileBirthday.SelectedDate.HasValue 
                    ? DateOnly.FromDateTime(dpProfileBirthday.SelectedDate.Value) 
                    : null;

                _customerService.UpdateCustomer(customer);
                MessageBox.Show("Profile updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            HideAllPanels();
            panelHistory.Visibility = Visibility.Visible;
            LoadHistoryForCustomer();
        }

        private void LoadHistoryForCustomer()
        {
            // Load booking history cho customer (y chang admin show booking)
            dgBookReserabcvation.ItemsSource = _bookingService.GetBookingReservations();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Logout successful!");
            Close();
        }

        private void btnStReport_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDt = dpStart.SelectedDate ?? DateTime.MinValue;
            DateTime endDt = dpEnd.SelectedDate ?? DateTime.MaxValue;

            DateOnly start = DateOnly.FromDateTime(startDt);
            DateOnly end = DateOnly.FromDateTime(endDt);

            dgReport.ItemsSource = _bookingService.GetStatisticReport(start, end);
        }
    }
}