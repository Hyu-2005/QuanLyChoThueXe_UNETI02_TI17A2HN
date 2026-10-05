// ThanhToanListVM.cs - viewmodel danh sach thanh toan
// sinh vien thuc hien: Duong Lam Huy - 23103100120
// module 5 - tinh tien, thanh toan, lich su va thong ke

using Microsoft.AspNetCore.Mvc.Rendering;
using ThanhToanEntity = QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities.ThanhToan;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.ThanhToan
{
    public class ThanhToanListVM
    {
        public List<ThanhToanEntity> DanhSach { get; set; } = new();

        public string? TuKhoa { get; set; }
        public string? TrangThaiThanhToan { get; set; }
        public string? PhuongThucThanhToan { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string? SapXepTheo { get; set; }
        public string? ThuTuSapXep { get; set; }

        public int Trang { get; set; } = 1;
        public int KichThuocTrang { get; set; } = 10;
        public int TongSoBanGhi { get; set; }
        public int TongSoTrang => (int)Math.Ceiling((double)TongSoBanGhi / KichThuocTrang);
        public bool CoTrangTruoc => Trang > 1;
        public bool CoTrangSau => Trang < TongSoTrang;

        public List<SelectListItem> DanhSachTrangThai { get; set; } = new();
        public List<SelectListItem> DanhSachPhuongThuc { get; set; } = new();

        // Thong ke tong
        public decimal TongDoanhThu { get; set; }
    }
}