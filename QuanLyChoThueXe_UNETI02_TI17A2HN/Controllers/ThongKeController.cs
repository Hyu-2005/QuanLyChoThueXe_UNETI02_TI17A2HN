// ThongKeController.cs - dashboard va thong ke
// sinh vien thuc hien: Duong Lam Huy - 23103100120
// module 5 - tinh tien, thanh toan, lich su va thong ke
// + Chuc nang nang cao: Xuat bao cao PDF

using Microsoft.AspNetCore.Mvc;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Helpers;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Services.Interfaces;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Controllers
{
    [SessionAuthorize(Roles = new[] { VaiTro.Admin, VaiTro.NhanVien })]
    public class ThongKeController : Controller
    {
        private readonly IThongKeService _thongKeService;
        private readonly IExportService _exportService;

        // =====================================================
        // CHI 1 CONSTRUCTOR DUY NHAT - Inject ca 2 service
        // =====================================================
        public ThongKeController(
            IThongKeService thongKeService,
            IExportService exportService)
        {
            _thongKeService = thongKeService;
            _exportService = exportService;
        }

        // =====================================================
        // DASHBOARD
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = await _thongKeService.LayDashboardAsync();
            return View(vm);
        }

        // =====================================================
        // THONG KE CHI TIET
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> ChiTiet(DateTime? tuNgay, DateTime? denNgay)
        {
            var tu = tuNgay ?? new DateTime(DateTime.Now.Year, 1, 1);
            var den = denNgay ?? DateTime.Now;

            var vm = await _thongKeService.LayThongKeAsync(tu, den);
            return View(vm);
        }

        // =====================================================
        // XUAT BAO CAO PDF  (Chuc nang nang cao #12)
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> XuatPdf(DateTime? tuNgay, DateTime? denNgay)
        {
            var tu = tuNgay ?? new DateTime(DateTime.Now.Year, 1, 1);
            var den = denNgay ?? DateTime.Now;

            // Lay du lieu thong ke
            var thongKe = await _thongKeService.LayThongKeAsync(tu, den);

            // Sinh file PDF
            var bytes = _exportService.XuatBaoCaoThongKePdf(thongKe);

            var fileName = $"BaoCaoThongKe_{tu:yyyyMMdd}_{den:yyyyMMdd}.pdf";
            return File(bytes, "application/pdf", fileName);
        }
    }
}