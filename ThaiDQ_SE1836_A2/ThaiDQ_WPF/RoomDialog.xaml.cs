using BLL.Services;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ThaiDQ_WPF
{
    public partial class RoomDialog : Window
    {
        private readonly RoomService _roomService;
        private readonly List<RoomInformation> _allRooms;
        public RoomInformation Room { get; private set; }
        public bool IsCreateMode { get; set; }

        // Constructor: null room = Create, non-null = Update
        public RoomDialog(RoomInformation? room, List<RoomType> roomTypes, List<RoomInformation> allRooms)
        {
            InitializeComponent();
            _roomService = new RoomService();
            _allRooms = allRooms;
            cbRoomType.ItemsSource = roomTypes;
            cbStatus.SelectedIndex = 0; // Default Active

            if (room == null)
            {
                // Create mode
                IsCreateMode = true;
            }
            else
            {
                // Update mode
                IsCreateMode = false;
                Room = room;
                LoadRoomData();
            }
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

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtRoomNumber.Text))
            {
                MessageBox.Show("Room Number is required!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (cbRoomType.SelectedValue == null)
            {
                MessageBox.Show("Please select a Room Type!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtCapacity.Text) || !int.TryParse(txtCapacity.Text.Trim(), out int capacity) || capacity < 1 || capacity > 10)
            {
                MessageBox.Show("Room Capacity must be between 1 and 10!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPrice.Text) || !decimal.TryParse(txtPrice.Text.Trim(), out decimal price) || price <= 0)
            {
                MessageBox.Show("Room Price must be greater than 0!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (cbStatus.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a Status!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            // Check duplicate room number
            if (_allRooms.Any(r => r.RoomNumber.Equals(txtRoomNumber.Text.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Room number already exists!", "Duplicate", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int.TryParse(txtCapacity.Text.Trim(), out int capacity);
            decimal.TryParse(txtPrice.Text.Trim(), out decimal price);

            Room = new RoomInformation
            {
                RoomNumber = txtRoomNumber.Text.Trim(),
                RoomTypeId = (int)cbRoomType.SelectedValue,
                RoomMaxCapacity = capacity,
                RoomPricePerDay = price,
                RoomStatus = cbStatus.SelectedIndex == 0 ? (byte)1 : (byte)0,
                RoomDetailDescription = txtDescription.Text.Trim()
            };

            _roomService.AddRoom(Room);
            DialogResult = true;
            Close();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            // Check duplicate room number (excluding current room)
            if (_allRooms.Any(r => r.RoomId != Room.RoomId && 
                r.RoomNumber.Equals(txtRoomNumber.Text.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Room number already exists!", "Duplicate", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int.TryParse(txtCapacity.Text.Trim(), out int capacity);
            decimal.TryParse(txtPrice.Text.Trim(), out decimal price);

            Room.RoomNumber = txtRoomNumber.Text.Trim();
            Room.RoomTypeId = (int)cbRoomType.SelectedValue;
            Room.RoomMaxCapacity = capacity;
            Room.RoomPricePerDay = price;
            Room.RoomStatus = cbStatus.SelectedIndex == 0 ? (byte)1 : (byte)0;
            Room.RoomDetailDescription = txtDescription.Text.Trim();

            _roomService.UpdateRoom(Room);
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
