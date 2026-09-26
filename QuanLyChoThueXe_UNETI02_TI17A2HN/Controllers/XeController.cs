// XeController.cs - quan ly xe (CRUD + tim kiem + loc + sap xep + phan trang)
// sinh vien thuc hien: Dao Gia Hung - 23103100065
// module 2 - quan ly xe, bang gia thue

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

            if (maLoaiXe.HasValue) query = query.Where(x => x.MaLoaiXe == maLoaiXe.Value);
            if (maHangXe.HasValue) query = query.Where(x => x.MaHangXe == maHangXe.Value);
            if (soCho.HasValue) query = query.Where(x => x.SoCho == soCho.Value);
            if (!string.IsNullOrWhiteSpace(tinhTrang))
                query = query.Where(x => x.TinhTrang == tinhTrang);
            if (namSanXuatTu.HasValue) query = query.Where(x => x.NamSanXuat >= namSanXuatTu.Value);
            if (namSanXuatDen.HasValue) query = query.Where(x => x.NamSanXuat <= namSanXuatDen.Value);

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

            var nowSort = DateTime.Now;
            query = (sapXepTheo?.ToLower(), thuTuSapXep?.ToLower()) switch
            {
                ("tenxe", "desc") => query.OrderByDescending(x => x.TenXe),
                ("tenxe", _) => query.OrderBy(x => x.TenXe),

                ("namsanxuat", "desc") => query.OrderByDescending(x => x.NamSanXuat),
                ("namsanxuat", _) => query.OrderBy(x => x.NamSanXuat),

                ("socho", "desc") => query.OrderByDescending(x => x.SoCho),
                ("socho", _) => query.OrderBy(x => x.SoCho),

                ("dongia", "desc") => query.OrderByDescending(x =>
                    _context.BangGiaThues
                        .Where(b => b.TrangThai &&
                                    b.TuNgay <= nowSort && b.DenNgay >= nowSort &&
                                    (b.MaXe == x.MaXe || b.MaLoaiXe == x.MaLoaiXe))
                        .Select(b => (decimal?)b.DonGiaNgay)
                        .FirstOrDefault() ?? 0),

                ("dongia", _) => query.OrderBy(x =>
                    _context.BangGiaThues
                        .Where(b => b.TrangThai &&
                                    b.TuNgay <= nowSort && b.DenNgay >= nowSort &&
                                    (b.MaXe == x.MaXe || b.MaLoaiXe == x.MaLoaiXe))
                        .Select(b => (decimal?)b.DonGiaNgay)
                        .FirstOrDefault() ?? 0),

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
                await LoadDropdownsAsync(vm);
                return View(vm);
            }

            var trungBienSo = await _context.Xes
                .AnyAsync(x => x.BienSo == vm.BienSo.Trim());
            if (trungBienSo)
            {
                ModelState.AddModelError(nameof(vm.BienSo), "Bien so da ton tai");
                await LoadDropdownsAsync(vm);
                return View(vm);
            }

            var loaiXe = await _context.LoaiXes.FindAsync(vm.MaLoaiXe);
            if (loaiXe == null || !loaiXe.TrangThai)
            {
                ModelState.AddModelError(nameof(vm.MaLoaiXe),
                    "Loai xe khong ton tai hoac da ngung hoat dong");
                await LoadDropdownsAsync(vm);
                return View(vm);
            }

            var hangXe = await _context.HangXes.FindAsync(vm.MaHangXe);
            if (hangXe == null || !hangXe.TrangThai)
            {
                ModelState.AddModelError(nameof(vm.MaHangXe),
                    "Hang xe khong ton tai hoac da ngung hoat dong");
                await LoadDropdownsAsync(vm);
                return View(vm);
            }

            string? duongDanAnh = null;
            try
            {
                duongDanAnh = await LuuAnhAsync(vm.FileAnh);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(vm.FileAnh), ex.Message);
                await LoadDropdownsAsync(vm);
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
                AnhXe = duongDanAnh
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
            if (id != vm.MaXe) return BadRequest();

            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync(vm);
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
                await LoadDropdownsAsync(vm);
                return View(vm);
            }

            if (vm.SoKmHienTai < xe.SoKmHienTai)
            {
                ModelState.AddModelError(nameof(vm.SoKmHienTai),
                    $"So km moi phai lon hon hoac bang {xe.SoKmHienTai:N0} km");
                await LoadDropdownsAsync(vm);
                return View(vm);
            }

            if (vm.FileAnh != null && vm.FileAnh.Length > 0)
            {
                try
                {
                    var anhMoi = await LuuAnhAsync(vm.FileAnh);
                    XoaAnhCu(xe.AnhXe);
                    xe.AnhXe = anhMoi;
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(nameof(vm.FileAnh), ex.Message);
                    await LoadDropdownsAsync(vm);
                    return View(vm);
                }
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

            if (tinhTrangMoi == TinhTrangXe.NgungHoatDong)
            {
                var conLichDat = await _context.DatXes.AnyAsync(d =>
                    d.MaXe == id &&
                    d.TrangThai != TrangThaiDatXe.DaHuy &&
                    d.TrangThai != TrangThaiDatXe.HoanThanh);

                if (conLichDat)
                {
                    TempData["Error"] =
                        "Xe con lich dat chua hoan thanh, khong the ngung hoat dong";
                    return RedirectToAction(nameof(Index));
                }
            }

            if (tinhTrangMoi == TinhTrangXe.BaoDuong)
            {
                var now = DateTime.Now;
                var coLichSapToi = await _context.DatXes.AnyAsync(d =>
                    d.MaXe == id &&
                    d.TrangThai != TrangThaiDatXe.DaHuy &&
                    d.TrangThai != TrangThaiDatXe.HoanThanh &&
                    d.ThoiGianNhanDuKien >= now);

                if (coLichSapToi)
                {
                    TempData["Error"] =
                        "Xe co lich dat sap toi, khong the chuyen sang bao duong";
                    return RedirectToAction(nameof(Index));
                }
            }

            xe.TinhTrang = tinhTrangMoi;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Da doi tinh trang xe thanh '{tinhTrangMoi}'";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> KiemTraCoTheChoThue(int id)
        {
            var xe = await _context.Xes.FindAsync(id);
            if (xe == null)
                return NotFound(new { CoThe = false, LyDo = "Khong tim thay xe" });

            if (xe.TinhTrang == TinhTrangXe.BaoDuong)
                return Ok(new { CoThe = false, LyDo = "Xe dang bao duong" });

            if (xe.TinhTrang == TinhTrangXe.NgungHoatDong)
                return Ok(new { CoThe = false, LyDo = "Xe da ngung hoat dong" });

            if (xe.TinhTrang == TinhTrangXe.DangChoThue)
                return Ok(new { CoThe = false, LyDo = "Xe dang duoc thue" });

            return Ok(new { CoThe = true, LyDo = "" });
        }

        [HttpGet]
        public async Task<IActionResult> KiemTraTrongKhoang(int id, DateTime tuNgay, DateTime denNgay)
        {
            if (tuNgay >= denNgay)
                return BadRequest(new { ThongBao = "Khoang thoi gian khong hop le" });

            var xe = await _context.Xes.FindAsync(id);
            if (xe == null)
                return NotFound(new { ThongBao = "Khong tim thay xe" });

            if (xe.TinhTrang == TinhTrangXe.BaoDuong ||
                xe.TinhTrang == TinhTrangXe.NgungHoatDong)
            {
                return Ok(new
                {
                    Trong = false,
                    LyDo = $"Xe dang o trang thai '{xe.TinhTrang}'"
                });
            }

            var biTrung = await _context.DatXes.AnyAsync(d =>
                d.MaXe == id &&
                d.TrangThai != TrangThaiDatXe.DaHuy &&
                d.TrangThai != TrangThaiDatXe.HoanThanh &&
                d.ThoiGianNhanDuKien < denNgay &&
                d.ThoiGianTraDuKien > tuNgay);

            return Ok(new
            {
                Trong = !biTrung,
                LyDo = biTrung ? "Xe da co lich dat trong khoang nay" : ""
            });
        }

        private async Task<string?> LuuAnhAsync(IFormFile? fileAnh)
        {
            if (fileAnh == null || fileAnh.Length == 0) return null;

            var ext = Path.GetExtension(fileAnh.FileName).ToLower();
            var choPhep = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            if (!choPhep.Contains(ext))
                throw new InvalidOperationException(
                    "Chi cho phep file anh (.jpg, .jpeg, .png, .gif, .webp)");

            var thuMuc = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "xe");
            if (!Directory.Exists(thuMuc)) Directory.CreateDirectory(thuMuc);

            var tenFile = Guid.NewGuid().ToString() + ext;
            var duongDanDayDu = Path.Combine(thuMuc, tenFile);

            using (var stream = new FileStream(duongDanDayDu, FileMode.Create))
            {
                await fileAnh.CopyToAsync(stream);
            }

            return "/images/xe/" + tenFile;
        }

        private void XoaAnhCu(string? duongDanAnh)
        {
            if (string.IsNullOrEmpty(duongDanAnh)) return;

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot",
                duongDanAnh.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

            if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
        }

        private async Task LoadDropdownsAsync(XeFormVM vm)
        {
            vm.DanhSachLoaiXe = await LayDanhSachLoaiXe();
            vm.DanhSachHangXe = await LayDanhSachHangXe();
            vm.DanhSachTinhTrang = LayDanhSachTinhTrang();
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