using BLL.Services;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ThaiDQ_WPF
{
    public partial class BookingDialog : Window
    {
        private BookingService _bookingService;
        private List<BookingReservation> _allBookings;
        private List<Customer> _customers;
        private List<RoomInformation> _availableRooms;

        public BookingReservation Booking { get; set; }
        public bool IsCreateMode { get; set; }

        public BookingDialog(BookingReservation? booking, List<BookingReservation> allBookings, List<Customer> customers, List<RoomInformation> availableRooms)
        {
            InitializeComponent();
            _bookingService = new BookingService();
            _allBookings = allBookings;
            _customers = customers;
            _availableRooms = availableRooms;

            cbCustomer.ItemsSource = _customers;
            cbRoom.ItemsSource = _availableRooms;

            if (booking == null)
            {
                IsCreateMode = true;
                Booking = new BookingReservation();
                txtBookingDate.Text = DateOnly.FromDateTime(DateTime.Now).ToString("dd/MM/yyyy");
                cbStatus.SelectedIndex = 0;
            }
            else
            {
                IsCreateMode = false;
                Booking = booking;
                LoadBookingData();
            }
        }

        private void LoadBookingData()
        {
            cbCustomer.SelectedValue = Booking.CustomerId;
            txtBookingDate.Text = Booking.BookingDate?.ToString("dd/MM/yyyy") ?? "";
            txtTotalPrice.Text = Booking.TotalPrice?.ToString() ?? "";
            cbStatus.SelectedIndex = Booking.BookingStatus ?? 0;

            if (Booking.BookingDetails.Any())
            {
                var firstDetail = Booking.BookingDetails.First();
                cbRoom.SelectedValue = firstDetail.RoomId;
                txtStartDate.Text = firstDetail.StartDate.ToString("dd/MM/yyyy");
                txtEndDate.Text = firstDetail.EndDate.ToString("dd/MM/yyyy");
                txtActualPrice.Text = firstDetail.ActualPrice?.ToString() ?? "";
            }
        }

        private bool ValidateForm()
        {
            if (cbCustomer.SelectedValue == null)
            {
                MessageBox.Show("Please select a customer.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtBookingDate.Text) || !DateOnly.TryParseExact(txtBookingDate.Text, "dd/MM/yyyy", out _))
            {
                MessageBox.Show("Please enter a valid booking date in format dd/MM/yyyy.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtTotalPrice.Text) || !decimal.TryParse(txtTotalPrice.Text, out decimal totalPrice) || totalPrice <= 0)
            {
                MessageBox.Show("Total price must be greater than 0.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (cbStatus.SelectedItem == null)
            {
                MessageBox.Show("Please select a booking status.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (cbRoom.SelectedItem == null)
            {
                MessageBox.Show("Please select a room.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtStartDate.Text) || !DateOnly.TryParseExact(txtStartDate.Text, "dd/MM/yyyy", out _))
            {
                MessageBox.Show("Please enter a valid start date in format dd/MM/yyyy.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEndDate.Text) || !DateOnly.TryParseExact(txtEndDate.Text, "dd/MM/yyyy", out _))
            {
                MessageBox.Show("Please enter a valid end date in format dd/MM/yyyy.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            DateOnly startDate = DateOnly.ParseExact(txtStartDate.Text, "dd/MM/yyyy");
            DateOnly endDate = DateOnly.ParseExact(txtEndDate.Text, "dd/MM/yyyy");

            if (endDate <= startDate)
            {
                MessageBox.Show("End date must be after start date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtActualPrice.Text) || !decimal.TryParse(txtActualPrice.Text, out decimal actualPrice) || actualPrice <= 0)
            {
                MessageBox.Show("Actual price must be greater than 0.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            int customerId = (int)cbCustomer.SelectedValue;
            DateOnly bookingDate = DateOnly.ParseExact(txtBookingDate.Text, "dd/MM/yyyy");
            decimal totalPrice = decimal.Parse(txtTotalPrice.Text);
            byte bookingStatus = (byte)cbStatus.SelectedIndex;

            var selectedRoom = (RoomInformation)cbRoom.SelectedItem;
            DateOnly startDate = DateOnly.ParseExact(txtStartDate.Text, "dd/MM/yyyy");
            DateOnly endDate = DateOnly.ParseExact(txtEndDate.Text, "dd/MM/yyyy");
            decimal actualPrice = decimal.Parse(txtActualPrice.Text);

            try
            {
                var newBooking = new BookingReservation
                {
                    CustomerId = customerId,
                    BookingDate = bookingDate,
                    TotalPrice = totalPrice,
                    BookingStatus = bookingStatus,
                    BookingDetails = new List<BookingDetail>
                    {
                        new BookingDetail
                        {
                            RoomId = selectedRoom.RoomId,
                            StartDate = startDate,
                            EndDate = endDate,
                            ActualPrice = actualPrice
                        }
                    }
                };

                _bookingService.AddBookingReservation(newBooking);
                Booking = newBooking;
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating booking: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            Booking.CustomerId = (int)cbCustomer.SelectedValue;
            Booking.BookingDate = DateOnly.ParseExact(txtBookingDate.Text, "dd/MM/yyyy");
            Booking.TotalPrice = decimal.Parse(txtTotalPrice.Text);
            Booking.BookingStatus = (byte)cbStatus.SelectedIndex;

            if (Booking.BookingDetails.Any())
            {
                var firstDetail = Booking.BookingDetails.First();
                var selectedRoom = (RoomInformation)cbRoom.SelectedItem;
                firstDetail.RoomId = selectedRoom.RoomId;
                firstDetail.StartDate = DateOnly.ParseExact(txtStartDate.Text, "dd/MM/yyyy");
                firstDetail.EndDate = DateOnly.ParseExact(txtEndDate.Text, "dd/MM/yyyy");
                firstDetail.ActualPrice = decimal.Parse(txtActualPrice.Text);
            }

            try
            {
                _bookingService.UpdateBookingReservation(Booking);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating booking: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
