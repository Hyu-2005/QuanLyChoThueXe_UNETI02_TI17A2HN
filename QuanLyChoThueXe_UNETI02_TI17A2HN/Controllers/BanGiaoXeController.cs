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
using QuanLyChoThueXe_UNETI02_TI17A2HN.Services.Interfaces;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Controllers
{
    public class BanGiaoXeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IQRCodeService _qrCodeService;

        public BanGiaoXeController(AppDbContext context, IQRCodeService qrCodeService)
        {
            _context = context;
            _qrCodeService = qrCodeService;
        }

        [SessionAuthorize(Roles = new[] { VaiTro.Admin, VaiTro.NhanVien })]
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

        [SessionAuthorize(Roles = new[] { VaiTro.Admin, VaiTro.NhanVien })]
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
                
                TempData["Success"] = "Đã bàn giao xe cho khách hàng thành công. Dưới đây là biên bản bàn giao điện tử và Mã QR xác thực.";
                return RedirectToAction(nameof(BienBanDienTu), new { id = id });
            }

            ViewBag.DatXe = datXe;
            return View(model);
        }
       
        // ==============================================================
        // Bắt Đầu Code của SV4: Nguyễn Hoàng Đức Hiếu
        // file: Controllers/BanGiaoXeController.cs
        // nội dung: STT 9 - Tạo Biên bản bàn giao điện tử kèm Mã QR xác thực nhanh
        // sinh viên thực hiện : Nguyễn Hoàng Đức Hiếu -  23103100116 - SV4 
        // ==============================================================
        [SessionAuthorize]
        [HttpGet("/BanGiaoXe/BienBanDienTu/{id}")]
        public async Task<IActionResult> BienBanDienTu(int id)
        {
            var datXe = await _context.DatXes
                .Include(d => d.KhachHang)
                .Include(d => d.Xe).ThenInclude(x => x!.HangXe)
                .Include(d => d.Xe).ThenInclude(x => x!.LoaiXe)
                .Include(d => d.BanGiaoXe)
                .FirstOrDefaultAsync(d => d.MaDatXe == id);

            if (datXe == null || datXe.BanGiaoXe == null)
            {
                TempData["Error"] = "Không tìm thấy biên bản bàn giao của đơn đặt xe này.";
                return RedirectToAction("Index", "DatXe");
            }

            // Kiểm tra quyền: nếu là khách hàng, chỉ được xem biên bản đơn xe của chính mình
            var vaiTro = HttpContext.Session.GetVaiTro();
            if (vaiTro == VaiTro.KhachHang)
            {
                var maKhachHang = HttpContext.Session.GetMaKhachHang();
                if (datXe.MaKhachHang != maKhachHang)
                {
                    TempData["Error"] = "Bạn không có quyền truy cập biên bản bàn giao này.";
                    return RedirectToAction("DonCuaToi", "DatXe");
                }
            }

            // Tạo chuỗi nội dung xác thực mã QR
            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            var urlXacThuc = $"{baseUrl}/BanGiaoXe/BienBanDienTu/{id}";

            var noiDungQR = $"BIEN BAN BAN GIAO XE #{id}\n" +
                            $"Bien so: {datXe.Xe?.BienSo}\n" +
                            $"Xe: {datXe.Xe?.TenXe}\n" +
                            $"Khach hang: {datXe.KhachHang?.HoTen} ({datXe.KhachHang?.SoDienThoai})\n" +
                            $"Ngay giao: {datXe.BanGiaoXe.ThoiGianBanGiao:dd/MM/yyyy HH:mm}\n" +
                            $"So km: {datXe.BanGiaoXe.SoKmBanGiao:N0} km\n" +
                            $"Xang: {datXe.BanGiaoXe.MucNhienLieuBanGiao}%\n" +
                            $"Nguoi giao: {datXe.BanGiaoXe.NguoiBanGiao}\n" +
                            $"Xac thuc: {urlXacThuc}";

            var qrBytes = _qrCodeService.GenerateQRCode(noiDungQR, 10);
            ViewBag.QRCodeBase64 = Convert.ToBase64String(qrBytes);
            ViewBag.UrlXacThuc = urlXacThuc;

            return View(datXe);
        }

        [SessionAuthorize]
        [HttpGet("/BanGiaoXe/QRCodeImage/{id}")]
        public async Task<IActionResult> QRCodeImage(int id)
        {
            var datXe = await _context.DatXes
                .Include(d => d.BanGiaoXe)
                .Include(d => d.Xe)
                .Include(d => d.KhachHang)
                .FirstOrDefaultAsync(d => d.MaDatXe == id);

            if (datXe == null || datXe.BanGiaoXe == null)
                return NotFound();

            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            var urlXacThuc = $"{baseUrl}/BanGiaoXe/BienBanDienTu/{id}";

            var noiDungQR = $"BIEN BAN BAN GIAO XE #{id}\n" +
                            $"Bien so: {datXe.Xe?.BienSo}\n" +
                            $"Xe: {datXe.Xe?.TenXe}\n" +
                            $"Khach hang: {datXe.KhachHang?.HoTen}\n" +
                            $"Ngay giao: {datXe.BanGiaoXe.ThoiGianBanGiao:dd/MM/yyyy HH:mm}\n" +
                            $"Xac thuc: {urlXacThuc}";

            var qrBytes = _qrCodeService.GenerateQRCode(noiDungQR, 15);
            return File(qrBytes, "image/png", $"QRCode_BanGiao_{id}.png");
        }
        // ==============================================================
        // Kết thúc Code của SV4
        // ==============================================================
    }
}
