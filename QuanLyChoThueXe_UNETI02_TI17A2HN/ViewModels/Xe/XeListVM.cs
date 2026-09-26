// ============================================================
// File: ViewModels/Xe/XeListVM.cs
// Noi dung: ViewModel danh sach Xe co Search/Filter/Sort/Pagination
// SV: Dao Gia Hung - 23103100065
// Module 2
// ============================================================

using Microsoft.AspNetCore.Mvc.Rendering;
using XeEntity = QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities.Xe;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.Xe
{
    public class XeListVM
    {
        // 👇 Dùng alias XeEntity để tránh trùng namespace "Xe"
        public List<XeEntity> DanhSach { get; set; } = new();

        // ===== Filter =====
        public string? TuKhoa { get; set; }
        public int? MaLoaiXe { get; set; }
        public int? MaHangXe { get; set; }
        public int? SoCho { get; set; }
        public string? TinhTrang { get; set; }
        public int? NamSanXuatTu { get; set; }
        public int? NamSanXuatDen { get; set; }
        public decimal? DonGiaTu { get; set; }
        public decimal? DonGiaDen { get; set; }

        // ===== Sort =====
        public string? SapXepTheo { get; set; }
        public string? ThuTuSapXep { get; set; }

        // ===== Phân trang =====
        public int Trang { get; set; } = 1;
        public int KichThuocTrang { get; set; } = 10;
        public int TongSoBanGhi { get; set; }
        public int TongSoTrang => (int)Math.Ceiling((double)TongSoBanGhi / KichThuocTrang);
        public bool CoTrangTruoc => Trang > 1;
        public bool CoTrangSau => Trang < TongSoTrang;

        // ===== Dropdown =====
        public List<SelectListItem> DanhSachLoaiXe { get; set; } = new();
        public List<SelectListItem> DanhSachHangXe { get; set; } = new();
        public List<SelectListItem> DanhSachTinhTrang { get; set; } = new();
    }
}