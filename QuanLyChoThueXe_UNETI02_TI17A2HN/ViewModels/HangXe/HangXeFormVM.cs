// File: ViewModels/LoaiXe/LoaiXeFormVM.cs
// Noi dung: ViewModel cho form Them/Sua LoaiXe
// Sinh vien thuc hien: Pham Gia Minh Hoang - 23103100087 - SV1
// Module: Module 1 - Tai khoan, Dang nhap, Phan quyen, Loai xe, Hang xe

using System.ComponentModel.DataAnnotations;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.HangXe
{
    public class HangXeFormVM
    {
        public int MaHangXe { get; set; }

        [Required(ErrorMessage = "Ten hang xe khong duoc de trong")]
        [StringLength(50, ErrorMessage = "Ten hang xe toi da 50 ky tu")]
        [Display(Name = "Ten hang xe")]
        public string TenHangXe { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Quoc gia toi da 50 ky tu")]
        [Display(Name = "Quoc gia")]
        public string? QuocGia { get; set; }

        [StringLength(500, ErrorMessage = "Mo ta toi da 500 ky tu")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Mo ta")]
        public string? MoTa { get; set; }

        [Display(Name = "Trang thai hoat dong")]
        public bool TrangThai { get; set; } = true;
    }
}