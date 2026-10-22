// ==============================================================
// Bắt Đầu Code của SV4: Nguyễn Hoàng Đức Hiếu
// file: Services/Interfaces/IEmailService.cs
// nội dung: STT 10 - Interface dịch vụ gửi Email thông báo tự động khi xét duyệt đơn đặt xe
// sinh viên thực hiện : Nguyễn Hoàng Đức Hiếu -  23103100116 - SV4 
// ==============================================================
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Services.Interfaces
{
    public interface IEmailService
    {
        /// <summary>
        /// Gửi email chúc mừng và xác nhận đặt xe thành công kèm thông tin chi tiết nhận xe
        /// </summary>
        Task GuiEmailXacNhanDatXeAsync(DatXe datXe);

        /// <summary>
        /// Gửi email thông báo từ chối đơn đặt xe kèm lý do cụ thể
        /// </summary>
        Task GuiEmailTuChoiDatXeAsync(DatXe datXe, string lyDoTuChoi);

        /// <summary>
        /// Gửi email thử nghiệm kết nối trực tiếp đến hòm thư cá nhân
        /// </summary>
        Task<bool> GuiEmailTestAsync(string? toEmail = null);
    }
}
// ==============================================================
// Kết thúc Code của SV4
// ==============================================================
