// ============================================================
// File: ViewModels/Home/HomeIndexVM.cs
// Họ và tên: Vũ Tiến Đạt
// Mã sinh viên: 23103100119
// Nội dung thực hiện: ViewModel trang chủ - danh sách xe nổi bật hiển thị theo giao diện VIRELLO
// Module: Module 3 - Khách hàng
// ============================================================

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.Home
{
    public class HomeIndexVM
    {
        public List<XeNoiBatVM> XeNoiBat { get; set; } = new();
    }

    public class XeNoiBatVM
    {
        public int MaXe { get; set; }
        public string TenXe { get; set; } = string.Empty;
        public string TenLoaiXe { get; set; } = string.Empty;
        public string TenHangXe { get; set; } = string.Empty;
        public int SoCho { get; set; }
        public int NamSanXuat { get; set; }
        public string? MauSac { get; set; }
        public string? AnhXe { get; set; }
        public decimal DonGiaNgay { get; set; }
    }
}
