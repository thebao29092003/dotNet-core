using Microsoft.AspNetCore.Identity;

namespace coreC_.Models
{

    /*
     Giải thích: IdentityUser là lớp có sẵn của Microsoft chứa các thông tin cơ bản: Id, Username, Email, PasswordHash, PhoneNumber...
     Tại sao cần lớp này? Việc tạo AppUser kế thừa từ IdentityUser giúp bạn 
      có thể mở rộng sau này. Ví dụ: bạn muốn thêm cột Bio hay Address, bạn chỉ cần khai báo vào đây.
     */
    public class AppUser: IdentityUser
    {
        // Đây là mối quan hệ 1-n giữa AppUser và Portfolio. Một người dùng có thể sở hữu nhiều cổ phiếu khác nhau.
        public List<Portfolio> Portfolios { get; set; } = new List<Portfolio>();
    }
}
