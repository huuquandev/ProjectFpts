using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WebSiteBanHang.Commons;
using WebSiteBanHang.Models;
using WebSiteBanHang.Models.Commom;
using static System.Collections.Specialized.BitVector32;

namespace WebSiteBanHang.Controllers
{
    public class TaiKhoanController : Controller
    {
        // GET: TaiKhoan
        dbWebSiteBanHang db = new dbWebSiteBanHang();
        public User getItem(String email)
        {
            return db.Users.FirstOrDefault(x => x.email == email);
        }
        public List<User> getListUser()
        {
            return db.Users.ToList();
        }
        public Role getRole(int roleId)
        {
            return db.Roles.FirstOrDefault(r => r.id == roleId);
        }
        public ActionResult login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UserStart(User _User)
        {
            if (ModelState.IsValid)
            {
                Session.Remove("Count");
                var result = getItem(_User.email);
                if (result != null)
                {
                    if (result.password == _User.password)
                    {
                        var User = getItem(_User.email);
                        var session = new User
                        {
                            Id = User.Id,
                            IdTaiKhoan = User.IdTaiKhoan,
                            ho = User.ho,
                            ten = User.ten,
                            email = User.email,
                            Addresses = User.Addresses,
                            password = User.password,
                            role_id = User.role_id,
                            sodt = User.sodt,
                            ngaysinh = User.ngaysinh, 
                            gioitinh = User.gioitinh,
                            Role = getRole((int)User.role_id)
                        }; 
                        Session.Add(sessionLogin.USER_SESSION, session);

                        Cart cart = db.Carts.FirstOrDefault(c => c.id_user == User.Id);
                        if(cart != null)
                        {
                            List<CartItem> cartItems = db.CartItems.Where(ci => ci.id_cart == cart.id).ToList();
                            Session["CountUser"] = cartItems.Count;
                        }
                        // Kiểm tra và chuyển hướng trở lại trang Checkout nếu có thông tin returnUrl
                        return RedirectToAction("accountDetail", "TaiKhoan");
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
            return View("login");
        }
        public ActionResult Logout()
        {
            Session.Remove(sessionLogin.USER_SESSION); // Xóa session người dùng;
            Session.Remove("CountUser");
            return RedirectToAction("Index", "TrangChu"); // Chuyển hướng đến trang chủ
        }
        public ActionResult register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RegisterStart(User _User)
        {
            if(ModelState.IsValid)
            {
                var result = getItem(_User.email);
                if (result == null)
                {
                    //Tạo mã tài khoản ngẫu nhiên
                    Random random = new Random();
                    string newUserCode;
                    do
                    {
                        int randomNumber = random.Next(1000, 9999);
                        newUserCode = "SP" + randomNumber.ToString();
                    } while (db.Orders.Any(o => o.madh == newUserCode));

                    var newUser = new User
                    {                        
                        ho = _User.ho,
                        ten = _User.ten,
                        IdTaiKhoan = newUserCode,
                        email = _User.email,
                        password = _User.password,
                        role_id = 2,
                        sodt = _User.sodt,
                        ngaysinh = _User.ngaysinh,
                        gioitinh = _User.gioitinh                       
                    };
                    db.Users.Add(newUser);
                    db.SaveChanges();
                    Session.Add(sessionLogin.USER_SESSION, newUser);
                    return RedirectToAction("Index", "TrangChu");
                }
                else
                {
                    ModelState.AddModelError("", "Email đã tồn tại.");
                }
            }
            return View("register");
        }
        public ActionResult accountDetail()
        {
            if (Session[sessionLogin.USER_SESSION] != null)
            {              
                return View();
            }
            else
            {
                // Session không tồn tại, chuyển hướng đến trang đăng nhập
                return RedirectToAction("login");
            }
        }
        public ActionResult addresses()
        {
            if (Session[sessionLogin.USER_SESSION] != null)
            {
                var userSession = (User)Session[sessionLogin.USER_SESSION];

                List<Address> addresses = db.Addresses.Where(ad => ad.userId == userSession.Id).ToList();

                if (addresses.Count > 0)
                {
                    addressModel model = new addressModel
                    {
                        Addresses = addresses,
                        NewAddress = new Address()
                    };

                    return View(model);
                }
                else
                {
                    addressModel model = new addressModel
                    {
                        Addresses = new List<Address>(),
                        NewAddress = new Address()
                    };

                    return View(model);
                }
            }
            return RedirectToAction("login", "TaiKhoan");
        }
        [HttpPost]
        public ActionResult AddToaAddress(addressModel address, int userId)
        {
            Address addressUser = db.Addresses.FirstOrDefault(ad => ad.userId == userId);
            if (addressUser != null)
            {
                if (address.NewAddress.diachimacdinh == 1) // Kiểm tra địa chỉ mới có phải là địa chỉ mặc định không
                {
                    // Cập nhật tất cả các địa chỉ của người dùng thành không mặc định (0)
                    var userAddresses = db.Addresses.Where(ad => ad.userId == userId);
                    foreach (var userAddress in userAddresses)
                    {
                        userAddress.diachimacdinh = 0;
                    }
                }

                db.Addresses.Add(new Address
                {
                    hoten = address.NewAddress.hoten,
                    userId = userId,
                    sodt = address.NewAddress.sodt,
                    congty = address.NewAddress.congty,
                    diachi = address.NewAddress.diachi,
                    quocgia = address.NewAddress.quocgia,
                    tinhthanh = address.NewAddress.tinhthanh,
                    quanhuyen = address.NewAddress.quanhuyen,
                    phuongxa = address.NewAddress.phuongxa,
                    mazip = address.NewAddress.mazip,
                    diachimacdinh = address.NewAddress.diachimacdinh
                });
                db.SaveChanges();
                return RedirectToAction("addresses", "TaiKhoan");
            }
            return View();
        }
        [HttpPost]
        public JsonResult RemoveToAddress(int addressId)
        {
            var userSession = (User)Session[sessionLogin.USER_SESSION];
            int customerId = userSession.Id;
            Address addressUser = db.Addresses.FirstOrDefault(ad => ad.userId == customerId);
            List<Address> addresses = db.Addresses.Where(ad => ad.userId == userSession.Id).ToList();
            var addressToRemove = addresses.FirstOrDefault(item => item.Id == addressId);

            if (addressToRemove != null)
            {
                db.Addresses.Remove(addressToRemove);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        public ActionResult Orders()
        {
            if (Session[sessionLogin.USER_SESSION] != null)
            {
                var userSession = (User)Session[sessionLogin.USER_SESSION];
                List<Order> orderList = db.Orders.Where(o => o.id_user == userSession.Id).ToList();
                return View(orderList);
            }
            return RedirectToAction("login", "TaiKhoan");
        }
        public ActionResult OrderDetail(int orderId)
        {
            var order = db.Orders.FirstOrDefault(o => o.id == orderId);
            return View(order);
        }
        public ActionResult ChangePassword()
        {
            if (Session[sessionLogin.USER_SESSION] != null)
            {
                return View();
            }
            return RedirectToAction("login", "TaiKhoan");
        }
        [HttpPost]
        public ActionResult ChangePassword(string oldPassword, string newPassword, string cfnewPassword)
        {           
            if (ModelState.IsValid)
            {
                if (Session[sessionLogin.USER_SESSION] != null)
                {
                    ViewBag.error = 0;
                    var userSession = (User)Session[sessionLogin.USER_SESSION];

                    if (oldPassword == userSession.password)
                    {
                        if(newPassword == cfnewPassword)
                        {
                            if(newPassword.Length > 6)
                            {
                                var user = db.Users.FirstOrDefault(u => u.Id == userSession.Id);
                                if (user != null)
                                {
                                    user.password = newPassword;
                                }
                                db.Users.AddOrUpdate(user);
                                db.SaveChanges();
                                TempData["SuccessMessage"] = "Mật khẩu đã được thay đổi thành công.";

                            }
                            else
                            {
                                ViewBag.error = 1;
                                ModelState.AddModelError("", "Mật khẩu mới dài từ 6 đến 50 ký tự.");  
                            }
                        }
                        else
                        {
                            ViewBag.error = 1;
                            ModelState.AddModelError("", "Xác nhận mật khẩu không khớp.");
                        }
                    }
                    else
                    {
                        ViewBag.error = 1;
                        ModelState.AddModelError("", "Mật khẩu không đúng.");

                    }

                }
                else
                {
                    return RedirectToAction("login", "TaiKhoan");
                }

            }
            return View();
        }
    }
}