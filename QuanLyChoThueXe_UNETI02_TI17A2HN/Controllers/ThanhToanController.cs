// ThanhToanController.cs - quản lý thanh toán (có voucher + điểm thưởng)
// sinh vien thuc hien: Duong Lam Huy - 23103100120
// module 5 - tinh tien, thanh toan, lich su va thong ke
// + Chuc nang nang cao: Xuat Excel danh sach + In hoa don PDF
// + Chuc nang moi: Voucher + Vi diem thuong (Loyalty Points)

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Data;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Helpers;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Services.Interfaces;
using QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.ThanhToan;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Controllers
{
    [SessionAuthorize]
    public class ThanhToanController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ITinhTienService _tinhTienService;
        private readonly IExportService _exportService;
        private readonly IMaGiamGiaService _maGiamGiaService;

        public ThanhToanController(
            AppDbContext context,
            ITinhTienService tinhTienService,
            IExportService exportService,
            IMaGiamGiaService maGiamGiaService)
        {
            _context = context;
            _tinhTienService = tinhTienService;
            _exportService = exportService;
            _maGiamGiaService = maGiamGiaService;
        }

        // =====================================================
        // DANH SACH THANH TOAN
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> Index(
            string? tuKhoa,
            string? trangThaiThanhToan,
            string? phuongThucThanhToan,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? sapXepTheo,
            string? thuTuSapXep,
            int trang = 1,
            int kichThuocTrang = 10)
        {
            var vaiTro = HttpContext.Session.GetVaiTro();
            var laKhachHang = vaiTro == VaiTro.KhachHang;

            var query = _context.ThanhToans
                .Include(t => t.DatXe)
                    .ThenInclude(d => d!.KhachHang)
                .Include(t => t.DatXe)
                    .ThenInclude(d => d!.Xe)
                .Include(t => t.MaGiamGia)
                .AsNoTracking()
                .AsQueryable();

            // Khach hang chi xem thanh toan cua minh
            if (laKhachHang)
            {
                var maKhachHang = HttpContext.Session.GetMaKhachHang();
                query = query.Where(t => t.DatXe != null
                                      && t.DatXe.MaKhachHang == maKhachHang);
            }

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var kw = tuKhoa.Trim();
                query = query.Where(t =>
                    (t.DatXe != null && t.DatXe.KhachHang != null
                        && t.DatXe.KhachHang.HoTen.Contains(kw)) ||
                    (t.DatXe != null && t.DatXe.Xe != null
                        && t.DatXe.Xe.BienSo.Contains(kw)));
            }

            if (!string.IsNullOrWhiteSpace(trangThaiThanhToan))
                query = query.Where(t => t.TrangThaiThanhToan == trangThaiThanhToan);

            if (!string.IsNullOrWhiteSpace(phuongThucThanhToan))
                query = query.Where(t => t.PhuongThucThanhToan == phuongThucThanhToan);

            if (tuNgay.HasValue)
                query = query.Where(t => t.NgayThanhToan >= tuNgay.Value);
            if (denNgay.HasValue)
                query = query.Where(t => t.NgayThanhToan <= denNgay.Value);

            query = (sapXepTheo?.ToLower(), thuTuSapXep?.ToLower()) switch
            {
                ("ngaythanhtoan", "asc") => query.OrderBy(t => t.NgayThanhToan),
                ("ngaythanhtoan", _) => query.OrderByDescending(t => t.NgayThanhToan),
                ("tongtien", "asc") => query.OrderBy(t => t.TongThanhToan),
                ("tongtien", _) => query.OrderByDescending(t => t.TongThanhToan),
                _ => query.OrderByDescending(t => t.NgayThanhToan)
            };

            var tongSo = await query.CountAsync();
            if (trang < 1) trang = 1;

            var danhSach = await query
                .Skip((trang - 1) * kichThuocTrang)
                .Take(kichThuocTrang)
                .ToListAsync();

            // Tong doanh thu tren toan bo query (không chỉ trang hiện tại)
            var tongDoanhThu = await query
                .Where(t => t.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                .SumAsync(t => (decimal?)t.TongThanhToan) ?? 0;

            var vm = new ThanhToanListVM
            {
                DanhSach = danhSach,
                TuKhoa = tuKhoa,
                TrangThaiThanhToan = trangThaiThanhToan,
                PhuongThucThanhToan = phuongThucThanhToan,
                TuNgay = tuNgay,
                DenNgay = denNgay,
                SapXepTheo = sapXepTheo,
                ThuTuSapXep = thuTuSapXep,
                Trang = trang,
                KichThuocTrang = kichThuocTrang,
                TongSoBanGhi = tongSo,
                DanhSachTrangThai = LayDanhSachTrangThai(),
                DanhSachPhuongThuc = LayDanhSachPhuongThuc(),
                TongDoanhThu = tongDoanhThu
            };

            return View(vm);
        }

        // =====================================================
        // THANH TOAN DON - GET
        // =====================================================
        [HttpGet]
        [SessionAuthorize(Roles = new[] { VaiTro.Admin, VaiTro.NhanVien })]
        public async Task<IActionResult> ThanhToan(int id)
        {
            var datXe = await _context.DatXes
                .Include(d => d.KhachHang)
                .Include(d => d.Xe)
                .Include(d => d.TraXe)
                .Include(d => d.ThanhToans)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.MaDatXe == id);

            if (datXe == null)
            {
                TempData["Error"] = "Khong tim thay don dat xe";
                return RedirectToAction(nameof(Index));
            }

            if (datXe.TrangThai != TrangThaiDatXe.ChoThanhToan)
            {
                TempData["Error"] = $"Don dang o trang thai '{datXe.TrangThai}', khong the thanh toan";
                return RedirectToAction(nameof(Index));
            }

            var daThanhToan = datXe.ThanhToans
                .Any(t => t.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan);
            if (daThanhToan)
            {
                TempData["Error"] = "Don nay da duoc thanh toan truoc do";
                return RedirectToAction(nameof(Index));
            }

            // Lấy thông tin khách hàng để nạp điểm
            var khachHang = await _context.KhachHangs
                .AsNoTracking()
                .FirstOrDefaultAsync(k => k.MaKhachHang == datXe.MaKhachHang);

            // Tính toán ban đầu (chưa voucher, chưa điểm)
            var ketQua = _tinhTienService.TinhThanhToanDayDu(datXe, datXe.TraXe, null, 0);

            var giaTriMotDiem = HttpContext.RequestServices
                .GetRequiredService<IConfiguration>()
                .GetValue<decimal?>("LoyaltyPoints:GiaTriMotDiem") ?? 5000m;

            var vm = new ThanhToanVM
            {
                MaDatXe = datXe.MaDatXe,
                BienSo = datXe.Xe?.BienSo,
                TenXe = datXe.Xe?.TenXe,
                TenKhachHang = datXe.KhachHang?.HoTen,
                SoDienThoaiKhachHang = datXe.KhachHang?.SoDienThoai,
                ThoiGianNhanDuKien = datXe.ThoiGianNhanDuKien,
                ThoiGianTraDuKien = datXe.ThoiGianTraDuKien,
                ThoiGianTraThucTe = datXe.TraXe?.ThoiGianTraThucTe,

                SoNgayThue = ketQua.SoNgayThue,
                TienThueGoc = ketQua.TienThueGoc,
                TienThue = ketQua.TienThue,
                TongPhuPhi = ketQua.TongPhuPhi,
                TienCocDaThu = ketQua.TienCocDaThu,
                TongThanhToan = ketQua.TongThanhToan,
                SoTienConLai = ketQua.SoTienConLai,

                PhiQuaHan = datXe.TraXe?.PhiQuaHan ?? 0,
                PhiVuotKm = datXe.TraXe?.PhiVuotKm ?? 0,
                PhiNhienLieu = datXe.TraXe?.PhiNhienLieu ?? 0,
                PhiHuHong = datXe.TraXe?.PhiHuHong ?? 0,

                // Điểm thưởng
                DiemHienCo = khachHang?.DiemTichLuy ?? 0,
                GiaTriMotDiem = giaTriMotDiem,
                DiemToiDaCoTheDung = 0,
                DiemSuDung = 0,
                TienTruTuDiem = 0,
                DiemDuocCongSauThanhToan = _tinhTienService.TinhDiemDuocCong(ketQua.TienThue),

                PhuongThucThanhToan = PhuongThucThanhToan.TienMat,
                DanhSachPhuongThuc = LayDanhSachPhuongThuc()
            };

            // Tính điểm tối đa có thể dùng
            if (khachHang != null && khachHang.DiemTichLuy > 0)
            {
                var (hopLe, _) = _tinhTienService.KiemTraDiemSuDung(
                    khachHang.DiemTichLuy, khachHang.DiemTichLuy, ketQua.TienThueGoc);
                vm.DiemToiDaCoTheDung = hopLe;
            }

            return View(vm);
        }

        // =====================================================
        // AJAX: ÁP DỤNG VOUCHER + ĐIỂM (preview trước khi lưu)
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize(Roles = new[] { VaiTro.Admin, VaiTro.NhanVien })]
        public async Task<IActionResult> ApDungVoucher([FromBody] ApDungVoucherRequestVM req)
        {
            if (req == null || req.MaDatXe <= 0)
                return Json(new ApDungVoucherResponseVM { ThanhCong = false, Loi = "Dữ liệu không hợp lệ" });

            var datXe = await _context.DatXes
                .Include(d => d.KhachHang)
                .Include(d => d.TraXe)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.MaDatXe == req.MaDatXe);

            if (datXe == null)
                return Json(new ApDungVoucherResponseVM { ThanhCong = false, Loi = "Không tìm thấy đơn đặt xe" });

            if (datXe.TrangThai != TrangThaiDatXe.ChoThanhToan)
                return Json(new ApDungVoucherResponseVM { ThanhCong = false, Loi = "Đơn không ở trạng thái chờ thanh toán" });

            var khachHang = datXe.KhachHang;

            // 1. Tính tiền gốc (chưa voucher, chưa điểm)
            var tienThueGoc = _tinhTienService.TinhThanhToan(datXe, datXe.TraXe).TienThueGoc;

            // 2. Validate voucher
            Models.Entities.MaGiamGia? voucher = null;
            string? loiVoucher = null;
            if (!string.IsNullOrWhiteSpace(req.Code))
            {
                (voucher, loiVoucher) = await _maGiamGiaService.TimVaKiemTraAsync(
                    req.Code, datXe.MaKhachHang, tienThueGoc);
                if (loiVoucher != null)
                    return Json(new ApDungVoucherResponseVM { ThanhCong = false, Loi = loiVoucher });
            }

            // 3. Validate điểm
            int diemSuDung = Math.Max(0, req.DiemSuDung);
            if (diemSuDung > 0 && khachHang != null)
            {
                // Tính tiền sau voucher (để kiểm tra điểm tối đa trên số tiền đã giảm voucher)
                decimal tienThueSauVoucher = tienThueGoc;
                if (voucher != null) tienThueSauVoucher -= voucher.TinhTienGiam(tienThueGoc);
                if (tienThueSauVoucher < 0) tienThueSauVoucher = 0;

                var (diemHopLe, loiDiem) = _tinhTienService.KiemTraDiemSuDung(
                    diemSuDung, khachHang.DiemTichLuy, tienThueSauVoucher);

                if (loiDiem != null)
                {
                    return Json(new ApDungVoucherResponseVM
                    {
                        ThanhCong = false,
                        Loi = loiDiem,
                        MaGiamGiaId = voucher?.MaGiamGiaId,
                        TienGiamVoucher = voucher?.TinhTienGiam(tienThueGoc) ?? 0,
                        Code = voucher?.Code
                    });
                }
                diemSuDung = diemHopLe;
            }

            // 4. Tính toán đầy đủ
            var ketQua = _tinhTienService.TinhThanhToanDayDu(datXe, datXe.TraXe, voucher, diemSuDung);

            return Json(new ApDungVoucherResponseVM
            {
                ThanhCong = true,
                Code = voucher?.Code,
                MaGiamGiaId = voucher?.MaGiamGiaId,
                TenChuongTrinh = voucher?.TenChuongTrinh,
                TienGiamVoucher = ketQua.TienGiamVoucher,

                TienTruTuDiem = ketQua.TienTruTuDiem,
                DiemDaDung = ketQua.DiemDaDung,
                DiemHienCo = khachHang?.DiemTichLuy ?? 0,
                GiaTriMotDiem = HttpContext.RequestServices
                    .GetRequiredService<IConfiguration>()
                    .GetValue<decimal?>("LoyaltyPoints:GiaTriMotDiem") ?? 5000m,

                TienThueGoc = ketQua.TienThueGoc,
                TienThueSauGiam = ketQua.TienThue,
                TongPhuPhi = ketQua.TongPhuPhi,
                TongThanhToan = ketQua.TongThanhToan,
                TienCocDaThu = ketQua.TienCocDaThu,
                SoTienConLai = ketQua.SoTienConLai
            });
        }

        // =====================================================
        // THANH TOAN DON - POST
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionAuthorize(Roles = new[] { VaiTro.Admin, VaiTro.NhanVien })]
        public async Task<IActionResult> ThanhToan(int id, ThanhToanVM vm)
        {
            if (id != vm.MaDatXe)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                await LoadThongTinThanhToanAsync(vm);
                return View(vm);
            }

            var datXe = await _context.DatXes
                .Include(d => d.KhachHang)
                .Include(d => d.TraXe)
                .Include(d => d.ThanhToans)
                .FirstOrDefaultAsync(d => d.MaDatXe == id);

            if (datXe == null)
            {
                TempData["Error"] = "Khong tim thay don dat xe";
                return RedirectToAction(nameof(Index));
            }

            if (datXe.TrangThai != TrangThaiDatXe.ChoThanhToan)
            {
                TempData["Error"] = $"Don dang o trang thai '{datXe.TrangThai}', khong the thanh toan";
                return RedirectToAction(nameof(Index));
            }

            var daThanhToan = datXe.ThanhToans
                .Any(t => t.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan);
            if (daThanhToan)
            {
                TempData["Error"] = "Don nay da duoc thanh toan truoc do";
                return RedirectToAction(nameof(Index));
            }

            // ===== TÍNH TOÁN LẠI TỪ DB =====
            var khachHang = datXe.KhachHang;
            if (khachHang == null)
            {
                TempData["Error"] = "Không tìm thấy khách hàng của đơn";
                return RedirectToAction(nameof(Index));
            }

            // 1. Kiểm tra voucher
            Models.Entities.MaGiamGia? voucher = null;
            if (!string.IsNullOrWhiteSpace(vm.MaGiamGiaCode))
            {
                var (v, loi) = await _maGiamGiaService.TimVaKiemTraAsync(
                    vm.MaGiamGiaCode, datXe.MaKhachHang,
                    _tinhTienService.TinhThanhToan(datXe, datXe.TraXe).TienThueGoc);

                if (loi != null)
                {
                    ModelState.AddModelError(nameof(vm.MaGiamGiaCode), loi);
                    await LoadThongTinThanhToanAsync(vm);
                    return View(vm);
                }
                voucher = v;
            }

            // 2. Kiểm tra điểm
            int diemSuDung = Math.Max(0, vm.DiemSuDung);
            if (diemSuDung > 0)
            {
                decimal tienThueSauVoucher = _tinhTienService.TinhThanhToan(datXe, datXe.TraXe).TienThueGoc;
                if (voucher != null) tienThueSauVoucher -= voucher.TinhTienGiam(tienThueSauVoucher);
                if (tienThueSauVoucher < 0) tienThueSauVoucher = 0;

                var (diemHopLe, loiDiem) = _tinhTienService.KiemTraDiemSuDung(
                    diemSuDung, khachHang.DiemTichLuy, tienThueSauVoucher);

                if (loiDiem != null)
                {
                    ModelState.AddModelError(nameof(vm.DiemSuDung), loiDiem);
                    await LoadThongTinThanhToanAsync(vm);
                    return View(vm);
                }
                diemSuDung = diemHopLe;
            }

            // 3. Tính toán đầy đủ
            var ketQua = _tinhTienService.TinhThanhToanDayDu(datXe, datXe.TraXe, voucher, diemSuDung);

            // 4. Số điểm được cộng sau khi thanh toán = dựa trên tiền thuê sau giảm
            var diemDuocCong = _tinhTienService.TinhDiemDuocCong(ketQua.TienThue);

            // ===== LƯU THANH TOÁN =====
            var thanhToan = datXe.ThanhToans
                .FirstOrDefault(t => t.TrangThaiThanhToan == TrangThaiThanhToan.ChuaThanhToan);

            if (thanhToan != null)
            {
                // Cập nhật bản ghi hiện tại
                thanhToan.TienThueGoc = ketQua.TienThueGoc;
                thanhToan.TienGiamVoucher = ketQua.TienGiamVoucher;
                thanhToan.DiemDaDung = ketQua.DiemDaDung;
                thanhToan.TienTruTuDiem = ketQua.TienTruTuDiem;
                thanhToan.DiemDuocCong = diemDuocCong;
                thanhToan.MaGiamGiaId = voucher?.MaGiamGiaId;
                thanhToan.TienThue = ketQua.TienThue;
                thanhToan.TongPhuPhi = ketQua.TongPhuPhi;
                thanhToan.TienCocDaThu = ketQua.TienCocDaThu;
                thanhToan.TongThanhToan = ketQua.TongThanhToan;
                thanhToan.SoTienConLai = ketQua.SoTienConLai;
                thanhToan.PhuongThucThanhToan = vm.PhuongThucThanhToan;
                thanhToan.NgayThanhToan = DateTime.Now;
                thanhToan.TrangThaiThanhToan = TrangThaiThanhToan.DaThanhToan;
                thanhToan.GhiChu = vm.GhiChu?.Trim();
            }
            else
            {
                // Tạo bản ghi thanh toán mới
                thanhToan = new Models.Entities.ThanhToan
                {
                    MaDatXe = datXe.MaDatXe,
                    TienThueGoc = ketQua.TienThueGoc,
                    TienGiamVoucher = ketQua.TienGiamVoucher,
                    DiemDaDung = ketQua.DiemDaDung,
                    TienTruTuDiem = ketQua.TienTruTuDiem,
                    DiemDuocCong = diemDuocCong,
                    MaGiamGiaId = voucher?.MaGiamGiaId,
                    TienThue = ketQua.TienThue,
                    TongPhuPhi = ketQua.TongPhuPhi,
                    TienCocDaThu = ketQua.TienCocDaThu,
                    TongThanhToan = ketQua.TongThanhToan,
                    SoTienConLai = ketQua.SoTienConLai,
                    PhuongThucThanhToan = vm.PhuongThucThanhToan,
                    NgayThanhToan = DateTime.Now,
                    TrangThaiThanhToan = TrangThaiThanhToan.DaThanhToan,
                    GhiChu = vm.GhiChu?.Trim()
                };
                _context.ThanhToans.Add(thanhToan);
            }

            // ===== TRỪ ĐIỂM ĐÃ DÙNG + CỘNG ĐIỂM MỚI =====
            if (ketQua.DiemDaDung > 0)
            {
                khachHang.DiemTichLuy -= ketQua.DiemDaDung;
                if (khachHang.DiemTichLuy < 0) khachHang.DiemTichLuy = 0;
            }

            if (diemDuocCong > 0)
            {
                khachHang.DiemTichLuy += diemDuocCong;
                khachHang.TongDiemDaTichLuy += diemDuocCong;
            }

            // ===== CHUYỂN ĐƠN SANG HOÀN THÀNH =====
            datXe.TrangThai = TrangThaiDatXe.HoanThanh;

            await _context.SaveChangesAsync();

            // ===== GHI NHẬN LƯỢT DÙNG VOUCHER (sau khi đã lưu thanh toán thành công) =====
            if (voucher != null && ketQua.TienGiamVoucher > 0)
            {
                await _maGiamGiaService.GhiNhanSuDungAsync(
                    voucher, datXe.MaKhachHang, datXe.MaDatXe, ketQua.TienGiamVoucher);
            }

            // ===== THÔNG BÁO =====
            var thongBao = $"Đã thanh toán đơn #{datXe.MaDatXe} thành công";
            if (ketQua.TienGiamVoucher > 0)
                thongBao += $" | Giảm voucher: {ketQua.TienGiamVoucher:N0}đ";
            if (ketQua.DiemDaDung > 0)
                thongBao += $" | Dùng {ketQua.DiemDaDung} điểm = {ketQua.TienTruTuDiem:N0}đ";
            if (diemDuocCong > 0)
                thongBao += $" | Cộng +{diemDuocCong} điểm";

            TempData["Success"] = thongBao;
            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // CHI TIET THANH TOAN
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var thanhToan = await _context.ThanhToans
                .Include(t => t.DatXe)
                    .ThenInclude(d => d!.KhachHang)
                .Include(t => t.DatXe)
                    .ThenInclude(d => d!.Xe)
                .Include(t => t.DatXe)
                    .ThenInclude(d => d!.TraXe)
                .Include(t => t.MaGiamGia)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.MaThanhToan == id);

            if (thanhToan == null)
            {
                TempData["Error"] = "Khong tim thay ban ghi thanh toan";
                return RedirectToAction(nameof(Index));
            }

            var vaiTro = HttpContext.Session.GetVaiTro();
            if (vaiTro == VaiTro.KhachHang)
            {
                var maKhachHang = HttpContext.Session.GetMaKhachHang();
                if (thanhToan.DatXe?.MaKhachHang != maKhachHang)
                {
                    TempData["Error"] = "Ban khong co quyen xem thanh toan nay";
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(thanhToan);
        }

        // =====================================================
        // XUAT EXCEL DANH SACH THANH TOAN
        // =====================================================
        [HttpGet]
        [SessionAuthorize(Roles = new[] { VaiTro.Admin, VaiTro.NhanVien })]
        public async Task<IActionResult> XuatExcel(
            string? tuKhoa,
            string? trangThaiThanhToan,
            string? phuongThucThanhToan,
            DateTime? tuNgay,
            DateTime? denNgay)
        {
            var query = _context.ThanhToans
                .Include(t => t.DatXe).ThenInclude(d => d!.KhachHang)
                .Include(t => t.DatXe).ThenInclude(d => d!.Xe)
                .Include(t => t.MaGiamGia)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var kw = tuKhoa.Trim();
                query = query.Where(t =>
                    (t.DatXe != null && t.DatXe.KhachHang != null && t.DatXe.KhachHang.HoTen.Contains(kw)) ||
                    (t.DatXe != null && t.DatXe.Xe != null && t.DatXe.Xe.BienSo.Contains(kw)));
            }

            if (!string.IsNullOrWhiteSpace(trangThaiThanhToan))
                query = query.Where(t => t.TrangThaiThanhToan == trangThaiThanhToan);

            if (!string.IsNullOrWhiteSpace(phuongThucThanhToan))
                query = query.Where(t => t.PhuongThucThanhToan == phuongThucThanhToan);

            if (tuNgay.HasValue)
                query = query.Where(t => t.NgayThanhToan >= tuNgay.Value);
            if (denNgay.HasValue)
                query = query.Where(t => t.NgayThanhToan <= denNgay.Value);

            var danhSach = await query.OrderByDescending(t => t.NgayThanhToan).ToListAsync();

            var bytes = _exportService.XuatDanhSachThanhToanExcel(danhSach);
            var fileName = $"DanhSachThanhToan_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            return File(bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        // =====================================================
        // XUAT PDF HOA DON
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> XuatHoaDonPdf(int id)
        {
            var thanhToan = await _context.ThanhToans
                .Include(t => t.DatXe).ThenInclude(d => d!.KhachHang)
                .Include(t => t.DatXe).ThenInclude(d => d!.Xe)
                .Include(t => t.MaGiamGia)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.MaThanhToan == id);

            if (thanhToan == null)
                return NotFound();

            var bytes = _exportService.XuatHoaDonPdf(thanhToan);
            return File(bytes, "application/pdf",
                $"HoaDon_{id}_{DateTime.Now:yyyyMMdd}.pdf");
        }

        // =====================================================
        // HELPER
        // =====================================================
        private async Task LoadThongTinThanhToanAsync(ThanhToanVM vm)
        {
            var datXe = await _context.DatXes
                .Include(d => d.KhachHang)
                .Include(d => d.Xe)
                .Include(d => d.TraXe)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.MaDatXe == vm.MaDatXe);

            if (datXe == null) return;

            var khachHang = datXe.KhachHang;

            vm.BienSo = datXe.Xe?.BienSo;
            vm.TenXe = datXe.Xe?.TenXe;
            vm.TenKhachHang = khachHang?.HoTen;
            vm.SoDienThoaiKhachHang = khachHang?.SoDienThoai;
            vm.ThoiGianNhanDuKien = datXe.ThoiGianNhanDuKien;
            vm.ThoiGianTraDuKien = datXe.ThoiGianTraDuKien;
            vm.ThoiGianTraThucTe = datXe.TraXe?.ThoiGianTraThucTe;
            vm.PhiQuaHan = datXe.TraXe?.PhiQuaHan ?? 0;
            vm.PhiVuotKm = datXe.TraXe?.PhiVuotKm ?? 0;
            vm.PhiNhienLieu = datXe.TraXe?.PhiNhienLieu ?? 0;
            vm.PhiHuHong = datXe.TraXe?.PhiHuHong ?? 0;

            // Nếu form post có mã voucher + điểm thì tính lại theo giá trị đã post
            Models.Entities.MaGiamGia? voucher = null;
            if (!string.IsNullOrWhiteSpace(vm.MaGiamGiaCode))
            {
                voucher = await _context.MaGiamGias
                    .FirstOrDefaultAsync(v => v.Code.ToUpper() == vm.MaGiamGiaCode.Trim().ToUpper());
            }

            var ketQua = _tinhTienService.TinhThanhToanDayDu(
                datXe, datXe.TraXe, voucher, Math.Max(0, vm.DiemSuDung));

            vm.SoNgayThue = ketQua.SoNgayThue;
            vm.TienThueGoc = ketQua.TienThueGoc;
            vm.TienGiamVoucher = ketQua.TienGiamVoucher;
            vm.TienTruTuDiem = ketQua.TienTruTuDiem;
            vm.TienThue = ketQua.TienThue;
            vm.TongPhuPhi = ketQua.TongPhuPhi;
            vm.TienCocDaThu = ketQua.TienCocDaThu;
            vm.TongThanhToan = ketQua.TongThanhToan;
            vm.SoTienConLai = ketQua.SoTienConLai;

            vm.DiemHienCo = khachHang?.DiemTichLuy ?? 0;
            vm.GiaTriMotDiem = HttpContext.RequestServices
                .GetRequiredService<IConfiguration>()
                .GetValue<decimal?>("LoyaltyPoints:GiaTriMotDiem") ?? 5000m;
            vm.DiemDuocCongSauThanhToan = _tinhTienService.TinhDiemDuocCong(ketQua.TienThue);

            vm.DanhSachPhuongThuc = LayDanhSachPhuongThuc();
        }

        private List<SelectListItem> LayDanhSachTrangThai()
        {
            return TrangThaiThanhToan.TatCa
                .Select(t => new SelectListItem { Value = t, Text = t })
                .ToList();
        }

        private List<SelectListItem> LayDanhSachPhuongThuc()
        {
            return PhuongThucThanhToan.TatCa
                .Select(p => new SelectListItem { Value = p, Text = p })
                .ToList();
        }
    }
}