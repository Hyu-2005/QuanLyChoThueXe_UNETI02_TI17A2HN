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
using QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.LoaiXe;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Controllers
{
    [SessionAuthorize(Roles = new[] { VaiTro.Admin, VaiTro.NhanVien })]
    public class LoaiXeController : Controller
    {
        private readonly AppDbContext _context;

        public LoaiXeController(AppDbContext context)
        {
            _context = context;
        }

        // DANH SACH - co Tim kiem + Loc + Sap xep + Phan trang
        [HttpGet]
        public async Task<IActionResult> Index(
            string? tuKhoa,
            int? soChoTu,
            int? soChoDen,
            bool? trangThai,
            string? sapXepTheo,
            string? thuTuSapXep,
            int trang = 1,
            int kichThuocTrang = 10)
        {
            // 1. Truy van goc
            var query = _context.LoaiXes
    .Include(l => l.Xes)              // <-- THEM DONG NAY
    .AsNoTracking()
    .AsQueryable();

            // 2. Tim kiem theo ten
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var kw = tuKhoa.Trim();
                query = query.Where(l => l.TenLoaiXe.Contains(kw));
            }

            // 3. Loc theo so cho
            if (soChoTu.HasValue)
                query = query.Where(l => l.SoCho >= soChoTu.Value);
            if (soChoDen.HasValue)
                query = query.Where(l => l.SoCho <= soChoDen.Value);

            // 4. Loc theo trang thai
            if (trangThai.HasValue)
                query = query.Where(l => l.TrangThai == trangThai.Value);

            // 5. Sap xep
            query = (sapXepTheo?.ToLower(), thuTuSapXep?.ToLower()) switch
            {
                ("tenloaixe", "desc") => query.OrderByDescending(l => l.TenLoaiXe),
                ("tenloaixe", _) => query.OrderBy(l => l.TenLoaiXe),
                ("socho", "desc") => query.OrderByDescending(l => l.SoCho),
                ("socho", _) => query.OrderBy(l => l.SoCho),
                (_, "desc") => query.OrderByDescending(l => l.MaLoaiXe),
                _ => query.OrderBy(l => l.MaLoaiXe)
            };

            // 6. Dem tong so ban ghi TRUOC khi phan trang
            var tongSo = await query.CountAsync();

            // 7. Phan trang
            if (trang < 1) trang = 1;
            if (kichThuocTrang < 1) kichThuocTrang = 10;
            if (kichThuocTrang > 100) kichThuocTrang = 100;

            var danhSach = await query
                .Skip((trang - 1) * kichThuocTrang)
                .Take(kichThuocTrang)
                .ToListAsync();

            // 8. Tao ViewModel
            var vm = new LoaiXeListVM
            {
                DanhSach = danhSach,
                TuKhoa = tuKhoa,
                SoChoTu = soChoTu,
                SoChoDen = soChoDen,
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
            var loaiXe = await _context.LoaiXes
                .Include(l => l.Xes)
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.MaLoaiXe == id);

            if (loaiXe == null)
            {
                TempData["Error"] = "Khong tim thay loai xe";
                return RedirectToAction(nameof(Index));
            }

            return View(loaiXe);
        }

        // THEM MOI
        [HttpGet]
        public IActionResult Create()
        {
            return View(new LoaiXeFormVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LoaiXeFormVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // Kiem tra trung ten loai xe
            var daTonTai = await _context.LoaiXes
                .AnyAsync(l => l.TenLoaiXe == vm.TenLoaiXe.Trim());

            if (daTonTai)
            {
                ModelState.AddModelError(nameof(vm.TenLoaiXe),
                    "Ten loai xe da ton tai");
                return View(vm);
            }

            var loaiXe = new LoaiXe
            {
                TenLoaiXe = vm.TenLoaiXe.Trim(),
                SoCho = vm.SoCho,
                MoTa = vm.MoTa?.Trim(),
                TrangThai = vm.TrangThai
            };

            _context.LoaiXes.Add(loaiXe);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Them loai xe '{loaiXe.TenLoaiXe}' thanh cong";
            return RedirectToAction(nameof(Index));
        }

        // SUA
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var loaiXe = await _context.LoaiXes.FindAsync(id);
            if (loaiXe == null)
            {
                TempData["Error"] = "Khong tim thay loai xe";
                return RedirectToAction(nameof(Index));
            }

            var vm = new LoaiXeFormVM
            {
                MaLoaiXe = loaiXe.MaLoaiXe,
                TenLoaiXe = loaiXe.TenLoaiXe,
                SoCho = loaiXe.SoCho,
                MoTa = loaiXe.MoTa,
                TrangThai = loaiXe.TrangThai
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LoaiXeFormVM vm)
        {
            if (id != vm.MaLoaiXe)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(vm);

            var loaiXe = await _context.LoaiXes.FindAsync(id);
            if (loaiXe == null)
            {
                TempData["Error"] = "Khong tim thay loai xe";
                return RedirectToAction(nameof(Index));
            }

            // Kiem tra trung ten (tru chinh no)
            var daTonTai = await _context.LoaiXes
                .AnyAsync(l => l.TenLoaiXe == vm.TenLoaiXe.Trim()
                            && l.MaLoaiXe != id);

            if (daTonTai)
            {
                ModelState.AddModelError(nameof(vm.TenLoaiXe),
                    "Ten loai xe da ton tai");
                return View(vm);
            }

            // Kiem tra: neu loai xe ngung hoat dong nhung con xe dang hoat dong
            if (!vm.TrangThai && loaiXe.TrangThai)
            {
                var conXeHoatDong = await _context.Xes
                    .AnyAsync(x => x.MaLoaiXe == id
                                && x.TinhTrang != TinhTrangXe.NgungHoatDong);

                if (conXeHoatDong)
                {
                    TempData["Warning"] = "Loai xe nay con xe dang hoat dong, " +
                        "viec ngung hoat dong loai xe se khong cho phep gan vao xe moi";
                }
            }

            loaiXe.TenLoaiXe = vm.TenLoaiXe.Trim();
            loaiXe.SoCho = vm.SoCho;
            loaiXe.MoTa = vm.MoTa?.Trim();
            loaiXe.TrangThai = vm.TrangThai;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Cap nhat loai xe thanh cong";
            return RedirectToAction(nameof(Index));
        }

        // DOI TRANG THAI (thay cho Delete vat ly)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiTrangThai(int id)
        {
            var loaiXe = await _context.LoaiXes
                .Include(l => l.Xes)
                .FirstOrDefaultAsync(l => l.MaLoaiXe == id);

            if (loaiXe == null)
            {
                TempData["Error"] = "Khong tim thay loai xe";
                return RedirectToAction(nameof(Index));
            }

            // Neu dang ngung hoat dong ma con xe dang hoat dong -> canh bao
            if (loaiXe.TrangThai)
            {
                var conXeHoatDong = loaiXe.Xes
                    .Any(x => x.TinhTrang != TinhTrangXe.NgungHoatDong);

                if (conXeHoatDong)
                {
                    TempData["Error"] = "Khong the ngung hoat dong loai xe " +
                        "vi con xe dang hoat dong. Vui long xu ly cac xe truoc.";
                    return RedirectToAction(nameof(Index));
                }
            }

            loaiXe.TrangThai = !loaiXe.TrangThai;
            await _context.SaveChangesAsync();

            TempData["Success"] = loaiXe.TrangThai
                ? "Da kich hoat lai loai xe"
                : "Da ngung hoat dong loai xe";

            return RedirectToAction(nameof(Index));
        }
    }
}