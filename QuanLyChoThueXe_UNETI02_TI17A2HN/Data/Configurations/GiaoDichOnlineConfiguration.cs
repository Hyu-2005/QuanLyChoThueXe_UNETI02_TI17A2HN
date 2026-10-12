using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Data.Configurations
{
    public class GiaoDichOnlineConfiguration : IEntityTypeConfiguration<GiaoDichOnline>
    {
        public void Configure(EntityTypeBuilder<GiaoDichOnline> builder)
        {
            builder.ToTable("GiaoDichOnline");
            builder.HasKey(g => g.MaGiaoDich);

            builder.Property(g => g.MaGiaoDichCode)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.HasIndex(g => g.MaGiaoDichCode)
                   .IsUnique()
                   .HasDatabaseName("UX_GiaoDichOnline_MaGiaoDichCode");

            builder.Property(g => g.CongThanhToan).IsRequired().HasMaxLength(30);
            builder.Property(g => g.SoTien).HasColumnType("decimal(18,0)").IsRequired();
            builder.Property(g => g.TrangThai).IsRequired().HasMaxLength(30);
            builder.Property(g => g.GhiChu).HasMaxLength(500);

            // Check constraint: số tiền > 0
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_GiaoDichOnline_SoTien", "[SoTien] > 0");
            });

            // 1 - n với ThanhToan
            builder.HasOne(g => g.ThanhToan)
                   .WithMany()
                   .HasForeignKey(g => g.MaThanhToan)
                   .OnDelete(DeleteBehavior.Cascade);

            // Index hỗ trợ tra cứu
            builder.HasIndex(g => new { g.TrangThai, g.NgayTao })
                   .HasDatabaseName("IX_GiaoDichOnline_TrangThai_NgayTao");
        }
    }
}