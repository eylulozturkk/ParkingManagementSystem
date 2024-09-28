using ParkingManagementSystem.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagementSystem.BL.Dto.Request
{
    public class ParkingSpotRequest : BaseRequest
    {
        public string Name { get; set; }

        public VehicleSizeType Size { get; set; }

        public int MaxCapacity { get; set; }
    }
}
