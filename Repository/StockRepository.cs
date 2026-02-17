using AutoMapper;
using coreC_.Data;
using coreC_.Dtos.Stock;
using coreC_.Helpers;
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

        public Task<List<Stock>> GetAllStocksCommentAsync()
        {
            return _context.Stocks.Include(s => s.Comments).ToListAsync();
        }

        public async Task<Stock?> GetStockByIdAsync(int id)
        {
            // find tìm theo khóa chính
            // còn firstordefault tìm theo điều kiện (bất kỳ cột nào)
            return await _context.Stocks.FindAsync(id);
        }

        public async Task<Stock?> GetStockCommentByIdAsync(int id)
        {
            // find tìm theo khóa chính của Stock kèm comments
            return await _context.Stocks
                .Include(s => s.Comments)
                .FirstOrDefaultAsync(s => s.ID == id);
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

        public async Task<bool> StockExistsAsync(int id)
        {
            /*
             * _context.Stocks: Truy cập vào bảng Stocks trong Database.
                AnyAsync(...):
                    Any: Có nghĩa là "có bất kỳ cái nào không?". Nó sẽ trả về kiểu boolean (true nếu tìm thấy ít nhất một bản ghi thỏa mãn điều kiện, false nếu không có cái nào).
                    Async: Đây là phiên bản bất đồng bộ (không chặn luồng xử lý), giống như chúng ta đã thảo luận về async/await.
                x => x.ID == id: Đây là điều kiện lọc (Lambda Expression). Nó bảo Database tìm bản ghi nào có cột ID bằng với giá trị biến id truyền vào.
                await: Đợi kết quả trả về từ Database mà không làm treo ứng dụng.
             */
            return await _context.Stocks.AnyAsync(x => x.ID == id);
        }

        public async Task<List<Stock>> GetAllAsyncSearch(QueryObject query)
        {
            /*
             * 1. _context.Stocks.AsQueryable()
                Dòng này cực kỳ quan trọng.
                    AsQueryable() giúp bạn tạo ra một "truy vấn chờ". Nó chưa hề gọi xuống Database ngay lập tức.
                    Nó cho phép bạn "cộng dồn" thêm các điều kiện lọc (Where) vào biến stocks trước khi thực thi.
             */
            var stocks =  _context.Stocks.AsQueryable();

            // Đoạn code sử dụng hai khối if để kiểm tra xem người dùng có truyền tham số tìm kiếm vào hay không:
            if (!string.IsNullOrEmpty(query.Symbol))
            {
                stocks = stocks.Where(s => s.Symbol.Contains(query.Symbol));
            }

            if(!string.IsNullOrEmpty(query.CompanyName))
            {
                stocks = stocks.Where(s => s.CompanyName.Contains(query.CompanyName));
            }

            // query.SortBy.Equals("Symbol", ...): Kiểm tra xem giá trị mà người dùng gửi lên trong trường SortBy
            // có phải là chuỗi "Symbol" hay không (không phân biệt hoa thường).
            if (!string.IsNullOrEmpty(query.SortBy))
            {
                if (query.SortBy.Equals("Symbol", StringComparison.OrdinalIgnoreCase))
                {
                    stocks = query.IsDescending 
                        ? stocks.OrderByDescending(s => s.Symbol) // Nếu IsDescending = true -> Sắp xếp giảm dần
                        : stocks.OrderBy(s => s.Symbol);   // Nếu IsDescending = false -> Sắp xếp tăng dần
                }
                if (query.SortBy.Equals("CompanyName", StringComparison.OrdinalIgnoreCase))
                {
                    stocks = query.IsDescending
                        ? stocks.OrderByDescending(s => s.Symbol) // Nếu IsDescending = true -> Sắp xếp giảm dần
                        : stocks.OrderBy(s => s.Symbol);   // Nếu IsDescending = false -> Sắp xếp tăng dần
                }
            }

            // mặc định không có sắp xếp theo symbol hay companyName thì sắp xếp theo id
            // bới vì dùng pagination không có sắp xếp thì nó trả về ngẫu nhiễn có thể làm trang 1
            // và trang 2 trùng nhau làm hỏng đi logic phân trang
            stocks = query.IsDescending
                        ? stocks.OrderByDescending(s => s.ID) // Nếu IsDescending = true -> Sắp xếp giảm dần
                        : stocks.OrderBy(s => s.ID);

            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            /*
             * 3.  return await stocks.Skip(skipNumber).Take(query.PageSize).ToListAsync()
                Đây mới là lúc thực thi.
                    Đến dòng này, Entity Framework mới tổng hợp tất cả các if ở trên thành một câu lệnh SQL duy nhất và gửi xuống Database.
                    Nếu người dùng không nhập gì cả, nó sẽ chạy: SELECT * FROM Stocks.
                    Nếu người dùng nhập cả hai, nó sẽ chạy: SELECT * FROM Stocks WHERE Symbol LIKE ... AND CompanyName LIKE .

                skipNumber = (3 - 1) * 10 = 20: Nghĩa là để xem trang 3, chúng ta phải nhảy qua (bỏ qua) 20 bản ghi đầu tiên (của trang 1 và trang 2).
                .Skip(20): Bảo Database bỏ qua 20 dòng đầu.
                .Take(10): Bảo Database chỉ lấy đúng 10 dòng tiếp theo sau khi đã nhảy qua 20 dòng kia.
                 
                Nếu không có OrderBy, Database có thể trả về thứ tự ngẫu nhiên ở mỗi lần gọi, dẫn đến việc trang 1 và trang 2 có thể chứa các bản ghi trùng nhau, 
                 làm hỏng logic phân trang của bạn.
             */
            return await stocks.Skip(skipNumber).Take(query.PageSize).ToListAsync();
        }

        public async Task<Stock?> GetStockBySymbol(string symbol)
        {
            return await _context.Stocks.FirstOrDefaultAsync(s => s.Symbol == symbol);
        }
    }
}
