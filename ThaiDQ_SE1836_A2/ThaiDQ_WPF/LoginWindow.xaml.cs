using BLL.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ThaiDQ_WPF
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        CustomerService _customerService;
        private IConfiguration _config;
        public LoginWindow()
        {
            InitializeComponent();
            _customerService = new CustomerService();

            _config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
        }













        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text;
            string pasword = txtPassword.Password;

            string adminEmail = _config["DefaultAccount:Email"];
            string adminPassword = _config["DefaultAccount:Password"];
            string role = "Customer";


            if(email == adminEmail && pasword == adminPassword)
            {
                role = "Admin";
                MessageBox.Show("Welcome to mini hotel, admin", "login", MessageBoxButton.OK, MessageBoxImage.Information);
                  MainWindow mainWindow = new MainWindow(role);
                  mainWindow.Show();
                  this.Close();
                  return; //  dừng lại, không kiểm tra Customer nữa


            }
            var customer = _customerService.GetCustomer(email, pasword);

            if (customer != null)
            {
                MessageBox.Show("Đăng nhập thành công thế giới bí ẩn", "Login", MessageBoxButton.OK, MessageBoxImage.Information);
                MainWindow mainWindow = new MainWindow("Admin");
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Sai rồi nhóc", "Login", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
