// ============================================================
// File: ViewModels/LoaiXe/LoaiXeListVM.cs
// Noi dung: ViewModel danh sach LoaiXe
// Sinh vien thuc hien: Pham Gia Minh Hoang - 23103100087 - SV1
// Module: Module 1
// ============================================================

using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.LoaiXe
{
    public class LoaiXeListVM
    {
        // Dung duong dan day du de tranh trung ten voi namespace hien tai
        public List<Models.Entities.LoaiXe> DanhSach { get; set; } = new();

        public string? TuKhoa { get; set; }
        public int? SoChoTu { get; set; }
        public int? SoChoDen { get; set; }
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