// ============================================================
// File: Models/Constants/LoaiGiayTo.cs
// Họ và tên: [Vũ Tiến Đạt]
// Mã sinh viên: [23103100119]
// Nội dung thực hiện: Hằng số loại giấy tờ khách hàng (CCCD, GPLX)
// Module: Module 3 - Khách hàng
// ============================================================

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants
{
    public static class LoaiGiayTo
    {
        public const string CCCD = "CCCD";
        public const string GPLX = "GPLX";

        public static readonly string[] TatCa = { CCCD, GPLX };

        public static string GetDisplayName(string loai) => loai switch
        {
            CCCD => "Căn cước công dân / CMND",
            GPLX => "Giấy phép lái xe",
            _ => loai
        };
    }
}
