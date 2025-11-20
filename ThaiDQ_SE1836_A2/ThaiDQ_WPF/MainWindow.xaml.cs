using BLL.Services;
using DAL.Entities;
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

        // Cache full lists for search
        private List<Customer> _allCustomers;
        private List<RoomInformation> _allRooms;
        
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
            _allCustomers = _customerService.GetCustomers();
            dgCustomer.ItemsSource = _allCustomers;
        }

        private void LoadRoom()
        {
            _allRooms = _roomService.GetRooms();
            dgRoom.ItemsSource = _allRooms;
            cbRoomType.ItemsSource = _roomService.GetRoomTypes();
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

        private void btnCustomer_Click(object sender, RoutedEventArgs e)
        {
            HideAllPanels();
            panelCustomer.Visibility = Visibility.Visible;
        }

        private void btnManageCustomer_Click(object sender, RoutedEventArgs e)
        {
            Customer? selectedCustomer = dgCustomer.SelectedItem as Customer;
            var dialog = new CustomerDialog(selectedCustomer, _allCustomers);
            
            if (dialog.ShowDialog() == true)
            {
                LoadCustomer();
                MessageBox.Show(dialog.IsCreateMode ? "Customer added successfully!" : "Customer updated successfully!", 
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnSearchCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearchCustomer.Text))
            {
                LoadCustomer(); // Reload all if search is empty
                return;
            }

            var searchText = txtSearchCustomer.Text.Trim().ToLower();
            var results = _allCustomers.Where(c =>
                c.CustomerFullName.ToLower().Contains(searchText) ||
                c.EmailAddress.ToLower().Contains(searchText) ||
                (c.Telephone != null && c.Telephone.Contains(searchText))
            ).ToList();

            dgCustomer.ItemsSource = results;
        }

        private void btnDeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (dgCustomer.SelectedItem is not Customer selectedCustomer)
            {
                MessageBox.Show("Please select a customer to delete!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete customer '{selectedCustomer.CustomerFullName}'?", 
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _customerService.DeleteCustomer(selectedCustomer.CustomerId);
                MessageBox.Show("Customer deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadCustomer();
            }
        }

        // Room CRUD Actions
        private void btnRoom_Click(object sender, RoutedEventArgs e)
        {
            HideAllPanels();
            panelRoom.Visibility = Visibility.Visible;
        }

        private void btnAddRoom_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new RoomDialog(_roomService.GetRoomTypes());
            if (dialog.ShowDialog() == true)
            {
                // Check room number duplicate
                if (_allRooms.Any(r => r.RoomNumber.Equals(dialog.Room.RoomNumber, StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show("Room number already exists!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _roomService.AddRoom(dialog.Room);
                MessageBox.Show("Room added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadRoom();
            }
        }

        private void btnUpdateRoom_Click(object sender, RoutedEventArgs e)
        {
            if (dgRoom.SelectedItem is not RoomInformation selectedRoom)
            {
                MessageBox.Show("Please select a room to update!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new RoomDialog(selectedRoom, _roomService.GetRoomTypes());
            if (dialog.ShowDialog() == true)
            {
                // Check room number duplicate (trừ chính nó)
                if (_allRooms.Any(r => r.RoomNumber.Equals(dialog.Room.RoomNumber, StringComparison.OrdinalIgnoreCase)
                    && r.RoomId != selectedRoom.RoomId))
                {
                    MessageBox.Show("Room number already exists!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _roomService.UpdateRoom(dialog.Room);
                MessageBox.Show("Room updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadRoom();
            }
        }

        private void btnDeleteRoom_Click(object sender, RoutedEventArgs e)
        {
            if (dgRoom.SelectedItem is not RoomInformation selectedRoom)
            {
                MessageBox.Show("Please select a room to delete!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete room '{selectedRoom.RoomNumber}'?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _roomService.DeleteRoom(selectedRoom.RoomId);
                MessageBox.Show("Room deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadRoom();
            }
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
            if (customerId == null)
            {
                MessageBox.Show("Customer ID not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Load ONLY booking của customer hiện tại (filter by customerId)
            dgBookReserabcvation.ItemsSource = _bookingService.GetBookingReservationsByCustomerId(customerId.Value);
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

        // Search functionality for Customer
        private void txtSearchCustomer_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearchCustomer.Text.ToLower().Trim();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                // Show all if search is empty
                dgCustomer.ItemsSource = _allCustomers;
            }
            else
            {
                // Filter by Name, Email, or Phone
                var filteredList = _allCustomers.Where(c =>
                    (c.CustomerFullName != null && c.CustomerFullName.ToLower().Contains(searchText)) ||
                    (c.EmailAddress != null && c.EmailAddress.ToLower().Contains(searchText)) ||
                    (c.Telephone != null && c.Telephone.Contains(searchText))
                ).ToList();

                dgCustomer.ItemsSource = filteredList;
            }
        }

        // Search functionality for Room
        private void txtSearchRoom_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearchRoom.Text.ToLower().Trim();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                // Show all if search is empty
                dgRoom.ItemsSource = _allRooms;
            }
            else
            {
                // Filter by Room Number or Description
                var filteredList = _allRooms.Where(r =>
                    (r.RoomNumber != null && r.RoomNumber.ToLower().Contains(searchText)) ||
                    (r.RoomDetailDescription != null && r.RoomDetailDescription.ToLower().Contains(searchText)) ||
                    (r.RoomType?.RoomTypeName != null && r.RoomType.RoomTypeName.ToLower().Contains(searchText))
                ).ToList();

                dgRoom.ItemsSource = filteredList;
            }
        }
    }
}