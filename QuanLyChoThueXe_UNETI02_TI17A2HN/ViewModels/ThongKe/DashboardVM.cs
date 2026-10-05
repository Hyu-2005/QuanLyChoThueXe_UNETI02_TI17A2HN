// DashboardVM.cs - viewmodel dashboard tong quan
// sinh vien thuc hien: Duong Lam Huy - 23103100120
// module 5 - tinh tien, thanh toan, lich su va thong ke

using DatXeEntity = QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities.DatXe;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.ThongKe
{
    public class DashboardVM
    {
        // Xe
        public int TongSoXe { get; set; }
        public int XeSanSang { get; set; }
        public int XeDangThue { get; set; }
        public int XeBaoDuong { get; set; }
        public int XeNgung { get; set; }

        // Don
        public int DonChoDuyet { get; set; }
        public int DonDaDuyet { get; set; }
        public int DonDangThue { get; set; }
        public int DonChoThanhToan { get; set; }
        public int DonHoanThanh { get; set; }
        public int DonDaHuy { get; set; }
        public int DonTuChoi { get; set; }

        // Trong ngay
        public int LuotNhanXeHomNay { get; set; }
        public int LuotTraXeHomNay { get; set; }
        public decimal DoanhThuHomNay { get; set; }
        public decimal DoanhThuThangNay { get; set; }

        // Canh bao
        public List<DatXeEntity> DonSapDenGioNhan { get; set; } = new();
        public List<DatXeEntity> DonQuaGioTra { get; set; } = new();
    }
}