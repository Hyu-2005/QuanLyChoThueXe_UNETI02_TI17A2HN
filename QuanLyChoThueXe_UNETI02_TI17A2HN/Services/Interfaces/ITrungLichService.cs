// ============================================================
// File: Services/Interfaces/ITrungLichService.cs
// Noi dung: Interface kiem tra trung lich xe theo khoang thoi gian
// Sinh vien thuc hien: Vu Tien Dat - 23103100119 - SV3
// Module: Module 3 - Khach hang, Tim xe trong va Dat xe
// ============================================================

using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Services.Interfaces
{
    public interface ITrungLichService
    {
        /// <summary>
        /// Kiem tra xe co bi trung lich trong khoang [batDauMoi, ketThucMoi] hay khong.
        /// Cong thuc giao nhau: BatDauMoi &lt; KetThucCu AND KetThucMoi &gt; BatDauCu
        /// </summary>
        /// <param name="maXe">Ma xe can kiem tra</param>
        /// <param name="batDauMoi">Thoi gian nhan du kien cua don moi</param>
        /// <param name="ketThucMoi">Thoi gian tra du kien cua don moi</param>
        /// <param name="boQuaMaDatXe">Bo qua 1 don (dung khi sua/duyet lai don cu)</param>
        /// <returns>True neu bi trung, False neu khong trung</returns>
        Task<bool> KiemTraTrungLich(int maXe, DateTime batDauMoi, DateTime ketThucMoi,
                                    int? boQuaMaDatXe = null);

        /// <summary>
        /// Lay danh sach cac don dang chiem lich cua 1 xe
        /// </summary>
        Task<List<DatXe>> LayDonChiemLich(int maXe);

        /// <summary>
        /// Kiem tra 2 khoang thoi gian co giao nhau khong (dung cho unit test)
        /// </summary>
        bool KiemTraGiaoNhau(DateTime batDauCu, DateTime ketThucCu,
                             DateTime batDauMoi, DateTime ketThucMoi);
    }
}