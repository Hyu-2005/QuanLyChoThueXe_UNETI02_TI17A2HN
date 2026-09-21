namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants
{
    public static class PhuongThucThanhToan
    {
        public const string TienMat = "Tiền mặt";
        public const string ChuyenKhoan = "Chuyển khoản";
        public const string The = "Thẻ ngân hàng";
        public const string ViDienTu = "Ví điện tử";

        public static readonly string[] TatCa = { TienMat, ChuyenKhoan, The, ViDienTu };
    }

    public static class TrangThaiThanhToan
    {
        public const string ChuaThanhToan = "Chưa thanh toán";
        public const string DaThanhToan = "Đã thanh toán";
        public const string HoanTien = "Hoàn tiền";

        public static readonly string[] TatCa = { ChuaThanhToan, DaThanhToan, HoanTien };
    }
}
