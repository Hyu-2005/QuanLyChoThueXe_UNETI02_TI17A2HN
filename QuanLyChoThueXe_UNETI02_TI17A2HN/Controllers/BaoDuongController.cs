// ==============================================================
// Bắt Đầu Code của SV4: Nguyễn Hoàng Đức Hiếu
// file: Controllers/BaoDuongController.cs
// nội dung: STT 11 - Quản lý và Theo dõi Bảo dưỡng xe nâng cao (Lập phiếu, Cảnh báo định kỳ theo KM, Nghiệm thu hoàn tất & tự động mở khóa xe về Sẵn sàng)
// sinh viên thực hiện : Nguyễn Hoàng Đức Hiếu -  23103100116 - SV4 
// ==============================================================
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Data;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Helpers;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Controllers
{
    [SessionAuthorize(Roles = new[] { VaiTro.Admin, VaiTro.NhanVien })]
    public class BaoDuongController : Controller
    {
        private readonly AppDbContext _context;

        public BaoDuongController(AppDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // 1. DANH SÁCH & DASHBOARD BẢO DƯỠNG
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> Index(
            string? tuKhoa,
            string? trangThai,
            string? loaiBaoDuong,
            string tab = "dangbaoduong")
        {
            ViewBag.TabHienTai = tab;
            ViewBag.TuKhoa = tuKhoa;
            ViewBag.TrangThai = trangThai;
            ViewBag.LoaiBaoDuong = loaiBaoDuong;

            var query = _context.LichSuBaoDuongs
                .Include(b => b.Xe).ThenInclude(x => x!.HangXe)
                .Include(b => b.Xe).ThenInclude(x => x!.LoaiXe)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var kw = tuKhoa.Trim();
                query = query.Where(b => (b.Xe != null && (b.Xe.BienSo.Contains(kw) || b.Xe.TenXe.Contains(kw)))
                                      || (b.GaraBaoDuong != null && b.GaraBaoDuong.Contains(kw))
                                      || b.NoiDungBaoDuong.Contains(kw));
            }

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(b => b.TrangThai == trangThai);
            }

            if (!string.IsNullOrWhiteSpace(loaiBaoDuong))
            {
                query = query.Where(b => b.LoaiBaoDuong == loaiBaoDuong);
            }

            var danhSach = await query
                .OrderByDescending(b => b.TrangThai == TrangThaiBaoDuong.DangBaoDuong)
                .ThenByDescending(b => b.NgayBatDau)
                .ToListAsync();

            // Thống kê nhanh
            ViewBag.TongDangBaoDuong = await _context.LichSuBaoDuongs.CountAsync(b => b.TrangThai == TrangThaiBaoDuong.DangBaoDuong);
            ViewBag.TongHoanThanh = await _context.LichSuBaoDuongs.CountAsync(b => b.TrangThai == TrangThaiBaoDuong.DaHoanThanh);
            ViewBag.TongChiPhiBaoDuong = await _context.LichSuBaoDuongs
                .Where(b => b.TrangThai == TrangThaiBaoDuong.DaHoanThanh)
                .SumAsync(b => (decimal?)b.ChiPhi) ?? 0;

            // Danh sách các xe có cảnh báo bảo dưỡng định kỳ:
            // Tiêu chí: Mỗi chu kỳ 5.000 km xe cần bảo dưỡng định kỳ (ví dụ dư >= 4.200 km)
            var tatCaXe = await _context.Xes
                .Include(x => x.HangXe)
                .Include(x => x.LoaiXe)
                .AsNoTracking()
                .ToListAsync();

            var xeCanBaoDuong = tatCaXe
                .Where(x => x.TinhTrang == TinhTrangXe.BaoDuong || (x.SoKmHienTai % 5000 >= 4000 && x.SoKmHienTai > 0))
                .Select(x => new
                {
                    Xe = x,
                    KmHienTai = x.SoKmHienTai,
                    KmMocTiepTheo = ((int)(x.SoKmHienTai / 5000) + 1) * 5000,
                    KmConLai = (((int)(x.SoKmHienTai / 5000) + 1) * 5000) - x.SoKmHienTai,
                    PhanTramChuKy = (x.SoKmHienTai % 5000) / 5000 * 100,
                    DangBaoDuong = x.TinhTrang == TinhTrangXe.BaoDuong
                })
                .ToList();

            ViewBag.DanhSachXeCanBaoDuong = xeCanBaoDuong;
            ViewBag.TongXeCanBaoDuong = xeCanBaoDuong.Count;

            ViewBag.DanhSachLoai = LoaiBaoDuong.TatCa
                .Select(l => new SelectListItem { Value = l, Text = l, Selected = l == loaiBaoDuong })
                .ToList();

            ViewBag.DanhSachTrangThai = TrangThaiBaoDuong.TatCa
                .Select(t => new SelectListItem { Value = t, Text = t, Selected = t == trangThai })
                .ToList();

            return View(danhSach);
        }

        // =====================================================
        // 2. LẬP PHIẾU ĐƯA XE VÀO BẢO DƯỠNG (CHỦ ĐỘNG HOẶC ĐỊNH KỲ)
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> TaoMoi(int? maXe)
        {
            await NapDanhSachXeAsync(maXe);

            var model = new LichSuBaoDuong
            {
                NgayBatDau = DateTime.Now,
                TrangThai = TrangThaiBaoDuong.DangBaoDuong,
                LoaiBaoDuong = LoaiBaoDuong.DinhKy,
                NguoiPhuTrach = HttpContext.Session.GetString("HoTen") ?? "Nhân viên kỹ thuật"
            };

            if (maXe.HasValue)
            {
                var xe = await _context.Xes.FindAsync(maXe.Value);
                if (xe != null)
                {
                    model.MaXe = xe.MaXe;
                    model.SoKmKhiBaoDuong = xe.SoKmHienTai;
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaoMoi(LichSuBaoDuong model)
        {
            var xe = await _context.Xes.FindAsync(model.MaXe);
            if (xe == null)
            {
                ModelState.AddModelError(nameof(model.MaXe), "Phương tiện không tồn tại.");
            }

            if (ModelState.IsValid)
            {
                // Cập nhật số km nếu chưa nhập
                if (model.SoKmKhiBaoDuong <= 0 && xe != null)
                {
                    model.SoKmKhiBaoDuong = xe.SoKmHienTai;
                }

                model.TrangThai = TrangThaiBaoDuong.DangBaoDuong;
                _context.LichSuBaoDuongs.Add(model);

                // Chuyển trạng thái xe sang Bảo dưỡng
                if (xe != null)
                {
                    xe.TinhTrang = TinhTrangXe.BaoDuong;
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = $"Đã lập phiếu bảo dưỡng cho xe '{xe?.BienSo}'. Xe đã chuyển sang trạng thái 'Bảo dưỡng'.";
                return RedirectToAction(nameof(Index));
            }

            await NapDanhSachXeAsync(model.MaXe);
            return View(model);
        }

        // =====================================================
        // 3. NGHIỆM THU & HOÀN TẤT BẢO DƯỠNG (TỰ ĐỘNG MỞ KHÓA VỀ SẴN SÀNG)
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> HoanThanh(int id)
        {
            var phieu = await _context.LichSuBaoDuongs
                .Include(b => b.Xe).ThenInclude(x => x!.HangXe)
                .Include(b => b.Xe).ThenInclude(x => x!.LoaiXe)
                .FirstOrDefaultAsync(b => b.MaBaoDuong == id);

            if (phieu == null)
            {
                TempData["Error"] = "Không tìm thấy phiếu bảo dưỡng.";
                return RedirectToAction(nameof(Index));
            }

            if (phieu.TrangThai == TrangThaiBaoDuong.DaHoanThanh)
            {
                TempData["Warning"] = "Phiếu bảo dưỡng này đã được nghiệm thu hoàn tất trước đó.";
                return RedirectToAction(nameof(Index));
            }

            phieu.NgayHoanThanh ??= DateTime.Now;
            return View(phieu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HoanThanh(int id, DateTime ngayHoanThanh, decimal chiPhi, string? garaBaoDuong, string noiDungBaoDuong, string? ghiChu)
        {
            var phieu = await _context.LichSuBaoDuongs
                .Include(b => b.Xe)
                .FirstOrDefaultAsync(b => b.MaBaoDuong == id);

            if (phieu == null)
            {
                TempData["Error"] = "Không tìm thấy phiếu bảo dưỡng.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(noiDungBaoDuong))
            {
                ModelState.AddModelError("NoiDungBaoDuong", "Vui lòng nhập chi tiết các hạng mục đã hoàn thành sửa chữa/bảo dưỡng.");
                return View(phieu);
            }

            phieu.NgayHoanThanh = ngayHoanThanh;
            phieu.ChiPhi = chiPhi;
            phieu.GaraBaoDuong = garaBaoDuong;
            phieu.NoiDungBaoDuong = noiDungBaoDuong;
            phieu.GhiChu = ghiChu;
            phieu.TrangThai = TrangThaiBaoDuong.DaHoanThanh;

            // ==============================================================
            // Bắt Đầu Code của SV4: Nguyễn Hoàng Đức Hiếu
            // file: Controllers/BaoDuongController.cs
            // nội dung: STT 11 - Tự động mở khóa xe sau bảo dưỡng: chuyển từ "Bảo dưỡng" về "Sẵn sàng" để đưa vào kinh doanh tiếp
            // sinh viên thực hiện : Nguyễn Hoàng Đức Hiếu -  23103100116 - SV4 
            // ==============================================================
            if (phieu.Xe != null)
            {
                phieu.Xe.TinhTrang = TinhTrangXe.SanSang;
            }
            // ==============================================================
            // Kết thúc Code của SV4
            // ==============================================================

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã nghiệm thu hoàn tất bảo dưỡng cho xe '{phieu.Xe?.BienSo}'. Xe đã được đưa về trạng thái 'Sẵn sàng' và sẵn sàng đón khách thuê!";
            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // 4. XÓA PHIẾU BẢO DƯỠNG (KHI NHẬP NHẦM)
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Xoa(int id)
        {
            var phieu = await _context.LichSuBaoDuongs
                .Include(b => b.Xe)
                .FirstOrDefaultAsync(b => b.MaBaoDuong == id);

            if (phieu != null)
            {
                // Nếu xe đang ở trạng thái Bảo dưỡng và không còn phiếu bảo dưỡng nào khác đang mở thì trả về Sẵn sàng
                if (phieu.Xe != null && phieu.Xe.TinhTrang == TinhTrangXe.BaoDuong)
                {
                    bool conPhieuKhac = await _context.LichSuBaoDuongs
                        .AnyAsync(b => b.MaXe == phieu.MaXe && b.MaBaoDuong != id && b.TrangThai == TrangThaiBaoDuong.DangBaoDuong);
                    if (!conPhieuKhac)
                    {
                        phieu.Xe.TinhTrang = TinhTrangXe.SanSang;
                    }
                }

                _context.LichSuBaoDuongs.Remove(phieu);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã xóa phiếu bảo dưỡng.";
            }

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // HELPER
        // =====================================================
        private async Task NapDanhSachXeAsync(int? selectedMaXe = null)
        {
            var xes = await _context.Xes
                .Include(x => x.HangXe)
                .OrderBy(x => x.TenXe)
                .Select(x => new
                {
                    x.MaXe,
                    DisplayText = $"{x.TenXe} ({x.BienSo}) - {x.TinhTrang}"
                })
                .ToListAsync();

            ViewBag.DanhSachXe = new SelectList(xes, "MaXe", "DisplayText", selectedMaXe);
            ViewBag.DanhSachLoaiBaoDuong = new SelectList(LoaiBaoDuong.TatCa);
        }
    }
}
// ==============================================================
// Kết thúc Code của SV4
// ==============================================================
