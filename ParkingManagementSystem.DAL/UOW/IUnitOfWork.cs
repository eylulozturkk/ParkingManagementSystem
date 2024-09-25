using ParkingManagementSystem.DAL.Entity;
using ParkingManagementSystem.DAL.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagementSystem.DAL.UOW
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> GetRepository<T>() where T : BaseEntity;
        int SaveChanges(bool useAudit = false);
        Task<int> SaveChangesAsync(bool useAudit = false);
    }
}
