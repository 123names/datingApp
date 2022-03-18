namespace api.Helpers
{
    public class PaginationParams
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int displayPageSize = 10;
        public int PageSize
        {
            get => displayPageSize;
            set => displayPageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}