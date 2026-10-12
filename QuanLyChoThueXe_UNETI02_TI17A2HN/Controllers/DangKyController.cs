// ============================================================
// File: Controllers/DangKyController.cs
// Họ và tên: [Vũ Tiến Đạt]
// Mã sinh viên: [23103100119]
// Nội dung thực hiện: Khách hàng tự đăng ký tài khoản (tạo TaiKhoan + KhachHang)
// Module: Module 3 - Khách hàng
// ============================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Data;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Helpers;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants;
using QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.TaiKhoan;
using TaiKhoanEntity = QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities.TaiKhoan;
using KhachHangEntity = QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities.KhachHang;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Controllers
{
    public class DangKyController : Controller
    {
        private readonly AppDbContext _context;

        public DangKyController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Da dang nhap thi khong can dang ky
            if (HttpContext.Session.DaDangNhap())
                return RedirectToAction("Index", "Home");

            return View(new DangKyVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(DangKyVM vm)
        {
            if (HttpContext.Session.DaDangNhap())
                return RedirectToAction("Index", "Home");

            var tenDangNhap = vm.TenDangNhap?.Trim() ?? string.Empty;
            var email = vm.Email?.Trim() ?? string.Empty;
            var soDienThoai = vm.SoDienThoai?.Trim() ?? string.Empty;

            // Kiem tra trung
            if (tenDangNhap.Length > 0 &&
                await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap == tenDangNhap))
                ModelState.AddModelError(nameof(vm.TenDangNhap), "Tên đăng nhập đã tồn tại");

            if (email.Length > 0 &&
                await _context.TaiKhoans.AnyAsync(t => t.Email == email))
                ModelState.AddModelError(nameof(vm.Email), "Email đã được sử dụng");

            if (soDienThoai.Length > 0 &&
                await _context.KhachHangs.AnyAsync(k => k.SoDienThoai == soDienThoai))
                ModelState.AddModelError(nameof(vm.SoDienThoai),
                    "Số điện thoại đã được đăng ký. Nếu là của bạn, hãy liên hệ nhân viên để được hỗ trợ");

            if (!ModelState.IsValid)
                return View(vm);

            var hoTen = vm.HoTen.Trim();

            var taiKhoan = new TaiKhoanEntity
            {
                TenDangNhap = tenDangNhap,
                MatKhau = vm.MatKhau,           // giu cung cach luu nhu man hinh dang nhap hien tai
                HoTen = hoTen,
                Email = email,
                VaiTro = VaiTro.KhachHang,
                TrangThai = true
            };

            var khachHang = new KhachHangEntity
            {
                HoTen = hoTen,
                SoDienThoai = soDienThoai,
                Email = email,
                DiaChi = string.IsNullOrWhiteSpace(vm.DiaChi) ? null : vm.DiaChi.Trim(),
                TrangThai = true,
                TaiKhoan = taiKhoan             // EF tu gan MaTaiKhoan, luu 1 lan SaveChanges (atomic)
            };

            try
            {
                _context.KhachHangs.Add(khachHang);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty,
                    "Không thể tạo tài khoản (tên đăng nhập có thể vừa bị đăng ký). Vui lòng thử lại");
                return View(vm);
            }

            TempData["Success"] = "Đăng ký thành công. Hãy đăng nhập và tải giấy tờ để được kiểm tra.";
            return RedirectToAction("DangNhap", "TaiKhoan");
        }
    }
}
