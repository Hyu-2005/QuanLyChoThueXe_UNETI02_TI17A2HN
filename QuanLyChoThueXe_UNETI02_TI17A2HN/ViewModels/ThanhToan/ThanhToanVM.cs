// ThanhToanVM.cs - viewmodel cho man hinh thanh toan
// sinh vien thuc hien: Duong Lam Huy - 23103100120
// module 5 - tinh tien, thanh toan, lich su va thong ke

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.ThanhToan
{
    public class ThanhToanVM
    {
        public int MaDatXe { get; set; }

        // Thong tin hien thi
        public string? BienSo { get; set; }
        public string? TenXe { get; set; }
        public string? TenKhachHang { get; set; }
        public string? SoDienThoaiKhachHang { get; set; }
        public DateTime ThoiGianNhanDuKien { get; set; }
        public DateTime ThoiGianTraDuKien { get; set; }
        public DateTime? ThoiGianTraThucTe { get; set; }

        // Tinh toan (khong cho user nhap)
        public int SoNgayThue { get; set; }
        public decimal TienThue { get; set; }
        public decimal TongPhuPhi { get; set; }
        public decimal TienCocDaThu { get; set; }
        public decimal TongThanhToan { get; set; }
        public decimal SoTienConLai { get; set; }

        // Chi tiet phu phi de hien thi
        public decimal PhiQuaHan { get; set; }
        public decimal PhiVuotKm { get; set; }
        public decimal PhiNhienLieu { get; set; }
        public decimal PhiHuHong { get; set; }

        // Nhap lieu thanh toan
        [Required(ErrorMessage = "Vui long chon phuong thuc thanh toan")]
        [Display(Name = "Phuong thuc thanh toan")]
        public string PhuongThucThanhToan { get; set; } = string.Empty;

        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Ghi chu")]
        public string? GhiChu { get; set; }

        public List<SelectListItem> DanhSachPhuongThuc { get; set; } = new();

        /// <summary>
        /// Co phai thanh toan lan dau (tu ChoThanhToan) hay xem lai (da HoanThanh)
        /// </summary>
        public bool DaThanhToan { get; set; }
    }
}