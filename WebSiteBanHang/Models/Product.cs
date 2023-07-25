//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebSiteBanHang.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.CartItems = new HashSet<CartItem>();
            this.OderItems = new HashSet<OderItem>();
            this.ProductImages = new HashSet<ProductImage>();
        }
    
        public int id { get; set; }
        public string ten { get; set; }
        public decimal giatien { get; set; }
        public string hinhanh { get; set; }
        public Nullable<int> giamgia { get; set; }
        public Nullable<decimal> giacu { get; set; }
        public Nullable<System.DateTime> ngaynhap { get; set; }
        public string masanpham { get; set; }
        public Nullable<int> soluong { get; set; }
        public Nullable<int> idloaisanpham { get; set; }
        public Nullable<int> trangthai { get; set; }
        public Nullable<int> Id_representativelmage { get; set; }
        public string mota { get; set; }
        public Nullable<int> id_info { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CartItem> CartItems { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OderItem> OderItems { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductImage> ProductImages { get; set; }
        public virtual ProductDesc ProductDesc { get; set; }
        public virtual ProductImage ProductImage { get; set; }
        public virtual Product Product1 { get; set; }
        public virtual Product Product2 { get; set; }
        public virtual ProductType ProductType { get; set; }
    }
}
