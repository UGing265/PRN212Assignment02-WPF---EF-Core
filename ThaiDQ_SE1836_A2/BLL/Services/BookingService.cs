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

        public List<BookingDetail> GetStatisticReport(DateOnly start, DateOnly end)
        {
            return _bookingRepository.GetStatisticReport(start, end);
        }
    }
}
