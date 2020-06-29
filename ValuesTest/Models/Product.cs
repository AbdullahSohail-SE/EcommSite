using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class Product : BaseModel
    {
        public int product_id { get; set; }
        [Required]
        [Display(Name = "Product Name")]
        public string name { get; set; }
        [Required]
        [Display(Name = "Price")]
        public double price { get; set; }
        [Display(Name = "Category")]
        [Required]
        public string category { get; set; }
        [Display(Name = "Description")]
        [Required]
        public string description { get; set; }

    }

}