using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class BookingRepository
    {
        private FuminiHotelManagementContext _db;
        public BookingRepository()
        {
            _db = new FuminiHotelManagementContext();
        }
        public List<BookingReservation> GetBookingReservations()
        {
            return _db.BookingReservations.Include(x => x.Customer)
                .Include(x => x.BookingDetails)
                    .ThenInclude(x => x.Room)
                    .ThenInclude(x => x.RoomType)
                .ToList();
        }

        public List<BookingReservation> GetBookingReservationsByCustomerId(int customerId)
        {
            return _db.BookingReservations
                .Include(x => x.Customer)
                .Include(x => x.BookingDetails)
                    .ThenInclude(x => x.Room)
                    .ThenInclude(x => x.RoomType)
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.BookingDate)
                .ToList();
        }

        public List<BookingDetail> GetStatisticReport(DateOnly start, DateOnly end)
        {
            return _db.BookingDetails
                .Include(x => x.BookingReservation)
                    .ThenInclude(br => br.Customer)
                .Where(x => x.StartDate >= start && x.EndDate <= end)
                .OrderByDescending(x => x.StartDate)
                .ToList();
        }

        public void AddBookingReservation(BookingReservation booking)
        {
            _db.BookingReservations.Add(booking);
            _db.SaveChanges();
        }

        public void UpdateBookingReservation(BookingReservation booking)
        {
            _db.BookingReservations.Update(booking);
            _db.SaveChanges();
        }

        public void DeleteBookingReservation(int bookingId)
        {
            var booking = _db.BookingReservations.Find(bookingId);
            if (booking != null)
            {
                _db.BookingReservations.Remove(booking);
                _db.SaveChanges();
            }
        }

        public List<Customer> GetCustomers()
        {
            return _db.Customers.ToList();
        }

        public List<RoomInformation> GetAvailableRooms()
        {
            return _db.RoomInformations
                .Include(r => r.RoomType)
                .Where(r => r.RoomStatus == 1)
                .ToList();
        }
    }
}
