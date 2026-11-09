// ============================================================
// File: Services/Interfaces/IMaGiamGiaService.cs
// Nội dung: Interface quản lý voucher
// Sinh viên thực hiện: Dương Lâm Huy - 23103100120 - SV5
// Module: Module 5 - Tính tiền, Thanh toán
// ============================================================

using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Services.Interfaces
{
    public interface IMaGiamGiaService
    {
        /// <summary>
        /// Tìm voucher theo code và kiểm tra tất cả điều kiện áp dụng.
        /// Trả về (null, null) nếu code rỗng (không dùng voucher).
        /// </summary>
        Task<(MaGiamGia? voucher, string? loi)> TimVaKiemTraAsync(
            string? code,
            int maKhachHang,
            decimal tienThueGoc);

        /// <summary>
        /// Ghi nhận lượt sử dụng voucher: tăng SoLuotDaDung + thêm SuDungMaGiamGia.
        /// Gọi sau khi thanh toán thành công.
        /// </summary>
        Task GhiNhanSuDungAsync(
            MaGiamGia voucher,
            int maKhachHang,
            int maDatXe,
            decimal soTienGiam);

        /// <summary>
        /// Kiểm tra 1 khách đã dùng voucher này chưa
        /// </summary>
        Task<bool> KhachDaDungVoucherAsync(int maGiamGiaId, int maKhachHang);
    }
}