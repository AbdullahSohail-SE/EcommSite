using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALA
{
    public class OrderProductDTO
    {
        public int product_id { get; set; }

        public string name { get; set; }

        public double price { get; set; }

        public string category { get; set; }

        public string description { get; set; }
        public int quantity { get; set; }

    }
}
