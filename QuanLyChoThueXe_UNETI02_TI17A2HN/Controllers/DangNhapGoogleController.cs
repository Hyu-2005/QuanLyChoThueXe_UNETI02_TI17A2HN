// ============================================================
// File: Controllers/DangNhapGoogleController.cs
// Họ và tên: Vũ Tiến Đạt
// Mã sinh viên: 23103100119
// Nội dung thực hiện: Đăng nhập bằng tài khoản Google (Gmail) cho khách hàng
// Module: Module 3 - Khách hàng
// ============================================================

using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Data;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Helpers;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants;
using TaiKhoanEntity = QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities.TaiKhoan;
using KhachHangEntity = QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities.KhachHang;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Controllers
{
    public class DangNhapGoogleController : Controller
    {
        // Phai trung voi ten scheme cookie tam khai bao trong Program.cs
        private const string ExternalScheme = "DangNhapNgoai";

        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public DangNhapGoogleController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // BUOC 1: chuyen khach sang trang dang nhap cua Google
        [HttpGet]
        public IActionResult Index(string? returnUrl = null)
        {
            if (HttpContext.Session.DaDangNhap())
                return RedirectToAction("Index", "Home");

            if (string.IsNullOrWhiteSpace(_config["Authentication:Google:ClientId"]) ||
                string.IsNullOrWhiteSpace(_config["Authentication:Google:ClientSecret"]))
            {
                TempData["Error"] = "Chức năng đăng nhập bằng Google chưa được cấu hình";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(Callback), "DangNhapGoogle", new { returnUrl })
            };
            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }

        // BUOC 2: Google tra khach ve day sau khi dong y
        [HttpGet]
        public async Task<IActionResult> Callback(string? returnUrl = null)
        {
            var ketQua = await HttpContext.AuthenticateAsync(ExternalScheme);
            var principal = ketQua.Principal;

            // Cookie tam chi dung 1 lan, xoa ngay
            await HttpContext.SignOutAsync(ExternalScheme);

            var email = principal?.FindFirst(ClaimTypes.Email)?.Value?.Trim().ToLowerInvariant();
            if (!ketQua.Succeeded || string.IsNullOrWhiteSpace(email))
            {
                TempData["Error"] = "Đăng nhập Google không thành công. Vui lòng thử lại";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            // Chi gop voi tai khoan KHACH HANG co cung email (khong cho vao tai khoan admin/nhan vien)
            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.KhachHang)
                .FirstOrDefaultAsync(t => t.Email == email && t.VaiTro == VaiTro.KhachHang);

            bool taiKhoanMoi = false;

            if (taiKhoan == null)
            {
                // Lan dau dang nhap bang Google -> tu dong tao tai khoan khach hang
                var hoTen = principal!.FindFirst(ClaimTypes.Name)?.Value?.Trim();
                if (string.IsNullOrWhiteSpace(hoTen)) hoTen = email.Split('@')[0];
                if (hoTen.Length > 100) hoTen = hoTen[..100];

                var tenDangNhap = email.Length <= 50 ? email : TaoTenDangNhap();
                if (await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap == tenDangNhap))
                    tenDangNhap = TaoTenDangNhap();

                taiKhoan = new TaiKhoanEntity
                {
                    TenDangNhap = tenDangNhap,
                    // Mat khau ngau nhien, khong ai biet: tai khoan nay chi dang nhap bang Google
                    MatKhau = Convert.ToHexString(RandomNumberGenerator.GetBytes(16)),
                    HoTen = hoTen,
                    Email = email,
                    VaiTro = VaiTro.KhachHang,
                    TrangThai = true,
                    KhachHang = new KhachHangEntity
                    {
                        HoTen = hoTen,
                        SoDienThoai = string.Empty,   // Google khong cung cap SDT
                        Email = email,
                        TrangThai = true
                    }
                };
                _context.TaiKhoans.Add(taiKhoan);
                taiKhoanMoi = true;
            }
            else
            {
                if (!taiKhoan.TrangThai)
                {
                    TempData["Error"] = "Tài khoản đã bị khóa. Vui lòng liên hệ quản trị viên";
                    return RedirectToAction("DangNhap", "TaiKhoan");
                }

                // Tai khoan khach hang chua co ho so khach hang -> tao bo sung
                if (taiKhoan.KhachHang == null)
                {
                    taiKhoan.KhachHang = new KhachHangEntity
                    {
                        HoTen = taiKhoan.HoTen,
                        SoDienThoai = string.Empty,
                        Email = taiKhoan.Email,
                        TrangThai = true
                    };
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Không thể tạo tài khoản từ Google, vui lòng thử lại";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            // Luu Session giong man hinh dang nhap thuong
            HttpContext.Session.SetInt(SessionKeys.MaTaiKhoan, taiKhoan.MaTaiKhoan);
            HttpContext.Session.SetString(SessionKeys.TenDangNhap, taiKhoan.TenDangNhap);
            HttpContext.Session.SetString(SessionKeys.HoTen, taiKhoan.HoTen);
            HttpContext.Session.SetString(SessionKeys.VaiTro, taiKhoan.VaiTro);
            if (taiKhoan.KhachHang != null)
                HttpContext.Session.SetInt(SessionKeys.MaKhachHang, taiKhoan.KhachHang.MaKhachHang);

            if (taiKhoanMoi)
            {
                TempData["Success"] = $"Chào mừng {taiKhoan.HoTen}! Hãy tải CCCD và GPLX để được xác minh.";
                return RedirectToAction("CuaToi", "GiayTo");
            }

            TempData["Success"] = $"Xin chào {taiKhoan.HoTen}!";

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        private static string TaoTenDangNhap() => "gg_" + Guid.NewGuid().ToString("N")[..16];
    }
}