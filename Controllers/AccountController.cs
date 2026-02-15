using coreC_.Dtos.Account;
using coreC_.Interfaces;
using coreC_.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace coreC_.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;
        public AccountController(
            UserManager<AppUser> userManager,
            ITokenService tokenService,
            SignInManager<AppUser> signInManager
        )
            {
                _userManager = userManager;
                _tokenService = tokenService;
                _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto register)
        {
            try
            {
                // khỏi tạo đối tượng
                var appUser = new AppUser
                {
                    UserName = register.Username,
                    Email = register.Email
                };

                /*
                 * Đây là bước quan trọng nhất. UserManager sẽ làm các việc:
                    Kiểm tra xem Email/Username đã tồn tại chưa.
                    Kiểm tra mật khẩu có thỏa mãn các quy tắc (độ dài, ký tự đặc biệt...) mà bạn đã cấu hình trong Program.cs không.
                    Mã hóa (Hash) mật khẩu trước khi lưu.
                    Lưu vào bảng AspNetUsers.
                 */
                var createUser = await _userManager.CreateAsync(appUser, register.Password);

                if (createUser.Succeeded)
                {
                    // Sau khi tạo User thành công, hệ thống tiếp tục gán cho họ vai trò là "User".
                    // Cái tên "User" này phải khớp chính xác với dữ liệu bạn đã Seed trong hàm OnModelCreating trước đó.
                    var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
                    if (roleResult.Succeeded)
                    {
                        // trả về username, email và token khi đăng kí thành công
                        return Ok(
                            new NewUserDto
                            {
                                Username = appUser.UserName,
                                Email = appUser.Email,
                                Token = _tokenService.CreateToken(appUser)
                            }
                        );
                    }
                    else
                    {
                        /*
                            Vấn đề: Việc tạo một tài khoản thực tế gồm 2 bước riêng biệt ở 2 bảng khác nhau trong DB (AspNetUsers và AspNetUserRoles).
                            Giải pháp: Nếu bước 1 (Tạo User) thành công, nhưng bước 2 (Gán Role) thất bại (có thể do Role "User" bị xóa nhầm trong DB), thì đoạn code sẽ thực hiện Xóa User vừa tạo.
                            Tại sao? Điều này ngăn chặn việc tạo ra các "User mồ côi" (Orphan users) — những tài khoản tồn tại nhưng không có quyền hạn, không thể đăng nhập hoặc gây lỗi cho hệ thống về sau.
                         */
                        await _userManager.DeleteAsync(appUser);
                        return StatusCode(500, roleResult.Errors);
                    }
                }
                return StatusCode(500, createUser.Errors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            /*
                 _userManager.Users: Truy cập vào danh sách người dùng trong Database.
                FirstOrDefaultAsync: Tìm người đầu tiên có UserName trùng với tên đăng nhập mà khách hàng gửi lên (loginDto.Username).
                Lưu ý: Nếu không tìm thấy, biến user sẽ mang giá trị null.
             */
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username);
            if (user == null) return Unauthorized("Invalid username or password");

            /*
                CheckPasswordSignInAsync: Đây là hàm cực kỳ quan trọng. Nó không so sánh mật khẩu kiểu chữ thường (như if (pass == "123")). Thay vào đó:
                    Nó lấy mật khẩu người dùng nhập vào.
                    Dùng thuật toán mã hóa (Hashing) để biến nó thành một chuỗi mã.
                    So sánh chuỗi mã đó với chuỗi đã lưu trong Database.
                Tham số false: Đây là lockoutOnFailure. Nếu để là true, sau một số lần nhập sai liên tiếp (thường là 5 lần), tài khoản sẽ bị khóa tạm thời. 
                Ở đây bạn đang để false (không khóa).
             */
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded) return Unauthorized("Invalid username or password");

            return Ok(
                new NewUserDto
                {
                    Username = user.UserName,
                    Email = user.Email,
                    Token = _tokenService.CreateToken(user)
                }
            );
        }
    }
}
