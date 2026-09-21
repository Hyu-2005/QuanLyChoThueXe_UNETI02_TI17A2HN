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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ap dung tat ca file cau hinh trong namespace Data.Configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
