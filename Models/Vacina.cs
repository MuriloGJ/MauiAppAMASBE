using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiAppAMASBE.Models
{
    public class Vacina
    {
        [PrimaryKey, AutoIncrement]
        public int IdVacina { get; set; }

        [Indexed]
        public int IdCadastro { get; set; }

        public string NomeVacina { get; set; }

        public DateTime? DataVacina { get; set; }

        public string Dose { get; set; }
    }
}
