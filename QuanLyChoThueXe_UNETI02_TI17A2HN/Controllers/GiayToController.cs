// ============================================================
// File: Controllers/GiayToController.cs
// Họ và tên: [Vũ Tiến Đạt]
// Mã sinh viên: [23103100119]
// Nội dung thực hiện: Khách hàng tải giấy tờ (CCCD, GPLX) + nhân viên kiểm tra/xác minh
// Module: Module 3 - Khách hàng
// ============================================================

using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Data;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Helpers;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;
using QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.GiayTo;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Controllers
{
    public class GiayToController : Controller
    {
        private const long KichThuocToiDa = 5 * 1024 * 1024; // 5 MB
        private static readonly string[] DuoiChoPhep = { ".jpg", ".jpeg", ".png", ".webp", ".pdf" };

        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public GiayToController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // Giay to luu NGOAI wwwroot de khong truy cap truc tiep duoc bang URL
        private string ThuMucGoc => Path.Combine(_env.ContentRootPath, "App_Data", "GiayTo");

        // =====================================================
        // KHACH HANG: XEM + TAI GIAY TO CUA TOI
        // =====================================================

        [SessionAuthorize(Roles = new[] { VaiTro.KhachHang })]
        [HttpGet]
        public async Task<IActionResult> CuaToi()
        {
            var maKh = HttpContext.Session.GetMaKhachHang();
            if (!maKh.HasValue)
            {
                TempData["Error"] = "Không tìm thấy thông tin khách hàng, vui lòng đăng nhập lại";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var vm = new GiayToCuaToiVM();
            await NapDanhSachAsync(vm, maKh.Value);
            return View(vm);
        }

        [SessionAuthorize(Roles = new[] { VaiTro.KhachHang })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaiLen(GiayToCuaToiVM vm)
        {
            var maKh = HttpContext.Session.GetMaKhachHang();
            if (!maKh.HasValue)
            {
                TempData["Error"] = "Không tìm thấy thông tin khách hàng, vui lòng đăng nhập lại";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            // ----- Kiem tra du lieu -----
            var so = (vm.SoGiayTo ?? string.Empty).Trim();

            if (!LoaiGiayTo.TatCa.Contains(vm.LoaiGiayTo))
            {
                ModelState.AddModelError(nameof(vm.LoaiGiayTo), "Loại giấy tờ không hợp lệ");
            }
            else if (vm.LoaiGiayTo == LoaiGiayTo.CCCD)
            {
                if (!Regex.IsMatch(so, @"^(\d{9}|\d{12})$"))
                    ModelState.AddModelError(nameof(vm.SoGiayTo), "Số CCCD/CMND phải gồm 9 hoặc 12 chữ số");
            }
            else // GPLX
            {
                if (!Regex.IsMatch(so, @"^[A-Za-z0-9]{6,20}$"))
                    ModelState.AddModelError(nameof(vm.SoGiayTo), "Số GPLX phải gồm 6-20 chữ cái hoặc chữ số");

                if (!vm.NgayHetHan.HasValue)
                    ModelState.AddModelError(nameof(vm.NgayHetHan), "Vui lòng nhập ngày hết hạn GPLX");
                else if (vm.NgayHetHan.Value.Date < DateTime.Today)
                    ModelState.AddModelError(nameof(vm.NgayHetHan), "GPLX đã hết hạn, không thể tải lên");
            }

            var loiTruoc = await KiemTraFileAsync(vm.FileMatTruoc, batBuoc: true);
            if (loiTruoc != null) ModelState.AddModelError(nameof(vm.FileMatTruoc), loiTruoc);

            var loiSau = await KiemTraFileAsync(vm.FileMatSau, batBuoc: false);
            if (loiSau != null) ModelState.AddModelError(nameof(vm.FileMatSau), loiSau);

            if (!ModelState.IsValid)
            {
                await NapDanhSachAsync(vm, maKh.Value);
                return View(nameof(CuaToi), vm);
            }

            // ----- Luu file -----
            string? duongDanTruoc = null;
            string? duongDanSau = null;
            try
            {
                duongDanTruoc = await LuuFileAsync(vm.FileMatTruoc!, maKh.Value);
                if (vm.FileMatSau != null && vm.FileMatSau.Length > 0)
                    duongDanSau = await LuuFileAsync(vm.FileMatSau, maKh.Value);

                var giayTo = await _context.GiayToKhachHangs
                    .FirstOrDefaultAsync(g => g.MaKhachHang == maKh.Value && g.LoaiGiayTo == vm.LoaiGiayTo);

                string? truocCu = null, sauCu = null;

                if (giayTo == null)
                {
                    giayTo = new GiayToKhachHang
                    {
                        MaKhachHang = maKh.Value,
                        LoaiGiayTo = vm.LoaiGiayTo
                    };
                    _context.GiayToKhachHangs.Add(giayTo);
                }
                else
                {
                    truocCu = giayTo.DuongDanMatTruoc;
                    sauCu = giayTo.DuongDanMatSau;
                }

                giayTo.SoGiayTo = so;
                giayTo.NgayHetHan = vm.LoaiGiayTo == LoaiGiayTo.GPLX ? vm.NgayHetHan!.Value.Date : null;
                giayTo.DuongDanMatTruoc = duongDanTruoc;
                giayTo.DuongDanMatSau = duongDanSau;
                giayTo.TrangThai = TrangThaiGiayTo.ChoKiemTra;   // tai lai thi phai kiem tra lai
                giayTo.LyDoTuChoi = null;
                giayTo.NgayTai = DateTime.Now;
                giayTo.NgayKiemTra = null;
                giayTo.NguoiKiemTra = null;

                await _context.SaveChangesAsync();

                // Luu thanh cong moi xoa file cu
                XoaFile(truocCu);
                XoaFile(sauCu);
            }
            catch
            {
                // Loi giua chung: don dep file moi vua ghi
                XoaFile(duongDanTruoc);
                XoaFile(duongDanSau);
                TempData["Error"] = "Không thể lưu giấy tờ, vui lòng thử lại";
                return RedirectToAction(nameof(CuaToi));
            }

            TempData["Success"] = $"Đã tải lên {LoaiGiayTo.GetDisplayName(vm.LoaiGiayTo)}. Giấy tờ đang chờ nhân viên kiểm tra.";
            return RedirectToAction(nameof(CuaToi));
        }

        [SessionAuthorize(Roles = new[] { VaiTro.KhachHang })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Xoa(int id)
        {
            var maKh = HttpContext.Session.GetMaKhachHang();
            var giayTo = await _context.GiayToKhachHangs
                .FirstOrDefaultAsync(g => g.MaGiayTo == id && g.MaKhachHang == maKh);

            if (giayTo == null)
            {
                TempData["Error"] = "Không tìm thấy giấy tờ";
                return RedirectToAction(nameof(CuaToi));
            }

            if (giayTo.TrangThai == TrangThaiGiayTo.DaXacMinh)
            {
                TempData["Error"] = "Giấy tờ đã được xác minh, không thể xóa. Hãy tải bản mới nếu cần thay đổi";
                return RedirectToAction(nameof(CuaToi));
            }

            var truoc = giayTo.DuongDanMatTruoc;
            var sau = giayTo.DuongDanMatSau;

            _context.GiayToKhachHangs.Remove(giayTo);
            await _context.SaveChangesAsync();

            XoaFile(truoc);
            XoaFile(sau);

            TempData["Success"] = "Đã xóa giấy tờ";
            return RedirectToAction(nameof(CuaToi));
        }

        // =====================================================
        // XEM ANH GIAY TO (chu so huu hoac nhan vien/admin)
        // =====================================================

        [SessionAuthorize]
        [HttpGet]
        public async Task<IActionResult> Xem(int id, string mat = "truoc")
        {
            var giayTo = await _context.GiayToKhachHangs
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.MaGiayTo == id);

            if (giayTo == null) return NotFound();

            var session = HttpContext.Session;
            var laChuSoHuu = session.LaKhachHang() && giayTo.MaKhachHang == session.GetMaKhachHang();
            if (!laChuSoHuu && !session.LaQuanTri())
                return RedirectToAction("AccessDenied", "TaiKhoan");

            var duongDan = mat == "sau" ? giayTo.DuongDanMatSau : giayTo.DuongDanMatTruoc;
            var fileDayDu = TimFileAnToan(duongDan);
            if (fileDayDu == null || !System.IO.File.Exists(fileDayDu)) return NotFound();

            var contentType = Path.GetExtension(fileDayDu).ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                ".pdf" => "application/pdf",
                _ => "application/octet-stream"
            };

            Response.Headers["X-Content-Type-Options"] = "nosniff";
            Response.Headers["Cache-Control"] = "private, no-store";
            return PhysicalFile(fileDayDu, contentType);
        }

        // =====================================================
        // NHAN VIEN / ADMIN: KIEM TRA GIAY TO
        // =====================================================

        [SessionAuthorize(Roles = new[] { VaiTro.Admin, VaiTro.NhanVien })]
        [HttpGet]
        public async Task<IActionResult> Index(string? trangThai, string? tuKhoa)
        {
            var query = _context.GiayToKhachHangs
                .Include(g => g.KhachHang)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrEmpty(trangThai))
                query = query.Where(g => g.TrangThai == trangThai);

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var kw = tuKhoa.Trim();
                query = query.Where(g =>
                    g.SoGiayTo.Contains(kw) ||
                    (g.KhachHang != null &&
                        (g.KhachHang.HoTen.Contains(kw) || g.KhachHang.SoDienThoai.Contains(kw))));
            }

            var danhSach = await query
                .OrderBy(g => g.TrangThai == TrangThaiGiayTo.ChoKiemTra ? 0 : 1)
                .ThenByDescending(g => g.NgayTai)
                .Take(200)
                .ToListAsync();

            return View(new GiayToDuyetVM
            {
                DanhSach = danhSach,
                TrangThai = trangThai,
                TuKhoa = tuKhoa
            });
        }

        [SessionAuthorize(Roles = new[] { VaiTro.Admin, VaiTro.NhanVien })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacMinh(int id)
        {
            var giayTo = await _context.GiayToKhachHangs
                .Include(g => g.KhachHang)
                .FirstOrDefaultAsync(g => g.MaGiayTo == id);

            if (giayTo == null)
            {
                TempData["Error"] = "Không tìm thấy giấy tờ";
                return RedirectToAction(nameof(Index));
            }

            if (giayTo.LoaiGiayTo == LoaiGiayTo.GPLX &&
                giayTo.NgayHetHan.HasValue && giayTo.NgayHetHan.Value.Date < DateTime.Today)
            {
                TempData["Error"] = "GPLX đã hết hạn, hãy từ chối giấy tờ này";
                return RedirectToAction(nameof(Index));
            }

            giayTo.TrangThai = TrangThaiGiayTo.DaXacMinh;
            giayTo.LyDoTuChoi = null;
            giayTo.NgayKiemTra = DateTime.Now;
            giayTo.NguoiKiemTra = HttpContext.Session.GetString(SessionKeys.HoTen);

            // GPLX duoc xac minh -> cap nhat thong tin GPLX vao ho so khach hang
            if (giayTo.LoaiGiayTo == LoaiGiayTo.GPLX && giayTo.KhachHang != null)
            {
                giayTo.KhachHang.SoGiayPhepLaiXe = giayTo.SoGiayTo;
                giayTo.KhachHang.NgayHetHanGPLX = giayTo.NgayHetHan;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã xác minh giấy tờ";
            return RedirectToAction(nameof(Index), new { trangThai = TrangThaiGiayTo.ChoKiemTra });
        }

        [SessionAuthorize(Roles = new[] { VaiTro.Admin, VaiTro.NhanVien })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TuChoi(int id, string? lyDo)
        {
            if (string.IsNullOrWhiteSpace(lyDo))
            {
                TempData["Error"] = "Vui lòng nhập lý do từ chối";
                return RedirectToAction(nameof(Index), new { trangThai = TrangThaiGiayTo.ChoKiemTra });
            }

            var giayTo = await _context.GiayToKhachHangs.FirstOrDefaultAsync(g => g.MaGiayTo == id);
            if (giayTo == null)
            {
                TempData["Error"] = "Không tìm thấy giấy tờ";
                return RedirectToAction(nameof(Index));
            }

            lyDo = lyDo.Trim();
            giayTo.TrangThai = TrangThaiGiayTo.TuChoi;
            giayTo.LyDoTuChoi = lyDo.Length > 300 ? lyDo[..300] : lyDo;
            giayTo.NgayKiemTra = DateTime.Now;
            giayTo.NguoiKiemTra = HttpContext.Session.GetString(SessionKeys.HoTen);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã từ chối giấy tờ";
            return RedirectToAction(nameof(Index), new { trangThai = TrangThaiGiayTo.ChoKiemTra });
        }

        // =====================================================
        // HELPER
        // =====================================================

        private async Task NapDanhSachAsync(GiayToCuaToiVM vm, int maKhachHang)
        {
            vm.DanhSach = await _context.GiayToKhachHangs
                .AsNoTracking()
                .Where(g => g.MaKhachHang == maKhachHang)
                .OrderBy(g => g.LoaiGiayTo)
                .ToListAsync();
        }

        /// <summary>Tra ve thong bao loi, hoac null neu file hop le.</summary>
        private static async Task<string?> KiemTraFileAsync(IFormFile? file, bool batBuoc)
        {
            if (file == null || file.Length == 0)
                return batBuoc ? "Vui lòng chọn file ảnh/PDF" : null;

            if (file.Length > KichThuocToiDa)
                return "File quá lớn (tối đa 5 MB)";

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!DuoiChoPhep.Contains(ext))
                return "Chỉ chấp nhận file .jpg, .jpeg, .png, .webp hoặc .pdf";

            // Kiem tra noi dung that cua file (chan doi duoi file gia mao)
            var dau = new byte[12];
            int doc;
            using (var stream = file.OpenReadStream())
            {
                doc = await stream.ReadAsync(dau, 0, dau.Length);
            }
            if (doc < 4) return "File không hợp lệ";

            bool hopLe = ext switch
            {
                ".jpg" or ".jpeg" => dau[0] == 0xFF && dau[1] == 0xD8 && dau[2] == 0xFF,
                ".png" => dau[0] == 0x89 && dau[1] == 0x50 && dau[2] == 0x4E && dau[3] == 0x47,
                ".pdf" => dau[0] == 0x25 && dau[1] == 0x50 && dau[2] == 0x44 && dau[3] == 0x46,
                ".webp" => doc >= 12 && dau[0] == 0x52 && dau[1] == 0x49 && dau[2] == 0x46 && dau[3] == 0x46
                           && dau[8] == 0x57 && dau[9] == 0x45 && dau[10] == 0x42 && dau[11] == 0x50,
                _ => false
            };

            return hopLe ? null : "Nội dung file không đúng định dạng";
        }

        /// <summary>Luu file vao App_Data/GiayTo/{maKhachHang}/ va tra ve duong dan tuong doi.</summary>
        private async Task<string> LuuFileAsync(IFormFile file, int maKhachHang)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var thuMuc = Path.Combine(ThuMucGoc, maKhachHang.ToString());
            Directory.CreateDirectory(thuMuc);

            var tenFile = Guid.NewGuid().ToString("N") + ext;
            var duongDanDayDu = Path.Combine(thuMuc, tenFile);

            using (var stream = new FileStream(duongDanDayDu, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"{maKhachHang}/{tenFile}";
        }

        /// <summary>Doi duong dan tuong doi thanh duong dan day du, chan truy cap ra ngoai thu muc goc.</summary>
        private string? TimFileAnToan(string? duongDanTuongDoi)
        {
            if (string.IsNullOrWhiteSpace(duongDanTuongDoi)) return null;

            var goc = Path.GetFullPath(ThuMucGoc) + Path.DirectorySeparatorChar;
            var full = Path.GetFullPath(Path.Combine(ThuMucGoc,
                duongDanTuongDoi.Replace('/', Path.DirectorySeparatorChar)));

            return full.StartsWith(goc, StringComparison.OrdinalIgnoreCase) ? full : null;
        }

        private void XoaFile(string? duongDanTuongDoi)
        {
            var full = TimFileAnToan(duongDanTuongDoi);
            if (full != null && System.IO.File.Exists(full))
            {
                try { System.IO.File.Delete(full); } catch { /* bo qua loi xoa file */ }
            }
        }
    }
}
