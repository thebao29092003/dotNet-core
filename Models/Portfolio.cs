using System.ComponentModel.DataAnnotations.Schema;

namespace coreC_.Models
{

    // này là bảng trung gian để lưu thông tin về những cổ phiếu
    // mà người dùng đã mua
    [Table("Portfolios")]
    public class Portfolio
    {
        public string AppUserId { get; set; }
        public int StockId { get; set; }
        public AppUser AppUser { get; set; }
        public Stock Stock { get; set; }
    }
}
