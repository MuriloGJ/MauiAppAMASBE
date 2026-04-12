using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace MauiAppAMASBE.Models
{
    public class CadastroSaudeUsuario
    {
        [PrimaryKey, AutoIncrement]
        public int IdCadastro { get; set; }

        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Sexo { get; set; }
        public string Cpf { get; set; }
        public string RuaUsuario { get; set; }
        public string NumeroUsuario { get; set; }
        public string BairroUsuario { get; set; }
        public string CidadeUsuario { get; set; }
        public string EstadoUsuario { get; set; }
        public string CepUsuario { get; set; }
        public string ComplementoUsuario { get; set; }
        public string TelefoneUsuario { get; set; }
        public string Email { get; set; }
        public string ContatoEmergencia { get; set; }
        public string TipoSanguineo { get; set; }
        public double Peso { get; set; }
        public double Altura { get; set; }
        public string Senha { get; set; }
    }
}