using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Migrations
{
    /// <inheritdoc />
    public partial class ThemTinhNangGPS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "KinhDoBanGiao",
                table: "BanGiaoXe",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ViDoBanGiao",
                table: "BanGiaoXe",
                type: "float",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LichSuDiChuyen",
                columns: table => new
                {
                    MaDiChuyen = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDatXe = table.Column<int>(type: "int", nullable: false),
                    ViDo = table.Column<double>(type: "float", nullable: false),
                    KinhDo = table.Column<double>(type: "float", nullable: false),
                    ThoiGian = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuDiChuyen", x => x.MaDiChuyen);
                    table.ForeignKey(
                        name: "FK_LichSuDiChuyen_DatXe_MaDatXe",
                        column: x => x.MaDatXe,
                        principalTable: "DatXe",
                        principalColumn: "MaDatXe",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LichSuDiChuyen_MaDatXe",
                table: "LichSuDiChuyen",
                column: "MaDatXe");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LichSuDiChuyen");

            migrationBuilder.DropColumn(
                name: "KinhDoBanGiao",
                table: "BanGiaoXe");

            migrationBuilder.DropColumn(
                name: "ViDoBanGiao",
                table: "BanGiaoXe");
        }
    }
}
