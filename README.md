# 🛍️ eShop - Hệ Thống Website Bán Hàng Trực Tuyến

---

### 👨‍🎓 THÔNG TIN SINH VIÊN THỰC HIỆN
* **Họ và tên:** Nguyễn Viết Mẫn
* **Mã sinh viên:** 23K4080026
* **Trường:** Trường Đại học Kinh tế - Đại học Huế (HUE)
* **Học phần:** Thực hành Visual Studio / Phát triển ứng dụng Web với .NET
* **Đề tài:** Xây dựng Website Thương Mại Điện Tử (eShop) theo kiến trúc Clean Architecture
* **Nền tảng công nghệ:** 
  * **Framework:** ASP.NET Core 8.0 (.NET 8)
  * **Giao diện:** Blazor Interactive Server
  * **Cơ sở dữ liệu:** Microsoft SQL Server
  * **Data Access (Micro-ORM):** Dapper 2.1
  * **Bảo mật & Phân quyền:** ASP.NET Core Cookie Authentication
  * **Mô hình kiến trúc:** Clean Architecture (Domain, UseCases, Infrastructure/Plugins, Presentation/Web)

---

## 📖 GIỚI THIỆU ĐỀ TÀI & NỘI DUNG THỰC HIỆN

Dự án **eShop** được xây dựng nhằm mô phỏng một hệ thống bán hàng trực tuyến hoàn chỉnh từ bài toán thực tế, áp dụng các tiêu chuẩn kiến trúc phần mềm chuyên nghiệp của doanh nghiệp:

### 🛒 1. Phân hệ Khách hàng (Customer Portal)
* **Danh mục sản phẩm:** Hiển thị danh mục sản phẩm được truy vấn trực tiếp từ cơ sở dữ liệu SQL Server.
* **Chi tiết sản phẩm:** Xem thông tin, giá bán, hình ảnh và mô tả sản phẩm; thao tác thêm vào giỏ hàng.
* **Quản lý Giỏ hàng (State Store):** 
  * Áp dụng State Management (`ShoppingCartStateStore`) duy trì giỏ hàng theo thời gian thực.
  * Cập nhật số lượng sản phẩm, xóa sản phẩm khỏi giỏ, tự động tính tổng tiền.
  * Huy hiệu giỏ hàng (Cart badge) trên thanh điều hướng cập nhật tức thì.
* **Đặt hàng & Thanh toán (Checkout):** 
  * Biểu mẫu nhập thông tin người nhận (`CustomerViewModel`).
  * Áp dụng AutoMapper ánh xạ dữ liệu sang đối tượng đơn hàng (`Order`) và lưu trữ vào SQL Server.
* **Xác nhận đơn hàng (Order Confirmation):** Cấp mã đơn hàng duy nhất (`UniqueId`) để khách hàng tra cứu tiến độ xử lý.

### 🛡️ 2. Phân hệ Quản trị & Bảo mật (Admin Portal)
* **Bảo mật xác thực Cookie (Cookie Authentication):** Quản lý phiên làm việc bảo mật cho tài khoản Admin (`admin` / `adminadmin`).
* **Bảo vệ đường dẫn (Route Protection):** Áp dụng thuộc tính `@attribute [Authorize]` và thẻ `<AuthorizeRouteView>` để bảo vệ toàn bộ các trang quản trị; tự động chuyển hướng về trang đăng nhập nếu chưa xác thực.
* **Đơn hàng chờ xử lý (`/outstandingorders`):** Bảng theo dõi danh sách các đơn hàng mới đặt cần duyệt.
* **Chi tiết đơn hàng & Duyệt đơn (`/orderdetail/{id}`):** Xem thông tin người nhận, danh sách từng món hàng và nút duyệt đơn (cập nhật `DateProcessed` và định danh `AdminUser`).
* **Lịch sử đơn hàng đã xử lý (`/processedorders`):** Lưu trữ và tra cứu lịch sử các đơn hàng đã hoàn tất.

