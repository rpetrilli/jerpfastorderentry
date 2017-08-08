using Npgsql;
using portaleAgenti.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace portaleAgenti.Controllers
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

            NpgsqlConnection con = null;
            con = Helpers.DbUtils.GetDefaultConnection();
            con.Open();

            UtenteModel utente = new UtenteModel();

            if (utente.login(con, model.nomeUtente, model.password))
            {
                var identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, utente.ragione_sociale ),
                    new Claim(ClaimTypes.NameIdentifier, model.nomeUtente),
                }, "ApplicationCookie");

                var ctx = Request.GetOwinContext();
                var authManager = ctx.Authentication;
                authManager.SignIn(identity);

                return Redirect(GetRedirectUrl(model.ReturnUrl));
            }

            con.Close();

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