using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Web.Models;
using Newtonsoft.Json;
using Newtonsoft;
using Newtonsoft.Json.Converters;
using System.Web.Security;
using Newtonsoft.Json.Linq;

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
           

            if (Session["User"] as User != null )
                return true;

            return false;
        }
        public ActionResult terminateUserSession()
        {
            //HttpContext.Response.Cookies["UserInfo"].Expires = DateTime.Now.AddDays(-1);
            Session.Abandon();
            return new EmptyResult();
            
        }
        public ActionResult setCookie()
        {
            var user = Session["User"] as User;

            var cookieUser = new User() { Email = user.Email, Name = user.Name, Password = user.Password, UserId = user.UserId };


            cookieUser.Email = EncryptionManager.Protect(user.Email);
            cookieUser.Password = EncryptionManager.Protect(user.Password);

            var value = JsonConvert.SerializeObject(cookieUser);

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
                var emailStr = (user.Email as JValue).ToString();
                var passwordStr = (user.Password as JValue).ToString();
                
                user.Email = EncryptionManager.Unprotect(emailStr);
                user.Password = EncryptionManager.Unprotect(passwordStr);
                var userObj = new User() { Email = user.Email, Password = user.Password, Name = user.Name, UserId = user.UserId };
                return userObj;
            }
            return null;
        }
    }
}