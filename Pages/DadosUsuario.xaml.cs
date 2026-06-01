using MauiAppAMASBE.Models;
using MauiAppAMASBE.ViewModel;

namespace MauiAppAMASBE.Pages;

public partial class DadosUsuario : ContentPage
{
    CadastroViewModel viewModel = new CadastroViewModel();

    public DadosUsuario()
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (!App.VerificarLogin()) return;

        CadastroSaudeUsuario usuario = App.UsuarioLogado;
        viewModel.SexoSelecionado          = usuario.Sexo;
        viewModel.EstadoSelecionado        = usuario.EstadoUsuario;
        viewModel.TipoSanguineoSelecionado = usuario.TipoSanguineo;
        dtp_nascimento.Date = usuario.DataNascimento;
        viewModel.Usuario   = usuario;
    }

    private void Button_Atualizar(object sender, EventArgs e)
    {
        CadastroSaudeUsuario usuario = App.UsuarioLogado;
        viewModel.SexoSelecionado   = usuario.Sexo;
        viewModel.EstadoSelecionado = usuario.EstadoUsuario;
        CadastroCard.IsVisible = true;
        EmptyState.IsVisible   = false;
    }

    private async void Button_Salvar(object sender, EventArgs e)
    {
        try
        {
            // CORREÇÃO: verificação única de usuário nulo (estava duplicada)
            CadastroSaudeUsuario usuario = App.UsuarioLogado;
            if (usuario == null)
            {
                await DisplayAlert("Erro", "Usuário não está logado", "OK"); return;
            }

            // Validação do CPF
            if (!CpfValido(txtCpf.Text))
            {
                await DisplayAlert("Erro", "CPF inválido", "OK"); return;
            }

            if (dtp_nascimento.Date == null)
            {
                await DisplayAlert("Erro", "Data inválida", "OK"); return;
            }

            string tel = usuario.TelefoneUsuario ?? "";
            if (string.IsNullOrWhiteSpace(tel) || tel.Length < 10 || tel.Length > 11)
            {
                await DisplayAlert("Erro", "Telefone inválido (10 ou 11 dígitos sem formatação)", "OK"); return;
            }

            if (usuario.Peso <= 0 || usuario.Peso > 500)
            {
                await DisplayAlert("Erro", "Peso inválido (1–500 kg)", "OK"); return;
            }

            // CORREÇÃO: altura em centímetros (campo aceita 50–300 cm) — consistente com CalculosHelper
            if (usuario.Altura <= 20 || usuario.Altura > 300)
            {
                await DisplayAlert("Erro", "Altura inválida (21–300 cm)", "OK"); return;
            }

            if (string.IsNullOrEmpty(viewModel.SexoSelecionado))
            {
                await DisplayAlert("Erro", "Selecione o sexo", "OK"); return;
            }

            usuario.DataNascimento = dtp_nascimento.Date.Value;
            usuario.EstadoUsuario  = viewModel.EstadoSelecionado;
            usuario.Sexo           = viewModel.SexoSelecionado;
            usuario.TipoSanguineo  = viewModel.TipoSanguineoSelecionado;

            await App.Db.UpdateUsuario(usuario);
            await DisplayAlert("Sucesso!", "Dados atualizados", "OK");

            // CORREÇÃO: PopAsync, reset visual e OnAppearing dentro do try (só após sucesso)
            CadastroCard.IsVisible = false;
            EmptyState.IsVisible   = true;
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private static bool CpfValido(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf)) return false;
        cpf = cpf.Replace(".", "").Replace("-", "").Trim();
        if (cpf.Length != 11) return false;
        if (new string(cpf[0], 11) == cpf) return false;

        int[] m1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] m2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        string temp = cpf[..9];
        int soma = 0;
        for (int i = 0; i < 9; i++) soma += int.Parse(temp[i].ToString()) * m1[i];
        int resto = soma % 11; resto = resto < 2 ? 0 : 11 - resto;
        string digito = resto.ToString();
        temp += digito; soma = 0;
        for (int i = 0; i < 10; i++) soma += int.Parse(temp[i].ToString()) * m2[i];
        resto = soma % 11; resto = resto < 2 ? 0 : 11 - resto;
        digito += resto.ToString();
        return cpf.EndsWith(digito);
    }

    private async void Button_Voltar(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void ButtonVoltar(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
