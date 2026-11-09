// ============================================================
// File: ViewModels/ThanhToan/ApDungVoucherVM.cs
// Nội dung: ViewModel cho AJAX kiểm tra voucher ở màn hình thanh toán
// Sinh viên thực hiện: Dương Lâm Huy - 23103100120 - SV5
// Module: Module 5 - Tính tiền, Thanh toán
// ============================================================

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.ThanhToan
{
    /// <summary>
    /// Request AJAX khi khách bấm "Áp dụng mã"
    /// </summary>
    public class ApDungVoucherRequestVM
    {
        public int MaDatXe { get; set; }
        public string? Code { get; set; }
        public int DiemSuDung { get; set; } = 0;
    }

    /// <summary>
    /// Response trả về cho AJAX
    /// </summary>
    public class ApDungVoucherResponseVM
    {
        public bool ThanhCong { get; set; }
        public string? Loi { get; set; }

        // Thông tin voucher
        public string? Code { get; set; }
        public int? MaGiamGiaId { get; set; }
        public string? TenChuongTrinh { get; set; }
        public decimal TienGiamVoucher { get; set; }

        // Thông tin điểm
        public decimal TienTruTuDiem { get; set; }
        public int DiemDaDung { get; set; }
        public int DiemHienCo { get; set; }
        public decimal GiaTriMotDiem { get; set; }

        // Tổng kết sau khi áp dụng
        public decimal TienThueGoc { get; set; }
        public decimal TienThueSauGiam { get; set; }
        public decimal TongPhuPhi { get; set; }
        public decimal TongThanhToan { get; set; }
        public decimal TienCocDaThu { get; set; }
        public decimal SoTienConLai { get; set; }
    }
}