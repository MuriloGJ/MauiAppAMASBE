using MauiAppAMASBE.Models;
using SQLite;

namespace MauiAppAMASBE.Helpers
{
    public class SQLiteDatabaseHelper
    {
        readonly SQLiteAsyncConnection _conn;
        private bool _initialized = false;
        private readonly SemaphoreSlim _initLock = new SemaphoreSlim(1, 1);

        public SQLiteDatabaseHelper(string path)
        {
            _conn = new SQLiteAsyncConnection(path);
            // CORREÇÃO: não usar .Wait() no construtor — usa inicialização lazy assíncrona
        }

        /// <summary>
        /// Garante que as tabelas existam antes de qualquer operação.
        /// Usa SemaphoreSlim para evitar race condition em chamadas paralelas.
        /// </summary>
        private async Task EnsureInitializedAsync()
        {
            if (_initialized) return;
            await _initLock.WaitAsync();
            try
            {
                if (_initialized) return;
                await _conn.CreateTableAsync<CadastroSaudeUsuario>();
                await _conn.CreateTableAsync<Medicamento>();
                await _conn.CreateTableAsync<Alergia>();
                await _conn.CreateTableAsync<DoencaCronica>();
                await _conn.CreateTableAsync<Vacina>();
                await _conn.CreateTableAsync<Habito>();
                await _conn.CreateTableAsync<MensagemMotivacional>();
                await _conn.CreateTableAsync<Lembrete>();
                await _conn.CreateTableAsync<Notificacao>();
                await _conn.CreateTableAsync<ConteudoSaude>();
                await _conn.CreateTableAsync<PerguntaFrequente>();
                await _conn.CreateTableAsync<ServicoSus>();
                _initialized = true;
            }
            finally { _initLock.Release(); }
        }

        #region CadastroSaudeUsuario
        public async Task<int> InsertUsuario(CadastroSaudeUsuario u)
        {
            await EnsureInitializedAsync();
            return await _conn.InsertAsync(u);
        }
        public async Task<int> UpdateUsuario(CadastroSaudeUsuario u)
        {
            await EnsureInitializedAsync();
            return await _conn.UpdateAsync(u);
        }
        public async Task<CadastroSaudeUsuario> GetUsuarioPorId(int id)
        {
            await EnsureInitializedAsync();
            return await _conn.Table<CadastroSaudeUsuario>()
                               .Where(u => u.IdCadastro == id).FirstOrDefaultAsync();
        }
        public async Task<List<CadastroSaudeUsuario>> GetUsuarios()
        {
            await EnsureInitializedAsync();
            return await _conn.Table<CadastroSaudeUsuario>().ToListAsync();
        }
        public async Task<CadastroSaudeUsuario> GetUsuario(string email, string nomeUsuario, string senha)
        {
            await EnsureInitializedAsync();
            return await _conn.Table<CadastroSaudeUsuario>()
                .Where(u => ((email != null && u.Email == email) ||
                             (nomeUsuario != null && u.NomeUsuario == nomeUsuario))
                             && u.Senha == senha)
                .FirstOrDefaultAsync();
        }
        public async Task<CadastroSaudeUsuario> GetUsuarioSenha(string login, string senha)
        {
            await EnsureInitializedAsync();
            return await _conn.Table<CadastroSaudeUsuario>()
                .Where(u => (u.Email == login || u.Cpf == login) && u.Senha == senha)
                .FirstOrDefaultAsync();
        }
        public async Task<CadastroSaudeUsuario> GetUsuarioPorNomeUsuario(string nomeUsuario)
        {
            await EnsureInitializedAsync();
            return await _conn.Table<CadastroSaudeUsuario>()
                               .Where(u => u.NomeUsuario == nomeUsuario).FirstOrDefaultAsync();
        }
        public async Task<int> DeleteUsuario(int id)
        {
            await EnsureInitializedAsync();
            return await _conn.Table<CadastroSaudeUsuario>().DeleteAsync(u => u.IdCadastro == id);
        }
        public async Task<bool> EmailExiste(string email)
        {
            await EnsureInitializedAsync();
            var u = await _conn.Table<CadastroSaudeUsuario>()
                                .Where(x => x.Email == email).FirstOrDefaultAsync();
            return u != null;
        }
        public async Task<List<CadastroSaudeUsuario>> SearchUsuario(string u)
        {
            await EnsureInitializedAsync();
            return await _conn.QueryAsync<CadastroSaudeUsuario>(
                "SELECT * FROM CadastroSaudeUsuario WHERE Nome LIKE ?", "%" + u + "%");
        }
        public async Task CriarAdministradorPadrao()
        {
            await EnsureInitializedAsync();
            var adminExistente = await _conn.Table<CadastroSaudeUsuario>()
                .Where(u => u.TipoUsuario == "Administrador").FirstOrDefaultAsync();
            if (adminExistente == null)
            {
                await _conn.InsertAsync(new CadastroSaudeUsuario
                {
                    Nome = "admin", NomeUsuario = "admin", TipoUsuario = "Administrador",
                    DataNascimento = new DateTime(2000, 1, 1), Sexo = "Outro",
                    Cpf = "00000000000", RuaUsuario = "", NumeroUsuario = "",
                    BairroUsuario = "", CidadeUsuario = "", EstadoUsuario = "",
                    CepUsuario = "", ComplementoUsuario = "", TelefoneUsuario = "",
                    Email = "admin@admin.com", ContatoEmergencia = "",
                    TipoSanguineo = "O+", Peso = 0, Altura = 0, Senha = "admin123"
                });
            }
        }
        #endregion

