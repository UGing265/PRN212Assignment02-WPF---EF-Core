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
            ClearCustomerForm();
        }

        // Customer CRUD Actions
        private void dgCustomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgCustomer.SelectedItem is Customer selectedCustomer)
            {
                // Auto fill form khi click vào row
                txtCusName.Text = selectedCustomer.CustomerFullName;
                txtCusEmail.Text = selectedCustomer.EmailAddress;
                txtCusPhone.Text = selectedCustomer.Telephone;
                txtCusPassword.Text = selectedCustomer.Password;
                dpCusBirthday.SelectedDate = selectedCustomer.CustomerBirthday?.ToDateTime(TimeOnly.MinValue);
                cbCusStatus.SelectedIndex = selectedCustomer.CustomerStatus == 1 ? 0 : 1;
            }
        }

        private void btnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            // Validation đầy đủ
            string validationError = ValidateCustomerInput();
            if (!string.IsNullOrEmpty(validationError))
            {
                MessageBox.Show(validationError, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Check email duplicate
            var existingCustomers = _customerService.GetCustomers();
            if (existingCustomers.Any(c => c.EmailAddress.Equals(txtCusEmail.Text.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Email already exists!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newCustomer = new Customer
            {
                CustomerFullName = txtCusName.Text.Trim(),
                EmailAddress = txtCusEmail.Text.Trim(),
                Telephone = txtCusPhone.Text.Trim(),
                Password = string.IsNullOrWhiteSpace(txtCusPassword.Text) ? "123456" : txtCusPassword.Text.Trim(),
                CustomerBirthday = dpCusBirthday.SelectedDate.HasValue ? DateOnly.FromDateTime(dpCusBirthday.SelectedDate.Value) : null,
                CustomerStatus = cbCusStatus.SelectedIndex == 0 ? (byte)1 : (byte)0
            };

            _customerService.AddCustomer(newCustomer);
            MessageBox.Show("Customer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadCustomer();
            ClearCustomerForm();
        }

        private void btnUpdateCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (dgCustomer.SelectedItem is not Customer selectedCustomer)
            {
                MessageBox.Show("Please select a customer to update!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validation đầy đủ
            string validationError = ValidateCustomerInput();
            if (!string.IsNullOrEmpty(validationError))
            {
                MessageBox.Show(validationError, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Check email duplicate (trừ chính nó)
            var existingCustomers = _customerService.GetCustomers();
            if (existingCustomers.Any(c => c.EmailAddress.Equals(txtCusEmail.Text.Trim(), StringComparison.OrdinalIgnoreCase) 
                && c.CustomerId != selectedCustomer.CustomerId))
            {
                MessageBox.Show("Email already exists!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            selectedCustomer.CustomerFullName = txtCusName.Text.Trim();
            selectedCustomer.EmailAddress = txtCusEmail.Text.Trim();
            selectedCustomer.Telephone = txtCusPhone.Text.Trim();
            selectedCustomer.Password = txtCusPassword.Text.Trim();
            selectedCustomer.CustomerBirthday = dpCusBirthday.SelectedDate.HasValue ? DateOnly.FromDateTime(dpCusBirthday.SelectedDate.Value) : null;
            selectedCustomer.CustomerStatus = cbCusStatus.SelectedIndex == 0 ? (byte)1 : (byte)0;

            _customerService.UpdateCustomer(selectedCustomer);
            MessageBox.Show("Customer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadCustomer();
            ClearCustomerForm();
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
                ClearCustomerForm();
            }
        }

        private void btnClearCustomer_Click(object sender, RoutedEventArgs e)
        {
            ClearCustomerForm();
        }

        private void ClearCustomerForm()
        {
            txtCusName.Clear();
            txtCusEmail.Clear();
            txtCusPhone.Clear();
            txtCusPassword.Clear();
            dpCusBirthday.SelectedDate = null;
            cbCusStatus.SelectedIndex = 0;
            dgCustomer.SelectedItem = null;
        }

        private string ValidateCustomerInput()
        {
            // Full Name validation
            if (string.IsNullOrWhiteSpace(txtCusName.Text))
            {
                return "Full Name is required!";
            }
            if (txtCusName.Text.Trim().Length < 2)
            {
                return "Full Name must be at least 2 characters!";
            }

            // Email validation
            if (string.IsNullOrWhiteSpace(txtCusEmail.Text))
            {
                return "Email is required!";
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(txtCusEmail.Text.Trim(), 
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return "Invalid email format!";
            }

            // Password validation
            if (string.IsNullOrWhiteSpace(txtCusPassword.Text))
            {
                return "Password is required!";
            }
            if (txtCusPassword.Text.Trim().Length < 3)
            {
                return "Password must be at least 3 characters!";
            }

            // Phone validation (nếu có nhập)
            if (!string.IsNullOrWhiteSpace(txtCusPhone.Text))
            {
                string phone = txtCusPhone.Text.Trim();
                if (!System.Text.RegularExpressions.Regex.IsMatch(phone, @"^[0-9]{10,11}$"))
                {
                    return "Phone number must be 10-11 digits!";
                }
            }

            // Birthday validation (không được quá trẻ hoặc quá già)
            if (dpCusBirthday.SelectedDate.HasValue)
            {
                var age = DateTime.Now.Year - dpCusBirthday.SelectedDate.Value.Year;
                if (age < 18)
                {
                    return "Customer must be at least 18 years old!";
                }
                if (age > 120)
                {
                    return "Invalid birthday!";
                }
            }

            // Status validation
            if (cbCusStatus.SelectedIndex == -1)
            {
                return "Please select a status!";
            }

            return string.Empty; // No error
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
    }
}