// File: Helpers/SessionExtensions.cs
// Noi dung: Extension methods de lam viec voi Session
// Sinh vien thuc hien: Pham Gia Minh Hoang - 23103100087 - SV1
// Module: Module 1 - Tai khoan, Dang nhap, Phan quyen, Loai xe, Hang xe

using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Helpers
{
    public static class SessionExtensions
    {
        public static void SetInt(this ISession session, string key, int value)
        {
            session.SetString(key, value.ToString());
        }

        public static int? GetInt(this ISession session, string key)
        {
            var value = session.GetString(key);
            return int.TryParse(value, out var result) ? result : null;
        }

        public static void SetObject<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T? GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }

        // ====== Helpers tien ich cho phan quyen ======
        public static bool DaDangNhap(this ISession session)
        {
            return session.GetInt(SessionKeys.MaTaiKhoan).HasValue;
        }

        public static string? GetVaiTro(this ISession session)
        {
            return session.GetString(SessionKeys.VaiTro);
        }

        public static int? GetMaTaiKhoan(this ISession session)
        {
            return session.GetInt(SessionKeys.MaTaiKhoan);
        }

        public static int? GetMaKhachHang(this ISession session)
        {
            return session.GetInt(SessionKeys.MaKhachHang);
        }

        public static bool LaAdmin(this ISession session)
        {
            return session.GetVaiTro() == Models.Constants.VaiTro.Admin;
        }

        public static bool LaNhanVien(this ISession session)
        {
            return session.GetVaiTro() == Models.Constants.VaiTro.NhanVien;
        }

        public static bool LaKhachHang(this ISession session)
        {
            return session.GetVaiTro() == Models.Constants.VaiTro.KhachHang;
        }

        public static bool LaQuanTri(this ISession session)
        {
            var vt = session.GetVaiTro();
            return vt == Models.Constants.VaiTro.Admin
                || vt == Models.Constants.VaiTro.NhanVien;
        }
    }
}