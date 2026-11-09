// ============================================================
// File: Models/Entities/SuDungMaGiamGia.cs
// Nội dung: Lịch sử sử dụng mã giảm giá (audit + kiểm soát mỗi khách 1 lần)
// Sinh viên thực hiện: Dương Lâm Huy - 23103100120 - SV5
// Module: Module 5 - Tính tiền, Thanh toán, Lịch sử, Thống kê
// ============================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities
{
    [Table("SuDungMaGiamGia")]
    public class SuDungMaGiamGia
    {
        [Key]
        public int MaSuDung { get; set; }

        [Required]
        public int MaGiamGiaId { get; set; }

        [Required]
        public int MaKhachHang { get; set; }

        [Required]
        public int MaDatXe { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        public decimal SoTienDuocGiam { get; set; }

        public DateTime NgaySuDung { get; set; } = DateTime.Now;

        // ===== Navigation =====
        [ForeignKey(nameof(MaGiamGiaId))]
        public MaGiamGia? MaGiamGia { get; set; }

        [ForeignKey(nameof(MaKhachHang))]
        public KhachHang? KhachHang { get; set; }

        [ForeignKey(nameof(MaDatXe))]
        public DatXe? DatXe { get; set; }
    }
}