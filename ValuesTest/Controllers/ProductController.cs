using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Web.Models;
using DAL.Models;

namespace Web.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ProductDTO ProductDTOMapper(Product product)
        {
            var ProductDTO = new ProductDTO()
            {
                category = product.category,
                description = product.description,
                name = product.name,
                price = product.price,
                product_id = product.product_id
            };
            return ProductDTO;
        }
        [Route("Product/Add")]
        public ActionResult AddProduct()
        {
           
            return View();

        }

        public ActionResult AddToCart(Product product)
        {
            var ProductDTO = ProductDTOMapper(product);
            if(!ModelState.IsValid)
            {

                return View();
            }
            var manager = new DBManager("ValuesTest");
            var product_id=manager.AddToCart(ProductDTO);

            return RedirectToAction("AddProduct");
        }
        
        public ActionResult DisplayProducts()
        {
            return View();
        }
        public ActionResult GetProducts()
        {
            var manager = new DBManager("ValuesTest");
            var productsList = manager.GetProducts();

            var json = JsonConvert.SerializeObject(productsList);
            return Content(json);
        }
        [HttpDelete]
        public void DeleteProduct(int id)
        {
            var manager = new DBManager("ValuesTest");
            manager.DeleteProduct(id);

        }
        [Route("Product/Search")]
        public ActionResult SearchProduct()
        {
            return View();
        }

        public ActionResult SearchByKeyword(string keywords)
        {
            var manager = new DBManager("ValuesTest");
            var productsList=manager.SearchByKeywords(keywords);

            var json = JsonConvert.SerializeObject(productsList);
            return Content(json);
        }
    }
}