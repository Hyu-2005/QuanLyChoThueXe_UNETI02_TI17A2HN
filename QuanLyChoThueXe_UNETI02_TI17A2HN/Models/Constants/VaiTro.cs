namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants
{
    public static class VaiTro
    {
        public const string Admin = "Admin";
        public const string NhanVien = "NhanVien";
        public const string KhachHang = "KhachHang";

        public static readonly string[] TatCa = { Admin, NhanVien, KhachHang };

        public static string GetDisplayName(string vaiTro) => vaiTro switch
        {
            Admin => "Quản trị viên",
            NhanVien => "Nhân viên",
            KhachHang => "Khách hàng",
            _ => vaiTro
        };
    }
}
