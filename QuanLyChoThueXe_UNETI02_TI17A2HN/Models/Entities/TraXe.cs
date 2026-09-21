using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities
{
    [Table("TraXe")]
    public class TraXe
    {
        [Key]
        public int MaTraXe { get; set; }

        [Required]
        [Display(Name = "Đơn đặt xe")]
        public int MaDatXe { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Thời gian trả thực tế")]
        public DateTime ThoiGianTraThucTe { get; set; } = DateTime.Now;

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Số km phải >= 0")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Số km trả")]
        public decimal SoKmTra { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Mức nhiên liệu phải từ 0 đến 100%")]
        [Display(Name = "Mức nhiên liệu trả (%)")]
        public int MucNhienLieuTra { get; set; }

        [Required]
        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Tình trạng trả")]
        public string TinhTrangTra { get; set; } = string.Empty;

        // ===== Phụ phí =====
        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Phí quá hạn")]
        public decimal PhiQuaHan { get; set; } = 0;

        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Phí vượt km")]
        public decimal PhiVuotKm { get; set; } = 0;

        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Phí nhiên liệu")]
        public decimal PhiNhienLieu { get; set; } = 0;

        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Phí hư hỏng")]
        public decimal PhiHuHong { get; set; } = 0;

        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        // ===== Navigation =====
        [ForeignKey(nameof(MaDatXe))]
        public DatXe? DatXe { get; set; }

        // ===== Computed =====
        [NotMapped]
        [Display(Name = "Tổng phụ phí")]
        public decimal TongPhuPhi => PhiQuaHan + PhiVuotKm + PhiNhienLieu + PhiHuHong;
    }
}
