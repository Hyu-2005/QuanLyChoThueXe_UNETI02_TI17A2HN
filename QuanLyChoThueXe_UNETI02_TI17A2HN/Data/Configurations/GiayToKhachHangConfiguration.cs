// ============================================================
// File: Data/Configurations/GiayToKhachHangConfiguration.cs
// Họ và tên: [Vũ Tiến Đạt]
// Mã sinh viên: [23103100119]
// Nội dung thực hiện: Cấu hình Fluent API cho bảng GiayToKhachHang
// Module: Module 3 - Khách hàng
// ============================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Data.Configurations
{
    public class GiayToKhachHangConfiguration : IEntityTypeConfiguration<GiayToKhachHang>
    {
        public void Configure(EntityTypeBuilder<GiayToKhachHang> builder)
        {
            builder.ToTable("GiayToKhachHang");

            builder.HasKey(g => g.MaGiayTo);

            builder.Property(g => g.LoaiGiayTo).IsRequired().HasMaxLength(10);
            builder.Property(g => g.SoGiayTo).IsRequired().HasMaxLength(30);
            builder.Property(g => g.NgayHetHan).HasColumnType("date");
            builder.Property(g => g.DuongDanMatTruoc).IsRequired().HasMaxLength(300);
            builder.Property(g => g.DuongDanMatSau).HasMaxLength(300);
            builder.Property(g => g.TrangThai).IsRequired().HasMaxLength(20)
                   .HasDefaultValue(TrangThaiGiayTo.ChoKiemTra);
            builder.Property(g => g.LyDoTuChoi).HasMaxLength(300);
            builder.Property(g => g.NguoiKiemTra).HasMaxLength(100);

            // Moi khach hang chi co 1 ho so cho moi loai giay to
            builder.HasIndex(g => new { g.MaKhachHang, g.LoaiGiayTo })
                   .IsUnique()
                   .HasDatabaseName("UX_GiayToKhachHang_KhachHang_Loai");

            // 1 - n voi KhachHang (khong can sua file KhachHang.cs)
            builder.HasOne(g => g.KhachHang)
                   .WithMany()
                   .HasForeignKey(g => g.MaKhachHang)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
