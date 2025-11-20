using BLL.Services;
using DAL.Entities;
using System;
using System.Linq;
using System.Windows;

namespace ThaiDQ_WPF
{
    public partial class CustomerDialog : Window
    {
        private readonly CustomerService _customerService;
        private readonly List<Customer> _allCustomers;
        public Customer Customer { get; private set; }
        public bool IsCreateMode { get; set; }

        // Constructor: null customer = Create, non-null = Update
        public CustomerDialog(Customer? customer, List<Customer> allCustomers)
        {
            InitializeComponent();
            _customerService = new CustomerService();
            _allCustomers = allCustomers;
            cbStatus.SelectedIndex = 0; // Default Active

            if (customer == null)
            {
                // Create mode
                IsCreateMode = true;
            }
            else
            {
                // Update mode
                IsCreateMode = false;
                Customer = customer;
                LoadCustomerData();
            }
        }

        private void LoadCustomerData()
        {
            if (Customer != null)
            {
                txtName.Text = Customer.CustomerFullName;
                txtEmail.Text = Customer.EmailAddress;
                txtPassword.Text = Customer.Password;
                txtPhone.Text = Customer.Telephone;
                txtBirthday.Text = Customer.CustomerBirthday?.ToString("dd/MM/yyyy") ?? "";
                cbStatus.SelectedIndex = Customer.CustomerStatus == 1 ? 0 : 1;
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Full Name is required!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Email is required!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(txtEmail.Text.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Invalid email format!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text) || txtPassword.Text.Trim().Length < 3)
            {
                MessageBox.Show("Password must be at least 3 characters!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(txtPhone.Text.Trim(), @"^[0-9]{10,11}$"))
                {
                    MessageBox.Show("Phone number must be 10-11 digits!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(txtBirthday.Text))
            {
                if (!DateOnly.TryParseExact(txtBirthday.Text.Trim(), "dd/MM/yyyy", out var birthDate))
                {
                    MessageBox.Show("Invalid birthday format! Use dd/MM/yyyy (e.g., 25/12/1990)", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                var age = DateTime.Now.Year - birthDate.Year;
                if (age < 18 || age > 120)
                {
                    MessageBox.Show("Customer must be between 18 and 120 years old!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }

            return true;
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            // Check duplicate email
            if (_allCustomers.Any(c => c.EmailAddress.Equals(txtEmail.Text.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Email already exists!", "Duplicate", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateOnly? birthday = null;
            if (!string.IsNullOrWhiteSpace(txtBirthday.Text) && DateOnly.TryParseExact(txtBirthday.Text.Trim(), "dd/MM/yyyy", out var birthDate))
            {
                birthday = birthDate;
            }

            Customer = new Customer
            {
                CustomerFullName = txtName.Text.Trim(),
                EmailAddress = txtEmail.Text.Trim(),
                Password = txtPassword.Text.Trim(),
                Telephone = txtPhone.Text.Trim(),
                CustomerBirthday = birthday,
                CustomerStatus = cbStatus.SelectedIndex == 0 ? (byte)1 : (byte)0
            };

            _customerService.AddCustomer(Customer);
            DialogResult = true;
            Close();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            // Check duplicate email (excluding current customer)
            if (_allCustomers.Any(c => c.CustomerId != Customer.CustomerId && 
                c.EmailAddress.Equals(txtEmail.Text.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Email already exists!", "Duplicate", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateOnly? birthday = null;
            if (!string.IsNullOrWhiteSpace(txtBirthday.Text) && DateOnly.TryParseExact(txtBirthday.Text.Trim(), "dd/MM/yyyy", out var birthDate))
            {
                birthday = birthDate;
            }

            Customer.CustomerFullName = txtName.Text.Trim();
            Customer.EmailAddress = txtEmail.Text.Trim();
            Customer.Password = txtPassword.Text.Trim();
            Customer.Telephone = txtPhone.Text.Trim();
            Customer.CustomerBirthday = birthday;
            Customer.CustomerStatus = cbStatus.SelectedIndex == 0 ? (byte)1 : (byte)0;

            _customerService.UpdateCustomer(Customer);
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
