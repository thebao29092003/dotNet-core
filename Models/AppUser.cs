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
    }
}
