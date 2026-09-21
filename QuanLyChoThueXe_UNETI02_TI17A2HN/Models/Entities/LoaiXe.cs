using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities
{
    [Table("LoaiXe")]
    public class LoaiXe
    {
        [Key]
        public int MaLoaiXe { get; set; }

        [Required(ErrorMessage = "Tên loại xe không được để trống")]
        [StringLength(50)]
        [Display(Name = "Tên loại xe")]
        public string TenLoaiXe { get; set; } = string.Empty;

        [Required]
        [Range(1, 50, ErrorMessage = "Số chỗ phải từ 1 đến 50")]
        [Display(Name = "Số chỗ")]
        public int SoCho { get; set; }

        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true;

        // ===== Navigation =====
        public ICollection<Xe> Xes { get; set; } = new List<Xe>();
        public ICollection<BangGiaThue> BangGiaThues { get; set; } = new List<BangGiaThue>();
    }
}
