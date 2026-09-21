using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities
{
    [Table("ThanhToan")]
    public class ThanhToan
    {
        [Key]
        public int MaThanhToan { get; set; }

        [Required]
        [Display(Name = "Đơn đặt xe")]
        public int MaDatXe { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Tiền thuê")]
        public decimal TienThue { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Tổng phụ phí")]
        public decimal TongPhuPhi { get; set; } = 0;

        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Tiền cọc đã thu")]
        public decimal TienCocDaThu { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Tổng thanh toán")]
        public decimal TongThanhToan { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Số tiền còn lại")]
        public decimal SoTienConLai { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "Phương thức thanh toán")]
        public string PhuongThucThanhToan { get; set; } = Constants.PhuongThucThanhToan.TienMat;

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày thanh toán")]
        public DateTime NgayThanhToan { get; set; } = DateTime.Now;

        [Required]
        [StringLength(30)]
        [Display(Name = "Trạng thái thanh toán")]
        public string TrangThaiThanhToan { get; set; } = Constants.TrangThaiThanhToan.ChuaThanhToan;

        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        // ===== Navigation =====
        [ForeignKey(nameof(MaDatXe))]
        public DatXe? DatXe { get; set; }
    }
}