        #region Habito
        public async Task<int> InsertHabito(Habito h)
        {
            await EnsureInitializedAsync(); return await _conn.InsertAsync(h);
        }
        public async Task<List<Habito>> GetHabitosPorUsuario(int idUsuario)
        {
            await EnsureInitializedAsync();
            return await _conn.Table<Habito>().Where(h => h.IdCadastro == idUsuario).ToListAsync();
        }
        public async Task<Habito> GetHabitoPorId(int idH)
        {
            await EnsureInitializedAsync();
            return await _conn.Table<Habito>().Where(h => h.IdHabito == idH).FirstOrDefaultAsync();
        }
        public async Task<List<Habito>> GetHabitos()
        {
            await EnsureInitializedAsync(); return await _conn.Table<Habito>().ToListAsync();
        }
        public async Task<int> UpdateHabito(Habito h)
        {
            await EnsureInitializedAsync(); return await _conn.UpdateAsync(h);
        }
        public async Task<int> DeleteHabito(int idH)
        {
            await EnsureInitializedAsync();
            return await _conn.Table<Habito>().DeleteAsync(h => h.IdHabito == idH);
        }
        public async Task<List<Habito>> SearchHabito(string h)
        {
            await EnsureInitializedAsync();
            return await _conn.QueryAsync<Habito>(
                "SELECT * FROM Habito WHERE NomeHabito LIKE ?", "%" + h + "%");
        }
        #endregion

        #region Notificação
        public async Task<int> InsertNotificacao(Notificacao n)
        {
            await EnsureInitializedAsync(); return await _conn.InsertAsync(n);
        }
        public async Task<List<Notificacao>> GetNotificacaoPorLembrete(int idLembrete)
        {
            await EnsureInitializedAsync();
            return await _conn.Table<Notificacao>().Where(n => n.IdLembrete == idLembrete).ToListAsync();
        }
        public async Task<Notificacao> GetNotificacaoPorId(int idN)
        {
            await EnsureInitializedAsync();
            return await _conn.Table<Notificacao>().Where(n => n.IdNotificacao == idN).FirstOrDefaultAsync();
        }
        public async Task<List<Notificacao>> GetNotificacoes()
        {
            await EnsureInitializedAsync(); return await _conn.Table<Notificacao>().ToListAsync();
        }
        public async Task<int> UpdateNotificacao(Notificacao n)
        {
            await EnsureInitializedAsync(); return await _conn.UpdateAsync(n);
        }
        public async Task<int> DeleteNotificacao(int idN)
        {
            await EnsureInitializedAsync();
            return await _conn.Table<Notificacao>().DeleteAsync(n => n.IdNotificacao == idN);
        }
        #endregion

