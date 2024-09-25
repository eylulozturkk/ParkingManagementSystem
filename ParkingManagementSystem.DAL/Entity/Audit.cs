using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagementSystem.DAL.Entity
{
    public class Audit : BaseEntity
    {
        public long EntityId { get; set; }

        public string TableName { get; set; }

        public string OldValues { get; set; }

        public string NewValues { get; set; }

        public string EntityState { get; set; }
    }
}
