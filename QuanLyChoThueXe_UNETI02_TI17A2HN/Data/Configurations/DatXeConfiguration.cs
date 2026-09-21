using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Data.Configurations
{
    public class DatXeConfiguration : IEntityTypeConfiguration<DatXe>
    {
        public void Configure(EntityTypeBuilder<DatXe> builder)
        {
            builder.ToTable("DatXe");

            builder.HasKey(d => d.MaDatXe);

            builder.Property(d => d.ThoiGianNhanDuKien)
                   .IsRequired();

            builder.Property(d => d.ThoiGianTraDuKien)
                   .IsRequired();

            builder.Property(d => d.DiaDiemNhan)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(d => d.DiaDiemTra)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(d => d.NgayDat)
                   .HasDefaultValueSql("GETDATE()");

            builder.Property(d => d.DonGiaApDung)
                   .HasColumnType("decimal(18,0)")
                   .IsRequired();

            builder.Property(d => d.TienCoc)
                   .HasColumnType("decimal(18,0)")
                   .IsRequired();

            builder.Property(d => d.TrangThai)
                   .IsRequired()
                   .HasMaxLength(30);

            builder.Property(d => d.GhiChu)
                   .HasMaxLength(500);

            builder.Property(d => d.LyDoTuChoi)
                   .HasMaxLength(500);

            // Check constraint: ThoiGianTra > ThoiGianNhan
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_DatXe_ThoiGian",
                    "[ThoiGianTraDuKien] > [ThoiGianNhanDuKien]");
                t.HasCheckConstraint("CK_DatXe_Tien",
                    "[DonGiaApDung] > 0 AND [TienCoc] >= 0");
            });

            // 1 - 0..1 voi BanGiaoXe
            builder.HasOne(d => d.BanGiaoXe)
                   .WithOne(b => b.DatXe)
                   .HasForeignKey<BanGiaoXe>(b => b.MaDatXe)
                   .OnDelete(DeleteBehavior.Cascade);

            // 1 - 0..1 voi TraXe
            builder.HasOne(d => d.TraXe)
                   .WithOne(t => t.DatXe)
                   .HasForeignKey<TraXe>(t => t.MaDatXe)
                   .OnDelete(DeleteBehavior.Cascade);

            // 1 - n voi ThanhToan
            builder.HasMany(d => d.ThanhToans)
                   .WithOne(t => t.DatXe)
                   .HasForeignKey(t => t.MaDatXe)
                   .OnDelete(DeleteBehavior.Cascade);

            // Index ho tro kiem tra trung lich
            builder.HasIndex(d => new { d.MaXe, d.TrangThai, d.ThoiGianNhanDuKien, d.ThoiGianTraDuKien })
                   .HasDatabaseName("IX_DatXe_TrungLich");

            // Index ho tro loc danh sach
            builder.HasIndex(d => new { d.MaKhachHang, d.TrangThai })
                   .HasDatabaseName("IX_DatXe_KhachHang_TrangThai");
        }
    }
}
