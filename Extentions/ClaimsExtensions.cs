using System.Security.Claims;

namespace coreC_.Extentions
{
    /*
        1. Tại sao phải dùng static?
        Trong C#, static (tĩnh) có nghĩa là "thuộc về chính lớp đó, chứ không thuộc về một đối tượng cụ thể nào".
            Không cần new: Nếu class là static, bạn không bao giờ có thể ghi var x = new ClaimsExtensions();. Nó luôn tồn tại sẵn trong bộ nhớ ngay khi ứng dụng chạy.
            Tiết kiệm bộ nhớ: Vì đây chỉ là một hàm bổ trợ (helper), bạn không cần tạo ra hàng nghìn đối tượng ClaimsExtensions làm gì cho tốn ram. Bạn chỉ cần cái "hành động" của nó thôi.
            Quy định của ngôn ngữ: Đây là quy tắc bắt buộc của C#: Để tạo một phương thức mở rộng (Extension Method), cả Class và Method đó đều phải là static.
     */
    public static class ClaimsExtensions
    {
        /*
            this ClaimsPrincipal user: Từ khóa this đặt trước tham số đầu tiên biến hàm này thành một Extension Method.
                Ý nghĩa: Nó cho phép bạn gọi hàm GetUsername() trực tiếp từ đối tượng User trong Controller (ví dụ: User.GetUsername()) thay vì phải truyền đối tượng đó vào một hàm thông thường.
                ClaimsPrincipal chính là kiểu dữ liệu của thuộc tính User có sẵn trong các Controller.
         */
        public static string GetUsername(this ClaimsPrincipal user)
        {
            /*
             *  user.Claims: Một người dùng sau khi đăng nhập sẽ có một danh sách các "yêu cầu" (Claims) - giống như các dòng thông tin trên thẻ căn cước (Tên, Email, Ngày sinh, Role...).
                FirstOrDefault: Hàm này sẽ duyệt qua danh sách Claims đó:
                    Nó tìm cái Claim đầu tiên thỏa mãn điều kiện trong ngoặc.
                    Nếu tìm thấy: Trả về đối tượng Claim.
                    Nếu không tìm thấy: Trả về null.
                Điều kiện tìm kiếm: c => c.Type == ClaimTypes.GivenName
                    Ở đây, bạn đang bảo hệ thống: "Hãy tìm trong danh sách thông tin của người dùng này, cái nào có Loại (Type) là GivenName".
                    Lưu ý: Trong đoạn code TokenService trước đó bạn đã viết:
                    new Claim(JwtRegisteredClaimNames.GivenName, user.UserName)
                    Khi Token được giải mã bởi middleware của ASP.NET Core, JwtRegisteredClaimNames.GivenName thường được ánh xạ (map) vào hằng số chuẩn ClaimTypes.GivenName. 
                    Vì vậy, hai bên sẽ khớp nhau.
                Nếu FirstOrDefault trả về một Claim (khác null), nó sẽ lấy thuộc tính .Value
             */
            return user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value ?? string.Empty;
        }
    }
}
