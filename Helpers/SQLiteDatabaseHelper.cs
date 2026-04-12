using MauiAppAMASBE.Models;
using SQLite;

namespace MauiAppAMASBE.Helpers
{
    public class SQLiteDatabaseHelper
    {
        readonly SQLiteAsyncConnection _conn;

        public SQLiteDatabaseHelper(string path)
        {
            _conn = new SQLiteAsyncConnection(path);

            _conn.CreateTableAsync<CadastroSaudeUsuario>().Wait();
            _conn.CreateTableAsync<Habito>().Wait();
            _conn.CreateTableAsync<Lembrete>().Wait();
            _conn.CreateTableAsync<ConteudoSaude>().Wait();
            _conn.CreateTableAsync<Notificacao>().Wait();
            _conn.CreateTableAsync<PerguntaFrequente>().Wait();
            _conn.CreateTableAsync<MensagemMotivacional>().Wait();
            _conn.CreateTableAsync<Medicamento>().Wait();
            _conn.CreateTableAsync<Alergia>().Wait();
            _conn.CreateTableAsync<DoencaCronica>().Wait();
            _conn.CreateTableAsync<Vacina>().Wait();
            _conn.CreateTableAsync<ServicoSus>().Wait();
        }

        #region USUÁRIO
        public Task<int> InsertUsuario(CadastroSaudeUsuario u)
        {
            return _conn.InsertAsync(u);
        }

        public Task<int> UpdateUsuario(CadastroSaudeUsuario u)
        {
            return _conn.UpdateAsync(u);
        }

        public Task<int> DeleteUsuario(int id)
        {
            return _conn.Table<CadastroSaudeUsuario>()
                        .DeleteAsync(x => x.IdCadastro == id);
        }

        public Task<List<CadastroSaudeUsuario>> GetUsuarios()
        {
            return _conn.Table<CadastroSaudeUsuario>().ToListAsync();
        }

        public Task<CadastroSaudeUsuario> GetUsuarioPorId(int id)
        {
            return _conn.Table<CadastroSaudeUsuario>()
                        .Where(x => x.IdCadastro == id)
                        .FirstOrDefaultAsync();
        }

        public Task<CadastroSaudeUsuario> GetUsuarioSenha(string login, string senha)
        {
            return _conn.Table<CadastroSaudeUsuario>()
                        .Where(x => (x.Email == login || x.Cpf == login) && x.Senha == senha)
                        .FirstOrDefaultAsync();
        }

        public Task<CadastroSaudeUsuario> GetUsuarioPorCpf(string cpf)
        {
            return _conn.Table<CadastroSaudeUsuario>()
                        .Where(x => x.Cpf == cpf)
                        .FirstOrDefaultAsync();
        }

        public async Task<bool> EmailExiste(string email)
        {
            var usuario = await _conn.Table<CadastroSaudeUsuario>()
                                     .Where(x => x.Email == email)
                                     .FirstOrDefaultAsync();

            return usuario != null;
        }
        #endregion

        #region HÁBITO
        public Task<int> InsertHabito(Habito h) => _conn.InsertAsync(h);

        public Task<List<Habito>> GetHabitosPorUsuario(int idUsuario)
        {
            return _conn.Table<Habito>()
                        .Where(h => h.IdCadastro == idUsuario)
                        .ToListAsync();
        }

        public Task<int> DeleteHabito(int id)
        {
            return _conn.Table<Habito>().DeleteAsync(h => h.IdHabito == id);
        }
        #endregion

        #region LEMBRETE
        public Task<int> InsertLembrete(Lembrete l) => _conn.InsertAsync(l);

        public Task<List<Lembrete>> GetLembretePorUsuario(int idUsuario)
        {
            return _conn.Table<Lembrete>()
                        .Where(l => l.IdCadastro == idUsuario)
                        .ToListAsync();
        }

        public Task<int> DeleteLembrete(int id)
        {
            return _conn.Table<Lembrete>().DeleteAsync(l => l.IdLembrete == id);
        }
        #endregion

        #region CONTEÚDO
        public Task<int> InsertConteudo(ConteudoSaude c) => _conn.InsertAsync(c);

        public Task<List<ConteudoSaude>> GetConteudo()
        {
            return _conn.Table<ConteudoSaude>().ToListAsync();
        }

        public Task<int> DeleteConteudo(int id)
        {
            return _conn.Table<ConteudoSaude>().DeleteAsync(c => c.IdConteudo == id);
        }
        #endregion

        #region NOTIFICAÇÃO
        public Task<int> InsertNotificacao(Notificacao n) => _conn.InsertAsync(n);

        public Task<List<Notificacao>> GetNotificacoes()
        {
            return _conn.Table<Notificacao>().ToListAsync();
        }

        public Task<int> UpdateNotificacao(Notificacao n)
        {
            return _conn.UpdateAsync(n);
        }
        #endregion
    }
}