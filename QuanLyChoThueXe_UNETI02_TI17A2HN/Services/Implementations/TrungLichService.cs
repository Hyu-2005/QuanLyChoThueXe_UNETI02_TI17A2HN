// ============================================================
// File: Services/Implementations/TrungLichService.cs
// Noi dung: Cai dat kiem tra trung lich xe bang EF Core + LINQ
// Sinh vien thuc hien: Vu Tien Dat - 23103100119 - SV3
// Module: Module 3 - Khach hang, Tim xe trong va Dat xe
// ============================================================

using Microsoft.EntityFrameworkCore;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Data;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Services.Interfaces;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Services.Implementations
{
    public class TrungLichService : ITrungLichService
    {
        private readonly AppDbContext _context;

        public TrungLichService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Kiem tra trung lich: 2 khoang thoi gian giao nhau khi va chi khi
        ///     BatDauMoi &lt; KetThucCu  AND  KetThucMoi &gt; BatDauCu
        ///
        /// Vi du:
        ///     Cu:  10h ---- 12h
        ///     Moi:      11h ---- 13h   -> trung (11 &lt; 12 AND 13 &gt; 10)
        ///     Moi:  12h ---- 14h       -> KHONG trung (12 &lt; 12 la false)
        /// </summary>
        public async Task<bool> KiemTraTrungLich(int maXe, DateTime batDauMoi,
            DateTime ketThucMoi, int? boQuaMaDatXe = null)
        {
            // 1. Validate khoang thoi gian moi
            if (ketThucMoi <= batDauMoi)
                throw new ArgumentException("Thoi gian tra phai sau thoi gian nhan");

            // 2. Truy van cac don dang chiem lich cua xe nay
            var query = _context.DatXes
                .Where(d => d.MaXe == maXe)
                .Where(d => TrangThaiDatXe.ChiemLich.Contains(d.TrangThai));

            // 3. Bo qua don cu (khi sua/duyet lai)
            if (boQuaMaDatXe.HasValue)
                query = query.Where(d => d.MaDatXe != boQuaMaDatXe.Value);

            // 4. Ap dung cong thuc giao nhau
            //    BatDauMoi < KetThucCu  AND  KetThucMoi > BatDauCu
            return await query.AnyAsync(d =>
                batDauMoi < d.ThoiGianTraDuKien &&
                ketThucMoi > d.ThoiGianNhanDuKien);
        }

        /// <summary>
        /// Lay danh sach cac don dang chiem lich cua xe.
        /// Dung cho man hinh xem lich xe hoac debug.
        /// </summary>
        public async Task<List<DatXe>> LayDonChiemLich(int maXe)
        {
            return await _context.DatXes
                .Include(d => d.KhachHang)
                .Where(d => d.MaXe == maXe)
                .Where(d => TrangThaiDatXe.ChiemLich.Contains(d.TrangThai))
                .OrderBy(d => d.ThoiGianNhanDuKien)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Kiem tra 2 khoang thoi gian co giao nhau khong (pure function, khong DB).
        /// Dung cho unit test 6 truong hop.
        /// </summary>
        public bool KiemTraGiaoNhau(DateTime batDauCu, DateTime ketThucCu,
            DateTime batDauMoi, DateTime ketThucMoi)
        {
            // Cong thuc giao nhau:
            //     BatDauMoi < KetThucCu  AND  KetThucMoi > BatDauCu
            return batDauMoi < ketThucCu && ketThucMoi > batDauCu;
        }
    }
}