using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Data.Configurations
{
    public class BanGiaoXeConfiguration : IEntityTypeConfiguration<BanGiaoXe>
    {
        public void Configure(EntityTypeBuilder<BanGiaoXe> builder)
        {
            builder.ToTable("BanGiaoXe");

            builder.HasKey(b => b.MaBanGiao);

            builder.Property(b => b.ThoiGianBanGiao)
                   .IsRequired();

            builder.Property(b => b.SoKmBanGiao)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(b => b.MucNhienLieuBanGiao)
                   .IsRequired();

            builder.Property(b => b.TinhTrangBanGiao)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(b => b.NguoiBanGiao)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(b => b.GhiChu)
                   .HasMaxLength(500);

            // Rang buoc: Muc nhien lieu 0..100
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_BanGiaoXe_NhienLieu",
                    "[MucNhienLieuBanGiao] >= 0 AND [MucNhienLieuBanGiao] <= 100");
                t.HasCheckConstraint("CK_BanGiaoXe_SoKm",
                    "[SoKmBanGiao] >= 0");
            });

            // Rang buoc unique: 1 don chi co 1 ban giao
            builder.HasIndex(b => b.MaDatXe)
                   .IsUnique()
                   .HasDatabaseName("UX_BanGiaoXe_MaDatXe");
        }
    }
}
