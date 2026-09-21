using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities
{
    [Table("Xe")]
    public class Xe
    {
        [Key]
        public int MaXe { get; set; }

        [Required(ErrorMessage = "Biển số xe không được để trống")]
        [StringLength(20)]
        [Display(Name = "Biển số xe")]
        public string BienSo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên xe không được để trống")]
        [StringLength(100)]
        [Display(Name = "Tên xe")]
        public string TenXe { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Loại xe")]
        public int MaLoaiXe { get; set; }

        [Required]
        [Display(Name = "Hãng xe")]
        public int MaHangXe { get; set; }

        [Required]
        [Range(1990, 2100, ErrorMessage = "Năm sản xuất không hợp lệ")]
        [Display(Name = "Năm sản xuất")]
        public int NamSanXuat { get; set; }

        [StringLength(30)]
        [Display(Name = "Màu sắc")]
        public string? MauSac { get; set; }

        [Required]
        [Range(1, 50, ErrorMessage = "Số chỗ phải từ 1 đến 50")]
        [Display(Name = "Số chỗ")]
        public int SoCho { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Số km phải >= 0")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Số km hiện tại")]
        public decimal SoKmHienTai { get; set; } = 0;

        [Required]
        [StringLength(30)]
        [Display(Name = "Tình trạng")]
        public string TinhTrang { get; set; } = Constants.TinhTrangXe.SanSang;

        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [StringLength(255)]
        [Display(Name = "Ảnh xe")]
        public string? AnhXe { get; set; }

        // ===== Navigation =====
        [ForeignKey(nameof(MaLoaiXe))]
        public LoaiXe? LoaiXe { get; set; }

        [ForeignKey(nameof(MaHangXe))]
        public HangXe? HangXe { get; set; }

        public ICollection<BangGiaThue> BangGiaThues { get; set; } = new List<BangGiaThue>();
        public ICollection<DatXe> DatXes { get; set; } = new List<DatXe>();
    }
}
