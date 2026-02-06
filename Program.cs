//Dòng này tạo một đối tượng builder để thiết lập cấu hình cho ứng dụng.
//Nó mặc định tải các cấu hình từ tệp appsettings.json, các biến môi trường, và các tham số dòng lệnh.
using coreC_.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


//AddControllers(): Đăng ký dịch vụ cho các Controller. Đây là thành phần chính để xử lý các yêu cầu API trong mô hình MVC/API.
//AddEndpointsApiExplorer(): Giúp ứng dụng hiểu và liệt kê được các "điểm cuối" (endpoints) của API, cần thiết để Swagger hoạt động.
//AddSwaggerGen(): Cấu hình bộ tạo tài liệu Swagger. Nó sẽ tự động tạo ra đặc tả OpenAPI cho các API của bạn.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = "Server=localhost;Database=stock;User=root;Password=29092003";
builder.Services.AddDbContext<ApplicationDBContext>(options =>
{
    // ServerVersion.AutoDetect(connectionString)); // Tự động nhận diện phiên bản MySQL);
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)); // Tự động nhận diện phiên bản MySQL);
}

);

//Sau khi đã đăng ký xong tất cả các dịch vụ cần thiết, lệnh Build() sẽ tạo ra đối tượng app. Đối tượng này dùng để thiết lập các Middleware (phần mềm trung gian).
var app = builder.Build();

//Đoạn này quyết định một yêu cầu gửi đến server sẽ đi qua những bước nào:
//Kiểm tra môi trường: Nếu ứng dụng đang chạy ở chế độ Phát triển (Development), nó sẽ kích hoạt Swagger.
//UseSwagger(): Tạo ra file JSON mô tả về các API.
//UseSwaggerUI(): Tạo ra giao diện web (thường ở đường dẫn /swagger) để bạn có thể xem và dùng thử các API trực tiếp trên trình duyệt.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Tự động chuyển hướng các yêu cầu từ HTTP sang HTTPS để tăng tính bảo mật.
app.UseHttpsRedirection();

//Dòng này cực kỳ quan trọng. Nó ánh xạ các yêu cầu HTTP (GET, POST, PUT, DELETE...) đến các phương thức tương ứng trong các lớp Controller mà bạn viết.
app.MapControllers();

//Đây là lệnh cuối cùng để khởi động server và bắt đầu lắng nghe các yêu cầu từ phía client (như trình duyệt hoặc ứng dụng di động).
app.Run();
