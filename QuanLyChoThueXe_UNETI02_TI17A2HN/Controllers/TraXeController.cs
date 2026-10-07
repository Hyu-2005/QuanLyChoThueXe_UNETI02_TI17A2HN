// ==============================================================
// Bắt Đầu Code của SV4: Nguyễn Hoàng Đức Hiếu
// file: Controllers/TraXeController.cs
// nội dung: Tạo mới Controller độc lập để xử lý logic Trả xe và cập nhật luồng tự động chuyển hướng (redirect) sang trang Thanh toán sau khi trả xe xong.
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
    public class TraXeController : Controller
    {
        private readonly AppDbContext _context;

        public TraXeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("/TraXe/{id}")]
        public async Task<IActionResult> Index(int id)
        {
            var datXe = await _context.DatXes
                .Include(d => d.Xe)
                .Include(d => d.KhachHang)
                .Include(d => d.BanGiaoXe)
                .FirstOrDefaultAsync(d => d.MaDatXe == id);

            if (datXe == null || datXe.TrangThai != TrangThaiDatXe.DangThue)
                return NotFound();
            
            var model = new TraXe {
                MaDatXe = id,
                ThoiGianTraThucTe = DateTime.Now,
                SoKmTra = datXe.Xe.SoKmHienTai,
                MucNhienLieuTra = datXe.BanGiaoXe?.MucNhienLieuBanGiao ?? 100
            };

            // Lay DonGiaGio tu BangGiaThue
            var bangGia = await _context.BangGiaThues.FirstOrDefaultAsync(b => b.MaXe == datXe.MaXe && b.TrangThai)
                       ?? await _context.BangGiaThues.FirstOrDefaultAsync(b => b.MaLoaiXe == datXe.Xe.MaLoaiXe && b.TrangThai);
            
            ViewBag.DonGiaGio = bangGia?.DonGiaGio ?? 0;
            ViewBag.DatXe = datXe;
            return View(model);
        }

        [HttpPost("/TraXe/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int id, TraXe model)
        {
            if (id != model.MaDatXe) return BadRequest();
            
            var datXe = await _context.DatXes
                .Include(d => d.Xe)
                .FirstOrDefaultAsync(d => d.MaDatXe == id);

            if (datXe == null || datXe.TrangThai != TrangThaiDatXe.DangThue)
                return NotFound();

            if (ModelState.IsValid)
            {
                var banGiao = datXe.BanGiaoXe ?? await _context.BanGiaoXes.FirstOrDefaultAsync(b => b.MaDatXe == id);
                if (banGiao != null && model.SoKmTra < banGiao.SoKmBanGiao)
                {
                    ModelState.AddModelError("SoKmTra", "Số km trả không được nhỏ hơn số km bàn giao.");
                    ViewBag.DatXe = datXe;
                    return View(model);
                }

                datXe.TrangThai = TrangThaiDatXe.ChoThanhToan;
                datXe.Xe.SoKmHienTai = model.SoKmTra;
                
                if (model.PhiHuHong > 0)
                {
                    datXe.Xe.TinhTrang = TinhTrangXe.BaoDuong;
                }
                else
                {
                    datXe.Xe.TinhTrang = TinhTrangXe.SanSang;
                }
                
                _context.TraXes.Add(model);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Đã nhận trả xe và ghi nhận phụ phí thành công. Vui lòng tiến hành thanh toán.";
                return RedirectToAction("ThanhToan", "ThanhToan", new { id = id });
            }

            ViewBag.DatXe = datXe;
            return View(model);
        }
    }
}
// ==============================================================
// Kết thúc Code của SV4
// ==============================================================
