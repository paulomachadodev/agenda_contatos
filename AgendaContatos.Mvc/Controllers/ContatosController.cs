using AgendaContatos.Data.Entities;
using AgendaContatos.Data.Repositories;
using AgendaContatos.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AgendaContatos.Mvc.Controllers
{
    [Authorize]
    public class ContatosController : Controller
    {
        //ROTA: /Contatos/Cadastro
        public IActionResult Cadastro()
        {
            return View();
        }

        [HttpPost] //método para receber os dados enviados pelo formulário (POST)
        public IActionResult Cadastro(ContatosCadastroModel model)
        {
            //verificar se os campos da model passaram nas validações
            if (ModelState.IsValid)
            {
                try
                {
                    var authenticationModel = ObterUsuarioAutenticado();

                    var contato = new Contato();

                    contato.IdContato = Guid.NewGuid();
                    contato.Nome = model.Nome;
                    contato.Email = model.Email;
                    contato.Telefone = model.Telefone;
                    contato.DataNascimento = DateTime.Parse(model.DataNascimento);
                    contato.IdUsuario = authenticationModel.idUsuario;

                    var contatoRepository = new ContatoRepository();
                    contatoRepository.Create(contato);

                    TempData["Mensagem"] = $"Contato {contato.Nome}, cadastrado com sucesso!";
                    ModelState.Clear(); //limpar os campos do formulário
                }
                catch (Exception e)
                {
                    TempData["Mensagem"] = $"Falha ao cadastrar contato: {e.Message}.";
                }
            }

            return View();
        }

        //ROTA: /Contatos/Consulta
        public IActionResult Consulta()
        {
            return View();
        }

        //ROTA: /Contatos/Edicao
        public IActionResult Edicao()
        {
            return View();
        }

        [HttpPost] //método para receber o submit do formulário
        public IActionResult Edicao(ContatosEdicaoModel model)
        {
            if (ModelState.IsValid)
            {

            }

            return View();
        }

        public AuthenticationModel ObterUsuarioAutenticado()
        {
            var json = User.Identity.Name; //lendo o conteudo do Cookie de autenticação do AspNet
            return JsonConvert.DeserializeObject<AuthenticationModel>(json);
        }
    }
}



