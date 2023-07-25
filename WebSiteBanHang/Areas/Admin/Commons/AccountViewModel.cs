using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSiteBanHang.Areas.Admin.Commons
{
    public class AccountViewModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public string cfpassword { get; set; }
        public string ho { get; set; }
        public string ten { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public int role { get; set; }
        public int gioitinh { get; set; }
        public Nullable<System.DateTime> ngaysinh { get; set; }

    }
}