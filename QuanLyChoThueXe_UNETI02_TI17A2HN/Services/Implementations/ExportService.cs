// File: Services/Implementations/ExportService.cs
// Nội dung: Triển khai xuất báo cáo Excel (ClosedXML) và PDF (QuestPDF)
// Sinh viên thực hiện: Dương Lâm Huy - 23103100120 - SV5
// Module: Module 5 - Tính tiền, Thanh toán, Lịch sử, Thống kê

using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Services.Interfaces;
using QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.ThongKe;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Services.Implementations
{
    public class ExportService : IExportService
    {
        // =====================================================
        // EXCEL: DANH SÁCH ĐƠN ĐẶT XE
        // =====================================================
        public byte[] XuatDanhSachDonExcel(List<DatXe> danhSach)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("DanhSachDon");

            // ===== TIÊU ĐỀ =====
            ws.Cell(1, 1).Value = "DANH SÁCH ĐƠN ĐẶT XE";
            ws.Range(1, 1, 1, 10).Merge();
            ws.Cell(1, 1).Style.Font.Bold = true;
            ws.Cell(1, 1).Style.Font.FontSize = 16;
            ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(1, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#1E3A8A");
            ws.Cell(1, 1).Style.Font.FontColor = XLColor.White;
            ws.Row(1).Height = 30;

            // ===== NGÀY XUẤT =====
            ws.Cell(2, 1).Value = $"Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm}";
            ws.Range(2, 1, 2, 10).Merge();
            ws.Cell(2, 1).Style.Font.Italic = true;
            ws.Cell(2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // ===== HEADER =====
            var headers = new[]
            {
                "STT", "Mã đơn", "Khách hàng", "SĐT",
                "Xe", "Biển số", "Nhận dự kiến", "Trả dự kiến",
                "Đơn giá", "Trạng thái"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(4, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#3B82F6");
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.OutsideBorderColor = XLColor.White;
            }
            ws.Row(4).Height = 22;

            // ===== DỮ LIỆU =====
            int row = 5;
            int stt = 1;
            foreach (var don in danhSach)
            {
                ws.Cell(row, 1).Value = stt++;
                ws.Cell(row, 2).Value = don.MaDatXe;
                ws.Cell(row, 3).Value = don.KhachHang?.HoTen ?? "-";
                ws.Cell(row, 4).Value = don.KhachHang?.SoDienThoai ?? "-";
                ws.Cell(row, 5).Value = don.Xe?.TenXe ?? "-";
                ws.Cell(row, 6).Value = don.Xe?.BienSo ?? "-";
                ws.Cell(row, 7).Value = don.ThoiGianNhanDuKien.ToString("dd/MM/yyyy HH:mm");
                ws.Cell(row, 8).Value = don.ThoiGianTraDuKien.ToString("dd/MM/yyyy HH:mm");
                ws.Cell(row, 9).Value = don.DonGiaApDung;
                ws.Cell(row, 9).Style.NumberFormat.Format = "#,##0";
                ws.Cell(row, 10).Value = don.TrangThai;

                // Border
                ws.Range(row, 1, row, 10).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Range(row, 1, row, 10).Style.Border.OutsideBorderColor = XLColor.FromHtml("#E5E7EB");

                // Xen kẽ màu
                if (stt % 2 == 0)
                {
                    ws.Range(row, 1, row, 10).Style.Fill.BackgroundColor = XLColor.FromHtml("#F9FAFB");
                }

                // Căn giữa cột số
                ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(row, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                row++;
            }

            // ===== ĐỊNH DẠNG CỘT =====
            ws.Columns().AdjustToContents();
            ws.Column(1).Width = 6;   // STT
            ws.Column(3).Width = 22;  // Tên KH
            ws.Column(5).Width = 25;  // Tên xe

            // ===== FREEZE HEADER =====
            ws.SheetView.FreezeRows(4);

            // ===== AUTO FILTER =====
            ws.Range(4, 1, row - 1, 10).SetAutoFilter();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        // =====================================================
        // EXCEL: DANH SÁCH THANH TOÁN
        // =====================================================
        public byte[] XuatDanhSachThanhToanExcel(List<ThanhToan> danhSach)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("ThanhToan");

            // ===== TIÊU ĐỀ =====
            ws.Cell(1, 1).Value = "DANH SÁCH HÓA ĐƠN THANH TOÁN";
            ws.Range(1, 1, 1, 11).Merge();
            ws.Cell(1, 1).Style.Font.Bold = true;
            ws.Cell(1, 1).Style.Font.FontSize = 16;
            ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(1, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#059669");
            ws.Cell(1, 1).Style.Font.FontColor = XLColor.White;
            ws.Row(1).Height = 30;

            ws.Cell(2, 1).Value = $"Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm}";
            ws.Range(2, 1, 2, 11).Merge();
            ws.Cell(2, 1).Style.Font.Italic = true;
            ws.Cell(2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // ===== HEADER =====
            var headers = new[]
            {
                "STT", "Mã HĐ", "Khách hàng", "Xe", "Biển số",
                "Tiền thuê", "Phụ phí", "Tiền cọc", "Tổng TT",
                "Phương thức", "Trạng thái"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(4, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#10B981");
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }
            ws.Row(4).Height = 22;

            // ===== DỮ LIỆU =====
            int row = 5;
            int stt = 1;
            foreach (var tt in danhSach)
            {
                ws.Cell(row, 1).Value = stt++;
                ws.Cell(row, 2).Value = tt.MaThanhToan;
                ws.Cell(row, 3).Value = tt.DatXe?.KhachHang?.HoTen ?? "-";
                ws.Cell(row, 4).Value = tt.DatXe?.Xe?.TenXe ?? "-";
                ws.Cell(row, 5).Value = tt.DatXe?.Xe?.BienSo ?? "-";
                ws.Cell(row, 6).Value = tt.TienThue;
                ws.Cell(row, 7).Value = tt.TongPhuPhi;
                ws.Cell(row, 8).Value = tt.TienCocDaThu;
                ws.Cell(row, 9).Value = tt.TongThanhToan;
                ws.Cell(row, 10).Value = tt.PhuongThucThanhToan;
                ws.Cell(row, 11).Value = tt.TrangThaiThanhToan;

                // Format tiền
                for (int c = 6; c <= 9; c++)
                {
                    ws.Cell(row, c).Style.NumberFormat.Format = "#,##0";
                    ws.Cell(row, c).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }

                ws.Range(row, 1, row, 11).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Range(row, 1, row, 11).Style.Border.OutsideBorderColor = XLColor.FromHtml("#E5E7EB");

                if (stt % 2 == 0)
                    ws.Range(row, 1, row, 11).Style.Fill.BackgroundColor = XLColor.FromHtml("#F9FAFB");

                row++;
            }

            // ===== TỔNG CỘNG =====
            ws.Cell(row, 5).Value = "TỔNG CỘNG:";
            ws.Cell(row, 5).Style.Font.Bold = true;
            ws.Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            ws.Cell(row, 6).FormulaA1 = $"SUM(F5:F{row - 1})";
            ws.Cell(row, 7).FormulaA1 = $"SUM(G5:G{row - 1})";
            ws.Cell(row, 8).FormulaA1 = $"SUM(H5:H{row - 1})";
            ws.Cell(row, 9).FormulaA1 = $"SUM(I5:I{row - 1})";

            for (int c = 6; c <= 9; c++)
            {
                ws.Cell(row, c).Style.Font.Bold = true;
                ws.Cell(row, c).Style.NumberFormat.Format = "#,##0";
                ws.Cell(row, c).Style.Fill.BackgroundColor = XLColor.FromHtml("#FEF3C7");
                ws.Cell(row, c).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            }

            ws.Columns().AdjustToContents();
            ws.SheetView.FreezeRows(4);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        // =====================================================
        // PDF: BÁO CÁO THỐNG KÊ
        // =====================================================
        public byte[] XuatBaoCaoThongKePdf(ThongKeVM thongKe)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    // ===== HEADER =====
                    page.Header().Column(col =>
                    {
                        col.Item().AlignCenter().Text("HỆ THỐNG QUẢN LÝ CHO THUÊ XE")
                            .FontSize(20).Bold().FontColor("#1E3A8A");
                        col.Item().AlignCenter().Text("UNETI02 - Lớp TI17A2HN")
                            .FontSize(11).FontColor("#64748B");
                        col.Item().PaddingTop(10).LineHorizontal(2).LineColor("#3B82F6");
                        col.Item().PaddingTop(15).AlignCenter()
                            .Text("BÁO CÁO THỐNG KÊ HOẠT ĐỘNG")
                            .FontSize(16).Bold().FontColor("#0F172A");
                        col.Item().AlignCenter()
                            .Text($"Kỳ báo cáo: {thongKe.TuNgay:dd/MM/yyyy} - {thongKe.DenNgay:dd/MM/yyyy}")
                            .FontSize(11).Italic().FontColor("#64748B");
                        col.Item().PaddingTop(5).AlignRight()
                            .Text($"Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm}")
                            .FontSize(9).FontColor("#94A3B8");
                    });

                    // ===== CONTENT =====
                    page.Content().PaddingVertical(15).Column(col =>
                    {
                        col.Spacing(15);

                        // ----- 1. TỔNG QUAN -----
                        col.Item().Text("1. TỔNG QUAN").FontSize(14).Bold().FontColor("#1E3A8A");

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn();
                                c.RelativeColumn();
                                c.RelativeColumn();
                                c.RelativeColumn();
                            });

                            // Header
                            table.Header(header =>
                            {
                                header.Cell().Background("#EFF6FF").Padding(8)
                                    .Text("Chỉ số").Bold();
                                header.Cell().Background("#EFF6FF").Padding(8)
                                    .Text("Giá trị").Bold();
                                header.Cell().Background("#EFF6FF").Padding(8)
                                    .Text("Chỉ số").Bold();
                                header.Cell().Background("#EFF6FF").Padding(8)
                                    .Text("Giá trị").Bold();
                            });

                            // Row 1
                            table.Cell().BorderBottom(1).BorderColor("#E5E7EB").Padding(8).Text("Tổng số đơn");
                            table.Cell().BorderBottom(1).BorderColor("#E5E7EB").Padding(8)
                                .Text($"{thongKe.TongSoDon}").Bold();
                            table.Cell().BorderBottom(1).BorderColor("#E5E7EB").Padding(8).Text("Tổng doanh thu");
                            table.Cell().BorderBottom(1).BorderColor("#E5E7EB").Padding(8)
                                .Text($"{thongKe.TongDoanhThu:N0} đ").Bold().FontColor("#059669");

                            // Row 2
                            table.Cell().BorderBottom(1).BorderColor("#E5E7EB").Padding(8).Text("Đơn hủy");
                            table.Cell().BorderBottom(1).BorderColor("#E5E7EB").Padding(8)
                                .Text($"{thongKe.SoDonHuy}");
                            table.Cell().BorderBottom(1).BorderColor("#E5E7EB").Padding(8).Text("Đơn từ chối");
                            table.Cell().BorderBottom(1).BorderColor("#E5E7EB").Padding(8)
                                .Text($"{thongKe.SoDonTuChoi}");

                            // Row 3
                            table.Cell().BorderBottom(1).BorderColor("#E5E7EB").Padding(8).Text("Tỷ lệ hủy");
                            table.Cell().BorderBottom(1).BorderColor("#E5E7EB").Padding(8)
                                .Text($"{thongKe.TyLeHuy:N2}%").FontColor("#DC2626");
                            table.Cell().BorderBottom(1).BorderColor("#E5E7EB").Padding(8).Text("Tỷ lệ khai thác");
                            table.Cell().BorderBottom(1).BorderColor("#E5E7EB").Padding(8)
                                .Text($"{thongKe.TyLeKhaiThac:N2}%").FontColor("#1D4ED8");

                            // Row 4
                            table.Cell().BorderBottom(1).BorderColor("#E5E7EB").Padding(8).Text("Tổng phụ phí");
                            table.Cell().BorderBottom(1).BorderColor("#E5E7EB").Padding(8)
                                .Text($"{thongKe.TongPhuPhi:N0} đ").FontColor("#B45309");
                            table.Cell().BorderBottom(1).BorderColor("#E5E7EB").Padding(8).Text("Thời lượng TB");
                            table.Cell().BorderBottom(1).BorderColor("#E5E7EB").Padding(8)
                                .Text($"{thongKe.ThoiLuongThueTrungBinh:N1} ngày");
                        });

                        // ----- 2. TOP XE -----
                        col.Item().PaddingTop(5).Text("2. TOP XE ĐƯỢC THUÊ NHIỀU NHẤT")
                            .FontSize(14).Bold().FontColor("#1E3A8A");

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(40);   // Hạng
                                c.RelativeColumn(2);    // Biển số
                                c.RelativeColumn(3);    // Tên xe
                                c.RelativeColumn(2);    // Số lần
                                c.RelativeColumn(3);    // Doanh thu
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background("#1E3A8A").Padding(6)
                                    .Text("Hạng").FontColor("#FFF").Bold();
                                header.Cell().Background("#1E3A8A").Padding(6)
                                    .Text("Biển số").FontColor("#FFF").Bold();
                                header.Cell().Background("#1E3A8A").Padding(6)
                                    .Text("Tên xe").FontColor("#FFF").Bold();
                                header.Cell().Background("#1E3A8A").Padding(6)
                                    .Text("Số lần thuê").FontColor("#FFF").Bold();
                                header.Cell().Background("#1E3A8A").Padding(6)
                                    .Text("Doanh thu").FontColor("#FFF").Bold();
                            });

                            int rank = 1;
                            foreach (var xe in thongKe.TopXeThueNhieu.Take(10))
                            {
                                var bgColor = rank <= 3 ? "#FEF3C7" : "#FFF";
                                table.Cell().Background(bgColor).BorderBottom(1).BorderColor("#E5E7EB").Padding(6)
                                    .Text($"{rank}").Bold().FontColor(rank <= 3 ? "#92400E" : "#0F172A");
                                table.Cell().Background(bgColor).BorderBottom(1).BorderColor("#E5E7EB").Padding(6)
                                    .Text(xe.BienSo ?? "-");
                                table.Cell().Background(bgColor).BorderBottom(1).BorderColor("#E5E7EB").Padding(6)
                                    .Text(xe.TenXe ?? "-");
                                table.Cell().Background(bgColor).BorderBottom(1).BorderColor("#E5E7EB").Padding(6)
                                    .Text($"{xe.SoLanThue}");
                                table.Cell().Background(bgColor).BorderBottom(1).BorderColor("#E5E7EB").Padding(6)
                                    .Text($"{xe.DoanhThu:N0} đ");
                                rank++;
                            }
                        });

                        // ----- 3. TOP KHÁCH HÀNG -----
                        col.Item().PaddingTop(5).Text("3. TOP KHÁCH HÀNG THUÊ NHIỀU NHẤT")
                            .FontSize(14).Bold().FontColor("#1E3A8A");

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(40);
                                c.RelativeColumn(3);
                                c.RelativeColumn(2);
                                c.RelativeColumn(2);
                                c.RelativeColumn(3);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background("#059669").Padding(6)
                                    .Text("Hạng").FontColor("#FFF").Bold();
                                header.Cell().Background("#059669").Padding(6)
                                    .Text("Khách hàng").FontColor("#FFF").Bold();
                                header.Cell().Background("#059669").Padding(6)
                                    .Text("SĐT").FontColor("#FFF").Bold();
                                header.Cell().Background("#059669").Padding(6)
                                    .Text("Số lần thuê").FontColor("#FFF").Bold();
                                header.Cell().Background("#059669").Padding(6)
                                    .Text("Tổng chi tiêu").FontColor("#FFF").Bold();
                            });

                            int rankKh = 1;
                            foreach (var kh in thongKe.TopKhachHang.Take(10))
                            {
                                table.Cell().BorderBottom(1).BorderColor("#E5E7EB").Padding(6)
                                    .Text($"{rankKh}").Bold();
                                table.Cell().BorderBottom(1).BorderColor("#E5E7EB").Padding(6)
                                    .Text(kh.HoTen ?? "-");
                                table.Cell().BorderBottom(1).BorderColor("#E5E7EB").Padding(6)
                                    .Text(kh.SoDienThoai ?? "-");
                                table.Cell().BorderBottom(1).BorderColor("#E5E7EB").Padding(6)
                                    .Text($"{kh.SoLanThue}");
                                table.Cell().BorderBottom(1).BorderColor("#E5E7EB").Padding(6)
                                    .Text($"{kh.TongChiTieu:N0} đ").FontColor("#059669");
                                rankKh++;
                            }
                        });

                        // ----- 4. PHỤ PHÍ -----
                        col.Item().PaddingTop(5).Text("4. TỔNG PHỤ PHÍ THEO LOẠI")
                            .FontSize(14).Bold().FontColor("#1E3A8A");

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(3);
                                c.RelativeColumn(2);
                                c.RelativeColumn(3);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background("#B45309").Padding(6)
                                    .Text("Loại phí").FontColor("#FFF").Bold();
                                header.Cell().Background("#B45309").Padding(6)
                                    .Text("Số lần").FontColor("#FFF").Bold();
                                header.Cell().Background("#B45309").Padding(6)
                                    .Text("Tổng tiền").FontColor("#FFF").Bold();
                            });

                            foreach (var pp in thongKe.PhuPhiTheoLoai)
                            {
                                table.Cell().BorderBottom(1).BorderColor("#E5E7EB").Padding(6)
                                    .Text(pp.LoaiPhi ?? "-");
                                table.Cell().BorderBottom(1).BorderColor("#E5E7EB").Padding(6)
                                    .Text($"{pp.SoLan}");
                                table.Cell().BorderBottom(1).BorderColor("#E5E7EB").Padding(6)
                                    .Text($"{pp.TongTien:N0} đ").FontColor("#B45309");
                            }
                        });
                    });

                    // ===== FOOTER =====
                    page.Footer().Column(col =>
                    {
                        col.Item().LineHorizontal(1).LineColor("#E5E7EB");
                        col.Item().PaddingTop(8).AlignCenter()
                            .Text($"Báo cáo được tạo tự động bởi hệ thống - Trang {DateTime.Now.Year}")
                            .FontSize(9).FontColor("#94A3B8");
                    });
                });
            });

            return document.GeneratePdf();
        }

        // =====================================================
        // PDF: HÓA ĐƠN THANH TOÁN
        // =====================================================
        public byte[] XuatHoaDonPdf(ThanhToan thanhToan)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header().Column(col =>
                    {
                        col.Item().AlignCenter().Text("HÓA ĐƠN THANH TOÁN")
                            .FontSize(24).Bold().FontColor("#1E3A8A");
                        col.Item().AlignCenter().Text($"Mã hóa đơn: #{thanhToan.MaThanhToan}")
                            .FontSize(13).FontColor("#64748B");
                        col.Item().PaddingTop(10).LineHorizontal(2).LineColor("#3B82F6");
                    });

                    page.Content().PaddingVertical(15).Column(col =>
                    {
                        col.Spacing(15);

                        // Thông tin khách hàng
                        col.Item().Text("THÔNG TIN KHÁCH HÀNG").FontSize(13).Bold().FontColor("#1E3A8A");
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(140);
                                c.RelativeColumn();
                            });

                            void Row(string label, string value)
                            {
                                table.Cell().Padding(6).Text(label).FontColor("#64748B");
                                table.Cell().Padding(6).Text(value).Bold();
                            }

                            Row("Họ tên:", thanhToan.DatXe?.KhachHang?.HoTen ?? "-");
                            Row("Số điện thoại:", thanhToan.DatXe?.KhachHang?.SoDienThoai ?? "-");
                            Row("Email:", thanhToan.DatXe?.KhachHang?.Email ?? "-");
                            Row("Địa chỉ:", thanhToan.DatXe?.KhachHang?.DiaChi ?? "-");
                        });

                        col.Item().PaddingTop(10).Text("THÔNG TIN THUÊ XE").FontSize(13).Bold().FontColor("#1E3A8A");
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(140);
                                c.RelativeColumn();
                            });

                            void Row(string label, string value)
                            {
                                table.Cell().Padding(6).Text(label).FontColor("#64748B");
                                table.Cell().Padding(6).Text(value).Bold();
                            }

                            Row("Xe:", $"{thanhToan.DatXe?.Xe?.TenXe} - {thanhToan.DatXe?.Xe?.BienSo}");
                            Row("Thời gian nhận:", thanhToan.DatXe?.ThoiGianNhanDuKien.ToString("dd/MM/yyyy HH:mm") ?? "-");
                            Row("Thời gian trả:", thanhToan.DatXe?.ThoiGianTraDuKien.ToString("dd/MM/yyyy HH:mm") ?? "-");
                            Row("Ngày thanh toán:", thanhToan.NgayThanhToan.ToString("dd/MM/yyyy HH:mm"));
                            Row("Phương thức:", thanhToan.PhuongThucThanhToan);
                        });

                        // Chi tiết thanh toán
                        col.Item().PaddingTop(10).Text("CHI TIẾT THANH TOÁN").FontSize(13).Bold().FontColor("#1E3A8A");
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(2);
                                c.RelativeColumn();
                            });

                            void Row(string label, string value, string? color = null)
                            {
                                table.Cell().Padding(6).Text(label).FontColor("#64748B");
                                var txt = table.Cell().Padding(6).Text(value);
                                if (color != null) txt.FontColor(color);
                            }

                            Row("Tiền thuê:", $"{thanhToan.TienThue:N0} đ");
                            Row("Tổng phụ phí:", $"{thanhToan.TongPhuPhi:N0} đ");
                            Row("Tổng thanh toán:", $"{thanhToan.TongThanhToan:N0} đ", "#1D4ED8");
                            Row("Tiền cọc đã thu:", $"- {thanhToan.TienCocDaThu:N0} đ", "#059669");
                        });

                        // Tổng kết
                        col.Item().PaddingTop(5).Background("#FEF3C7").Padding(12).Row(row =>
                        {
                            row.RelativeItem().AlignRight().Text("SỐ TIỀN CÒN LẠI:")
                                .FontSize(14).Bold().FontColor("#92400E");
                            row.ConstantItem(150).AlignRight()
                                .Text($"{thanhToan.SoTienConLai:N0} đ")
                                .FontSize(18).Bold().FontColor("#B45309");
                        });

                        // Trạng thái
                        col.Item().PaddingTop(10).AlignRight()
                            .Text($"Trạng thái: {thanhToan.TrangThaiThanhToan}")
                            .FontSize(12).Bold()
                            .FontColor(thanhToan.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan
                                ? "#059669" : "#B45309");
                    });

                    page.Footer().Column(col =>
                    {
                        col.Item().LineHorizontal(1).LineColor("#E5E7EB");
                        col.Item().PaddingTop(8).AlignCenter()
                            .Text("Cảm ơn quý khách đã sử dụng dịch vụ!")
                            .FontSize(11).Italic().FontColor("#64748B");
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}