using Microsoft.EntityFrameworkCore;
using ParkingManagementSystem.DAL.Context;
using ParkingManagementSystem.DAL.Dto;
using ParkingManagementSystem.DAL.Entity;
using ParkingManagementSystem.DAL.GenericRepository;

namespace ParkingManagementSystem.DAL.UOW
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly DataContext context;
        public UnitOfWork(DataContext dbContext)
        {
            context = dbContext;
        }

        public IGenericRepository<T> GetRepository<T>() where T : BaseEntity
        {
            return new GenericRepository<T>(context);
        }

        public int SaveChanges(bool useAudit = false)
        {
            if (!useAudit) return context.SaveChanges();

            var auditEntries = OnBeforeSaveChanges();
            var result = context.SaveChanges();
            OnAfterSaveChanges(auditEntries);

            return result;
        }

        public async Task<int> SaveChangesAsync(bool useAudit = false)
        {
            if (!useAudit) return await context.SaveChangesAsync();

            var auditEntries = OnBeforeSaveChanges();
            var result = context.SaveChangesAsync();
            await OnAfterSaveChangesAsync(auditEntries);

            return await result;
        }


        #region Audit

        private List<AuditEntry> OnBeforeSaveChanges()
        {
            var auditRepo = new GenericRepository<Audit>(context);
            var auditEntries = new List<AuditEntry>();
            var entries = context.ChangeTracker.Entries();

            foreach (var entry in entries)
            {
                if (entry.Entity is Audit || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                var auditEntry = SetAuditEntryPtoperties(entry);

                auditEntries.Add(auditEntry);
            }

            foreach (var auditEntry in auditEntries.Where(_ => !_.HasTemporaryProperties))
            {
                auditRepo.Add(auditEntry.ToAudit());
            }

            return auditEntries.Where(_ => _.HasTemporaryProperties).ToList();
        }

        private static AuditEntry SetAuditEntryPtoperties(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
        {
            var auditEntry = new AuditEntry(entry)
            {
                TableName = entry.Metadata.GetTableName(),
                EntityState = entry.State.ToString()
            };

            foreach (var property in entry.Properties)
            {
                if (property.IsTemporary)
                {
                    auditEntry.TemporaryProperties.Add(property);
                    continue;
                }

                string propertyName = property.Metadata.Name;

                if (property.Metadata.IsPrimaryKey())
                {
                    auditEntry.EntityId = Convert.ToInt64(property.CurrentValue);
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.NewValues[propertyName] = property.CurrentValue;
                        break;

                    case EntityState.Deleted:
                        auditEntry.OldValues[propertyName] = property.OriginalValue;
                        break;

                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                        }
                        break;
                }
            }

            return auditEntry;
        }

        private void OnAfterSaveChanges(List<AuditEntry> auditEntries)
        {
            if (auditEntries == null || auditEntries.Count == 0)
                return;

            var auditRepo = new GenericRepository<Audit>(context);

            foreach (var auditEntry in auditEntries)
            {
                foreach (var prop in auditEntry.TemporaryProperties)
                {
                    if (prop.Metadata.IsPrimaryKey())
                    {
                        auditEntry.EntityId = Convert.ToInt64(prop.CurrentValue);
                    }
                    else
                    {
                        auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                }

                auditRepo.Add(auditEntry.ToAudit());
            }

            SaveChanges();
        }

        private async Task<int> OnAfterSaveChangesAsync(List<AuditEntry> auditEntries)
        {
            if (auditEntries == null || auditEntries.Count == 0)
                return 0;

            var auditRepo = new GenericRepository<Audit>(context);

            foreach (var auditEntry in auditEntries)
            {
                foreach (var prop in auditEntry.TemporaryProperties)
                {
                    if (prop.Metadata.IsPrimaryKey())
                    {
                        auditEntry.EntityId = Convert.ToInt64(prop.CurrentValue);
                    }
                    else
                    {
                        auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                }

                await auditRepo.AddAsync(auditEntry.ToAudit());
            }

            return await context.SaveChangesAsync();
        }

        #endregion

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed && disposing && context != null)
            {
                context.Dispose();
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
