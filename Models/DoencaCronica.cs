using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiAppAMASBE.Models
{
    public class DoencaCronica
    {
        [PrimaryKey, AutoIncrement]
        public int IdDoenca { get; set; }

        [Indexed]
        public int IdCadastro { get; set; }

        public string NomeDoenca { get; set; }
        public string DescricaoDoenca { get; set; }
    }
}
