// ============================================================
// File: ViewModels/GiayTo/GiayToDuyetVM.cs
// Họ và tên: [Vũ Tiến Đạt]
// Mã sinh viên: [23103100119]
// Nội dung thực hiện: ViewModel danh sách giấy tờ để nhân viên kiểm tra
// Module: Module 3 - Khách hàng
// ============================================================

using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.GiayTo
{
    public class GiayToDuyetVM
    {
        public List<GiayToKhachHang> DanhSach { get; set; } = new();
        public string? TrangThai { get; set; }
        public string? TuKhoa { get; set; }
    }
}
