// LichSuThueXeVM.cs - viewmodel lich su thue cua xe
// sinh vien thuc hien: Duong Lam Huy - 23103100120
// module 5 - tinh tien, thanh toan, lich su va thong ke

using DatXeEntity = QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities.DatXe;
using XeEntity = QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities.Xe;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.Xe
{
    public class LichSuThueXeVM
    {
        public XeEntity Xe { get; set; } = null!;
        public List<DatXeEntity> DanhSachDon { get; set; } = new();

        // Thong ke nhanh
        public int TongSoLanThue { get; set; }
        public int SoLanDangThue { get; set; }
        public decimal TongDoanhThu { get; set; }
        public decimal TongKmDaDi { get; set; }
    }
}