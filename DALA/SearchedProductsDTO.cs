using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALA
{
    public class SearchedProductsDTO
    {
        public List<ProductDTO> List { get; set; }
        public int TotalResults { get; set; }
    }
}
