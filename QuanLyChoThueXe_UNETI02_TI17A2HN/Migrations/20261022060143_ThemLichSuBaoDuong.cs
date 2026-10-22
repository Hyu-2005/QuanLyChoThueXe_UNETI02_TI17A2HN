// ==============================================================
// Bắt Đầu Code của SV4: Nguyễn Hoàng Đức Hiếu
// file: Migrations/20261022060143_ThemLichSuBaoDuong.cs
// nội dung: STT 11 - Migration bổ sung bảng LichSuBaoDuong theo dõi bảo dưỡng nâng cao
// sinh viên thực hiện : Nguyễn Hoàng Đức Hiếu -  23103100116 - SV4 
// ==============================================================
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Migrations
{
    /// <inheritdoc />
    public partial class ThemLichSuBaoDuong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LichSuBaoDuong",
                columns: table => new
                {
                    MaBaoDuong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaXe = table.Column<int>(type: "int", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayHoanThanh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LoaiBaoDuong = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoKmKhiBaoDuong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ChiPhi = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    GaraBaoDuong = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NoiDungBaoDuong = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    NguoiPhuTrach = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuBaoDuong", x => x.MaBaoDuong);
                    table.ForeignKey(
                        name: "FK_LichSuBaoDuong_Xe_MaXe",
                        column: x => x.MaXe,
                        principalTable: "Xe",
                        principalColumn: "MaXe",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LichSuBaoDuong_MaXe",
                table: "LichSuBaoDuong",
                column: "MaXe");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuBaoDuong_TrangThai",
                table: "LichSuBaoDuong",
                column: "TrangThai");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LichSuBaoDuong");
        }
    }
}
