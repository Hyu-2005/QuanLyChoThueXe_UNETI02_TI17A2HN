// file: Controllers/BangGiaThueController.cs
// noi dung: CRUD Bang gia thue xe
// sinh vien thuc hien: Dao Gia Hung - 23103100065
// module: Module 2 - Quan ly xe, Bang gia thue

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Data;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Helpers;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Controllers
{
    [SessionAuthorize(Roles = new[] { VaiTro.Admin, VaiTro.NhanVien })]
    public class BangGiaThueController : Controller
    {
        private readonly AppDbContext _context;

        public BangGiaThueController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? tuKhoa,
            int? maLoaiXe,
            int? maXe,
            bool? trangThai,
            DateTime? tuNgayLoc,
            DateTime? denNgayLoc,
            string? sapXepTheo,
            string? thuTuSapXep,
            int trang = 1,
            int kichThuocTrang = 10)
        {
            var query = _context.BangGiaThues
                .Include(b => b.LoaiXe)
                .Include(b => b.Xe)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var kw = tuKhoa.Trim();
                query = query.Where(b =>
                    (b.LoaiXe != null && b.LoaiXe.TenLoaiXe.Contains(kw)) ||
                    (b.Xe != null && b.Xe.BienSo.Contains(kw)) ||
                    (b.Xe != null && b.Xe.TenXe.Contains(kw)));
            }

            if (maLoaiXe.HasValue)
                query = query.Where(b => b.MaLoaiXe == maLoaiXe.Value);
            if (maXe.HasValue)
                query = query.Where(b => b.MaXe == maXe.Value);
            if (trangThai.HasValue)
                query = query.Where(b => b.TrangThai == trangThai.Value);
            if (tuNgayLoc.HasValue)
                query = query.Where(b => b.TuNgay >= tuNgayLoc.Value);
            if (denNgayLoc.HasValue)
                query = query.Where(b => b.DenNgay <= denNgayLoc.Value);

            query = (sapXepTheo?.ToLower(), thuTuSapXep?.ToLower()) switch
            {
                ("dongia", "desc") => query.OrderByDescending(b => b.DonGiaNgay),
                ("dongia", _) => query.OrderBy(b => b.DonGiaNgay),
                ("tungay", "desc") => query.OrderByDescending(b => b.TuNgay),
                ("tungay", _) => query.OrderBy(b => b.TuNgay),
                ("denngay", "desc") => query.OrderByDescending(b => b.DenNgay),
                ("denngay", _) => query.OrderBy(b => b.DenNgay),
                (_, "desc") => query.OrderByDescending(b => b.MaBangGia),
                _ => query.OrderBy(b => b.MaBangGia)
            };

            var tongSo = await query.CountAsync();

            if (trang < 1) trang = 1;
            if (kichThuocTrang < 1) kichThuocTrang = 10;
            if (kichThuocTrang > 100) kichThuocTrang = 100;

            var danhSach = await query
                .Skip((trang - 1) * kichThuocTrang)
                .Take(kichThuocTrang)
                .ToListAsync();

            var vm = new ViewModels.BangGiaThue.BangGiaThueListVM
            {
                DanhSach = danhSach,
                TuKhoa = tuKhoa,
                MaLoaiXe = maLoaiXe,
                MaXe = maXe,
                TrangThai = trangThai,
                TuNgayLoc = tuNgayLoc,
                DenNgayLoc = denNgayLoc,
                SapXepTheo = sapXepTheo,
                ThuTuSapXep = thuTuSapXep,
                Trang = trang,
                KichThuocTrang = kichThuocTrang,
                TongSoBanGhi = tongSo,
                DanhSachLoaiXe = await LayDanhSachLoaiXe(),
                DanhSachXe = await LayDanhSachXe()
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var bangGia = await _context.BangGiaThues
                .Include(b => b.LoaiXe)
                .Include(b => b.Xe)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.MaBangGia == id);

            if (bangGia == null)
            {
                TempData["Error"] = "Khong tim thay bang gia";
                return RedirectToAction(nameof(Index));
            }

            return View(bangGia);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new ViewModels.BangGiaThue.BangGiaThueFormVM
            {
                DanhSachLoaiXe = await LayDanhSachLoaiXe(),
                DanhSachXe = await LayDanhSachXe()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ViewModels.BangGiaThue.BangGiaThueFormVM vm)
        {
            if (!vm.MaLoaiXe.HasValue && !vm.MaXe.HasValue)
            {
                ModelState.AddModelError(string.Empty,
                    "Phai chon 1 trong 2: Loai xe HOAC Xe cu the");
            }
            if (vm.MaLoaiXe.HasValue && vm.MaXe.HasValue)
            {
                ModelState.AddModelError(string.Empty,
                    "Chi duoc chon 1 trong 2: Loai xe HOAC Xe cu the");
            }

            if (vm.DenNgay <= vm.TuNgay)
            {
                ModelState.AddModelError(nameof(vm.DenNgay),
                    "Ngay ket thuc phai sau ngay bat dau");
            }

            if (vm.MaLoaiXe.HasValue || vm.MaXe.HasValue)
            {
                var trung = await _context.BangGiaThues.AnyAsync(b =>
                    b.TrangThai == vm.TrangThai &&
                    b.MaLoaiXe == vm.MaLoaiXe &&
                    b.MaXe == vm.MaXe &&
                    b.TuNgay <= vm.DenNgay &&
                    b.DenNgay >= vm.TuNgay);

                if (trung)
                {
                    ModelState.AddModelError(string.Empty,
                        "Da co bang gia khac trung khoang hieu luc cho doi tuong nay");
                }
            }

            if (!ModelState.IsValid)
            {
                vm.DanhSachLoaiXe = await LayDanhSachLoaiXe();
                vm.DanhSachXe = await LayDanhSachXe();
                return View(vm);
            }

            var bangGia = new Models.Entities.BangGiaThue
            {
                MaLoaiXe = vm.MaLoaiXe,
                MaXe = vm.MaXe,
                DonGiaNgay = vm.DonGiaNgay,
                DonGiaGio = vm.DonGiaGio,
                TienCocMacDinh = vm.TienCocMacDinh,
                TuNgay = vm.TuNgay,
                DenNgay = vm.DenNgay,
                TrangThai = vm.TrangThai
            };

            _context.BangGiaThues.Add(bangGia);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Them bang gia thanh cong";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var bangGia = await _context.BangGiaThues.FindAsync(id);
            if (bangGia == null)
            {
                TempData["Error"] = "Khong tim thay bang gia";
                return RedirectToAction(nameof(Index));
            }

            var vm = new ViewModels.BangGiaThue.BangGiaThueFormVM
            {
                MaBangGia = bangGia.MaBangGia,
                MaLoaiXe = bangGia.MaLoaiXe,
                MaXe = bangGia.MaXe,
                DonGiaNgay = bangGia.DonGiaNgay,
                DonGiaGio = bangGia.DonGiaGio,
                TienCocMacDinh = bangGia.TienCocMacDinh,
                TuNgay = bangGia.TuNgay,
                DenNgay = bangGia.DenNgay,
                TrangThai = bangGia.TrangThai,
                DanhSachLoaiXe = await LayDanhSachLoaiXe(),
                DanhSachXe = await LayDanhSachXe()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ViewModels.BangGiaThue.BangGiaThueFormVM vm)
        {
            if (id != vm.MaBangGia)
                return BadRequest();

            if (!vm.MaLoaiXe.HasValue && !vm.MaXe.HasValue)
                ModelState.AddModelError(string.Empty,
                    "Phai chon 1 trong 2: Loai xe HOAC Xe cu the");
            if (vm.MaLoaiXe.HasValue && vm.MaXe.HasValue)
                ModelState.AddModelError(string.Empty,
                    "Chi duoc chon 1 trong 2: Loai xe HOAC Xe cu the");

            if (vm.DenNgay <= vm.TuNgay)
                ModelState.AddModelError(nameof(vm.DenNgay),
                    "Ngay ket thuc phai sau ngay bat dau");

            if (vm.MaLoaiXe.HasValue || vm.MaXe.HasValue)
            {
                var trung = await _context.BangGiaThues.AnyAsync(b =>
                    b.MaBangGia != id &&
                    b.TrangThai == vm.TrangThai &&
                    b.MaLoaiXe == vm.MaLoaiXe &&
                    b.MaXe == vm.MaXe &&
                    b.TuNgay <= vm.DenNgay &&
                    b.DenNgay >= vm.TuNgay);

                if (trung)
                {
                    ModelState.AddModelError(string.Empty,
                        "Da co bang gia khac trung khoang hieu luc cho doi tuong nay");
                }
            }

            if (!ModelState.IsValid)
            {
                vm.DanhSachLoaiXe = await LayDanhSachLoaiXe();
                vm.DanhSachXe = await LayDanhSachXe();
                return View(vm);
            }

            var bangGia = await _context.BangGiaThues.FindAsync(id);
            if (bangGia == null)
            {
                TempData["Error"] = "Khong tim thay bang gia";
                return RedirectToAction(nameof(Index));
            }

            bangGia.MaLoaiXe = vm.MaLoaiXe;
            bangGia.MaXe = vm.MaXe;
            bangGia.DonGiaNgay = vm.DonGiaNgay;
            bangGia.DonGiaGio = vm.DonGiaGio;
            bangGia.TienCocMacDinh = vm.TienCocMacDinh;
            bangGia.TuNgay = vm.TuNgay;
            bangGia.DenNgay = vm.DenNgay;
            bangGia.TrangThai = vm.TrangThai;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Cap nhat bang gia thanh cong";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiTrangThai(int id)
        {
            var bangGia = await _context.BangGiaThues.FindAsync(id);
            if (bangGia == null)
            {
                TempData["Error"] = "Khong tim thay bang gia";
                return RedirectToAction(nameof(Index));
            }

            bangGia.TrangThai = !bangGia.TrangThai;
            await _context.SaveChangesAsync();

            TempData["Success"] = bangGia.TrangThai
                ? "Da kich hoat bang gia"
                : "Da ngung bang gia";

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

        private async Task<List<SelectListItem>> LayDanhSachXe()
        {
            return await _context.Xes
                .OrderBy(x => x.BienSo)
                .Select(x => new SelectListItem
                {
                    Value = x.MaXe.ToString(),
                    Text = x.BienSo + " - " + x.TenXe
                })
                .ToListAsync();
        }
    }
}