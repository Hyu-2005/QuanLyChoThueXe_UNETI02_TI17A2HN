// LichSuThueVM.cs - viewmodel lich su thue cua khach hang
// sinh vien thuc hien: Duong Lam Huy - 23103100120
// module 5 - tinh tien, thanh toan, lich su va thong ke

using DatXeEntity = QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities.DatXe;
using KhachHangEntity = QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities.KhachHang;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.KhachHang
{
    public class LichSuThueVM
    {
        public KhachHangEntity KhachHang { get; set; } = null!;
        public List<DatXeEntity> DanhSachDon { get; set; } = new();

        // Thong ke nhanh
        public int TongSoDon { get; set; }
        public int SoDonHoanThanh { get; set; }
        public int SoDonHuy { get; set; }
        public decimal TongChiTieu { get; set; }
        public decimal TongPhuPhi { get; set; }
    }
}