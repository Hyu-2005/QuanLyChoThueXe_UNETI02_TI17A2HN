// ============================================================
// File: Services/Implementations/XeTrongService.cs
// Noi dung: Cai dat tim xe trong bang EF Core + LINQ
// Sinh vien thuc hien: Vu Tien Dat - 23103100119 - SV3
// Module: Module 3 - Khach hang, Tim xe trong va Dat xe
// ============================================================

using Microsoft.EntityFrameworkCore;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Data;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Services.Interfaces;
using QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.DatXe;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Services.Implementations
{
    public class XeTrongService : IXeTrongService
    {
        private readonly AppDbContext _context;

        public XeTrongService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<XeTrongItemVM>> TimXeTrongAsync(
            DateTime thoiGianNhan,
            DateTime thoiGianTra,
            int? maLoaiXe = null,
            int? maHangXe = null,
            int? soCho = null,
            decimal? donGiaTu = null,
            decimal? donGiaDen = null)
        {
            if (thoiGianTra <= thoiGianNhan)
                throw new ArgumentException("Thoi gian tra phai sau thoi gian nhan");

            // 1. Truy van xe co the khai thac (San sang hoac Dang giu cho)
            var query = _context.Xes
                .Include(x => x.LoaiXe)
                .Include(x => x.HangXe)
                .Where(x => TinhTrangXe.CoTheChoThue.Contains(x.TinhTrang))
                .AsQueryable();

            // 2. Loc theo loai xe
            if (maLoaiXe.HasValue)
                query = query.Where(x => x.MaLoaiXe == maLoaiXe.Value);

            // 3. Loc theo hang xe
            if (maHangXe.HasValue)
                query = query.Where(x => x.MaHangXe == maHangXe.Value);

            // 4. Loc theo so cho
            if (soCho.HasValue)
                query = query.Where(x => x.SoCho == soCho.Value);

            // 5. Loai bo xe co don chiem lich giao voi khoang yeu cau
            //    Cong thuc giao nhau: BatDauMoi < KetThucCu AND KetThucMoi > BatDauCu
            query = query.Where(x => !_context.DatXes.Any(d =>
                d.MaXe == x.MaXe &&
                TrangThaiDatXe.ChiemLich.Contains(d.TrangThai) &&
                thoiGianNhan < d.ThoiGianTraDuKien &&
                thoiGianTra > d.ThoiGianNhanDuKien));

            // 6. Lay danh sach xe
            var danhSachXe = await query
                .OrderBy(x => x.TenXe)
                .AsNoTracking()
                .ToListAsync();

            // 7. Lay bang gia hieu luc cho tung xe
            var now = DateTime.Now;
            var ketQua = new List<XeTrongItemVM>();

            foreach (var xe in danhSachXe)
            {
                // Uu tien bang gia rieng cho xe (MaXe), neu khong co thi lay bang gia theo loai
                var bangGia = await _context.BangGiaThues
                    .Where(b => b.TrangThai)
                    .Where(b => b.TuNgay <= thoiGianNhan && b.DenNgay >= thoiGianTra)
                    .Where(b => b.MaXe == xe.MaXe || b.MaLoaiXe == xe.MaLoaiXe)
                    .OrderByDescending(b => b.MaXe.HasValue)   // uu tien gia rieng xe
                    .FirstOrDefaultAsync();

                // Bo qua xe khong co bang gia hieu luc
                if (bangGia == null) continue;

                // Loc theo khoang gia (tinh tren DonGiaNgay)
                if (donGiaTu.HasValue && bangGia.DonGiaNgay < donGiaTu.Value) continue;
                if (donGiaDen.HasValue && bangGia.DonGiaNgay > donGiaDen.Value) continue;

                // Tinh so ngay thue (lam tron len)
                var soNgay = Math.Max(1,
                    (int)Math.Ceiling((thoiGianTra - thoiGianNhan).TotalDays));

                ketQua.Add(new XeTrongItemVM
                {
                    MaXe = xe.MaXe,
                    BienSo = xe.BienSo,
                    TenXe = xe.TenXe,
                    TenLoaiXe = xe.LoaiXe?.TenLoaiXe ?? "-",
                    TenHangXe = xe.HangXe?.TenHangXe ?? "-",
                    SoCho = xe.SoCho,
                    NamSanXuat = xe.NamSanXuat,
                    AnhXe = xe.AnhXe,
                    DonGiaNgay = bangGia.DonGiaNgay,
                    DonGiaGio = bangGia.DonGiaGio,
                    TienCocMacDinh = bangGia.TienCocMacDinh,
                    SoNgayThue = soNgay,
                    TienThueDuKien = soNgay * bangGia.DonGiaNgay
                });
            }

            return ketQua;
        }
    }
}