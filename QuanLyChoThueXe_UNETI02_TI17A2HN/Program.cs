// ============================================================
// Project: QuanLyChoThueXe_UNETI20_TI17A1HN
// File: Program.cs
// Nội dung: Cấu hình DbContext, Session, MVC, Middleware
// ============================================================

using Microsoft.EntityFrameworkCore;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình MVC
builder.Services.AddControllersWithViews();

// 2. Cấu hình DbContext (EF Core 10 + SQL Server)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Cấu hình Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);   // Session hết hạn sau 60 phút
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 4. Đăng ký HttpContextAccessor (dùng cho Service đọc Session)
builder.Services.AddHttpContextAccessor();

// 5. Đăng ký các Service nghiệp vụ (sẽ thêm ở Phần sau)
// builder.Services.AddScoped<ITrungLichService, TrungLichService>();
// builder.Services.AddScoped<IXeTrongService, XeTrongService>();
// builder.Services.AddScoped<ITinhTienService, TinhTienService>();
// builder.Services.AddScoped<IThongKeService, ThongKeService>();

var app = builder.Build();

// 6. Cấu hình Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 7. Bật Session TRƯỚC Authorization
app.UseSession();

app.UseAuthorization();

// 8. Định tuyến mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();