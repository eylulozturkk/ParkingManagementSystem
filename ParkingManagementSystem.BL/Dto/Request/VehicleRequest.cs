using ParkingManagementSystem.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagementSystem.BL.Dto.Request
{
    public class VehicleRequest : BaseRequest
    {
        public string LicensePlate { get; set; }
        public VehicleSizeType Size { get; set; }
        public DateTime EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }
        public decimal TotalParkingFee { get; set; }
    }
}
