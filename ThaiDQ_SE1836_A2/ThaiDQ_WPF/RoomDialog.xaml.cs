using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Windows;

namespace ThaiDQ_WPF
{
    public partial class RoomDialog : Window
    {
        public RoomInformation Room { get; private set; }
        public bool IsEditMode { get; set; }

        public RoomDialog(List<RoomType> roomTypes)
        {
            InitializeComponent();
            cbRoomType.ItemsSource = roomTypes;
            cbStatus.SelectedIndex = 0; // Default Active
        }

        public RoomDialog(RoomInformation room, List<RoomType> roomTypes) : this(roomTypes)
        {
            IsEditMode = true;
            Room = room;
            LoadRoomData();
        }

        private void LoadRoomData()
        {
            if (Room != null)
            {
                txtRoomNumber.Text = Room.RoomNumber;
                cbRoomType.SelectedValue = Room.RoomTypeId;
                txtCapacity.Text = Room.RoomMaxCapacity?.ToString();
                txtPrice.Text = Room.RoomPricePerDay?.ToString();
                cbStatus.SelectedIndex = Room.RoomStatus == 1 ? 0 : 1;
                txtDescription.Text = Room.RoomDetailDescription;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtRoomNumber.Text))
            {
                MessageBox.Show("Room Number is required!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cbRoomType.SelectedValue == null)
            {
                MessageBox.Show("Please select a Room Type!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCapacity.Text) || !int.TryParse(txtCapacity.Text.Trim(), out int capacity) || capacity < 1 || capacity > 10)
            {
                MessageBox.Show("Room Capacity must be between 1 and 10!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPrice.Text) || !decimal.TryParse(txtPrice.Text.Trim(), out decimal price) || price <= 0)
            {
                MessageBox.Show("Room Price must be greater than 0!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cbStatus.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a Status!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Create or update room object
            if (Room == null)
            {
                Room = new RoomInformation();
            }

            Room.RoomNumber = txtRoomNumber.Text.Trim();
            Room.RoomTypeId = (int)cbRoomType.SelectedValue;
            Room.RoomMaxCapacity = capacity;
            Room.RoomPricePerDay = price;
            Room.RoomStatus = cbStatus.SelectedIndex == 0 ? (byte)1 : (byte)0;
            Room.RoomDetailDescription = txtDescription.Text.Trim();

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
