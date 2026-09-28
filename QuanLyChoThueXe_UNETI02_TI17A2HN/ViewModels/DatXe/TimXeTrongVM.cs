// File: ViewModels/DatXe/TimXeTrongVM.cs
// Noi dung: ViewModel tim kiem xe trong theo khoang thoi gian (cho khach hang)
// Module: Module 3 - Khach hang tim xe trong, dat xe, kiem tra trung lich

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.DatXe
{
    public class TimXeTrongVM
    {
        [Required(ErrorMessage = "Vui long chon thoi gian nhan")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Thoi gian nhan du kien")]
        public DateTime? ThoiGianNhan { get; set; }

        [Required(ErrorMessage = "Vui long chon thoi gian tra")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Thoi gian tra du kien")]
        public DateTime? ThoiGianTra { get; set; }

        [Display(Name = "Loai xe")]
        public int? MaLoaiXe { get; set; }

        [Display(Name = "So cho toi thieu")]
        public int? SoCho { get; set; }

        public List<SelectListItem> DanhSachLoaiXe { get; set; } = new();

        /// <summary>Ket qua tim kiem - chi co gia tri sau khi da submit form</summary>
        public List<XeTrongVM> KetQua { get; set; } = new();

        public bool DaTimKiem { get; set; } = false;
    }
}