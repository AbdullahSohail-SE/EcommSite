using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALA
{
    public class ProductsWithPagination
    {
        public List<ProductDTO> List { get; set; }
        public int TotalPages { get; set; }

    }
}
