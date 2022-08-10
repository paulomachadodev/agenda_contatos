using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgendaContatos.Mvc.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        //ROTA: /Usuarios/Dados
        public IActionResult Dados()
        {
            return View();
        }
    }
}
