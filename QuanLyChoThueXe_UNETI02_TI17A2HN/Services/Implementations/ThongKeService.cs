// ThongKeService.cs - cai dat thong ke bang LINQ
// sinh vien thuc hien: Duong Lam Huy - 23103100120
// module 5 - tinh tien, thanh toan, lich su va thong ke

using Microsoft.EntityFrameworkCore;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Data;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Services.Interfaces;
using QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.ThongKe;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Services.Implementations
{
    public class ThongKeService : IThongKeService
    {
        private readonly AppDbContext _context;

        public ThongKeService(AppDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // DASHBOARD
        // =====================================================
        public async Task<DashboardVM> LayDashboardAsync()
        {
            var today = DateTime.Today;
            var ngayMai = today.AddDays(1);
            var thangNay = new DateTime(today.Year, today.Month, 1);
            var thangTruoc = thangNay.AddMonths(-1);
            var thangTruocEnd = thangNay.AddDays(-1);

            var vm = new DashboardVM
            {
                // ================= XE =================
                TongSoXe = await _context.Xes.CountAsync(),
                XeSanSang = await _context.Xes.CountAsync(x => x.TinhTrang == TinhTrangXe.SanSang),
                XeDangThue = await _context.Xes.CountAsync(x => x.TinhTrang == TinhTrangXe.DangChoThue),
                XeBaoDuong = await _context.Xes.CountAsync(x => x.TinhTrang == TinhTrangXe.BaoDuong),
                XeNgung = await _context.Xes.CountAsync(x => x.TinhTrang == TinhTrangXe.NgungHoatDong),

                // ================= DON =================
                DonChoDuyet = await _context.DatXes.CountAsync(d => d.TrangThai == TrangThaiDatXe.ChoDuyet),
                DonDaDuyet = await _context.DatXes.CountAsync(d => d.TrangThai == TrangThaiDatXe.DaDuyet),
                DonDangThue = await _context.DatXes.CountAsync(d => d.TrangThai == TrangThaiDatXe.DangThue),
                DonChoThanhToan = await _context.DatXes.CountAsync(d => d.TrangThai == TrangThaiDatXe.ChoThanhToan),
                DonHoanThanh = await _context.DatXes.CountAsync(d => d.TrangThai == TrangThaiDatXe.HoanThanh),
                DonDaHuy = await _context.DatXes.CountAsync(d => d.TrangThai == TrangThaiDatXe.DaHuy),
                DonTuChoi = await _context.DatXes.CountAsync(d => d.TrangThai == TrangThaiDatXe.TuChoi),

                // ================= TRONG NGAY =================
                LuotNhanXeHomNay = await _context.BanGiaoXes
                    .CountAsync(b => b.ThoiGianBanGiao >= today && b.ThoiGianBanGiao < ngayMai),
                LuotTraXeHomNay = await _context.TraXes
                    .CountAsync(t => t.ThoiGianTraThucTe >= today && t.ThoiGianTraThucTe < ngayMai),

                // ================= DOANH THU =================
                DoanhThuHomNay = await _context.ThanhToans
                    .Where(t => t.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    .Where(t => t.NgayThanhToan >= today && t.NgayThanhToan < ngayMai)
                    .SumAsync(t => (decimal?)t.TongThanhToan) ?? 0,

                DoanhThuThangNay = await _context.ThanhToans
                    .Where(t => t.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    .Where(t => t.NgayThanhToan >= thangNay)
                    .SumAsync(t => (decimal?)t.TongThanhToan) ?? 0,

                // ================= SO SÁNH THÁNG TRƯỚC =================
                // Bảng Xe chưa có cột NgayTao → fallback dùng tổng hiện tại
                TongSoXeThangTruoc = await _context.Xes.CountAsync(),
                // NOTE: Vì bảng Xe không có cột NgayTao, tạm thời lấy tổng hiện tại.
                // Nếu muốn chính xác, cần thêm cột NgayTao vào bảng Xe.
            };

            // Copy tạm cho các field tháng trước
            vm.XeSanSangThangTruoc = vm.XeSanSang;
            vm.XeDangThueThangTruoc = vm.XeDangThue;
            vm.XeBaoDuongThangTruoc = vm.XeBaoDuong + vm.XeNgung;

            // ============================================
            // DOANH THU 7 NGÀY GẦN NHẤT
            // ============================================
            var sevenDaysAgo = today.AddDays(-6);
            var thanhToans7Ngay = await _context.ThanhToans
                .Where(t => t.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                .Where(t => t.NgayThanhToan >= sevenDaysAgo && t.NgayThanhToan < ngayMai)
                .Select(t => new { t.NgayThanhToan })
                .ToListAsync();

            var doanhThuTheoNgay = thanhToans7Ngay
                .GroupBy(t => t.NgayThanhToan.Date)
                .ToDictionary(g => g.Key, g => g.Count());

            vm.DoanhThu7Ngay = new List<DoanhThuNgayItem>();
            for (int i = 0; i < 7; i++)
            {
                var ngay = sevenDaysAgo.AddDays(i);
                var doanhThuNgay = await _context.ThanhToans
                    .Where(t => t.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                    .Where(t => t.NgayThanhToan >= ngay && t.NgayThanhToan < ngay.AddDays(1))
                    .SumAsync(t => (decimal?)t.TongThanhToan) ?? 0;

                vm.DoanhThu7Ngay.Add(new DoanhThuNgayItem
                {
                    Ngay = ngay,
                    DoanhThu = doanhThuNgay,
                    SoDon = doanhThuTheoNgay.TryGetValue(ngay, out var count) ? count : 0
                });
            }

            // ============================================
            // PHÂN BỐ TRẠNG THÁI ĐƠN
            // ============================================
            vm.PhanBoTrangThaiDon = await _context.DatXes
                .GroupBy(d => d.TrangThai)
                .Select(g => new TrangThaiDonItem
                {
                    TrangThai = g.Key,
                    SoLuong = g.Count()
                })
                .OrderByDescending(x => x.SoLuong)
                .ToListAsync();

            // ============================================
            // TOP 5 XE ĐƯỢC THUÊ NHIỀU NHẤT (dựa vào đơn Hoàn thành)
            // ============================================
            var topXeData = await _context.DatXes
                .Where(d => d.TrangThai == TrangThaiDatXe.HoanThanh)
                .GroupBy(d => d.MaXe)
                .Select(g => new
                {
                    MaXe = g.Key,
                    SoLanThue = g.Count(),
                    DoanhThu = g
                        .SelectMany(d => d.ThanhToans)
                        .Where(t => t.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                        .Sum(t => (decimal?)t.TongThanhToan) ?? 0
                })
                .OrderByDescending(x => x.SoLanThue)
                .Take(5)
                .ToListAsync();

            var maXeList = topXeData.Select(x => x.MaXe).ToList();
            var xeInfo = await _context.Xes
                .Where(x => maXeList.Contains(x.MaXe))
                .Select(x => new { x.MaXe, x.BienSo, x.TenXe, x.AnhXe })
                .ToDictionaryAsync(x => x.MaXe);

            vm.TopXeThueNhieu = topXeData.Select(x => new TopXeItem
            {
                MaXe = x.MaXe,
                SoLanThue = x.SoLanThue,
                DoanhThu = x.DoanhThu,
                BienSo = xeInfo.TryGetValue(x.MaXe, out var info) ? info.BienSo : null,
                TenXe = xeInfo.TryGetValue(x.MaXe, out var info2) ? info2.TenXe : null,
                AnhXe = xeInfo.TryGetValue(x.MaXe, out var info3) ? info3.AnhXe : null
            }).ToList();

            // ============================================
            // HOẠT ĐỘNG GẦN ĐÂY (5 hoạt động mới nhất)
            // ============================================
            var hoatDong = new List<HoatDongItem>();

            // Đơn mới đặt (5 cái gần nhất)
            var donMoi = await _context.DatXes
                .Include(d => d.KhachHang)
                .Include(d => d.Xe)
                .OrderByDescending(d => d.NgayDat)
                .Take(5)
                .AsNoTracking()
                .ToListAsync();

            foreach (var d in donMoi)
            {
                hoatDong.Add(new HoatDongItem
                {
                    Loai = "don-moi",
                    TieuDe = "Đơn đặt xe mới",
                    MoTa = $"{d.KhachHang?.HoTen} - {d.Xe?.TenXe}",
                    ThoiGian = d.NgayDat
                });
            }

            // Thanh toán gần đây
            var thanhToanGanDay = await _context.ThanhToans
                .Include(t => t.DatXe)
                .Where(t => t.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                .OrderByDescending(t => t.NgayThanhToan)
                .Take(3)
                .AsNoTracking()
                .ToListAsync();

            foreach (var t in thanhToanGanDay)
            {
                hoatDong.Add(new HoatDongItem
                {
                    Loai = "thanh-toan",
                    TieuDe = "Thanh toán thành công",
                    MoTa = $"Đơn #{t.MaDatXe} - {t.TongThanhToan:N0}đ",
                    ThoiGian = t.NgayThanhToan
                });
            }

            // Trả xe gần đây
            var traXeGanDay = await _context.TraXes
                .Include(t => t.DatXe).ThenInclude(d => d!.KhachHang)
                .Include(t => t.DatXe).ThenInclude(d => d!.Xe)
                .OrderByDescending(t => t.ThoiGianTraThucTe)
                .Take(3)
                .AsNoTracking()
                .ToListAsync();

            foreach (var t in traXeGanDay)
            {
                hoatDong.Add(new HoatDongItem
                {
                    Loai = "tra-xe",
                    TieuDe = "Xe đã được trả",
                    MoTa = $"{t.DatXe?.KhachHang?.HoTen} - {t.DatXe?.Xe?.TenXe}",
                    ThoiGian = t.ThoiGianTraThucTe
                });
            }

            // Sắp xếp theo thời gian giảm dần, lấy 5
            vm.HoatDongGanDay = hoatDong
                .OrderByDescending(h => h.ThoiGian)
                .Take(5)
                .ToList();

            // ============================================
            // CẢNH BÁO (giữ nguyên)
            // ============================================
            var sau24h = DateTime.Now.AddHours(24);
            vm.DonSapDenGioNhan = await _context.DatXes
                .Include(d => d.KhachHang)
                .Include(d => d.Xe)
                .Where(d => d.TrangThai == TrangThaiDatXe.DaDuyet)
                .Where(d => d.ThoiGianNhanDuKien >= DateTime.Now
                         && d.ThoiGianNhanDuKien <= sau24h)
                .OrderBy(d => d.ThoiGianNhanDuKien)
                .Take(10)
                .AsNoTracking()
                .ToListAsync();

            vm.DonQuaGioTra = await _context.DatXes
                .Include(d => d.KhachHang)
                .Include(d => d.Xe)
                .Where(d => d.TrangThai == TrangThaiDatXe.DangThue)
                .Where(d => d.ThoiGianTraDuKien < DateTime.Now)
                .OrderBy(d => d.ThoiGianTraDuKien)
                .Take(10)
                .AsNoTracking()
                .ToListAsync();

            return vm;
        }

        // =====================================================
        // THONG KE CHI TIET
        // =====================================================
        public async Task<ThongKeVM> LayThongKeAsync(DateTime tuNgay, DateTime denNgay)
        {
            // Dam bao den ngay bao gom ca ngay do
            var denNgayFull = denNgay.Date.AddDays(1).AddTicks(-1);

            var vm = new ThongKeVM
            {
                TuNgay = tuNgay,
                DenNgay = denNgay
            };

            // ============================================
            // 1. Top xe duoc thue nhieu nhat
            // ============================================
            vm.TopXeThueNhieu = await _context.DatXes
                .Where(d => d.TrangThai == TrangThaiDatXe.HoanThanh)
                .Where(d => d.NgayDat >= tuNgay && d.NgayDat <= denNgayFull)
                .GroupBy(d => d.MaXe)
                .Select(g => new XeThongKeItem
                {
                    MaXe = g.Key,
                    SoLanThue = g.Count(),
                    DoanhThu = g.SelectMany(d => d.ThanhToans)
                        .Where(t => t.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                        .Sum(t => (decimal?)t.TongThanhToan) ?? 0
                })
                .OrderByDescending(x => x.SoLanThue)
                .Take(10)
                .ToListAsync();

            // Lay them thong tin bien so, ten xe
            var maXeList = vm.TopXeThueNhieu.Select(x => x.MaXe).ToList();
            var xeInfo = await _context.Xes
                .Include(x => x.LoaiXe)
                .Where(x => maXeList.Contains(x.MaXe))
                .AsNoTracking()
                .ToDictionaryAsync(x => x.MaXe);

            foreach (var item in vm.TopXeThueNhieu)
            {
                if (xeInfo.TryGetValue(item.MaXe, out var xe))
                {
                    item.BienSo = xe.BienSo;
                    item.TenXe = xe.TenXe;
                    item.TenLoaiXe = xe.LoaiXe?.TenLoaiXe;
                }
            }

            // ============================================
            // 2. Thong ke theo loai xe
            // ============================================
            vm.ThongKeTheoLoaiXe = await _context.DatXes
                .Where(d => d.TrangThai == TrangThaiDatXe.HoanThanh)
                .Where(d => d.NgayDat >= tuNgay && d.NgayDat <= denNgayFull)
                .Where(d => d.Xe != null)
                .GroupBy(d => d.Xe!.MaLoaiXe)
                .Select(g => new LoaiXeThongKeItem
                {
                    MaLoaiXe = g.Key,
                    SoLanThue = g.Count(),
                    DoanhThu = g.SelectMany(d => d.ThanhToans)
                        .Where(t => t.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                        .Sum(t => (decimal?)t.TongThanhToan) ?? 0
                })
                .OrderByDescending(x => x.SoLanThue)
                .ToListAsync();

            var loaiXeInfo = await _context.LoaiXes
                .ToDictionaryAsync(l => l.MaLoaiXe, l => l.TenLoaiXe);

            foreach (var item in vm.ThongKeTheoLoaiXe)
            {
                if (loaiXeInfo.TryGetValue(item.MaLoaiXe, out var ten))
                    item.TenLoaiXe = ten;
            }

            // ============================================
            // 3. Thong ke theo hang xe
            // ============================================
            vm.ThongKeTheoHangXe = await _context.DatXes
                .Where(d => d.TrangThai == TrangThaiDatXe.HoanThanh)
                .Where(d => d.NgayDat >= tuNgay && d.NgayDat <= denNgayFull)
                .Where(d => d.Xe != null)
                .GroupBy(d => d.Xe!.MaHangXe)
                .Select(g => new HangXeThongKeItem
                {
                    MaHangXe = g.Key,
                    SoLanThue = g.Count(),
                    DoanhThu = g.SelectMany(d => d.ThanhToans)
                        .Where(t => t.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                        .Sum(t => (decimal?)t.TongThanhToan) ?? 0
                })
                .OrderByDescending(x => x.SoLanThue)
                .ToListAsync();

            var hangXeInfo = await _context.HangXes
                .ToDictionaryAsync(h => h.MaHangXe, h => h.TenHangXe);

            foreach (var item in vm.ThongKeTheoHangXe)
            {
                if (hangXeInfo.TryGetValue(item.MaHangXe, out var ten))
                    item.TenHangXe = ten;
            }

            // ============================================
            // 4. Doanh thu theo thang
            // ============================================
            vm.DoanhThuTheoThang = await _context.ThanhToans
                .Where(t => t.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                .Where(t => t.NgayThanhToan >= tuNgay && t.NgayThanhToan <= denNgayFull)
                .GroupBy(t => new { t.NgayThanhToan.Year, t.NgayThanhToan.Month })
                .Select(g => new DoanhThuThangItem
                {
                    Nam = g.Key.Year,
                    Thang = g.Key.Month,
                    SoDon = g.Count(),
                    DoanhThu = g.Sum(t => t.TongThanhToan)
                })
                .OrderBy(x => x.Nam)
                .ThenBy(x => x.Thang)
                .ToListAsync();

            // ============================================
            // 5. Top khach hang
            // ============================================
            vm.TopKhachHang = await _context.DatXes
                .Where(d => d.TrangThai == TrangThaiDatXe.HoanThanh)
                .Where(d => d.NgayDat >= tuNgay && d.NgayDat <= denNgayFull)
                .GroupBy(d => d.MaKhachHang)
                .Select(g => new KhachHangThongKeItem
                {
                    MaKhachHang = g.Key,
                    SoLanThue = g.Count(),
                    TongChiTieu = g.SelectMany(d => d.ThanhToans)
                        .Where(t => t.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                        .Sum(t => (decimal?)t.TongThanhToan) ?? 0
                })
                .OrderByDescending(x => x.SoLanThue)
                .Take(10)
                .ToListAsync();

            var khInfo = await _context.KhachHangs
                .Where(k => vm.TopKhachHang.Select(x => x.MaKhachHang).Contains(k.MaKhachHang))
                .ToDictionaryAsync(k => k.MaKhachHang, k => new { k.HoTen, k.SoDienThoai });

            foreach (var item in vm.TopKhachHang)
            {
                if (khInfo.TryGetValue(item.MaKhachHang, out var kh))
                {
                    item.HoTen = kh.HoTen;
                    item.SoDienThoai = kh.SoDienThoai;
                }
            }

            // ============================================
            // 6. Phu phi theo loai
            // ============================================
            var traXes = await _context.TraXes
                .Where(t => t.ThoiGianTraThucTe >= tuNgay && t.ThoiGianTraThucTe <= denNgayFull)
                .AsNoTracking()
                .ToListAsync();

            vm.PhuPhiTheoLoai = new List<PhuPhiThongKeItem>
            {
                new() { LoaiPhi = "Phi qua han",      TongTien = traXes.Sum(t => t.PhiQuaHan),      SoLan = traXes.Count(t => t.PhiQuaHan > 0) },
                new() { LoaiPhi = "Phi vuot km",       TongTien = traXes.Sum(t => t.PhiVuotKm),      SoLan = traXes.Count(t => t.PhiVuotKm > 0) },
                new() { LoaiPhi = "Phi nhien lieu",    TongTien = traXes.Sum(t => t.PhiNhienLieu),   SoLan = traXes.Count(t => t.PhiNhienLieu > 0) },
                new() { LoaiPhi = "Phi hu hong",       TongTien = traXes.Sum(t => t.PhiHuHong),      SoLan = traXes.Count(t => t.PhiHuHong > 0) }
            };

            // ============================================
            // 7. Tong hop
            // ============================================
            var allDon = await _context.DatXes
                .Where(d => d.NgayDat >= tuNgay && d.NgayDat <= denNgayFull)
                .AsNoTracking()
                .ToListAsync();

            vm.TongSoDon = allDon.Count;
            vm.SoDonHuy = allDon.Count(d => d.TrangThai == TrangThaiDatXe.DaHuy);
            vm.SoDonTuChoi = allDon.Count(d => d.TrangThai == TrangThaiDatXe.TuChoi);
            vm.TyLeHuy = vm.TongSoDon > 0
                ? Math.Round((double)(vm.SoDonHuy + vm.SoDonTuChoi) / vm.TongSoDon * 100, 2)
                : 0;

            vm.TongDoanhThu = await _context.ThanhToans
                .Where(t => t.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan)
                .Where(t => t.NgayThanhToan >= tuNgay && t.NgayThanhToan <= denNgayFull)
                .SumAsync(t => (decimal?)t.TongThanhToan) ?? 0;

            vm.TongPhuPhi = traXes.Sum(t => t.TongPhuPhi);

            // Thoi luong thue trung binh
            var donHoanThanh = allDon
                .Where(d => d.TrangThai == TrangThaiDatXe.HoanThanh)
                .ToList();

            if (donHoanThanh.Any())
            {
                vm.ThoiLuongThueTrungBinh = Math.Round(
                    donHoanThanh.Average(d => (d.ThoiGianTraDuKien - d.ThoiGianNhanDuKien).TotalDays), 2);
            }

            // Ty le khai thac xe = so ngay xe duoc thue / tong so ngay trong ky
            var tongSoXe = await _context.Xes.CountAsync();
            var soNgayTrongKy = Math.Max(1, (denNgay - tuNgay).Days);
            var tongNgayThue = donHoanThanh
                .Sum(d => (d.ThoiGianTraDuKien - d.ThoiGianNhanDuKien).TotalDays);

            vm.TyLeKhaiThac = tongSoXe > 0 && soNgayTrongKy > 0
                ? Math.Round(tongNgayThue / (tongSoXe * soNgayTrongKy) * 100, 2)
                : 0;

            return vm;
        }
    }
}