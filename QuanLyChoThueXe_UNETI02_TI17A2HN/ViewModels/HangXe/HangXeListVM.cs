// ============================================================
// File: ViewModels/HangXe/HangXeListVM.cs
// Noi dung: ViewModel danh sach HangXe
// Sinh vien thuc hien: Pham Gia Minh Hoang - 23103100087 - SV1
// Module: Module 1
// ============================================================

using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.HangXe
{
    public class HangXeListVM
    {
        // Dung duong dan day du de tranh trung ten voi namespace hien tai
        public List<Models.Entities.HangXe> DanhSach { get; set; } = new();

        public string? TuKhoa { get; set; }
        public string? QuocGia { get; set; }
        public bool? TrangThai { get; set; }
        public string? SapXepTheo { get; set; }
        public string? ThuTuSapXep { get; set; }

        public int Trang { get; set; } = 1;
        public int KichThuocTrang { get; set; } = 10;
        public int TongSoBanGhi { get; set; }
        public int TongSoTrang => (int)Math.Ceiling((double)TongSoBanGhi / KichThuocTrang);
        public bool CoTrangTruoc => Trang > 1;
        public bool CoTrangSau => Trang < TongSoTrang;
    }
}