namespace ParkingManagementSystem.BL.Dto.Response
{
    public class PageListResponse<T>
    {
        public int Index { get; set; }

        public int Size { get; set; }

        public int TotalCount { get; set; }

        public T Data { get; set; }

        public bool HasPreviousPage => Index > 1;

        public bool HasNextPage => Index < TotalPages;

        public int TotalPages => TotalCount % Size == 0 ? TotalCount / Size : TotalCount / Size + 1;
    }
}
