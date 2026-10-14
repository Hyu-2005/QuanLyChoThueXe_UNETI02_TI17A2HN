using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Migrations
{
    /// <inheritdoc />
    public partial class AddGiaoDichOnline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GiaoDichOnline",
                columns: table => new
                {
                    MaGiaoDich = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaThanhToan = table.Column<int>(type: "int", nullable: false),
                    MaGiaoDichCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CongThanhToan = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    SoTien = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayThanhToan = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiaoDichOnline", x => x.MaGiaoDich);
                    table.CheckConstraint("CK_GiaoDichOnline_SoTien", "[SoTien] > 0");
                    table.ForeignKey(
                        name: "FK_GiaoDichOnline_ThanhToan_MaThanhToan",
                        column: x => x.MaThanhToan,
                        principalTable: "ThanhToan",
                        principalColumn: "MaThanhToan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GiaoDichOnline_MaThanhToan",
                table: "GiaoDichOnline",
                column: "MaThanhToan");

            migrationBuilder.CreateIndex(
                name: "IX_GiaoDichOnline_TrangThai_NgayTao",
                table: "GiaoDichOnline",
                columns: new[] { "TrangThai", "NgayTao" });

            migrationBuilder.CreateIndex(
                name: "UX_GiaoDichOnline_MaGiaoDichCode",
                table: "GiaoDichOnline",
                column: "MaGiaoDichCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GiaoDichOnline");
        }
    }
}
