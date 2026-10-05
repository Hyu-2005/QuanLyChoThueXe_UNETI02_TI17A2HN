// TinhTienService.cs - cai dat tinh tien thue va thanh toan
// sinh vien thuc hien: Duong Lam Huy - 23103100120
// module 5 - tinh tien, thanh toan, lich su va thong ke

using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Services.Interfaces;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Services.Implementations
{
    public class TinhTienService : ITinhTienService
    {
        /// <summary>
        /// Tinh so ngay thue: lam tron len, toi thieu 1 ngay
        /// Vi du: 1.5 ngay -> 2 ngay; 0.5 ngay -> 1 ngay
        /// </summary>
        public int TinhSoNgayThue(DateTime thoiGianNhan, DateTime thoiGianTra)
        {
            if (thoiGianTra <= thoiGianNhan) return 1;

            var soNgay = (int)Math.Ceiling((thoiGianTra - thoiGianNhan).TotalDays);
            return Math.Max(1, soNgay);
        }

        /// <summary>
        /// Tinh tien thue = SoNgayThue * DonGiaApDung
        /// DonGiaApDung da duoc luu o thoi diem dat/duyet de bao toan lich su
        /// </summary>
        public decimal TinhTienThue(DatXe datXe)
        {
            var soNgay = TinhSoNgayThue(datXe.ThoiGianNhanDuKien, datXe.ThoiGianTraDuKien);
            return soNgay * datXe.DonGiaApDung;
        }

        /// <summary>
        /// Tinh toan day du: tien thue + phu phi - tien coc = so tien con lai
        /// </summary>
        public ThanhToanResult TinhThanhToan(DatXe datXe, TraXe? traXe)
        {
            var soNgay = TinhSoNgayThue(datXe.ThoiGianNhanDuKien, datXe.ThoiGianTraDuKien);
            var tienThue = soNgay * datXe.DonGiaApDung;
            var tongPhuPhi = traXe?.TongPhuPhi ?? 0;
            var tongThanhToan = tienThue + tongPhuPhi;
            var soTienConLai = tongThanhToan - datXe.TienCoc;

            return new ThanhToanResult
            {
                SoNgayThue = soNgay,
                TienThue = tienThue,
                TongPhuPhi = tongPhuPhi,
                TienCocDaThu = datXe.TienCoc,
                TongThanhToan = tongThanhToan,
                SoTienConLai = soTienConLai < 0 ? 0 : soTienConLai
            };
        }
    }
}