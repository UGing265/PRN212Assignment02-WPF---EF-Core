using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class RoomRepository
    {
        private FuminiHotelManagementContext _db;
        public RoomRepository()
        {
            _db = new FuminiHotelManagementContext();
        }
        public List<RoomInformation> GetRooms()
        {
            return _db.RoomInformations.Include(x => x.RoomType).ToList();
        }
    }
}
