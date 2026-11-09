// ============================================================
// File: ViewModels/DatXe/DatXeCreateVM.cs
// ============================================================

using System.ComponentModel.DataAnnotations;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.DatXe
{
    public class DatXeCreateVM
    {
        public int MaXe { get; set; }

        [Required(ErrorMessage = "Vui long chon thoi gian nhan")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Thoi gian nhan")]
        public DateTime ThoiGianNhanDuKien { get; set; }

        [Required(ErrorMessage = "Vui long chon thoi gian tra")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Thoi gian tra")]
        public DateTime ThoiGianTraDuKien { get; set; }

        [Required(ErrorMessage = "Dia diem nhan khong duoc de trong")]
        [StringLength(200, ErrorMessage = "Dia diem nhan toi da 200 ky tu")]
        [Display(Name = "Dia diem nhan")]
        public string DiaDiemNhan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Dia diem tra khong duoc de trong")]
        [StringLength(200, ErrorMessage = "Dia diem tra toi da 200 ky tu")]
        [Display(Name = "Dia diem tra")]
        public string DiaDiemTra { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Ghi chu toi da 500 ky tu")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Ghi chu")]
        public string? GhiChu { get; set; }

        // ĐÂY LÀ THUỘC TÍNH BỊ THIẾU GÂY RA LỖI
        [StringLength(30, ErrorMessage = "Mã giảm giá tối đa 30 ký tự")]
        [Display(Name = "Mã giảm giá (nếu có)")]
        public string? MaGiamGiaCode { get; set; }

        // ===== Du lieu hien thi (khong bind tu form) =====
        public string? BienSo { get; set; }
        public string? TenXe { get; set; }
        public string? TenLoaiXe { get; set; }
        public string? TenHangXe { get; set; }
        public int SoCho { get; set; }
        public string? AnhXe { get; set; }

        public decimal DonGiaNgay { get; set; }
        public decimal DonGiaGio { get; set; }
        public decimal TienCoc { get; set; }

        public int SoNgayThue { get; set; }
        public decimal TienThueDuKien { get; set; }

        public decimal TienGiamDuKien { get; set; } = 0;
    }
}