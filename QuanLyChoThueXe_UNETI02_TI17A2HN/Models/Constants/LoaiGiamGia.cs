// ============================================================
// File: Models/Constants/LoaiGiamGia.cs
// Nội dung: Hằng số loại giảm giá cho Voucher
// Sinh viên thực hiện: Dương Lâm Huy - 23103100120 - SV5
// Module: Module 5 - Tính tiền, Thanh toán
// ============================================================

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants
{
    public static class LoaiGiamGia
    {
        public const string PhanTram = "Phần trăm";
        public const string SoTienCoDinh = "Số tiền cố định";

        public static readonly string[] TatCa = { PhanTram, SoTienCoDinh };

        public static string GetDisplayName(string loai) => loai switch
        {
            PhanTram => "Giảm theo phần trăm (%)",
            SoTienCoDinh => "Giảm số tiền cố định (VNĐ)",
            _ => loai
        };
    }
}