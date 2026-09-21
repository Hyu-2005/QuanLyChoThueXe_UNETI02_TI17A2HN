using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Data.Configurations
{
    public class XeConfiguration : IEntityTypeConfiguration<Xe>
    {
        public void Configure(EntityTypeBuilder<Xe> builder)
        {
            builder.ToTable("Xe");

            builder.HasKey(x => x.MaXe);

            builder.Property(x => x.BienSo)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.HasIndex(x => x.BienSo)
                   .IsUnique()
                   .HasDatabaseName("UX_Xe_BienSo");

            builder.Property(x => x.TenXe)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.NamSanXuat)
                   .IsRequired();

            builder.Property(x => x.MauSac)
                   .HasMaxLength(30);

            builder.Property(x => x.SoCho)
                   .IsRequired();

            builder.Property(x => x.SoKmHienTai)
                   .HasColumnType("decimal(18,2)")
                   .HasDefaultValue(0m);

            builder.Property(x => x.TinhTrang)
                   .IsRequired()
                   .HasMaxLength(30);

            builder.Property(x => x.MoTa)
                   .HasMaxLength(500);

            builder.Property(x => x.AnhXe)
                   .HasMaxLength(255);

            // 1 - n voi BangGiaThue
            builder.HasMany(x => x.BangGiaThues)
                   .WithOne(b => b.Xe)
                   .HasForeignKey(b => b.MaXe)
                   .OnDelete(DeleteBehavior.Restrict);

            // 1 - n voi DatXe
            builder.HasMany(x => x.DatXes)
                   .WithOne(d => d.Xe)
                   .HasForeignKey(d => d.MaXe)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
