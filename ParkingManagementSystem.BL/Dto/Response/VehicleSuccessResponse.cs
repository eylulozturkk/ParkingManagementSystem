using ParkingManagementSystem.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagementSystem.BL.Dto.Response
{
    public class VehicleSuccessResponse : SuccessResponse
    {
        public VehicleResponse VehicleResponse { get; set; }
    }
}
