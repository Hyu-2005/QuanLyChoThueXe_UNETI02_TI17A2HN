// File: Models/Entities/GiaoDichOnline.cs
// Nội dung: Entity giao dịch thanh toán trực tuyến giả lập
// Sinh viên thực hiện: Dương Lâm Huy - 23103100120 - SV5
// Module: Module 5 - Tính tiền, Thanh toán, Lịch sử, Thống kê

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities
{
    [Table("GiaoDichOnline")]
    public class GiaoDichOnline
    {
        [Key]
        public int MaGiaoDich { get; set; }

        [Required]
        [Display(Name = "Hóa đơn")]
        public int MaThanhToan { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Mã giao dịch")]
        public string MaGiaoDichCode { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        [Display(Name = "Cổng thanh toán")]
        public string CongThanhToan { get; set; } = Constants.CongThanhToan.VNPay;

        [Required]
        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Số tiền")]
        public decimal SoTien { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = TrangThaiGiaoDich.ChoThanhToan;

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        [Display(Name = "Ngày thanh toán")]
        public DateTime? NgayThanhToan { get; set; }

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        // ===== Navigation =====
        [ForeignKey(nameof(MaThanhToan))]
        public ThanhToan? ThanhToan { get; set; }
    }
}