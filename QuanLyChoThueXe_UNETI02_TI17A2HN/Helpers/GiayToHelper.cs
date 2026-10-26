// ============================================================
// File: Helpers/GiayToHelper.cs
// Họ và tên: Vũ Tiến Đạt
// Mã sinh viên: 23103100119
// Nội dung thực hiện: Kiểm tra khách hàng đã có CCCD và GPLX được xác minh (còn hạn) trước khi cho thuê xe
// Module: Module 3 - Khách hàng
// ============================================================

using Microsoft.EntityFrameworkCore;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Data;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Helpers
{
    public static class GiayToHelper
    {
        /// <summary>
        /// Khách chỉ được thuê xe khi CẢ HAI giấy tờ (CCCD, GPLX) đều ở trạng thái "Đã xác minh"
        /// và GPLX còn hạn đến hết ngày nhận xe.
        /// Trả về null nếu đủ điều kiện, ngược lại trả về thông báo lỗi tiếng Việt nêu rõ lý do.
        /// </summary>
        public static async Task<string?> KiemTraDieuKienThueXeAsync(
            AppDbContext context, int maKhachHang, DateTime? thoiDiemNhan = null)
        {
            var ngayNhan = (thoiDiemNhan ?? DateTime.Now).Date;

            var giayTos = await context.GiayToKhachHangs
                .AsNoTracking()
                .Where(g => g.MaKhachHang == maKhachHang)
                .ToListAsync();

            var cacLoi = new List<string>();

            foreach (var loai in LoaiGiayTo.TatCa)
            {
                var ten = LoaiGiayTo.GetDisplayName(loai);
                var giayTo = giayTos.FirstOrDefault(g => g.LoaiGiayTo == loai);

                if (giayTo == null)
                {
                    cacLoi.Add($"chưa tải {ten}");
                    continue;
                }

                if (giayTo.TrangThai == TrangThaiGiayTo.ChoKiemTra)
                {
                    cacLoi.Add($"{ten} đang chờ nhân viên kiểm tra");
                    continue;
                }

                if (giayTo.TrangThai == TrangThaiGiayTo.TuChoi)
                {
                    cacLoi.Add(string.IsNullOrWhiteSpace(giayTo.LyDoTuChoi)
                        ? $"{ten} bị từ chối"
                        : $"{ten} bị từ chối (lý do: {giayTo.LyDoTuChoi})");
                    continue;
                }

                // Đã xác minh: GPLX phải còn hạn tới ngày nhận xe
                if (loai == LoaiGiayTo.GPLX &&
                    (!giayTo.NgayHetHan.HasValue || giayTo.NgayHetHan.Value.Date < ngayNhan))
                {
                    cacLoi.Add($"{ten} đã hết hạn hoặc sẽ hết hạn trước ngày nhận xe");
                }
            }

            if (cacLoi.Count == 0) return null;

            return "Bạn chưa thể thuê xe: " + string.Join("; ", cacLoi)
                   + ". Vui lòng hoàn tất giấy tờ tại mục Giấy tờ của tôi.";
        }
    }
}
