// ============================================================
// File: ViewModels/DatXe/DatXeListVM.cs
// Noi dung: ViewModel danh sach don dat xe (khach hang + admin)
// Sinh vien thuc hien: Vu Tien Dat - 23103100119 - SV3
// Module: Module 3 - Khach hang, Tim xe trong va Dat xe
// ============================================================

using Microsoft.AspNetCore.Mvc.Rendering;
using DatXeEntity = QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities.DatXe;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.DatXe
{
    public class DatXeListVM
    {
        public List<DatXeEntity> DanhSach { get; set; } = new();

        // Che do: khach hang xem don cua minh, hay admin xem tat ca
        public bool LaKhachHang { get; set; } = false;

        // Tim kiem
        public string? TuKhoa { get; set; }      // ten KH, bien so, ten xe

        // Loc
        public string? TrangThai { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public int? MaLoaiXe { get; set; }

        // Sap xep
        public string? SapXepTheo { get; set; }   // "ngaydat" hoac "thoigiannhan"
        public string? ThuTuSapXep { get; set; }  // "asc" hoac "desc"

        // Phan trang
        public int Trang { get; set; } = 1;
        public int KichThuocTrang { get; set; } = 10;
        public int TongSoBanGhi { get; set; }
        public int TongSoTrang => (int)Math.Ceiling((double)TongSoBanGhi / KichThuocTrang);
        public bool CoTrangTruoc => Trang > 1;
        public bool CoTrangSau => Trang < TongSoTrang;

        // Dropdown cho bo loc
        public List<SelectListItem> DanhSachTrangThai { get; set; } = new();
        public List<SelectListItem> DanhSachLoaiXe { get; set; } = new();
    }
}