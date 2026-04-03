using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiAppAMASBE.Models
{
    public class ServicoSus
    {
        [PrimaryKey, AutoIncrement]
        public int IdServico { get; set; }

        public string NomeServico { get; set; }
        public string TipoServico { get; set; }

        public string RuaServico { get; set; }
        public string NumeroServico { get; set; }
        public string BairroServico { get; set; }
        public string CidadeServico { get; set; }
        public string EstadoServico { get; set; }
        public string CepServico { get; set; }
        public string ComplementoServico { get; set; }

        public string Disponibilidade { get; set; }
    }
}
