using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Data.Configurations
{
    public class KhachHangConfiguration : IEntityTypeConfiguration<KhachHang>
    {
        public void Configure(EntityTypeBuilder<KhachHang> builder)
        {
            builder.ToTable("KhachHang");

            builder.HasKey(k => k.MaKhachHang);

            builder.Property(k => k.HoTen)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(k => k.SoDienThoai)
                   .IsRequired()
                   .HasMaxLength(15);

            builder.Property(k => k.Email)
                   .HasMaxLength(100);

            builder.Property(k => k.DiaChi)
                   .HasMaxLength(200);

            builder.Property(k => k.SoGiayPhepLaiXe)
                   .HasMaxLength(30);

            builder.Property(k => k.NgayHetHanGPLX)
                   .HasColumnType("date");

            builder.Property(k => k.TrangThai)
                   .HasDefaultValue(true);

            // 1 - n voi DatXe
            builder.HasMany(k => k.DatXes)
                   .WithOne(d => d.KhachHang)
                   .HasForeignKey(d => d.MaKhachHang)
                   .OnDelete(DeleteBehavior.Restrict);

            // Index ho tro tim kiem
            builder.HasIndex(k => k.SoDienThoai)
                   .HasDatabaseName("IX_KhachHang_SoDienThoai");
        }
    }
}
