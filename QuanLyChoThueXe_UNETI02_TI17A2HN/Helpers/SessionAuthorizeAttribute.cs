// File: Helpers/SessionAuthorizeAttribute.cs
// Noi dung: Attribute kiem tra dang nhap + phan quyen tai Controller
// Sinh vien thuc hien: Pham Gia Minh Hoang - 23103100087 - SV1
// Module: Module 1 - Tai khoan, Dang nhap, Phan quyen, Loai xe, Hang xe

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QuanLyChoThueXe_UNETI02_TI17A2HN.Helpers
{
    /// Kiem tra dang nhap va phan quyen dua tren Session.
    /// Neu chua dang nhap: chuyen ve trang DangNhap.
    /// Neu khong du quyen: chuyen ve trang AccessDenied.
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        /// Danh sach vai tro duoc phep truy cap.
        /// Neu rong: chi can dang nhap.
        public string[] Roles { get; set; } = Array.Empty<string>();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;

            // 1. Kiem tra da dang nhap chua
            if (!session.DaDangNhap())
            {
                // Neu la Ajax request thi tra JSON
                if (context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    context.Result = new JsonResult(new { success = false, message = "Chua dang nhap" })
                    {
                        StatusCode = 401
                    };
                    return;
                }

                var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
                context.Result = new RedirectToActionResult(
                    "DangNhap", "TaiKhoan",
                    new { returnUrl = returnUrl.ToString() });
                return;
            }

            // 2. Kiem tra vai tro(neu co yeu cau)
            if (Roles.Length > 0)
            {
                var vaiTro = session.GetVaiTro();
                if (string.IsNullOrEmpty(vaiTro) || !Roles.Contains(vaiTro))
                {
                    context.Result = new RedirectToActionResult(
                        "AccessDenied", "TaiKhoan", null);
                    return;
                }
            }

            base.OnActionExecuting(context);
        }
    }
}