// PasswordHelper.cs - ho tro ma hoa mat khau
// Họ và tên: Pham Gia Minh Hoang
// Mã sinh viên: 23103100087
// Nội dung thực hiện: Mã hóa và kiểm tra mật khẩu bằng BCrypt
// Module: Module 1 (nâng cao)

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Helpers
{
    public static class PasswordHelper
    {
        /// Ma hoa mat khau bang BCrypt
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// Kiem tra mat khau voi hash
        /// Ho tro ca truong hop hash cu la plaintext (de migrate)
        public static bool VerifyPassword(string password, string hash)
        {
            // Neu hash khong phai BCrypt (vi du plaintext cu) thi so sanh truc tiep
            if (string.IsNullOrEmpty(hash) || !hash.StartsWith("$2"))
            {
                return password == hash;
            }

            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}