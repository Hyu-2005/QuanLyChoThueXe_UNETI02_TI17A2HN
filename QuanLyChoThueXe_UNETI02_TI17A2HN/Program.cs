using Microsoft.EntityFrameworkCore;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Data;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Data.SeedData;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Services.Implementations;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".QuanLyChoThueXe.Session";
});

builder.Services.AddHttpContextAccessor();

// ==============================================================
// Bắt Đầu Code của SV3: Vũ Tiến Đạt
// file: Program.cs
// nội dung: Đăng nhập bằng Google (OAuth). Chỉ bật khi đã cấu hình ClientId và ClientSecret.
// sinh viên thực hiện: Vũ Tiến Đạt - 23103100119 - SV3
// ==============================================================
var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

var authBuilder = builder.Services.AddAuthentication()
    .AddCookie("DangNhapNgoai", options =>
    {
        options.Cookie.Name = ".QuanLyChoThueXe.External";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    });

if (!string.IsNullOrWhiteSpace(googleClientId) && !string.IsNullOrWhiteSpace(googleClientSecret))
{
    authBuilder.AddGoogle(options =>
    {
        options.ClientId = googleClientId;
        options.ClientSecret = googleClientSecret;
        options.SignInScheme = "DangNhapNgoai";

        options.Events.OnRedirectToAuthorizationEndpoint = context =>
        {
            context.Response.Redirect(context.RedirectUri + "&prompt=select_account");
            return Task.CompletedTask;
        };

        options.Events.OnRemoteFailure = context =>
        {
            context.Response.Redirect("/TaiKhoan/DangNhap");
            context.HandleResponse();
            return Task.CompletedTask;
        };
    });
}
// ==============================================================
// Kết thúc Code của SV3
// ==============================================================

builder.Services.AddScoped<ITrungLichService, TrungLichService>();
builder.Services.AddScoped<IXeTrongService, XeTrongService>();
builder.Services.AddScoped<ITinhTienService, TinhTienService>();
builder.Services.AddScoped<IThongKeService, ThongKeService>();
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddScoped<IQRCodeService, QRCodeService>();

// ==============================================================
// Bắt Đầu Code của SV4: Nguyễn Hoàng Đức Hiếu
// file: Program.cs
// nội dung: STT 10 - Đăng ký IEmailService phục vụ gửi email xác nhận và từ chối duyệt đơn
// sinh viên thực hiện : Nguyễn Hoàng Đức Hiếu -  23103100116 - SV4 
// ==============================================================
builder.Services.AddScoped<IEmailService, EmailService>();
// ==============================================================
// Kết thúc Code của SV4
// ==============================================================

// ==============================================================
// Bắt Đầu Code của SV5: Dương Lâm Huy - 23103100120
// file: Program.cs
// nội dung: Đăng ký IMaGiamGiaService cho Voucher + Ví điểm thưởng
// Module: Module 5 - Tính tiền, Thanh toán
// ==============================================================
builder.Services.AddScoped<IMaGiamGiaService, MaGiamGiaService>();
// ==============================================================
// Kết thúc Code của SV5
// ==============================================================

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
        await DbInitializer.SeedAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Loi khi seed du lieu");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();