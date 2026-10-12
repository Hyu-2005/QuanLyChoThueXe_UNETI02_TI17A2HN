// File: ViewModels/ThongKe/DashboardVM.cs
// Nội dung: ViewModel Dashboard với dữ liệu thực từ Service
// Sinh viên thực hiện: Dương Lâm Huy - 23103100120 - SV5
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

        // ================= BIEU DO =================
        /// Doanh thu 7 ngày gần nhất (có thể null nếu chưa đủ dữ liệu)
        public List<DoanhThuNgayItem> DoanhThu7Ngay { get; set; } = new();

        /// Phân bố trạng thái đơn
        public List<TrangThaiDonItem> PhanBoTrangThaiDon { get; set; } = new();

        // ================= TOP XE (THÊM MỚI) =================
        /// Top 5 xe được thuê nhiều nhất (dựa vào số đơn Hoàn thành)
        public List<TopXeItem> TopXeThueNhieu { get; set; } = new();

        // ================= HOẠT ĐỘNG GẦN ĐÂY (THÊM MỚI) =================
        /// Hoạt động gần đây (đơn mới, thanh toán, trả xe...)
        public List<HoatDongItem> HoatDongGanDay { get; set; } = new();

        // ================= SO SÁNH THÁNG TRƯỚC (THÊM MỚI) =================
        public int TongSoXeThangTruoc { get; set; }
        public int XeSanSangThangTruoc { get; set; }
        public int XeDangThueThangTruoc { get; set; }
        public int XeBaoDuongThangTruoc { get; set; }
    }

    // ============================================================
    // CÁC CLASS CON
    // ============================================================

    public class DoanhThuNgayItem
    {
        public DateTime Ngay { get; set; }
        public decimal DoanhThu { get; set; }
        public int SoDon { get; set; }   // THÊM: số đơn trong ngày
    }

    public class TrangThaiDonItem
    {
        public string? TrangThai { get; set; }
        public int SoLuong { get; set; }
    }

    /// Top xe được thuê nhiều
    public class TopXeItem
    {
        public int MaXe { get; set; }
        public string? BienSo { get; set; }
        public string? TenXe { get; set; }
        public string? AnhXe { get; set; }
        public int SoLanThue { get; set; }
        public decimal DoanhThu { get; set; }
    }

    /// Hoạt động gần đây
    public class HoatDongItem
    {
        public string Loai { get; set; } = "";      // "don-moi", "tra-xe", "bao-duong", "thanh-toan"
        public string TieuDe { get; set; } = "";
        public string MoTa { get; set; } = "";
        public DateTime ThoiGian { get; set; }
    }
}