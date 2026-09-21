using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Data.Configurations
{
    public class LoaiXeConfiguration : IEntityTypeConfiguration<LoaiXe>
    {
        public void Configure(EntityTypeBuilder<LoaiXe> builder)
        {
            builder.ToTable("LoaiXe");

            builder.HasKey(l => l.MaLoaiXe);

            builder.Property(l => l.TenLoaiXe)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasIndex(l => l.TenLoaiXe)
                   .IsUnique()
                   .HasDatabaseName("UX_LoaiXe_TenLoaiXe");

            builder.Property(l => l.SoCho)
                   .IsRequired();

            builder.Property(l => l.MoTa)
                   .HasMaxLength(500);

            builder.Property(l => l.TrangThai)
                   .HasDefaultValue(true);

            // 1 - n voi Xe
            builder.HasMany(l => l.Xes)
                   .WithOne(x => x.LoaiXe)
                   .HasForeignKey(x => x.MaLoaiXe)
                   .OnDelete(DeleteBehavior.Restrict);

            // 1 - n voi BangGiaThue
            builder.HasMany(l => l.BangGiaThues)
                   .WithOne(b => b.LoaiXe)
                   .HasForeignKey(b => b.MaLoaiXe)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
