// File: ViewModels/TaiKhoan/DangNhapVM.cs
// Noi dung: ViewModel cho man hinh dang nhap
// Sinh vien thuc hien: Pham Gia Minh Hoang - 23103100087 - SV1
// Module: Module 1 - Tai khoan, Dang nhap, Phan quyen, Loai xe, Hang xe

using System.ComponentModel.DataAnnotations;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.TaiKhoan
{
    public class DangNhapVM
    {
        [Required(ErrorMessage = "Ten dang nhap khong duoc de trong")]
        [Display(Name = "Ten dang nhap")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mat khau khong duoc de trong")]
        [DataType(DataType.Password)]
        [Display(Name = "Mat khau")]
        public string MatKhau { get; set; } = string.Empty;

        [Display(Name = "Ghi nho dang nhap")]
        public bool GhiNho { get; set; } = false;

        public string? ReturnUrl { get; set; }
    }
}