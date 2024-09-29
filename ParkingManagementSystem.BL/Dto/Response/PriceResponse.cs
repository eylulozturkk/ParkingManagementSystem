using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagementSystem.BL.Dto.Response
{
    public class PriceResponse : SuccessResponse
    {
        public decimal ParkingPrice { get; set; }
        public double ParkingTime { get; set; }
        public DateTime EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }
    }
}
