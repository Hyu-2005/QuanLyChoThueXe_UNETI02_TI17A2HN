using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HangXe",
                columns: table => new
                {
                    MaHangXe = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenHangXe = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    QuocGia = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HangXe", x => x.MaHangXe);
                });

            migrationBuilder.CreateTable(
                name: "LoaiXe",
                columns: table => new
                {
                    MaLoaiXe = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLoaiXe = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SoCho = table.Column<int>(type: "int", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiXe", x => x.MaLoaiXe);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoan",
                columns: table => new
                {
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VaiTro = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoan", x => x.MaTaiKhoan);
                });

            migrationBuilder.CreateTable(
                name: "Xe",
                columns: table => new
                {
                    MaXe = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BienSo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenXe = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaLoaiXe = table.Column<int>(type: "int", nullable: false),
                    MaHangXe = table.Column<int>(type: "int", nullable: false),
                    NamSanXuat = table.Column<int>(type: "int", nullable: false),
                    MauSac = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    SoCho = table.Column<int>(type: "int", nullable: false),
                    SoKmHienTai = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    TinhTrang = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AnhXe = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Xe", x => x.MaXe);
                    table.ForeignKey(
                        name: "FK_Xe_HangXe_MaHangXe",
                        column: x => x.MaHangXe,
                        principalTable: "HangXe",
                        principalColumn: "MaHangXe",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Xe_LoaiXe_MaLoaiXe",
                        column: x => x.MaLoaiXe,
                        principalTable: "LoaiXe",
                        principalColumn: "MaLoaiXe",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KhachHang",
                columns: table => new
                {
                    MaKhachHang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: true),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SoGiayPhepLaiXe = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    NgayHetHanGPLX = table.Column<DateTime>(type: "date", nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhachHang", x => x.MaKhachHang);
                    table.ForeignKey(
                        name: "FK_KhachHang_TaiKhoan_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TaiKhoan",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "BangGiaThue",
                columns: table => new
                {
                    MaBangGia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaLoaiXe = table.Column<int>(type: "int", nullable: true),
                    MaXe = table.Column<int>(type: "int", nullable: true),
                    DonGiaNgay = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    DonGiaGio = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    TienCocMacDinh = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    TuNgay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DenNgay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BangGiaThue", x => x.MaBangGia);
                    table.CheckConstraint("CK_BangGiaThue_DonGia", "[DonGiaNgay] > 0 AND [DonGiaGio] >= 0 AND [TienCocMacDinh] >= 0");
                    table.CheckConstraint("CK_BangGiaThue_KhoangHieuLuc", "[DenNgay] > [TuNgay]");
                    table.CheckConstraint("CK_BangGiaThue_PhamViApDung", "([MaLoaiXe] IS NOT NULL AND [MaXe] IS NULL) OR ([MaLoaiXe] IS NULL AND [MaXe] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_BangGiaThue_LoaiXe_MaLoaiXe",
                        column: x => x.MaLoaiXe,
                        principalTable: "LoaiXe",
                        principalColumn: "MaLoaiXe",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BangGiaThue_Xe_MaXe",
                        column: x => x.MaXe,
                        principalTable: "Xe",
                        principalColumn: "MaXe",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DatXe",
                columns: table => new
                {
                    MaDatXe = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKhachHang = table.Column<int>(type: "int", nullable: false),
                    MaXe = table.Column<int>(type: "int", nullable: false),
                    ThoiGianNhanDuKien = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianTraDuKien = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiaDiemNhan = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DiaDiemTra = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NgayDat = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DonGiaApDung = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    TienCoc = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LyDoTuChoi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatXe", x => x.MaDatXe);
                    table.CheckConstraint("CK_DatXe_ThoiGian", "[ThoiGianTraDuKien] > [ThoiGianNhanDuKien]");
                    table.CheckConstraint("CK_DatXe_Tien", "[DonGiaApDung] > 0 AND [TienCoc] >= 0");
                    table.ForeignKey(
                        name: "FK_DatXe_KhachHang_MaKhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "KhachHang",
                        principalColumn: "MaKhachHang",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DatXe_Xe_MaXe",
                        column: x => x.MaXe,
                        principalTable: "Xe",
                        principalColumn: "MaXe",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BanGiaoXe",
                columns: table => new
                {
                    MaBanGiao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDatXe = table.Column<int>(type: "int", nullable: false),
                    ThoiGianBanGiao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoKmBanGiao = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MucNhienLieuBanGiao = table.Column<int>(type: "int", nullable: false),
                    TinhTrangBanGiao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NguoiBanGiao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BanGiaoXe", x => x.MaBanGiao);
                    table.CheckConstraint("CK_BanGiaoXe_NhienLieu", "[MucNhienLieuBanGiao] >= 0 AND [MucNhienLieuBanGiao] <= 100");
                    table.CheckConstraint("CK_BanGiaoXe_SoKm", "[SoKmBanGiao] >= 0");
                    table.ForeignKey(
                        name: "FK_BanGiaoXe_DatXe_MaDatXe",
                        column: x => x.MaDatXe,
                        principalTable: "DatXe",
                        principalColumn: "MaDatXe",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThanhToan",
                columns: table => new
                {
                    MaThanhToan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDatXe = table.Column<int>(type: "int", nullable: false),
                    TienThue = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    TongPhuPhi = table.Column<decimal>(type: "decimal(18,0)", nullable: false, defaultValue: 0m),
                    TienCocDaThu = table.Column<decimal>(type: "decimal(18,0)", nullable: false, defaultValue: 0m),
                    TongThanhToan = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    SoTienConLai = table.Column<decimal>(type: "decimal(18,0)", nullable: false, defaultValue: 0m),
                    PhuongThucThanhToan = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    NgayThanhToan = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    TrangThaiThanhToan = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThanhToan", x => x.MaThanhToan);
                    table.CheckConstraint("CK_ThanhToan_Tien", "[TienThue] >= 0 AND [TongPhuPhi] >= 0 AND [TienCocDaThu] >= 0 AND [TongThanhToan] >= 0 AND [SoTienConLai] >= 0");
                    table.ForeignKey(
                        name: "FK_ThanhToan_DatXe_MaDatXe",
                        column: x => x.MaDatXe,
                        principalTable: "DatXe",
                        principalColumn: "MaDatXe",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TraXe",
                columns: table => new
                {
                    MaTraXe = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDatXe = table.Column<int>(type: "int", nullable: false),
                    ThoiGianTraThucTe = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoKmTra = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MucNhienLieuTra = table.Column<int>(type: "int", nullable: false),
                    TinhTrangTra = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PhiQuaHan = table.Column<decimal>(type: "decimal(18,0)", nullable: false, defaultValue: 0m),
                    PhiVuotKm = table.Column<decimal>(type: "decimal(18,0)", nullable: false, defaultValue: 0m),
                    PhiNhienLieu = table.Column<decimal>(type: "decimal(18,0)", nullable: false, defaultValue: 0m),
                    PhiHuHong = table.Column<decimal>(type: "decimal(18,0)", nullable: false, defaultValue: 0m),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraXe", x => x.MaTraXe);
                    table.CheckConstraint("CK_TraXe_NhienLieu", "[MucNhienLieuTra] >= 0 AND [MucNhienLieuTra] <= 100");
                    table.CheckConstraint("CK_TraXe_PhuPhi", "[PhiQuaHan] >= 0 AND [PhiVuotKm] >= 0 AND [PhiNhienLieu] >= 0 AND [PhiHuHong] >= 0");
                    table.CheckConstraint("CK_TraXe_SoKm", "[SoKmTra] >= 0");
                    table.ForeignKey(
                        name: "FK_TraXe_DatXe_MaDatXe",
                        column: x => x.MaDatXe,
                        principalTable: "DatXe",
                        principalColumn: "MaDatXe",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BangGiaThue_MaXe",
                table: "BangGiaThue",
                column: "MaXe");

            migrationBuilder.CreateIndex(
                name: "IX_BangGiaThue_PhamVi_TrangThai",
                table: "BangGiaThue",
                columns: new[] { "MaLoaiXe", "MaXe", "TrangThai" });

            migrationBuilder.CreateIndex(
                name: "UX_BanGiaoXe_MaDatXe",
                table: "BanGiaoXe",
                column: "MaDatXe",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DatXe_KhachHang_TrangThai",
                table: "DatXe",
                columns: new[] { "MaKhachHang", "TrangThai" });

            migrationBuilder.CreateIndex(
                name: "IX_DatXe_TrungLich",
                table: "DatXe",
                columns: new[] { "MaXe", "TrangThai", "ThoiGianNhanDuKien", "ThoiGianTraDuKien" });

            migrationBuilder.CreateIndex(
                name: "UX_HangXe_TenHangXe",
                table: "HangXe",
                column: "TenHangXe",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KhachHang_MaTaiKhoan",
                table: "KhachHang",
                column: "MaTaiKhoan",
                unique: true,
                filter: "[MaTaiKhoan] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_KhachHang_SoDienThoai",
                table: "KhachHang",
                column: "SoDienThoai");

            migrationBuilder.CreateIndex(
                name: "UX_LoaiXe_TenLoaiXe",
                table: "LoaiXe",
                column: "TenLoaiXe",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_TaiKhoan_TenDangNhap",
                table: "TaiKhoan",
                column: "TenDangNhap",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThanhToan_MaDatXe",
                table: "ThanhToan",
                column: "MaDatXe");

            migrationBuilder.CreateIndex(
                name: "IX_ThanhToan_Ngay_TrangThai",
                table: "ThanhToan",
                columns: new[] { "NgayThanhToan", "TrangThaiThanhToan" });

            migrationBuilder.CreateIndex(
                name: "UX_TraXe_MaDatXe",
                table: "TraXe",
                column: "MaDatXe",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Xe_MaHangXe",
                table: "Xe",
                column: "MaHangXe");

            migrationBuilder.CreateIndex(
                name: "IX_Xe_MaLoaiXe",
                table: "Xe",
                column: "MaLoaiXe");

            migrationBuilder.CreateIndex(
                name: "UX_Xe_BienSo",
                table: "Xe",
                column: "BienSo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BangGiaThue");

            migrationBuilder.DropTable(
                name: "BanGiaoXe");

            migrationBuilder.DropTable(
                name: "ThanhToan");

            migrationBuilder.DropTable(
                name: "TraXe");

            migrationBuilder.DropTable(
                name: "DatXe");

            migrationBuilder.DropTable(
                name: "KhachHang");

            migrationBuilder.DropTable(
                name: "Xe");

            migrationBuilder.DropTable(
                name: "TaiKhoan");

            migrationBuilder.DropTable(
                name: "HangXe");

            migrationBuilder.DropTable(
                name: "LoaiXe");
        }
    }
}
