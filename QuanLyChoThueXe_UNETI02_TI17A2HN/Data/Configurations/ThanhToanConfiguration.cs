using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Data.Configurations
{
    public class ThanhToanConfiguration : IEntityTypeConfiguration<ThanhToan>
    {
        public void Configure(EntityTypeBuilder<ThanhToan> builder)
        {
            builder.ToTable("ThanhToan");

            builder.HasKey(t => t.MaThanhToan);

            builder.Property(t => t.TienThue)
                   .HasColumnType("decimal(18,0)")
                   .IsRequired();

            builder.Property(t => t.TongPhuPhi)
                   .HasColumnType("decimal(18,0)")
                   .HasDefaultValue(0m);

            builder.Property(t => t.TienCocDaThu)
                   .HasColumnType("decimal(18,0)")
                   .HasDefaultValue(0m);

            builder.Property(t => t.TongThanhToan)
                   .HasColumnType("decimal(18,0)")
                   .IsRequired();

            builder.Property(t => t.SoTienConLai)
                   .HasColumnType("decimal(18,0)")
                   .HasDefaultValue(0m);

            builder.Property(t => t.PhuongThucThanhToan)
                   .IsRequired()
                   .HasMaxLength(30);

            builder.Property(t => t.NgayThanhToan)
                   .HasDefaultValueSql("GETDATE()");

            builder.Property(t => t.TrangThaiThanhToan)
                   .IsRequired()
                   .HasMaxLength(30);

            builder.Property(t => t.GhiChu)
                   .HasMaxLength(500);

            // Check constraint
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_ThanhToan_Tien",
                    "[TienThue] >= 0 AND [TongPhuPhi] >= 0 AND [TienCocDaThu] >= 0 AND [TongThanhToan] >= 0 AND [SoTienConLai] >= 0");
            });

            // Index ho tro thong ke
            builder.HasIndex(t => new { t.NgayThanhToan, t.TrangThaiThanhToan })
                   .HasDatabaseName("IX_ThanhToan_Ngay_TrangThai");
        }
    }
}
