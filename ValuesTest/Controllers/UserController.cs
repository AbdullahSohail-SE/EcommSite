using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models.ViewModels;
using DALA;

namespace Web.Controllers
{
    public class UserController : BaseController
    {
        // GET: User
        public ActionResult Login(LoginViewModel loginViewModel)
        {
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
            return RedirectToAction("DisplayProducts", "Product");    
        }

        public ActionResult SignOut()
        {
            terminateUserSession();
            return RedirectToAction("Login", "User");
        }
    }
}