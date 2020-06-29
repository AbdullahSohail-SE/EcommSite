using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.ViewModels
{
    public class ProductDetailViewModel
    {
        public int product_id { get; set; }
       
        public string name { get; set; }
       
        public double price { get; set; }
        
        public string category { get; set; }
        
        public string description { get; set; }
    }
}