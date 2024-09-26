using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagementSystem.DAL.Entity
{
    public class VehicleParkingSpotMapping : BaseEntity
    {

        public long VehicleId { get; set; }
        public long ParkingSpotId { get; set; }
    
    }
}
