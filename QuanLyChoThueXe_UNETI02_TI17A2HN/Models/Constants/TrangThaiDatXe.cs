namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants
{
    public static class TrangThaiDatXe
    {
        public const string ChoDuyet = "Chờ duyệt";
        public const string DaDuyet = "Đã duyệt";
        public const string DaBanGiao = "Đã bàn giao";
        public const string DangThue = "Đang thuê";
        public const string ChoThanhToan = "Chờ thanh toán";
        public const string HoanThanh = "Hoàn thành";
        public const string TuChoi = "Từ chối";
        public const string DaHuy = "Đã hủy";

        public static readonly string[] TatCa =
        {
            ChoDuyet, DaDuyet, DaBanGiao, DangThue,
            ChoThanhToan, HoanThanh, TuChoi, DaHuy
        };

        /// <summary>
        /// ⚠️ QUAN TRỌNG: Các trạng thái CHIẾM LỊCH xe
        /// Dùng để kiểm tra trùng lịch khi tạo/duyệt đơn.
        /// Theo đề bài: tối thiểu Đã duyệt, Đã bàn giao, Đang thuê phải chiếm lịch.
        /// Nhóm bổ sung thêm Chờ thanh toán (xe chưa về Sẵn sàng hoàn toàn).
        /// </summary>
        public static readonly string[] ChiemLich =
        {
            DaDuyet, DaBanGiao, DangThue, ChoThanhToan
        };

        /// <summary>
        /// Trạng thái khách hàng ĐƯỢC PHÉP hủy đơn
        /// </summary>
        public static readonly string[] CoTheHuy =
        {
            ChoDuyet, DaDuyet
        };

        /// <summary>
        /// Trạng thái đơn ĐÃ KẾT THÚC (không thao tác được nữa)
        /// </summary>
        public static readonly string[] DaKetThuc =
        {
            HoanThanh, TuChoi, DaHuy
        };
    }
}
