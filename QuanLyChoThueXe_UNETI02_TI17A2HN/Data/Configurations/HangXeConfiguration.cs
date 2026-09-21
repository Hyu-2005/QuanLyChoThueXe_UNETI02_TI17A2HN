using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Data.Configurations
{
    public class HangXeConfiguration : IEntityTypeConfiguration<HangXe>
    {
        public void Configure(EntityTypeBuilder<HangXe> builder)
        {
            builder.ToTable("HangXe");

            builder.HasKey(h => h.MaHangXe);

            builder.Property(h => h.TenHangXe)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasIndex(h => h.TenHangXe)
                   .IsUnique()
                   .HasDatabaseName("UX_HangXe_TenHangXe");

            builder.Property(h => h.QuocGia)
                   .HasMaxLength(50);

            builder.Property(h => h.MoTa)
                   .HasMaxLength(500);

            builder.Property(h => h.TrangThai)
                   .HasDefaultValue(true);

            // 1 - n voi Xe
            builder.HasMany(h => h.Xes)
                   .WithOne(x => x.HangXe)
                   .HasForeignKey(x => x.MaHangXe)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
