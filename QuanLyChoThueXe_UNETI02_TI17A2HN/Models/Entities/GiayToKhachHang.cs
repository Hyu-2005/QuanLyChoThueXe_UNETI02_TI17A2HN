// ============================================================
// File: Models/Entities/GiayToKhachHang.cs
// Họ và tên: [Vũ Tiến Đạt]
// Mã sinh viên: [23103100119]
// Nội dung thực hiện: Entity giấy tờ của khách hàng (CCCD, GPLX) do khách tự tải lên
// Module: Module 3 - Khách hàng
// ============================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities
{
    [Table("GiayToKhachHang")]
    public class GiayToKhachHang
    {
        [Key]
        public int MaGiayTo { get; set; }

        [Display(Name = "Khách hàng")]
        public int MaKhachHang { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "Loại giấy tờ")]
        public string LoaiGiayTo { get; set; } = Constants.LoaiGiayTo.CCCD;

        [Required]
        [StringLength(30)]
        [Display(Name = "Số giấy tờ")]
        public string SoGiayTo { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Ngày hết hạn")]
        public DateTime? NgayHetHan { get; set; }

        // Duong dan tuong doi ben trong App_Data/GiayTo (khong nam trong wwwroot)
        [Required]
        [StringLength(300)]
        public string DuongDanMatTruoc { get; set; } = string.Empty;

        [StringLength(300)]
        public string? DuongDanMatSau { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = TrangThaiGiayTo.ChoKiemTra;

        [StringLength(300)]
        [Display(Name = "Lý do từ chối")]
        public string? LyDoTuChoi { get; set; }

        [Display(Name = "Ngày tải lên")]
        public DateTime NgayTai { get; set; } = DateTime.Now;

        [Display(Name = "Ngày kiểm tra")]
        public DateTime? NgayKiemTra { get; set; }

        [StringLength(100)]
        [Display(Name = "Người kiểm tra")]
        public string? NguoiKiemTra { get; set; }

        // ===== Navigation =====
        [ForeignKey(nameof(MaKhachHang))]
        public KhachHang? KhachHang { get; set; }
    }
}
