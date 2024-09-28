using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagementSystem.BL.Dto.Request
{
    public class ParkingSpotPriceRequest : BaseRequest
    {
        public long ParkingSpotId { get; set; }
        public decimal ParkingPrice { get; set; }
        public int ParkingMinTime { get; set; }
        public int ParkingMaxTime { get; set; }
    }
}
