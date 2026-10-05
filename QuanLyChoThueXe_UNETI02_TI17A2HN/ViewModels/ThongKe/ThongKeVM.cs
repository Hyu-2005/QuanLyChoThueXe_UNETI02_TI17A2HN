// ThongKeVM.cs - viewmodel thong ke chi tiet bang LINQ
// sinh vien thuc hien: Duong Lam Huy - 23103100120
// module 5 - tinh tien, thanh toan, lich su va thong ke

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.ThongKe
{
    public class ThongKeVM
    {
        public DateTime TuNgay { get; set; } = new DateTime(DateTime.Now.Year, 1, 1);
        public DateTime DenNgay { get; set; } = DateTime.Now;

        // Thong ke theo xe
        public List<XeThongKeItem> TopXeThueNhieu { get; set; } = new();

        // Thong ke theo loai xe
        public List<LoaiXeThongKeItem> ThongKeTheoLoaiXe { get; set; } = new();

        // Thong ke theo hang xe
        public List<HangXeThongKeItem> ThongKeTheoHangXe { get; set; } = new();

        // Doanh thu theo thang
        public List<DoanhThuThangItem> DoanhThuTheoThang { get; set; } = new();

        // Khach hang
        public List<KhachHangThongKeItem> TopKhachHang { get; set; } = new();

        // Phu phi
        public List<PhuPhiThongKeItem> PhuPhiTheoLoai { get; set; } = new();

        // Tong hop
        public decimal TongDoanhThu { get; set; }
        public decimal TongPhuPhi { get; set; }
        public int TongSoDon { get; set; }
        public int SoDonHuy { get; set; }
        public int SoDonTuChoi { get; set; }
        public double TyLeHuy { get; set; }        // %
        public double TyLeKhaiThac { get; set; }   // %
        public double ThoiLuongThueTrungBinh { get; set; }  // ngay
    }

    public class XeThongKeItem
    {
        public int MaXe { get; set; }
        public string? BienSo { get; set; }
        public string? TenXe { get; set; }
        public string? TenLoaiXe { get; set; }
        public int SoLanThue { get; set; }
        public decimal DoanhThu { get; set; }
    }

    public class LoaiXeThongKeItem
    {
        public int MaLoaiXe { get; set; }
        public string? TenLoaiXe { get; set; }
        public int SoLanThue { get; set; }
        public decimal DoanhThu { get; set; }
    }

    public class HangXeThongKeItem
    {
        public int MaHangXe { get; set; }
        public string? TenHangXe { get; set; }
        public int SoLanThue { get; set; }
        public decimal DoanhThu { get; set; }
    }

    public class DoanhThuThangItem
    {
        public int Thang { get; set; }
        public int Nam { get; set; }
        public int SoDon { get; set; }
        public decimal DoanhThu { get; set; }
    }

    public class KhachHangThongKeItem
    {
        public int MaKhachHang { get; set; }
        public string? HoTen { get; set; }
        public string? SoDienThoai { get; set; }
        public int SoLanThue { get; set; }
        public decimal TongChiTieu { get; set; }
    }

    public class PhuPhiThongKeItem
    {
        public string? LoaiPhi { get; set; }
        public decimal TongTien { get; set; }
        public int SoLan { get; set; }
    }
}