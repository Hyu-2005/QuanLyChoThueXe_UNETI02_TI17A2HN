// ==============================================================
// Bắt Đầu Code của SV4: Nguyễn Hoàng Đức Hiếu
// file: Controllers/BanGiaoXeController.cs
// nội dung: Tạo mới Controller độc lập để xử lý logic Bàn giao xe 
// sinh viên thực hiện : Nguyễn Hoàng Đức Hiếu -  23103100116 - SV4 
// ==============================================================
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Data;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Helpers;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Controllers
{
    [SessionAuthorize(Roles = new[] { VaiTro.Admin, VaiTro.NhanVien })]
    public class BanGiaoXeController : Controller
    {
        private readonly AppDbContext _context;

        public BanGiaoXeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("/BanGiaoXe/{id}")]
        public async Task<IActionResult> Index(int id)
        {
            var datXe = await _context.DatXes
                .Include(d => d.Xe)
                .Include(d => d.KhachHang)
                .FirstOrDefaultAsync(d => d.MaDatXe == id);

            if (datXe == null || datXe.TrangThai != TrangThaiDatXe.DaDuyet)
                return NotFound();
            
            var model = new BanGiaoXe {
                MaDatXe = id,
                SoKmBanGiao = datXe.Xe.SoKmHienTai,
                ThoiGianBanGiao = DateTime.Now,
                NguoiBanGiao = HttpContext.Session.GetString("HoTen") ?? "Admin",
                MucNhienLieuBanGiao = 100 // Default 100%
            };

            ViewBag.DatXe = datXe;
            return View(model);
        }

        [HttpPost("/BanGiaoXe/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int id, BanGiaoXe model)
        {
            if (id != model.MaDatXe) return BadRequest();
            
            var datXe = await _context.DatXes
                .Include(d => d.Xe)
                .FirstOrDefaultAsync(d => d.MaDatXe == id);

            if (datXe == null || datXe.TrangThai != TrangThaiDatXe.DaDuyet)
                return NotFound();

            if (ModelState.IsValid)
            {
                datXe.TrangThai = TrangThaiDatXe.DangThue;
                datXe.Xe.SoKmHienTai = model.SoKmBanGiao;
                datXe.Xe.TinhTrang = TinhTrangXe.DangChoThue;
                
                _context.BanGiaoXes.Add(model);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Đã bàn giao xe cho khách hàng thành công.";
                return RedirectToAction("Details", "DatXe", new { id = id });
            }

            ViewBag.DatXe = datXe;
            return View(model);
        }
    }
}
// ==============================================================
// Kết thúc Code của SV4
// ==============================================================
