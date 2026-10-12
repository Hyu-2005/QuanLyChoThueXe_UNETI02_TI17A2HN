// File: Services/Interfaces/IQRCodeService.cs
// Nội dung: Interface sinh mã QR Code
// Sinh viên thực hiện: Dương Lâm Huy - 23103100120 - SV5
// Module: Module 5 - Tính tiền, Thanh toán, Lịch sử, Thống kê

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Services.Interfaces
{
    public interface IQRCodeService
    {
        /// <summary>
        /// Sinh mã QR Code dạng PNG (trả về byte array)
        /// </summary>
        /// <param name="content">Nội dung cần mã hóa</param>
        /// <param name="pixelsPerModule">Kích thước mỗi module (mặc định 15)</param>
        byte[] GenerateQRCode(string content, int pixelsPerModule = 15);
    }
}