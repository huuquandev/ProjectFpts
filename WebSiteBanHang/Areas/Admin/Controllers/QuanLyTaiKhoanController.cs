using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSiteBanHang.Areas.Admin.Commons;
using WebSiteBanHang.Commons;
using WebSiteBanHang.Models;

namespace WebSiteBanHang.Areas.Admin.Controllers
{
    [AdminAuthorizationFilter]
    public class QuanLyTaiKhoanController : Controller
    {
        // GET: Admin/QuanLyTaiKhoan
        dbWebSiteBanHang db = new dbWebSiteBanHang();

        public User getItem(String email)
        {
            return db.Users.FirstOrDefault(x => x.email == email);
        }
        public ActionResult DanhSachTaiKhoan()
        {
            List<User> DanhSachTaiKhoan = db.Users.ToList();
            return View(DanhSachTaiKhoan);
        }
        public ActionResult ThemTaiKhoan()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ThemTaiKhoan(AccountViewModel _User)
        {
            if (ModelState.IsValid && _User != null)
            {
                var result = getItem(_User.email);
                if (result == null)
                {                  
                    if(_User.password == _User.cfpassword)
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
                            username = _User.username,
                            ho = _User.ho,
                            ten = _User.ten,
                            IdTaiKhoan = newUserCode,
                            email = _User.email,
                            password = _User.password,
                            role_id = _User.role,
                            sodt = _User.phone,
                            ngaysinh = _User.ngaysinh,
                            gioitinh = _User.gioitinh
                        };
                        db.Users.Add(newUser);
                        db.SaveChanges();
                        return Json(new { success = true });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Mật khẩu không trùng nhau.");
                        return Json(new { success = false });
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Email đã được sử dụng.");
                    return Json(new { success = false });
                }
            }
            return Json(new { success = false });
        }
        [HttpPost]
        public ActionResult SuaTaiKhoan(User _User)
        {
            if (ModelState.IsValid)
            {

                User userOther = db.Users.FirstOrDefault(o => o.Id == _User.Id);

                if (userOther != null)
                {
                    var result = getItem(_User.email);
                    if(result != null)
                    {
                        if (userOther.email == _User.email)
                        {
                            userOther.username = _User.username;
                            userOther.password = _User.password;
                            userOther.email = _User.email;
                            userOther.ho = _User.ho;
                            userOther.ten = _User.ten;
                            userOther.sodt = _User.sodt;
                            userOther.ngaysinh = _User.ngaysinh;
                            userOther.gioitinh = _User.gioitinh;
                            userOther.role_id = _User.role_id;

                            db.Users.AddOrUpdate(userOther);
                            db.SaveChanges();

                            return Json(new { success = true });
                        }
                        return Json(new { success = false, message = "Email đã được sử dụng." });

                    }
                    else
                    {
                        userOther.username = _User.username;
                        userOther.password = _User.password;
                        userOther.email = _User.email;
                        userOther.ho = _User.ho;
                        userOther.ten = _User.ten;
                        userOther.sodt = _User.sodt;
                        userOther.ngaysinh = _User.ngaysinh;
                        userOther.gioitinh = _User.gioitinh;
                        userOther.role_id = _User.role_id;

                        db.Users.AddOrUpdate(userOther);
                        db.SaveChanges();

                        return Json(new { success = true });
                    }           
                }
                else
                {
                    return Json(new { success = false });
                }

            }
            return Json(new { success = false });
        }

