namespace ParkingManagementSystem.BL.Dto.Response
{
    public class BaseResponse
    {
        public long Id { get; set; }
        public bool IsActive { get; set; }
        public long UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public long? CreatedById { get; set; }
        public DateTime UpdatedAt { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? DeletedAt { get; set; }
        public long? DeletedById { get; set; }
        public bool IsDeleted { get; set; }
    }
}
