// ============================================================
// File: Models/Entities/MaGiamGia.cs
// Nội dung: Entity lưu trữ mã giảm giá (Voucher)
// Sinh viên thực hiện: Dương Lâm Huy - 23103100120 - SV5
// Module: Module 5 - Tính tiền, Thanh toán, Lịch sử, Thống kê
// ============================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities
{
    [Table("MaGiamGia")]
    public class MaGiamGia
    {
        [Key]
        public int MaGiamGiaId { get; set; }

        [Required(ErrorMessage = "Mã giảm giá không được để trống")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Mã phải từ 3 đến 30 ký tự")]
        [Display(Name = "Mã giảm giá")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên chương trình không được để trống")]
        [StringLength(200)]
        [Display(Name = "Tên chương trình")]
        public string TenChuongTrinh { get; set; } = string.Empty;

        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Loại giảm giá")]
        public string LoaiGiamGia { get; set; } = Constants.LoaiGiamGia.PhanTram;

        /// <summary>
        /// Nếu LoaiGiamGia = PhanTram: đây là % (1-100)
        /// Nếu LoaiGiamGia = SoTienCoDinh: đây là số tiền VNĐ
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,0)")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá trị giảm phải >= 0")]
        [Display(Name = "Giá trị giảm")]
        public decimal GiaTriGiam { get; set; }

        /// <summary>Chỉ áp dụng cho PhanTram. VD: giảm 10% tối đa 500.000đ</summary>
        [Column(TypeName = "decimal(18,0)")]
        [Range(0, double.MaxValue)]
        [Display(Name = "Giảm tối đa (VNĐ)")]
        public decimal? GiamToiDa { get; set; }

        /// <summary>Đơn hàng tối thiểu để áp dụng voucher này</summary>
        [Column(TypeName = "decimal(18,0)")]
        [Range(0, double.MaxValue)]
        [Display(Name = "Đơn hàng tối thiểu (VNĐ)")]
        public decimal DonHangToiThieu { get; set; } = 0;

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày bắt đầu")]
        public DateTime NgayBatDau { get; set; } = DateTime.Now;

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày kết thúc")]
        public DateTime NgayKetThuc { get; set; } = DateTime.Now.AddMonths(1);

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượt dùng phải >= 1")]
        [Display(Name = "Số lượt dùng tối đa")]
        public int SoLuotDungToiDa { get; set; } = 100;

        [Display(Name = "Số lượt đã dùng")]
        public int SoLuotDaDung { get; set; } = 0;

        [Display(Name = "Mỗi khách chỉ dùng 1 lần")]
        public bool MoiKhachMotLan { get; set; } = true;

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true;

        // ===== Navigation =====
        public ICollection<SuDungMaGiamGia> SuDungMaGiamGias { get; set; } = new List<SuDungMaGiamGia>();

        // ===== Computed helpers =====
        [NotMapped]
        public bool ConHieuLuc => TrangThai
            && NgayBatDau <= DateTime.Now
            && NgayKetThuc >= DateTime.Now
            && SoLuotDaDung < SoLuotDungToiDa;

        [NotMapped]
        public int SoLuotConLai => Math.Max(0, SoLuotDungToiDa - SoLuotDaDung);

        /// <summary>Tính số tiền giảm thực tế cho đơn hàng</summary>
        public decimal TinhTienGiam(decimal tienThueGoc)
        {
            if (tienThueGoc < DonHangToiThieu) return 0;

            if (LoaiGiamGia == Constants.LoaiGiamGia.SoTienCoDinh)
                return Math.Min(GiaTriGiam, tienThueGoc);

            // PhanTram
            var giam = tienThueGoc * GiaTriGiam / 100m;
            if (GiamToiDa.HasValue && giam > GiamToiDa.Value)
                giam = GiamToiDa.Value;
            return Math.Min(giam, tienThueGoc);
        }
    }
}