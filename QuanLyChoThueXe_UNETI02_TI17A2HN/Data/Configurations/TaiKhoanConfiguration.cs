using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Data.Configurations
{
    public class TaiKhoanConfiguration : IEntityTypeConfiguration<TaiKhoan>
    {
        public void Configure(EntityTypeBuilder<TaiKhoan> builder)
        {
            builder.ToTable("TaiKhoan");

            builder.HasKey(t => t.MaTaiKhoan);

            builder.Property(t => t.TenDangNhap)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasIndex(t => t.TenDangNhap)
                   .IsUnique()
                   .HasDatabaseName("UX_TaiKhoan_TenDangNhap");

            builder.Property(t => t.MatKhau)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(t => t.HoTen)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(t => t.Email)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(t => t.VaiTro)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(t => t.TrangThai)
                   .HasDefaultValue(true);

            // 1 - 0..1 voi KhachHang
            builder.HasOne(t => t.KhachHang)
                   .WithOne(k => k.TaiKhoan)
                   .HasForeignKey<KhachHang>(k => k.MaTaiKhoan)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
