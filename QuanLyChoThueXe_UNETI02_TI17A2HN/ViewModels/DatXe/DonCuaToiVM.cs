// File: ViewModels/DatXe/DonCuaToiVM.cs
// Noi dung: ViewModel danh sach don dat xe cua khach hang dang dang nhap
// Module: Module 3 - Khach hang tim xe trong, dat xe, kiem tra trung lich

using DatXeEntity = QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities.DatXe;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.DatXe
{
    public class DonCuaToiVM
    {
        public List<DatXeEntity> DanhSach { get; set; } = new();

        public string? TuKhoa { get; set; }
        public string? TrangThai { get; set; }

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