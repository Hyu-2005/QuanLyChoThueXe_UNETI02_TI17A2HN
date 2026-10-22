using Microsoft.EntityFrameworkCore;
using QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // ===== 10 DbSet chinh =====
        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<LoaiXe> LoaiXes { get; set; }
        public DbSet<HangXe> HangXes { get; set; }
        public DbSet<Xe> Xes { get; set; }
        public DbSet<BangGiaThue> BangGiaThues { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<DatXe> DatXes { get; set; }
        public DbSet<BanGiaoXe> BanGiaoXes { get; set; }
        public DbSet<TraXe> TraXes { get; set; }
        public DbSet<ThanhToan> ThanhToans { get; set; }
        public DbSet<GiayToKhachHang> GiayToKhachHangs { get; set; }
        public DbSet<GiaoDichOnline> GiaoDichOnlines { get; set; }

        // ==============================================================
        // Bắt Đầu Code của SV4: Nguyễn Hoàng Đức Hiếu
        // file: Data/AppDbContext.cs
        // nội dung: STT 11 - Bổ sung bảng LichSuBaoDuong theo dõi bảo dưỡng nâng cao
        // sinh viên thực hiện : Nguyễn Hoàng Đức Hiếu -  23103100116 - SV4 
        // ==============================================================
        public DbSet<LichSuBaoDuong> LichSuBaoDuongs { get; set; }
        // ==============================================================
        // Kết thúc Code của SV4
        // ==============================================================

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ap dung tat ca file cau hinh trong namespace Data.Configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
