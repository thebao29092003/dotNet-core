using AutoMapper;
using coreC_.Dtos.Stock;
using coreC_.Interfaces;
using coreC_.MappingProfiles;
using coreC_.Models;
using Microsoft.AspNetCore.Mvc;


namespace coreC_.Controllers
{
    [Route("api/stock")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IStockRepository _stockRepository;
        private readonly IMapper _mapper;

        public StockController(IStockRepository stockRepository, IMapper mapper)
        {
            _mapper = mapper;
            _stockRepository = stockRepository;
        }

        // lấy toàn bộ danh sách
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var stocks = await _stockRepository.GetAllStocksAsync();
            var stocksDto = _mapper.Map<List<StockDto>>(stocks);

            return Ok(stocksDto);
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
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var stock = await _stockRepository.GetStockByIdAsync(id);
            if (stock == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<StockDto>(stock));

        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StockRequestDto stockDto)
        {


            // cái này không cần ID vì trong database mình cấu hình tự động tăng rồi
            // 1. Chuyển đổi (Map) từ DTO sang Entity (Model)
            // stockDto chỉ là cái "vỏ" chứa dữ liệu người dùng gửi lên.
            // stockModel là đối tượng có cấu trúc giống hệt bảng trong Database.
            var stockModel = _mapper.Map<Stock>(stockDto);

            await _stockRepository.CreateStockAsync(stockModel);


            //return stockModel;

            // 4. Trả về kết quả theo chuẩn RESTful API
            // - Trả về mã lỗi 201 (Created).
            // - Header "Location" sẽ chứa URL để lấy thông tin bản ghi vừa tạo (GetById).
            // - Body trả về chính đối tượng stockModel vừa tạo (đã có kèm ID mới từ DB).

            /*
             * Ý NGHĨA 3 THAM SỐ CỦA CreatedAtAction:
                actionName (nameof(GetById)): Tên của hàm dùng để lấy thông tin chi tiết của đối tượng vừa tạo dùng nameof tốt
                    hơn là điền chuỗi string vì khi tên hàm viết sai hoặc sau này đổi tên hàm thì nó sẽ báo.
                routeValues (new { id = stockModel.ID }): Các tham số cần thiết để hàm GetById hoạt động. Ở đây chúng ta truyền vào id của bản ghi vừa được lưu vào Database.
                value (stockModel): Toàn bộ dữ liệu của đối tượng vừa tạo để trả về cho người dùng xem lại.

              * KẾT QUẢ: 
               Khi bạn trả về CreatedAtAction, nó sẽ thực hiện 3 việc quan trọng trong phản hồi HTTP gửi về cho Client (Postman, Frontend...):
                Trạng thái HTTP 201 (Created): Thay vì trả về 200 (OK) thông thường, nó trả về mã 201 để khẳng định: "Dữ liệu đã được tạo mới thành công trên Server".
                Header "Location": Đây là phần quan trọng nhất. Server sẽ tự động tạo ra một đường dẫn (URL) dẫn trực tiếp đến bản ghi vừa tạo.
                    Ví dụ: Location: https://localhost:5000/api/stock/15 (trong đó 15 là ID vừa sinh ra).
                Response Body: Trả về đối tượng JSON của stockModel để Client có thể sử dụng ngay mà không cần gọi thêm lệnh GET.
             */
            return CreatedAtAction(nameof(GetById), new { id = stockModel.ID }, stockModel);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStockRequestDto updateDto)
        {
            var stockModel = await _stockRepository.UpdateStockAsync(id, updateDto);

            if (stockModel == null)
            {
                return NotFound();
            }

            return Ok(stockModel);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var stockModel = await _stockRepository.DeleteStockAsync(id);
            if (stockModel == null)
            {
                return NotFound();
            }
            return Ok(stockModel);
        }
    }
}
