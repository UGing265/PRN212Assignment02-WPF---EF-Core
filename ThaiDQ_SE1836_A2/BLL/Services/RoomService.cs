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
    }
}
