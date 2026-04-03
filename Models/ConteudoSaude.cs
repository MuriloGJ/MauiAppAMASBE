using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiAppAMASBE.Models
{
    public class ConteudoSaude
    {
        [PrimaryKey, AutoIncrement]
        public int IdConteudo { get; set; }

        public string TituloConteudo { get; set; }
        public string Categoria { get; set; }

        public bool Favorito { get; set; }
        public bool OfflineDisponivel { get; set; }
    }
}