        public ActionResult XoaTaiKhoan(int accountId)
        {
            User user = db.Users.FirstOrDefault(u => u.Id == accountId);
            if (user != null)
            {
                db.Users.Remove(user);
                db.SaveChanges();

                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        public ActionResult DanhSachQuyen()
        {
            List<Role> danhSachQuyen = db.Roles.ToList();
            return View(danhSachQuyen);
        }
        [HttpPost]
        public ActionResult UpdateRolePermission(int roleId, string roleName ,List<PermissionData> permissions)
        {
            // Kiểm tra xem vai trò có tồn tại trong CSDL hay không
            var role = db.Roles.FirstOrDefault(r => r.id == roleId);
            if (role == null)
            {
                return Json(new { success = false, message = "Không tìm thấy vai trò." });
            }
            if(roleName != null)
            {
                role.name = roleName;
                db.Roles.AddOrUpdate(role);
            }
            foreach (var permission in permissions)
            {
                if (permission.IsChecked)
                {
                    // Kiểm tra xem quan hệ giữa vai trò và chức năng đã tồn tại hay chưa
                    var existingRolePermission = db.RolePermissions.FirstOrDefault(rp => rp.role_id == roleId && rp.permission_id == permission.PermissionId);
                    if (existingRolePermission == null)
                    {
                        // Nếu quan hệ chưa tồn tại, thêm bản ghi mới vào bảng "RolePermissions"
                        var newRolePermission = new RolePermission
                        {                          
                            role_id = roleId,
                            permission_id = permission.PermissionId
                        };
                        db.RolePermissions.Add(newRolePermission);
                    }
                }
                else
                {
                    // Xóa quan hệ giữa vai trò và chức năng trong bảng "RolePermissions"
                    var rolePermission = db.RolePermissions.FirstOrDefault(rp => rp.role_id == roleId && rp.permission_id == permission.PermissionId);
                    if (rolePermission != null)
                    {
                        db.RolePermissions.Remove(rolePermission);
                    }
                }
            }
            // Lưu thay đổi vào CSDL
            db.SaveChanges();

            return Json(new { success = true, message = "Cập nhật thành công." });
        }
        [HttpPost]
        public ActionResult UpdatePermission(int permissionId, string permissionName)
        {
            // Kiểm tra xem vai trò có tồn tại trong CSDL hay không
            var permission = db.Permissions.FirstOrDefault(r => r.id == permissionId);
            if (permission == null)
            {
                return Json(new { success = false, message = "Không tìm thấy vai trò." });
            }
            if (permissionName != null)
            {
                permission.name = permissionName;
                db.Permissions.AddOrUpdate(permission);
            }
            // Lưu thay đổi vào CSDL
            db.SaveChanges();

            return Json(new { success = true, message = "Cập nhật thành công." });
        }
        public ActionResult ThemQuyen()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddRole(Role model)
        {
            if (model != null)
            {
                var checkRole = db.Roles.FirstOrDefault(r => r.name.ToLower() == model.name.ToLower());
                if (checkRole == null)
                {
                    var newRole = new Role
                    {
                        name = model.name
                    };
                    db.Roles.Add(newRole);
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Vai trò đã tồn tại." });
                }
            }
            return Json(new { success = false });
        }
        [HttpPost]
        public ActionResult AddPermission(Permission model)
        {
            if (model != null)
            {
                var checkPermission = db.Permissions.FirstOrDefault(r => r.name.ToLower() == model.name.ToLower());
                if (checkPermission == null)
                {
                    var newPermission = new Permission
                    {
                        name = model.name
                    };
                    db.Permissions.Add(newPermission);
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                else
                {   
                    return Json(new { success = false, message = "Chức năng đã tồn tại." });
                }
            }
            return Json(new { success = false });
        }
        [HttpPost]
        public ActionResult UpdadteRole(Role model)
        {
            if (model != null)
            {
                var checkRole = db.Roles.FirstOrDefault(r => r.name.ToLower() == model.name.ToLower());
                if (checkRole == null)
                {
                    var newRole = new Role
                    {
                        name = model.name
                    };
                    db.Roles.Add(newRole);
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Vai trò đã tồn tại." });
                }
            }
            return Json(new { success = false });
        }
        [HttpPost] 
        public ActionResult DeleteRole(int roleId)
        {
            var Role = db.Roles.FirstOrDefault(r => r.id == roleId);
            if (Role != null)
            {
                db.Roles.Remove(Role);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        [HttpPost]
        public ActionResult DeletePermission(int permissionId)
        {
            var Permission = db.Permissions.FirstOrDefault(r => r.id == permissionId);
            if (Permission != null)
            {
                db.Permissions.Remove(Permission);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}