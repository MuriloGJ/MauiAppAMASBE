using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiAppAMASBE.Models
{
    public class MensagemMotivacional
    {
        [PrimaryKey, AutoIncrement]
        public int IdMensagem { get; set; }

        public string Mensagem { get; set; }

        public DateTime DataMensagem { get; set; }

        public TimeSpan HoraMensagem { get; set; }
    }
}
