// ============================================================
// File: ViewModels/MaGiamGia/MaGiamGiaFormVM.cs
// Nội dung: ViewModel cho form Thêm/Sửa mã giảm giá
// Sinh viên thực hiện: Dương Lâm Huy - 23103100120 - SV5
// Module: Module 5 - Tính tiền, Thanh toán
// ============================================================

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.MaGiamGia
{
    public class MaGiamGiaFormVM
    {
        public int MaGiamGiaId { get; set; }

        [Required(ErrorMessage = "Mã giảm giá không được để trống")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Mã phải từ 3 đến 30 ký tự")]
        [RegularExpression(@"^[A-Z0-9_]+$",
            ErrorMessage = "Mã chỉ gồm chữ in hoa, số và dấu gạch dưới")]
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

        [Required(ErrorMessage = "Vui lòng chọn loại giảm giá")]
        [Display(Name = "Loại giảm giá")]
        public string LoaiGiamGia { get; set; } = "Phần trăm";

        [Required(ErrorMessage = "Giá trị giảm không được để trống")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá trị giảm phải > 0")]
        [Display(Name = "Giá trị giảm")]
        public decimal GiaTriGiam { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giảm tối đa phải >= 0")]
        [Display(Name = "Giảm tối đa (VNĐ)")]
        public decimal? GiamToiDa { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Đơn hàng tối thiểu phải >= 0")]
        [Display(Name = "Đơn hàng tối thiểu (VNĐ)")]
        public decimal DonHangToiThieu { get; set; } = 0;

        [Required(ErrorMessage = "Ngày bắt đầu không được để trống")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày bắt đầu")]
        public DateTime NgayBatDau { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Ngày kết thúc không được để trống")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày kết thúc")]
        public DateTime NgayKetThuc { get; set; } = DateTime.Now.AddMonths(1);

        [Required(ErrorMessage = "Số lượt dùng tối đa không được để trống")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượt dùng phải >= 1")]
        [Display(Name = "Số lượt dùng tối đa")]
        public int SoLuotDungToiDa { get; set; } = 100;

        [Display(Name = "Mỗi khách chỉ dùng 1 lần")]
        public bool MoiKhachMotLan { get; set; } = true;

        [Display(Name = "Trạng thái hoạt động")]
        public bool TrangThai { get; set; } = true;

        // ===== Dropdown =====
        public List<SelectListItem> DanhSachLoaiGiamGia { get; set; } = new();
    }
}