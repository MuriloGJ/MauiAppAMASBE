using MauiAppAMASBE.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiAppAMASBE.Helpers
{
    public class SQLiteDatabaseHelper
    {
        readonly SQLiteAsyncConnection _conn;
        public SQLiteDatabaseHelper(string path)
        {
            _conn = new SQLiteAsyncConnection(path);
            _conn.CreateTableAsync<CadastroSaudeUsuario>().Wait();
            _conn.CreateTableAsync<Medicamento>().Wait();
            _conn.CreateTableAsync<Alergia>().Wait();
            _conn.CreateTableAsync<DoencaCronica>().Wait();
            _conn.CreateTableAsync<Vacina>().Wait();
            _conn.CreateTableAsync<Habito>().Wait();
            _conn.CreateTableAsync<MensagemMotivacional>().Wait();
            _conn.CreateTableAsync<Lembrete>().Wait();
            _conn.CreateTableAsync<Notificacao>().Wait();
            _conn.CreateTableAsync<ConteudoSaude>().Wait();
            _conn.CreateTableAsync<PerguntaFrequente>().Wait();
            _conn.CreateTableAsync<ServicoSus>().Wait();
        }
        #region cadastroUsuario //organização dos metodos
        public Task<int> InsertUsuario(CadastroSaudeUsuario u)
        {
            return _conn.InsertAsync(u);
        }
        public Task<int> UpdateUsuario(CadastroSaudeUsuario u)
        {
            return _conn.UpdateAsync(u);
        }
        public Task<CadastroSaudeUsuario> GetUsuarioPorId(int id)
        {
            return _conn.Table<CadastroSaudeUsuario>()
                        .Where(u => u.IdCadastro == id)
                        .FirstOrDefaultAsync();
        }
        public Task<List<CadastroSaudeUsuario>> GetUsuarios()
        {
            return _conn.Table<CadastroSaudeUsuario>().ToListAsync();
        }
        public Task<CadastroSaudeUsuario> GetUsuario(string login, string senha)
        {
            return _conn.Table<CadastroSaudeUsuario>()
                .Where(u => (u.Email == login || u.Cpf == login) && u.Senha == senha)
                .FirstOrDefaultAsync();
        }
        public Task<CadastroSaudeUsuario> GetUsuarioPorCpf(string cpf)
        {
            return _conn.Table<CadastroSaudeUsuario>()
                        .Where(u => u.Cpf == cpf)
                        .FirstOrDefaultAsync();
        }
        
        public Task<int> DeleteUsuario(int id)
        {
            return _conn.Table<CadastroSaudeUsuario>()
                        .DeleteAsync(u => u.IdCadastro == id);
        }
        public async Task<bool> EmailExiste(string email)
        {
            var usuario = await _conn.Table<CadastroSaudeUsuario>()
                                     .Where(u => u.Email == email)
                                     .FirstOrDefaultAsync();

            return usuario != null;
        }
        #endregion
        #region Habito
        public Task<int> InsertHabito(Habito h)
        {
            return _conn.InsertAsync(h);
        }
        public Task<List<Habito>> GetHabitosPorUsuario(int idUsuario)
        {
            return _conn.Table<Habito>()
                        .Where(h => h.IdCadastro == idUsuario)
                        .ToListAsync();
        }
        public Task<Habito> GetHabitoPorId(int idH)
        {
            return _conn.Table<Habito>()
                        .Where(h => h.IdHabito == idH)
                        .FirstOrDefaultAsync();
        }
        public Task<List<Habito>> GetHabitos()
        {
            return _conn.Table<Habito>().ToListAsync();
        }

        public Task<int> UpdateHabito(Habito h)
        {
            return _conn.UpdateAsync(h);
        }
        public Task<int> DeleteHabito(int idH)
        {
            return _conn.Table<Habito>()
                        .DeleteAsync(h => h.IdHabito == idH);
        }
        
        #endregion
        #region Notificação
        public Task<int> InsertNotificacao(Notificacao n)
        {
            return _conn.InsertAsync(n);
        }
        public Task<List<Notificacao>> GetNotificacaoPorLembrete(int idLembrete)
        {
            return _conn.Table<Notificacao>()
                        .Where(n => n.IdLembrete == idLembrete)
                        .ToListAsync();
        }
        public Task<Notificacao> GetNotificacaoPorId(int idN)
        {
            return _conn.Table<Notificacao>()
                        .Where(n => n.IdNotificacao == idN)
                        .FirstOrDefaultAsync();
        }
        public Task<List<Notificacao>> GetNotificacoes()
        {
            return _conn.Table<Notificacao>().ToListAsync();
        }

        public Task<int> UpdateNotificacao(Notificacao n)
        {
            return _conn.UpdateAsync(n);
        }
        public Task<int> DeleteNotificacao(int idN)
        {
            return _conn.Table<Notificacao>()
                        .DeleteAsync(n => n.IdNotificacao == idN);
        }
        #endregion
        #region Lembrete
        public Task<int> InsertLembrete(Lembrete l)
        {
            return _conn.InsertAsync(l);
        }
        public Task<List<Lembrete>> GetLembretePorUsuario(int idUsuario)
        {
            return _conn.Table<Lembrete>()
                        .Where(l => l.IdCadastro == idUsuario)
                        .ToListAsync();
        }
        public Task<Lembrete> GetLembretePorId(int idL)
        {
            return _conn.Table<Lembrete>()
                        .Where(l => l.IdLembrete == idL)
                        .FirstOrDefaultAsync();
        }

        public Task<int> UpdateLembrete(Lembrete l)
        {
            return _conn.UpdateAsync(l);
        }
        public Task<int> DeleteLembrete(int idL)
        {
            return _conn.Table<Lembrete>()
                        .DeleteAsync(l => l.IdLembrete == idL);
        }

        #endregion
    }

}

