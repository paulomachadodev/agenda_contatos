using Microsoft.AspNetCore.Mvc;

namespace AgendaContatos.Mvc.Controllers
{
    public class AccountController : Controller
    {
        //ROTA: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        //ROTA: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        //ROTA: /Account/PasswordRecover
        public IActionResult PasswordRecover()
        {
            return View();
        }
    }
}
