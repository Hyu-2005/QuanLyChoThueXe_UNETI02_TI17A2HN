using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities
{
    [Table("BanGiaoXe")]
    public class BanGiaoXe
    {
        [Key]
        public int MaBanGiao { get; set; }

        [Required]
        [Display(Name = "Đơn đặt xe")]
        public int MaDatXe { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Thời gian bàn giao")]
        public DateTime ThoiGianBanGiao { get; set; } = DateTime.Now;

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Số km phải >= 0")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Số km bàn giao")]
        public decimal SoKmBanGiao { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Mức nhiên liệu phải từ 0 đến 100%")]
        [Display(Name = "Mức nhiên liệu (%)")]
        public int MucNhienLieuBanGiao { get; set; }

        [Required]
        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Tình trạng bàn giao")]
        public string TinhTrangBanGiao { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Người bàn giao")]
        public string NguoiBanGiao { get; set; } = string.Empty;

        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        // ===== Navigation =====
        [ForeignKey(nameof(MaDatXe))]
        public DatXe? DatXe { get; set; }
    }
}
