using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSiteBanHang.Areas.Admin.Commons;
using WebSiteBanHang.Models;

namespace WebSiteBanHang.Areas.Admin.Controllers
{
    [AdminAuthorizationFilter]
    public class QuanLyDonHangController : Controller
    {
        // GET: Admin/QuanLyDonHang
        dbWebSiteBanHang db = new dbWebSiteBanHang();

        public Order GetOrderById(int orderId)
        {
            // Truy vấn cơ sở dữ liệu để lấy thông tin chi tiết về sản phẩm
            return db.Orders.FirstOrDefault(o => o.id == orderId);
        }
        public ActionResult DanhSachDonHang()
        {
            List<Order> danhSachDonhang = db.Orders.ToList();
            return View(danhSachDonhang);
        }
        public ActionResult ChiTietDonHang(int orderId)
        {
            var item = GetOrderById(orderId);
            if(item != null)
            {
                return View(item);
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public ActionResult UpdateTrangThai(int orderId, int trangThai)
        {
            Order order = db.Orders.FirstOrDefault(o => o.id == orderId);
            if (order != null)
            {
                db.Orders.Attach(order);
                order.trangthaidonhang = trangThai;
                db.Entry(order).Property(x => x.trangthaidonhang).IsModified = true;
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}