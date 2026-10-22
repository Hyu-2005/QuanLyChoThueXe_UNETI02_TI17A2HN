// ==============================================================
// Bắt Đầu Code của SV4: Nguyễn Hoàng Đức Hiếu
// file: Models/Constants/LoaiBaoDuong.cs
// nội dung: STT 11 - Định nghĩa hằng số loại bảo dưỡng và trạng thái bảo dưỡng xe
// sinh viên thực hiện : Nguyễn Hoàng Đức Hiếu -  23103100116 - SV4 
// ==============================================================
namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants
{
    public static class LoaiBaoDuong
    {
        public const string DinhKy = "Định kỳ theo km";
        public const string SuCoSauThue = "Sự cố / Hư hại sau thuê";
        public const string ThayDauLocNhot = "Thay dầu & Lọc nhớt";
        public const string ThayLopPhuTung = "Thay lốp & Phụ tùng hao mòn";
        public const string DangKiemBaoHiem = "Đăng kiểm & Bảo hiểm";
        public const string TongThe = "Bảo dưỡng toàn diện";

        public static readonly string[] TatCa =
        {
            DinhKy,
            SuCoSauThue,
            ThayDauLocNhot,
            ThayLopPhuTung,
            DangKiemBaoHiem,
            TongThe
        };
    }

    public static class TrangThaiBaoDuong
    {
        public const string DangBaoDuong = "Đang bảo dưỡng";
        public const string DaHoanThanh = "Đã hoàn thành";
        public const string DaHuy = "Đã hủy";

        public static readonly string[] TatCa =
        {
            DangBaoDuong,
            DaHoanThanh,
            DaHuy
        };
    }
}
// ==============================================================
// Kết thúc Code của SV4
// ==============================================================
