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

        public List<BookingDetail> GetStatisticReport(DateOnly start, DateOnly end)
        {
            return _db.BookingDetails
                .Include(x => x.BookingReservation)
                    .ThenInclude(br => br.Customer)
                .Where(x => x.StartDate >= start && x.EndDate <= end)
                .OrderByDescending(x => x.StartDate)
                .ToList();
        }
    }
}
