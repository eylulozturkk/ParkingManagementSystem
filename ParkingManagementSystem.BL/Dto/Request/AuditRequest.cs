using Microsoft.EntityFrameworkCore;
namespace ParkingManagementSystem.BL.Dto.Request
{
    public class AuditRequest : BaseFilter
    {
        public long EntityId { get; set; }

        public string TableName { get; set; }

        public EntityState? EntityState { get; set; }

        public DateTime? StartCreatedDate { get; set; }

        public DateTime? EndCreatedDate { get; set; }
    }
}
