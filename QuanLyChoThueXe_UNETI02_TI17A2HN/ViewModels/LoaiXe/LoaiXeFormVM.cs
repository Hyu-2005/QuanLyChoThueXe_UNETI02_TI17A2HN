// File: ViewModels/LoaiXe/LoaiXeFormVM.cs
// Noi dung: ViewModel cho form Them/Sua LoaiXe
// Sinh vien thuc hien: Pham Gia Minh Hoang - 23103100087 - SV1
// Module: Module 1 - Tai khoan, Dang nhap, Phan quyen, Loai xe, Hang xe

using System.ComponentModel.DataAnnotations;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.LoaiXe
{
    public class LoaiXeFormVM
    {
        public int MaLoaiXe { get; set; }

        [Required(ErrorMessage = "Ten loai xe khong duoc de trong")]
        [StringLength(50, ErrorMessage = "Ten loai xe toi da 50 ky tu")]
        [Display(Name = "Ten loai xe")]
        public string TenLoaiXe { get; set; } = string.Empty;

        [Required(ErrorMessage = "So cho khong duoc de trong")]
        [Range(1, 50, ErrorMessage = "So cho phai tu 1 den 50")]
        [Display(Name = "So cho")]
        public int SoCho { get; set; }

        [StringLength(500, ErrorMessage = "Mo ta toi da 500 ky tu")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Mo ta")]
        public string? MoTa { get; set; }

        [Display(Name = "Trang thai hoat dong")]
        public bool TrangThai { get; set; } = true;
    }
}