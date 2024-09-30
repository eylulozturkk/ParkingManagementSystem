using Microsoft.EntityFrameworkCore;
using ParkingManagementSystem.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagementSystem.DAL.Context
{
    public interface IDataContext
    {
        DbSet<Vehicle> Vehicles { get; set; }
        DbSet<ParkingSpot> ParkingSpots { get; set; }
        DbSet<PriceParkingSpotMapping> PriceParkingSpotMappings { get; set; }
        DbSet<VehicleParkingSpotMapping> VehicleParkingSpotMappings { get; set; }
        DbSet<Log> Logs { get; set; }


    }
}
