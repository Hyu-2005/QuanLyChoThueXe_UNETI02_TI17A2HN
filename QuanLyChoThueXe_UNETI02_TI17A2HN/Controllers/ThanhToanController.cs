// ThanhToanController.cs - quan ly thanh toan
// sinh vien thuc hien: Duong Lam Huy - 23103100120
// module 5 - tinh tien, thanh toan, lich su va thong ke

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Data;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Helpers;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Services.Interfaces;
using QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.ThanhToan;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Controllers
{
    [SessionAuthorize]
    public class ThanhToanController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ITinhTienService _tinhTienService;

        public ThanhToanController(AppDbContext context, ITinhTienService tinhTienService)
        {
            _context = context;
            _tinhTienService = tinhTienService;
        }

        // =====================================================
        // DANH SACH THANH TOAN
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Index(
            string? tuKhoa,
            string? trangThaiThanhToan,
            string? phuongThucThanhToan,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? sapXepTheo,
            string? thuTuSapXep,
            int trang = 1,
            int kichThuocTrang = 10)
        {
            var vaiTro = HttpContext.Session.GetVaiTro();
            var laKhachHang = vaiTro == VaiTro.KhachHang;

            var query = _context.ThanhToans
                .Include(t => t.DatXe)
                    .ThenInclude(d => d!.KhachHang)
                .Include(t => t.DatXe)
                    .ThenInclude(d => d!.Xe)
                .AsNoTracking()
                .AsQueryable();

            // Khach hang chi xem thanh toan cua minh
            if (laKhachHang)
            {
                var maKhachHang = HttpContext.Session.GetMaKhachHang();
                query = query.Where(t => t.DatXe != null
                                      && t.DatXe.MaKhachHang == maKhachHang);
            }

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var kw = tuKhoa.Trim();
                query = query.Where(t =>
                    (t.DatXe != null && t.DatXe.KhachHang != null
                        && t.DatXe.KhachHang.HoTen.Contains(kw)) ||
                    (t.DatXe != null && t.DatXe.Xe != null
                        && t.DatXe.Xe.BienSo.Contains(kw)));
            }

            if (!string.IsNullOrWhiteSpace(trangThaiThanhToan))
                query = query.Where(t => t.TrangThaiThanhToan == trangThaiThanhToan);

            if (!string.IsNullOrWhiteSpace(phuongThucThanhToan))
                query = query.Where(t => t.PhuongThucThanhToan == phuongThucThanhToan);

            if (tuNgay.HasValue)
                query = query.Where(t => t.NgayThanhToan >= tuNgay.Value);
            if (denNgay.HasValue)
                query = query.Where(t => t.NgayThanhToan <= denNgay.Value);

            query = (sapXepTheo?.ToLower(), thuTuSapXep?.ToLower()) switch
            {
                ("ngaythanhtoan", "asc") => query.OrderBy(t => t.NgayThanhToan),
                ("ngaythanhtoan", _) => query.OrderByDescending(t => t.NgayThanhToan),
                ("tongtien", "asc") => query.OrderBy(t => t.TongThanhToan),
                ("tongtien", _) => query.OrderByDescending(t => t.TongThanhToan),
                _ => query.OrderByDescending(t => t.NgayThanhToan)
            };

            var tongSo = await query.CountAsync();
            if (trang < 1) trang = 1;

            var danhSach = await query
                .Skip((trang - 1) * kichThuocTrang)
                .Take(kichThuocTrang)
                .ToListAsync();

            var vm = new ThanhToanListVM
            {
                DanhSach = danhSach,
                TuKhoa = tuKhoa,
                TrangThaiThanhToan = trangThaiThanhToan,
                PhuongThucThanhToan = phuongThucThanhToan,
                TuNgay = tuNgay,
                DenNgay = denNgay,
                SapXepTheo = sapXepTheo,
                ThuTuSapXep = thuTuSapXep,
                Trang = trang,
                KichThuocTrang = kichThuocTrang,
                TongSoBanGhi = tongSo,
                DanhSachTrangThai = LayDanhSachTrangThai(),
                DanhSachPhuongThuc = LayDanhSachPhuongThuc(),
                TongDoanhThu = danhSach
                    .Where(t => t.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    .Sum(t => t.TongThanhToan)
            };

            return View(vm);
        }

        // =====================================================
        // THANH TOAN DON - GET
        // =====================================================

        [HttpGet]
        [SessionAuthorize(Roles = new[] { VaiTro.Admin, VaiTro.NhanVien })]
        public async Task<IActionResult> ThanhToan(int id)
        {
            var datXe = await _context.DatXes
                .Include(d => d.KhachHang)
                .Include(d => d.Xe)
                .Include(d => d.TraXe)
                .Include(d => d.ThanhToans)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.MaDatXe == id);

            if (datXe == null)
            {
                TempData["Error"] = "Khong tim thay don dat xe";
                return RedirectToAction(nameof(Index));
            }

            // Chi don Cho thanh toan moi duoc thanh toan
            if (datXe.TrangThai != TrangThaiDatXe.ChoThanhToan)
            {
                TempData["Error"] = $"Don dang o trang thai '{datXe.TrangThai}', khong the thanh toan";
                return RedirectToAction(nameof(Index));
            }

            // Kiem tra khong thanh toan lai
            var daThanhToan = datXe.ThanhToans
                .Any(t => t.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan);

            if (daThanhToan)
            {
                TempData["Error"] = "Don nay da duoc thanh toan truoc do";
                return RedirectToAction(nameof(Index));
            }

            // Tinh toan
            var ketQua = _tinhTienService.TinhThanhToan(datXe, datXe.TraXe);

            var vm = new ThanhToanVM
            {
                MaDatXe = datXe.MaDatXe,
                BienSo = datXe.Xe?.BienSo,
                TenXe = datXe.Xe?.TenXe,
                TenKhachHang = datXe.KhachHang?.HoTen,
                SoDienThoaiKhachHang = datXe.KhachHang?.SoDienThoai,
                ThoiGianNhanDuKien = datXe.ThoiGianNhanDuKien,
                ThoiGianTraDuKien = datXe.ThoiGianTraDuKien,
                ThoiGianTraThucTe = datXe.TraXe?.ThoiGianTraThucTe,
                SoNgayThue = ketQua.SoNgayThue,
                TienThue = ketQua.TienThue,
                TongPhuPhi = ketQua.TongPhuPhi,
                TienCocDaThu = ketQua.TienCocDaThu,
                TongThanhToan = ketQua.TongThanhToan,
                SoTienConLai = ketQua.SoTienConLai,
                PhiQuaHan = datXe.TraXe?.PhiQuaHan ?? 0,
                PhiVuotKm = datXe.TraXe?.PhiVuotKm ?? 0,
                PhiNhienLieu = datXe.TraXe?.PhiNhienLieu ?? 0,
                PhiHuHong = datXe.TraXe?.PhiHuHong ?? 0,
                PhuongThucThanhToan = PhuongThucThanhToan.TienMat,
                DanhSachPhuongThuc = LayDanhSachPhuongThuc()
            };

            return View(vm);
        }

        // =====================================================
        // THANH TOAN DON - POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize(Roles = new[] { VaiTro.Admin, VaiTro.NhanVien })]
        public async Task<IActionResult> ThanhToan(int id, ThanhToanVM vm)
        {
            if (id != vm.MaDatXe)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                await LoadThongTinThanhToanAsync(vm);
                return View(vm);
            }

            var datXe = await _context.DatXes
                .Include(d => d.TraXe)
                .Include(d => d.ThanhToans)
                .FirstOrDefaultAsync(d => d.MaDatXe == id);

            if (datXe == null)
            {
                TempData["Error"] = "Khong tim thay don dat xe";
                return RedirectToAction(nameof(Index));
            }

            if (datXe.TrangThai != TrangThaiDatXe.ChoThanhToan)
            {
                TempData["Error"] = $"Don dang o trang thai '{datXe.TrangThai}', khong the thanh toan";
                return RedirectToAction(nameof(Index));
            }

            var daThanhToan = datXe.ThanhToans
                .Any(t => t.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan);

            if (daThanhToan)
            {
                TempData["Error"] = "Don nay da duoc thanh toan truoc do";
                return RedirectToAction(nameof(Index));
            }

            // Tinh lai tu DB (khong tin du lieu client)
            var ketQua = _tinhTienService.TinhThanhToan(datXe, datXe.TraXe);

            // Kiem tra xem da co ban ghi Chua Thanh Toan nao chua (thuong do Seed tao)
            var thanhToan = datXe.ThanhToans.FirstOrDefault(t => t.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan);
            
            if (thanhToan != null)
            {
                // Cap nhat ban ghi hien tai
                thanhToan.TienThue = ketQua.TienThue;
                thanhToan.TongPhuPhi = ketQua.TongPhuPhi;
                thanhToan.TienCocDaThu = ketQua.TienCocDaThu;
                thanhToan.TongThanhToan = ketQua.TongThanhToan;
                thanhToan.SoTienConLai = ketQua.SoTienConLai;
                thanhToan.PhuongThucThanhToan = vm.PhuongThucThanhToan;
                thanhToan.NgayThanhToan = DateTime.Now;
                thanhToan.TrangThaiThanhToan = TrangThaiThanhToan.DaThanhToan;
                thanhToan.GhiChu = vm.GhiChu?.Trim();
            }
            else
            {
                // Tao ban ghi thanh toan moi
                thanhToan = new Models.Entities.ThanhToan
                {
                    MaDatXe = datXe.MaDatXe,
                    TienThue = ketQua.TienThue,
                    TongPhuPhi = ketQua.TongPhuPhi,
                    TienCocDaThu = ketQua.TienCocDaThu,
                    TongThanhToan = ketQua.TongThanhToan,
                    SoTienConLai = ketQua.SoTienConLai,
                    PhuongThucThanhToan = vm.PhuongThucThanhToan,
                    NgayThanhToan = DateTime.Now,
                    TrangThaiThanhToan = TrangThaiThanhToan.DaThanhToan,
                    GhiChu = vm.GhiChu?.Trim()
                };
                _context.ThanhToans.Add(thanhToan);
            }

            // Chuyen don sang Hoan thanh
            datXe.TrangThai = TrangThaiDatXe.HoanThanh;

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Da thanh toan don #{datXe.MaDatXe} thanh cong";
            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // CHI TIET THANH TOAN
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var thanhToan = await _context.ThanhToans
                .Include(t => t.DatXe)
                    .ThenInclude(d => d!.KhachHang)
                .Include(t => t.DatXe)
                    .ThenInclude(d => d!.Xe)
                .Include(t => t.DatXe)
                    .ThenInclude(d => d!.TraXe)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.MaThanhToan == id);

            if (thanhToan == null)
            {
                TempData["Error"] = "Khong tim thay ban ghi thanh toan";
                return RedirectToAction(nameof(Index));
            }

            // Kiem tra quyen: khach hang chi xem cua minh
            var vaiTro = HttpContext.Session.GetVaiTro();
            if (vaiTro == VaiTro.KhachHang)
            {
                var maKhachHang = HttpContext.Session.GetMaKhachHang();
                if (thanhToan.DatXe?.MaKhachHang != maKhachHang)
                {
                    TempData["Error"] = "Ban khong co quyen xem thanh toan nay";
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(thanhToan);
        }

        // =====================================================
        // HELPER
        // =====================================================

        private async Task LoadThongTinThanhToanAsync(ThanhToanVM vm)
        {
            var datXe = await _context.DatXes
                .Include(d => d.KhachHang)
                .Include(d => d.Xe)
                .Include(d => d.TraXe)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.MaDatXe == vm.MaDatXe);

            if (datXe == null) return;

            vm.BienSo = datXe.Xe?.BienSo;
            vm.TenXe = datXe.Xe?.TenXe;
            vm.TenKhachHang = datXe.KhachHang?.HoTen;
            vm.SoDienThoaiKhachHang = datXe.KhachHang?.SoDienThoai;
            vm.ThoiGianNhanDuKien = datXe.ThoiGianNhanDuKien;
            vm.ThoiGianTraDuKien = datXe.ThoiGianTraDuKien;
            vm.ThoiGianTraThucTe = datXe.TraXe?.ThoiGianTraThucTe;
            vm.PhiQuaHan = datXe.TraXe?.PhiQuaHan ?? 0;
            vm.PhiVuotKm = datXe.TraXe?.PhiVuotKm ?? 0;
            vm.PhiNhienLieu = datXe.TraXe?.PhiNhienLieu ?? 0;
            vm.PhiHuHong = datXe.TraXe?.PhiHuHong ?? 0;

            var ketQua = _tinhTienService.TinhThanhToan(datXe, datXe.TraXe);
            vm.SoNgayThue = ketQua.SoNgayThue;
            vm.TienThue = ketQua.TienThue;
            vm.TongPhuPhi = ketQua.TongPhuPhi;
            vm.TienCocDaThu = ketQua.TienCocDaThu;
            vm.TongThanhToan = ketQua.TongThanhToan;
            vm.SoTienConLai = ketQua.SoTienConLai;

            vm.DanhSachPhuongThuc = LayDanhSachPhuongThuc();
        }

        private List<SelectListItem> LayDanhSachTrangThai()
        {
            return TrangThaiThanhToan.TatCa
                .Select(t => new SelectListItem { Value = t, Text = t })
                .ToList();
        }

        private List<SelectListItem> LayDanhSachPhuongThuc()
        {
            return PhuongThucThanhToan.TatCa
                .Select(p => new SelectListItem { Value = p, Text = p })
                .ToList();
        }
    }
}