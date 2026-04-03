using SQLite;
using System;
using System.Collections.Generic;
using System.Text;


namespace MauiAppAMASBE.Models
{
    public class Medicamento
    {
        [PrimaryKey, AutoIncrement]
        public int IdMedicamento { get; set; }

        [Indexed]
        public int IdCadastro { get; set; }

        public string NomeMedicamento { get; set; }
        public string Dosagem { get; set; }
        public string Frequencia { get; set; }

        public TimeSpan? Horario { get; set; }
    }
}
