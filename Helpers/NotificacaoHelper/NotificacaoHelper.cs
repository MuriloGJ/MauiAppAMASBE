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
    }
    public static async Task CriarNotificacao(int idLembrete, int idMensagem)
        {
            var notificacao = new Notificacao
            {
                IdLembrete = idLembrete,
                IdMensagem = idMensagem,
                TituloNotificacao = "Novo alerta",//tem que usar as estradas dos usuarios
                TipoNotificacao = "lembrete",//tem que usar as estradas dos usuarios
                DataNotificacao = DateTime.Now,
                HorarioNotificacao = DateTime.Now.TimeOfDay,
                StatusNotificacao = "pendente"
            };

            await App.Db.InsertNotificacao(notificacao);
        }
    }