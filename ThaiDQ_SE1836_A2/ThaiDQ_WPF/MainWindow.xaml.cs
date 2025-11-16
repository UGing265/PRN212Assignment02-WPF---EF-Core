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

        private void btnRoom_Click(object sender, RoutedEventArgs e)
        {
            HideAllPanels();
            panelRoom.Visibility = Visibility.Visible;
            ClearRoomForm();
        }

        // Room CRUD Actions
        private void dgRoom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgRoom.SelectedItem is RoomInformation selectedRoom)
            {
                // Auto fill form khi click vào row
                txtRoomNumber.Text = selectedRoom.RoomNumber;
                cbRoomType.SelectedValue = selectedRoom.RoomTypeId;
                txtRoomCapacity.Text = selectedRoom.RoomMaxCapacity?.ToString();
                txtRoomPrice.Text = selectedRoom.RoomPricePerDay?.ToString();
                cbRoomStatus.SelectedIndex = selectedRoom.RoomStatus == 1 ? 0 : 1;
                txtRoomDescription.Text = selectedRoom.RoomDetailDescription;
            }
        }

        private void btnAddRoom_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            string validationError = ValidateRoomInput();
            if (!string.IsNullOrEmpty(validationError))
            {
                MessageBox.Show(validationError, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Check room number duplicate
            var existingRooms = _roomService.GetRooms();
            if (existingRooms.Any(r => r.RoomNumber.Equals(txtRoomNumber.Text.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Room number already exists!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newRoom = new RoomInformation
            {
                RoomNumber = txtRoomNumber.Text.Trim(),
                RoomTypeId = (int)cbRoomType.SelectedValue,
                RoomMaxCapacity = int.Parse(txtRoomCapacity.Text.Trim()),
                RoomPricePerDay = decimal.Parse(txtRoomPrice.Text.Trim()),
                RoomStatus = cbRoomStatus.SelectedIndex == 0 ? (byte)1 : (byte)0,
                RoomDetailDescription = txtRoomDescription.Text.Trim()
            };

            _roomService.AddRoom(newRoom);
            MessageBox.Show("Room added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadRoom();
            ClearRoomForm();
        }

        private void btnUpdateRoom_Click(object sender, RoutedEventArgs e)
        {
            if (dgRoom.SelectedItem is not RoomInformation selectedRoom)
            {
                MessageBox.Show("Please select a room to update!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validation
            string validationError = ValidateRoomInput();
            if (!string.IsNullOrEmpty(validationError))
            {
                MessageBox.Show(validationError, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Check room number duplicate (trừ chính nó)
            var existingRooms = _roomService.GetRooms();
            if (existingRooms.Any(r => r.RoomNumber.Equals(txtRoomNumber.Text.Trim(), StringComparison.OrdinalIgnoreCase)
                && r.RoomId != selectedRoom.RoomId))
            {
                MessageBox.Show("Room number already exists!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            selectedRoom.RoomNumber = txtRoomNumber.Text.Trim();
            selectedRoom.RoomTypeId = (int)cbRoomType.SelectedValue;
            selectedRoom.RoomMaxCapacity = int.Parse(txtRoomCapacity.Text.Trim());
            selectedRoom.RoomPricePerDay = decimal.Parse(txtRoomPrice.Text.Trim());
            selectedRoom.RoomStatus = cbRoomStatus.SelectedIndex == 0 ? (byte)1 : (byte)0;
            selectedRoom.RoomDetailDescription = txtRoomDescription.Text.Trim();

            _roomService.UpdateRoom(selectedRoom);
            MessageBox.Show("Room updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadRoom();
            ClearRoomForm();
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
                ClearRoomForm();
            }
        }

        private void btnClearRoom_Click(object sender, RoutedEventArgs e)
        {
            ClearRoomForm();
        }

        private void ClearRoomForm()
        {
            txtRoomNumber.Clear();
            cbRoomType.SelectedIndex = -1;
            txtRoomCapacity.Clear();
            txtRoomPrice.Clear();
            cbRoomStatus.SelectedIndex = 0;
            txtRoomDescription.Clear();
            dgRoom.SelectedItem = null;
        }

        private string ValidateRoomInput()
        {
            // Room Number validation
            if (string.IsNullOrWhiteSpace(txtRoomNumber.Text))
            {
                return "Room Number is required!";
            }

            // Room Type validation
            if (cbRoomType.SelectedValue == null)
            {
                return "Please select a Room Type!";
            }

            // Capacity validation
            if (string.IsNullOrWhiteSpace(txtRoomCapacity.Text))
            {
                return "Room Capacity is required!";
            }
            if (!int.TryParse(txtRoomCapacity.Text.Trim(), out int capacity) || capacity < 1 || capacity > 10)
            {
                return "Room Capacity must be between 1 and 10!";
            }

            // Price validation
            if (string.IsNullOrWhiteSpace(txtRoomPrice.Text))
            {
                return "Room Price is required!";
            }
            if (!decimal.TryParse(txtRoomPrice.Text.Trim(), out decimal price) || price <= 0)
            {
                return "Room Price must be greater than 0!";
            }

            // Status validation
            if (cbRoomStatus.SelectedIndex == -1)
            {
                return "Please select a Status!";
            }

            return string.Empty; // No error
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