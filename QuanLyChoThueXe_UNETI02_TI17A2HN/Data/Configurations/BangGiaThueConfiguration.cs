using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Data.Configurations
{
    public class BangGiaThueConfiguration : IEntityTypeConfiguration<BangGiaThue>
    {
        public void Configure(EntityTypeBuilder<BangGiaThue> builder)
        {
            builder.ToTable("BangGiaThue");

            builder.HasKey(b => b.MaBangGia);

            builder.Property(b => b.DonGiaNgay)
                   .HasColumnType("decimal(18,0)")
                   .IsRequired();

            builder.Property(b => b.DonGiaGio)
                   .HasColumnType("decimal(18,0)")
                   .IsRequired();

            builder.Property(b => b.TienCocMacDinh)
                   .HasColumnType("decimal(18,0)")
                   .IsRequired();

            builder.Property(b => b.TuNgay)
                   .IsRequired();

            builder.Property(b => b.DenNgay)
                   .IsRequired();

            builder.Property(b => b.TrangThai)
                   .HasDefaultValue(true);

            // Rang buoc nghiep vu: DenNgay > TuNgay (check constraint)
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_BangGiaThue_KhoangHieuLuc",
                    "[DenNgay] > [TuNgay]");
                t.HasCheckConstraint("CK_BangGiaThue_DonGia",
                    "[DonGiaNgay] > 0 AND [DonGiaGio] >= 0 AND [TienCocMacDinh] >= 0");
                t.HasCheckConstraint("CK_BangGiaThue_PhamViApDung",
                    "([MaLoaiXe] IS NOT NULL AND [MaXe] IS NULL) OR ([MaLoaiXe] IS NULL AND [MaXe] IS NOT NULL)");
            });

            // Index ho tro truy van tim bang gia hieu luc
            builder.HasIndex(b => new { b.MaLoaiXe, b.MaXe, b.TrangThai })
                   .HasDatabaseName("IX_BangGiaThue_PhamVi_TrangThai");
        }
    }
}
