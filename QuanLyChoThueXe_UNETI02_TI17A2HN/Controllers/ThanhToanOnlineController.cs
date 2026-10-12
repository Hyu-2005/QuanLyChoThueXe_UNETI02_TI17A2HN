// File: Controllers/ThanhToanOnlineController.cs
// Nội dung: Thanh toán trực tuyến giả lập (VNPay, MoMo, ZaloPay, Banking)
// Sinh viên thực hiện: Dương Lâm Huy - 23103100120 - SV5
// Module: Module 5 - Tính tiền, Thanh toán, Lịch sử, Thống kê

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Data;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Helpers;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Services.Interfaces;
using QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.ThanhToan;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Controllers
{
    [SessionAuthorize(Roles = new[] { VaiTro.Admin, VaiTro.NhanVien, VaiTro.KhachHang })]
    public class ThanhToanOnlineController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IQRCodeService _qrCodeService;

        // CHỈ 1 CONSTRUCTOR DUY NHẤT
        public ThanhToanOnlineController(
            AppDbContext context,
            IQRCodeService qrCodeService)
        {
            _context = context;
            _qrCodeService = qrCodeService;
        }

        // =====================================================
        // BƯỚC 1: CHỌN CỔNG THANH TOÁN
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            var thanhToan = await _context.ThanhToans
                .Include(t => t.DatXe).ThenInclude(d => d!.KhachHang)
                .Include(t => t.DatXe).ThenInclude(d => d!.Xe)
                .FirstOrDefaultAsync(t => t.MaThanhToan == id);

            if (thanhToan == null)
            {
                TempData["Error"] = "Không tìm thấy hóa đơn";
                return RedirectToAction("Index", "ThanhToan");
            }

            // Kiểm tra quyền: khách hàng chỉ xem hóa đơn của mình
            var vaiTro = HttpContext.Session.GetVaiTro();
            if (vaiTro == VaiTro.KhachHang)
            {
                var maKh = HttpContext.Session.GetMaKhachHang();
                if (thanhToan.DatXe?.MaKhachHang != maKh)
                {
                    TempData["Error"] = "Bạn không có quyền truy cập hóa đơn này";
                    return RedirectToAction("Index", "ThanhToan");
                }
            }

            if (thanhToan.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
            {
                TempData["Error"] = "Hóa đơn này đã được thanh toán";
                return RedirectToAction("Details", "ThanhToan", new { id });
            }

            var soTienConLai = thanhToan.SoTienConLai > 0
                ? thanhToan.SoTienConLai
                : thanhToan.TongThanhToan;

            var vm = new ThanhToanOnlineVM
            {
                MaThanhToan = thanhToan.MaThanhToan,
                TenKhachHang = thanhToan.DatXe?.KhachHang?.HoTen,
                SoDienThoai = thanhToan.DatXe?.KhachHang?.SoDienThoai,
                TenXe = thanhToan.DatXe?.Xe?.TenXe,
                BienSo = thanhToan.DatXe?.Xe?.BienSo,
                SoTien = soTienConLai
            };

            return View(vm);
        }

        // =====================================================
        // BƯỚC 2: TẠO GIAO DỊCH VÀ HIỂN THỊ MÃ QR
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaoGiaoDich(ThanhToanOnlineVM vm)
        {
            if (!ModelState.IsValid)
                return View("Index", vm);

            var thanhToan = await _context.ThanhToans
                .Include(t => t.DatXe).ThenInclude(d => d!.KhachHang)
                .Include(t => t.DatXe).ThenInclude(d => d!.Xe)
                .FirstOrDefaultAsync(t => t.MaThanhToan == vm.MaThanhToan);

            if (thanhToan == null)
            {
                TempData["Error"] = "Không tìm thấy hóa đơn";
                return RedirectToAction("Index", "ThanhToan");
            }

            if (thanhToan.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
            {
                TempData["Error"] = "Hóa đơn này đã được thanh toán";
                return RedirectToAction("Details", "ThanhToan", new { id = vm.MaThanhToan });
            }

            // Tạo mã giao dịch duy nhất
            var maGD = $"GD{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(100, 999)}";

            var giaoDich = new GiaoDichOnline
            {
                MaThanhToan = thanhToan.MaThanhToan,
                MaGiaoDichCode = maGD,
                CongThanhToan = vm.CongThanhToan,
                SoTien = vm.SoTien,
                TrangThai = TrangThaiGiaoDich.ChoThanhToan,
                NgayTao = DateTime.Now,
                GhiChu = $"Tạo giao dịch qua {vm.CongThanhToan} (giả lập)"
            };

            _context.GiaoDichOnlines.Add(giaoDich);
            await _context.SaveChangesAsync();

            // Tạo nội dung QR
            var noiDung = $"Thanh toan HD#{thanhToan.MaThanhToan} | " +
                          $"KH: {thanhToan.DatXe?.KhachHang?.HoTen} | " +
                          $"So tien: {vm.SoTien:N0}d | " +
                          $"Ma GD: {maGD} | " +
                          $"Cong: {vm.CongThanhToan}";

            var qrBytes = _qrCodeService.GenerateQRCode(noiDung, 15);
            var qrBase64 = Convert.ToBase64String(qrBytes);

            vm.MaGiaoDichCode = maGD;
            vm.QRCodeBase64 = qrBase64;
            vm.NoiDung = noiDung;
            vm.TenKhachHang = thanhToan.DatXe?.KhachHang?.HoTen;
            vm.SoDienThoai = thanhToan.DatXe?.KhachHang?.SoDienThoai;
            vm.TenXe = thanhToan.DatXe?.Xe?.TenXe;
            vm.BienSo = thanhToan.DatXe?.Xe?.BienSo;

            return View("QRCode", vm);
        }

        // =====================================================
        // BƯỚC 3: XÁC NHẬN THANH TOÁN (giả lập callback)
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanThanhToan(string maGiaoDich)
        {
            var giaoDich = await _context.GiaoDichOnlines
                .Include(g => g.ThanhToan)
                    .ThenInclude(t => t!.DatXe)
                .FirstOrDefaultAsync(g => g.MaGiaoDichCode == maGiaoDich);

            if (giaoDich == null)
            {
                TempData["Error"] = "Không tìm thấy giao dịch";
                return RedirectToAction("Index", "ThanhToan");
            }

            if (giaoDich.TrangThai == TrangThaiGiaoDich.ThanhCong)
            {
                TempData["Error"] = "Giao dịch đã được xử lý trước đó";
                return RedirectToAction("Details", "ThanhToan",
                    new { id = giaoDich.MaThanhToan });
            }

            if (giaoDich.TrangThai == TrangThaiGiaoDich.DaHuy)
            {
                TempData["Error"] = "Giao dịch đã bị hủy";
                return RedirectToAction("Index", "ThanhToan");
            }

            // Cập nhật giao dịch
            giaoDich.TrangThai = TrangThaiGiaoDich.ThanhCong;
            giaoDich.NgayThanhToan = DateTime.Now;
            giaoDich.GhiChu = $"Thanh toán thành công qua {giaoDich.CongThanhToan} (giả lập)";

            // Cập nhật hóa đơn
            if (giaoDich.ThanhToan != null)
            {
                giaoDich.ThanhToan.TrangThaiThanhToan = TrangThaiThanhToan.DaThanhToan;
                giaoDich.ThanhToan.NgayThanhToan = DateTime.Now;
                giaoDich.ThanhToan.PhuongThucThanhToan = giaoDich.CongThanhToan;
                giaoDich.ThanhToan.GhiChu =
                    $"Thanh toán online qua {giaoDich.CongThanhToan} (Mã GD: {giaoDich.MaGiaoDichCode})";

                // Chuyển đơn sang Hoàn thành
                if (giaoDich.ThanhToan.DatXe != null)
                {
                    giaoDich.ThanhToan.DatXe.TrangThai = TrangThaiDatXe.HoanThanh;
                }
            }

            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"Thanh toán thành công! Mã giao dịch: {maGiaoDich}";

            return RedirectToAction("Details", "ThanhToan",
                new { id = giaoDich.MaThanhToan });
        }

        // =====================================================
        // HỦY GIAO DỊCH
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyGiaoDich(string maGiaoDich)
        {
            var giaoDich = await _context.GiaoDichOnlines
                .FirstOrDefaultAsync(g => g.MaGiaoDichCode == maGiaoDich);

            if (giaoDich == null)
            {
                TempData["Error"] = "Không tìm thấy giao dịch";
                return RedirectToAction("Index", "ThanhToan");
            }

            if (giaoDich.TrangThai == TrangThaiGiaoDich.ThanhCong)
            {
                TempData["Error"] = "Giao dịch đã thành công, không thể hủy";
                return RedirectToAction("Details", "ThanhToan",
                    new { id = giaoDich.MaThanhToan });
            }

            giaoDich.TrangThai = TrangThaiGiaoDich.DaHuy;
            giaoDich.GhiChu = "Khách hàng hủy giao dịch";
            giaoDich.NgayThanhToan = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Warning"] = "Đã hủy giao dịch";
            return RedirectToAction("Index", "ThanhToan");
        }
    }
}