using AgendaContatos.Data.Entities;
using AgendaContatos.Data.Repositories;
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

        [HttpPost] //Método para receber os dados enviados pelo formulário
        public IActionResult Register(string nome, string email, string senha, string senhaConfirmacao)
        {
            try
            {
                var usuario = new Usuario();
                
                usuario.IdUsuario = Guid.NewGuid();
                usuario.Nome = nome;
                usuario.Email = email;
                usuario.Senha = senha;
                usuario.DataCadastro = DateTime.Now;

                var usuarioRepository = new UsuarioRepository();
                usuarioRepository.Create(usuario);

                TempData["Mensagem"] = $"Parabéns {usuario.Nome}, seu cadastro foi criado com sucesso!";
            }
            catch (Exception e)
            {
                TempData["Mensagem"] = $"Falha ao cadastrar: {e.Message}";
            }
            return View();
        }

        //ROTA: /Account/PasswordRecover
        public IActionResult PasswordRecover()
        {
            return View();
        }
    }
}
