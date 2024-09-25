using ParkingManagementSystem.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagementSystem.DAL.GenericRepository
{
    public interface IGenericRepository<T> : IDisposable where T : BaseEntity
    {
        T Add(T entity);

        Task<T> AddAsync(T entity);

        int Count();

        Task<int> CountAsync();

        void Delete(T entity);

        Task<int> DeleteAsync(T entity);

        T Find(Expression<Func<T, bool>> predicate);

        ICollection<T> FindAll(Expression<Func<T, bool>> predicate);

        Task<ICollection<T>> FindAllAsync(Expression<Func<T, bool>> predicate);

        Task<T> FindAsync(Expression<Func<T, bool>> predicate);

        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);

        Task<ICollection<T>> FindByAsync(Expression<Func<T, bool>> predicate);

        T Get(long id);

        T Get(long id, params Expression<Func<T, object>>[] includeProperties);

        IQueryable<T> GetAll();

        IQueryable<T> GetAll(params Expression<Func<T, object>>[] includeProperties);

        Task<ICollection<T>> GetAllAsync();

        Task<ICollection<T>> GetAllAsNoTrackingAsync();

        Task<T> GetAsync(long id);

        T Update(T entity);

        Task<T> UpdateAsync(T entity);
    }

}
