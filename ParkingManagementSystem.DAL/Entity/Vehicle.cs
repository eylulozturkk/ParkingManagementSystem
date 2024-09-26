using Castle.MicroKernel.SubSystems.Conversion;
using ParkingManagementSystem.DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagementSystem.DAL.Entity
{
    public class Vehicle : BaseEntity
    {
        [Column(TypeName = "varchar(127)")]
        public string LicensePlate { get; set; }
        public VehicleSizeType Size { get; set; }
        public DateTime EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }
        public decimal TotalParkingFee { get; set; }

        #region Navigation Properties

        [ForeignKey("VehicleId")]
        public virtual ICollection<VehicleParkingSpotMapping> VehicleParkingSpotMappings { get; set; }

        #endregion

    }
}
