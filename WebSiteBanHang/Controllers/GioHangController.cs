using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using WebSiteBanHang.Commons;
using WebSiteBanHang.Models;
using WebSiteBanHang.Models.Commom;

namespace WebSiteBanHang.Controllers
{
    // Trạng thái thanh toán
    public enum PaymentStatus
    {
        Success = 1,
        Pending = 2,
        Failed = 3
    }
    public class GioHangController : Controller
    {
        // GET: GioHang
        dbWebSiteBanHang db = new dbWebSiteBanHang();
        public Cart GetCartByCustomerId(int customerId)
        {
            // Truy vấn cơ sở dữ liệu để lấy giỏ hàng của khách hàng
            return db.Carts.FirstOrDefault(c => c.id_user == customerId);
        }
        public List<CartItem> GetCartItemsByCartId(int cartId)
        {
            // Truy vấn cơ sở dữ liệu để lấy danh sách các mục hàng trong giỏ hàng
            return db.CartItems.Where(ci => ci.id_cart == cartId).ToList();
        }
        public Product GetProductById(int productId)
        {
            // Truy vấn cơ sở dữ liệu để lấy thông tin chi tiết về sản phẩm
            return db.Products.FirstOrDefault(p => p.id == productId);
        }
        public ActionResult cart()
        {
            // Kiểm tra xem khách hàng đã đăng nhập hay chưa
            if (Session[sessionLogin.USER_SESSION] != null)
            {
                // Lấy thông tin khách hàng từ session
                var userSession = (User)Session[sessionLogin.USER_SESSION];
                int customerId = userSession.Id;

                // Lấy thông tin giỏ hàng của khách hàng từ cơ sở dữ liệu
                Cart cart = GetCartByCustomerId(customerId);

                if (cart != null)
                {
                    // Lấy danh sách sản phẩm trong giỏ hàng của khách hàng
                    List<CartItem> cartItems = GetCartItemsByCartId(cart.id);
                    Session["CountUser"] = cartItems.Count;
                    return View(cartItems);
                }
            }
            else if (Session[sessionCart.CART_SESSION] != null)
            {
                // Lấy danh sách sản phẩm trong giỏ hàng từ Session
                List<CartItem> cartItems = (List<CartItem>)Session[sessionCart.CART_SESSION];
                Session["CountUser"] = cartItems.Count;
                return View(cartItems);
            }
            // Nếu giỏ hàng không tồn tại hoặc không có sản phẩm, trả về dữ liệu trống dưới dạng JSON
            return View(new List<CartItem>());
        }
        [HttpPost]
        public ActionResult AddToCart(int productId, int quantity)
         {
            decimal totalPrice = 0;

            // Kiểm tra xem khách hàng đã đăng nhập hay chưa
            if (Session[sessionLogin.USER_SESSION] != null)
                {
                    // Lấy thông tin khách hàng từ session
                    var userSession = (User)Session[sessionLogin.USER_SESSION];
                    int customerId = userSession.Id;
                    Cart cart = GetCartByCustomerId(customerId);
                    // Nếu khác hàng chưa có giỏ hàng, tạo mới giỏ hàng 
                    if (cart == null)
                    {
                        cart = new Cart
                        {
                            id_user = customerId
                        };                  
                        db.Carts.Add(cart);
                        db.CartItems.Add(new CartItem { Product = GetProductById(productId), id_product = productId, soluong = 1 });
                        Session["CountUser"] = 1;
                        db.SaveChanges();
                    }
                    else
                    {
                        // Kiểm tra xem sản phẩm đã tồn tại trong giỏ hàng của khác hàng chưa
                        CartItem cartItem = db.CartItems.FirstOrDefault(ci => ci.id_cart == cart.id && ci.id_product == productId);
                        if (cartItem == null)
                        {
                            // Nếu sản phẩm chưa tồn tại trong giỏ hàng, thêm sản phẩm vào giỏ hàng
                            cartItem = new CartItem
                            {
                                id_cart = cart.id,
                                id_product = productId,
                                soluong = 1 // Số lượng mặc định khi thêm vào giỏ hàng
                            };
                            db.CartItems.Add(cartItem);
                            Session["CountUser"] = Convert.ToInt32(Session["CountUser"]) + 1;
                        }
                        else
                        {
                            // Nếu đã tồn tại, tăng số lượng sản phẩm lên
                            cartItem.soluong++;
                        }
                        db.SaveChanges();
                   
                    }
                List<CartItem> cartItems = GetCartItemsByCartId(cart.id);

                totalPrice = CalculateTotalPrice(cartItems);
            }
            else
                {
                    // Kiểm tra xem Session giỏ hàng đã tồn tại chưa
                    if (Session[sessionCart.CART_SESSION] == null)
                    {
                        // Nếu chưa tồn tại, tạo một danh sách mới để lưu trữ sản phẩm
                        List<CartItem> cart = new List<CartItem>();

                        // Thêm sản phẩm vào giỏ hàng
                        cart.Add(new CartItem { Product = GetProductById(productId), id_product = productId, soluong = 1 });

                        // Lưu danh sách giỏ hàng vào Session
                        Session.Add(sessionCart.CART_SESSION, cart);
                        Session["Count"] = 1;
                    }
                    else
                    {
                        // Nếu Session giỏ hàng đã tồn tại, lấy danh sách từ Session
                        List<CartItem> cart = (List<CartItem>)Session[sessionCart.CART_SESSION];

                        // Kiểm tra xem sản phẩm đã tồn tại trong giỏ hàng chưa
                        var existingItem = cart.FirstOrDefault(item => item.id_product == productId);
                        if (existingItem == null)
                        {
                            // Nếu chưa tồn tại, thêm sản phẩm vào giỏ hàng
                            cart.Add(new CartItem { Product = GetProductById(productId), id_product = productId, soluong = 1 });
                            Session["Count"] = Convert.ToInt32(Session["Count"]) + 1;
                        }
                        else
                        {
                            // Nếu đã tồn tại, tăng số lượng sản phẩm lên
                            existingItem.soluong += quantity;
                        }
                        // Cập nhật lại danh sách giỏ hàng trong Session
                        Session[sessionCart.CART_SESSION] = cart;
                    }
                List<CartItem> cartSession = (List<CartItem>)Session[sessionCart.CART_SESSION];
                totalPrice = CalculateTotalPrice(cartSession);


            }

            return Json(new { Count = Convert.ToInt32(Session["Count"]), CountUser = Convert.ToInt32(Session["CountUser"]), totalPrice }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult RemoveFromCart(int productId)
        {
            decimal totalPrice = 0;

            if (Session[sessionLogin.USER_SESSION] != null)
            {
                var userSession = (User)Session[sessionLogin.USER_SESSION];
                int customerId = userSession.Id;

                // Lấy thông tin giỏ hàng của khách hàng từ cơ sở dữ liệu
                Cart cart = GetCartByCustomerId(customerId);
                List<CartItem> cartItems = GetCartItemsByCartId(cart.id);
                // Tìm sản phẩm cần xóa trong danh sách giỏ hàng
                var itemToRemove = cartItems.FirstOrDefault(item => item.id_product == productId);
                if (itemToRemove != null)
                {
                    // Xóa sản phẩm khỏi danh sách giỏ hàng
                    cartItems.Remove(itemToRemove);
                    db.CartItems.Remove(itemToRemove);
                    Session["CountUser"] = Convert.ToInt32(Session["CountUser"]) - 1;
                    db.SaveChanges();
                }
                totalPrice = CalculateTotalPrice(cartItems);
            }
            else
            {
                // Kiểm tra xem Session giỏ hàng đã tồn tại chưa
                if (Session[sessionCart.CART_SESSION] != null)
                {
                    // Lấy danh sách giỏ hàng từ Session
                    List<CartItem> cart = (List<CartItem>)Session[sessionCart.CART_SESSION];

                    // Tìm sản phẩm cần xóa trong danh sách giỏ hàng
                    var itemToRemove = cart.FirstOrDefault(item => item.id_product == productId);

                    if (itemToRemove != null)
                    {
                        // Xóa sản phẩm khỏi danh sách giỏ hàng
                        cart.Remove(itemToRemove);
                        Session["Count"] = Convert.ToInt32(Session["Count"]) - 1;
                        if (Convert.ToInt32(Session["Count"]) == 0)
                        {
                            // Xóa giỏ hàng khỏi Session khi số lượng sản phẩm về 0
                            Session.Remove(sessionCart.CART_SESSION);
                        }
                        else
                        {
                            // Cập nhật lại danh sách giỏ hàng trong Session
                            Session[sessionCart.CART_SESSION] = cart;
                        }
                    }
                    // Tính lại tổng tiền
                    totalPrice = CalculateTotalPrice(cart);
                }
            }
            return Json(new { success = true, Count = Convert.ToInt32(Session["Count"]), CountUser = Convert.ToInt32(Session["CountUser"]), totalPrice });
        }
        [HttpPost]
        public JsonResult UpdateQuantity(int productId, int quantity)
        {
            if (Session[sessionLogin.USER_SESSION] != null)
            {
                var userSession = (User)Session[sessionLogin.USER_SESSION];
                int customerId = userSession.Id;

                // Lấy thông tin giỏ hàng của khách hàng từ cơ sở dữ liệu
                Cart cart = GetCartByCustomerId(customerId);
                List<CartItem> cartItems = GetCartItemsByCartId(cart.id);

                // Tìm sản phẩm trong giỏ hàng
                var itemToUpdate = cartItems.FirstOrDefault(item => item.id_product == productId);

                if (itemToUpdate != null)
                {
                    // Cập nhật số lượng sản phẩm
                    itemToUpdate.soluong = quantity;
                }
                // Lưu thay đổi vào cơ sở dữ liệu
                db.CartItems.AddOrUpdate(itemToUpdate);
                db.SaveChanges();

                // Tính lại tổng tiền
                decimal totalPrice = CalculateTotalPrice(cartItems);

                return Json(new { success = true, totalPrice });

            }
            else
            {
                // Lấy giỏ hàng từ session
                var cartSession = (List<CartItem>)Session[sessionCart.CART_SESSION];

                // Tìm sản phẩm trong giỏ hàng
                var cartItem = cartSession.FirstOrDefault(item => item.id_product == productId);

                if (cartItem != null)
                {
                    // Cập nhật số lượng sản phẩm
                    cartItem.soluong = quantity;
                }

                // Lưu lại thay đổi vào session
                Session[sessionCart.CART_SESSION] = cartSession;

                // Tính lại tổng tiền
                decimal totalPrice = CalculateTotalPrice(cartSession);

                return Json(new { success = true, totalPrice });

            }

            return Json(new { success = true});
        }

        private decimal CalculateTotalPrice(List<CartItem> cart)
        {
            // Tính tổng tiền từ giỏ hàng
            decimal totalPrice = 0;

            foreach (var cartItem in cart)
            {
                totalPrice += (decimal)cartItem.soluong * cartItem.Product.giatien;
            }

            return totalPrice;
        }
        private Order GetOrderById(int orderId)
        {
            return db.Orders.FirstOrDefault(o => o.id == orderId);
        }
        public ActionResult CheckOut()
        {
            if (Session[sessionLogin.USER_SESSION] != null)
            {
                // Lấy thông tin khách hàng từ session
                var userSession = (User)Session[sessionLogin.USER_SESSION];
                int customerId = userSession.Id;

                // Lấy thông tin giỏ hàng của khách hàng từ cơ sở dữ liệu
                Cart cart = GetCartByCustomerId(customerId);

                if (cart != null)
                {
                    List<CartItem> cartItems = GetCartItemsByCartId(cart.id);
                    Address addressUser = db.Addresses.FirstOrDefault(ad => ad.userId == customerId);
                    List<Address> addresses = db.Addresses.Where(ad => ad.userId == userSession.Id).ToList();

                    var viewModel = new CheckoutViewModel
                    {
                        Addresses = addresses,
                        CartItems = cartItems
                    };

                    return View(viewModel);
                }
            }
            else if (Session[sessionCart.CART_SESSION] != null)
            {
                List<CartItem> cartItems = (List<CartItem>)Session[sessionCart.CART_SESSION];
                var viewModel = new CheckoutViewModel
                {
                    CartItems = cartItems
                };
                return View(viewModel);
            }
            return RedirectToAction("cart");
        }

        [HttpPost]
        public ActionResult CheckOut(CheckoutViewModel checkoutViewModel)
        {
            if (ModelState.IsValid)
            {
                if (Session[sessionLogin.USER_SESSION] != null)
                {
                    var userSession = (User)Session[sessionLogin.USER_SESSION];
                    int customerId = userSession.Id;
                    // Lưu thông tin địa chỉ mới khi tạo mới một địa chỉ
                    Address newAddress = new Address
                    {
                        email = userSession.email,
                        hoten = checkoutViewModel.NewAddress.hoten,
                        userId = customerId,
                        sodt = checkoutViewModel.NewAddress.sodt,
                        congty = checkoutViewModel.NewAddress.congty,
                        diachi = checkoutViewModel.NewAddress.diachi,
                        quocgia = checkoutViewModel.NewAddress.quocgia,
                        tinhthanh = checkoutViewModel.NewAddress.tinhthanh,
                        quanhuyen = checkoutViewModel.NewAddress.quanhuyen,
                        phuongxa = checkoutViewModel.NewAddress.phuongxa,
                        diachimacdinh = 0
                    };
                    db.Addresses.Add(newAddress);
                    db.SaveChanges();

                    // Lấy địa chỉ để lưu vào đơn hàng
                    var parts = new List<string>();
                    if (!string.IsNullOrEmpty(checkoutViewModel.NewAddress.diachi))
                    {
                        parts.Add(checkoutViewModel.NewAddress.diachi);
                    }
                    if (!string.IsNullOrEmpty(checkoutViewModel.NewAddress.phuongxa))
                    {
                        parts.Add(checkoutViewModel.NewAddress.phuongxa);
                    }
                    if (!string.IsNullOrEmpty(checkoutViewModel.NewAddress.quanhuyen))
                    {
                        parts.Add(checkoutViewModel.NewAddress.quanhuyen);
                    }
                    if (!string.IsNullOrEmpty(checkoutViewModel.NewAddress.tinhthanh))
                    {
                        parts.Add(checkoutViewModel.NewAddress.tinhthanh);
                    }

                    //Tạo mã đơn hàng ngẫu nhiên
                    Random random = new Random();
                    string newOrderCode;
                    do
                    {
                        int randomNumber = random.Next(1000, 9999);
                        newOrderCode = "DH" + randomNumber.ToString();
                    } while (db.Orders.Any(o => o.madh == newOrderCode));

                    // Lưu thông tin mục sản phẩm trong đơn hàng
                    Cart cart = GetCartByCustomerId(customerId);
                    List<CartItem> cartItems = GetCartItemsByCartId(cart.id);

                    // Tạo đơn hàng
                    Order newOrder = new Order
                    {
                        id_user = customerId,
                        ngay_dathang = DateTime.Now,
                        madh = newOrderCode,
                        diachi = string.Join(", ", parts),
                        phuongthucthanhtoan = checkoutViewModel.Order.phuongthucthanhtoan,
                        trangthaidonhang = 2,
                        tongdh = CalculateTotalPrice(cartItems),
                        hoten = checkoutViewModel.NewAddress.hoten,
                        email = checkoutViewModel.NewAddress.email,
                        sodt = checkoutViewModel.NewAddress.sodt,
                        trangthaivanchuyen = 2,
                        ghichu = checkoutViewModel.Order.ghichu
                    };
                    db.Orders.Add(newOrder);
                    db.SaveChanges();
                   
                    foreach (var cartItem in cartItems)
                    {
                        // Tạo mục sản phẩm trong đơn hàng
                        var orderItem = new OderItem
                        {
                            id_oder = newOrder.id,
                            id_product = cartItem.id_product,
                            soluong = cartItem.soluong,
                            tongdh = cartItem.soluong * cartItem.Product.giatien
                        };
                        db.OderItems.Add(orderItem);
                    }
                    db.SaveChanges();

                    // Xóa thông tin giỏ hàng sau khi đã thanh toán
                    ClearCart();
                    return RedirectToAction("PaymentSuccess", new { orderId = newOrder.id });
                }
                else
                {
                    List<CartItem> cartItems = (List<CartItem>)Session[sessionCart.CART_SESSION];

                    // Lấy địa chỉ để lưu vào đơn hàng
                    var parts = new List<string>();
                    if (!string.IsNullOrEmpty(checkoutViewModel.NewAddress.diachi))
                    {
                        parts.Add(checkoutViewModel.NewAddress.diachi);
                    }
                    if (!string.IsNullOrEmpty(checkoutViewModel.NewAddress.phuongxa))
                    {
                        parts.Add(checkoutViewModel.NewAddress.phuongxa);
                    }
                    if (!string.IsNullOrEmpty(checkoutViewModel.NewAddress.quanhuyen))
                    {
                        parts.Add(checkoutViewModel.NewAddress.quanhuyen);
                    }
                    if (!string.IsNullOrEmpty(checkoutViewModel.NewAddress.tinhthanh))
                    {
                        parts.Add(checkoutViewModel.NewAddress.tinhthanh);
                    }

                    //Tạo mã đơn hàng ngẫu nhiên
                    Random random = new Random();
                    string newOrderCode;
                    do
                    {
                        int randomNumber = random.Next(1000, 9999);
                        newOrderCode = "DH" + randomNumber.ToString();
                    } while (db.Orders.Any(o => o.madh == newOrderCode));


                    // Tạo đối tượng Order
                    Order newOrder = new Order
                    {
                        ngay_dathang = DateTime.Now,
                        madh = newOrderCode,
                        diachi = string.Join(", ", parts),
                        phuongthucthanhtoan = checkoutViewModel.Order.phuongthucthanhtoan,
                        trangthaidonhang = 2,
                        tongdh = CalculateTotalPrice(cartItems),
                        hoten = checkoutViewModel.NewAddress.hoten,
                        email = checkoutViewModel.NewAddress.email,
                        sodt = checkoutViewModel.NewAddress.sodt,
                        trangthaivanchuyen = 2,
                        ghichu = checkoutViewModel.Order.ghichu
                    };
                    db.Orders.Add(newOrder);
                    db.SaveChanges();

                    // Lưu thông tin mục sản phẩm trong đơn hàng
                    foreach (var cartItem in cartItems)
                    {
                        // Tạo mục sản phẩm trong đơn hàng
                        var orderItem = new OderItem
                        {
                            id_oder = newOrder.id,
                            id_product = cartItem.id_product,
                            soluong = cartItem.soluong,
                            tongdh = cartItem.soluong * cartItem.Product.giatien
                        };
                        db.OderItems.Add(orderItem);
                    }
                    db.SaveChanges();
                    // Xóa thông tin giỏ hàng sau khi đã thanh toán
                    ClearCart();
                    return RedirectToAction("PaymentSuccess", new { orderId = newOrder.id });

                }
            }
            return View();
        }


        private void ClearCart()
        {
            if (Session[sessionLogin.USER_SESSION] != null)
            {
                var userSession = (User)Session[sessionLogin.USER_SESSION];
                int customerId = userSession.Id;

                Cart cart = GetCartByCustomerId(customerId);

                if (cart != null)
                {
                    // Xóa tất cả các mặt hàng trong giỏ hàng (CartItems)
                    DeleteAllCartItems(cart.id);

                    // Xóa giỏ hàng (Cart)
                    DeleteCart(cart.id);
                }
            }
            else if (Session[sessionCart.CART_SESSION] != null)
            {
                Session.Remove(sessionCart.CART_SESSION); // Xóa session giỏ hàng
                Session.Remove("Count");
            }
        }
        private void DeleteAllCartItems(int cartId)
        {
                
            var cartItems = db.CartItems.Where(ci => ci.id_cart == cartId).ToList();
            foreach (var cartItem in cartItems)
            {
                db.CartItems.Remove(cartItem);
            }
            db.SaveChanges();
        }

        private void DeleteCart(int cartId)
        {
            var cart = db.Carts.FirstOrDefault(c => c.id == cartId);
            if (cart != null)
            {
                db.Carts.Remove(cart);
                db.SaveChanges();
            }
        }
        public ActionResult PaymentSuccess(int orderId)
        {
            //Lấy danh sách đơn hàng của khách hàng từ cơ sở dữ liệu
            Order order = db.Orders.FirstOrDefault(o => o.id == orderId);
            if(order != null)
            {
                return View(order);
            }
            return View();
        }
        [HttpPost]
        public ActionResult UpdatePaymentMethod(int orderId, int method)
        {
            Order order = db.Orders.FirstOrDefault(o => o.id == orderId);
            if(order!= null)
            {
                db.Orders.Attach(order);
                order.trangthaidonhang = method;
                db.Entry(order).Property(x => x.trangthaidonhang).IsModified = true;
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}