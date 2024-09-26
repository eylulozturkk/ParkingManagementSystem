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
    public class ParkingSpot : BaseEntity
    {
        [Column(TypeName = "varchar(127)")]
        public string Name { get; set; }

        public VehicleSizeType Size { get; set; }

        public int MaxCapacity { get; set; }


        #region Navigation Properties

        [ForeignKey("ParkingSpotId")]
        public virtual ICollection<VehicleParkingSpotMapping> VehicleParkingSpotMappings { get; set; }

        [ForeignKey("ParkingSpotId")]
        public virtual ICollection<PriceParkingSpotMapping> PriceParkingSpotMappings { get; set; }
        

        #endregion

    }
}
