// ============================================================
// File: Models/Constants/QuyDinhDiemThuong.cs
// Nội dung: Hằng số quy định ví điểm thưởng
// Sinh viên thực hiện: Dương Lâm Huy - 23103100120 - SV5
// Module: Module 5 - Tính tiền, Thanh toán
// ============================================================

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants
{
    public static class QuyDinhDiemThuong
    {
        /// <summary>Số điểm tối thiểu để bắt đầu sử dụng</summary>
        public const int DiemToiThieuDeSuDung = 10;

        /// <summary>Fallback nếu chưa cấu hình trong appsettings.json</summary>
        public const decimal FallbackSoTienDeTichMotDiem = 100_000m;
        public const decimal FallbackGiaTriMotDiem = 5_000m;
        public const decimal FallbackTyLeToiDaDungDiem = 0.5m;
        public const int FallbackDiemToiDaMoiHoaDon = 500;
    }
}