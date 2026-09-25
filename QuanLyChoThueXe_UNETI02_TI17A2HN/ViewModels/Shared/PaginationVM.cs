// File: ViewModels/Shared/PaginationVM.cs
// Noi dung: ViewModel phan trang dung chung
// Sinh vien thuc hien: Pham Gia Minh Hoang - 23103100087 - SV1
// Module: Module 1 - Tai khoan, Dang nhap, Phan quyen, Loai xe, Hang xe

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.Shared
{
    public class PaginationVM
    {
        public int Trang { get; set; }
        public int TongSoTrang { get; set; }
        public bool CoTrangTruoc => Trang > 1;
        public bool CoTrangSau => Trang < TongSoTrang;
        public string Action { get; set; } = "Index";
        public string Controller { get; set; } = "";
        public Dictionary<string, string?> QueryParams { get; set; } = new();
    }
}