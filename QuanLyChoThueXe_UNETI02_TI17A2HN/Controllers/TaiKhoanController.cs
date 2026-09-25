// File: Controllers/TaiKhoanController.cs
// Noi dung: Dang nhap, dang xuat, doi mat khau, AccessDenied
// Sinh vien thuc hien: Pham Gia Minh Hoang - 23103100087 - SV1
// Module: Module 1 - Tai khoan, Dang nhap, Phan quyen, Loai xe, Hang xe

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Data;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Helpers;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants;
using QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.TaiKhoan;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly AppDbContext _context;

        public TaiKhoanController(AppDbContext context)
        {
            _context = context;
        }

        // DANG NHAP
        [HttpGet]
        public IActionResult DangNhap(string? returnUrl = null)
        {
            // Neu da dang nhap thi chuyen ve trang chu
            if (HttpContext.Session.DaDangNhap())
                return RedirectToAction("Index", "Home");

            return View(new DangNhapVM { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangNhap(DangNhapVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // 1. Tim tai khoan theo TenDangNhap (dung EF Core + LINQ)
            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.KhachHang)
                .FirstOrDefaultAsync(t => t.TenDangNhap == vm.TenDangNhap);

            // 2. Kiem tra ton tai
            if (taiKhoan == null)
            {
                ModelState.AddModelError(string.Empty,
                    "Ten dang nhap hoac mat khau khong dung");
                return View(vm);
            }

            // 3. Kiem tra mat khau (so sanh thuong, co the nang cap hash sau)
            if (taiKhoan.MatKhau != vm.MatKhau)
            {
                ModelState.AddModelError(string.Empty,
                    "Ten dang nhap hoac mat khau khong dung");
                return View(vm);
            }

            // 4. Kiem tra trang thai tai khoan
            if (!taiKhoan.TrangThai)
            {
                ModelState.AddModelError(string.Empty,
                    "Tai khoan da bi khoa. Vui long lien he quan tri vien");
                return View(vm);
            }

            // 5. Luu Session
            HttpContext.Session.SetInt(SessionKeys.MaTaiKhoan, taiKhoan.MaTaiKhoan);
            HttpContext.Session.SetString(SessionKeys.TenDangNhap, taiKhoan.TenDangNhap);
            HttpContext.Session.SetString(SessionKeys.HoTen, taiKhoan.HoTen);
            HttpContext.Session.SetString(SessionKeys.VaiTro, taiKhoan.VaiTro);

            // Neu la khach hang thi luu them MaKhachHang
            if (taiKhoan.VaiTro == VaiTro.KhachHang && taiKhoan.KhachHang != null)
            {
                HttpContext.Session.SetInt(SessionKeys.MaKhachHang,
                    taiKhoan.KhachHang.MaKhachHang);
            }

            TempData["Success"] = $"Xin chao {taiKhoan.HoTen}!";

            // 6. Chuyen huong theo vai tro
            if (!string.IsNullOrEmpty(vm.ReturnUrl) && Url.IsLocalUrl(vm.ReturnUrl))
                return Redirect(vm.ReturnUrl);

            return taiKhoan.VaiTro switch
            {
                VaiTro.Admin => RedirectToAction("Index", "ThongKe"),
                VaiTro.NhanVien => RedirectToAction("Index", "ThongKe"),
                VaiTro.KhachHang => RedirectToAction("Index", "Home"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        // DANG XUAT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("DangNhap");
        }

        [HttpGet]
        public IActionResult DangXuat(string xacNhan)
        {
            // Cho phep dang xuat bang GET trong truong hop khan cap
            HttpContext.Session.Clear();
            return RedirectToAction("DangNhap");
        }


        // DOI MAT KHAU
        [SessionAuthorize]
        [HttpGet]
        public IActionResult DoiMatKhau()
        {
            return View(new DoiMatKhauVM());
        }

        [SessionAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiMatKhau(DoiMatKhauVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var maTaiKhoan = HttpContext.Session.GetMaTaiKhoan();
            if (!maTaiKhoan.HasValue)
                return RedirectToAction("DangNhap");

            var taiKhoan = await _context.TaiKhoans.FindAsync(maTaiKhoan.Value);
            if (taiKhoan == null)
                return RedirectToAction("DangNhap");

            // Kiem tra mat khau cu
            if (taiKhoan.MatKhau != vm.MatKhauCu)
            {
                ModelState.AddModelError(nameof(vm.MatKhauCu),
                    "Mat khau cu khong dung");
                return View(vm);
            }

            // Cap nhat mat khau moi
            taiKhoan.MatKhau = vm.MatKhauMoi;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Doi mat khau thanh cong. Vui long dang nhap lai";

            // Xoa Session de dang nhap lai
            HttpContext.Session.Clear();
            return RedirectToAction("DangNhap");
        }

        // ACCESS DENIED
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}