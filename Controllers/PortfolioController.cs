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
            // trả về 1 list stock theo 1 user nào đó
            var userPortfolio = await _portfolioRepository.GetUserPortfolio(appUser);
            return Ok(userPortfolio);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPortfolio([FromQuery] string symbol)
        {
            try
            {
                // không cần check null vì đã có [Authorize] rồi, nếu không có token thì sẽ không vào được đây
                var username = User.GetUsername();
                var appUser = await _userManager.FindByNameAsync(username);

                var stock = await _stockRepository.GetStockBySymbol(symbol);
                if (stock == null) return NotFound("Stock not found");

                // trả về 1 list stock theo 1 user nào đó, để kiểm tra xem user đã có stock này trong portfolio chưa,
                // nếu có rồi thì báo lỗi và không cho thêm nữa
                var userPortfolio = await _portfolioRepository.GetUserPortfolio(appUser);

                if (userPortfolio.Any((stock => stock.Symbol == symbol.ToUpper()))) 
                    return BadRequest("Stock already in portfolio");

                var portfolio = new Portfolio
                {
                    AppUserId = appUser.Id,
                    StockId = stock.ID
                };

                await _portfolioRepository.createPortfolio(portfolio);

                if (portfolio == null) return BadRequest("Failed to create portfolio");
                return Created();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeletePortfolio([FromQuery] string symbol)
        {
            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);

            var userPortfolio = await _portfolioRepository.GetUserPortfolio(appUser);

            var filteredStock = userPortfolio.FirstOrDefault(stock => stock.Symbol == symbol.ToUpper());
            if (filteredStock == null) return NotFound("Stock not found in portfolio");

            var portfolioModel = await _portfolioRepository.DeletePortfolio(appUser, filteredStock);
            if(portfolioModel == null) return BadRequest("Failed to delete portfolio");
            return Ok("Xóa thanh công");
        }
    }
}
