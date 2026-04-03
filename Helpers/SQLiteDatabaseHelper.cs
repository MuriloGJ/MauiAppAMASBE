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
    }
}

