using coreC_.Dtos.Account;
using coreC_.Interfaces;
using coreC_.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace coreC_.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
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
    }
}
