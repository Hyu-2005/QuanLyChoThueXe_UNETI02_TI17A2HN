// ThongKeController.cs - dashboard va thong ke
// sinh vien thuc hien: Duong Lam Huy - 23103100120
// module 5 - tinh tien, thanh toan, lich su va thong ke

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

        public ThongKeController(IThongKeService thongKeService)
        {
            _thongKeService = thongKeService;
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
    }
}