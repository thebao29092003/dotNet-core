namespace coreC_.Helpers
{
    public class QueryObject
    {
        public string? Symbol { get; set; } = null;
        public string? CompanyName { get; set; } = null;
        public string? SortBy { get; set; } = null; // "Symbol" hoặc "CompanyName"
        public bool IsDescending { get; set; } = false; // Mặc định là tăng dần
        public int PageNumber { get; set; } = 1; // Trang hiện tại, mặc định là trang 1
        public int PageSize { get; set; } = 10; // Số bản ghi trên mỗi trang, mặc định là 2
    }
}
