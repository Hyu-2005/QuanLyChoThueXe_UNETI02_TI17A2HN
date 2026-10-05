// IThongKeService.cs - interface thong ke
// sinh vien thuc hien: Duong Lam Huy - 23103100120
// module 5 - tinh tien, thanh toan, lich su va thong ke

using QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.ThongKe;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Services.Interfaces
{
    public interface IThongKeService
    {
        Task<DashboardVM> LayDashboardAsync();
        Task<ThongKeVM> LayThongKeAsync(DateTime tuNgay, DateTime denNgay);
    }
}