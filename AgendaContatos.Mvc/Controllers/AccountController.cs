using AgendaContatos.Data.Entities;
using AgendaContatos.Data.Repositories;
using AgendaContatos.Messages;
using AgendaContatos.Mvc.Models;
using Bogus;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace AgendaContatos.Mvc.Controllers
{
    public class AccountController : Controller
    {
        //ROTA: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost] //Método para receber os dados enviados pelo formulário
        public IActionResult Login(AccountLoginModel model)
        {
            //verificar se os dados estão corretos
            if (ModelState.IsValid)
            {
                try
                {
                    //consultar o usuário no banco de dados através do email e da senha
                    var usuarioRepository = new UsuarioRepository();
                    var usuario = usuarioRepository.GetByEmailAndSenha(model.Email, model.Senha);

                    //verificar se o usuário foi encontrado
                    if (usuario != null)
                    {
                        //Gravar os dados do usuário autenticado em um arquivo de cookie do AspNet
                        //este cookie de autenticação é que fará com que o AspNet saiba diferenciar
                        //um usuário autenticado de um usuário não autenticado
                        var authenticationModel = new AuthenticationModel();
                        authenticationModel.idUsuario = usuario.IdUsuario;
                        authenticationModel.Nome = usuario.Nome;
                        authenticationModel.Email = usuario.Email;
                        authenticationModel.DataHoraAcesso = DateTime.Now;

                        //para gravar os dados do objeto no Cookie do AspNet precisamos
                        //serializar este objeto em formato JSON
                        var json = JsonConvert.SerializeObject(authenticationModel);

                        //Autenticar o usuário!!
                        GravarCookieDeAutenticacao(json);

                        //redirecionando para outra página
                        return RedirectToAction("Consulta", "Contatos");
                    }
                    else
                    {
                        TempData["Mensagem"] = "Acesso negado. Usuário inválido.";
                    }
                }
                catch (Exception e)
                {
                    TempData["Mensagem"] = $"Falha ao cadastrar: {e.Message}";
                }
            }

            return View();
        }

        //ROTA: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost] //Método para receber os dados enviados pelo formulário
        public IActionResult Register(AccountRegisterModel model)
        {
            //verificar se todos os campos trazidos pela classe model
            //passaram nas regras de validação
            if (ModelState.IsValid)
            {
                try
                {
                    var usuarioRepository = new UsuarioRepository();

                    //verificando se já existe um usuário já cadastrado com o email informado
                    if (usuarioRepository.GetByEmail(model.Email) != null)
                    {
                        TempData["Mensagem"] = $"O email {model.Email} já está cadastrado para outro usuário. Tente outro email.";
                    }
                    else
                    {
                        var usuario = new Usuario();

                        usuario.IdUsuario = Guid.NewGuid();
                        usuario.Nome = model.Nome;
                        usuario.Email = model.Email;
                        usuario.Senha = model.Senha;
                        usuario.DataCadastro = DateTime.Now;

                        usuarioRepository.Create(usuario);

                        TempData["Mensagem"] = $"Parabéns {usuario.Nome}, sua conta foi cadastrada com sucesso!";
                    }
                }
                catch (Exception e)
                {
                    TempData["Mensagem"] = $"Falha ao cadastrar: {e.Message}";
                }
            }

            return View();
        }

        //ROTA: /Account/PasswordRecover
        public IActionResult PasswordRecover()
        {
            return View();
        }

        [HttpPost] //Método para receber o SUBMIT da página de recuperação de senha
        public IActionResult PasswordRecover(AccountPasswordRecoverModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //buscar o usuário no banco de dados através do email informado
                    var usuarioRepository = new UsuarioRepository();
                    var usuario = usuarioRepository.GetByEmail(model.Email);

                    //verificar se o usuário foi encontrado
                    if (usuario != null)
                    {
                        RecuperarSenhaDoUsuario(usuario);


                        TempData["Mensagem"] = $"Olá {usuario.Nome}, você receberá um email contendo uma nova senha de acesso.";
                    }
                    else
                    {
                        TempData["Mensagem"] = "O email informado não existe no sistema, por favor verifique.";
                    }
                }
                catch (Exception e)
                {
                    TempData["Mensagem"] = $"Falha ao recuperar senha: {e.Message}";
                }
            }

            return View();
        }

        //ROTA: /Account/Logout
        public IActionResult Logout()
        {
            //Apagando o Cookie de autenticação do usuário
            RemoverCookieDeAutenticacao();

            //Redirecionar o usuário de volta para a página de login
            return RedirectToAction("Login", "Account");
        }

        //método para gravar o cookie de autenticação do usuário
        private void GravarCookieDeAutenticacao(string json)
        {
            var claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, json) }, CookieAuthenticationDefaults.AuthenticationScheme);

            //gravando o cookie de autenticação
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
        }

        //método para apagar o Cookie de autenticação do usuário
        private void RemoverCookieDeAutenticacao()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        //método para fazer a recuperação da senha do usuário
        private void RecuperarSenhaDoUsuario(Usuario usuario)
        {
            //Gerando uma nova senha para o usuario
            var faker = new Faker();
            var novaSenha = faker.Internet.Password(10);

            //enviando a senha para o e-mail do usuario
            var mailTo = usuario.Email;
            var subject = "Recuperação de senha de acesso - Agenda de Contatos";
            var body = $@"
                    <div>
                        <p>Olá {usuario.Nome}, uma nova senha foi gerada com sucesso.</P>
                        <p>Utilize a senha <strong>{novaSenha}</strong> para acessar sua conta.</p>
                        <p>Depois de acessar, você poderá atualizar essa senha para outra de sua preferência.</p>
                        <p>Att,</p>
                        <p>Equipe Agenda de Contatos</p>
                    </div>
            ";

            //enviando a senha para o email do usuario
            var emailMessage = new EmailMessage();
            emailMessage.SendMail(mailTo, subject, body);

            //atualizando a senha do usuario no banco de dados
            var usuarioRepository = new UsuarioRepository();
            usuarioRepository.Update(usuario.IdUsuario, novaSenha);
        }

    }
}
