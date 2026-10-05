// ============================================================
// File: Controllers/KhachHangController.cs
// Họ và tên: Vu Tien Dat
// Mã sinh viên: 23103100119
// Nội dung thực hiện: Danh sách khách hàng
//
// Họ và tên: Duong Lam Huy
// Mã sinh viên: 23103100120
// Nội dung thực hiện: Xem lịch sử thuê của khách hàng
// Module: Module 3 + Module 5
// ============================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Data;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Helpers;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants;
using QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.KhachHang;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Controllers
{
    [SessionAuthorize(Roles = new[] { VaiTro.Admin, VaiTro.NhanVien })]
    public class KhachHangController : Controller
    {
        private readonly AppDbContext _context;

        public KhachHangController(AppDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // DANH SACH KHACH HANG (SV3)
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Index(
            string? tuKhoa,
            bool? trangThai,
            int trang = 1,
            int kichThuocTrang = 10)
        {
            var query = _context.KhachHangs
                .Include(k => k.TaiKhoan)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var kw = tuKhoa.Trim();
                query = query.Where(k =>
                    k.HoTen.Contains(kw) ||
                    k.SoDienThoai.Contains(kw) ||
                    (k.Email != null && k.Email.Contains(kw)));
            }

            if (trangThai.HasValue)
                query = query.Where(k => k.TrangThai == trangThai.Value);

            var tongSo = await query.CountAsync();
            if (trang < 1) trang = 1;

            var danhSach = await query
                .OrderByDescending(k => k.MaKhachHang)
                .Skip((trang - 1) * kichThuocTrang)
                .Take(kichThuocTrang)
                .ToListAsync();

            var vm = new KhachHangListVM
            {
                DanhSach = danhSach,
                TuKhoa = tuKhoa,
                TrangThai = trangThai,
                Trang = trang,
                KichThuocTrang = kichThuocTrang,
                TongSoBanGhi = tongSo
            };

            return View(vm);
        }

        // =====================================================
        // LICH SU THUE (SV5)
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> LichSu(int id)
        {
            var khachHang = await _context.KhachHangs
                .Include(k => k.TaiKhoan)
                .AsNoTracking()
                .FirstOrDefaultAsync(k => k.MaKhachHang == id);

            if (khachHang == null)
            {
                TempData["Error"] = "Khong tim thay khach hang";
                return RedirectToAction(nameof(Index));
            }

            var danhSachDon = await _context.DatXes
                .Include(d => d.Xe).ThenInclude(x => x!.LoaiXe)
                .Include(d => d.Xe).ThenInclude(x => x!.HangXe)
                .Include(d => d.TraXe)
                .Include(d => d.ThanhToans)
                .Where(d => d.MaKhachHang == id)
                .OrderByDescending(d => d.NgayDat)
                .AsNoTracking()
                .ToListAsync();

            var vm = new LichSuThueVM
            {
                KhachHang = khachHang,
                DanhSachDon = danhSachDon,
                TongSoDon = danhSachDon.Count,
                SoDonHoanThanh = danhSachDon.Count(d => d.TrangThai == TrangThaiDatXe.HoanThanh),
                SoDonHuy = danhSachDon.Count(d => d.TrangThai == TrangThaiDatXe.DaHuy
                                                || d.TrangThai == TrangThaiDatXe.TuChoi),
                TongChiTieu = danhSachDon
                    .Where(d => d.TrangThai == TrangThaiDatXe.HoanThanh)
                    .SelectMany(d => d.ThanhToans)
                    .Where(t => t.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    .Sum(t => t.TongThanhToan),
                TongPhuPhi = danhSachDon
                    .Where(d => d.TraXe != null)
                    .Sum(d => d.TraXe!.TongPhuPhi)
            };

            return View(vm);
        }

        // =====================================================
        // KHOA/MO KHOA KHACH HANG (SV3)
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiTrangThai(int id)
        {
            var khachHang = await _context.KhachHangs.FindAsync(id);
            if (khachHang == null)
            {
                TempData["Error"] = "Khong tim thay khach hang";
                return RedirectToAction(nameof(Index));
            }

            khachHang.TrangThai = !khachHang.TrangThai;
            await _context.SaveChangesAsync();

            TempData["Success"] = khachHang.TrangThai
                ? "Da mo khoa khach hang"
                : "Da khoa khach hang";

            return RedirectToAction(nameof(Index));
        }
    }
}