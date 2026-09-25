// File: Program.cs
// Noi dung: Cau hinh DbContext, Session, MVC, Middleware, Seed Data
// Sinh vien thuc hien: Pham Gia Minh Hoang - 23103100087 - SV1 (cau hinh Session)
// Module: Module 1 - Tai khoan, Dang nhap, Phan quyen, Loai xe, Hang xe

using Microsoft.EntityFrameworkCore;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Data;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Data.SeedData;

var builder = WebApplication.CreateBuilder(args);

// 1. Cau hinh MVC
builder.Services.AddControllersWithViews();

// 2. Cau hinh DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Cau hinh Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".QuanLyChoThueXe.Session";
});

// 4. HttpContextAccessor
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// 5. Seed Data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        await DbInitializer.SeedAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Loi khi seed du lieu");
    }
}

// 6. Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// 7. Session PHAI dat truoc Authorization
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();