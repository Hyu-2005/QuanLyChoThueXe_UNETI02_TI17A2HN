// ============================================================
// File: Services/Interfaces/ITinhTienService.cs
// Nội dung: Interface tính tiền + điểm thưởng + voucher
// Sinh viên thực hiện: Dương Lâm Huy - 23103100120 - SV5
// Module: Module 5 - Tính tiền, Thanh toán, Lịch sử, Thống kê
// ============================================================

using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Services.Interfaces
{
    public interface ITinhTienService
    {
        /// <summary>Tính số ngày thuê (làm tròn lên, tối thiểu 1)</summary>
        int TinhSoNgayThue(DateTime thoiGianNhan, DateTime thoiGianTra);

        /// <summary>Tính tiền thuê = SốNgày × ĐơnGiá</summary>
        decimal TinhTienThue(DatXe datXe);

        /// <summary>
        /// Tính toán đầy đủ (bản cũ - không voucher, không điểm)
        /// Giữ lại để tương thích với các chỗ đang dùng
        /// </summary>
        ThanhToanResult TinhThanhToan(DatXe datXe, TraXe? traXe);

        /// <summary>
        /// Tính toán đầy đủ: voucher → điểm → phụ phí → cọc
        /// </summary>
        ThanhToanResult TinhThanhToanDayDu(
            DatXe datXe,
            TraXe? traXe,
            MaGiamGia? voucher,
            int diemSuDung);

        /// <summary>Tính số điểm được cộng sau khi thanh toán thành công</summary>
        int TinhDiemDuocCong(decimal tienThueSauGiamGia);

        /// <summary>Kiểm tra số điểm có hợp lệ để sử dụng không</summary>
        (int hopLe, string? loi) KiemTraDiemSuDung(int diemSuDung, int diemHienCo, decimal tienThueGoc);
    }

    public class ThanhToanResult
    {
        public int SoNgayThue { get; set; }

        /// <summary>Tiền thuê gốc (trước voucher và điểm)</summary>
        public decimal TienThueGoc { get; set; }

        /// <summary>Tiền được giảm từ voucher</summary>
        public decimal TienGiamVoucher { get; set; }

        /// <summary>Tiền được trừ từ điểm</summary>
        public decimal TienTruTuDiem { get; set; }

        /// <summary>Số điểm thực sự đã dùng (có thể ít hơn số điểm user nhập)</summary>
        public int DiemDaDung { get; set; }

        /// <summary>Tiền thuê sau khi trừ voucher + điểm</summary>
        public decimal TienThue { get; set; }

        public decimal TongPhuPhi { get; set; }
        public decimal TienCocDaThu { get; set; }
        public decimal TongThanhToan { get; set; }
        public decimal SoTienConLai { get; set; }
    }
}