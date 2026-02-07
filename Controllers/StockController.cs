using coreC_.Data;
using Microsoft.AspNetCore.Mvc;

namespace coreC_.Controllers
{
    [Route("api/stock")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public StockController(ApplicationDBContext context)
        {
            _context = context;
        }

        // lấy toàn bộ danh sách
        [HttpGet]
        public IActionResult GetAll()
        {
            var stocks = _context.Stocks.ToList();
            return Ok(stocks);
        }

        /*
         * 1. Attribute [HttpGet("{id}")]
                [HttpGet]: Chỉ định rằng phương thức này chỉ phản hồi các yêu cầu sử dụng giao thức HTTP GET (dùng để đọc dữ liệu).
                "{id}": Đây là một tham số đường dẫn (route parameter).
                    Ví dụ: Nếu URL của bạn là api/stock, thì phương thức này sẽ khớp với URL api/stock/5 (trong đó 5 là id).
           2. Tham số [FromRoute] int id
                [FromRoute]: Chỉ thị cho ứng dụng rằng giá trị của biến id phải được lấy từ trên thanh địa chỉ (URL), khớp với cái tên {id} đã khai báo ở trên.
                int id: Ép kiểu dữ liệu từ URL về kiểu số nguyên. Nếu người dùng nhập api/stock/abc, API sẽ báo lỗi vì không phải là số.
           3. Truy vấn dữ liệu _context.Stocks.Find(id)
                 _context: Là đối tượng ApplicationDBContext (đã được kết nối với database).
                .Stocks: Truy cập vào bảng Stocks.
                .Find(id): Đây là phương thức của Entity Framework Core để tìm kiếm một bản ghi dựa trên Khóa chính (Primary Key). Nó rất nhanh và hiệu quả cho việc tìm kiếm theo ID.
         */
        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var stock = _context.Stocks.Find(id);

            if (stock == null)
            {
                return NotFound();
            }
            return Ok(stock);
        }
    }
}
