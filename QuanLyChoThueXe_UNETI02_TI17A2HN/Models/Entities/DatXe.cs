using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities
{
    [Table("DatXe")]
    public class DatXe
    {
        [Key]
        public int MaDatXe { get; set; }

        [Required]
        [Display(Name = "Khách hàng")]
        public int MaKhachHang { get; set; }

        [Required]
        [Display(Name = "Xe")]
        public int MaXe { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Thời gian nhận dự kiến")]
        public DateTime ThoiGianNhanDuKien { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Thời gian trả dự kiến")]
        public DateTime ThoiGianTraDuKien { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Địa điểm nhận")]
        public string DiaDiemNhan { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        [Display(Name = "Địa điểm trả")]
        public string DiaDiemTra { get; set; } = string.Empty;

        [Display(Name = "Ngày đặt")]
        public DateTime NgayDat { get; set; } = DateTime.Now;

        /// <summary>Đơn giá áp dụng - SNAPSHOT tại thời điểm đặt (bảo toàn lịch sử)</summary>
        [Required]
        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Đơn giá áp dụng")]
        public decimal DonGiaApDung { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Tiền cọc")]
        public decimal TienCoc { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = Constants.TrangThaiDatXe.ChoDuyet;

        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        [StringLength(500)]
        [Display(Name = "Lý do từ chối")]
        public string? LyDoTuChoi { get; set; }

        // ===== Navigation =====
        [ForeignKey(nameof(MaKhachHang))]
        public KhachHang? KhachHang { get; set; }

        [ForeignKey(nameof(MaXe))]
        public Xe? Xe { get; set; }

        public BanGiaoXe? BanGiaoXe { get; set; }
        public TraXe? TraXe { get; set; }
        public ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();

        // ===== Computed helpers =====

        /// <summary>Đơn có đang chiếm lịch xe hay không</summary>
        public bool DangChiemLich()
            => TrangThaiDatXe.ChiemLich.Contains(TrangThai);

        /// <summary>Đơn có thể hủy hay không (theo trạng thái)</summary>
        public bool CoTheHuy()
            => TrangThaiDatXe.CoTheHuy.Contains(TrangThai);
    }
}