        #region Lembrete
        public async Task<int> InsertLembrete(Lembrete l)
        {
            await EnsureInitializedAsync(); return await _conn.InsertAsync(l);
        }
        public async Task<List<Lembrete>> GetLembrete()
        {
            await EnsureInitializedAsync(); return await _conn.Table<Lembrete>().ToListAsync();
        }
        public async Task<List<Lembrete>> GetLembretePorUsuario(int idUsuario)
        {
            await EnsureInitializedAsync();
            return await _conn.Table<Lembrete>().Where(l => l.IdCadastro == idUsuario).ToListAsync();
        }
        public async Task<Lembrete> GetLembretePorId(int idL)
        {
            await EnsureInitializedAsync();
            return await _conn.Table<Lembrete>().Where(l => l.IdLembrete == idL).FirstOrDefaultAsync();
        }
        public async Task<int> UpdateLembrete(Lembrete l)
        {
            await EnsureInitializedAsync(); return await _conn.UpdateAsync(l);
        }
        public async Task<int> DeleteLembrete(int idL)
        {
            await EnsureInitializedAsync();
            return await _conn.Table<Lembrete>().DeleteAsync(l => l.IdLembrete == idL);
        }
        #endregion

        #region Medicamento
        public async Task<int> InsertMedicamento(Medicamento m)
        {
            await EnsureInitializedAsync(); return await _conn.InsertAsync(m);
        }
        public async Task<List<Medicamento>> GetMedicamentoPorUsuario(int idUsuario)
        {
            await EnsureInitializedAsync();
            return await _conn.Table<Medicamento>().Where(m => m.IdCadastro == idUsuario).ToListAsync();
        }
        public async Task<Medicamento> GetMedicamentoPorId(int idM)
        {
            await EnsureInitializedAsync();
            return await _conn.Table<Medicamento>().Where(m => m.IdMedicamento == idM).FirstOrDefaultAsync();
        }
        public async Task<int> UpdateMedicamento(Medicamento m)
        {
            await EnsureInitializedAsync(); return await _conn.UpdateAsync(m);
        }
        public async Task<int> DeleteMedicamento(int idM)
        {
            await EnsureInitializedAsync();
            return await _conn.Table<Medicamento>().DeleteAsync(m => m.IdMedicamento == idM);
        }
        public async Task<List<Medicamento>> SearchMedicamento(string m)
        {
            await EnsureInitializedAsync();
            return await _conn.QueryAsync<Medicamento>(
                "SELECT * FROM Medicamento WHERE NomeMedicamento LIKE ?", "%" + m + "%");
        }
        #endregion

        #region Alergia
        public async Task<int> InsertAlergia(Alergia a)
        {
            await EnsureInitializedAsync(); return await _conn.InsertAsync(a);
        }
        public async Task<List<Alergia>> GetAlergiaPorUsuario(int idUsuario)
        {
            await EnsureInitializedAsync();
            return await _conn.Table<Alergia>().Where(a => a.IdCadastro == idUsuario).ToListAsync();
        }
        public async Task<Alergia> GetAlergiaPorId(int idA)
        {
            await EnsureInitializedAsync();
            return await _conn.Table<Alergia>().Where(a => a.IdAlergia == idA).FirstOrDefaultAsync();
        }
        public async Task<int> UpdateAlergia(Alergia a)
        {
            await EnsureInitializedAsync(); return await _conn.UpdateAsync(a);
        }
        public async Task<int> DeleteAlergia(int idA)
        {
            await EnsureInitializedAsync();
            return await _conn.Table<Alergia>().DeleteAsync(a => a.IdAlergia == idA);
        }
        public async Task<List<Alergia>> SearchAlergia(string a)
        {
            await EnsureInitializedAsync();
            return await _conn.QueryAsync<Alergia>(
                "SELECT * FROM Alergia WHERE NomeAlergia LIKE ?", "%" + a + "%");
        }
        #endregion

        #region DoencaCronica
        public async Task<int> InsertDoenca(DoencaCronica d)
        {
            await EnsureInitializedAsync(); return await _conn.InsertAsync(d);
        }
        public async Task<List<DoencaCronica>> GetDoencaPorUsuario(int idUsuario)
        {
            await EnsureInitializedAsync();
            return await _conn.Table<DoencaCronica>().Where(d => d.IdCadastro == idUsuario).ToListAsync();
        }
        public async Task<DoencaCronica> GetDoencaPorId(int idD)
        {
            await EnsureInitializedAsync();
            return await _conn.Table<DoencaCronica>().Where(d => d.IdDoenca == idD).FirstOrDefaultAsync();
        }
        public async Task<List<DoencaCronica>> GetDoenca()
        {
            await EnsureInitializedAsync(); return await _conn.Table<DoencaCronica>().ToListAsync();
        }
        public async Task<int> UpdateDoenca(DoencaCronica d)
        {
            await EnsureInitializedAsync(); return await _conn.UpdateAsync(d);
        }
        public async Task<int> DeleteDoenca(int idD)
        {
            await EnsureInitializedAsync();
            return await _conn.Table<DoencaCronica>().DeleteAsync(d => d.IdDoenca == idD);
        }
        public async Task<List<DoencaCronica>> SearchDoenca(string d)
        {
            await EnsureInitializedAsync();
            return await _conn.QueryAsync<DoencaCronica>(
                "SELECT * FROM DoencaCronica WHERE NomeDoenca LIKE ?", "%" + d + "%");
        }
        #endregion

