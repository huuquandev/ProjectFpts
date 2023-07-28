using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
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
        public ActionResult mat_ong_nhap_khau(int? page, int? pageSize, int loaiSanPhamId)
        {
            if (page == null)
            {
                page = 1;
            }
            if (pageSize == null)
            {
                pageSize = 12;
            }
            var danhSachSanPham = db.Products.Where(p => p.idloaisanpham == loaiSanPhamId).ToList();
            return View(danhSachSanPham.ToPagedList((int)page, (int)pageSize));
        }
        public ActionResult mat_ong_rung(int? page, int? pageSize, int loaiSanPhamId)
        {
            if (page == null)
            {
                page = 1;
            }
            if (pageSize == null)
            {
                pageSize = 12;
            }
            var danhSachSanPham = db.Products.Where(p => p.idloaisanpham == loaiSanPhamId).ToList();
            return View(danhSachSanPham.ToPagedList((int)page, (int)pageSize));
        }
        public ActionResult mat_ong_rung_nhiet_doi(int? page, int? pageSize, int loaiSanPhamId)
        {
            if (page == null)
            {
                page = 1;
            }
            if (pageSize == null)
            {
                pageSize = 12;
            }
            var danhSachSanPham = db.Products.Where(p => p.idloaisanpham == loaiSanPhamId).ToList();
            return View(danhSachSanPham.ToPagedList((int)page, (int)pageSize));
        }
        public ActionResult mat_ong_rung_hoa(int? page, int? pageSize, int loaiSanPhamId)
        {
            if (page == null)
            {
                page = 1;
            }
            if (pageSize == null)
            {
                pageSize = 12;
            }
            var danhSachSanPham = db.Products.Where(p => p.idloaisanpham == loaiSanPhamId).ToList();
            return View(danhSachSanPham.ToPagedList((int)page, (int)pageSize));
        }
        public ActionResult mat_ong_rung_ngap_man(int? page, int? pageSize, int loaiSanPhamId)
        {
            if (page == null)
            {
                page = 1;
            }
            if (pageSize == null)
            {
                pageSize = 12;
            }
            var danhSachSanPham = db.Products.Where(p => p.idloaisanpham == loaiSanPhamId).ToList();
            return View(danhSachSanPham.ToPagedList((int)page, (int)pageSize));
        }

        public ActionResult mat_ong_hoa(int? page, int? pageSize, int loaiSanPhamId)
        {
            if (page == null)
            {
                page = 1;
            }
            if (pageSize == null)
            {
                pageSize = 12;
            }
            var danhSachSanPham = db.Products.Where(p => p.idloaisanpham == loaiSanPhamId).ToList();
            return View(danhSachSanPham.ToPagedList((int)page, (int)pageSize));
        }

        public ActionResult mat_ong_thien_nhien(int? page, int? pageSize, int loaiSanPhamId)
        {
            if (page == null)
            {
                page = 1;
            }
            if (pageSize == null)
            {
                pageSize = 12;
            }
            var danhSachSanPham = db.Products.Where(p => p.idloaisanpham == loaiSanPhamId).ToList();
            return View(danhSachSanPham.ToPagedList((int)page, (int)pageSize));
        }

        public ActionResult mat_ong_nguyen_chat(int? page, int? pageSize, int loaiSanPhamId)
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
        public ActionResult mat_ong_hoa_sam_ngoc_linh(int? page, int? pageSize, int loaiSanPhamId)
        {
            if (page == null)
            {
                page = 1;
            }
            if (pageSize == null)
            {
                pageSize = 12;
            }
            var danhSachSanPham = db.Products.Where(p => p.idloaisanpham == loaiSanPhamId).ToList();
            return View(danhSachSanPham.ToPagedList((int)page, (int)pageSize));
        }
        public ActionResult chiTietSanPham(int productId)
        {
            var objProduct = db.Products.FirstOrDefault(p => p.id == productId);

            return View(objProduct);
        }
        public ActionResult SortProducts(string sortBy, int? page, int? pageSize)
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

            switch (sortBy)
            {
                case "alpha-asc":
                    danhSachSanPham = danhSachSanPham.OrderBy(p => p.ten).ToList();
                    break;
                case "alpha-desc":
                    danhSachSanPham = danhSachSanPham.OrderByDescending(p => p.ten).ToList();
                    break;
                case "price-asc":
                    danhSachSanPham = danhSachSanPham.OrderBy(p => p.giatien).ToList();
                    break;
                case "price-desc":
                    danhSachSanPham = danhSachSanPham.OrderByDescending(p => p.giatien).ToList();
                    break;
                case "created-asc":
                    danhSachSanPham = danhSachSanPham.OrderBy(p => p.ngaynhap).ToList();
                    break;
                case "created-desc":
                    danhSachSanPham = danhSachSanPham.OrderByDescending(p => p.ngaynhap).ToList();
                    break;
                default:
                    break;
            }

            return PartialView("PartialProductList", danhSachSanPham.ToPagedList((int)page, (int)pageSize));
        }

    }
}