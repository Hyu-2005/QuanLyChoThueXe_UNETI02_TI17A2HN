// ITinhTienService.cs - interface tinh tien thue va thanh toan
// sinh vien thuc hien: Duong Lam Huy - 23103100120
// module 5 - tinh tien, thanh toan, lich su va thong ke

using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Services.Interfaces
{
    public interface ITinhTienService
    {
        /// <summary>
        /// Tinh so ngay thue (lam tron len, toi thieu 1 ngay)
        /// </summary>
        int TinhSoNgayThue(DateTime thoiGianNhan, DateTime thoiGianTra);

        /// <summary>
        /// Tinh tien thue = SoNgay * DonGiaApDung
        /// </summary>
        decimal TinhTienThue(DatXe datXe);

        /// <summary>
        /// Tinh tong hop thanh toan tu du lieu he thong
        /// </summary>
        ThanhToanResult TinhThanhToan(DatXe datXe, TraXe? traXe);
    }

    /// <summary>
    /// Ket qua tinh toan thanh toan
    /// </summary>
    public class ThanhToanResult
    {
        public int SoNgayThue { get; set; }
        public decimal TienThue { get; set; }
        public decimal TongPhuPhi { get; set; }
        public decimal TienCocDaThu { get; set; }
        public decimal TongThanhToan { get; set; }
        public decimal SoTienConLai { get; set; }
    }
}