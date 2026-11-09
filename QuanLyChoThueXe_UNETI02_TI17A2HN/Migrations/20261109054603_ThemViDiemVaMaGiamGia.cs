using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Migrations
{
    /// <inheritdoc />
    public partial class ThemViDiemVaMaGiamGia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiemDaDung",
                table: "ThanhToan",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DiemDuocCong",
                table: "ThanhToan",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaGiamGiaId",
                table: "ThanhToan",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TienGiamVoucher",
                table: "ThanhToan",
                type: "decimal(18,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TienThueGoc",
                table: "ThanhToan",
                type: "decimal(18,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TienTruTuDiem",
                table: "ThanhToan",
                type: "decimal(18,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "DiemTichLuy",
                table: "KhachHang",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TongDiemDaTichLuy",
                table: "KhachHang",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MaGiamGia",
                columns: table => new
                {
                    MaGiamGiaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TenChuongTrinh = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LoaiGiamGia = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GiaTriGiam = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    GiamToiDa = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    DonHangToiThieu = table.Column<decimal>(type: "decimal(18,0)", nullable: false, defaultValue: 0m),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoLuotDungToiDa = table.Column<int>(type: "int", nullable: false, defaultValue: 100),
                    SoLuotDaDung = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    MoiKhachMotLan = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaGiamGia", x => x.MaGiamGiaId);
                    table.CheckConstraint("CK_MaGiamGia_GiaTri", "[GiaTriGiam] >= 0 AND [DonHangToiThieu] >= 0");
                    table.CheckConstraint("CK_MaGiamGia_ThoiGian", "[NgayKetThuc] > [NgayBatDau]");
                });

            migrationBuilder.CreateTable(
                name: "SuDungMaGiamGia",
                columns: table => new
                {
                    MaSuDung = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaGiamGiaId = table.Column<int>(type: "int", nullable: false),
                    MaKhachHang = table.Column<int>(type: "int", nullable: false),
                    MaDatXe = table.Column<int>(type: "int", nullable: false),
                    SoTienDuocGiam = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    NgaySuDung = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuDungMaGiamGia", x => x.MaSuDung);
                    table.ForeignKey(
                        name: "FK_SuDungMaGiamGia_DatXe_MaDatXe",
                        column: x => x.MaDatXe,
                        principalTable: "DatXe",
                        principalColumn: "MaDatXe",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SuDungMaGiamGia_KhachHang_MaKhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "KhachHang",
                        principalColumn: "MaKhachHang",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SuDungMaGiamGia_MaGiamGia_MaGiamGiaId",
                        column: x => x.MaGiamGiaId,
                        principalTable: "MaGiamGia",
                        principalColumn: "MaGiamGiaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ThanhToan_MaGiamGiaId",
                table: "ThanhToan",
                column: "MaGiamGiaId");

            migrationBuilder.CreateIndex(
                name: "UX_MaGiamGia_Code",
                table: "MaGiamGia",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SuDungMaGiamGia_MaDatXe",
                table: "SuDungMaGiamGia",
                column: "MaDatXe");

            migrationBuilder.CreateIndex(
                name: "IX_SuDungMaGiamGia_MaKhachHang",
                table: "SuDungMaGiamGia",
                column: "MaKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_SuDungMaGiamGia_Voucher_Khach",
                table: "SuDungMaGiamGia",
                columns: new[] { "MaGiamGiaId", "MaKhachHang" });

            migrationBuilder.AddForeignKey(
                name: "FK_ThanhToan_MaGiamGia_MaGiamGiaId",
                table: "ThanhToan",
                column: "MaGiamGiaId",
                principalTable: "MaGiamGia",
                principalColumn: "MaGiamGiaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThanhToan_MaGiamGia_MaGiamGiaId",
                table: "ThanhToan");

            migrationBuilder.DropTable(
                name: "SuDungMaGiamGia");

            migrationBuilder.DropTable(
                name: "MaGiamGia");

            migrationBuilder.DropIndex(
                name: "IX_ThanhToan_MaGiamGiaId",
                table: "ThanhToan");

            migrationBuilder.DropColumn(
                name: "DiemDaDung",
                table: "ThanhToan");

            migrationBuilder.DropColumn(
                name: "DiemDuocCong",
                table: "ThanhToan");

            migrationBuilder.DropColumn(
                name: "MaGiamGiaId",
                table: "ThanhToan");

            migrationBuilder.DropColumn(
                name: "TienGiamVoucher",
                table: "ThanhToan");

            migrationBuilder.DropColumn(
                name: "TienThueGoc",
                table: "ThanhToan");

            migrationBuilder.DropColumn(
                name: "TienTruTuDiem",
                table: "ThanhToan");

            migrationBuilder.DropColumn(
                name: "DiemTichLuy",
                table: "KhachHang");

            migrationBuilder.DropColumn(
                name: "TongDiemDaTichLuy",
                table: "KhachHang");
        }
    }
}
