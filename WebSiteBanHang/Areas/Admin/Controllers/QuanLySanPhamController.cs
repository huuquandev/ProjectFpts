using System;
using System.Collections.Generic;
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
    public class QuanLySanPhamController : Controller
    {
        // GET: Admin/QuanLySanPham 

        dbWebSiteBanHang db = new dbWebSiteBanHang();

        public Product GetProductById(int productId)
        {
            // Truy vấn cơ sở dữ liệu để lấy thông tin chi tiết về sản phẩm
            return db.Products.FirstOrDefault(p => p.id == productId);
        }

        public ActionResult DanhSachSanPham() 
        {
            List<Product> danhSachSanPham = db.Products.ToList();
            return View(danhSachSanPham);
        }
        public ActionResult ChiTietSanPham(int productId)
        {
            var item = GetProductById(productId);
            return View(item);
        }
        public ActionResult ThemSanPham()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ThemSanPham(Product _product, HttpPostedFileBase ImageUpload)
        {
            if (ModelState.IsValid)
            {
                var result = GetProductById(_product.id);
                if (result == null)
                {
                    if (ImageUpload != null && ImageUpload.ContentLength > 0)
                    {
                        // Lưu file hình ảnh vào thư mục trên máy chủ
                        var fileName = Path.GetFileName(ImageUpload.FileName);
                        var url = "~/Assets/image/large"; // Lấy đường dẫn lưu file database 
                        var urlServer = Server.MapPath(url); // Lấy đường dẫn lưu file trên server
                        var filePath = Path.Combine(urlServer, fileName);
                        ImageUpload.SaveAs(filePath);

                        //Tạo mã sản phẩm ngẫu nhiên
                        Random random = new Random();
                        string newProductCode;
                        do
                        {
                            int randomNumber = random.Next(1000, 9999);
                            newProductCode = "SP" + randomNumber.ToString();
                        } while (db.Orders.Any(o => o.madh == newProductCode));

                        var newProduct = new Product
                        {
                            masanpham = newProductCode,
                            ten = _product.ten,
                            hinhanh = filePath,
                            soluong = _product.soluong,
                            idloaisanpham = _product.idloaisanpham,
                            ngaynhap = _product.ngaynhap,
                            giatien = _product.giatien,
                            giacu = _product.giacu
                                                    
                        };
                        if (_product.soluong != 0)
                        {
                            newProduct.trangthai = 1;
                        }
                        else
                        {
                            newProduct.trangthai = 0;
                        }
                        db.Products.Add(newProduct);
                        db.SaveChanges();

                        return RedirectToAction("DanhSachSanPham", "QuanLySanPham");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Vui lòng chọn hình ảnh sản phẩm.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Sản phẩm đã tồn tại.");
                }
            }

            return View("ThemSanPham");
        }
        [HttpPost]
        public JsonResult SuaSanPham(Product _product, HttpPostedFileBase[] ImageUploads, ProductImage[] imageIds)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = db.Products.FirstOrDefault(p => p.id == _product.id);

                if (existingProduct != null)
                {
                    if (ImageUploads != null && ImageUploads.Any(x => x != null && x.ContentLength > 0))
                    {
                        var newProductImage = new ProductImage();
                        foreach (var imageUpload in ImageUploads)
                        {
                            if (imageUpload != null && imageUpload.ContentLength > 0)
                            {
                                // Lưu file hình ảnh vào thư mục trên máy chủ
                                var fileName = Path.GetFileName(imageUpload.FileName);
                                var url = "~/Assets/image/large"; // Lấy đường dẫn lưu file database 
                                var urlServer = Server.MapPath(url); // Lấy đường dẫn lưu file trên server
                                var filePath = Path.Combine(urlServer, fileName);
                                imageUpload.SaveAs(filePath);
                                // Tạo mới một ảnh sản phẩm và cập nhật thông tin
                                newProductImage = new ProductImage
                                {
                                    Product = db.Products.FirstOrDefault(p => p.id == _product.id),
                                    product_id = _product.id,
                                    image = filePath,
                                };
                                db.ProductImages.Add(newProductImage);
                                db.SaveChanges();
                            }
                        }
                    }

                    if(imageIds != null)
                    {
                        foreach (var item in imageIds)
                        {
                            var image = db.ProductImages.FirstOrDefault(i => i.Id == item.Id);
                            if (image != null)
                            {
                                //Xóa tệp hình ảnh liên quan đến sản phẩm
                                string filePath = Server.MapPath("~/Assets/image/large/" + Path.GetFileName(item.image));

                                System.IO.File.Delete(filePath);

                                db.ProductImages.Remove(image);
                                db.SaveChanges();

                            }
                        }
                    }
                    // Cập nhật thông tin sản phẩm đã tồn tại
                    existingProduct.ten = _product.ten;
                    existingProduct.soluong = _product.soluong;
                    existingProduct.idloaisanpham = _product.idloaisanpham;
                    existingProduct.ngaynhap = _product.ngaynhap;
                    existingProduct.giatien = _product.giatien;
                    existingProduct.giacu = _product.giacu;
                    var selectedImageId = _product.ProductImages.FirstOrDefault(x => x.image_representative == 1);


                    // Cập nhật trường image_representative cho từng ảnh
                    foreach (var item in existingProduct.ProductImages)
                    {
                        if (item.Id == selectedImageId.Id)
                        {
                            existingProduct.hinhanh = item.image;
                            item.image_representative = 1;
                        }
                        else
                        {
                            item.image_representative = 0;
                        }
                    }
                    db.Products.AddOrUpdate(existingProduct);
                    db.SaveChanges();

                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Sản phẩm không tồn tại." });
                }
            }
            return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
        }

        [HttpPost]
        public JsonResult XoaAnh(int imageId)
        {
            

            return Json(new { success = false });
        }
        [HttpPost]
        public JsonResult XoaSanPham(int productId)
        {
            var existingProduct = db.Products.FirstOrDefault(p => p.id == productId);

            if (existingProduct != null)
            {
                //Xóa tệp hình ảnh liên quan đến sản phẩm
                string filePath = Server.MapPath("~/Assets/image/large/" + Path.GetFileName(existingProduct.hinhanh));

                System.IO.File.Delete(filePath);

                db.Products.Remove(existingProduct);
                db.SaveChanges();

                return Json(new { success = true });
            }

            return Json(new { success = false });
        }
        [HttpPost]
        public JsonResult setOutstanding(int productId)
        {
            var existingProduct = db.Products.FirstOrDefault(p => p.id == productId);

            if (existingProduct != null)
            {
                var product = db.Products.ToList();
                foreach (var productItem in product)
                {
                    productItem.noibat = 0;
                }
                existingProduct.noibat = 1;
                db.Products.AddOrUpdate(existingProduct);
                db.SaveChanges();

                return Json(new { success = true });
            }

            return Json(new { success = false });
        }
    }
}