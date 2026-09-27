// file: ViewModels/BangGiaThue/BangGiaThueListVM.cs
// noi dung: ViewModel danh sach Bang gia thue
// sinh vien thuc hien: Dao Gia Hung - 23103100065
// module: Module 2 - Quan ly xe, Bang gia thue

using Microsoft.AspNetCore.Mvc.Rendering;
using BangGiaThueEntity = QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities.BangGiaThue;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.BangGiaThue
{
    public class BangGiaThueListVM
    {
        public List<BangGiaThueEntity> DanhSach { get; set; } = new();

        public string? TuKhoa { get; set; }

        public int? MaLoaiXe { get; set; }
        public int? MaXe { get; set; }
        public bool? TrangThai { get; set; }
        public DateTime? TuNgayLoc { get; set; }
        public DateTime? DenNgayLoc { get; set; }

        public string? SapXepTheo { get; set; }
        public string? ThuTuSapXep { get; set; }

        public int Trang { get; set; } = 1;
        public int KichThuocTrang { get; set; } = 10;
        public int TongSoBanGhi { get; set; }
        public int TongSoTrang => (int)Math.Ceiling((double)TongSoBanGhi / KichThuocTrang);
        public bool CoTrangTruoc => Trang > 1;
        public bool CoTrangSau => Trang < TongSoTrang;

        public List<SelectListItem> DanhSachLoaiXe { get; set; } = new();
        public List<SelectListItem> DanhSachXe { get; set; } = new();
    }
}