        #region Vacina
        public async Task<int> InsertVacina(Vacina v)
        {
            await EnsureInitializedAsync(); return await _conn.InsertAsync(v);
        }
        public async Task<List<Vacina>> GetVacinaPorUsuario(int idUsuario)
        {
            await EnsureInitializedAsync();
            return await _conn.Table<Vacina>().Where(v => v.IdCadastro == idUsuario).ToListAsync();
        }
        public async Task<Vacina> GetVacinaPorId(int idV)
        {
            await EnsureInitializedAsync();
            return await _conn.Table<Vacina>().Where(v => v.IdVacina == idV).FirstOrDefaultAsync();
        }
        public async Task<List<Vacina>> GetVacina()
        {
            await EnsureInitializedAsync(); return await _conn.Table<Vacina>().ToListAsync();
        }
        public async Task<int> UpdateVacina(Vacina v)
        {
            await EnsureInitializedAsync(); return await _conn.UpdateAsync(v);
        }
        public async Task<int> DeleteVacina(int idV)
        {
            await EnsureInitializedAsync();
            return await _conn.Table<Vacina>().DeleteAsync(v => v.IdVacina == idV);
        }
        public async Task<List<Vacina>> SearchVacina(string v)
        {
            await EnsureInitializedAsync();
            return await _conn.QueryAsync<Vacina>(
                "SELECT * FROM Vacina WHERE NomeVacina LIKE ?", "%" + v + "%");
        }
        #endregion

        #region MensagemMotivacional
        public async Task<int> InsertMensagem(MensagemMotivacional msg)
        {
            await EnsureInitializedAsync(); return await _conn.InsertAsync(msg);
        }
        public async Task<MensagemMotivacional> GetMensagemId(int idMSG)
        {
            await EnsureInitializedAsync();
            return await _conn.Table<MensagemMotivacional>()
                               .Where(msg => msg.IdMensagem == idMSG).FirstOrDefaultAsync();
        }
        public async Task<List<MensagemMotivacional>> GetMensagem()
        {
            await EnsureInitializedAsync(); return await _conn.Table<MensagemMotivacional>().ToListAsync();
        }
        public async Task<int> UpdateMensagem(MensagemMotivacional msg)
        {
            await EnsureInitializedAsync(); return await _conn.UpdateAsync(msg);
        }
        public async Task<int> DeleteMensagem(int idMSG)
        {
            await EnsureInitializedAsync();
            return await _conn.Table<MensagemMotivacional>()
                               .DeleteAsync(msg => msg.IdMensagem == idMSG);
        }
        #endregion

