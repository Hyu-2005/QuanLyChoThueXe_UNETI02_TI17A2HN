using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Entities
{
    [Table("KhachHang")]
    public class KhachHang
    {
        [Key]
        public int MaKhachHang { get; set; }

        [Display(Name = "Tài khoản liên kết")]
        public int? MaTaiKhoan { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100)]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
        [StringLength(15)]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [StringLength(200)]
        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }

        [StringLength(30)]
        [Display(Name = "Số giấy phép lái xe")]
        public string? SoGiayPhepLaiXe { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày hết hạn GPLX")]
        public DateTime? NgayHetHanGPLX { get; set; }

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true;

        // ===== Navigation =====
        [ForeignKey(nameof(MaTaiKhoan))]
        public TaiKhoan? TaiKhoan { get; set; }

        public ICollection<DatXe> DatXes { get; set; } = new List<DatXe>();

        /// <summary>
        /// Kiểm tra GPLX còn hiệu lực tại thời điểm nhận xe
        /// </summary>
        public bool GPLXConHieuLuc(DateTime thoiDiemNhan)
        {
            return NgayHetHanGPLX.HasValue && NgayHetHanGPLX.Value.Date >= thoiDiemNhan.Date;
        }
    }
}
