using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities
{
    [Table("HangXe")]
    public class HangXe
    {
        [Key]
        public int MaHangXe { get; set; }

        [Required(ErrorMessage = "Tên hãng xe không được để trống")]
        [StringLength(50)]
        [Display(Name = "Tên hãng xe")]
        public string TenHangXe { get; set; } = string.Empty;

        [StringLength(50)]
        [Display(Name = "Quốc gia")]
        public string? QuocGia { get; set; }

        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true;

        // ===== Navigation =====
        public ICollection<Xe> Xes { get; set; } = new List<Xe>();
    }
}
