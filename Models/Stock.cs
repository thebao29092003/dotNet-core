using System.ComponentModel.DataAnnotations.Schema;

namespace coreC_.Models
{
    /*
     *  ID: Khóa chính (Primary Key).
        Symbol, CompanyName, Industry: Các thông tin văn bản về mã cổ phiếu, tên công ty và ngành nghề.
        [Column(TypeName = "decimal(18,2)")]: Đây là một Data Annotation. Nó chỉ định cho cơ sở dữ liệu (SQL Server) rằng cột này phải lưu kiểu decimal với 18 chữ số, trong đó có 2 chữ số sau dấu phẩy (rất quan trọng cho dữ liệu tiền tệ/tài chính).
        Purchase, Divdend, LastDiv: Các giá trị số liên quan đến giá mua và cổ tức.
        MarketCap: Vốn hóa thị trường (kiểu long để chứa số rất lớn).
        List<Comment> Comments: Đây là Collection Navigation Property. Nó cho biết một cổ phiếu có thể có nhiều bình luận (mối quan hệ 1-nhiều).
     */
    public class Stock
    {
        public int ID { get; set; }

        // = string.Empty;: Đây là cách khởi tạo giá trị mặc định để tránh lỗi null
        // (Null Reference Exception) trong các phiên bản .NET mới (C# 8+).
        public string Symbol { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Purchase { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Divdend { get; set; }

        public decimal LastDiv {get; set;}
        public string Industry { get; set; } = string.Empty;
        public long MarketCap { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
