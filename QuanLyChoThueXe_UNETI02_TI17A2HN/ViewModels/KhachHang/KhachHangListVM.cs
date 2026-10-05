// KhachHangListVM.cs - viewmodel danh sach khach hang
// Họ và tên: Vu Tien Dat
// Mã sinh viên: 23103100119
// Nội dung thực hiện: ViewModel danh sách khách hàng
// Module: Module 3 - Khách hàng

using KhachHangEntity = QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities.KhachHang;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.KhachHang
{
    public class KhachHangListVM
    {
        public List<KhachHangEntity> DanhSach { get; set; } = new();
        public string? TuKhoa { get; set; }
        public bool? TrangThai { get; set; }

        public int Trang { get; set; } = 1;
        public int KichThuocTrang { get; set; } = 10;
        public int TongSoBanGhi { get; set; }
        public int TongSoTrang => (int)Math.Ceiling((double)TongSoBanGhi / KichThuocTrang);
        public bool CoTrangTruoc => Trang > 1;
        public bool CoTrangSau => Trang < TongSoTrang;
    }
}