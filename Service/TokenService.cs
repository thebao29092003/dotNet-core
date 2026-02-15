using coreC_.Interfaces;
using coreC_.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace coreC_.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;

        // IConfiguration: Dùng để đọc các thiết lập trong file appsettings.json (như Secret Key, Issuer, Audience).
        public TokenService(IConfiguration config)
        {
            _config = config;

            // SymmetricSecurityKey: Đây là cái "khuôn đúc chìa khóa".Nó sử dụng một chuỗi bí mật(SigningKey) để mã hóa token. Chỉ có server giữ chuỗi này mới có thể tạo và kiểm tra tính hợp lệ của token.
            // Lấy chuỗi mã hóa SigningKey từ file appsettings.json và biến nó thành một dãy nhị phân (byte array)
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]));
        }
        public string CreateToken(AppUser user)
        {
            /*
                Claim: Bạn hãy tưởng tượng đây là các thông tin ghi trên một cái "thẻ căn cước". Ở đây, chúng ta đính kèm Email và Username vào bên trong token.
                Khi người dùng gửi token này ngược lại cho server, server sẽ đọc các Claim này để biết người đang gửi là ai mà không cần kiểm tra lại database.
             */
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, user.UserName)
            };

            /*
             Dòng này chọn thuật toán mã hóa. Ở đây dùng HmacSha512 (một trong những thuật toán bảo mật nhất hiện nay).
             Nó kết hợp cái "khuôn" (_key) và thuật toán để chuẩn bị ký tên lên token.
             */
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // Nội dung thẻ (Email, Name)
                Subject = new ClaimsIdentity(claims),
                // Thời hạn: 7 ngày sau sẽ hết hạn
                Expires = DateTime.Now.AddDays(7),
                // Chữ ký bảo mật
                SigningCredentials = creds,
                // Ai là người cấp (thường là tên domain của server)
                Issuer = _config["JWT:Issuer"],
                // Cấp cho ai dùng (thường là tên ứng dụng client)
                Audience = _config["JWT:Audience"] 
            };

            // Đây là "cỗ máy" thực hiện việc đúc token.
            var tokenHandler = new JwtSecurityTokenHandler();

            // tạo token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Trả về token đã được mã hóa dưới dạng chuỗi để client có thể sử dụng.
            return tokenHandler.WriteToken(token);
        }
    }
}
