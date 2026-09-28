// File: ViewModels/DatXe/XeTrongVM.cs
// Noi dung: ViewModel 1 xe trong ket qua tim xe trong, kem gia ap dung tai thoi diem tim
// Module: Module 3 - Khach hang tim xe trong, dat xe, kiem tra trung lich

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.DatXe
{
    public class XeTrongVM
    {
        public int MaXe { get; set; }
        public string BienSo { get; set; } = string.Empty;
        public string TenXe { get; set; } = string.Empty;
        public string? AnhXe { get; set; }
        public string TenLoaiXe { get; set; } = string.Empty;
        public string TenHangXe { get; set; } = string.Empty;
        public int SoCho { get; set; }

        // Gia ap dung tai thoi diem tim kiem - se duoc dung lam SNAPSHOT khi dat xe
        public decimal DonGiaNgay { get; set; }
        public decimal DonGiaGio { get; set; }
        public decimal TienCocMacDinh { get; set; }

        /// <summary>Uoc tinh tong tien thue cho khoang thoi gian da tim (chi de hien thi tham khao)</summary>
        public decimal TongTienDuKien { get; set; }
    }
}