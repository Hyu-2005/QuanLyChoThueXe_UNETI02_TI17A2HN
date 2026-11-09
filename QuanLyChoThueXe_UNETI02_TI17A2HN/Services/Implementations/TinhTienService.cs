// ============================================================
// File: Services/Implementations/TinhTienService.cs
// Nội dung: Cài đặt tính tiền thuê + voucher + điểm thưởng
// Sinh viên thực hiện: Dương Lâm Huy - 23103100120 - SV5
// Module: Module 5 - Tính tiền, Thanh toán, Lịch sử, Thống kê
// ============================================================

using Microsoft.Extensions.Configuration;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Services.Interfaces;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Services.Implementations
{
    public class TinhTienService : ITinhTienService
    {
        private readonly IConfiguration _config;

        public TinhTienService(IConfiguration config)
        {
            _config = config;
        }

        // =====================================================
        // 1. TÍNH SỐ NGÀY THUÊ
        // =====================================================
        public int TinhSoNgayThue(DateTime thoiGianNhan, DateTime thoiGianTra)
        {
            if (thoiGianTra <= thoiGianNhan) return 1;
            var soNgay = (int)Math.Ceiling((thoiGianTra - thoiGianNhan).TotalDays);
            return Math.Max(1, soNgay);
        }

        // =====================================================
        // 2. TÍNH TIỀN THUÊ (CHƯA GIẢM GIÁ)
        // =====================================================
        public decimal TinhTienThue(DatXe datXe)
        {
            var soNgay = TinhSoNgayThue(datXe.ThoiGianNhanDuKien, datXe.ThoiGianTraDuKien);
            return soNgay * datXe.DonGiaApDung;
        }

        // =====================================================
        // 3. TÍNH TOÁN CƠ BẢN (BẢN CŨ - KHÔNG VOUCHER/ĐIỂM)
        // =====================================================
        public ThanhToanResult TinhThanhToan(DatXe datXe, TraXe? traXe)
        {
            var soNgay = TinhSoNgayThue(datXe.ThoiGianNhanDuKien, datXe.ThoiGianTraDuKien);
            var tienThue = soNgay * datXe.DonGiaApDung;
            var tongPhuPhi = traXe?.TongPhuPhi ?? 0;
            var tongThanhToan = tienThue + tongPhuPhi;
            var soTienConLai = tongThanhToan - datXe.TienCoc;

            return new ThanhToanResult
            {
                SoNgayThue = soNgay,
                TienThueGoc = tienThue,
                TienGiamVoucher = 0,
                TienTruTuDiem = 0,
                DiemDaDung = 0,
                TienThue = tienThue,
                TongPhuPhi = tongPhuPhi,
                TienCocDaThu = datXe.TienCoc,
                TongThanhToan = tongThanhToan,
                SoTienConLai = soTienConLai < 0 ? 0 : soTienConLai
            };
        }

        // =====================================================
        // 4. TÍNH TOÁN ĐẦY ĐỦ (VOUCHER + ĐIỂM + PHỤ PHÍ + CỌC)
        // =====================================================
        public ThanhToanResult TinhThanhToanDayDu(
            DatXe datXe,
            TraXe? traXe,
            MaGiamGia? voucher,
            int diemSuDung)
        {
            var soNgay = TinhSoNgayThue(datXe.ThoiGianNhanDuKien, datXe.ThoiGianTraDuKien);
            var tienThueGoc = soNgay * datXe.DonGiaApDung;

            // ----- Bước 1: Trừ voucher -----
            decimal tienGiamVoucher = 0;
            if (voucher != null)
            {
                tienGiamVoucher = voucher.TinhTienGiam(tienThueGoc);
            }
            var tienThueSauVoucher = tienThueGoc - tienGiamVoucher;
            if (tienThueSauVoucher < 0) tienThueSauVoucher = 0;

            // ----- Bước 2: Trừ điểm -----
            decimal tienTruTuDiem = 0;
            int diemThucDung = 0;
            if (diemSuDung > 0 && tienThueSauVoucher > 0)
            {
                var giaTriMotDiem = GetGiaTriMotDiem();
                if (giaTriMotDiem > 0)
                {
                    // Giới hạn 1: tối đa theo tỷ lệ % hóa đơn
                    var toiDaTheoTyLe = tienThueSauVoucher * GetTyLeToiDaDungDiem();

                    // Giới hạn 2: tối đa theo config DiemToiDaMoiHoaDon
                    var toiDaTheoConfig = GetDiemToiDaMoiHoaDon() * giaTriMotDiem;

                    // Giới hạn 3: tối đa theo giá trị điểm user muốn dùng
                    var toiDaTheoDiem = diemSuDung * giaTriMotDiem;

                    tienTruTuDiem = Math.Min(toiDaTheoDiem, Math.Min(toiDaTheoTyLe, toiDaTheoConfig));
                    tienTruTuDiem = Math.Min(tienTruTuDiem, tienThueSauVoucher);

                    diemThucDung = (int)Math.Floor(tienTruTuDiem / giaTriMotDiem);
                    tienTruTuDiem = diemThucDung * giaTriMotDiem;
                }
            }

            var tienThueSauGiam = tienThueSauVoucher - tienTruTuDiem;
            if (tienThueSauGiam < 0) tienThueSauGiam = 0;

            // ----- Bước 3: Cộng phụ phí -----
            var tongPhuPhi = traXe?.TongPhuPhi ?? 0;
            var tongThanhToan = tienThueSauGiam + tongPhuPhi;

            // ----- Bước 4: Trừ cọc -----
            var soTienConLai = tongThanhToan - datXe.TienCoc;
            if (soTienConLai < 0) soTienConLai = 0;

            return new ThanhToanResult
            {
                SoNgayThue = soNgay,
                TienThueGoc = tienThueGoc,
                TienGiamVoucher = tienGiamVoucher,
                TienTruTuDiem = tienTruTuDiem,
                DiemDaDung = diemThucDung,
                TienThue = tienThueSauGiam,
                TongPhuPhi = tongPhuPhi,
                TienCocDaThu = datXe.TienCoc,
                TongThanhToan = tongThanhToan,
                SoTienConLai = soTienConLai
            };
        }

        // =====================================================
        // 5. TÍNH ĐIỂM ĐƯỢC CỘNG SAU KHI THANH TOÁN
        // =====================================================
        public int TinhDiemDuocCong(decimal tienThueSauGiamGia)
        {
            if (tienThueSauGiamGia <= 0) return 0;
            var soTienDeTichMotDiem = GetSoTienDeTichMotDiem();
            if (soTienDeTichMotDiem <= 0) return 0;
            return (int)Math.Floor(tienThueSauGiamGia / soTienDeTichMotDiem);
        }

        // =====================================================
        // 6. KIỂM TRA ĐIỂM SỬ DỤNG CÓ HỢP LỆ KHÔNG
        // =====================================================
        public (int hopLe, string? loi) KiemTraDiemSuDung(
            int diemSuDung, int diemHienCo, decimal tienThueGoc)
        {
            if (diemSuDung <= 0)
                return (0, null); // Không dùng điểm, hợp lệ

            if (diemSuDung > diemHienCo)
                return (0, $"Bạn chỉ có {diemHienCo:N0} điểm");

            if (diemSuDung < QuyDinhDiemThuong.DiemToiThieuDeSuDung)
                return (0, $"Cần tối thiểu {QuyDinhDiemThuong.DiemToiThieuDeSuDung} điểm để sử dụng");

            var giaTriMotDiem = GetGiaTriMotDiem();
            if (giaTriMotDiem <= 0)
                return (0, "Cấu hình giá trị điểm không hợp lệ");

            var toiDaTheoTyLe = tienThueGoc * GetTyLeToiDaDungDiem();
            var toiDaTheoConfig = GetDiemToiDaMoiHoaDon() * giaTriMotDiem;
            var toiDaTien = Math.Min(toiDaTheoTyLe, toiDaTheoConfig);
            var toiDaDiem = (int)Math.Floor(toiDaTien / giaTriMotDiem);

            if (diemSuDung > toiDaDiem)
                return (toiDaDiem, $"Chỉ được dùng tối đa {toiDaDiem:N0} điểm cho hóa đơn này");

            return (diemSuDung, null);
        }

        // =====================================================
        // HELPERS ĐỌC CONFIG
        // =====================================================
        private decimal GetGiaTriMotDiem()
            => _config.GetValue<decimal?>("LoyaltyPoints:GiaTriMotDiem")
               ?? QuyDinhDiemThuong.FallbackGiaTriMotDiem;

        private decimal GetSoTienDeTichMotDiem()
            => _config.GetValue<decimal?>("LoyaltyPoints:SoTienDeTichMotDiem")
               ?? QuyDinhDiemThuong.FallbackSoTienDeTichMotDiem;

        private decimal GetTyLeToiDaDungDiem()
            => _config.GetValue<decimal?>("LoyaltyPoints:TyLeToiDaDungDiem")
               ?? QuyDinhDiemThuong.FallbackTyLeToiDaDungDiem;

        private int GetDiemToiDaMoiHoaDon()
            => _config.GetValue<int?>("LoyaltyPoints:DiemToiDaMoiHoaDon")
               ?? QuyDinhDiemThuong.FallbackDiemToiDaMoiHoaDon;
    }
}