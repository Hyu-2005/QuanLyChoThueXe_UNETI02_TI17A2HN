// ============================================================
// File: ViewModels/ThanhToan/ThanhToanVM.cs
// Nội dung: ViewModel cho màn hình thanh toán (có voucher + điểm)
// Sinh viên thực hiện: Dương Lâm Huy - 23103100120 - SV5
// Module: Module 5 - Tính tiền, Thanh toán, Lịch sử, Thống kê
// ============================================================

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.ThanhToan
{
    public class ThanhToanVM
    {
        public int MaDatXe { get; set; }

        // ===== Thông tin hiển thị =====
        public string? BienSo { get; set; }
        public string? TenXe { get; set; }
        public string? TenKhachHang { get; set; }
        public string? SoDienThoaiKhachHang { get; set; }
        public DateTime ThoiGianNhanDuKien { get; set; }
        public DateTime ThoiGianTraDuKien { get; set; }
        public DateTime? ThoiGianTraThucTe { get; set; }

        // ===== Tính toán =====
        public int SoNgayThue { get; set; }

        /// <summary>Tiền thuê gốc trước giảm giá</summary>
        public decimal TienThueGoc { get; set; }

        /// <summary>Tiền thuê sau khi trừ voucher + điểm</summary>
        public decimal TienThue { get; set; }

        public decimal TongPhuPhi { get; set; }
        public decimal TienCocDaThu { get; set; }
        public decimal TongThanhToan { get; set; }
        public decimal SoTienConLai { get; set; }

        // ===== Chi tiết phụ phí =====
        public decimal PhiQuaHan { get; set; }
        public decimal PhiVuotKm { get; set; }
        public decimal PhiNhienLieu { get; set; }
        public decimal PhiHuHong { get; set; }

        // ============================================================
        // BẮT ĐẦU CODE CỦA SV5: Dương Lâm Huy - 23103100120
        // Nội dung: Thêm field cho Voucher + Điểm thưởng
        // Module: Module 5 - Tính tiền, Thanh toán
        // ============================================================

        // ===== Voucher =====
        [Display(Name = "Mã giảm giá")]
        [StringLength(30, ErrorMessage = "Mã tối đa 30 ký tự")]
        public string? MaGiamGiaCode { get; set; }

        public decimal TienGiamVoucher { get; set; }
        public int? MaGiamGiaId { get; set; }
        public string? TenChuongTrinhVoucher { get; set; }

        // ===== Điểm thưởng =====
        [Display(Name = "Sử dụng điểm thưởng")]
        [Range(0, int.MaxValue, ErrorMessage = "Số điểm phải >= 0")]
        public int DiemSuDung { get; set; } = 0;

        public int DiemHienCo { get; set; }
        public decimal GiaTriMotDiem { get; set; }
        public int DiemToiDaCoTheDung { get; set; }
        public decimal TienTruTuDiem { get; set; }

        /// <summary>Điểm sẽ được cộng sau khi thanh toán thành công</summary>
        public int DiemDuocCongSauThanhToan { get; set; }

        // ============================================================
        // KẾT THÚC CODE CỦA SV5
        // ============================================================

        // ===== Nhập liệu thanh toán =====
        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        [Display(Name = "Phương thức thanh toán")]
        public string PhuongThucThanhToan { get; set; } = string.Empty;

        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        public List<SelectListItem> DanhSachPhuongThuc { get; set; } = new();

        public bool DaThanhToan { get; set; }
    }
}