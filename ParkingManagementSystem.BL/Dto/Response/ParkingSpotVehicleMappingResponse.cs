using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagementSystem.BL.Dto.Response
{
    public class ParkingSpotVehicleMappingResponse : BaseResponse 
    {
        public long VehicleId { get; set; }
        public long ParkingSpotId { get; set; }
    }
}
