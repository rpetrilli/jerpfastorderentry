using fastOrderEntry.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace fastOrderEntry.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private NpgsqlConnection con = null;

        public ActionResult Login(string returnUrl)
        {
            var model = new LoginModel
            {
                ReturnUrl = returnUrl
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid) //Checks if input fields have the correct format
            {
                ModelState.AddModelError("LoginMessage", "Specificare nome utente e password");
                return View(model); //Returns the view with the input values so that the user doesn't have to retype again
            }



            if (model.nomeUtente == "admin" && model.password == "admin")
            {
                var identity = new ClaimsIdentity( new[] {
                    new Claim(ClaimTypes.Name, "Amministratore"),
                    new Claim(ClaimTypes.NameIdentifier, "admin"),
                }, "ApplicationCookie");

                var ctx = Request.GetOwinContext();
                var authManager = ctx.Authentication;
                authManager.SignIn(identity);

                return Redirect(GetRedirectUrl(model.ReturnUrl));
            }

            ModelState.AddModelError("LoginMessage", "Nome utente o password non validi");

            return View(model);
        }

        public ActionResult Logout()
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;

            authManager.SignOut("ApplicationCookie");
            return RedirectToAction("Login", "Auth");
        }

        private string GetRedirectUrl(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                return Url.Action("Index", "Home");
            }
            return returnUrl;
        }
    }


}