using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using ParkingManagementSystem.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagementSystem.DAL.Dto
{
    public class AuditEntry
    {
        public AuditEntry(EntityEntry entry)
        {
            Entry = entry;
        }

        public EntityEntry Entry { get; }

        public long EntityId { get; set; }

        public string TableName { get; set; }

        public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();

        public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();

        public List<PropertyEntry> TemporaryProperties { get; } = new List<PropertyEntry>();

        public bool HasTemporaryProperties => TemporaryProperties.Any();

        public string EntityState { get; set; }

        public Audit ToAudit()
        {
            var audit = new Audit();
            audit.EntityId = EntityId;
            audit.TableName = TableName;
            audit.OldValues = OldValues.Count == 0 ? null : JsonConvert.SerializeObject(OldValues);
            audit.NewValues = NewValues.Count == 0 ? null : JsonConvert.SerializeObject(NewValues);
            audit.EntityState = EntityState.ToString();
            audit.IsActive = true;
            audit.CreatedAt = DateTime.UtcNow;
            audit.CreatedById = 0;
            audit.UpdatedAt = DateTime.UtcNow;
            audit.UpdatedById = 0;
            audit.IsDeleted = false;


            return audit;
        }
    }

}