        #region ConteudoSaude
        public async Task<int> InsertConteudo(ConteudoSaude c)
        {
            await EnsureInitializedAsync(); return await _conn.InsertAsync(c);
        }
        public async Task<ConteudoSaude> GetConteudoPorId(int idC)
        {
            await EnsureInitializedAsync();
            return await _conn.Table<ConteudoSaude>().Where(c => c.IdConteudo == idC).FirstOrDefaultAsync();
        }
        public async Task<List<ConteudoSaude>> GetConteudo()
        {
            await EnsureInitializedAsync(); return await _conn.Table<ConteudoSaude>().ToListAsync();
        }
        public async Task<int> UpdateConteudo(ConteudoSaude c)
        {
            await EnsureInitializedAsync(); return await _conn.UpdateAsync(c);
        }
        public async Task<int> DeleteConteudo(int idC)
        {
            await EnsureInitializedAsync();
            return await _conn.Table<ConteudoSaude>().DeleteAsync(c => c.IdConteudo == idC);
        }
        public async Task<List<ConteudoSaude>> SearchConteudoTitulo(string ct)
        {
            await EnsureInitializedAsync();
            return await _conn.QueryAsync<ConteudoSaude>(
                "SELECT * FROM ConteudoSaude WHERE TituloConteudo LIKE ?", "%" + ct + "%");
        }
        public async Task<List<ConteudoSaude>> SearchConteudoCategoria(string ct)
        {
            await EnsureInitializedAsync();
            return await _conn.QueryAsync<ConteudoSaude>(
                "SELECT * FROM ConteudoSaude WHERE Categoria LIKE ?", "%" + ct + "%");
        }
        public async Task CriarConteudosPadrao()
        {
            await EnsureInitializedAsync();
            var conteudos = await _conn.Table<ConteudoSaude>().ToListAsync();
            if (conteudos.Count == 0)
            {
                await _conn.InsertAllAsync(new List<ConteudoSaude>
                {
                    new ConteudoSaude { TituloConteudo = "Importância da hidratação",
                        CategoriaConteudo = "Bem-estar",
                        TextoConteudo = "Beber água diariamente ajuda no funcionamento do organismo.",
                        Favorito = false, OfflineDisponivel = true },
                    new ConteudoSaude { TituloConteudo = "Sono saudável",
                        CategoriaConteudo = "Saúde",
                        TextoConteudo = "Dormir bem melhora a imunidade e a concentração.",
                        Favorito = false, OfflineDisponivel = true }
                });
            }
        }
        #endregion

        #region PerguntaFrequente
        public async Task<int> InsertPergunta(PerguntaFrequente p)
        {
            await EnsureInitializedAsync(); return await _conn.InsertAsync(p);
        }
        public async Task<PerguntaFrequente> GetPerguntaPorId(int idP)
        {
            await EnsureInitializedAsync();
            return await _conn.Table<PerguntaFrequente>().Where(p => p.IdPergunta == idP).FirstOrDefaultAsync();
        }
        public async Task<List<PerguntaFrequente>> GetPergunta()
        {
            await EnsureInitializedAsync(); return await _conn.Table<PerguntaFrequente>().ToListAsync();
        }
        public async Task<int> UpdatePergunta(PerguntaFrequente p)
        {
            await EnsureInitializedAsync(); return await _conn.UpdateAsync(p);
        }
        public async Task<int> DeletePergunta(int idP)
        {
            await EnsureInitializedAsync();
            return await _conn.Table<PerguntaFrequente>().DeleteAsync(p => p.IdPergunta == idP);
        }
        public async Task<List<PerguntaFrequente>> SearchPerguntaCategoria(string pc)
        {
            await EnsureInitializedAsync();
            return await _conn.QueryAsync<PerguntaFrequente>(
                "SELECT * FROM PerguntaFrequente WHERE CategoriaPergunta LIKE ?", "%" + pc + "%");
        }
        #endregion

        #region ServicoSus
        public async Task<int> InsertServico(ServicoSus s)
        {
            await EnsureInitializedAsync(); return await _conn.InsertAsync(s);
        }
        public async Task<ServicoSus> GetServicoPorId(int idS)
        {
            await EnsureInitializedAsync();
            return await _conn.Table<ServicoSus>().Where(s => s.IdServico == idS).FirstOrDefaultAsync();
        }
        public async Task<List<ServicoSus>> GetServico()
        {
            await EnsureInitializedAsync(); return await _conn.Table<ServicoSus>().ToListAsync();
        }
        public async Task<int> UpdateServico(ServicoSus s)
        {
            await EnsureInitializedAsync(); return await _conn.UpdateAsync(s);
        }
        public async Task<int> DeleteServico(int idS)
        {
            await EnsureInitializedAsync();
            return await _conn.Table<ServicoSus>().DeleteAsync(s => s.IdServico == idS);
        }
        public async Task<List<ServicoSus>> SearchServicoNome(string sn)
        {
            await EnsureInitializedAsync();
            return await _conn.QueryAsync<ServicoSus>(
                "SELECT * FROM ServicoSus WHERE NomeServico LIKE ?", "%" + sn + "%");
        }
        public async Task<List<ServicoSus>> SearchServicoTipo(string st)
        {
            await EnsureInitializedAsync();
            return await _conn.QueryAsync<ServicoSus>(
                "SELECT * FROM ServicoSus WHERE TipoServico LIKE ?", "%" + st + "%");
        }
        #endregion
    }
}
