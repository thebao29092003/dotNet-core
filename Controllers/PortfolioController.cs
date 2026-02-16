using coreC_.Extentions;
using coreC_.Interfaces;
using coreC_.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace coreC_.Controllers
{
    [Route("api/portfolio")]
    public class PortfolioController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IStockRepository _stockRepository;
        private readonly IPortfolioRepository _portfolioRepository;
        public PortfolioController(
            UserManager<AppUser> userManager,
            IStockRepository stockRepository,
            IPortfolioRepository portfolioRepository
        )
            {
                _userManager = userManager;
                _stockRepository = stockRepository;
                _portfolioRepository = portfolioRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserPortfolio()
        {
            // var username = ClaimsExtensions.GetUsername(User): class.method(User)
            // cách viết này tương đương
            // cách viết dưới đây, nhưng ngắn gọn hơn rất nhiều nhờ vào Extension Method.
            // ngoại ra tại sao bên class kia là ClaimsPrincipal mà ở đây là User? Vì User là một thuộc tính có sẵn trong ControllerBase
            // nó đã được định nghĩa là kiểu ClaimsPrincipal rồi nên bạn không cần phải khai báo lại. có thể ấn f12 ở User để kiểm tra
            var username = User.GetUsername(); // User.method()

           
            //_userManager: Đây là một instance (đối tượng) của lớp UserManager<TUser>. Đây là một dịch vụ (Service) được ASP.NET Core cung cấp sẵn để quản lý người dùng (tạo, xóa, tìm kiếm, đổi mật khẩu...).
            //FindByNameAsync(username): Đây là phương thức dùng để truy vấn vào bảng người dùng (thường là bảng AspNetUsers) và tìm bản ghi nào có cột UserName khớp với giá trị của biến username truyền vào.
            var appUser = await _userManager.FindByNameAsync(username);
            var userPortfolio = await _portfolioRepository.GetUserPortfolio(appUser);
            return Ok(userPortfolio);
        }
    }
}
