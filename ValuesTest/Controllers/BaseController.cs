using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Web.Models;
using Newtonsoft.Json;

namespace Web.Controllers
{
    public class BaseController : Controller
    {
        public ActionResult setSessionUser(User user)
        {
            Session["User"] = user;
            return new EmptyResult();
        }
        [ChildActionOnly]
        public ActionResult getSessionUserName()
        {
            var user=Session["User"] as User;
            return Content(user.Name);
        }
        public bool checkUserAuthenticated()
        {
            var user=getValidCookieUser();

            if (user != null)
                Session["User"] = user;

            if (Session["User"] as User != null )
                return true;

            return false;
        }
        public ActionResult terminateUserSession()
        {
            HttpContext.Response.Cookies["UserInfo"].Expires = DateTime.Now.AddDays(-1);
            Session.Abandon();
            return new EmptyResult();
            
        }
        public ActionResult setCookie()
        {
            var value = JsonConvert.SerializeObject(Session["User"] as User);
            var cookie = new HttpCookie("UserInfo", value);
            cookie.Expires = DateTime.Now.AddHours(.5);
            HttpContext.Response.Cookies.Add(cookie);
            return new EmptyResult();
        }
        public User getValidCookieUser()
        {
            
            var cookie = HttpContext.Request.Cookies["UserInfo"];
            if ( cookie != null)
            {
                dynamic user = JsonConvert.DeserializeObject(cookie.Value);
                var userObj = new User() { Email = user.Email, Password = user.Password, Name = user.Name, UserId = user.UserId };
                return userObj;
            }
            return null;
        }
    }
}