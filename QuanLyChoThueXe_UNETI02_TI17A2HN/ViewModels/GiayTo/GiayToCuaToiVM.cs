// ============================================================
// File: ViewModels/GiayTo/GiayToCuaToiVM.cs
// Họ và tên: [Vũ Tiến Đạt]
// Mã sinh viên: [23103100119]
// Nội dung thực hiện: ViewModel form tải giấy tờ + danh sách giấy tờ của khách hàng
// Module: Module 3 - Khách hàng
// ============================================================

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.GiayTo
{
    public class GiayToCuaToiVM
    {
        // ===== Form tai len =====
        [Required(ErrorMessage = "Vui lòng chọn loại giấy tờ")]
        [Display(Name = "Loại giấy tờ")]
        public string LoaiGiayTo { get; set; } = "CCCD";

        [Required(ErrorMessage = "Số giấy tờ không được để trống")]
        [StringLength(30)]
        [Display(Name = "Số giấy tờ")]
        public string SoGiayTo { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Ngày hết hạn")]
        public DateTime? NgayHetHan { get; set; }

        [Display(Name = "Ảnh mặt trước")]
        public IFormFile? FileMatTruoc { get; set; }

        [Display(Name = "Ảnh mặt sau (không bắt buộc)")]
        public IFormFile? FileMatSau { get; set; }

        // ===== Du lieu hien thi (khong bind tu form) =====
        [BindNever]
        [ValidateNever]
        public List<GiayToKhachHang> DanhSach { get; set; } = new();
    }
}
