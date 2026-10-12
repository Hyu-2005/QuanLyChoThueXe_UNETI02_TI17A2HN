using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Migrations
{
    /// <inheritdoc />
    public partial class ThemGiayToKhachHang : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GiayToKhachHang",
                columns: table => new
                {
                    MaGiayTo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKhachHang = table.Column<int>(type: "int", nullable: false),
                    LoaiGiayTo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SoGiayTo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    NgayHetHan = table.Column<DateTime>(type: "date", nullable: true),
                    DuongDanMatTruoc = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    DuongDanMatSau = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Chờ kiểm tra"),
                    LyDoTuChoi = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    NgayTai = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKiemTra = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NguoiKiemTra = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiayToKhachHang", x => x.MaGiayTo);
                    table.ForeignKey(
                        name: "FK_GiayToKhachHang_KhachHang_MaKhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "KhachHang",
                        principalColumn: "MaKhachHang",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "UX_GiayToKhachHang_KhachHang_Loai",
                table: "GiayToKhachHang",
                columns: new[] { "MaKhachHang", "LoaiGiayTo" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GiayToKhachHang");
        }
    }
}
