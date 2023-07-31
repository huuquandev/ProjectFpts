using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSiteBanHang.Models;

namespace WebSiteBanHang.Controllers
{
    public class TrangChuController : Controller
    {
        dbWebSiteBanHang db = new dbWebSiteBanHang();
        // GET: TrangChu
        public ActionResult Index()
        {
            var product = db.Products.FirstOrDefault(x => x.noibat == 1);
            return View(product);
        }
        public ActionResult search(String searchString)
        {
            var danhSachTimKiem = db.Products.Where(s => s.ten.Contains(searchString)).ToList();
            return View(danhSachTimKiem);
        }
        //[HttpPost]
        //public JsonResult SuaSanPham(Product _product, HttpPostedFileBase[] ImageUploads)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var existingProduct = db.Products.FirstOrDefault(p => p.id == _product.id);

        //        if (existingProduct != null)
        //        {
        //            if (ImageUploads != null && ImageUploads.Any(x => x != null && x.ContentLength > 0))
        //            {
        //                foreach (var imageUpload in ImageUploads)
        //                {
        //                    if (imageUpload != null && imageUpload.ContentLength > 0)
        //                    {
        //                        // Lưu file hình ảnh vào thư mục trên máy chủ
        //                        var fileName = Path.GetFileName(imageUpload.FileName);
        //                        var url = "~/Assets/image/large"; // Lấy đường dẫn lưu file database 
        //                        var urlServer = Server.MapPath(url); // Lấy đường dẫn lưu file trên server
        //                        var filePath = Path.Combine(urlServer, fileName);
        //                        imageUpload.SaveAs(filePath);

        //                        // Tạo mới một ảnh sản phẩm và cập nhật thông tin
        //                        var newProductImage = new ProductImage
        //                        {
        //                            image = filePath,
        //                        };

        //                        // Cập nhật thông tin sản phẩm đã tồn tại
        //                        existingProduct.ten = _product.ten;
        //                        existingProduct.soluong = _product.soluong;
        //                        existingProduct.idloaisanpham = _product.idloaisanpham;
        //                        existingProduct.ngaynhap = _product.ngaynhap;
        //                        existingProduct.giatien = _product.giatien;
        //                        existingProduct.giamgia = _product.giamgia;
        //                        var selectedImageId = _product.ProductImages.FirstOrDefault(x => x.Id == _product.ProductImage.image_representative)?.Id;


        //                        // Cập nhật trường image_representative cho từng ảnh
        //                        foreach (var item in existingProduct.ProductImages)
        //                        {
        //                            if (item.Id == selectedImageId)
        //                            {
        //                                existingProduct.hinhanh = item.image;
        //                                item.image_representative = 1;
        //                            }
        //                            else
        //                            {
        //                                item.image_representative = 0;
        //                            }
        //                        }

        //                        db.Products.AddOrUpdate(existingProduct);
        //                        db.SaveChanges();

        //                        return Json(new { success = true });
        //                    }
        //                    else
        //                    {
        //                        // Không có hình ảnh mới được tải lên
        //                        // Cập nhật thông tin sản phẩm đã tồn tại (không bao gồm hình ảnh)
        //                        existingProduct.ten = _product.ten;
        //                        existingProduct.soluong = _product.soluong;
        //                        existingProduct.idloaisanpham = _product.idloaisanpham;
        //                        existingProduct.ngaynhap = _product.ngaynhap;
        //                        existingProduct.giatien = _product.giatien;
        //                        existingProduct.giamgia = _product.giamgia;
        //                        existingProduct.ImageProducts = _product.ImageProducts;

        //                        var selectedImageId = _product.ProductImages.FirstOrDefault(x => x.image_representative == 1)?.Id;

        //                        // Cập nhật trường image_representative cho từng ảnh
        //                        foreach (var item in existingProduct.ProductImages)
        //                        {
        //                            if (item.Id == selectedImageId)
        //                            {
        //                                existingProduct.hinhanh = item.image;
        //                                item.image_representative = 1;
        //                            }
        //                            else
        //                            {
        //                                item.image_representative = 0;
        //                            }
        //                        }

        //                        db.Products.AddOrUpdate(existingProduct);
        //                        db.SaveChanges();

        //                        return Json(new { success = true });
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            return Json(new { success = false, message = "Sản phẩm không tồn tại." });
        //        }
        //    }
        //    return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
        //}
    }
}