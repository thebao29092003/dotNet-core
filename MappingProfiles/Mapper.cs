using AutoMapper;
using coreC_.Dtos.Stock;
using coreC_.Models;

namespace coreC_.MappingProfiles
{
    public class Mapper : Profile
    {

        //Trong C# và AutoMapper, việc gọi CreateMap trong Constructor (hàm khởi tạo) không phải là ngẫu nhiên,
        //mà nó dựa trên cả quy tắc cú pháp của ngôn ngữ C# và cơ chế hoạt động của thư viện AutoMapper.
        public Mapper()
        {
            //Đúng: Các lệnh thực thi(như gọi hàm) phải nằm trong một phương thức, một thuộc tính hoặc một Constructor.
            //Constructor là nơi lý tưởng nhất để thiết lập cấu hình ban đầu khi đối tượng được tạo ra.
            /*
             * Hãy tưởng tượng lớp StockMapper giống như một tờ hướng dẫn sử dụng:
                Class body: Là tờ giấy trắng.
                Constructor: Là lúc bạn cầm bút viết hướng dẫn lên tờ giấy đó.
                AutoMapper engine: Là người đọc hướng dẫn.
                Người đọc chỉ có thể đọc những gì bạn đã viết sẵn trên tờ giấy khi bạn đưa cho họ. 
                Nếu bạn không viết vào lúc khởi tạo (Constructor), tờ giấy sẽ trống rỗng và người đọc sẽ không biết phải map dữ liệu như thế nào.
             */

            // map 2 chiều giữa Stock và StockDto
            CreateMap<Stock, StockDto>().ReverseMap();
            CreateMap<Stock, StockRequestDto>().ReverseMap();
            CreateMap<Stock, UpdateStockRequestDto>().ReverseMap();
        }
    }
}
