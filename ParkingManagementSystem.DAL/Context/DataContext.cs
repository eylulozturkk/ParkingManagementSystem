using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;
using ParkingManagementSystem.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
            modelBuilder.Entity<Setting>().HasIndex(x => x.Name).IsUnique();
            modelBuilder.Entity<Template>().HasIndex(x => x.TemplateTypeId).IsUnique();

        }

        #region DbSets

        public DbSet<Provider> Provider { get; set; }
        public DbSet<Template> Template { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Audit> Audits { get; set; }

        #endregion

    }

}
