// ============================================================
// File: Controllers/DatXeController.cs
// Noi dung: Tim xe trong, dat xe, kiem tra trung lich, huy don
// Sinh vien thuc hien: Vu Tien Dat - 23103100119 - SV3
// Module: Module 3 - Khach hang, Tim xe trong va Dat xe
// ============================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Data;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Helpers;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Services.Interfaces;
using QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.DatXe;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Controllers
{
    [SessionAuthorize]
    public class DatXeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IXeTrongService _xeTrongService;
        private readonly ITrungLichService _trungLichService;

        public DatXeController(
            AppDbContext context,
            IXeTrongService xeTrongService,
            ITrungLichService trungLichService)
        {
            _context = context;
            _xeTrongService = xeTrongService;
            _trungLichService = trungLichService;
        }

        // =====================================================
        // TIM XE TRONG
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> TimXeTrong()
        {
            var vm = new TimXeTrongVM
            {
                DanhSachLoaiXe = await LayDanhSachLoaiXe(),
                DanhSachHangXe = await LayDanhSachHangXe()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TimXeTrong(TimXeTrongVM vm)
        {
            if (vm.ThoiGianTra <= vm.ThoiGianNhan)
            {
                ModelState.AddModelError(nameof(vm.ThoiGianTra),
                    "Thoi gian tra phai sau thoi gian nhan");
            }

            if (vm.ThoiGianNhan < DateTime.Now.AddHours(-1))
            {
                ModelState.AddModelError(nameof(vm.ThoiGianNhan),
                    "Thoi gian nhan khong duoc nam trong qua khu");
            }

            if (!ModelState.IsValid)
            {
                vm.DanhSachLoaiXe = await LayDanhSachLoaiXe();
                vm.DanhSachHangXe = await LayDanhSachHangXe();
                return View(vm);
            }

            vm.DanhSachXeTrong = await _xeTrongService.TimXeTrongAsync(
                vm.ThoiGianNhan, vm.ThoiGianTra,
                vm.MaLoaiXe, vm.MaHangXe, vm.SoCho,
                vm.DonGiaTu, vm.DonGiaDen);

            vm.DaTimKiem = true;
            vm.DanhSachLoaiXe = await LayDanhSachLoaiXe();
            vm.DanhSachHangXe = await LayDanhSachHangXe();

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> DatXe(int maXe, DateTime? thoiGianNhan = null, DateTime? thoiGianTra = null)
        {
            // 1. Kiem tra xe ton tai
            var xe = await _context.Xes
                .Include(x => x.LoaiXe)
                .Include(x => x.HangXe)
                .FirstOrDefaultAsync(x => x.MaXe == maXe);

            if (xe == null)
            {
                TempData["Error"] = "Khong tim thay xe";
                return RedirectToAction(nameof(TimXeTrong));
            }

            // 2. Kiem tra tinh trang xe co the cho thue
            if (!TinhTrangXe.CoTheChoThue.Contains(xe.TinhTrang))
            {
                TempData["Error"] = $"Xe dang o trang thai '{xe.TinhTrang}', khong the dat";
                return RedirectToAction(nameof(TimXeTrong));
            }

            // 3. Thoi gian: neu chua truyen thi mac dinh ngay mai -> 3 ngay sau
            var nhan = thoiGianNhan ?? DateTime.Now.Date.AddDays(1).AddHours(9);
            var tra = thoiGianTra ?? DateTime.Now.Date.AddDays(3).AddHours(9);

            if (tra <= nhan)
            {
                ModelState.AddModelError("", "Thoi gian tra phai sau thoi gian nhan");
                nhan = DateTime.Now.Date.AddDays(1).AddHours(9);
                tra = DateTime.Now.Date.AddDays(3).AddHours(9);
            }

            // 4. Kiem tra trung lich (chi khi co thoi gian cu the)
            var biTrung = await _trungLichService.KiemTraTrungLich(maXe, nhan, tra);
            if (biTrung)
            {
                TempData["Error"] = "Xe da co lich dat trong khoang thoi gian nay. Vui long chon khoang khac.";
                // Van hien form de user chon lai thoi gian
            }

            // 5. Lay bang gia hieu luc
            var bangGia = await _context.BangGiaThues
                .Where(b => b.TrangThai)
                .Where(b => b.TuNgay <= nhan && b.DenNgay >= tra)
                .Where(b => b.MaXe == maXe || b.MaLoaiXe == xe.MaLoaiXe)
                .OrderByDescending(b => b.MaXe.HasValue)
                .FirstOrDefaultAsync();

            // 6. Tinh toan so ngay + tien thue du kien
            var soNgay = Math.Max(1, (int)Math.Ceiling((tra - nhan).TotalDays));
            var donGiaNgay = bangGia?.DonGiaNgay ?? 0;
            var donGiaGio = bangGia?.DonGiaGio ?? 0;
            var tienCoc = bangGia?.TienCocMacDinh ?? 0;

            var vm = new DatXeCreateVM
            {
                MaXe = xe.MaXe,
                ThoiGianNhanDuKien = nhan,
                ThoiGianTraDuKien = tra,
                BienSo = xe.BienSo,
                TenXe = xe.TenXe,
                TenLoaiXe = xe.LoaiXe?.TenLoaiXe,
                TenHangXe = xe.HangXe?.TenHangXe,
                SoCho = xe.SoCho,
                AnhXe = xe.AnhXe,
                DonGiaNgay = donGiaNgay,
                DonGiaGio = donGiaGio,
                TienCoc = tienCoc,
                SoNgayThue = soNgay,
                TienThueDuKien = soNgay * donGiaNgay
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DatXe(DatXeCreateVM vm)
        {
            if (vm.ThoiGianTraDuKien <= vm.ThoiGianNhanDuKien)
                ModelState.AddModelError(nameof(vm.ThoiGianTraDuKien),
                    "Thoi gian tra phai sau thoi gian nhan");

            if (vm.ThoiGianNhanDuKien < DateTime.Now.AddHours(-1))
                ModelState.AddModelError(nameof(vm.ThoiGianNhanDuKien),
                    "Thoi gian nhan khong duoc nam trong qua khu");

            if (!ModelState.IsValid)
            {
                await LoadThongTinXeAsync(vm);
                return View(vm);
            }

            var maKhachHang = HttpContext.Session.GetMaKhachHang();
            if (!maKhachHang.HasValue)
            {
                TempData["Error"] = "Khong tim thay thong tin khach hang, vui long dang nhap lai";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var khachHang = await _context.KhachHangs.FindAsync(maKhachHang.Value);
            if (khachHang == null || !khachHang.TrangThai)
            {
                TempData["Error"] = "Tai khoan khach hang da bi khoa hoac khong ton tai";
                return RedirectToAction(nameof(TimXeTrong));
            }

            var xe = await _context.Xes
                .Include(x => x.LoaiXe)
                .FirstOrDefaultAsync(x => x.MaXe == vm.MaXe);

            if (xe == null)
            {
                TempData["Error"] = "Khong tim thay xe";
                return RedirectToAction(nameof(TimXeTrong));
            }

            if (!TinhTrangXe.CoTheChoThue.Contains(xe.TinhTrang))
            {
                TempData["Error"] = $"Xe dang o trang thai '{xe.TinhTrang}', khong the dat";
                return RedirectToAction(nameof(TimXeTrong));
            }

            var biTrung = await _trungLichService.KiemTraTrungLich(
                vm.MaXe, vm.ThoiGianNhanDuKien, vm.ThoiGianTraDuKien);
            if (biTrung)
            {
                ModelState.AddModelError(string.Empty,
                    "Xe vua co nguoi khac dat trong khoang thoi gian nay. Vui long chon xe khac.");
                await LoadThongTinXeAsync(vm);
                return View(vm);
            }

            var bangGia = await _context.BangGiaThues
                .Where(b => b.TrangThai)
                .Where(b => b.TuNgay <= vm.ThoiGianNhanDuKien && b.DenNgay >= vm.ThoiGianTraDuKien)
                .Where(b => b.MaXe == vm.MaXe || b.MaLoaiXe == xe.MaLoaiXe)
                .OrderByDescending(b => b.MaXe.HasValue)
                .FirstOrDefaultAsync();

            if (bangGia == null)
            {
                ModelState.AddModelError(string.Empty, "Xe khong co bang gia hieu luc");
                await LoadThongTinXeAsync(vm);
                return View(vm);
            }

            var datXe = new Models.Entities.DatXe
            {
                MaKhachHang = maKhachHang.Value,
                MaXe = vm.MaXe,
                ThoiGianNhanDuKien = vm.ThoiGianNhanDuKien,
                ThoiGianTraDuKien = vm.ThoiGianTraDuKien,
                DiaDiemNhan = vm.DiaDiemNhan.Trim(),
                DiaDiemTra = vm.DiaDiemTra.Trim(),
                NgayDat = DateTime.Now,
                DonGiaApDung = bangGia.DonGiaNgay,
                TienCoc = bangGia.TienCocMacDinh,
                TrangThai = TrangThaiDatXe.ChoDuyet,
                GhiChu = vm.GhiChu?.Trim()
            };

            _context.DatXes.Add(datXe);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Dat xe '{xe.BienSo}' thanh cong. Don dang cho duyet.";
            return RedirectToAction(nameof(DonCuaToi));
        }

        // =====================================================
        // DANH SACH DON CUA TOI (khach hang xem)
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> DonCuaToi(
            string? tuKhoa,
            string? trangThai,
            DateTime? tuNgay,
            DateTime? denNgay,
            int? maLoaiXe,
            string? sapXepTheo,
            string? thuTuSapXep,
            int trang = 1,
            int kichThuocTrang = 10)
        {
            var maKhachHang = HttpContext.Session.GetMaKhachHang();
            if (!maKhachHang.HasValue)
            {
                TempData["Error"] = "Vui long dang nhap bang tai khoan khach hang";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var query = _context.DatXes
                .Include(d => d.KhachHang)
                .Include(d => d.Xe).ThenInclude(x => x.LoaiXe)
                .Include(d => d.Xe).ThenInclude(x => x.HangXe)
                .Where(d => d.MaKhachHang == maKhachHang.Value)
                .AsNoTracking()
                .AsQueryable();

            query = await ApplyFilterAsync(query, tuKhoa, trangThai, tuNgay, denNgay, maLoaiXe);
            query = ApplySort(query, sapXepTheo, thuTuSapXep);

            var tongSo = await query.CountAsync();
            if (trang < 1) trang = 1;

            var danhSach = await query
                .Skip((trang - 1) * kichThuocTrang)
                .Take(kichThuocTrang)
                .ToListAsync();

            var vm = new DatXeListVM
            {
                DanhSach = danhSach,
                LaKhachHang = true,
                TuKhoa = tuKhoa,
                TrangThai = trangThai,
                TuNgay = tuNgay,
                DenNgay = denNgay,
                MaLoaiXe = maLoaiXe,
                SapXepTheo = sapXepTheo,
                ThuTuSapXep = thuTuSapXep,
                Trang = trang,
                KichThuocTrang = kichThuocTrang,
                TongSoBanGhi = tongSo,
                DanhSachTrangThai = LayDanhSachTrangThai(),
                DanhSachLoaiXe = await LayDanhSachLoaiXe()
            };

            return View(vm);
        }

        // =====================================================
        // DANH SACH TAT CA DON (admin/nhan vien xem)
        // =====================================================

        [HttpGet]
        [SessionAuthorize(Roles = new[] { VaiTro.Admin, VaiTro.NhanVien })]
        public async Task<IActionResult> Index(
            string? tuKhoa,
            string? trangThai,
            DateTime? tuNgay,
            DateTime? denNgay,
            int? maLoaiXe,
            string? sapXepTheo,
            string? thuTuSapXep,
            int trang = 1,
            int kichThuocTrang = 10)
        {
            var query = _context.DatXes
                .Include(d => d.KhachHang)
                .Include(d => d.Xe).ThenInclude(x => x.LoaiXe)
                .Include(d => d.Xe).ThenInclude(x => x.HangXe)
                .AsNoTracking()
                .AsQueryable();

            query = await ApplyFilterAsync(query, tuKhoa, trangThai, tuNgay, denNgay, maLoaiXe);
            query = ApplySort(query, sapXepTheo, thuTuSapXep);

            var tongSo = await query.CountAsync();
            if (trang < 1) trang = 1;

            var danhSach = await query
                .Skip((trang - 1) * kichThuocTrang)
                .Take(kichThuocTrang)
                .ToListAsync();

            var vm = new DatXeListVM
            {
                DanhSach = danhSach,
                LaKhachHang = false,
                TuKhoa = tuKhoa,
                TrangThai = trangThai,
                TuNgay = tuNgay,
                DenNgay = denNgay,
                MaLoaiXe = maLoaiXe,
                SapXepTheo = sapXepTheo,
                ThuTuSapXep = thuTuSapXep,
                Trang = trang,
                KichThuocTrang = kichThuocTrang,
                TongSoBanGhi = tongSo,
                DanhSachTrangThai = LayDanhSachTrangThai(),
                DanhSachLoaiXe = await LayDanhSachLoaiXe()
            };

            return View(vm);
        }

        // =====================================================
        // CHI TIET DON
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var datXe = await _context.DatXes
                .Include(d => d.KhachHang)
                .Include(d => d.Xe).ThenInclude(x => x.LoaiXe)
                .Include(d => d.Xe).ThenInclude(x => x.HangXe)
                .Include(d => d.BanGiaoXe)
                .Include(d => d.TraXe)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.MaDatXe == id);

            if (datXe == null)
            {
                TempData["Error"] = "Khong tim thay don dat xe";
                return RedirectToAction(nameof(DonCuaToi));
            }

            var vaiTro = HttpContext.Session.GetVaiTro();
            if (vaiTro == VaiTro.KhachHang)
            {
                var maKhachHang = HttpContext.Session.GetMaKhachHang();
                if (datXe.MaKhachHang != maKhachHang)
                {
                    TempData["Error"] = "Ban khong co quyen xem don nay";
                    return RedirectToAction(nameof(DonCuaToi));
                }
            }

            return View(datXe);
        }

        // =====================================================
        // HUY DON
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Huy(int id)
        {
            var datXe = await _context.DatXes.FindAsync(id);
            if (datXe == null)
            {
                TempData["Error"] = "Khong tim thay don dat xe";
                return RedirectToAction(nameof(DonCuaToi));
            }

            var vaiTro = HttpContext.Session.GetVaiTro();
            var laKhachHang = vaiTro == VaiTro.KhachHang;

            if (laKhachHang)
            {
                var maKhachHang = HttpContext.Session.GetMaKhachHang();
                if (datXe.MaKhachHang != maKhachHang)
                {
                    TempData["Error"] = "Ban khong co quyen huy don nay";
                    return RedirectToAction(nameof(DonCuaToi));
                }
            }

            if (!TrangThaiDatXe.CoTheHuy.Contains(datXe.TrangThai))
            {
                TempData["Error"] = $"Don dang o trang thai '{datXe.TrangThai}', khong the huy";
                return RedirectToAction(nameof(Details), new { id });
            }

            var soGioConLai = (datXe.ThoiGianNhanDuKien - DateTime.Now).TotalHours;
            if (soGioConLai < QuyDinhThue.SoGioToiThieuTruocKhiHuy)
            {
                TempData["Error"] =
                    $"Chi duoc huy truoc gio nhan it nhat {QuyDinhThue.SoGioToiThieuTruocKhiHuy} gio. " +
                    $"Don nay chi con {soGioConLai:N1} gio.";
                return RedirectToAction(nameof(Details), new { id });
            }

            datXe.TrangThai = TrangThaiDatXe.DaHuy;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Da huy don #{datXe.MaDatXe} thanh cong";

            if (laKhachHang)
                return RedirectToAction(nameof(DonCuaToi));
            else
                return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // HELPER
        // =====================================================

        private async Task LoadThongTinXeAsync(DatXeCreateVM vm)
        {
            var xe = await _context.Xes
                .Include(x => x.LoaiXe)
                .Include(x => x.HangXe)
                .FirstOrDefaultAsync(x => x.MaXe == vm.MaXe);

            if (xe == null) return;

            vm.BienSo = xe.BienSo;
            vm.TenXe = xe.TenXe;
            vm.TenLoaiXe = xe.LoaiXe?.TenLoaiXe;
            vm.TenHangXe = xe.HangXe?.TenHangXe;
            vm.SoCho = xe.SoCho;
            vm.AnhXe = xe.AnhXe;

            var bangGia = await _context.BangGiaThues
                .Where(b => b.TrangThai)
                .Where(b => b.TuNgay <= vm.ThoiGianNhanDuKien && b.DenNgay >= vm.ThoiGianTraDuKien)
                .Where(b => b.MaXe == xe.MaXe || b.MaLoaiXe == xe.MaLoaiXe)
                .OrderByDescending(b => b.MaXe.HasValue)
                .FirstOrDefaultAsync();

            if (bangGia != null)
            {
                vm.DonGiaNgay = bangGia.DonGiaNgay;
                vm.DonGiaGio = bangGia.DonGiaGio;
                vm.TienCoc = bangGia.TienCocMacDinh;
                vm.SoNgayThue = Math.Max(1,
                    (int)Math.Ceiling((vm.ThoiGianTraDuKien - vm.ThoiGianNhanDuKien).TotalDays));
                vm.TienThueDuKien = vm.SoNgayThue * bangGia.DonGiaNgay;
            }
        }

        private async Task<IQueryable<Models.Entities.DatXe>> ApplyFilterAsync(
            IQueryable<Models.Entities.DatXe> query,
            string? tuKhoa,
            string? trangThai,
            DateTime? tuNgay,
            DateTime? denNgay,
            int? maLoaiXe)
        {
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var kw = tuKhoa.Trim();
                query = query.Where(d =>
                    (d.KhachHang != null && d.KhachHang.HoTen.Contains(kw)) ||
                    (d.KhachHang != null && d.KhachHang.SoDienThoai.Contains(kw)) ||
                    (d.Xe != null && d.Xe.BienSo.Contains(kw)) ||
                    (d.Xe != null && d.Xe.TenXe.Contains(kw)));
            }

            if (!string.IsNullOrWhiteSpace(trangThai))
                query = query.Where(d => d.TrangThai == trangThai);

            if (tuNgay.HasValue)
                query = query.Where(d => d.ThoiGianNhanDuKien >= tuNgay.Value);
            if (denNgay.HasValue)
                query = query.Where(d => d.ThoiGianNhanDuKien <= denNgay.Value);

            if (maLoaiXe.HasValue)
                query = query.Where(d => d.Xe != null && d.Xe.MaLoaiXe == maLoaiXe.Value);

            return await Task.FromResult(query);
        }

        private static IQueryable<Models.Entities.DatXe> ApplySort(
            IQueryable<Models.Entities.DatXe> query,
            string? sapXepTheo,
            string? thuTuSapXep)
        {
            return (sapXepTheo?.ToLower(), thuTuSapXep?.ToLower()) switch
            {
                ("thoigiannhan", "asc") => query.OrderBy(d => d.ThoiGianNhanDuKien),
                ("thoigiannhan", _) => query.OrderByDescending(d => d.ThoiGianNhanDuKien),
                ("ngaydat", "asc") => query.OrderBy(d => d.NgayDat),
                ("ngaydat", _) => query.OrderByDescending(d => d.NgayDat),
                (_, "asc") => query.OrderBy(d => d.NgayDat),
                _ => query.OrderByDescending(d => d.NgayDat)
            };
        }

        private List<SelectListItem> LayDanhSachTrangThai()
        {
            return TrangThaiDatXe.TatCa
                .Select(t => new SelectListItem { Value = t, Text = t })
                .ToList();
        }

        private async Task<List<SelectListItem>> LayDanhSachLoaiXe()
        {
            return await _context.LoaiXes
                .Where(l => l.TrangThai)
                .OrderBy(l => l.TenLoaiXe)
                .Select(l => new SelectListItem
                {
                    Value = l.MaLoaiXe.ToString(),
                    Text = l.TenLoaiXe
                })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> LayDanhSachHangXe()
        {
            return await _context.HangXes
                .Where(h => h.TrangThai)
                .OrderBy(h => h.TenHangXe)
                .Select(h => new SelectListItem
                {
                    Value = h.MaHangXe.ToString(),
                    Text = h.TenHangXe
                })
                .ToListAsync();
        }
    }
}