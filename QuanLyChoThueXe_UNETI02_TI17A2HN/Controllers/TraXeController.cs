// ==============================================================
// Bắt Đầu Code của SV4: Nguyễn Hoàng Đức Hiếu
// file: Controllers/TraXeController.cs
// nội dung: Tạo mới Controller độc lập để xử lý logic Trả xe và cập nhật luồng tự động chuyển hướng (redirect) sang trang Thanh toán sau khi trả xe xong.
// sinh viên thực hiện : Nguyễn Hoàng Đức Hiếu -  23103100116 - SV4 
// ==============================================================
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Data;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Helpers;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Services.Interfaces;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Controllers
{
    [SessionAuthorize(Roles = new[] { VaiTro.Admin, VaiTro.NhanVien })]
    public class TraXeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ITinhTienService _tinhTienService;

        public TraXeController(AppDbContext context, ITinhTienService tinhTienService)
        {
            _context = context;
            _tinhTienService = tinhTienService;
        }

        [HttpGet("/TraXe/{id}")]
        public async Task<IActionResult> Index(int id)
        {
            var datXe = await _context.DatXes
                .Include(d => d.Xe)
                .Include(d => d.KhachHang)
                .Include(d => d.BanGiaoXe)
                .FirstOrDefaultAsync(d => d.MaDatXe == id);

            if (datXe == null || datXe.Xe == null || datXe.TrangThai != TrangThaiDatXe.DangThue)
                return NotFound();
            
            var model = new TraXe {
                MaDatXe = id,
                ThoiGianTraThucTe = DateTime.Now,
                SoKmTra = datXe.Xe.SoKmHienTai,
                MucNhienLieuTra = datXe.BanGiaoXe?.MucNhienLieuBanGiao ?? 100
            };

            // Lay DonGiaGio tu BangGiaThue
            var bangGia = await _context.BangGiaThues.FirstOrDefaultAsync(b => b.MaXe == datXe.MaXe && b.TrangThai)
                       ?? await _context.BangGiaThues.FirstOrDefaultAsync(b => b.MaLoaiXe == datXe.Xe.MaLoaiXe && b.TrangThai);
            
            ViewBag.DonGiaGio = bangGia?.DonGiaGio ?? 0;
            ViewBag.DatXe = datXe;
            return View(model);
        }

        [HttpPost("/TraXe/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int id, TraXe model, bool canBaoDuong = false)
        {
            if (id != model.MaDatXe) return BadRequest();
            
            var datXe = await _context.DatXes
                .Include(d => d.Xe)
                .FirstOrDefaultAsync(d => d.MaDatXe == id);

            if (datXe == null || datXe.Xe == null || datXe.TrangThai != TrangThaiDatXe.DangThue)
                return NotFound();

            if (ModelState.IsValid)
            {
                var banGiao = datXe.BanGiaoXe ?? await _context.BanGiaoXes.FirstOrDefaultAsync(b => b.MaDatXe == id);
                if (banGiao != null && model.SoKmTra < banGiao.SoKmBanGiao)
                {
                    ModelState.AddModelError("SoKmTra", "Số km trả không được nhỏ hơn số km bàn giao.");
                    ViewBag.DatXe = datXe;
                    return View(model);
                }

                // ==============================================================
                // Bắt Đầu Code của SV4: Nguyễn Hoàng Đức Hiếu
                // file: Controllers/TraXeController.cs
                // nội dung: Nghiệp vụ 8.7 - Cập nhật trạng thái xe sau trả (Sẵn sàng nếu bình thường; Bảo dưỡng nếu có lỗi hoặc phát sinh phí hư hỏng)
                // sinh viên thực hiện : Nguyễn Hoàng Đức Hiếu -  23103100116 - SV4 
                // ==============================================================
                datXe.TrangThai = TrangThaiDatXe.ChoThanhToan;
                datXe.Xe.SoKmHienTai = model.SoKmTra;
                
                bool phaiBaoDuong = canBaoDuong || (model.PhiHuHong > 0);
                if (phaiBaoDuong)
                {
                    datXe.Xe.TinhTrang = TinhTrangXe.BaoDuong;

                    // ==============================================================
                    // Bắt Đầu Code của SV4: Nguyễn Hoàng Đức Hiếu
                    // file: Controllers/TraXeController.cs
                    // nội dung: STT 11 - Tự động tạo phiếu theo dõi bảo dưỡng xe khi phát sinh sự cố/hư hại sau trả xe
                    // sinh viên thực hiện : Nguyễn Hoàng Đức Hiếu -  23103100116 - SV4 
                    // ==============================================================
                    var phieuBaoDuong = new LichSuBaoDuong
                    {
                        MaXe = datXe.MaXe,
                        NgayBatDau = DateTime.Now,
                        LoaiBaoDuong = model.PhiHuHong > 0 ? LoaiBaoDuong.SuCoSauThue : LoaiBaoDuong.TongThe,
                        SoKmKhiBaoDuong = model.SoKmTra,
                        ChiPhi = model.PhiHuHong,
                        GaraBaoDuong = "Gara liên kết UNETI",
                        NoiDungBaoDuong = !string.IsNullOrWhiteSpace(model.TinhTrangTra) 
                            ? $"Ghi nhận sau trả xe: {model.TinhTrangTra}" 
                            : $"Kiểm tra kỹ thuật và khắc phục hư hại từ đơn thuê #{datXe.MaDatXe}",
                        NguoiPhuTrach = HttpContext.Session.GetString("HoTen") ?? "Nhân viên nhận xe",
                        TrangThai = TrangThaiBaoDuong.DangBaoDuong,
                        GhiChu = $"Hệ thống tự động lập phiếu bảo dưỡng từ đơn #{datXe.MaDatXe}. Phụ phí hư hỏng đã thu: {model.PhiHuHong:N0}đ"
                    };
                    _context.LichSuBaoDuongs.Add(phieuBaoDuong);
                    // ==============================================================
                    // Kết thúc Code của SV4
                    // ==============================================================
                }
                else
                {
                    datXe.Xe.TinhTrang = TinhTrangXe.SanSang;
                }
                // ==============================================================
                // Kết thúc Code của SV4
                // ==============================================================
                
                _context.TraXes.Add(model);
                
                // Tự động tạo hóa đơn chờ thanh toán để hiển thị trên màn hình Thanh Toán
                var ketQua = _tinhTienService.TinhThanhToan(datXe, model);
                var thanhToan = new ThanhToan
                {
                    MaDatXe = datXe.MaDatXe,
                    TienThue = ketQua.TienThue,
                    TongPhuPhi = ketQua.TongPhuPhi,
                    TienCocDaThu = ketQua.TienCocDaThu,
                    TongThanhToan = ketQua.TongThanhToan,
                    SoTienConLai = ketQua.SoTienConLai,
                    PhuongThucThanhToan = QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants.PhuongThucThanhToan.TienMat,
                    NgayThanhToan = DateTime.Now,
                    TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
                    GhiChu = "Chờ khách thanh toán khi trả xe"
                };
                _context.ThanhToans.Add(thanhToan);

                await _context.SaveChangesAsync();
                
                // ==============================================================
                // Bắt Đầu Code của SV4: Nguyễn Hoàng Đức Hiếu
                // file: Controllers/TraXeController.cs
                // nội dung: Thông báo rõ ràng trạng thái xe sau trả cho nhân viên
                // sinh viên thực hiện : Nguyễn Hoàng Đức Hiếu -  23103100116 - SV4 
                // ==============================================================
                TempData["Success"] = phaiBaoDuong
                    ? $"Đã nhận trả xe thành công. Xe '{datXe.Xe?.BienSo}' có lỗi/cần kiểm tra nên đã chuyển sang trạng thái 'Bảo dưỡng'. Vui lòng tiến hành thanh toán."
                    : $"Đã nhận trả xe thành công. Xe '{datXe.Xe?.BienSo}' bình thường, đã chuyển về trạng thái 'Sẵn sàng'. Vui lòng tiến hành thanh toán.";
                // ==============================================================
                // Kết thúc Code của SV4
                // ==============================================================
                return RedirectToAction("ThanhToan", "ThanhToan", new { id = id });
            }

            ViewBag.DatXe = datXe;
            return View(model);
        }
    }
}
// ==============================================================
// Kết thúc Code của SV4
// ==============================================================
