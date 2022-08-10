﻿namespace AgendaContatos.Mvc.Models
{
    public class AuthenticationModel
    {
        public Guid idUsuario { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public DateTime DataHoraAcesso { get; set; }
    }
}
