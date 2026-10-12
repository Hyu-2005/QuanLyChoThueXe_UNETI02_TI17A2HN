// File: ViewModels/ThanhToan/ThanhToanOnlineVM.cs
// Nội dung: ViewModel cho thanh toán trực tuyến giả lập
// Sinh viên thực hiện: Dương Lâm Huy - 23103100120 - SV5
// Module: Module 5 - Tính tiền, Thanh toán, Lịch sử, Thống kê

using System.ComponentModel.DataAnnotations;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.ThanhToan
{
    public class ThanhToanOnlineVM
    {
        [Required]
        public int MaThanhToan { get; set; }

        public string MaGiaoDichCode { get; set; } = string.Empty;

        // Thông tin hiển thị
        public string? TenKhachHang { get; set; }
        public string? SoDienThoai { get; set; }
        public string? TenXe { get; set; }
        public string? BienSo { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0")]
        public decimal SoTien { get; set; }

        public string? NoiDung { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn cổng thanh toán")]
        [Display(Name = "Cổng thanh toán")]
        public string CongThanhToan { get; set; } = string.Empty;

        // Hiển thị QR
        public string? QRCodeBase64 { get; set; }
        public string? TrangThai { get; set; }
    }
}