using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Data.Configurations
{
    public class TraXeConfiguration : IEntityTypeConfiguration<TraXe>
    {
        public void Configure(EntityTypeBuilder<TraXe> builder)
        {
            builder.ToTable("TraXe");

            builder.HasKey(t => t.MaTraXe);

            builder.Property(t => t.ThoiGianTraThucTe)
                   .IsRequired();

            builder.Property(t => t.SoKmTra)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(t => t.MucNhienLieuTra)
                   .IsRequired();

            builder.Property(t => t.TinhTrangTra)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(t => t.PhiQuaHan)
                   .HasColumnType("decimal(18,0)")
                   .HasDefaultValue(0m);

            builder.Property(t => t.PhiVuotKm)
                   .HasColumnType("decimal(18,0)")
                   .HasDefaultValue(0m);

            builder.Property(t => t.PhiNhienLieu)
                   .HasColumnType("decimal(18,0)")
                   .HasDefaultValue(0m);

            builder.Property(t => t.PhiHuHong)
                   .HasColumnType("decimal(18,0)")
                   .HasDefaultValue(0m);

            builder.Property(t => t.GhiChu)
                   .HasMaxLength(500);

            // Check constraints
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_TraXe_NhienLieu",
                    "[MucNhienLieuTra] >= 0 AND [MucNhienLieuTra] <= 100");
                t.HasCheckConstraint("CK_TraXe_SoKm",
                    "[SoKmTra] >= 0");
                t.HasCheckConstraint("CK_TraXe_PhuPhi",
                    "[PhiQuaHan] >= 0 AND [PhiVuotKm] >= 0 AND [PhiNhienLieu] >= 0 AND [PhiHuHong] >= 0");
            });

            // Rang buoc unique: 1 don chi co 1 tra xe
            builder.HasIndex(t => t.MaDatXe)
                   .IsUnique()
                   .HasDatabaseName("UX_TraXe_MaDatXe");
        }
    }
}
