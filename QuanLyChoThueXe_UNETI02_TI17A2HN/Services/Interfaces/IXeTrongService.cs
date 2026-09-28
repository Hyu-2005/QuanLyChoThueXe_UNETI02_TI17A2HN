// ============================================================
// File: Services/Interfaces/IXeTrongService.cs
// Noi dung: Interface tim xe trong theo khoang thoi gian
// Sinh vien thuc hien: Vu Tien Dat - 23103100119 - SV3
// Module: Module 3 - Khach hang, Tim xe trong va Dat xe
// ============================================================

using QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.DatXe;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Services.Interfaces
{
    public interface IXeTrongService
    {
        /// <summary>
        /// Tim cac xe con trong (khong bi trung lich) trong khoang [thoiGianNhan, thoiGianTra].
        /// Ap dung bo loc: loai xe, hang xe, so cho, khoang gia.
        /// </summary>
        Task<List<XeTrongItemVM>> TimXeTrongAsync(
            DateTime thoiGianNhan,
            DateTime thoiGianTra,
            int? maLoaiXe = null,
            int? maHangXe = null,
            int? soCho = null,
            decimal? donGiaTu = null,
            decimal? donGiaDen = null);
    }
}