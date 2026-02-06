namespace coreC_.Models
{
    /*
        Id: Khóa chính.
        Title, Content: Tiêu đề và nội dung của bình luận.
        CreatedOn: Thời điểm tạo bình luận, mặc định lấy giờ hiện tại.
        StockId: Đây là Foreign Key (Khóa ngoại). Nó lưu ID của cổ phiếu mà bình luận này thuộc về. Việc để int? (nullable) có nghĩa là một bình luận có thể không thuộc về cổ phiếu nào (tùy vào logic nghiệp vụ của bạn).
        Stock? Stock: Đây là Navigation Property. Nó cho phép bạn truy cập trực tiếp các thông tin của Stock từ một đối tượng Comment.
     */
    public class Comment
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public int? StockId { get; set; }

        //Navigation Property: Những thuộc tính như public Stock? Stock không được lưu thành một cột dữ liệu thô trong bảng Comment,
        //mà nó là một "đường dẫn" để Entity Framework tự động thực hiện các câu lệnh JOIN trong SQL nhằm lấy dữ liệu liên quan.
        public Stock? Stock { get; set; }
    }
}
