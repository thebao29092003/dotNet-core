using AutoMapper;
using coreC_.Data;
using coreC_.Dtos.Stock;
using coreC_.Interfaces;
using coreC_.Models;
using Microsoft.EntityFrameworkCore;

namespace coreC_.Repository
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly IMapper _mapper;


        // Đây chính là Dependency Injection (DI) – cụ thể là kiểu Constructor Injection (Tiêm qua hàm khởi tạo).
        public StockRepository(ApplicationDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<Stock>> GetAllStocksAsync()
        {
            // nếu đơn giản chỉ lấy dữ liệu từ database về mà không cần theo dõi thay đổi
            // thì nên dùng AsNoTracking() để tăng hiệu suất
            return await _context.Stocks.AsNoTracking().ToListAsync();

        }

        public async Task<Stock?> GetStockByIdAsync(int id)
        {
            // find tìm theo khóa chính
            // còn firstordefault tìm theo điều kiện (bất kỳ cột nào)
            return await _context.Stocks.FindAsync(id);
        }

        public async Task<Stock> CreateStockAsync(Stock stock)
        {
            //// 2. Thêm đối tượng vào bộ theo dõi của Entity Framework
            //await _context.Stocks.AddAsync(stockModel);
            await _context.Stocks.AddAsync(stock);

            //// 3. Thực thi lưu vào Database (Lúc này lệnh INSERT trong SQL mới chạy)
            //await _context.SaveChangesAsync();
            await _context.SaveChangesAsync();
            return stock;


        }

        public async Task<Stock?> UpdateStockAsync(int id, UpdateStockRequestDto updateStock)
        {
            var existStock = await _context.Stocks.FirstOrDefaultAsync(s => s.ID == id);
            if (existStock == null)
            {
                return null;
            }

            // ĐÚNG: Map từ DTO (nguồn) vào Entity (đích)
            _mapper.Map(updateStock, existStock);
            await _context.SaveChangesAsync();

            return existStock;
        }

        public async Task<Stock?> DeleteStockAsync(int id)
        {

            var stockModel = await _context.Stocks.FirstOrDefaultAsync(s => s.ID == id);
            if (stockModel == null)
            {
                return null;
            }
            _context.Stocks.Remove(stockModel);
            await _context.SaveChangesAsync();
            return stockModel;
        }




    }
}
