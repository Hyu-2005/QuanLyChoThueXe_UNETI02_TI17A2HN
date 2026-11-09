// ============================================================
// File: ViewModels/MaGiamGia/MaGiamGiaListVM.cs
// Nội dung: ViewModel danh sách mã giảm giá
// Sinh viên thực hiện: Dương Lâm Huy - 23103100120 - SV5
// Module: Module 5 - Tính tiền, Thanh toán
// ============================================================

using MaGiamGiaEntity = QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities.MaGiamGia;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.MaGiamGia
{
    public class MaGiamGiaListVM
    {
        public List<MaGiamGiaEntity> DanhSach { get; set; } = new();

        public string? TuKhoa { get; set; }
        public string? LoaiGiamGia { get; set; }
        public string? TrangThaiLoc { get; set; } // "active" | "inactive" | "expired" | "sap_het"

        public int Trang { get; set; } = 1;
        public int KichThuocTrang { get; set; } = 10;
        public int TongSoBanGhi { get; set; }

        public int TongSoTrang => (int)Math.Ceiling((double)TongSoBanGhi / KichThuocTrang);
        public bool CoTrangTruoc => Trang > 1;
        public bool CoTrangSau => Trang < TongSoTrang;

        // Thống kê nhanh
        public int TongDangHoatDong { get; set; }
        public int TongDaHetHan { get; set; }
        public int TongSapHetLuot { get; set; }
    }
}