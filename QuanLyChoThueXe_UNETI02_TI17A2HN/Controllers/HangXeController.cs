// File: ViewModels/LoaiXe/LoaiXeFormVM.cs
// Noi dung: ViewModel cho form Them/Sua LoaiXe
// Sinh vien thuc hien: Pham Gia Minh Hoang - 23103100087 - SV1
// Module: Module 1 - Tai khoan, Dang nhap, Phan quyen, Loai xe, Hang xe

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Data;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Helpers;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;
using QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.HangXe;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Controllers
{
    [SessionAuthorize(Roles = new[] { VaiTro.Admin, VaiTro.NhanVien })]
    public class HangXeController : Controller
    {
        private readonly AppDbContext _context;

        public HangXeController(AppDbContext context)
        {
            _context = context;
        }

        // DANH SACH - co Tim kiem + Loc + Sap xep + Phan trang
        [HttpGet]
        public async Task<IActionResult> Index(
            string? tuKhoa,
            string? quocGia,
            bool? trangThai,
            string? sapXepTheo,
            string? thuTuSapXep,
            int trang = 1,
            int kichThuocTrang = 10)
        {
            var query = _context.HangXes.AsNoTracking().AsQueryable();

            // Tim kiem
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var kw = tuKhoa.Trim();
                query = query.Where(h => h.TenHangXe.Contains(kw));
            }

            // Loc theo quoc gia
            if (!string.IsNullOrWhiteSpace(quocGia))
                query = query.Where(h => h.QuocGia == quocGia);

            // Loc theo trang thai
            if (trangThai.HasValue)
                query = query.Where(h => h.TrangThai == trangThai.Value);

            // Sap xep
            query = (sapXepTheo?.ToLower(), thuTuSapXep?.ToLower()) switch
            {
                ("tenhangxe", "desc") => query.OrderByDescending(h => h.TenHangXe),
                ("tenhangxe", _) => query.OrderBy(h => h.TenHangXe),
                ("quocgia", "desc") => query.OrderByDescending(h => h.QuocGia),
                ("quocgia", _) => query.OrderBy(h => h.QuocGia),
                (_, "desc") => query.OrderByDescending(h => h.MaHangXe),
                _ => query.OrderBy(h => h.MaHangXe)
            };

            var tongSo = await query.CountAsync();

            if (trang < 1) trang = 1;
            if (kichThuocTrang < 1) kichThuocTrang = 10;
            if (kichThuocTrang > 100) kichThuocTrang = 100;

            var danhSach = await query
                .Skip((trang - 1) * kichThuocTrang)
                .Take(kichThuocTrang)
                .ToListAsync();

            // Lay danh sach quoc gia de do vao dropdown Loc
            ViewBag.QuocGiaList = await _context.HangXes
                .Where(h => h.QuocGia != null)
                .Select(h => h.QuocGia!)
                .Distinct()
                .OrderBy(q => q)
                .ToListAsync();

            var vm = new HangXeListVM
            {
                DanhSach = danhSach,
                TuKhoa = tuKhoa,
                QuocGia = quocGia,
                TrangThai = trangThai,
                SapXepTheo = sapXepTheo,
                ThuTuSapXep = thuTuSapXep,
                Trang = trang,
                KichThuocTrang = kichThuocTrang,
                TongSoBanGhi = tongSo
            };

            return View(vm);
        }

        // CHI TIET
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var hangXe = await _context.HangXes
                .Include(h => h.Xes)
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.MaHangXe == id);

            if (hangXe == null)
            {
                TempData["Error"] = "Khong tim thay hang xe";
                return RedirectToAction(nameof(Index));
            }

            return View(hangXe);
        }

        // THEM MOI
        [HttpGet]
        public IActionResult Create()
        {
            return View(new HangXeFormVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HangXeFormVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var daTonTai = await _context.HangXes
                .AnyAsync(h => h.TenHangXe == vm.TenHangXe.Trim());

            if (daTonTai)
            {
                ModelState.AddModelError(nameof(vm.TenHangXe),
                    "Ten hang xe da ton tai");
                return View(vm);
            }

            var hangXe = new HangXe
            {
                TenHangXe = vm.TenHangXe.Trim(),
                QuocGia = vm.QuocGia?.Trim(),
                MoTa = vm.MoTa?.Trim(),
                TrangThai = vm.TrangThai
            };

            _context.HangXes.Add(hangXe);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Them hang xe '{hangXe.TenHangXe}' thanh cong";
            return RedirectToAction(nameof(Index));
        }

        // SUA
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var hangXe = await _context.HangXes.FindAsync(id);
            if (hangXe == null)
            {
                TempData["Error"] = "Khong tim thay hang xe";
                return RedirectToAction(nameof(Index));
            }

            var vm = new HangXeFormVM
            {
                MaHangXe = hangXe.MaHangXe,
                TenHangXe = hangXe.TenHangXe,
                QuocGia = hangXe.QuocGia,
                MoTa = hangXe.MoTa,
                TrangThai = hangXe.TrangThai
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HangXeFormVM vm)
        {
            if (id != vm.MaHangXe)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(vm);

            var hangXe = await _context.HangXes.FindAsync(id);
            if (hangXe == null)
            {
                TempData["Error"] = "Khong tim thay hang xe";
                return RedirectToAction(nameof(Index));
            }

            var daTonTai = await _context.HangXes
                .AnyAsync(h => h.TenHangXe == vm.TenHangXe.Trim()
                            && h.MaHangXe != id);

            if (daTonTai)
            {
                ModelState.AddModelError(nameof(vm.TenHangXe),
                    "Ten hang xe da ton tai");
                return View(vm);
            }

            hangXe.TenHangXe = vm.TenHangXe.Trim();
            hangXe.QuocGia = vm.QuocGia?.Trim();
            hangXe.MoTa = vm.MoTa?.Trim();
            hangXe.TrangThai = vm.TrangThai;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Cap nhat hang xe thanh cong";
            return RedirectToAction(nameof(Index));
        }

        // DOI TRANG THAI
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiTrangThai(int id)
        {
            var hangXe = await _context.HangXes
                .Include(h => h.Xes)
                .FirstOrDefaultAsync(h => h.MaHangXe == id);

            if (hangXe == null)
            {
                TempData["Error"] = "Khong tim thay hang xe";
                return RedirectToAction(nameof(Index));
            }

            if (hangXe.TrangThai)
            {
                var conXeHoatDong = hangXe.Xes
                    .Any(x => x.TinhTrang != TinhTrangXe.NgungHoatDong);

                if (conXeHoatDong)
                {
                    TempData["Error"] = "Khong the ngung hoat dong hang xe " +
                        "vi con xe dang hoat dong. Vui long xu ly cac xe truoc.";
                    return RedirectToAction(nameof(Index));
                }
            }

            hangXe.TrangThai = !hangXe.TrangThai;
            await _context.SaveChangesAsync();

            TempData["Success"] = hangXe.TrangThai
                ? "Da kich hoat lai hang xe"
                : "Da ngung hoat dong hang xe";

            return RedirectToAction(nameof(Index));
        }
    }
}