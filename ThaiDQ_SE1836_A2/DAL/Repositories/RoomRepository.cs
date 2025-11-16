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

        public List<RoomType> GetRoomTypes()
        {
            return _db.RoomTypes.ToList();
        }

        public RoomInformation? GetRoomById(int roomId)
        {
            return _db.RoomInformations.Include(x => x.RoomType).FirstOrDefault(r => r.RoomId == roomId);
        }

        public void AddRoom(RoomInformation room)
        {
            _db.RoomInformations.Add(room);
            _db.SaveChanges();
        }

        public void UpdateRoom(RoomInformation room)
        {
            _db.RoomInformations.Update(room);
            _db.SaveChanges();
        }

        public void DeleteRoom(int roomId)
        {
            var room = _db.RoomInformations.FirstOrDefault(r => r.RoomId == roomId);
            if (room != null)
            {
                _db.RoomInformations.Remove(room);
                _db.SaveChanges();
            }
        }
    }
}
