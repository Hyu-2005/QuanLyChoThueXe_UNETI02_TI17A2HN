// ==============================================================
// Bắt Đầu Code của SV4: Nguyễn Hoàng Đức Hiếu
// file: Services/Implementations/EmailService.cs
// nội dung: STT 10 - Triển khai dịch vụ gửi Email thông báo tự động khi xét duyệt đơn đặt xe (hỗ trợ SMTP thực tế, gửi trực tiếp về Gmail cá nhân duchieu07122005@gmail.com & Mock an toàn)
// sinh viên thực hiện : Nguyễn Hoàng Đức Hiếu -  23103100116 - SV4 
// ==============================================================
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Services.Interfaces;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task GuiEmailXacNhanDatXeAsync(DatXe datXe)
        {
            if (datXe == null)
            {
                _logger.LogWarning("[EmailService] Đơn đặt xe null, không thể gửi email.");
                return;
            }

            // Đọc cấu hình chế độ gửi về email cá nhân của SV4 để phục vụ test và chấm điểm
            var cheDoMailCaNhan = bool.Parse(_config["EmailSettings:CheDoGuiVeMailCaNhan"] ?? "false");
            var emailCaNhan = _config["EmailSettings:EmailCaNhanNhanKetQua"] ?? "duchieu07122005@gmail.com";

            var toEmail = (cheDoMailCaNhan && !string.IsNullOrWhiteSpace(emailCaNhan))
                ? emailCaNhan
                : datXe.KhachHang?.Email;

            var toName = datXe.KhachHang?.HoTen ?? "Quý khách";

            if (string.IsNullOrWhiteSpace(toEmail))
            {
                _logger.LogWarning("[EmailService] Không thể gửi email duyệt đơn #{MaDatXe} do thiếu thông tin email người nhận.", datXe.MaDatXe);
                return;
            }

            var subject = $"[UNETI Car Rental] Xác nhận đặt xe thành công - Đơn #{datXe.MaDatXe}";

            // Banner hiển thị thông báo chuyển tiếp thư khi đang bật chế độ nhận email cá nhân
            var bannerCaNhan = (cheDoMailCaNhan && !string.IsNullOrWhiteSpace(emailCaNhan))
                ? $@"
                <div style=""background: #eff6ff; border: 2px dashed #3b82f6; border-radius: 8px; padding: 12px 16px; margin-bottom: 24px; color: #1e40af; font-size: 13px;"">
                    <strong style=""color: #1d4ed8;"">🔔 THÔNG BÁO NGHIỆM THU ĐỒ ÁN (SV4: Nguyễn Hoàng Đức Hiếu - 23103100116):</strong><br/>
                    Email này được tự động chuyển tiếp tới hòm thư cá nhân của bạn (<strong>{emailCaNhan}</strong>) để trực tiếp kiểm tra và demo tính năng.
                    (Email gốc trong hệ thống: <em>{datXe.KhachHang?.Email ?? "Không có"}</em>).
                </div>"
                : "";

            var bodyHtml = $@"
            <div style=""font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; max-width: 650px; margin: 0 auto; background: #ffffff; border: 1px solid #e2e8f0; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 12px rgba(0,0,0,0.06);"">
                <!-- HEADER -->
                <div style=""background: linear-gradient(135deg, #0b1220 0%, #1e293b 100%); color: #ffffff; padding: 28px 32px; text-align: center;"">
                    <h2 style=""margin: 0; font-size: 22px; text-transform: uppercase; letter-spacing: 1px; color: #38bdf8;"">CÔNG TY CHO THUÊ XE TỰ LÁI UNETI</h2>
                    <p style=""margin: 6px 0 0 0; font-size: 14px; opacity: 0.85;"">Dịch vụ cho thuê xe uy tín - an toàn - tiện lợi</p>
                </div>

                <!-- BODY -->
                <div style=""padding: 32px;"">
                    {bannerCaNhan}

                    <div style=""text-align: center; margin-bottom: 24px;"">
                        <span style=""display: inline-block; background: #dcfce7; color: #166534; font-weight: bold; padding: 6px 16px; border-radius: 999px; font-size: 13px;"">
                            ✓ ĐƠN ĐẶT XE ĐÃ ĐƯỢC PHÊ DUYỆT
                        </span>
                    </div>

                    <h3 style=""color: #0f172a; margin-top: 0;"">Kính gửi Quý khách: {toName},</h3>
                    <p style=""color: #475569; line-height: 1.6;"">
                        Chúng tôi xin trân trọng thông báo yêu cầu thuê xe của Quý khách cho đơn đặt mã <strong>#MDX-{datXe.MaDatXe}</strong> đã được phê duyệt thành công.
                    </p>

                    <!-- CHI TIẾT ĐƠN -->
                    <div style=""background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 8px; padding: 20px; margin: 24px 0;"">
                        <h4 style=""margin-top: 0; margin-bottom: 14px; color: #1e293b; border-bottom: 1px solid #cbd5e1; padding-bottom: 8px;"">
                            🚗 THÔNG TIN XE & LỊCH TRÌNH NHẬN XE
                        </h4>
                        <table style=""width: 100%; border-collapse: collapse; font-size: 14px; color: #334155;"">
                            <tr>
                                <td style=""padding: 6px 0; width: 40%; color: #64748b;"">Tên phương tiện:</td>
                                <td style=""padding: 6px 0; font-weight: 600; color: #0284c7;"">{datXe.Xe?.TenXe}</td>
                            </tr>
                            <tr>
                                <td style=""padding: 6px 0; color: #64748b;"">Biển số xe:</td>
                                <td style=""padding: 6px 0; font-weight: bold;"">{datXe.Xe?.BienSo}</td>
                            </tr>
                            <tr>
                                <td style=""padding: 6px 0; color: #64748b;"">Thời gian nhận dự kiến:</td>
                                <td style=""padding: 6px 0; font-weight: 600;"">{datXe.ThoiGianNhanDuKien:dd/MM/yyyy HH:mm}</td>
                            </tr>
                            <tr>
                                <td style=""padding: 6px 0; color: #64748b;"">Thời gian trả dự kiến:</td>
                                <td style=""padding: 6px 0; font-weight: 600;"">{datXe.ThoiGianTraDuKien:dd/MM/yyyy HH:mm}</td>
                            </tr>
                            <tr>
                                <td style=""padding: 6px 0; color: #64748b;"">Địa điểm nhận xe:</td>
                                <td style=""padding: 6px 0;"">{datXe.DiaDiemNhan}</td>
                            </tr>
                            <tr>
                                <td style=""padding: 6px 0; color: #64748b;"">Tiền cọc yêu cầu:</td>
                                <td style=""padding: 6px 0; font-weight: bold; color: #dc2626;"">{datXe.TienCoc:N0} VNĐ</td>
                            </tr>
                        </table>
                    </div>

                    <!-- LƯU Ý -->
                    <div style=""background: #fffbeb; border-left: 4px solid #f59e0b; padding: 14px 16px; border-radius: 4px; margin-bottom: 24px;"">
                        <strong style=""color: #b45309;"">📌 Lưu ý quan trọng khi nhận xe:</strong>
                        <ul style=""margin: 6px 0 0 0; padding-left: 20px; color: #78350f; font-size: 13.5px;"">
                            <li>Quý khách vui lòng mang theo <strong>Bản gốc Căn cước công dân (CCCD)</strong> và <strong>Giấy phép lái xe (GPLX)</strong> hợp lệ khi đến làm thủ tục bàn giao.</li>
                            <li>Chuẩn bị số tiền đặt cọc quy định nêu trên (bằng tiền mặt hoặc chuyển khoản QR).</li>
                            <li>Vui lòng có mặt đúng giờ nhận xe để nhân viên hỗ trợ bàn giao và kiểm tra kỹ thuật phương tiện.</li>
                        </ul>
                    </div>

                    <p style=""color: #475569; font-size: 14px; line-height: 1.6;"">
                        Mọi thắc mắc cần hỗ trợ, Quý khách vui lòng liên hệ hotline tổng đài <strong>1900 6868</strong> hoặc phản hồi trực tiếp email này.
                    </p>
                    <p style=""color: #0f172a; font-weight: 600; margin-bottom: 0;"">Trân trọng cảm ơn Quý khách!</p>
                </div>

                <!-- FOOTER -->
                <div style=""background: #f1f5f9; padding: 18px 32px; text-align: center; font-size: 12px; color: #64748b; border-top: 1px solid #e2e8f0;"">
                    © 2026 UNETI Car Rental System. Hệ thống quản lý cho thuê xe tự lái UNETI.<br>
                    Địa chỉ: 218 Lĩnh Nam, Quận Hoàng Mai, Hà Nội.
                </div>
            </div>";

            await SendEmailInternalAsync(toEmail, toName, subject, bodyHtml);
        }

        public async Task GuiEmailTuChoiDatXeAsync(DatXe datXe, string lyDoTuChoi)
        {
            if (datXe == null)
            {
                _logger.LogWarning("[EmailService] Đơn đặt xe null, không thể gửi email từ chối.");
                return;
            }

            var cheDoMailCaNhan = bool.Parse(_config["EmailSettings:CheDoGuiVeMailCaNhan"] ?? "false");
            var emailCaNhan = _config["EmailSettings:EmailCaNhanNhanKetQua"] ?? "duchieu07122005@gmail.com";

            var toEmail = (cheDoMailCaNhan && !string.IsNullOrWhiteSpace(emailCaNhan))
                ? emailCaNhan
                : datXe.KhachHang?.Email;

            var toName = datXe.KhachHang?.HoTen ?? "Quý khách";

            if (string.IsNullOrWhiteSpace(toEmail))
            {
                _logger.LogWarning("[EmailService] Không thể gửi email từ chối đơn #{MaDatXe} do thiếu email người nhận.", datXe.MaDatXe);
                return;
            }

            var subject = $"[UNETI Car Rental] Thông báo về yêu cầu đặt xe - Đơn #{datXe.MaDatXe}";

            var bannerCaNhan = (cheDoMailCaNhan && !string.IsNullOrWhiteSpace(emailCaNhan))
                ? $@"
                <div style=""background: #eff6ff; border: 2px dashed #3b82f6; border-radius: 8px; padding: 12px 16px; margin-bottom: 24px; color: #1e40af; font-size: 13px;"">
                    <strong style=""color: #1d4ed8;"">🔔 THÔNG BÁO NGHIỆM THU ĐỒ ÁN (SV4: Nguyễn Hoàng Đức Hiếu - 23103100116):</strong><br/>
                    Email này được tự động chuyển tiếp tới hòm thư cá nhân của bạn (<strong>{emailCaNhan}</strong>) để trực tiếp kiểm tra và demo tính năng.
                    (Email gốc trong hệ thống: <em>{datXe.KhachHang?.Email ?? "Không có"}</em>).
                </div>"
                : "";

            var bodyHtml = $@"
            <div style=""font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; max-width: 650px; margin: 0 auto; background: #ffffff; border: 1px solid #e2e8f0; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 12px rgba(0,0,0,0.06);"">
                <!-- HEADER -->
                <div style=""background: linear-gradient(135deg, #0b1220 0%, #1e293b 100%); color: #ffffff; padding: 28px 32px; text-align: center;"">
                    <h2 style=""margin: 0; font-size: 22px; text-transform: uppercase; letter-spacing: 1px; color: #f87171;"">CÔNG TY CHO THUÊ XE TỰ LÁI UNETI</h2>
                    <p style=""margin: 6px 0 0 0; font-size: 14px; opacity: 0.85;"">Thông báo cập nhật tình trạng yêu cầu đặt xe</p>
                </div>

                <!-- BODY -->
                <div style=""padding: 32px;"">
                    {bannerCaNhan}

                    <h3 style=""color: #0f172a; margin-top: 0;"">Kính gửi Quý khách: {toName},</h3>
                    <p style=""color: #475569; line-height: 1.6;"">
                        Cảm ơn Quý khách đã quan tâm và gửi yêu cầu thuê xe tại hệ thống của chúng tôi. Rất tiếc, đơn đặt xe mã <strong>#MDX-{datXe.MaDatXe}</strong> hiện tại chưa thể phê duyệt được.
                    </p>

                    <!-- LÝ DO TỪ CHỐI -->
                    <div style=""background: #fef2f2; border: 1px solid #fecaca; border-left: 4px solid #ef4444; border-radius: 6px; padding: 18px; margin: 24px 0;"">
                        <div style=""color: #991b1b; font-weight: bold; margin-bottom: 6px;"">Lý do chưa thể duyệt đơn:</div>
                        <div style=""color: #b91c1c; font-size: 14px; line-height: 1.5;"">
                            ""{lyDoTuChoi}""
                        </div>
                    </div>

                    <p style=""color: #475569; line-height: 1.6;"">
                        Quý khách có thể truy cập website để chọn phương tiện khác đang có sẵn hoặc điều chỉnh lại khung thời gian thuê xe phù hợp hơn. Đội ngũ nhân viên luôn sẵn sàng hỗ trợ Quý khách qua hotline <strong>1900 6868</strong>.
                    </p>

                    <p style=""color: #0f172a; font-weight: 600; margin-bottom: 0;"">Trân trọng cảm ơn và rất mong được phục vụ Quý khách trong các chuyến đi tới!</p>
                </div>

                <!-- FOOTER -->
                <div style=""background: #f1f5f9; padding: 18px 32px; text-align: center; font-size: 12px; color: #64748b; border-top: 1px solid #e2e8f0;"">
                    © 2026 UNETI Car Rental System. Hệ thống quản lý cho thuê xe tự lái UNETI.<br>
                    Địa chỉ: 218 Lĩnh Nam, Quận Hoàng Mai, Hà Nội.
                </div>
            </div>";

            await SendEmailInternalAsync(toEmail, toName, subject, bodyHtml);
        }

        public async Task<bool> GuiEmailTestAsync(string? toEmail = null)
        {
            var emailCaNhan = _config["EmailSettings:EmailCaNhanNhanKetQua"] ?? "duchieu07122005@gmail.com";
            var targetEmail = !string.IsNullOrWhiteSpace(toEmail) ? toEmail : emailCaNhan;
            var targetName = "Nguyễn Hoàng Đức Hiếu";

            var subject = "[UNETI Car Rental] Kiểm tra kết nối gửi Email hệ thống - SV4 Nguyễn Hoàng Đức Hiếu";
            var bodyHtml = $@"
            <div style=""font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; max-width: 650px; margin: 0 auto; background: #ffffff; border: 1px solid #e2e8f0; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 12px rgba(0,0,0,0.06);"">
                <div style=""background: linear-gradient(135deg, #0b1220 0%, #1e293b 100%); color: #ffffff; padding: 26px 32px; text-align: center;"">
                    <h2 style=""margin: 0; font-size: 22px; color: #38bdf8;"">HỆ THỐNG CHO THUÊ XE TỰ LÁI UNETI</h2>
                    <p style=""margin: 6px 0 0 0; font-size: 13.5px; opacity: 0.85;"">Kiểm thử tính năng gửi Email tự động - STT 10</p>
                </div>

                <div style=""padding: 32px;"">
                    <div style=""background: #ecfdf5; border-left: 4px solid #10b981; padding: 14px 16px; border-radius: 4px; margin-bottom: 22px;"">
                        <strong style=""color: #065f46; font-size: 15px;"">🎉 Kết nối SMTP Gmail cá nhân thành công!</strong>
                        <p style=""margin: 6px 0 0 0; color: #047857; font-size: 13.5px;"">
                            Hòm thư của bạn đã kết nối thông suốt với hệ thống và sẵn sàng nhận thông báo tự động mỗi khi duyệt hoặc từ chối đơn đặt xe.
                        </p>
                    </div>

                    <h4 style=""margin-top: 0; margin-bottom: 12px; color: #1e293b;"">📋 THÔNG TIN KIỂM THỬ TÍNH NĂNG</h4>
                    <table style=""width: 100%; border-collapse: collapse; font-size: 14px; color: #334155; margin-bottom: 22px;"">
                        <tr>
                            <td style=""padding: 7px 0; color: #64748b; width: 42%;"">Sinh viên thực hiện:</td>
                            <td style=""padding: 7px 0; font-weight: bold; color: #0f172a;"">Nguyễn Hoàng Đức Hiếu</td>
                        </tr>
                        <tr>
                            <td style=""padding: 7px 0; color: #64748b;"">Mã sinh viên (MSV):</td>
                            <td style=""padding: 7px 0; font-weight: bold; color: #0284c7;"">23103100116 (SV4 - Nhóm 2)</td>
                        </tr>
                        <tr>
                            <td style=""padding: 7px 0; color: #64748b;"">Lớp:</td>
                            <td style=""padding: 7px 0; font-weight: 600;"">Tin17A2HN</td>
                        </tr>
                        <tr>
                            <td style=""padding: 7px 0; color: #64748b;"">Hòm thư nhận thông báo:</td>
                            <td style=""padding: 7px 0; font-weight: 600; color: #166534;"">{targetEmail}</td>
                        </tr>
                        <tr>
                            <td style=""padding: 7px 0; color: #64748b;"">Thời điểm kiểm tra:</td>
                            <td style=""padding: 7px 0;"">{DateTime.Now:dd/MM/yyyy HH:mm:ss}</td>
                        </tr>
                    </table>

                    <div style=""background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 8px; padding: 14px 18px;"">
                        <strong style=""color: #334155; font-size: 13.5px;"">💡 Cách hoạt động khi demo:</strong>
                        <p style=""margin: 6px 0 0 0; color: #64748b; font-size: 13px; line-height: 1.5;"">
                            Bất cứ khi nào bạn nhấn <strong>Duyệt đơn</strong> hoặc <strong>Từ chối đơn</strong> trên phần mềm, hệ thống sẽ tự động gửi email xác nhận chi tiết về hòm thư này mà không phụ thuộc vào email khách hàng mẫu trong cơ sở dữ liệu.
                        </p>
                    </div>
                </div>

                <div style=""background: #f1f5f9; padding: 16px; text-align: center; font-size: 12px; color: #64748b; border-top: 1px solid #e2e8f0;"">
                    © 2026 UNETI Car Rental System - Module 4: Bàn giao xe & Gửi Email thông báo (SV4)
                </div>
            </div>";

            return await SendEmailInternalAsync(targetEmail, targetName, subject, bodyHtml);
        }

        private async Task<bool> SendEmailInternalAsync(string toEmail, string toName, string subject, string bodyHtml)
        {
            var smtpServer = _config["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
            var smtpPortStr = _config["EmailSettings:SmtpPort"] ?? "587";
            int.TryParse(smtpPortStr, out int smtpPort);
            if (smtpPort <= 0) smtpPort = 587;

            var senderEmail = _config["EmailSettings:SenderEmail"] ?? "duchieu07122005@gmail.com";
            var senderName = _config["EmailSettings:SenderName"] ?? "Hệ Thống Thuê Xe Tự Lái UNETI";
            var username = _config["EmailSettings:Username"] ?? senderEmail;
            var password = _config["EmailSettings:Password"];
            var enableSsl = bool.Parse(_config["EmailSettings:EnableSsl"] ?? "true");
            var moPhong = bool.Parse(_config["EmailSettings:MoPhongGhiLog"] ?? "false");

            // Chế độ mô phỏng / Mock an toàn khi test hoặc chưa điền Mật khẩu ứng dụng Gmail thực tế
            if (moPhong || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                _logger.LogInformation("\n================== [MOCK EMAIL SENT - SV4 NGUYEN HOANG DUC HIEU] ==================\n" +
                                        "Tới: {ToName} <{ToEmail}>\n" +
                                        "Tiêu đề: {Subject}\n" +
                                        "Thời gian: {Time:dd/MM/yyyy HH:mm:ss}\n" +
                                        "Trạng thái: Đã xử lý gửi qua luồng mô phỏng an toàn (Fail-safe)\n" +
                                        "Ghi chú: Để nhận email thật về hộp thư, bạn chỉ cần điền mật khẩu ứng dụng Gmail 16 ký tự vào appsettings.json (mục Password)\n" +
                                        "===================================================================================\n",
                                        toName, toEmail, subject, DateTime.Now);
                return false;
            }

            try
            {
                using var client = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = enableSsl,
                    Timeout = 12000 // 12s timeout
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = subject,
                    Body = bodyHtml,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(new MailAddress(toEmail, toName));
                await client.SendMailAsync(mailMessage);

                _logger.LogInformation("[EmailService] Đã gửi email thực tế thành công tới {ToEmail}", toEmail);
                return true;
            }
            catch (Exception ex)
            {
                // Bắt ngoại lệ an toàn để không làm đơ hoặc gián đoạn luồng duyệt đơn của nhân viên
                _logger.LogError(ex, "[EmailService] Lỗi khi gửi email SMTP tới {ToEmail}. Chi tiết: {Message}", toEmail, ex.Message);
                return false;
            }
        }
    }
}
// ==============================================================
// Kết thúc Code của SV4
// ==============================================================
