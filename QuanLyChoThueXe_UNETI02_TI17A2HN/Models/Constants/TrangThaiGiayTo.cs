// ============================================================
// File: Models/Constants/TrangThaiGiayTo.cs
// Họ và tên: [Vũ Tiến Đạt]
// Mã sinh viên: [23103100119]
// Nội dung thực hiện: Hằng số trạng thái kiểm tra giấy tờ
// Module: Module 3 - Khách hàng
// ============================================================

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants
{
    public static class TrangThaiGiayTo
    {
        public const string ChoKiemTra = "Chờ kiểm tra";
        public const string DaXacMinh = "Đã xác minh";
        public const string TuChoi = "Bị từ chối";

        public static readonly string[] TatCa = { ChoKiemTra, DaXacMinh, TuChoi };
    }
}
