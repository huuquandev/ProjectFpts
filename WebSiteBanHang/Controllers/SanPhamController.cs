using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using WebSiteBanHang.Commons;
using WebSiteBanHang.Models;

namespace WebSiteBanHang.Controllers
{
    public class SanPhamController : Controller
    {
        // GET: SanPham
        dbWebSiteBanHang db = new dbWebSiteBanHang();
        public ActionResult all(int? page, int?  pageSize)
        {
           if (page == null)
            {
                page = 1;
            }
           if (pageSize == null)
            {   
                pageSize = 12;
            }
            var danhSachSanPham = db.Products.ToList();
            return View(danhSachSanPham.ToPagedList((int)page, (int)pageSize));
        }
        public ActionResult mat_ong_nhap_khau()
        {
            return View();
        }
        public ActionResult mat_ong_rung()
        {
            return View();
        }
        public ActionResult mat_ong_rung_nhiet_doi()
        {
            return View();
        }
        public ActionResult mat_ong_rung_hoa()
        {
            return View();
        }
        public ActionResult mat_ong_rung_ngap_man()
        {
            return View();
        }

        public ActionResult mat_ong_hoa()
        {
            return View();
        }

        public ActionResult mat_ong_thien_nhien()
        {
            return View();
        }

        public ActionResult mat_ong_nguyen_chat()
        {
            return View();
        }
        public ActionResult mat_ong_hoa_sam_ngoc_linh()
        {
            return View();
        }
        public ActionResult chiTietSanPham(int productId)
        {
            var objProduct = db.Products.FirstOrDefault(p => p.id == productId);

            return View(objProduct);
        }
    }
}