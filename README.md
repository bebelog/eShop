# eShop - Clean Architecture (.NET 8 Blazor & SQL Server Dapper)

Hệ thống Website bán hàng trực tuyến thương mại điện tử **eShop** được xây dựng bằng **ASP.NET Core 8.0**, **Blazor Interactive Server**, áp dụng chuẩn kiến trúc **Clean Architecture** và kết nối cơ sở dữ liệu **Microsoft SQL Server** thông qua Micro-ORM **Dapper**.

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

## 🚀 Các Tính Năng Đã Hoàn Thiện

### 🛒 1. Phân hệ Khách hàng (Customer Portal)
* **Danh mục sản phẩm:** Hiển thị danh sách sản phẩm đọc trực tiếp từ SQL Server.
* **Chi tiết sản phẩm:** Xem thông tin, giá, mô tả và thêm vào giỏ hàng.
* **Giỏ hàng thời gian thực (State Store):** Quản lý trạng thái giỏ hàng xuyên suốt phiên làm việc, cập nhật số lượng badge trên thanh điều hướng.
* **Cập nhật & Xóa giỏ hàng:** Điều chỉnh số lượng món hàng, tự động tính tổng tiền.
* **Đặt hàng (Checkout):** Biểu mẫu thông tin khách hàng (`CustomerViewModel`) tích hợp AutoMapper map sang `Order`.
* **Màn hình xác nhận (Order Confirmation):** Cung cấp mã đơn hàng duy nhất (`UniqueId`) để khách hàng tra cứu.

### 🛡️ 2. Phân hệ Quản trị & Bảo mật (Admin Portal)
* **Đăng nhập xác thực Cookie (Cookie Authentication):** Quản lý phiên làm việc bảo mật cho tài khoản Admin (`admin` / `admin123`).
* **Bảo vệ đường dẫn (Route Protection):** Áp dụng `@attribute [Authorize]` cho toàn bộ trang Admin; tự động chặn truy cập trái phép và yêu cầu đăng nhập.
* **Đơn hàng chờ xử lý (`/outstandingorders`):** Danh sách các đơn mới đặt cần xử lý.
* **Chi tiết đơn hàng & Duyệt đơn (`/orderdetail/{id}`):** Xem chi tiết từng món hàng và nút duyệt đơn (cập nhật `DateProcessed` và `AdminUser`).
* **Đơn hàng đã xử lý (`/processedorders`):** Lịch sử lưu trữ các đơn đã hoàn thành.

### 🗄️ 3. Cơ sở dữ liệu & Tích hợp Dapper (SQL Server)
* Script khởi tạo: `eShop.SchemaAndData.sql` tạo database `eShop` với 3 bảng: `Product`, `Order`, `OrderLineItem`.
* Micro-ORM **Dapper 2.1**: Tối ưu tốc độ truy vấn SQL thuần.
* Kiến trúc Plug-and-Play: Dễ dàng hoán đổi DataStore trong `Program.cs` thông qua Dependency Injection.

---

## 🛠️ Hướng Dẫn Cài Đặt & Chạy Dự Án

### 1. Yêu cầu môi trường
* [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* [Microsoft SQL Server](https://www.microsoft.com/sql-server/) (Local hoặc LocalDB)
* Visual Studio 2022 hoặc Visual Studio Code

### 2. Thiết lập Cơ sở dữ liệu
1. Mở SQL Server Management Studio (SSMS) hoặc `sqlcmd`.
2. Chạy file script: `eShop.SchemaAndData.sql` có sẵn trong thư mục gốc của dự án.
3. Kiểm tra chuỗi kết nối trong `eShop.Web/appsettings.json`:
```json
"ConnectionStrings": {
  "eShop": "Server=localhost;Database=eShop;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 3. Build & Chạy dự án
```bash
# Di chuyển vào thư mục dự án
cd eShop

# Khôi phục dependencies và build solution
dotnet build

# Chạy ứng dụng web
dotnet run --project eShop.Web/eShop.Web.csproj
```

Truy cập:
* **Khách hàng:** `https://localhost:7080` (hoặc cổng hiển thị trên terminal).
* **Quản trị Admin:** Bấm nút **Login** trên thanh menu (Tài khoản: `admin` / Mật khẩu: `admin123`).
