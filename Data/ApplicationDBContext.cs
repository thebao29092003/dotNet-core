using coreC_.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace coreC_.Data
{

    /*
     *  Giải thích: Thay vì kế thừa DbContext thông thường, bạn kế thừa IdentityDbContext<AppUser>.
        Tác dụng: Khi bạn chạy Migration, Entity Framework sẽ tự động tạo ra một loạt các bảng hệ thống (thường bắt đầu bằng AspNet...) 
            như AspNetUsers (lưu người dùng), AspNetRoles (lưu quyền), AspNetUserClaims... để phục vụ việc quản lý tài khoản. 
            Nó gộp chung cả các bảng của bạn (Stocks, Comments) và các bảng Identity vào cùng một Database.
     */
    public class ApplicationDBContext : IdentityDbContext<AppUser>
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions): base(dbContextOptions) {

        }

        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Comment> Comments { get; set; }

        /*
         * 1. protected override void OnModelCreating
            Đây là một phương thức của Entity Framework Core. Nó được gọi khi Database lần đầu tiên được khởi tạo hoặc khi bạn thực hiện Migration.
            Nó cho phép bạn cấu hình các bảng, mối quan hệ và dữ liệu mẫu (Seed data) bằng code thay vì vào Database gõ bằng tay.
           2. base.OnModelCreating(builder);
            Cực kỳ quan trọng: Vì ApplicationDBContext của bạn kế thừa từ IdentityDbContext, lớp cha này đã có sẵn các cấu hình quan trọng cho các bảng Identity (như thiết lập khóa chính cho AspNetUsers, AspNetRoles...).
            Dòng này đảm bảo các cấu hình mặc định của Identity không bị mất đi khi bạn viết thêm các cấu hình riêng bên dưới. Luôn phải để dòng này ở đầu hàm.
         */
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            /*
             * Khởi tạo danh sách
             * Name: Tên của vai trò (dùng để hiển thị hoặc kiểm tra trong code).
                NormalizedName: Tên đã được "chuẩn hóa" (viết hoa).
                Tại sao cần? Identity sử dụng cột này để tìm kiếm quyền của người dùng một cách nhanh nhất và không phân biệt hoa thường. 
                Nếu bạn quên đặt NormalizedName, các hàm kiểm tra quyền như User.IsInRole("Admin") có thể sẽ không hoạt động chính xác.
             */
            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Name = "User", NormalizedName = "USER" }
            };

            /*
             * Đây là lệnh Seed Data. Nó ra lệnh cho Entity Framework: "Khi tạo bảng IdentityRole (tên thật trong DB là AspNetRoles), hãy nhét ngay danh sách roles này vào đó".
               Nếu những dòng này đã tồn tại trong Database, EF sẽ bỏ qua. Nếu chưa có, nó sẽ tự động INSERT.
             */
            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
