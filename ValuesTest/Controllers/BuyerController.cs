using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;
using Web.Models.ViewModels;
using DALA;


namespace Web.Controllers
{
    public class BuyerController : BaseController
    {
        // GET: Buyer
        public ActionResult Products()
        {
            if (!checkUserAuthenticated())
                return RedirectToAction("Login", "User");

            var manager = new DBManager("ValuesTests");
            var productWithPagination= manager.GetProducts();
            var categories = manager.GetCategories();

            return View(new BuyerProductsVM() { ProductsList =productWithPagination.List, Categories = categories,TotalPages=productWithPagination.TotalPages });
        }
      
        public JsonResult GetProductsByCategories(List<string> Categories)
        {
            
            var manager = new DBManager("ValuesTest");
            ProductsWithPagination productsList=new ProductsWithPagination();
            if (Categories == null)
            {
              productsList = manager.GetProducts();
            }
            else
            productsList.List = manager.GetProductsByCategories(Categories);
            var json = new JsonResult();
            json.Data = productsList.List;
            return json;
        }
        
        public JsonResult GetFilteredProducts(List<string> Categories,string keywords,int? min,int? max,string orderBy,bool? sortAscending,int page=1)
        {
            var manager = new DBManager("ValuesTest");
            var productsList= manager.GetFilteredProducts(Categories,  keywords,  min,  max, orderBy,  sortAscending ,page);
            var json = new JsonResult();
            json.Data = productsList.List;

            return json;
            
        }
    }
}