// ==============================================================
// Bắt Đầu Code của SV4: Nguyễn Hoàng Đức Hiếu
// file: Models/Entities/LichSuBaoDuong.cs
// nội dung: STT 11 - Thực thể Lưu trữ Lịch sử và Quy trình theo dõi Bảo dưỡng xe nâng cao
// sinh viên thực hiện : Nguyễn Hoàng Đức Hiếu -  23103100116 - SV4 
// ==============================================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities
{
    [Table("LichSuBaoDuong")]
    public class LichSuBaoDuong
    {
        [Key]
        public int MaBaoDuong { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn xe cần bảo dưỡng")]
        [Display(Name = "Phương tiện")]
        public int MaXe { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày bắt đầu")]
        public DateTime NgayBatDau { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày hoàn thành")]
        public DateTime? NgayHoanThanh { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phân loại bảo dưỡng")]
        [StringLength(100)]
        [Display(Name = "Loại bảo dưỡng")]
        public string LoaiBaoDuong { get; set; } = Constants.LoaiBaoDuong.DinhKy;

        [Range(0, double.MaxValue, ErrorMessage = "Số km phải >= 0")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Số km khi vào bảo dưỡng")]
        public decimal SoKmKhiBaoDuong { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Chi phí phải >= 0")]
        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Chi phí bảo dưỡng (VNĐ)")]
        public decimal ChiPhi { get; set; } = 0;

        [StringLength(200)]
        [Display(Name = "Gara / Trung tâm bảo dưỡng")]
        public string? GaraBaoDuong { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung bảo dưỡng")]
        [StringLength(1000)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Nội dung hạng mục bảo dưỡng")]
        public string NoiDungBaoDuong { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Người phụ trách")]
        public string? NguoiPhuTrach { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = TrangThaiBaoDuong.DangBaoDuong;

        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        // ===== Navigation =====
        [ForeignKey(nameof(MaXe))]
        public Xe? Xe { get; set; }
    }
}
// ==============================================================
// Kết thúc Code của SV4
// ==============================================================
