namespace ParkingManagementSystem.BL.Dto.Response
{
    public class AuditResponse : BaseResponse
    {
        public long EntityId { get; set; }

        public string TableName { get; set; }


        public string OldValues { get; set; }

        public string NewValues { get; set; }

        public string EntityState { get; set; }
    }
}
