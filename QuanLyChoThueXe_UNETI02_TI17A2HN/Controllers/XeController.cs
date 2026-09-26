// XeController.cs - Quan ly xe (CRUD + tim kiem + loc + phan trang)
// SV: Dao Gia Hung - 23103100065
// Module 2: Quan ly xe, bang gia thue

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Data;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Helpers;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;
using QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.Xe;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Controllers
{
    [SessionAuthorize(Roles = new[] { VaiTro.Admin, VaiTro.NhanVien })]
    public class XeController : Controller
    {
        private readonly AppDbContext _context;

        public XeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? tuKhoa,
            int? maLoaiXe,
            int? maHangXe,
            int? soCho,
            string? tinhTrang,
            int? namSanXuatTu,
            int? namSanXuatDen,
            decimal? donGiaTu,
            decimal? donGiaDen,
            string? sapXepTheo,
            string? thuTuSapXep,
            int trang = 1,
            int kichThuocTrang = 10)
        {
            var query = _context.Xes
                .Include(x => x.LoaiXe)
                .Include(x => x.HangXe)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var kw = tuKhoa.Trim();
                query = query.Where(x =>
                    x.BienSo.Contains(kw) ||
                    x.TenXe.Contains(kw) ||
                    (x.HangXe != null && x.HangXe.TenHangXe.Contains(kw)) ||
                    (x.LoaiXe != null && x.LoaiXe.TenLoaiXe.Contains(kw)));
            }

            if (maLoaiXe.HasValue)
                query = query.Where(x => x.MaLoaiXe == maLoaiXe.Value);

            if (maHangXe.HasValue)
                query = query.Where(x => x.MaHangXe == maHangXe.Value);

            if (soCho.HasValue)
                query = query.Where(x => x.SoCho == soCho.Value);

            if (!string.IsNullOrWhiteSpace(tinhTrang))
                query = query.Where(x => x.TinhTrang == tinhTrang);

            if (namSanXuatTu.HasValue)
                query = query.Where(x => x.NamSanXuat >= namSanXuatTu.Value);
            if (namSanXuatDen.HasValue)
                query = query.Where(x => x.NamSanXuat <= namSanXuatDen.Value);

            if (donGiaTu.HasValue || donGiaDen.HasValue)
            {
                var now = DateTime.Now;
                query = query.Where(x => _context.BangGiaThues.Any(b =>
                    b.TrangThai &&
                    b.TuNgay <= now && b.DenNgay >= now &&
                    (b.MaXe == x.MaXe || b.MaLoaiXe == x.MaLoaiXe) &&
                    (!donGiaTu.HasValue || b.DonGiaNgay >= donGiaTu.Value) &&
                    (!donGiaDen.HasValue || b.DonGiaNgay <= donGiaDen.Value)));
            }

            query = (sapXepTheo?.ToLower(), thuTuSapXep?.ToLower()) switch
            {
                ("tenxe", "desc") => query.OrderByDescending(x => x.TenXe),
                ("tenxe", _) => query.OrderBy(x => x.TenXe),
                ("namsanxuat", "desc") => query.OrderByDescending(x => x.NamSanXuat),
                ("namsanxuat", _) => query.OrderBy(x => x.NamSanXuat),
                ("socho", "desc") => query.OrderByDescending(x => x.SoCho),
                ("socho", _) => query.OrderBy(x => x.SoCho),
                (_, "desc") => query.OrderByDescending(x => x.MaXe),
                _ => query.OrderBy(x => x.MaXe)
            };

            var tongSo = await query.CountAsync();

            if (trang < 1) trang = 1;
            if (kichThuocTrang < 1) kichThuocTrang = 10;
            if (kichThuocTrang > 100) kichThuocTrang = 100;

            var danhSach = await query
                .Skip((trang - 1) * kichThuocTrang)
                .Take(kichThuocTrang)
                .ToListAsync();

            var vm = new XeListVM
            {
                DanhSach = danhSach,
                TuKhoa = tuKhoa,
                MaLoaiXe = maLoaiXe,
                MaHangXe = maHangXe,
                SoCho = soCho,
                TinhTrang = tinhTrang,
                NamSanXuatTu = namSanXuatTu,
                NamSanXuatDen = namSanXuatDen,
                DonGiaTu = donGiaTu,
                DonGiaDen = donGiaDen,
                SapXepTheo = sapXepTheo,
                ThuTuSapXep = thuTuSapXep,
                Trang = trang,
                KichThuocTrang = kichThuocTrang,
                TongSoBanGhi = tongSo,
                DanhSachLoaiXe = await LayDanhSachLoaiXe(),
                DanhSachHangXe = await LayDanhSachHangXe(),
                DanhSachTinhTrang = LayDanhSachTinhTrang()
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var xe = await _context.Xes
                .Include(x => x.LoaiXe)
                .Include(x => x.HangXe)
                .Include(x => x.BangGiaThues)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaXe == id);

            if (xe == null)
            {
                TempData["Error"] = "Khong tim thay xe";
                return RedirectToAction(nameof(Index));
            }

            return View(xe);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new XeFormVM
            {
                DanhSachLoaiXe = await LayDanhSachLoaiXe(),
                DanhSachHangXe = await LayDanhSachHangXe(),
                DanhSachTinhTrang = LayDanhSachTinhTrang()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(XeFormVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.DanhSachLoaiXe = await LayDanhSachLoaiXe();
                vm.DanhSachHangXe = await LayDanhSachHangXe();
                vm.DanhSachTinhTrang = LayDanhSachTinhTrang();
                return View(vm);
            }

            var trungBienSo = await _context.Xes
                .AnyAsync(x => x.BienSo == vm.BienSo.Trim());

            if (trungBienSo)
            {
                ModelState.AddModelError(nameof(vm.BienSo), "Bien so da ton tai");
                vm.DanhSachLoaiXe = await LayDanhSachLoaiXe();
                vm.DanhSachHangXe = await LayDanhSachHangXe();
                vm.DanhSachTinhTrang = LayDanhSachTinhTrang();
                return View(vm);
            }

            var loaiXe = await _context.LoaiXes.FindAsync(vm.MaLoaiXe);
            if (loaiXe == null || !loaiXe.TrangThai)
            {
                ModelState.AddModelError(nameof(vm.MaLoaiXe),
                    "Loai xe khong ton tai hoac da ngung hoat dong");
                vm.DanhSachLoaiXe = await LayDanhSachLoaiXe();
                vm.DanhSachHangXe = await LayDanhSachHangXe();
                vm.DanhSachTinhTrang = LayDanhSachTinhTrang();
                return View(vm);
            }

            var hangXe = await _context.HangXes.FindAsync(vm.MaHangXe);
            if (hangXe == null || !hangXe.TrangThai)
            {
                ModelState.AddModelError(nameof(vm.MaHangXe),
                    "Hang xe khong ton tai hoac da ngung hoat dong");
                vm.DanhSachLoaiXe = await LayDanhSachLoaiXe();
                vm.DanhSachHangXe = await LayDanhSachHangXe();
                vm.DanhSachTinhTrang = LayDanhSachTinhTrang();
                return View(vm);
            }

            var xe = new Xe
            {
                BienSo = vm.BienSo.Trim(),
                TenXe = vm.TenXe.Trim(),
                MaLoaiXe = vm.MaLoaiXe,
                MaHangXe = vm.MaHangXe,
                NamSanXuat = vm.NamSanXuat,
                MauSac = vm.MauSac?.Trim(),
                SoCho = vm.SoCho,
                SoKmHienTai = vm.SoKmHienTai,
                TinhTrang = vm.TinhTrang,
                MoTa = vm.MoTa?.Trim(),
                AnhXe = vm.AnhXe?.Trim()
            };

            _context.Xes.Add(xe);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Them xe '{xe.BienSo}' thanh cong";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var xe = await _context.Xes.FindAsync(id);
            if (xe == null)
            {
                TempData["Error"] = "Khong tim thay xe";
                return RedirectToAction(nameof(Index));
            }

            var vm = new XeFormVM
            {
                MaXe = xe.MaXe,
                BienSo = xe.BienSo,
                TenXe = xe.TenXe,
                MaLoaiXe = xe.MaLoaiXe,
                MaHangXe = xe.MaHangXe,
                NamSanXuat = xe.NamSanXuat,
                MauSac = xe.MauSac,
                SoCho = xe.SoCho,
                SoKmHienTai = xe.SoKmHienTai,
                TinhTrang = xe.TinhTrang,
                MoTa = xe.MoTa,
                AnhXe = xe.AnhXe,
                DanhSachLoaiXe = await LayDanhSachLoaiXe(),
                DanhSachHangXe = await LayDanhSachHangXe(),
                DanhSachTinhTrang = LayDanhSachTinhTrang()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, XeFormVM vm)
        {
            if (id != vm.MaXe)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                vm.DanhSachLoaiXe = await LayDanhSachLoaiXe();
                vm.DanhSachHangXe = await LayDanhSachHangXe();
                vm.DanhSachTinhTrang = LayDanhSachTinhTrang();
                return View(vm);
            }

            var xe = await _context.Xes.FindAsync(id);
            if (xe == null)
            {
                TempData["Error"] = "Khong tim thay xe";
                return RedirectToAction(nameof(Index));
            }

            var trungBienSo = await _context.Xes
                .AnyAsync(x => x.BienSo == vm.BienSo.Trim() && x.MaXe != id);

            if (trungBienSo)
            {
                ModelState.AddModelError(nameof(vm.BienSo), "Bien so da ton tai");
                vm.DanhSachLoaiXe = await LayDanhSachLoaiXe();
                vm.DanhSachHangXe = await LayDanhSachHangXe();
                vm.DanhSachTinhTrang = LayDanhSachTinhTrang();
                return View(vm);
            }

            if (vm.SoKmHienTai < xe.SoKmHienTai)
            {
                ModelState.AddModelError(nameof(vm.SoKmHienTai),
                    $"So km moi phai lon hon hoac bang {xe.SoKmHienTai:N0} km");
                vm.DanhSachLoaiXe = await LayDanhSachLoaiXe();
                vm.DanhSachHangXe = await LayDanhSachHangXe();
                vm.DanhSachTinhTrang = LayDanhSachTinhTrang();
                return View(vm);
            }

            xe.BienSo = vm.BienSo.Trim();
            xe.TenXe = vm.TenXe.Trim();
            xe.MaLoaiXe = vm.MaLoaiXe;
            xe.MaHangXe = vm.MaHangXe;
            xe.NamSanXuat = vm.NamSanXuat;
            xe.MauSac = vm.MauSac?.Trim();
            xe.SoCho = vm.SoCho;
            xe.SoKmHienTai = vm.SoKmHienTai;
            xe.TinhTrang = vm.TinhTrang;
            xe.MoTa = vm.MoTa?.Trim();
            xe.AnhXe = vm.AnhXe?.Trim();

            await _context.SaveChangesAsync();

            TempData["Success"] = "Cap nhat xe thanh cong";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiTinhTrang(int id, string tinhTrangMoi)
        {
            var xe = await _context.Xes.FindAsync(id);
            if (xe == null)
            {
                TempData["Error"] = "Khong tim thay xe";
                return RedirectToAction(nameof(Index));
            }

            if (!TinhTrangXe.TatCa.Contains(tinhTrangMoi))
            {
                TempData["Error"] = "Tinh trang khong hop le";
                return RedirectToAction(nameof(Index));
            }

            if (xe.TinhTrang == TinhTrangXe.DangChoThue)
            {
                TempData["Error"] = "Xe dang cho thue, khong the doi tinh trang";
                return RedirectToAction(nameof(Index));
            }

            xe.TinhTrang = tinhTrangMoi;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Da doi tinh trang xe thanh '{tinhTrangMoi}'";
            return RedirectToAction(nameof(Index));
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

        private List<SelectListItem> LayDanhSachTinhTrang()
        {
            return TinhTrangXe.TatCa
                .Select(t => new SelectListItem { Value = t, Text = t })
                .ToList();
        }
    }
}