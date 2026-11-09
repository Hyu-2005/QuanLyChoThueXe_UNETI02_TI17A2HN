using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities
{
    [Table("LichSuDiChuyen")]
    public class LichSuDiChuyen
    {
        [Key]
        public int MaDiChuyen { get; set; }

        [Required]
        public int MaDatXe { get; set; }

        [Required]
        public double ViDo { get; set; }

        [Required]
        public double KinhDo { get; set; }

        public DateTime ThoiGian { get; set; } = DateTime.Now;

        [ForeignKey(nameof(MaDatXe))]
        public DatXe? DatXe { get; set; }
    }
}