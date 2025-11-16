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
    public class RoomService
    {
        private RoomRepository _roomRepository;
        public RoomService()
        {
            _roomRepository = new RoomRepository();
        }

        public List<RoomInformation> GetRooms()
        {
            return _roomRepository.GetRooms();
        }

        public List<RoomType> GetRoomTypes()
        {
            return _roomRepository.GetRoomTypes();
        }

        public RoomInformation? GetRoomById(int roomId)
        {
            return _roomRepository.GetRoomById(roomId);
        }

        public void AddRoom(RoomInformation room)
        {
            _roomRepository.AddRoom(room);
        }

        public void UpdateRoom(RoomInformation room)
        {
            _roomRepository.UpdateRoom(room);
        }

        public void DeleteRoom(int roomId)
        {
            _roomRepository.DeleteRoom(roomId);
        }
    }
}
