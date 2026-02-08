using coreC_.Data;
using coreC_.Interfaces;
using coreC_.Models;
using Microsoft.EntityFrameworkCore;

namespace coreC_.Repository
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDBContext _context;

        // Đây chính là Dependency Injection (DI) – cụ thể là kiểu Constructor Injection (Tiêm qua hàm khởi tạo).
        public StockRepository(ApplicationDBContext context)
        {
            _context = context;
        }
        public Task<List<Stock>> GetAllStocksAsync()
        {
            // nếu đơn giản chỉ lấy dữ liệu từ database về mà không cần theo dõi thay đổi
            // thì nên dùng AsNoTracking() để tăng hiệu suất
            return _context.Stocks.AsNoTracking().ToListAsync();
        }
    }
}
