using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models.ViewModels;
using DALA;
using Web.Models;
using Newtonsoft.Json;

namespace Web.Controllers
{
    public class UserController : BaseController
    {
        // GET: User
        public ActionResult Login(LoginViewModel loginViewModel)
        {
            
            var userCookie = getValidCookieUser();
            if (userCookie!=null)
            {
                return View(new LoginViewModel() { Email=userCookie.Email, Password=userCookie.Password });
            }
            return View(loginViewModel);
        }

        [HttpPost]
        public ActionResult AuthenticateUser(LoginViewModel loginViewModel)
        {
            var manager = new DBManager("ValuesTest");
            var status = manager.AuthenticateUser(new UserDto() { Email = loginViewModel.Email, Password = loginViewModel.Password });

            if(status==AuthenticationStatus.UserNotFound)
                ModelState.AddModelError("Email", "User not found!");   
 
            if(status==AuthenticationStatus.WrongPassword)
                ModelState.AddModelError("Password", "Wrong Password!");
             
           if(!ModelState.IsValid)
            {
                return View("Login", loginViewModel);
            }

            var userDto = manager.GetUser(loginViewModel.Email);
            setSessionUser(new Models.User()
            {
                UserId = userDto.UserId,
                Name = userDto.Name,
                Email = userDto.Email,
                Password = userDto.Password
            });
            if(loginViewModel.RememberMe)
            {
                setCookie();
            }
            if(manager.CheckIfAdmin(loginViewModel.Email))
            {
                return RedirectToAction("DisplayProducts", "Product");
            }
            return RedirectToAction("Products", "Buyer");
        }

        public ActionResult SignOut()
        {
            terminateUserSession();
            return RedirectToAction("Login", "User");
        }
        public ActionResult ShowAddresses()
        {
            if (!checkUserAuthenticated())
                return RedirectToAction("Login", "User");

            return View(new BaseModel());
        }
        public ActionResult GetUserAddresses()
        {
            
            var manager = new DBManager("ValuesTest");

            var user = Session["User"] as User;
            var userId = user.UserId;

            var list=manager.GetUserAddresses(userId);

            return Content(JsonConvert.SerializeObject(list));
            
        }
        public ActionResult DeleteUserAddress(int id)
        {
            var manager = new DBManager("ValuesTest");

            var user = Session["User"] as User;
            var userId = user.UserId;

            manager.DeleteUserAddress(userId, id);

            return new EmptyResult();
        }
        public ActionResult AddUserAddress(string address)
        {
            var manager = new DBManager("ValuesTest");

            var user = Session["User"] as User;
            var userId = user.UserId;

            var newId=manager.AddUserAddress(userId, address);

            return Content(newId.ToString());
        }
    }
}