// File: ViewModels/TaiKhoan/DoiMatKhauVM.cs
// Noi dung: ViewModel cho man hinh doi mat khau
// Sinh vien thuc hien: Pham Gia Minh Hoang - 23103100087 - SV1
// Module: Module 1 - Tai khoan, Dang nhap, Phan quyen, Loai xe, Hang xe

using System.ComponentModel.DataAnnotations;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.TaiKhoan
{
    public class DoiMatKhauVM
    {
        [Required(ErrorMessage = "Mat khau cu khong duoc de trong")]
        [DataType(DataType.Password)]
        [Display(Name = "Mat khau cu")]
        public string MatKhauCu { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mat khau moi khong duoc de trong")]
        [StringLength(255, MinimumLength = 6,
            ErrorMessage = "Mat khau phai tu 6 ky tu tro len")]
        [DataType(DataType.Password)]
        [Display(Name = "Mat khau moi")]
        public string MatKhauMoi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Xac nhan mat khau khong duoc de trong")]
        [Compare(nameof(MatKhauMoi), ErrorMessage = "Xac nhan mat khau khong khop")]
        [DataType(DataType.Password)]
        [Display(Name = "Xac nhan mat khau moi")]
        public string XacNhanMatKhau { get; set; } = string.Empty;
    }
}