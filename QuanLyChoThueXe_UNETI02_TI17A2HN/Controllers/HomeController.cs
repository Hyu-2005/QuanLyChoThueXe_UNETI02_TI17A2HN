using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Data;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants;
using QuanLyChoThueXe_UNETI02_TI17A2HN.ViewModels.Home;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        // ==============================================================
        // Bắt Đầu Code của SV3: Vũ Tiến Đạt
        // file: Controllers/HomeController.cs
        // nội dung: Trang chủ lấy danh sách xe sẵn sàng (kèm giá/ngày) để hiện ở mục "Bộ sưu tập xe" theo giao diện VIRELLO.
        // sinh viên thực hiện: Vũ Tiến Đạt - 23103100119 - SV3
        // ==============================================================
        public async Task<IActionResult> Index()
        {
            var vm = new HomeIndexVM();

            try
            {
                vm.XeNoiBat = await LayXeNoiBatAsync();
            }
            catch
            {
                // Database chưa sẵn sàng -> trang chủ vẫn hiển thị bình thường, chỉ không có danh sách xe
            }

            return View(vm);
        }

        private async Task<List<XeNoiBatVM>> LayXeNoiBatAsync()
        {
            var homNay = DateTime.Today;

            var xes = await _context.Xes
                .AsNoTracking()
                .Include(x => x.LoaiXe)
                .Include(x => x.HangXe)
                .Where(x => x.TinhTrang == TinhTrangXe.SanSang)
                .OrderBy(x => x.MaXe)
                .Take(30)
                .ToListAsync();

            var bangGias = await _context.BangGiaThues
                .AsNoTracking()
                .Where(b => b.TrangThai && b.TuNgay <= homNay && b.DenNgay >= homNay)
                .ToListAsync();

            var ketQua = new List<XeNoiBatVM>();

            foreach (var xe in xes)
            {
                // Ưu tiên giá riêng của xe, không có thì dùng giá theo loại xe
                var gia = bangGias.Where(b => b.MaXe == xe.MaXe)
                                  .Select(b => b.DonGiaNgay).FirstOrDefault();
                if (gia <= 0)
                {
                    gia = bangGias.Where(b => b.MaXe == null && b.MaLoaiXe == xe.MaLoaiXe)
                                  .Select(b => b.DonGiaNgay).FirstOrDefault();
                }
                if (gia <= 0) continue;   // xe chưa có bảng giá thì không hiển thị (giống trang Tìm xe trống)

                ketQua.Add(new XeNoiBatVM
                {
                    MaXe = xe.MaXe,
                    TenXe = xe.TenXe,
                    TenLoaiXe = xe.LoaiXe?.TenLoaiXe ?? string.Empty,
                    TenHangXe = xe.HangXe?.TenHangXe ?? string.Empty,
                    SoCho = xe.SoCho,
                    NamSanXuat = xe.NamSanXuat,
                    MauSac = xe.MauSac,
                    AnhXe = xe.AnhXe,
                    DonGiaNgay = gia
                });

                if (ketQua.Count == 6) break;
            }

            return ketQua;
        }
        // ==============================================================
        // Kết thúc Code của SV3
        // ==============================================================

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
