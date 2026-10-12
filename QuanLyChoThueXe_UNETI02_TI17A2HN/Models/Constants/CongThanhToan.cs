// File: Models/Constants/CongThanhToan.cs
// Nội dung: Hằng số cổng thanh toán trực tuyến giả lập
// Sinh viên thực hiện: Dương Lâm Huy - 23103100120 - SV5
// Module: Module 5 - Tính tiền, Thanh toán, Lịch sử, Thống kê

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants
{
    public static class CongThanhToan
    {
        public const string VNPay = "VNPay";
        public const string MoMo = "MoMo";
        public const string ZaloPay = "ZaloPay";
        public const string Banking = "Banking QR";

        public static readonly string[] TatCa = { VNPay, MoMo, ZaloPay, Banking };

        public static string GetDisplayName(string cong) => cong switch
        {
            VNPay => "VNPay (giả lập)",
            MoMo => "Ví MoMo (giả lập)",
            ZaloPay => "ZaloPay (giả lập)",
            Banking => "Chuyển khoản QR (giả lập)",
            _ => cong
        };

        public static string GetIcon(string cong) => cong switch
        {
            VNPay => "🏦",
            MoMo => "📱",
            ZaloPay => "💠",
            Banking => "🏧",
            _ => "💳"
        };
    }

    public static class TrangThaiGiaoDich
    {
        public const string ChoThanhToan = "Chờ thanh toán";
        public const string ThanhCong = "Thành công";
        public const string ThatBai = "Thất bại";
        public const string DaHuy = "Đã hủy";

        public static readonly string[] TatCa = { ChoThanhToan, ThanhCong, ThatBai, DaHuy };
    }
}