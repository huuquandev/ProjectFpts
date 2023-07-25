using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSiteBanHang.Commons;
using WebSiteBanHang.Models;

namespace WebSiteBanHang.Areas.Admin.Commons
{
    public class AdminAuthorizationFilter : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var user = httpContext.Session[sessionLogin.USER_SESSION] as User;
            if (user != null)
            {
                if(user.role_id == 1  || user.role_id == 3)
                {
                    return true; // Cho phép truy cập nếu là admin và nhân viên
                }
                return false; // Không cho phép truy cập nếu không phải admin

            }
            return false; // Không cho phép truy cập nếu chưa đăng nhập
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Chuyển hướng người dùng đến trang đăng nhập nếu chưa đăng nhập
            filterContext.Result = new RedirectResult("~/Admin/Home/Index");
        }
    }
}