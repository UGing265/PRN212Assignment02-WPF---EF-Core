using DAL.Entities;
using DAL.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class BookingService
    {
        private BookingRepository _bookingRepository;
        public BookingService()
        {
            _bookingRepository = new BookingRepository();
        }

        public List<BookingReservation> GetBookingReservations()
        {
            return _bookingRepository.GetBookingReservations();
        }

        public List<BookingReservation> GetBookingReservationsByCustomerId(int customerId)
        {
            return _bookingRepository.GetBookingReservationsByCustomerId(customerId);
        }

        public List<BookingDetail> GetStatisticReport(DateOnly start, DateOnly end)
        {
            return _bookingRepository.GetStatisticReport(start, end);
        }

        public void AddBookingReservation(BookingReservation booking)
        {
            _bookingRepository.AddBookingReservation(booking);
        }

        public void UpdateBookingReservation(BookingReservation booking)
        {
            _bookingRepository.UpdateBookingReservation(booking);
        }

        public void DeleteBookingReservation(int bookingId)
        {
            _bookingRepository.DeleteBookingReservation(bookingId);
        }

        public List<Customer> GetCustomers()
        {
            return _bookingRepository.GetCustomers();
        }

        public List<RoomInformation> GetAvailableRooms()
        {
            return _bookingRepository.GetAvailableRooms();
        }
    }
}
