// File: ViewModels/DatXe/DatXeFormVM.cs
// Noi dung: ViewModel form dat xe (khach hang dat sau khi da tim xe trong)
// Module: Module 3 - Khach hang tim xe trong, dat xe, kiem tra trung lich

using System.ComponentModel.DataAnnotations;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.DatXe
{
    public class DatXeFormVM
    {
        [Required]
        [Display(Name = "Xe")]
        public int MaXe { get; set; }

        // ===== Thong tin xe - chi hien thi, khong cho sua =====
        public string BienSo { get; set; } = string.Empty;
        public string TenXe { get; set; } = string.Empty;
        public string TenLoaiXe { get; set; } = string.Empty;
        public string TenHangXe { get; set; } = string.Empty;
        public string? AnhXe { get; set; }

        [Required(ErrorMessage = "Thoi gian nhan khong duoc de trong")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Thoi gian nhan du kien")]
        public DateTime ThoiGianNhanDuKien { get; set; }

        [Required(ErrorMessage = "Thoi gian tra khong duoc de trong")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Thoi gian tra du kien")]
        public DateTime ThoiGianTraDuKien { get; set; }

        [Required(ErrorMessage = "Dia diem nhan khong duoc de trong")]
        [StringLength(200, ErrorMessage = "Dia diem nhan toi da 200 ky tu")]
        [Display(Name = "Dia diem nhan")]
        public string DiaDiemNhan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Dia diem tra khong duoc de trong")]
        [StringLength(200, ErrorMessage = "Dia diem tra toi da 200 ky tu")]
        [Display(Name = "Dia diem tra")]
        public string DiaDiemTra { get; set; } = string.Empty;

        // ===== Gia - SNAPSHOT lay tu BangGiaThue hieu luc tai thoi diem tim kiem =====
        // Khach hang khong duoc sua, chi hien thi lai de xac nhan truoc khi dat
        [Display(Name = "Don gia ap dung (VND/ngay)")]
        public decimal DonGiaApDung { get; set; }

        [Display(Name = "Tien coc (VND)")]
        public decimal TienCoc { get; set; }

        [Display(Name = "Tong tien du kien (VND)")]
        public decimal TongTienDuKien { get; set; }

        [StringLength(500, ErrorMessage = "Ghi chu toi da 500 ky tu")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Ghi chu")]
        public string? GhiChu { get; set; }
    }
}