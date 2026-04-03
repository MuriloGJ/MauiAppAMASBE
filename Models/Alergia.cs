using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiAppAMASBE.Models
{
    public class Alergia
    {
        [PrimaryKey, AutoIncrement]
        public int IdAlergia { get; set; }

        [Indexed]
        public int IdCadastro { get; set; }

        public string NomeAlergia { get; set; }
        public string DescricaoAlergia { get; set; }
    }
}
