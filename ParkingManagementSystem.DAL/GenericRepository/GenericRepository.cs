using Microsoft.EntityFrameworkCore;
using ParkingManagementSystem.DAL.Context;
using ParkingManagementSystem.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagementSystem.DAL.GenericRepository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected DataContext _context;

        public GenericRepository(DataContext context)
        {
            _context = context;
        }

        public IQueryable<T> GetAll()
        {
            return _context.Set<T>();
        }

        public IQueryable<T> GetAll(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> queryable = GetAll();
            foreach (Expression<Func<T, object>> includeProperty in includeProperties)
                queryable = queryable.Include<T, object>(includeProperty);

            return queryable;
        }

        public virtual async Task<ICollection<T>> GetAllAsync()
        {

            return await _context.Set<T>().ToListAsync();
        }

        public virtual async Task<ICollection<T>> GetAllAsNoTrackingAsync()
        {

            return await _context.Set<T>().AsNoTracking().ToListAsync();
        }

        public virtual T Get(long id)
        {
            return _context.Set<T>().FirstOrDefault(t => t.Id == id);
        }

        public virtual T Get(long id, params Expression<Func<T, object>>[] includeProperties)
        {
            var res = true;
            T item = null;

            while (res)
            {
                try
                {
                    var queryable = _context.Set<T>().AsQueryable();

                    foreach (Expression<Func<T, object>> includeProperty in includeProperties)
                        queryable = queryable.Include<T, object>(includeProperty);
                    res = false;

                    item = queryable.FirstOrDefault(t => t.Id == id);
                }
                catch (Exception)
                {
                    res = true;
                    item = null;
                }
            }
            return item;
        }

        public virtual async Task<T> GetAsync(long id)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(t => t.Id == id);
        }

        public virtual T Add(T entity)
        {
            _context.Set<T>().Add(entity);
            return entity;
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return entity;
        }

        public virtual T Find(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().SingleOrDefault(predicate);
        }

        public virtual async Task<T> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().SingleOrDefaultAsync(predicate);
        }

        public ICollection<T> FindAll(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Where(predicate).ToList();
        }

        public async Task<ICollection<T>> FindAllAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public virtual void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public virtual async Task<int> DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            return await _context.SaveChangesAsync();
        }

        public virtual T Update(T entity)
        {
            if (entity == null)
                return null;

            T exist = _context.Set<T>().Find(entity.Id);
            if (exist != null)
            {
                entity.CreatedById = exist.CreatedById;
                entity.CreatedAt = exist.CreatedAt;

                _context.Entry(exist).CurrentValues.SetValues(entity);

            }
            return exist;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            if (entity == null)
                return null;

            T exist = await _context.Set<T>().FindAsync(entity.Id);
            if (exist != null)
            {

                entity.CreatedById = exist.CreatedById;
                entity.CreatedAt = exist.CreatedAt;

                _context.Entry(exist).CurrentValues.SetValues(entity);
            }
            return exist;
        }

        public int Count()
        {
            return _context.Set<T>().Count();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Set<T>().CountAsync();
        }

        public virtual IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = _context.Set<T>().Where(predicate);
            return query;
        }

        public virtual async Task<ICollection<T>> FindByAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                    _context.Dispose();

                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

}
