using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiAppAMASBE.Models
{
    public class Notificacao
    {
        [PrimaryKey, AutoIncrement]
        public int IdNotificacao { get; set; }

        public int IdLembrete { get; set; }
        public int IdMensagem { get; set; }

        public string TituloNotificacao { get; set; }

        public string TipoNotificacao { get; set; }

        public DateTime DataNotificacao { get; set; }

        public TimeSpan HorarioNotificacao { get; set; }

        public string StatusNotificacao { get; set; } = "pendente";
    }
}
