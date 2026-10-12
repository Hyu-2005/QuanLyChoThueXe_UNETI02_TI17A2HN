// DashboardVM.cs - viewmodel dashboard tong quan
// Họ và tên: Duong Lam Huy
// Mã sinh viên: 23103100120
// Nội dung thực hiện: ViewModel Dashboard với dữ liệu cho biểu đồ Chart.js
// Module: Module 5 - Tính tiền, Thanh toán, Lịch sử, Thống kê

using DatXeEntity = QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities.DatXe;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.ThongKe
{
    public class DashboardVM
    {
        // ================= XE =================
        public int TongSoXe { get; set; }
        public int XeSanSang { get; set; }
        public int XeDangThue { get; set; }
        public int XeBaoDuong { get; set; }
        public int XeNgung { get; set; }

        // ================= DON =================
        public int DonChoDuyet { get; set; }
        public int DonDaDuyet { get; set; }
        public int DonDangThue { get; set; }
        public int DonChoThanhToan { get; set; }
        public int DonHoanThanh { get; set; }
        public int DonDaHuy { get; set; }
        public int DonTuChoi { get; set; }

        // ================= TRONG NGAY =================
        public int LuotNhanXeHomNay { get; set; }
        public int LuotTraXeHomNay { get; set; }
        public decimal DoanhThuHomNay { get; set; }
        public decimal DoanhThuThangNay { get; set; }

        // ================= CANH BAO =================
        public List<DatXeEntity> DonSapDenGioNhan { get; set; } = new();
        public List<DatXeEntity> DonQuaGioTra { get; set; } = new();

        // ================= BIEU DO CHART.JS =================

        /// Doanh thu 7 ngày gần nhất cho biểu đồ đường
        public List<DoanhThuNgayItem> DoanhThu7Ngay { get; set; } = new();

        /// Phân bố trạng thái đơn cho biểu đồ tròn
        public List<TrangThaiDonItem> PhanBoTrangThaiDon { get; set; } = new();
    }

    // ============================================================
    // CAC CLASS DUNG CHUNG CHO BIEU DO
    // (Su dung cho ca DashboardVM va ThongKeVM)
    // ============================================================

    /// Mot ngay trong bieu do doanh thu
    public class DoanhThuNgayItem
    {
        public DateTime Ngay { get; set; }
        public decimal DoanhThu { get; set; }
    }

    /// Mot trang thai don trong bieu do tron
    public class TrangThaiDonItem
    {
        public string? TrangThai { get; set; }
        public int SoLuong { get; set; }
    }
}