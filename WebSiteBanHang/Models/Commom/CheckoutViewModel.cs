using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSiteBanHang.Models.Commom
{
    public class CheckoutViewModel
    {
        public Order Order { get; set; }
        public List<Order> Orders { get; set; }
        public List<OderItem> OderItems { get; set; }
        public Cart Cart { get; set; }
        public List<CartItem> CartItems { get; set; }
        public Address NewAddress { get; set; }
        public List<Address> Addresses { get; set; }
    }
}