using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiAppAMASBE.Models
{
    public class PerguntaFrequente
    {
        [PrimaryKey, AutoIncrement]
        public int IdPergunta { get; set; }

        public string CategoriaPergunta { get; set; }

        public string TextoPergunta { get; set; }
        public string TextoResposta { get; set; }
    }
}
