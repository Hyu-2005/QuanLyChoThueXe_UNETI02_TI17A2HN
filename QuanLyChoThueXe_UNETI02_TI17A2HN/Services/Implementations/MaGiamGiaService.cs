// ============================================================
// File: Services/Implementations/MaGiamGiaService.cs
// Nội dung: Cài đặt quản lý voucher
// Sinh viên thực hiện: Dương Lâm Huy - 23103100120 - SV5
// Module: Module 5 - Tính tiền, Thanh toán
// ============================================================

using Microsoft.EntityFrameworkCore;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Data;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Services.Interfaces;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Services.Implementations
{
    public class MaGiamGiaService : IMaGiamGiaService
    {
        private readonly AppDbContext _context;

        public MaGiamGiaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(MaGiamGia? voucher, string? loi)> TimVaKiemTraAsync(
            string? code, int maKhachHang, decimal tienThueGoc)
        {
            // 1. Code rỗng → không áp dụng voucher
            if (string.IsNullOrWhiteSpace(code))
                return (null, null);

            code = code.Trim().ToUpperInvariant();

            // 2. Tìm voucher
            var voucher = await _context.MaGiamGias
                .FirstOrDefaultAsync(v => v.Code.ToUpper() == code);

            if (voucher == null)
                return (null, "Mã giảm giá không tồn tại");

            // 3. Kiểm tra trạng thái
            if (!voucher.TrangThai)
                return (null, "Mã giảm giá đã bị vô hiệu hóa");

            // 4. Kiểm tra thời gian hiệu lực
            if (DateTime.Now < voucher.NgayBatDau)
                return (null, $"Mã chỉ có hiệu lực từ {voucher.NgayBatDau:dd/MM/yyyy HH:mm}");

            if (DateTime.Now > voucher.NgayKetThuc)
                return (null, $"Mã đã hết hạn từ {voucher.NgayKetThuc:dd/MM/yyyy HH:mm}");

            // 5. Kiểm tra số lượt
            if (voucher.SoLuotDaDung >= voucher.SoLuotDungToiDa)
                return (null, "Mã đã hết lượt sử dụng");

            // 6. Kiểm tra đơn tối thiểu
            if (tienThueGoc < voucher.DonHangToiThieu)
                return (null, $"Đơn hàng tối thiểu {voucher.DonHangToiThieu:N0}đ mới dùng được mã này");

            // 7. Kiểm tra mỗi khách 1 lần
            if (voucher.MoiKhachMotLan)
            {
                var daDung = await _context.SuDungMaGiamGias
                    .AnyAsync(s => s.MaGiamGiaId == voucher.MaGiamGiaId
                                && s.MaKhachHang == maKhachHang);
                if (daDung)
                    return (null, "Bạn đã sử dụng mã này rồi");
            }

            return (voucher, null);
        }

        public async Task GhiNhanSuDungAsync(
            MaGiamGia voucher,
            int maKhachHang,
            int maDatXe,
            decimal soTienGiam)
        {
            voucher.SoLuotDaDung++;

            _context.SuDungMaGiamGias.Add(new SuDungMaGiamGia
            {
                MaGiamGiaId = voucher.MaGiamGiaId,
                MaKhachHang = maKhachHang,
                MaDatXe = maDatXe,
                SoTienDuocGiam = soTienGiam,   // ✅ ĐÚNG (trước đó ghi nhầm SoTienGiam)
                NgaySuDung = DateTime.Now
            });

            await _context.SaveChangesAsync();
        }

        public async Task<bool> KhachDaDungVoucherAsync(int maGiamGiaId, int maKhachHang)
        {
            return await _context.SuDungMaGiamGias
                .AnyAsync(s => s.MaGiamGiaId == maGiamGiaId
                            && s.MaKhachHang == maKhachHang);
        }
    }
}