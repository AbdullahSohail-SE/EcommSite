using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Web.Models;
using Web.Models.ViewModels;
using DALA;


namespace Web.Controllers
{
    [OutputCache(Duration = 0, VaryByParam = "*")]
    public class ProductController : BaseController
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
            if (!checkUserAuthenticated())
                return RedirectToAction("Login", "User");

            return View();
        }

        public ActionResult AddNewProduct(Product product)
        {
            var ProductDTO = ProductDTOMapper(product);
            if(!ModelState.IsValid)
            {

                return View();
            }
            var manager = new DBManager("ValuesTest");
            var product_id=manager.AddNewProduct(ProductDTO);

            return RedirectToAction("AddProduct");
        }
        
        public ActionResult DisplayProducts()
        {
            if (!checkUserAuthenticated())
                return RedirectToAction("Login", "User");

            return View(new BaseModel());
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
            if (!checkUserAuthenticated())
                return RedirectToAction("Login", "User");

            return View(new BaseModel());
        }

        public ActionResult SearchByKeyword(string keywords)
        {
            var manager = new DBManager("ValuesTest");
            var productsList=manager.SearchByKeywords(keywords);

            var json = JsonConvert.SerializeObject(productsList);
            return Content(json);
        }

        [Route("Product/Detail/{productId}")]
        public ActionResult ProductDetail(int productId)
        {
            if (!checkUserAuthenticated())
                return RedirectToAction("Login", "User");

            var manager = new DBManager("ValuesTest");
            var product=manager.GetProduct(productId);

            var productViewModel = new ProductDetailViewModel()
            {
                category = product.category,
                description = product.description,
                name = product.name,
                price = product.price,
                product_id = product.product_id
            };
            return View(productViewModel);
        }
        
    }
}