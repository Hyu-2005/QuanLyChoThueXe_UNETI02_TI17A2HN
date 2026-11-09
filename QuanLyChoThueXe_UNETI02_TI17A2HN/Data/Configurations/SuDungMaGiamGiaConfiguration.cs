using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Data.Configurations
{
    public class SuDungMaGiamGiaConfiguration : IEntityTypeConfiguration<SuDungMaGiamGia>
    {
        public void Configure(EntityTypeBuilder<SuDungMaGiamGia> builder)
        {
            builder.ToTable("SuDungMaGiamGia");
            builder.HasKey(s => s.MaSuDung);

            builder.Property(s => s.SoTienDuocGiam).HasColumnType("decimal(18,0)");
            builder.Property(s => s.NgaySuDung).HasDefaultValueSql("GETDATE()");

            builder.HasOne(s => s.MaGiamGia)
                   .WithMany(v => v.SuDungMaGiamGias)
                   .HasForeignKey(s => s.MaGiamGiaId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.KhachHang)
                   .WithMany()
                   .HasForeignKey(s => s.MaKhachHang)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.DatXe)
                   .WithMany()
                   .HasForeignKey(s => s.MaDatXe)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(s => new { s.MaGiamGiaId, s.MaKhachHang })
                   .HasDatabaseName("IX_SuDungMaGiamGia_Voucher_Khach");
        }
    }
}