using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;
using ParkingManagementSystem.DAL.Entity;
using System.Reflection;

namespace ParkingManagementSystem.DAL.Context
{
    public class DataContext : DbContext, IDataContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public void SetGlobalQuery<T>(ModelBuilder builder) where T : BaseEntity
        {
            builder.Entity<T>().HasQueryFilter(e => e.IsDeleted == false);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties().Where(x => x.PropertyInfo?.PropertyType == typeof(bool) || x.PropertyInfo?.PropertyType == typeof(bool?)))
                    property.SetValueConverter(new BoolToZeroOneConverter<int>());
                if (typeof(BaseEntity).IsAssignableFrom(entity.ClrType))
                {
                    var method = typeof(DataContext)
                        .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                        .Single(t => t.IsGenericMethod && t.Name == "SetGlobalQuery")
                        .MakeGenericMethod(entity.ClrType);
                    method.Invoke(this, new object[] { modelBuilder });
                }
            }

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ParkingSpot>().HasIndex(x => x.Name).IsUnique();
        }

        #region DbSets

        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<ParkingSpot> ParkingSpots { get; set; }
        public DbSet<VehicleParkingSpotMapping> VehicleParkingSpotMappings { get; set; }
        public DbSet<PriceParkingSpotMapping> PriceParkingSpotMappings { get; set; }
        public DbSet<Log> Logs { get; set; }


        #endregion

    }

}
