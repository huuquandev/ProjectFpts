using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSiteBanHang.Areas.Admin.Commons;
using WebSiteBanHang.Commons;
using WebSiteBanHang.Models;

namespace WebSiteBanHang.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        // GET: Admin/TrangChu
        dbWebSiteBanHang db = new dbWebSiteBanHang();

        public User getItem(String username)
        {
            return db.Users.FirstOrDefault(x => x.username == username);
        }
        public List<User> getListUser()
        {
            return db.Users.ToList();
        }
        public Role getRole(int roleId)
        {
            return db.Roles.FirstOrDefault(r => r.id == roleId);
        }
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult login(User _User)
        {
            if (ModelState.IsValid)
            {
                var result = getItem(_User.username);
                if (result != null)
                {
                    if (result.password == _User.password)
                    {
                        var User = getItem(_User.username);
                        var session = new User
                        {
                            Id = User.Id,
                            IdTaiKhoan = User.IdTaiKhoan,
                            ho = User.ho,
                            ten = User.ten,
                            Addresses = User.Addresses,
                            password = User.password,
                            role_id = User.role_id,
                            sodt = User.sodt,
                            ngaysinh = User.ngaysinh,
                            gioitinh = User.gioitinh,
                            Role = getRole((int)User.role_id)
                        };
                        Session.Add(sessionLogin.USER_SESSION, session);

                            return RedirectToAction("main", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Thông tin đăng nhập không chính xác.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Thông tin đăng nhập không chính xác.");
                }

            }
            return View("Index");
        }
        [AdminAuthorizationFilter]
        public ActionResult main()
        {
            List<Order> danhSachDonHang = db.Orders.ToList();
            return View(danhSachDonHang);
        }
        public ActionResult Logout()
        {
            Session.Remove(sessionLogin.USER_SESSION);
            return RedirectToAction("Index", "Home");
        }
    }
}