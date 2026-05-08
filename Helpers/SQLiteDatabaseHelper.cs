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
        #region CadastroSaudeUsuario //organização dos metodos
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
        public async Task<CadastroSaudeUsuario> GetUsuario(string email, string nomeUsuario, string senha)
        {
            return await _conn.Table<CadastroSaudeUsuario>()
                .Where(u => ((email != null && u.Email == email) ||(nomeUsuario != null && u.NomeUsuario == nomeUsuario)) && u.Senha == senha)
                .FirstOrDefaultAsync();
        }

        public Task<CadastroSaudeUsuario> GetUsuarioSenha(string login, string senha)
        {
            return _conn.Table<CadastroSaudeUsuario>()
                .Where(u => (u.Email == login || u.Cpf == login) && u.Senha == senha)
                .FirstOrDefaultAsync();
        }
        public Task<CadastroSaudeUsuario> GetUsuarioPorNomeUsuario(string nomeUsuario)
        {
            return _conn.Table<CadastroSaudeUsuario>()
                        .Where(u => u.NomeUsuario == nomeUsuario)
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
        public Task<List<CadastroSaudeUsuario>> SearchUsuario(string u)
        {
            string sql = "SELECT * FROM CadastroSaudeUsuario WHERE Nome LIKE ?";
            return _conn.QueryAsync<CadastroSaudeUsuario>(sql, "%" + u + "%");
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
        public Task<List<Habito>> SearchHabito(string h)
        {
            string sql = "SELECT * FROM Habito WHERE NomeHabito LIKE ?";
            return _conn.QueryAsync<Habito>(sql, "%" + h + "%");
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
        public Task<List<Lembrete>> GetLembrete()
        {
            return _conn.Table<Lembrete>().ToListAsync();
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
        #region Medicamento
        public Task<int> InsertMedicamento(Medicamento m)
        {
            return _conn.InsertAsync(m);
        }
        public Task<List<Medicamento>> GetMedicamentoPorUsuario(int idUsuario)
        {
            return _conn.Table<Medicamento>()
                        .Where(m => m.IdCadastro == idUsuario)
                        .ToListAsync();
        }
        public Task<Medicamento> GetMedicamentoPorId(int idM)
        {
            return _conn.Table<Medicamento>()
                        .Where(m => m.IdMedicamento == idM)
                        .FirstOrDefaultAsync();
        }
        public Task<int> UpdateMedicamento(Medicamento m)
        {
            return _conn.UpdateAsync(m);
        }
        public Task<int> DeleteMedicamento(int idM)
        {
            return _conn.Table<Medicamento>()
                        .DeleteAsync(m => m.IdMedicamento == idM);
        }
        public Task<List<Medicamento>> SearchMedicamento(string m)
        {
            string sql = "SELECT * FROM Medicamento WHERE NomeMedicamento LIKE ?";
            return _conn.QueryAsync<Medicamento>(sql, "%" + m + "%");
        }

        #endregion
        #region Alergia
        public Task<int> InsertAlergia(Alergia a)
        {
            return _conn.InsertAsync(a);
        }
        public Task<List<Alergia>> GetAlergiaPorUsuario(int idUsuario)
        {
            return _conn.Table<Alergia>()
                        .Where(a => a.IdCadastro == idUsuario)
                        .ToListAsync();
        }
        public Task<Alergia> GetAlergiaPorId(int idA)
        {
            return _conn.Table<Alergia>()
                        .Where(a => a.IdAlergia == idA)
                        .FirstOrDefaultAsync();
        }
        public Task<int> UpdateAlergia(Alergia a)
        {
            return _conn.UpdateAsync(a);
        }
        public Task<int> DeleteAlergia(int idA)
        {
            return _conn.Table<Alergia>()
                        .DeleteAsync(a => a.IdAlergia == idA);
        }
        public Task<List<Alergia>> SearchAlergia(string a)
        {
            string sql = "SELECT * FROM Alergia WHERE NomeAlergia LIKE ?";
            return _conn.QueryAsync<Alergia>(sql, "%" + a + "%");
        }

        #endregion
        #region DoencaCronica
        public Task<int> InsertDoenca(DoencaCronica d)
        {
            return _conn.InsertAsync(d);
        }
        public Task<List<DoencaCronica>> GetDoencaPorUsuario(int idUsuario)
        {
            return _conn.Table<DoencaCronica>()
                        .Where(d => d.IdCadastro == idUsuario)
                        .ToListAsync();
        }
        public Task<DoencaCronica> GetDoencaPorId(int idD)
        {
            return _conn.Table<DoencaCronica>()
                        .Where(d => d.IdDoenca == idD)
                        .FirstOrDefaultAsync();
        }
        public Task<List<DoencaCronica>> GetDoenca()
        {
            return _conn.Table<DoencaCronica>().ToListAsync();
        }

        public Task<int> UpdateDoenca(DoencaCronica d)
        {
            return _conn.UpdateAsync(d);
        }
        public Task<int> DeleteDoenca(int idD)
        {
            return _conn.Table<DoencaCronica>()
                        .DeleteAsync(d => d.IdDoenca == idD);
        }
        public Task<List<DoencaCronica>> SearchDoenca(string d)
        {
            string sql = "SELECT * FROM DoencaCronica WHERE NomeDoenca LIKE ?";
            return _conn.QueryAsync<DoencaCronica>(sql, "%" + d + "%");
        }
        #endregion
        #region Vacina
        public Task<int> InsertVacina(Vacina v)
        {
            return _conn.InsertAsync(v);
        }
        public Task<List<Vacina>> GetVacinaPorUsuario(int idUsuario)
        {
            return _conn.Table<Vacina>()
                        .Where(v => v.IdCadastro == idUsuario)
                        .ToListAsync();
        }
        public Task<Vacina> GetVacinaPorId(int idV)
        {
            return _conn.Table<Vacina>()
                        .Where(v => v.IdVacina == idV)
                        .FirstOrDefaultAsync();
        }
        public Task<List<Vacina>> GetVacina()
        {
            return _conn.Table<Vacina>().ToListAsync();
        }

        public Task<int> UpdateVacina(Vacina v)
        {
            return _conn.UpdateAsync(v);
        }
        public Task<int> DeleteVacina(int idV)
        {
            return _conn.Table<Vacina>()
                        .DeleteAsync(v => v.IdVacina == idV);
        }
        public Task<List<Vacina>> SearchVacina(string v)
        {
            string sql = "SELECT * FROM Vacina WHERE NomeVacina LIKE ?";
            return _conn.QueryAsync<Vacina>(sql, "%" + v + "%");
        }
        #endregion
        #region MensagemMotivacional
        public Task<int> InsertMensagem(MensagemMotivacional msg)
        {
            return _conn.InsertAsync(msg);
        }

        public Task<MensagemMotivacional> GetMensagemId(int idMSG)
        {
            return _conn.Table<MensagemMotivacional>()
                        .Where(msg => msg.IdMensagem == idMSG)
                        .FirstOrDefaultAsync();
        }
        public Task<List<MensagemMotivacional>> GetMensagem()
        {
            return _conn.Table<MensagemMotivacional>().ToListAsync();
        }

        public Task<int> UpdateMensagem(MensagemMotivacional msg)
        {
            return _conn.UpdateAsync(msg);
        }
        public Task<int> DeleteMensagem(int idMSG)
        {
            return _conn.Table<MensagemMotivacional>()
                        .DeleteAsync(msg => msg.IdMensagem == idMSG);
        }
        #endregion
        #region ConteudoSaude
        public Task<int> InsertConteudo(ConteudoSaude c)
        {
            return _conn.InsertAsync(c);
        }

        public Task<ConteudoSaude> GetConteudoPorId(int idC)
        {
            return _conn.Table<ConteudoSaude>()
                        .Where(c => c.IdConteudo == idC)
                        .FirstOrDefaultAsync();
        }
        public Task<List<ConteudoSaude>> GetConteudo()
        {
            return _conn.Table<ConteudoSaude>().ToListAsync();
        }

        public Task<int> UpdateConteudo(ConteudoSaude c)
        {
            return _conn.UpdateAsync(c);
        }
        public Task<int> DeleteConteudo(int idC)
        {
            return _conn.Table<ConteudoSaude>()
                        .DeleteAsync(c => c.IdConteudo == idC);
        }
        public Task<List<ConteudoSaude>> SearchConteudoTitulo(string ct)
        {
            string sql = "SELECT * FROM ConteudoSaude WHERE TituloConteudo LIKE ?";
            return _conn.QueryAsync<ConteudoSaude>(sql, "%" + ct + "%");

        }
        public Task<List<ConteudoSaude>> SearchConteudoCategoria(string ct)
        {
            string sql = "SELECT * FROM ConteudoSaude WHERE Categoria LIKE ?";
            return _conn.QueryAsync<ConteudoSaude>(sql, "%" + ct + "%");
        }
        #endregion
        #region PerguntaFrequente
        public Task<int> InsertPergunta(PerguntaFrequente p)
        {
            return _conn.InsertAsync(p);
        }

        public Task<PerguntaFrequente> GetPerguntaPorId(int idP)
        {
            return _conn.Table<PerguntaFrequente>()
                        .Where(p => p.IdPergunta == idP)
                        .FirstOrDefaultAsync();
        }
        public Task<List<PerguntaFrequente>> GetPergunta()
        {
            return _conn.Table<PerguntaFrequente>().ToListAsync();
        }

        public Task<int> UpdatePergunta(PerguntaFrequente p)
        {
            return _conn.UpdateAsync(p);
        }
        public Task<int> DeletePergunta(int idP)
        {
            return _conn.Table<PerguntaFrequente>()
                        .DeleteAsync(p => p.IdPergunta == idP);
        }
        public Task<List<PerguntaFrequente>> SearchPerguntaCategoria(string pc)
        {
            string sql = "SELECT * FROM PerguntaFrequente WHERE CategoriaPergunta LIKE ?";
            return _conn.QueryAsync<PerguntaFrequente>(sql, "%" + pc + "%");
        }
        #endregion
        #region ServicoSus
        public Task<int> InsertServico(ServicoSus s)
        {
            return _conn.InsertAsync(s);
        }

        public Task<ServicoSus> GetServicoPorId(int idS)
        {
            return _conn.Table<ServicoSus>()
                        .Where(s => s.IdServico == idS)
                        .FirstOrDefaultAsync();
        }
        public Task<List<ServicoSus>> GetServico()
        {
            return _conn.Table<ServicoSus>().ToListAsync();
        }

        public Task<int> UpdateServico(ServicoSus s)
        {
            return _conn.UpdateAsync(s);
        }
        public Task<int> DeleteServico(int idS)
        {
            return _conn.Table<ServicoSus>()
                        .DeleteAsync(s => s.IdServico == idS);
        }
        public Task<List<ServicoSus>> SearchServicoNome(string sn)
        {
            string sql = "SELECT * FROM ServicoSus WHERE NomeServico LIKE ?";
            return _conn.QueryAsync<ServicoSus>(sql, "%" + sn + "%");
        }
        public Task<List<ServicoSus>> SearchServicoTipo(string st)
        {
            string sql = "SELECT * FROM ServicoSus WHERE TipoServico LIKE ?";
            return _conn.QueryAsync<ServicoSus>(sql, "%" + st + "%");
        }
        #endregion
    }

}

