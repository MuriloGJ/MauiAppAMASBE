using MauiAppAMASBE.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiAppAMASBE.Helpers.HelperNotificacao
{
    public class NotificacaoHelper
    {
        public static async Task MarcarComoLida(Notificacao n)
        {
            n.StatusNotificacao = "lida";
            await App.Db.UpdateNotificacao(n);
        }

        public static async Task<List<Notificacao>> GetNaoLidas()
        {
            var lista = await App.Db.GetNotificacoes();

            return lista.Where(n => n.StatusNotificacao == "pendente").ToList();
        }

        public static async Task CriarNotificacao(Lembrete lembrete)
        {
            var notificacao = new Notificacao
            {
                IdLembrete = lembrete.IdLembrete,
                IdMensagem = 0,

                TituloNotificacao = lembrete.TituloLembrete,

                TipoNotificacao = lembrete.TipoLembrete,

                DataNotificacao = lembrete.DataLembrete,

                HorarioNotificacao = lembrete.HorarioLembrete,

                StatusNotificacao = "pendente"
            };

            await App.Db.InsertNotificacao(notificacao);
        }
    }
}