// ============================================================
// File: Data/SeedData/DbInitializer.cs
// Noi dung: Tao du lieu mau cho he thong QuanLyChoThueXe
// Sinh vien thuc hien: [Ca nhom]
// ============================================================

using Microsoft.EntityFrameworkCore;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Data.SeedData
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // Neu da co du lieu thi bo qua
            if (await context.TaiKhoans.AnyAsync())
                return;

            // =====================================================
            // BUOC 1: TAI KHOAN
            // =====================================================
            var taiKhoans = new List<TaiKhoan>
            {
                new() { TenDangNhap = "admin",      MatKhau = "admin123",   HoTen = "Quan Tri Vien",  Email = "admin@rentcar.vn",      VaiTro = VaiTro.Admin,    TrangThai = true },
                new() { TenDangNhap = "nhanvien01", MatKhau = "nv123456",   HoTen = "Nguyen Van Hung", Email = "hung.nv@rentcar.vn",   VaiTro = VaiTro.NhanVien, TrangThai = true },
                new() { TenDangNhap = "nhanvien02", MatKhau = "nv123456",   HoTen = "Tran Thi Lan",    Email = "lan.tt@rentcar.vn",    VaiTro = VaiTro.NhanVien, TrangThai = true },
                new() { TenDangNhap = "nhanvien03", MatKhau = "nv123456",   HoTen = "Le Minh Tuan",    Email = "tuan.lm@rentcar.vn",   VaiTro = VaiTro.NhanVien, TrangThai = true },
                // Tai khoan da bi khoa de test
                new() { TenDangNhap = "nhanvien04", MatKhau = "nv123456",   HoTen = "Pham Van Khoa",   Email = "khoa.pv@rentcar.vn",   VaiTro = VaiTro.NhanVien, TrangThai = false },
            };

            // 30 tai khoan khach hang
            for (int i = 1; i <= 30; i++)
            {
                taiKhoans.Add(new TaiKhoan
                {
                    TenDangNhap = $"kh{i:D2}",
                    MatKhau = "kh123456",
                    HoTen = $"Khach Hang {i:D2}",
                    Email = $"khachhang{i:D2}@gmail.com",
                    VaiTro = VaiTro.KhachHang,
                    TrangThai = i != 30 // kh30 bi khoa
                });
            }

            await context.TaiKhoans.AddRangeAsync(taiKhoans);
            await context.SaveChangesAsync();

            // =====================================================
            // BUOC 2: LOAI XE (5 loai)
            // =====================================================
            var loaiXes = new List<LoaiXe>
            {
                new() { TenLoaiXe = "Sedan 4 cho",    SoCho = 4, MoTa = "Xe sedan 4 cho phu hop di gia dinh nho", TrangThai = true },
                new() { TenLoaiXe = "SUV 5 cho",      SoCho = 5, MoTa = "Xe SUV 5 cho, gam cao, phu hop du lich",  TrangThai = true },
                new() { TenLoaiXe = "MPV 7 cho",      SoCho = 7, MoTa = "Xe 7 cho phu hop gia dinh dong nguoi",   TrangThai = true },
                new() { TenLoaiXe = "Xe 16 cho",      SoCho = 16, MoTa = "Xe khach 16 cho phu hop doan du lich",  TrangThai = true },
                new() { TenLoaiXe = "Xe 29 cho",      SoCho = 29, MoTa = "Xe khach 29 cho phu hop doan lon",      TrangThai = true },
            };

            await context.LoaiXes.AddRangeAsync(loaiXes);
            await context.SaveChangesAsync();

            // =====================================================
            // BUOC 3: HANG XE (5 hang)
            // =====================================================
            var hangXes = new List<HangXe>
            {
                new() { TenHangXe = "Toyota",   QuocGia = "Nhat Ban", MoTa = "Hang xe pho bien tai Viet Nam", TrangThai = true },
                new() { TenHangXe = "Honda",    QuocGia = "Nhat Ban", MoTa = "Hang xe ben bi, tiet kiem nhien lieu", TrangThai = true },
                new() { TenHangXe = "Kia",      QuocGia = "Han Quoc", MoTa = "Hang xe gia re, phu tung de tim", TrangThai = true },
                new() { TenHangXe = "Hyundai",  QuocGia = "Han Quoc", MoTa = "Hang xe hien dai, nhieu tinh nang", TrangThai = true },
                new() { TenHangXe = "Ford",     QuocGia = "My",       MoTa = "Hang xe ben bi, dong co manh", TrangThai = true },
            };

            await context.HangXes.AddRangeAsync(hangXes);
            await context.SaveChangesAsync();

            // =====================================================
            // BUOC 4: XE (25 xe)
            // =====================================================
            var mauSac = new[] { "Trang", "Den", "Bac", "Do", "Xanh duong" };
            var xeList = new List<Xe>();

            // 5 xe Sedan 4 cho
            var sedanNames = new[] { "Toyota Vios", "Honda City", "Kia Morning", "Hyundai Accent", "Toyota Corolla" };
            for (int i = 0; i < 5; i++)
            {
                xeList.Add(new Xe
                {
                    BienSo = $"30A-{10000 + i:D5}",
                    TenXe = sedanNames[i],
                    MaLoaiXe = loaiXes[0].MaLoaiXe,
                    MaHangXe = hangXes[i % 4].MaHangXe,
                    NamSanXuat = 2020 + (i % 4),
                    MauSac = mauSac[i % 5],
                    SoCho = 4,
                    SoKmHienTai = 15000 + i * 3000,
                    TinhTrang = TinhTrangXe.SanSang,
                    MoTa = $"Xe {sedanNames[i]} mau {mauSac[i % 5]}, tinh trang tot"
                });
            }

            // 5 xe SUV 5 cho
            var suvNames = new[] { "Toyota Fortuner", "Honda CR-V", "Kia Sorento", "Hyundai Tucson", "Ford Everest" };
            for (int i = 0; i < 5; i++)
            {
                xeList.Add(new Xe
                {
                    BienSo = $"30B-{20000 + i:D5}",
                    TenXe = suvNames[i],
                    MaLoaiXe = loaiXes[1].MaLoaiXe,
                    MaHangXe = hangXes[i % 5].MaHangXe,
                    NamSanXuat = 2021 + (i % 3),
                    MauSac = mauSac[(i + 1) % 5],
                    SoCho = 5,
                    SoKmHienTai = 20000 + i * 4000,
                    TinhTrang = TinhTrangXe.SanSang,
                    MoTa = $"Xe {suvNames[i]}, gam cao, phu hop du lich"
                });
            }

            // 7 xe MPV 7 cho
            var mpvNames = new[] { "Toyota Innova", "Honda Odyssey", "Kia Carnival", "Hyundai Staria", "Toyota Avanza", "Kia Rondo", "Hyundai Santafe" };
            for (int i = 0; i < 7; i++)
            {
                xeList.Add(new Xe
                {
                    BienSo = $"30C-{30000 + i:D5}",
                    TenXe = mpvNames[i],
                    MaLoaiXe = loaiXes[2].MaLoaiXe,
                    MaHangXe = hangXes[i % 5].MaHangXe,
                    NamSanXuat = 2020 + (i % 5),
                    MauSac = mauSac[i % 5],
                    SoCho = 7,
                    SoKmHienTai = 25000 + i * 5000,
                    TinhTrang = i switch
                    {
                        0 => TinhTrangXe.SanSang,
                        1 => TinhTrangXe.DangChoThue,
                        2 => TinhTrangXe.SanSang,
                        3 => TinhTrangXe.BaoDuong,
                        4 => TinhTrangXe.SanSang,
                        5 => TinhTrangXe.DangChoThue,
                        _ => TinhTrangXe.SanSang
                    },
                    MoTa = $"Xe {mpvNames[i]} 7 cho"
                });
            }

            // 5 xe 16 cho
            var xe16Names = new[] { "Ford Transit 16", "Hyundai Solati 16", "Toyota Hiace 16", "Kia Grandbird 16", "Mercedes Sprinter 16" };
            for (int i = 0; i < 5; i++)
            {
                xeList.Add(new Xe
                {
                    BienSo = $"29D-{40000 + i:D5}",
                    TenXe = xe16Names[i],
                    MaLoaiXe = loaiXes[3].MaLoaiXe,
                    MaHangXe = hangXes[i % 5].MaHangXe,
                    NamSanXuat = 2019 + (i % 4),
                    MauSac = mauSac[i % 3],
                    SoCho = 16,
                    SoKmHienTai = 50000 + i * 8000,
                    TinhTrang = i == 4 ? TinhTrangXe.NgungHoatDong : TinhTrangXe.SanSang,
                    MoTa = $"Xe khach 16 cho {xe16Names[i]}"
                });
            }

            // 3 xe 29 cho
            var xe29Names = new[] { "Hyundai Universe 29", "Thaco Mobihome 29", "Samco Autocaro 29" };
            for (int i = 0; i < 3; i++)
            {
                xeList.Add(new Xe
                {
                    BienSo = $"29E-{50000 + i:D5}",
                    TenXe = xe29Names[i],
                    MaLoaiXe = loaiXes[4].MaLoaiXe,
                    MaHangXe = hangXes[i % 5].MaHangXe,
                    NamSanXuat = 2018 + (i % 5),
                    MauSac = mauSac[i % 5],
                    SoCho = 29,
                    SoKmHienTai = 80000 + i * 10000,
                    TinhTrang = TinhTrangXe.SanSang,
                    MoTa = $"Xe khach 29 cho {xe29Names[i]}"
                });
            }

            await context.Xes.AddRangeAsync(xeList);
            await context.SaveChangesAsync();

            // =====================================================
            // BUOC 5: BANG GIA THUE
            // =====================================================
            var bangGias = new List<BangGiaThue>();

            // Bang gia hien hanh theo loai xe (tu 01/01/2024 den 31/12/2030)
            var giaTheoLoai = new[]
            {
                (loaiXes[0].MaLoaiXe, 700000m,  120000m, 3000000m),   // Sedan
                (loaiXes[1].MaLoaiXe, 1200000m, 200000m, 5000000m),   // SUV
                (loaiXes[2].MaLoaiXe, 1500000m, 250000m, 6000000m),   // MPV 7
                (loaiXes[3].MaLoaiXe, 2500000m, 400000m, 10000000m),  // 16 cho
                (loaiXes[4].MaLoaiXe, 4000000m, 700000m, 15000000m),  // 29 cho
            };

            foreach (var (maLoai, giaNgay, giaGio, tienCoc) in giaTheoLoai)
            {
                bangGias.Add(new BangGiaThue
                {
                    MaLoaiXe = maLoai,
                    MaXe = null,
                    DonGiaNgay = giaNgay,
                    DonGiaGio = giaGio,
                    TienCocMacDinh = tienCoc,
                    TuNgay = new DateTime(2024, 1, 1),
                    DenNgay = new DateTime(2030, 12, 31),
                    TrangThai = true
                });
            }

            // Bang gia cu (het hieu luc) de test
            foreach (var (maLoai, _, _, _) in giaTheoLoai)
            {
                bangGias.Add(new BangGiaThue
                {
                    MaLoaiXe = maLoai,
                    MaXe = null,
                    DonGiaNgay = 500000m,
                    DonGiaGio = 80000m,
                    TienCocMacDinh = 2000000m,
                    TuNgay = new DateTime(2020, 1, 1),
                    DenNgay = new DateTime(2023, 12, 31),
                    TrangThai = false
                });
            }

            // Bang gia rieng cho 3 xe dac biet (gia cao hon 20%)
            bangGias.Add(new BangGiaThue
            {
                MaXe = xeList[0].MaXe,
                MaLoaiXe = null,
                DonGiaNgay = 850000m,
                DonGiaGio = 145000m,
                TienCocMacDinh = 3500000m,
                TuNgay = new DateTime(2024, 1, 1),
                DenNgay = new DateTime(2030, 12, 31),
                TrangThai = true
            });

            bangGias.Add(new BangGiaThue
            {
                MaXe = xeList[10].MaXe,
                MaLoaiXe = null,
                DonGiaNgay = 1500000m,
                DonGiaGio = 250000m,
                TienCocMacDinh = 7000000m,
                TuNgay = new DateTime(2024, 1, 1),
                DenNgay = new DateTime(2030, 12, 31),
                TrangThai = true
            });

            await context.BangGiaThues.AddRangeAsync(bangGias);
            await context.SaveChangesAsync();

            // =====================================================
            // BUOC 6: KHACH HANG (30 khach)
            // =====================================================
            var khachHangs = new List<KhachHang>();
            var diaChiList = new[]
            {
                "Ha Noi", "Hai Phong", "Da Nang", "Ho Chi Minh", "Can Tho",
                "Nghe An", "Thanh Hoa", "Hue", "Quang Ninh", "Bac Ninh"
            };

            var khachTks = taiKhoans.Where(t => t.VaiTro == VaiTro.KhachHang).ToList();

            for (int i = 0; i < 30; i++)
            {
                khachHangs.Add(new KhachHang
                {
                    MaTaiKhoan = khachTks[i].MaTaiKhoan,
                    HoTen = $"Khach Hang {i + 1:D2}",
                    SoDienThoai = $"09{i + 1:D8}",
                    Email = $"khachhang{i + 1:D2}@gmail.com",
                    DiaChi = diaChiList[i % 10],
                    SoGiayPhepLaiXe = $"GPLX{i + 1:D6}",
                    NgayHetHanGPLX = i switch
                    {
                        29 => new DateTime(2023, 12, 31), // Het han
                        28 => new DateTime(2024, 6, 30),  // Het han (so voi 2026)
                        _ => new DateTime(2027, 12, 31)   // Con han
                    },
                    TrangThai = i != 29 // khach cuoi bi khoa
                });
            }

            await context.KhachHangs.AddRangeAsync(khachHangs);
            await context.SaveChangesAsync();

            // =====================================================
            // BUOC 7: DAT XE (50 don o nhieu trang thai)
            // =====================================================
            var datXes = new List<DatXe>();
            var diaDiemList = new[]
            {
                "San bay Noi Bai", "Ben xe My Dinh", "Trung tam Ha Noi",
                "San bay Tan Son Nhat", "Ben xe Mien Dong", "Trung tam Q1"
            };

            // Helper lay bang gia hieu luc cho 1 loai xe tai thoi diem
            decimal GetDonGiaNgay(int maLoaiXe, DateTime thoiDiem)
            {
                var bangGia = bangGias
                    .Where(b => b.TrangThai && b.MaLoaiXe == maLoaiXe)
                    .FirstOrDefault(b => b.TuNgay <= thoiDiem && b.DenNgay >= thoiDiem);
                return bangGia?.DonGiaNgay ?? 700000m;
            }

            decimal GetTienCoc(int maLoaiXe, DateTime thoiDiem)
            {
                var bangGia = bangGias
                    .Where(b => b.TrangThai && b.MaLoaiXe == maLoaiXe)
                    .FirstOrDefault(b => b.TuNgay <= thoiDiem && b.DenNgay >= thoiDiem);
                return bangGia?.TienCocMacDinh ?? 3000000m;
            }

            // Cac trang thai va so luong:
            // 8 ChoDuyet, 10 DaDuyet, 7 DaBanGiao, 8 DangThue,
            // 5 ChoThanhToan, 8 HoanThanh, 2 TuChoi, 2 DaHuy
            var trangThaiList = new[]
            {
                TrangThaiDatXe.ChoDuyet, TrangThaiDatXe.ChoDuyet, TrangThaiDatXe.ChoDuyet, TrangThaiDatXe.ChoDuyet,
                TrangThaiDatXe.ChoDuyet, TrangThaiDatXe.ChoDuyet, TrangThaiDatXe.ChoDuyet, TrangThaiDatXe.ChoDuyet,

                TrangThaiDatXe.DaDuyet, TrangThaiDatXe.DaDuyet, TrangThaiDatXe.DaDuyet, TrangThaiDatXe.DaDuyet,
                TrangThaiDatXe.DaDuyet, TrangThaiDatXe.DaDuyet, TrangThaiDatXe.DaDuyet, TrangThaiDatXe.DaDuyet,
                TrangThaiDatXe.DaDuyet, TrangThaiDatXe.DaDuyet,

                TrangThaiDatXe.DaBanGiao, TrangThaiDatXe.DaBanGiao, TrangThaiDatXe.DaBanGiao, TrangThaiDatXe.DaBanGiao,
                TrangThaiDatXe.DaBanGiao, TrangThaiDatXe.DaBanGiao, TrangThaiDatXe.DaBanGiao,

                TrangThaiDatXe.DangThue, TrangThaiDatXe.DangThue, TrangThaiDatXe.DangThue, TrangThaiDatXe.DangThue,
                TrangThaiDatXe.DangThue, TrangThaiDatXe.DangThue, TrangThaiDatXe.DangThue, TrangThaiDatXe.DangThue,

                TrangThaiDatXe.ChoThanhToan, TrangThaiDatXe.ChoThanhToan, TrangThaiDatXe.ChoThanhToan,
                TrangThaiDatXe.ChoThanhToan, TrangThaiDatXe.ChoThanhToan,

                TrangThaiDatXe.HoanThanh, TrangThaiDatXe.HoanThanh, TrangThaiDatXe.HoanThanh, TrangThaiDatXe.HoanThanh,
                TrangThaiDatXe.HoanThanh, TrangThaiDatXe.HoanThanh, TrangThaiDatXe.HoanThanh, TrangThaiDatXe.HoanThanh,

                TrangThaiDatXe.TuChoi, TrangThaiDatXe.TuChoi,

                TrangThaiDatXe.DaHuy, TrangThaiDatXe.DaHuy
            };

            var now = DateTime.Now;

            for (int i = 0; i < 50; i++)
            {
                var khach = khachHangs[i % 30];
                var xe = xeList[i % 25];
                var trangThai = trangThaiList[i];

                // Tinh thoi gian dua theo trang thai
                DateTime thoiGianNhan;
                DateTime thoiGianTra;

                if (trangThai == TrangThaiDatXe.HoanThanh || trangThai == TrangThaiDatXe.ChoThanhToan)
                {
                    // Da hoan thanh trong qua khu
                    thoiGianNhan = now.AddDays(-30 - i);
                    thoiGianTra = thoiGianNhan.AddDays(2 + (i % 3));
                }
                else if (trangThai == TrangThaiDatXe.DangThue || trangThai == TrangThaiDatXe.DaBanGiao)
                {
                    // Dang thue (nhan trong qua khu gan day, tra tuong lai gan)
                    thoiGianNhan = now.AddDays(-2 - (i % 3));
                    thoiGianTra = thoiGianNhan.AddDays(3 + (i % 3));
                }
                else if (trangThai == TrangThaiDatXe.DaDuyet)
                {
                    // Da duyet - sap toi
                    thoiGianNhan = now.AddDays(3 + i);
                    thoiGianTra = thoiGianNhan.AddDays(2 + (i % 3));
                }
                else
                {
                    // Cho duyet / Tu choi / Da huy
                    thoiGianNhan = now.AddDays(10 + i);
                    thoiGianTra = thoiGianNhan.AddDays(2 + (i % 3));
                }

                var maLoai = xe.MaLoaiXe;
                var donGia = GetDonGiaNgay(maLoai, thoiGianNhan);
                var tienCoc = GetTienCoc(maLoai, thoiGianNhan);

                var datXe = new DatXe
                {
                    MaKhachHang = khach.MaKhachHang,
                    MaXe = xe.MaXe,
                    ThoiGianNhanDuKien = thoiGianNhan,
                    ThoiGianTraDuKien = thoiGianTra,
                    DiaDiemNhan = diaDiemList[i % 6],
                    DiaDiemTra = diaDiemList[(i + 3) % 6],
                    NgayDat = thoiGianNhan.AddDays(-5),
                    DonGiaApDung = donGia,
                    TienCoc = tienCoc,
                    TrangThai = trangThai,
                    GhiChu = i % 5 == 0 ? "Khach yeu cau xe sach, day nhien lieu" : null,
                    LyDoTuChoi = trangThai == TrangThaiDatXe.TuChoi
                        ? "Xe da co lich trung, khong the sap xep"
                        : null
                };

                datXes.Add(datXe);
            }

            // Them 1 cap don trung lich CO CHU Y de test (khong nen co trong thuc te)
            // Cap nay xe 30A-10000 da co don DaDuyet tu 3-5 ngay toi
            // Nhung duoi day khong add vi da co logic rieng trong DanhSachDaDuyet

            await context.DatXes.AddRangeAsync(datXes);
            await context.SaveChangesAsync();

            // =====================================================
            // BUOC 8: BAN GIAO XE (cho cac don DaBanGiao, DangThue, ChoThanhToan, HoanThanh)
            // =====================================================
            var banGiaoXes = new List<BanGiaoXe>();
            var nhanVienNames = new[] { "Nguyen Van Hung", "Tran Thi Lan", "Le Minh Tuan" };
            var tinhTrangBanGiaoList = new[]
            {
                "Xe sach, day nhien lieu, khong hu hong",
                "Xe sach, con 80% nhien lieu, lop moi",
                "Xe co vai vet xuat nho o canh cua, may hoat dong tot"
            };

            var donCanBanGiao = datXes
                .Where(d => d.TrangThai == TrangThaiDatXe.DaBanGiao
                         || d.TrangThai == TrangThaiDatXe.DangThue
                         || d.TrangThai == TrangThaiDatXe.ChoThanhToan
                         || d.TrangThai == TrangThaiDatXe.HoanThanh)
                .ToList();

            var xeDict = xeList.ToDictionary(x => x.MaXe);

            for (int i = 0; i < donCanBanGiao.Count; i++)
            {
                var don = donCanBanGiao[i];
                var xeCuaDon = xeDict[don.MaXe];

                banGiaoXes.Add(new BanGiaoXe
                {
                    MaDatXe = don.MaDatXe,
                    ThoiGianBanGiao = don.ThoiGianNhanDuKien.AddMinutes(-15 + (i % 30)),
                    SoKmBanGiao = xeCuaDon.SoKmHienTai,
                    MucNhienLieuBanGiao = 80 + (i % 3) * 10,
                    TinhTrangBanGiao = tinhTrangBanGiaoList[i % 3],
                    NguoiBanGiao = nhanVienNames[i % 3],
                    GhiChu = i % 4 == 0 ? "Khach da kiem tra xe truoc khi nhan" : null
                });
            }

            await context.BanGiaoXes.AddRangeAsync(banGiaoXes);
            await context.SaveChangesAsync();

            // =====================================================
            // BUOC 9: TRA XE (cho cac don ChoThanhToan va HoanThanh)
            // =====================================================
            var traXes = new List<TraXe>();
            var tinhTrangTraList = new[]
            {
                "Xe binh thuong, khong hu hong",
                "Xe co vet xuat nho, khong anh huong",
                "Xe sach, con 60% nhien lieu, lop mon nhe"
            };

            var donCanTraXe = datXes
                .Where(d => d.TrangThai == TrangThaiDatXe.ChoThanhToan
                         || d.TrangThai == TrangThaiDatXe.HoanThanh)
                .ToList();

            var banGiaoDict = banGiaoXes.ToDictionary(b => b.MaDatXe);

            for (int i = 0; i < donCanTraXe.Count; i++)
            {
                var don = donCanTraXe[i];
                if (!banGiaoDict.TryGetValue(don.MaDatXe, out var bg)) continue;

                // 50% tra dung han, 30% tra tre nhe, 20% tra tre nhieu
                var thoiGianTraThucTe = don.ThoiGianTraDuKien;
                decimal phiQuaHan = 0;
                if (i % 5 == 1) // tra tre 2 gio
                {
                    thoiGianTraThucTe = don.ThoiGianTraDuKien.AddHours(2);
                    phiQuaHan = 2 * 200000m * 1.5m; // 600000
                }
                else if (i % 5 == 2) // tra tre 5 gio
                {
                    thoiGianTraThucTe = don.ThoiGianTraDuKien.AddHours(5);
                    phiQuaHan = 5 * 200000m * 1.5m; // 1500000
                }

                // So km: 50% khong vuot, 30% vuot nhe, 20% vuot nhieu
                var soNgayThue = Math.Max(1, (int)Math.Ceiling((don.ThoiGianTraDuKien - don.ThoiGianNhanDuKien).TotalDays));
                var kmChoPhep = 300m * soNgayThue;
                decimal soKmTraThem = 200 + (i % 3) * 100; // 200, 300, 400 km
                decimal phiVuotKm = 0;
                if (i % 5 == 3)
                {
                    soKmTraThem = kmChoPhep + 50; // vuot 50km
                    phiVuotKm = 50 * 3000m; // 150000
                }
                else if (i % 5 == 4)
                {
                    soKmTraThem = kmChoPhep + 150; // vuot 150km
                    phiVuotKm = 150 * 3000m; // 450000
                }

                // Nhien lieu: 60% day, 40% thieu
                var mucNhienLieuTra = 80 - (i % 3) * 10;
                decimal phiNhienLieu = 0;
                if (bg.MucNhienLieuBanGiao > mucNhienLieuTra)
                {
                    phiNhienLieu = (bg.MucNhienLieuBanGiao - mucNhienLieuTra) * 20000m;
                }

                // Hu hong: 20% co hu hong
                decimal phiHuHong = 0;
                if (i % 5 == 0 && i > 0)
                {
                    phiHuHong = 500000m;
                }

                traXes.Add(new TraXe
                {
                    MaDatXe = don.MaDatXe,
                    ThoiGianTraThucTe = thoiGianTraThucTe,
                    SoKmTra = bg.SoKmBanGiao + soKmTraThem,
                    MucNhienLieuTra = mucNhienLieuTra,
                    TinhTrangTra = tinhTrangTraList[i % 3],
                    PhiQuaHan = phiQuaHan,
                    PhiVuotKm = phiVuotKm,
                    PhiNhienLieu = phiNhienLieu,
                    PhiHuHong = phiHuHong,
                    GhiChu = (phiQuaHan + phiVuotKm + phiNhienLieu + phiHuHong) > 0
                        ? "Co phat sinh phu phi, da thong bao khach"
                        : null
                });
            }

            await context.TraXes.AddRangeAsync(traXes);
            await context.SaveChangesAsync();

            // =====================================================
            // BUOC 10: THANH TOAN (cho cac don HoanThanh)
            // =====================================================
            var thanhToans = new List<ThanhToan>();
            var phuongThucList = new[]
            {
                PhuongThucThanhToan.TienMat,
                PhuongThucThanhToan.ChuyenKhoan,
                PhuongThucThanhToan.The,
                PhuongThucThanhToan.ViDienTu
            };

            var donHoanThanh = datXes
                .Where(d => d.TrangThai == TrangThaiDatXe.HoanThanh)
                .ToList();

            var traXeDict = traXes.ToDictionary(t => t.MaDatXe);

            for (int i = 0; i < donHoanThanh.Count; i++)
            {
                var don = donHoanThanh[i];
                if (!traXeDict.TryGetValue(don.MaDatXe, out var tx)) continue;

                // Tinh tien thue theo so ngay
                var soNgay = Math.Max(1, (int)Math.Ceiling((don.ThoiGianTraDuKien - don.ThoiGianNhanDuKien).TotalDays));
                var tienThue = soNgay * don.DonGiaApDung;
                var tongPhuPhi = tx.TongPhuPhi;
                var tong = tienThue + tongPhuPhi;
                var conLai = tong - don.TienCoc;
                if (conLai < 0) conLai = 0;

                thanhToans.Add(new ThanhToan
                {
                    MaDatXe = don.MaDatXe,
                    TienThue = tienThue,
                    TongPhuPhi = tongPhuPhi,
                    TienCocDaThu = don.TienCoc,
                    TongThanhToan = tong,
                    SoTienConLai = conLai,
                    PhuongThucThanhToan = phuongThucList[i % 4],
                    NgayThanhToan = tx.ThoiGianTraThucTe.AddHours(1),
                    TrangThaiThanhToan = TrangThaiThanhToan.DaThanhToan,
                    GhiChu = tongPhuPhi > 0 ? $"Da thu phu phi {tongPhuPhi:N0} d" : null
                });
            }

            // 2 don ChoThanhToan cung co ban ghi thanh toan nhung chua thanh toan
            var donChoThanhToan = datXes
                .Where(d => d.TrangThai == TrangThaiDatXe.ChoThanhToan)
                .Take(2)
                .ToList();

            foreach (var don in donChoThanhToan)
            {
                if (!traXeDict.TryGetValue(don.MaDatXe, out var tx)) continue;

                var soNgay = Math.Max(1, (int)Math.Ceiling((don.ThoiGianTraDuKien - don.ThoiGianNhanDuKien).TotalDays));
                var tienThue = soNgay * don.DonGiaApDung;
                var tongPhuPhi = tx.TongPhuPhi;
                var tong = tienThue + tongPhuPhi;

                thanhToans.Add(new ThanhToan
                {
                    MaDatXe = don.MaDatXe,
                    TienThue = tienThue,
                    TongPhuPhi = tongPhuPhi,
                    TienCocDaThu = don.TienCoc,
                    TongThanhToan = tong,
                    SoTienConLai = tong,
                    PhuongThucThanhToan = PhuongThucThanhToan.TienMat,
                    NgayThanhToan = DateTime.Now,
                    TrangThaiThanhToan = TrangThaiThanhToan.ChuaThanhToan,
                    GhiChu = "Cho khach thanh toan khi tra xe"
                });
            }

            await context.ThanhToans.AddRangeAsync(thanhToans);
            await context.SaveChangesAsync();
        }
    }
}