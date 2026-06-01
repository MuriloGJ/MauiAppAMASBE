using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiAppAMASBE.Models
{
    public class Lembrete
    {
        [PrimaryKey, AutoIncrement]
        public int IdLembrete { get; set; }

        [Indexed]
        public int IdCadastro { get; set; }

        public bool Notificado { get; set; }
        public bool Concluido { get; set; }

        public string TituloLembrete { get; set; }

        public string TipoLembrete { get; set; }

        public DateTime DataLembrete { get; set; }

        public TimeSpan HorarioLembrete { get; set; }

        public string FrequenciaLembrete { get; set; }
    }
}
