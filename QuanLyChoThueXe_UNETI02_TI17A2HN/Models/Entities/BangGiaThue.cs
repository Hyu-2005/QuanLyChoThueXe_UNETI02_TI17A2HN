using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities
{
    [Table("BangGiaThue")]
    public class BangGiaThue
    {
        [Key]
        public int MaBangGia { get; set; }

        /// <summary>FK đến LoaiXe (nếu áp giá chung cho loại xe)</summary>
        [Display(Name = "Loại xe")]
        public int? MaLoaiXe { get; set; }

        /// <summary>FK đến Xe (nếu áp giá riêng cho xe)</summary>
        [Display(Name = "Xe")]
        public int? MaXe { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Đơn giá ngày phải > 0")]
        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Đơn giá ngày (VNĐ)")]
        public decimal DonGiaNgay { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Đơn giá giờ phải >= 0")]
        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Đơn giá giờ (VNĐ)")]
        public decimal DonGiaGio { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Tiền cọc phải >= 0")]
        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Tiền cọc mặc định (VNĐ)")]
        public decimal TienCocMacDinh { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Từ ngày")]
        public DateTime TuNgay { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Đến ngày")]
        public DateTime DenNgay { get; set; }

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true;

        // ===== Navigation =====
        [ForeignKey(nameof(MaLoaiXe))]
        public LoaiXe? LoaiXe { get; set; }

        [ForeignKey(nameof(MaXe))]
        public Xe? Xe { get; set; }

        /// <summary>
        /// Kiểm tra bảng giá có còn hiệu lực trong khoảng thời gian thuê hay không
        /// </summary>
        public bool ConHieuLuc(DateTime thoiGianNhan, DateTime thoiGianTra)
        {
            return TrangThai
                && TuNgay <= thoiGianNhan
                && DenNgay >= thoiGianTra;
        }
    }
}
