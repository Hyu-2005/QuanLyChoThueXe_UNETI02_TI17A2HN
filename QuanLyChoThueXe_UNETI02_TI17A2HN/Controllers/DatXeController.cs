// File: Controllers/DatXeController.cs
// Noi dung: Khach hang tim xe trong, dat xe, kiem tra trung lich, xem/huy don cua toi
// Module: Module 3 - Khach hang tim xe trong, dat xe, kiem tra trung lich

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Data;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Helpers;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants;
using QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.DatXe;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Controllers
{
    [SessionAuthorize(Roles = new[] { VaiTro.KhachHang })]
    public class DatXeController : Controller
    {
        private readonly AppDbContext _context;

        public DatXeController(AppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // TIM XE TRONG
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> TimXeTrong()
        {
            var vm = new TimXeTrongVM
            {
                DanhSachLoaiXe = await LayDanhSachLoaiXe()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TimXeTrong(TimXeTrongVM vm)
        {
            vm.DanhSachLoaiXe = await LayDanhSachLoaiXe();

            if (vm.ThoiGianNhan == null || vm.ThoiGianTra == null)
            {
                ModelState.AddModelError(string.Empty, "Vui long chon du thoi gian nhan va tra");
                return View(vm);
            }

            var tuNgay = vm.ThoiGianNhan.Value;
            var denNgay = vm.ThoiGianTra.Value;

            if (tuNgay >= denNgay)
            {
                ModelState.AddModelError(string.Empty, "Thoi gian tra phai sau thoi gian nhan");
                return View(vm);
            }

            if (tuNgay < DateTime.Now)
            {
                ModelState.AddModelError(string.Empty, "Thoi gian nhan khong duoc o trong qua khu");
                return View(vm);
            }

            var query = _context.Xes
                .Include(x => x.LoaiXe)
                .Include(x => x.HangXe)
                .AsNoTracking()
                .Where(x => TinhTrangXe.CoTheChoThue.Contains(x.TinhTrang));

            if (vm.MaLoaiXe.HasValue)
                query = query.Where(x => x.MaLoaiXe == vm.MaLoaiXe.Value);
            if (vm.SoCho.HasValue)
                query = query.Where(x => x.SoCho >= vm.SoCho.Value);

            var xeUngVien = await query.ToListAsync();

            var maXeBiTrung = await _context.DatXes
                .Where(d => TrangThaiDatXe.ChiemLich.Contains(d.TrangThai)
                         && d.ThoiGianNhanDuKien < denNgay
                         && d.ThoiGianTraDuKien > tuNgay)
                .Select(d => d.MaXe)
                .Distinct()
                .ToListAsync();

            xeUngVien = xeUngVien.Where(x => !maXeBiTrung.Contains(x.MaXe)).ToList();

            var ketQua = new List<XeTrongVM>();
            foreach (var xe in xeUngVien)
            {
                var bangGia = await LayBangGiaHieuLuc(xe.MaXe, xe.MaLoaiXe, tuNgay, denNgay);
                if (bangGia == null) continue;

                ketQua.Add(new XeTrongVM
                {
                    MaXe = xe.MaXe,
                    BienSo = xe.BienSo,
                    TenXe = xe.TenXe,
                    AnhXe = xe.AnhXe,
                    TenLoaiXe = xe.LoaiXe?.TenLoaiXe ?? "",
                    TenHangXe = xe.HangXe?.TenHangXe ?? "",
                    SoCho = xe.SoCho,
                    DonGiaNgay = bangGia.DonGiaNgay,
                    DonGiaGio = bangGia.DonGiaGio,
                    TienCocMacDinh = bangGia.TienCocMacDinh,
                    TongTienDuKien = TinhTongTienDuKien(bangGia.DonGiaNgay, bangGia.DonGiaGio, tuNgay, denNgay)
                });
            }

            vm.KetQua = ketQua.OrderBy(k => k.TongTienDuKien).ToList();
            vm.DaTimKiem = true;

            return View(vm);
        }

        // ============================================================
        // DAT XE
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> DatXe(int maXe, DateTime tuNgay, DateTime denNgay)
        {
            if (tuNgay >= denNgay || tuNgay < DateTime.Now)
            {
                TempData["Error"] = "Khoang thoi gian khong hop le, vui long tim lai";
                return RedirectToAction(nameof(TimXeTrong));
            }

            var xe = await _context.Xes
                .Include(x => x.LoaiXe)
                .Include(x => x.HangXe)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaXe == maXe);

            if (xe == null)
            {
                TempData["Error"] = "Khong tim thay xe";
                return RedirectToAction(nameof(TimXeTrong));
            }

            if (!TinhTrangXe.CoTheChoThue.Contains(xe.TinhTrang))
            {
                TempData["Error"] = $"Xe nay hien khong the cho thue (tinh trang: {xe.TinhTrang})";
                return RedirectToAction(nameof(TimXeTrong));
            }

            var biTrung = await _context.DatXes.AnyAsync(d =>
                d.MaXe == maXe &&
                TrangThaiDatXe.ChiemLich.Contains(d.TrangThai) &&
                d.ThoiGianNhanDuKien < denNgay &&
                d.ThoiGianTraDuKien > tuNgay);

            if (biTrung)
            {
                TempData["Error"] = "Xe nay da co lich trung trong khoang thoi gian ban chon, vui long tim lai";
                return RedirectToAction(nameof(TimXeTrong));
            }

            var bangGia = await LayBangGiaHieuLuc(xe.MaXe, xe.MaLoaiXe, tuNgay, denNgay);
            if (bangGia == null)
            {
                TempData["Error"] = "Xe nay chua co bang gia ap dung cho khoang thoi gian ban chon";
                return RedirectToAction(nameof(TimXeTrong));
            }

            var vm = new DatXeFormVM
            {
                MaXe = xe.MaXe,
                BienSo = xe.BienSo,
                TenXe = xe.TenXe,
                TenLoaiXe = xe.LoaiXe?.TenLoaiXe ?? "",
                TenHangXe = xe.HangXe?.TenHangXe ?? "",
                AnhXe = xe.AnhXe,
                ThoiGianNhanDuKien = tuNgay,
                ThoiGianTraDuKien = denNgay,
                DonGiaApDung = bangGia.DonGiaNgay,
                TienCoc = bangGia.TienCocMacDinh,
                TongTienDuKien = TinhTongTienDuKien(bangGia.DonGiaNgay, bangGia.DonGiaGio, tuNgay, denNgay)
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DatXe(DatXeFormVM vm)
        {
            var maKhachHang = HttpContext.Session.GetMaKhachHang();
            if (!maKhachHang.HasValue)
            {
                TempData["Error"] = "Tai khoan cua ban chua lien ket ho so khach hang, vui long lien he quan tri vien";
                return RedirectToAction(nameof(TimXeTrong));
            }

            var khachHang = await _context.KhachHangs.FindAsync(maKhachHang.Value);
            if (khachHang == null || !khachHang.TrangThai)
            {
                TempData["Error"] = "Ho so khach hang khong hop le hoac da bi khoa";
                return RedirectToAction(nameof(TimXeTrong));
            }

            if (vm.ThoiGianNhanDuKien >= vm.ThoiGianTraDuKien)
                ModelState.AddModelError(string.Empty, "Thoi gian tra phai sau thoi gian nhan");

            if (vm.ThoiGianNhanDuKien < DateTime.Now)
                ModelState.AddModelError(string.Empty, "Thoi gian nhan khong duoc o trong qua khu");

            if (!khachHang.GPLXConHieuLuc(vm.ThoiGianNhanDuKien))
                ModelState.AddModelError(string.Empty,
                    "Giay phep lai xe cua ban da het han hoac chua duoc cap nhat, vui long lien he quan tri vien truoc khi dat xe");

            var xe = await _context.Xes
                .Include(x => x.LoaiXe)
                .Include(x => x.HangXe)
                .FirstOrDefaultAsync(x => x.MaXe == vm.MaXe);

            if (xe == null)
            {
                TempData["Error"] = "Khong tim thay xe";
                return RedirectToAction(nameof(TimXeTrong));
            }

            if (!TinhTrangXe.CoTheChoThue.Contains(xe.TinhTrang))
                ModelState.AddModelError(string.Empty, $"Xe nay hien khong the cho thue (tinh trang: {xe.TinhTrang})");

            var biTrung = await _context.DatXes.AnyAsync(d =>
                d.MaXe == vm.MaXe &&
                TrangThaiDatXe.ChiemLich.Contains(d.TrangThai) &&
                d.ThoiGianNhanDuKien < vm.ThoiGianTraDuKien &&
                d.ThoiGianTraDuKien > vm.ThoiGianNhanDuKien);

            if (biTrung)
                ModelState.AddModelError(string.Empty, "Xe nay vua co nguoi khac dat trung lich, vui long tim lai");

            var bangGia = await LayBangGiaHieuLuc(xe.MaXe, xe.MaLoaiXe, vm.ThoiGianNhanDuKien, vm.ThoiGianTraDuKien);
            if (bangGia == null)
                ModelState.AddModelError(string.Empty, "Xe nay chua co bang gia ap dung cho khoang thoi gian ban chon");

            if (!ModelState.IsValid)
            {
                vm.BienSo = xe.BienSo;
                vm.TenXe = xe.TenXe;
                vm.TenLoaiXe = xe.LoaiXe?.TenLoaiXe ?? "";
                vm.TenHangXe = xe.HangXe?.TenHangXe ?? "";
                vm.AnhXe = xe.AnhXe;

                if (bangGia != null)
                {
                    vm.DonGiaApDung = bangGia.DonGiaNgay;
                    vm.TienCoc = bangGia.TienCocMacDinh;
                    vm.TongTienDuKien = TinhTongTienDuKien(
                        bangGia.DonGiaNgay, bangGia.DonGiaGio, vm.ThoiGianNhanDuKien, vm.ThoiGianTraDuKien);
                }

                return View(vm);
            }

            var datXe = new Models.Entities.DatXe
            {
                MaKhachHang = khachHang.MaKhachHang,
                MaXe = xe.MaXe,
                ThoiGianNhanDuKien = vm.ThoiGianNhanDuKien,
                ThoiGianTraDuKien = vm.ThoiGianTraDuKien,
                DiaDiemNhan = vm.DiaDiemNhan.Trim(),
                DiaDiemTra = vm.DiaDiemTra.Trim(),
                DonGiaApDung = bangGia!.DonGiaNgay,
                TienCoc = bangGia.TienCocMacDinh,
                TrangThai = TrangThaiDatXe.ChoDuyet,
                GhiChu = string.IsNullOrWhiteSpace(vm.GhiChu) ? null : vm.GhiChu.Trim()
            };

            _context.DatXes.Add(datXe);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Dat xe thanh cong! Ma don #{datXe.MaDatXe} dang cho duyet.";
            return RedirectToAction(nameof(DonCuaToi));
        }

        // ============================================================
        // DON CUA TOI
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> DonCuaToi(
            string? tuKhoa,
            string? trangThai,
            string? sapXepTheo,
            string? thuTuSapXep,
            int trang = 1,
            int kichThuocTrang = 10)
        {
            var maKhachHang = HttpContext.Session.GetMaKhachHang();
            if (!maKhachHang.HasValue)
            {
                TempData["Error"] = "Tai khoan cua ban chua lien ket ho so khach hang";
                return RedirectToAction("Index", "Home");
            }

            var query = _context.DatXes
                .Include(d => d.Xe)
                    .ThenInclude(x => x!.LoaiXe)
                .Include(d => d.Xe)
                    .ThenInclude(x => x!.HangXe)
                .AsNoTracking()
                .Where(d => d.MaKhachHang == maKhachHang.Value);

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var kw = tuKhoa.Trim();
                query = query.Where(d =>
                    (d.Xe != null && d.Xe.BienSo.Contains(kw)) ||
                    (d.Xe != null && d.Xe.TenXe.Contains(kw)));
            }

            if (!string.IsNullOrWhiteSpace(trangThai))
                query = query.Where(d => d.TrangThai == trangThai);

            query = (sapXepTheo?.ToLower(), thuTuSapXep?.ToLower()) switch
            {
                ("thoigiannhan", "asc") => query.OrderBy(d => d.ThoiGianNhanDuKien),
                ("thoigiannhan", _) => query.OrderByDescending(d => d.ThoiGianNhanDuKien),
                ("ngaydat", "asc") => query.OrderBy(d => d.NgayDat),
                (_, _) => query.OrderByDescending(d => d.NgayDat)
            };

            var tongSo = await query.CountAsync();

            if (trang < 1) trang = 1;
            if (kichThuocTrang < 1) kichThuocTrang = 10;
            if (kichThuocTrang > 100) kichThuocTrang = 100;

            var danhSach = await query
                .Skip((trang - 1) * kichThuocTrang)
                .Take(kichThuocTrang)
                .ToListAsync();

            var vm = new DonCuaToiVM
            {
                DanhSach = danhSach,
                TuKhoa = tuKhoa,
                TrangThai = trangThai,
                SapXepTheo = sapXepTheo,
                ThuTuSapXep = thuTuSapXep,
                Trang = trang,
                KichThuocTrang = kichThuocTrang,
                TongSoBanGhi = tongSo
            };

            return View(vm);
        }

        // HUY DON - chi khach hang so huu don moi duoc huy, va phai con dieu kien
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Huy(int id)
        {
            var maKhachHang = HttpContext.Session.GetMaKhachHang();
            if (!maKhachHang.HasValue)
                return RedirectToAction("Index", "Home");

            var datXe = await _context.DatXes.FirstOrDefaultAsync(d => d.MaDatXe == id);

            if (datXe == null)
            {
                TempData["Error"] = "Khong tim thay don dat xe";
                return RedirectToAction(nameof(DonCuaToi));
            }

            // Chi cho phep huy don CUA CHINH MINH
            if (datXe.MaKhachHang != maKhachHang.Value)
            {
                TempData["Error"] = "Ban khong co quyen huy don nay";
                return RedirectToAction(nameof(DonCuaToi));
            }

            if (!datXe.CoTheHuy())
            {
                TempData["Error"] = $"Don o trang thai '{datXe.TrangThai}' khong the huy";
                return RedirectToAction(nameof(DonCuaToi));
            }

            var soGioConLai = (datXe.ThoiGianNhanDuKien - DateTime.Now).TotalHours;
            if (soGioConLai < QuyDinhThue.SoGioToiThieuTruocKhiHuy)
            {
                TempData["Error"] =
                    $"Chi duoc huy don truoc gio nhan xe toi thieu {QuyDinhThue.SoGioToiThieuTruocKhiHuy} gio";
                return RedirectToAction(nameof(DonCuaToi));
            }

            datXe.TrangThai = TrangThaiDatXe.DaHuy;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Da huy don #{datXe.MaDatXe}";
            return RedirectToAction(nameof(DonCuaToi));
        }

        // ============================================================
        // HELPERS
        // ============================================================

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

        private async Task<Models.Entities.BangGiaThue?> LayBangGiaHieuLuc(
            int maXe, int maLoaiXe, DateTime tuNgay, DateTime denNgay)
        {
            return await _context.BangGiaThues
                .AsNoTracking()
                .Where(b => b.TrangThai
                         && b.TuNgay <= tuNgay
                         && b.DenNgay >= denNgay
                         && (b.MaXe == maXe || b.MaLoaiXe == maLoaiXe))
                .OrderByDescending(b => b.MaXe.HasValue ? 1 : 0)
                .FirstOrDefaultAsync();
        }

        private static decimal TinhTongTienDuKien(decimal donGiaNgay, decimal donGiaGio, DateTime tuNgay, DateTime denNgay)
        {
            var soGio = (denNgay - tuNgay).TotalHours;

            if (soGio >= QuyDinhThue.NguongGioSangNgay)
            {
                var soNgay = Math.Max(QuyDinhThue.SoNgayToiThieu, (int)Math.Ceiling((denNgay - tuNgay).TotalDays));
                return soNgay * donGiaNgay;
            }
            else
            {
                var soGioTinh = Math.Max(QuyDinhThue.SoGioToiThieu, (int)Math.Ceiling(soGio));
                return soGioTinh * donGiaGio;
            }
        }
    }
}