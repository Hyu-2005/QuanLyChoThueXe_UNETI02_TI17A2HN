// ============================================================
// File: Controllers/MaGiamGiaController.cs
// Nội dung: CRUD mã giảm giá (chỉ Admin)
// Sinh viên thực hiện: Dương Lâm Huy - 23103100120 - SV5
// Module: Module 5 - Tính tiền, Thanh toán
// ============================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Data;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Helpers;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;
using QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.MaGiamGia;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Controllers
{
    [SessionAuthorize(Roles = new[] { VaiTro.Admin })]
    public class MaGiamGiaController : Controller
    {
        private readonly AppDbContext _context;

        public MaGiamGiaController(AppDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // DANH SÁCH
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> Index(
            string? tuKhoa,
            string? loaiGiamGia,
            string? trangThaiLoc,
            int trang = 1,
            int kichThuocTrang = 10)
        {
            var query = _context.MaGiamGias
                .AsNoTracking()
                .AsQueryable();

            // Tìm kiếm
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var kw = tuKhoa.Trim();
                query = query.Where(v =>
                    v.Code.Contains(kw) ||
                    v.TenChuongTrinh.Contains(kw) ||
                    (v.MoTa != null && v.MoTa.Contains(kw)));
            }

            // Lọc theo loại
            if (!string.IsNullOrWhiteSpace(loaiGiamGia))
                query = query.Where(v => v.LoaiGiamGia == loaiGiamGia);

            // Lọc theo trạng thái
            var now = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(trangThaiLoc))
            {
                query = trangThaiLoc switch
                {
                    "active" => query.Where(v => v.TrangThai
                        && v.NgayBatDau <= now
                        && v.NgayKetThuc >= now
                        && v.SoLuotDaDung < v.SoLuotDungToiDa),
                    "inactive" => query.Where(v => !v.TrangThai),
                    "expired" => query.Where(v => v.NgayKetThuc < now),
                    "sap_het" => query.Where(v => v.TrangThai
                        && v.NgayKetThuc >= now
                        && v.SoLuotDaDung >= v.SoLuotDungToiDa * 0.8m
                        && v.SoLuotDaDung < v.SoLuotDungToiDa),
                    _ => query
                };
            }

            var tongSo = await query.CountAsync();

            if (trang < 1) trang = 1;
            if (kichThuocTrang < 1) kichThuocTrang = 10;
            if (kichThuocTrang > 100) kichThuocTrang = 100;

            var danhSach = await query
                .OrderByDescending(v => v.TrangThai)
                .ThenByDescending(v => v.NgayBatDau)
                .Skip((trang - 1) * kichThuocTrang)
                .Take(kichThuocTrang)
                .ToListAsync();

            // Thống kê nhanh (toàn bộ, không filter)
            var tongDangHoatDong = await _context.MaGiamGias
                .CountAsync(v => v.TrangThai
                    && v.NgayBatDau <= now
                    && v.NgayKetThuc >= now
                    && v.SoLuotDaDung < v.SoLuotDungToiDa);

            var tongDaHetHan = await _context.MaGiamGias
                .CountAsync(v => v.NgayKetThuc < now);

            var tongSapHetLuot = await _context.MaGiamGias
                .CountAsync(v => v.TrangThai
                    && v.NgayKetThuc >= now
                    && v.SoLuotDaDung >= v.SoLuotDungToiDa * 0.8m
                    && v.SoLuotDaDung < v.SoLuotDungToiDa);

            var vm = new MaGiamGiaListVM
            {
                DanhSach = danhSach,
                TuKhoa = tuKhoa,
                LoaiGiamGia = loaiGiamGia,
                TrangThaiLoc = trangThaiLoc,
                Trang = trang,
                KichThuocTrang = kichThuocTrang,
                TongSoBanGhi = tongSo,
                TongDangHoatDong = tongDangHoatDong,
                TongDaHetHan = tongDaHetHan,
                TongSapHetLuot = tongSapHetLuot
            };

            return View(vm);
        }

        // =====================================================
        // CHI TIẾT
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var voucher = await _context.MaGiamGias
                .Include(v => v.SuDungMaGiamGias)
                    .ThenInclude(s => s.KhachHang)
                .Include(v => v.SuDungMaGiamGias)
                    .ThenInclude(s => s.DatXe)
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.MaGiamGiaId == id);

            if (voucher == null)
            {
                TempData["Error"] = "Không tìm thấy mã giảm giá";
                return RedirectToAction(nameof(Index));
            }

            return View(voucher);
        }

        // =====================================================
        // TẠO MỚI - GET
        // =====================================================
        [HttpGet]
        public IActionResult Create()
        {
            var vm = new MaGiamGiaFormVM
            {
                DanhSachLoaiGiamGia = LayDanhSachLoaiGiamGia()
            };
            return View(vm);
        }

        // =====================================================
        // TẠO MỚI - POST
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MaGiamGiaFormVM vm)
        {
            // Validate business rules
            ValidateBusinessRules(vm);

            if (!ModelState.IsValid)
            {
                vm.DanhSachLoaiGiamGia = LayDanhSachLoaiGiamGia();
                return View(vm);
            }

            // Kiểm tra trùng code
            var code = vm.Code.Trim().ToUpperInvariant();
            var daTonTai = await _context.MaGiamGias
                .AnyAsync(v => v.Code.ToUpper() == code);

            if (daTonTai)
            {
                ModelState.AddModelError(nameof(vm.Code), "Mã giảm giá đã tồn tại");
                vm.DanhSachLoaiGiamGia = LayDanhSachLoaiGiamGia();
                return View(vm);
            }

            var voucher = new MaGiamGia
            {
                Code = code,
                TenChuongTrinh = vm.TenChuongTrinh.Trim(),
                MoTa = vm.MoTa?.Trim(),
                LoaiGiamGia = vm.LoaiGiamGia,
                GiaTriGiam = vm.GiaTriGiam,
                GiamToiDa = vm.GiamToiDa,
                DonHangToiThieu = vm.DonHangToiThieu,
                NgayBatDau = vm.NgayBatDau,
                NgayKetThuc = vm.NgayKetThuc,
                SoLuotDungToiDa = vm.SoLuotDungToiDa,
                MoiKhachMotLan = vm.MoiKhachMotLan,
                TrangThai = vm.TrangThai,
                SoLuotDaDung = 0
            };

            _context.MaGiamGias.Add(voucher);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã tạo mã giảm giá '{voucher.Code}' thành công";
            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // SỬA - GET
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var voucher = await _context.MaGiamGias.FindAsync(id);
            if (voucher == null)
            {
                TempData["Error"] = "Không tìm thấy mã giảm giá";
                return RedirectToAction(nameof(Index));
            }

            var vm = new MaGiamGiaFormVM
            {
                MaGiamGiaId = voucher.MaGiamGiaId,
                Code = voucher.Code,
                TenChuongTrinh = voucher.TenChuongTrinh,
                MoTa = voucher.MoTa,
                LoaiGiamGia = voucher.LoaiGiamGia,
                GiaTriGiam = voucher.GiaTriGiam,
                GiamToiDa = voucher.GiamToiDa,
                DonHangToiThieu = voucher.DonHangToiThieu,
                NgayBatDau = voucher.NgayBatDau,
                NgayKetThuc = voucher.NgayKetThuc,
                SoLuotDungToiDa = voucher.SoLuotDungToiDa,
                MoiKhachMotLan = voucher.MoiKhachMotLan,
                TrangThai = voucher.TrangThai,
                DanhSachLoaiGiamGia = LayDanhSachLoaiGiamGia()
            };

            return View(vm);
        }

        // =====================================================
        // SỬA - POST
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MaGiamGiaFormVM vm)
        {
            if (id != vm.MaGiamGiaId)
                return BadRequest();

            ValidateBusinessRules(vm);

            if (!ModelState.IsValid)
            {
                vm.DanhSachLoaiGiamGia = LayDanhSachLoaiGiamGia();
                return View(vm);
            }

            var voucher = await _context.MaGiamGias.FindAsync(id);
            if (voucher == null)
            {
                TempData["Error"] = "Không tìm thấy mã giảm giá";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra trùng code (trừ chính nó)
            var code = vm.Code.Trim().ToUpperInvariant();
            var daTonTai = await _context.MaGiamGias
                .AnyAsync(v => v.Code.ToUpper() == code && v.MaGiamGiaId != id);

            if (daTonTai)
            {
                ModelState.AddModelError(nameof(vm.Code), "Mã giảm giá đã tồn tại");
                vm.DanhSachLoaiGiamGia = LayDanhSachLoaiGiamGia();
                return View(vm);
            }

            voucher.Code = code;
            voucher.TenChuongTrinh = vm.TenChuongTrinh.Trim();
            voucher.MoTa = vm.MoTa?.Trim();
            voucher.LoaiGiamGia = vm.LoaiGiamGia;
            voucher.GiaTriGiam = vm.GiaTriGiam;
            voucher.GiamToiDa = vm.GiamToiDa;
            voucher.DonHangToiThieu = vm.DonHangToiThieu;
            voucher.NgayBatDau = vm.NgayBatDau;
            voucher.NgayKetThuc = vm.NgayKetThuc;
            voucher.SoLuotDungToiDa = vm.SoLuotDungToiDa;
            voucher.MoiKhachMotLan = vm.MoiKhachMotLan;
            voucher.TrangThai = vm.TrangThai;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật mã giảm giá thành công";
            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // ĐỔI TRẠNG THÁI
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiTrangThai(int id)
        {
            var voucher = await _context.MaGiamGias.FindAsync(id);
            if (voucher == null)
            {
                TempData["Error"] = "Không tìm thấy mã giảm giá";
                return RedirectToAction(nameof(Index));
            }

            voucher.TrangThai = !voucher.TrangThai;
            await _context.SaveChangesAsync();

            TempData["Success"] = voucher.TrangThai
                ? "Đã kích hoạt mã giảm giá"
                : "Đã vô hiệu hóa mã giảm giá";

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // XÓA (chỉ xóa được khi chưa có ai dùng)
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Xoa(int id)
        {
            var voucher = await _context.MaGiamGias
                .Include(v => v.SuDungMaGiamGias)
                .FirstOrDefaultAsync(v => v.MaGiamGiaId == id);

            if (voucher == null)
            {
                TempData["Error"] = "Không tìm thấy mã giảm giá";
                return RedirectToAction(nameof(Index));
            }

            if (voucher.SoLuotDaDung > 0 || voucher.SuDungMaGiamGias.Any())
            {
                TempData["Error"] = "Không thể xóa mã đã có người sử dụng. Hãy vô hiệu hóa thay vì xóa.";
                return RedirectToAction(nameof(Index));
            }

            _context.MaGiamGias.Remove(voucher);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã xóa mã giảm giá '{voucher.Code}'";
            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // HELPER
        // =====================================================
        private void ValidateBusinessRules(MaGiamGiaFormVM vm)
        {
            if (vm.NgayKetThuc <= vm.NgayBatDau)
                ModelState.AddModelError(nameof(vm.NgayKetThuc),
                    "Ngày kết thúc phải sau ngày bắt đầu");

            if (vm.LoaiGiamGia == LoaiGiamGia.PhanTram)
            {
                if (vm.GiaTriGiam > 100)
                    ModelState.AddModelError(nameof(vm.GiaTriGiam),
                        "Phần trăm giảm không được vượt quá 100%");

                if (vm.GiamToiDa.HasValue && vm.GiamToiDa.Value < 0)
                    ModelState.AddModelError(nameof(vm.GiamToiDa),
                        "Giảm tối đa phải >= 0");
            }
            else
            {
                // Số tiền cố định
                if (vm.GiamToiDa.HasValue)
                    ModelState.AddModelError(nameof(vm.GiamToiDa),
                        "Với loại giảm cố định, không cần nhập 'Giảm tối đa'");
            }
        }

        private List<SelectListItem> LayDanhSachLoaiGiamGia()
        {
            return LoaiGiamGia.TatCa
                .Select(l => new SelectListItem
                {
                    Value = l,
                    Text = LoaiGiamGia.GetDisplayName(l)
                })
                .ToList();
        }
    }
}