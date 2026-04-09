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
        //precisa definir o que é progresso e o metodo exibir progresso



        #endregion
    }
}

