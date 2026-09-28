// ============================================================
// File: ViewModels/DatXe/TimXeTrongVM.cs
// Noi dung: ViewModel cho man hinh tim xe trong theo khoang thoi gian
// Sinh vien thuc hien: Vu Tien Dat - 23103100119 - SV3
// Module: Module 3 - Khach hang, Tim xe trong va Dat xe
// ============================================================

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using XeEntity = QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities.Xe;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.DatXe
{
    public class TimXeTrongVM
    {
        [Required(ErrorMessage = "Vui long chon thoi gian nhan")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Thoi gian nhan")]
        public DateTime ThoiGianNhan { get; set; } = DateTime.Now.AddDays(1);

        [Required(ErrorMessage = "Vui long chon thoi gian tra")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Thoi gian tra")]
        public DateTime ThoiGianTra { get; set; } = DateTime.Now.AddDays(3);

        // Bo loc (tuy chon)
        [Display(Name = "Loai xe")]
        public int? MaLoaiXe { get; set; }

        [Display(Name = "Hang xe")]
        public int? MaHangXe { get; set; }

        [Display(Name = "So cho")]
        public int? SoCho { get; set; }

        [Display(Name = "Gia tu (d)")]
        public decimal? DonGiaTu { get; set; }

        [Display(Name = "Den gia (d)")]
        public decimal? DonGiaDen { get; set; }

        // Du lieu tra ve
        public List<XeTrongItemVM> DanhSachXeTrong { get; set; } = new();
        public bool DaTimKiem { get; set; } = false;

        // Dropdown
        public List<SelectListItem> DanhSachLoaiXe { get; set; } = new();
        public List<SelectListItem> DanhSachHangXe { get; set; } = new();
    }

    public class XeTrongItemVM
    {
        public int MaXe { get; set; }
        public string BienSo { get; set; } = string.Empty;
        public string TenXe { get; set; } = string.Empty;
        public string TenLoaiXe { get; set; } = string.Empty;
        public string TenHangXe { get; set; } = string.Empty;
        public int SoCho { get; set; }
        public int NamSanXuat { get; set; }
        public string? AnhXe { get; set; }

        // Gia ap dung cho khoang thoi gian
        public decimal DonGiaNgay { get; set; }
        public decimal DonGiaGio { get; set; }
        public decimal TienCocMacDinh { get; set; }

        // Tinh san cho view
        public int SoNgayThue { get; set; }
        public decimal TienThueDuKien { get; set; }
    }
}