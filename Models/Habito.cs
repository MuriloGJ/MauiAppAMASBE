using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using MauiAppAMASBE.ViewModel;

namespace MauiAppAMASBE.Models
{
    public class Habito
    {
        [PrimaryKey, AutoIncrement]
        public int IdHabito { get; set; }

        [Indexed]
        public int IdCadastro { get; set; }

        public string NomeHabito { get; set; }
        public string TipoHabito { get; set; }
        public string DescricaoHabito { get; set; }

        public double MetaValor { get; set; }
        public string MetaUnidade { get; set; }
        public string FrequenciaHabito { get; set; }

        public TimeSpan HorarioHabito { get; set; }

        public string StatusHabito { get; set; } = "pendente";

        public double Progresso { get; set; }
    }
}
