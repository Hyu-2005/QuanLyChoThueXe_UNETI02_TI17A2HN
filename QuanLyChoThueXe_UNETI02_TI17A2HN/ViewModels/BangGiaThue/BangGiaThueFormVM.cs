// file: ViewModels/BangGiaThue/BangGiaThueFormVM.cs
// noi dung: ViewModel form Them/Sua Bang gia thue
// sinh vien thuc hien: Dao Gia Hung - 23103100065
// module: Module 2 - Quan ly xe, Bang gia thue

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.BangGiaThue
{
    public class BangGiaThueFormVM
    {
        public int MaBangGia { get; set; }

        [Display(Name = "Áp dụng cho loại xe")]
        public int? MaLoaiXe { get; set; }

        [Display(Name = "Áp dụng cho xe cụ thể")]
        public int? MaXe { get; set; }

        [Required(ErrorMessage = "Đơn giá ngày không được để trống")]
        [Range(1, double.MaxValue, ErrorMessage = "Đơn giá ngày phải lớn hơn 0")]
        [Display(Name = "Đơn giá ngày (VNĐ)")]
        public decimal DonGiaNgay { get; set; }

        [Required(ErrorMessage = "Đơn giá giờ không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Đơn giá giờ phải lớn hơn hoặc bằng 0")]
        [Display(Name = "Đơn giá giờ (VNĐ)")]
        public decimal DonGiaGio { get; set; }

        [Required(ErrorMessage = "Tiền cọc không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Tiền cọc phải lớn hơn hoặc bằng 0")]
        [Display(Name = "Tiền cọc mặc định (VNĐ)")]
        public decimal TienCocMacDinh { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu không được để trống")]
        [DataType(DataType.Date)]
        [Display(Name = "Từ ngày")]
        public DateTime TuNgay { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Ngày kết thúc không được để trống")]
        [DataType(DataType.Date)]
        [Display(Name = "Đến ngày")]
        public DateTime DenNgay { get; set; } = DateTime.Today.AddYears(1);

        [Display(Name = "Trạng thái hoạt động")]
        public bool TrangThai { get; set; } = true;

        public List<SelectListItem> DanhSachLoaiXe { get; set; } = new();
        public List<SelectListItem> DanhSachXe { get; set; } = new();
    }
}