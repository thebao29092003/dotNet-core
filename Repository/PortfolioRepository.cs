using coreC_.Data;
using coreC_.Interfaces;
using coreC_.Models;
using Microsoft.EntityFrameworkCore;

namespace coreC_.Repository
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly ApplicationDBContext _context;

        public PortfolioRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<List<Stock>> GetUserPortfolio(AppUser user)
        {
            // TRONG CÔNG TY CHỖ NÀY MÌNH SẼ DÙNG CHỨ KHÔNG MAP THỦ CÔNG BÊN DƯỚI
            /*
               var portfolios = await _context.Portfolios
                    .Include(p => p.Stock) // Bắt buộc phải có Include để không bị null Stock
                    .Where(p => p.AppUserId == user.Id)
                    .ToListAsync();

                // Map sau khi dữ liệu đã nằm trên RAM
                return _mapper.Map<List<Stock>>(portfolios);
             */
            /*
             * Trong LINQ, .Select() đóng vai trò là Projection (Phép chiếu). Nó quyết định dữ liệu trả về sẽ có hình dáng như thế nào.
                    Đầu vào của Select: Là một danh sách các đối tượng Portfolio (sau khi đã lọc bằng Where). Biến stock (nên đặt tên là p hoặc portfolio cho đỡ nhầm) đại diện cho từng bản ghi trong bảng trung gian đó.
                    Hành động new Stock { ... }: Với mỗi bản ghi Portfolio, bạn yêu cầu EF tạo ra một đối tượng Stock mới và "đổ" dữ liệu vào các thuộc tính của nó.
                    Truy cập dữ liệu liên kết:
                        ID = stock.StockId: Lấy ID trực tiếp từ bảng trung gian Portfolio.
                        Symbol = stock.Stock.Symbol: Đây là phần quan trọng nhất. EF Core sẽ tự động thực hiện một phép JOIN ngầm định từ bảng Portfolio sang bảng Stock để lấy giá trị Symbol.
                    Tại sao lại dùng Select ở đây?
                    Mục đích của hàm này là trả về một List<Stock>.
                    Nếu bạn chỉ dùng Where, bạn sẽ nhận được List<Portfolio>.
                    Select giúp bạn chuyển đổi (Transform) từ Portfolio sang Stock.
             */
            return await _context.Portfolios.Where(p => p.AppUserId == user.Id).Select(portfolio => new Stock
            {
                ID = portfolio.StockId,
                Symbol = portfolio.Stock.Symbol,
                CompanyName = portfolio.Stock.CompanyName,
                Purchase = portfolio.Stock.Purchase,
                Divdend = portfolio.Stock.Divdend,
                Industry = portfolio.Stock.Industry,
                MarketCap = portfolio.Stock.MarketCap,
            }).ToListAsync();
        }
    }
}
