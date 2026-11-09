using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Data.Configurations
{
    public class MaGiamGiaConfiguration : IEntityTypeConfiguration<MaGiamGia>
    {
        public void Configure(EntityTypeBuilder<MaGiamGia> builder)
        {
            builder.ToTable("MaGiamGia");
            builder.HasKey(v => v.MaGiamGiaId);

            builder.Property(v => v.Code)
                   .IsRequired()
                   .HasMaxLength(30);
            builder.HasIndex(v => v.Code)
                   .IsUnique()
                   .HasDatabaseName("UX_MaGiamGia_Code");

            builder.Property(v => v.TenChuongTrinh)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(v => v.MoTa).HasMaxLength(500);

            builder.Property(v => v.LoaiGiamGia)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(v => v.GiaTriGiam)
                   .HasColumnType("decimal(18,0)")
                   .IsRequired();

            builder.Property(v => v.GiamToiDa)
                   .HasColumnType("decimal(18,0)");

            builder.Property(v => v.DonHangToiThieu)
                   .HasColumnType("decimal(18,0)")
                   .HasDefaultValue(0m);

            builder.Property(v => v.NgayBatDau).IsRequired();
            builder.Property(v => v.NgayKetThuc).IsRequired();

            builder.Property(v => v.SoLuotDungToiDa).HasDefaultValue(100);
            builder.Property(v => v.SoLuotDaDung).HasDefaultValue(0);
            builder.Property(v => v.MoiKhachMotLan).HasDefaultValue(true);
            builder.Property(v => v.TrangThai).HasDefaultValue(true);

            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_MaGiamGia_GiaTri",
                    "[GiaTriGiam] >= 0 AND [DonHangToiThieu] >= 0");
                t.HasCheckConstraint("CK_MaGiamGia_ThoiGian",
                    "[NgayKetThuc] > [NgayBatDau]");
            });
        }
    }
}