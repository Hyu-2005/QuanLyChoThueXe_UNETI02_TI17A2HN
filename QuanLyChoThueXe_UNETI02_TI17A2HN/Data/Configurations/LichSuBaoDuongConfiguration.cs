// ==============================================================
// Bắt Đầu Code của SV4: Nguyễn Hoàng Đức Hiếu
// file: Data/Configurations/LichSuBaoDuongConfiguration.cs
// nội dung: STT 11 - Cấu hình Fluent API cho bảng LichSuBaoDuong
// sinh viên thực hiện : Nguyễn Hoàng Đức Hiếu -  23103100116 - SV4 
// ==============================================================
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Data.Configurations
{
    public class LichSuBaoDuongConfiguration : IEntityTypeConfiguration<LichSuBaoDuong>
    {
        public void Configure(EntityTypeBuilder<LichSuBaoDuong> builder)
        {
            builder.ToTable("LichSuBaoDuong");

            builder.HasKey(b => b.MaBaoDuong);

            builder.Property(b => b.NgayBatDau)
                   .IsRequired();

            builder.Property(b => b.LoaiBaoDuong)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(b => b.SoKmKhiBaoDuong)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(b => b.ChiPhi)
                   .HasColumnType("decimal(18,0)")
                   .IsRequired();

            builder.Property(b => b.GaraBaoDuong)
                   .HasMaxLength(200);

            builder.Property(b => b.NoiDungBaoDuong)
                   .IsRequired()
                   .HasMaxLength(1000);

            builder.Property(b => b.NguoiPhuTrach)
                   .HasMaxLength(100);

            builder.Property(b => b.TrangThai)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(b => b.GhiChu)
                   .HasMaxLength(500);

            // Quan hệ với bảng Xe
            builder.HasOne(b => b.Xe)
                   .WithMany(x => x.LichSuBaoDuongs)
                   .HasForeignKey(b => b.MaXe)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(b => b.MaXe)
                   .HasDatabaseName("IX_LichSuBaoDuong_MaXe");

            builder.HasIndex(b => b.TrangThai)
                   .HasDatabaseName("IX_LichSuBaoDuong_TrangThai");
        }
    }
}
// ==============================================================
// Kết thúc Code của SV4
// ==============================================================