### 🗄️ 3. Cơ sở dữ liệu & Tích hợp Dapper (SQL Server)
* Thiết kế cơ sở dữ liệu với 3 bảng ràng buộc khóa chính/khóa ngoại: `Product`, `Order`, `OrderLineItem`.
* Xây dựng Plugin độc lập `eShop.DataStore.SQL.Dapper` sử dụng Micro-ORM **Dapper 2.1** để thực thi các câu truy vấn SQL thuần tối ưu hiệu năng cao.
* Áp dụng nguyên lý Dependency Injection: Dễ dàng chuyển đổi linh hoạt giữa dữ liệu mẫu (HardCode) và dữ liệu thật (SQL Server) mà không làm thay đổi tầng Business Logic hay Giao diện.

---

## 🏛️ Kiến trúc Hệ thống (Clean Architecture)

Dự án được phân chia thành các tầng độc lập theo nguyên lý Dependency Inversion:

```
eShop/
├── eShop.CoreBusiness/              # Domain Layer: Chứa các Entity (Product, Order, OrderLineItem)
├── eShop.UseCases/                  # Application Layer: Chứa Use Cases nghiệp vụ (Customer & Admin)
│   ├── AdminPortal/                 # Các UseCases xem đơn, duyệt đơn, chi tiết đơn hàng
│   ├── OrderConfirmationScreen/     # UseCases xác nhận đơn hàng
│   ├── PluginInterfaces/DataStore/  # Interfaces cho Repositories (IProductRepository, IOrderRepository)
│   ├── PluginInterfaces/StateStore/ # Interfaces cho Quản lý State giỏ hàng
│   ├── SearchProductScreen/         # UseCases tìm kiếm & xem sản phẩm
│   ├── ShoppingCartScreen/          # UseCases giỏ hàng
│   └── ViewProductScreen/           # UseCases chi tiết sản phẩm
├── Plugins/                         # Infrastructure Layer: Các module cắm ngoài
│   ├── eShop.DataStore.HardCode/    # Mock Data in-memory ban đầu
│   ├── eShop.StateStore.DI/         # ShoppingCartStateStore quản lý giỏ hàng Blazor
│   └── eShop.DataStore.SQL.Dapper/  # Kết nối Microsoft SQL Server qua thư viện Dapper
└── eShop.Web/                       # Presentation Layer: Blazor Server Web App
    ├── Controllers/                 # AuthenticationController (Cookie Auth /authenticate, /logout)
    └── eShop.Web.Modules/           # Razor Class Libraries
        ├── eShop.Web.AdminPortal/   # Giao diện Admin: Quản lý đơn hàng, duyệt đơn
        ├── eShop.Web.CustomerPortal/# Giao diện Khách hàng: Xem hàng, giỏ hàng, đặt hàng
        └── eShop.Web.Common/        # Controls & ViewModels dùng chung (Login, Header, Nav)
```

---

## 🛠️ Hướng Dẫn Cài Đặt & Chạy Dự Án

### 1. Yêu cầu môi trường
* [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* [Microsoft SQL Server](https://www.microsoft.com/sql-server/) (Local hoặc LocalDB)
* Visual Studio 2022 hoặc Visual Studio Code

### 2. Thiết lập Cơ sở dữ liệu
1. Mở SQL Server Management Studio (SSMS) hoặc `sqlcmd`.
2. Tạo cơ sở dữ liệu `eShop` và tạo các bảng `Product`, `Order`, `OrderLineItem`.
3. Kiểm tra chuỗi kết nối trong `eShop.Web/appsettings.json`:
```json
"ConnectionStrings": {
  "eShop": "Server=localhost;Database=eShop;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 3. Build & Chạy ứng dụng
```bash
# Di chuyển vào thư mục dự án
cd eShop

# Khôi phục dependencies và build solution
dotnet build

# Chạy ứng dụng web
dotnet run --project eShop.Web/eShop.Web.csproj
```

Truy cập hệ thống:
* **Khách hàng:** `https://localhost:7080` (hoặc cổng hiển thị trên terminal).
* **Quản trị Admin:** Bấm nút **Login** trên thanh menu (Tài khoản: `admin` / Mật khẩu: `adminadmin`).
