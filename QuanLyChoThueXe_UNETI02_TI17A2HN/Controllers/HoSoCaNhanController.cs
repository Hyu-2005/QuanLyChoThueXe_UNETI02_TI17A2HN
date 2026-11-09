using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Data;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Helpers;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Controllers
{
    [SessionAuthorize(Roles = new[] { VaiTro.KhachHang })]
    public class HoSoCaNhanController : Controller
    {
        private readonly AppDbContext _context;

        public HoSoCaNhanController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var maKhachHang = HttpContext.Session.GetMaKhachHang();
            if (!maKhachHang.HasValue) return RedirectToAction("DangNhap", "TaiKhoan");

            var khachHang = await _context.KhachHangs
                .Include(k => k.TaiKhoan)
                .AsNoTracking()
                .FirstOrDefaultAsync(k => k.MaKhachHang == maKhachHang.Value);

            if (khachHang == null) return NotFound();

            // Truyền cấu hình quy đổi điểm sang View
            ViewBag.GiaTriMotDiem = HttpContext.RequestServices.GetRequiredService<IConfiguration>()
                .GetValue<decimal?>("LoyaltyPoints:GiaTriMotDiem") ?? 5000m;

            return View(khachHang);
        }
    }
}