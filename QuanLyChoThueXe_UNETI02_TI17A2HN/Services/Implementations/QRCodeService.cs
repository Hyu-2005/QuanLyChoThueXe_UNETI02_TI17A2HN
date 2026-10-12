// File: Services/Implementations/QRCodeService.cs
// Nội dung: Triển khai sinh mã QR Code bằng QRCoder
// Sinh viên thực hiện: Dương Lâm Huy - 23103100120 - SV5
// Module: Module 5 - Tính tiền, Thanh toán, Lịch sử, Thống kê

using QRCoder;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Services.Interfaces;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Services.Implementations
{
    public class QRCodeService : IQRCodeService
    {
        public byte[] GenerateQRCode(string content, int pixelsPerModule = 15)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Nội dung QR không được rỗng", nameof(content));

            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(
                content,
                QRCodeGenerator.ECCLevel.Q);  // Mức sửa lỗi Q (25%)

            using var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(pixelsPerModule);
        }
    }
}