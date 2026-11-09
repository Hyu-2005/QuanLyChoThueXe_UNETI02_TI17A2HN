using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Data;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Helpers;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Controllers
{
    [SessionAuthorize(Roles = new[] { VaiTro.Admin, VaiTro.NhanVien })]
    public class GPSController : Controller
    {
        private readonly AppDbContext _context;

        public GPSController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GiaLapDiChuyen(int maDatXe)
        {
            var datXe = await _context.DatXes
                .Include(d => d.BanGiaoXe)
                .FirstOrDefaultAsync(d => d.MaDatXe == maDatXe);

            if (datXe == null || datXe.TrangThai != TrangThaiDatXe.DangThue)
            {
                TempData["Error"] = "Chỉ có thể giả lập khi xe đang trong trạng thái Đang thuê.";
                return RedirectToAction("Details", "DatXe", new { id = maDatXe });
            }

            double baseLat = datXe.BanGiaoXe?.ViDoBanGiao ?? 21.004682;
            double baseLng = datXe.BanGiaoXe?.KinhDoBanGiao ?? 105.843269;

            var lastPoint = await _context.LichSuDiChuyens
                .Where(l => l.MaDatXe == maDatXe)
                .OrderByDescending(l => l.ThoiGian)
                .FirstOrDefaultAsync();

            if (lastPoint != null)
            {
                baseLat = lastPoint.ViDo;
                baseLng = lastPoint.KinhDo;
            }

            var random = new Random();
            double newLat = baseLat + (random.NextDouble() - 0.5) * 0.01;
            double newLng = baseLng + (random.NextDouble() - 0.5) * 0.01;

            _context.LichSuDiChuyens.Add(new LichSuDiChuyen
            {
                MaDatXe = maDatXe,
                ViDo = newLat,
                KinhDo = newLng,
                ThoiGian = DateTime.Now
            });

            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã giả lập xe di chuyển thêm một điểm mới!";

            return RedirectToAction("Details", "DatXe", new { id = maDatXe });
        }
    }
}