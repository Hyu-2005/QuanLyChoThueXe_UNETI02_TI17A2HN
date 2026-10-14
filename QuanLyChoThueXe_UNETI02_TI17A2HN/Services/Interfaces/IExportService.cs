// File: Services/Interfaces/IExportService.cs
// Nội dung: Interface xuất báo cáo Excel/PDF
// Sinh viên thực hiện: Dương Lâm Huy - 23103100120 - SV5
// Module: Module 5 - Tính tiền, Thanh toán, Lịch sử, Thống kê

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Services.Interfaces
{
    public interface IExportService
    {
        /// Xuất danh sách đơn đặt xe ra Excel
        byte[] XuatDanhSachDonExcel(List<Models.Entities.DatXe> danhSach);

        /// Xuất danh sách hóa đơn thanh toán ra Excel
        byte[] XuatDanhSachThanhToanExcel(List<Models.Entities.ThanhToan> danhSach);

        /// Xuất báo cáo thống kê ra PDF
        byte[] XuatBaoCaoThongKePdf(ViewModels.ThongKe.ThongKeVM thongKe);

        /// Xuất hóa đơn thanh toán ra PDF
        byte[] XuatHoaDonPdf(Models.Entities.ThanhToan thanhToan);
    }
}