using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models.ViewModels;
using DALA;
using Newtonsoft.Json;
using Web.Models;


namespace Web.Controllers
{
    public class ShopController :BaseController
    {
        // GET: Shop
        public ActionResult Checkout()
        {
            if (!checkUserAuthenticated())
                return RedirectToAction("Login", "User");
            return View(new BaseModel());
        }
        
        public ActionResult GetCartProducts()
        {
            var cookie = Request.Cookies["CartProducts"];
            if (cookie != null)
            {
                var productIDs = cookie.Value.Split('-').ToList().Select(v => Convert.ToInt32(v)).ToList();
                var manager = new DBManager("ValuesTest");

                var products = manager.GetProductsByIds(productIDs);

                var json = JsonConvert.SerializeObject(products);
                if (String.IsNullOrEmpty(json))
                    return Content("[]");
                return Content(json);
            }
            return Content("[]");
        }

        public ActionResult Order(List<OrderProductDTO> productsList,int? Total,int? Id)
        {
            var user = Session["User"] as User;
            var UserId = user.UserId;

            


            var manager = new DBManager("ValuesTest");
            var DTOList = new List<ProductPurchaseDTO>();
            foreach (var item in productsList)
            {
                DTOList.Add(new ProductPurchaseDTO() { quantity = item.quantity, product_id = item.product_id });
            }


            var success = manager.PurchaseProducts(UserId, DTOList, Total ?? 0);
            if (success != -1)
            {
                var orderDetails = manager.GetOrderById(success);
                
                MailManager.InformUser(user, orderDetails);


            }
            return Content(success.ToString());
        }
        [HttpGet]
        public ActionResult GetOrdersList()
        {
            
            var user = Session["User"] as User;
            var UserId = user.UserId;

            var manager = new DBManager("ValuesTest");
            var list=manager.GetOrdersList(UserId);

            var json=JsonConvert.SerializeObject(list);
            return Content(json);
        }

        public ActionResult Orders()
        {
            if (!checkUserAuthenticated())
                return RedirectToAction("Login", "User");
            return View(new BaseModel());
        }
        public ActionResult ProceedToPay(List<ProductPurchase> list,int? Total)
        {
            var user = Session["User"] as User;
            var UserId = user.UserId;

            var manager = new DBManager("ValuesTest");

            var productDetailsList = list.Select((val) =>
             {
                 
                 var product = manager.GetProduct(val.product_id);
                 return new OrderProductDTO()
                 {
                     category = product.category,
                     description = product.description,
                     name = product.name,
                     price = product.price,
                     product_id = product.product_id,
                     quantity = val.quantity
                 };
             }).ToList();

            var addressList = manager.GetUserAddresses(UserId);

            return PartialView("ProceedToPayPartial", new ProceedToPayVM() { productsList =productDetailsList , Total = Total ,addressList=addressList});
        }
        
    }
}