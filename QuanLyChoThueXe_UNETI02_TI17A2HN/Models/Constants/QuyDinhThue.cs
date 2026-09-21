namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Models.Constants
{
    public static class QuyDinhThue
    {
        /// <summary>Số km tối đa cho phép mỗi ngày thuê (km)</summary>
        public const decimal KmChoPhepMoiNgay = 300m;

        /// <summary>Phí vượt km (đồng/km)</summary>
        public const decimal PhiVuotKmMoiKm = 3000m;

        /// <summary>Hệ số phí quá hạn (so với đơn giá giờ)</summary>
        public const decimal HeSoPhiQuaHan = 1.5m;

        /// <summary>Phí thiếu nhiên liệu (% → đồng mỗi 1%)</summary>
        public const decimal PhiNhienLieuMoiPhanTram = 20000m;

        /// <summary>Số giờ tối thiểu tính theo giờ (nếu thuê ngắn hơn → tính 1 đơn vị)</summary>
        public const int SoGioToiThieu = 1;

        /// <summary>Số ngày tối thiểu tính theo ngày</summary>
        public const int SoNgayToiThieu = 1;

        /// <summary>Ngưỡng phân biệt thuê theo giờ / theo ngày (giờ)</summary>
        public const int NguongGioSangNgay = 24;

        /// <summary>Thời hạn tối thiểu khách được hủy trước giờ nhận (giờ)</summary>
        public const int SoGioToiThieuTruocKhiHuy = 24;
    }
}
