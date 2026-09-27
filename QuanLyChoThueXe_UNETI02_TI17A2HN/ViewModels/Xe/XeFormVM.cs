// XeFormVM.cs - viewmodel form them/sua xe
// sinh vien thuc hien: Dao Gia Hung - 23103100065
// module 2 - quan ly xe, bang gia thue

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.Xe
{
    public class XeFormVM
    {
        public int MaXe { get; set; }

        [Required(ErrorMessage = "Bien so khong duoc de trong")]
        [StringLength(20, ErrorMessage = "Bien so toi da 20 ky tu")]
        [Display(Name = "Bien so xe")]
        public string BienSo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ten xe khong duoc de trong")]
        [StringLength(100, ErrorMessage = "Ten xe toi da 100 ky tu")]
        [Display(Name = "Ten xe")]
        public string TenXe { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui long chon loai xe")]
        [Display(Name = "Loai xe")]
        public int MaLoaiXe { get; set; }

        [Required(ErrorMessage = "Vui long chon hang xe")]
        [Display(Name = "Hang xe")]
        public int MaHangXe { get; set; }

        [Required(ErrorMessage = "Nam san xuat khong duoc de trong")]
        [Range(1990, 2100, ErrorMessage = "Nam san xuat phai tu 1990 den 2100")]
        [Display(Name = "Nam san xuat")]
        public int NamSanXuat { get; set; } = DateTime.Now.Year;

        [StringLength(30, ErrorMessage = "Mau sac toi da 30 ky tu")]
        [Display(Name = "Mau sac")]
        public string? MauSac { get; set; }

        [Required(ErrorMessage = "So cho khong duoc de trong")]
        [Range(1, 50, ErrorMessage = "So cho phai tu 1 den 50")]
        [Display(Name = "So cho")]
        public int SoCho { get; set; }

        [Required(ErrorMessage = "So km khong duoc de trong")]
        [Range(0, double.MaxValue, ErrorMessage = "So km phai lon hon hoac bang 0")]
        [Display(Name = "So km hien tai")]
        public decimal SoKmHienTai { get; set; } = 0;

        [Required(ErrorMessage = "Vui long chon tinh trang")]
        [StringLength(30)]
        [Display(Name = "Tinh trang")]
        public string TinhTrang { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Mo ta toi da 500 ky tu")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Mo ta")]
        public string? MoTa { get; set; }

        [Display(Name = "Anh hien tai")]
        public string? AnhXe { get; set; }       

        [Display(Name = "Chon anh moi")]
        public IFormFile? FileAnh { get; set; }  

        public List<SelectListItem> DanhSachLoaiXe { get; set; } = new();
        public List<SelectListItem> DanhSachHangXe { get; set; } = new();
        public List<SelectListItem> DanhSachTinhTrang { get; set; } = new();
    }
}