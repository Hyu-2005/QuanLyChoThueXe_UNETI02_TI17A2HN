// ThongKeVM.cs - viewmodel thong ke chi tiet bang LINQ
// Họ và tên: Duong Lam Huy
// Mã sinh viên: 23103100120
// Nội dung thực hiện: ViewModel thống kê chi tiết với dữ liệu cho biểu đồ Chart.js
// Module: Module 5 - Tính tiền, Thanh toán, Lịch sử, Thống kê

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.ThongKe
{
    public class ThongKeVM
    {
        // ================= KY BAO CAO =================
        public DateTime TuNgay { get; set; } = new DateTime(DateTime.Now.Year, 1, 1);
        public DateTime DenNgay { get; set; } = DateTime.Now;

        // ================= THONG KE THEO XE =================
        public List<XeThongKeItem> TopXeThueNhieu { get; set; } = new();

        // ================= THONG KE THEO LOAI XE =================
        public List<LoaiXeThongKeItem> ThongKeTheoLoaiXe { get; set; } = new();

        // ================= THONG KE THEO HANG XE =================
        public List<HangXeThongKeItem> ThongKeTheoHangXe { get; set; } = new();

        // ================= DOANH THU THEO THANG =================
        public List<DoanhThuThangItem> DoanhThuTheoThang { get; set; } = new();

        // ================= TOP KHACH HANG =================
        public List<KhachHangThongKeItem> TopKhachHang { get; set; } = new();

        // ================= PHU PHI THEO LOAI =================
        public List<PhuPhiThongKeItem> PhuPhiTheoLoai { get; set; } = new();

        // ================= TONG HOP =================
        public decimal TongDoanhThu { get; set; }
        public decimal TongPhuPhi { get; set; }
        public int TongSoDon { get; set; }
        public int SoDonHuy { get; set; }
        public int SoDonTuChoi { get; set; }
        public double TyLeHuy { get; set; }
        public double TyLeKhaiThac { get; set; }
        public double ThoiLuongThueTrungBinh { get; set; }

        // ================= BIEU DO CHART.JS =================

        /// Doanh thu 7 ngày gần nhất cho biểu đồ đường
        public List<DoanhThuNgayItem> DoanhThu7Ngay { get; set; } = new();

        /// Số đơn theo trạng thái cho biểu đồ tròn
        public List<TrangThaiDonItem> SoDonTheoTrangThai { get; set; } = new();
    }

    // ============================================================
    // CAC CLASS CON CHO TUNG LOAI THONG KE
    // ============================================================

    /// Thong ke theo xe
    public class XeThongKeItem
    {
        public int MaXe { get; set; }
        public string? BienSo { get; set; }
        public string? TenXe { get; set; }
        public string? TenLoaiXe { get; set; }
        public int SoLanThue { get; set; }
        public decimal DoanhThu { get; set; }
    }

    /// Thong ke theo loai xe
    public class LoaiXeThongKeItem
    {
        public int MaLoaiXe { get; set; }
        public string? TenLoaiXe { get; set; }
        public int SoLanThue { get; set; }
        public decimal DoanhThu { get; set; }
    }

    /// Thong ke theo hang xe
    public class HangXeThongKeItem
    {
        public int MaHangXe { get; set; }
        public string? TenHangXe { get; set; }
        public int SoLanThue { get; set; }
        public decimal DoanhThu { get; set; }
    }

    /// Doanh thu theo thang
    public class DoanhThuThangItem
    {
        public int Thang { get; set; }
        public int Nam { get; set; }
        public int SoDon { get; set; }
        public decimal DoanhThu { get; set; }
    }

    /// Top khach hang
    public class KhachHangThongKeItem
    {
        public int MaKhachHang { get; set; }
        public string? HoTen { get; set; }
        public string? SoDienThoai { get; set; }
        public int SoLanThue { get; set; }
        public decimal TongChiTieu { get; set; }
    }

    /// Phu phi theo loai
    public class PhuPhiThongKeItem
    {
        public string? LoaiPhi { get; set; }
        public decimal TongTien { get; set; }
        public int SoLan { get; set; }
    }
}