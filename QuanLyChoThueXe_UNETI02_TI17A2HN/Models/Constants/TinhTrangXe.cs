namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants
{
    public static class TinhTrangXe
    {
        public const string SanSang = "Sẵn sàng";
        public const string DangGiuCho = "Đang được giữ chỗ";
        public const string DangChoThue = "Đang cho thuê";
        public const string BaoDuong = "Bảo dưỡng";
        public const string NgungHoatDong = "Ngừng hoạt động";

        public static readonly string[] TatCa =
        {
            SanSang, DangGiuCho, DangChoThue, BaoDuong, NgungHoatDong
        };

        /// <summary>
        /// Các trạng thái xe CÓ THỂ nhận đơn mới (dùng trong tìm xe trống)
        /// </summary>
        public static readonly string[] CoTheChoThue =
        {
            SanSang, DangGiuCho
        };
    }
}
