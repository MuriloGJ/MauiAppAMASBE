using MauiAppAMASBE.Helpers.HelperNotificacao;
using MauiAppAMASBE.Models;

namespace MauiAppAMASBE.Pages;

public partial class NotificacaoPage : ContentPage
{
	public NotificacaoPage()
	{
		InitializeComponent();
        CarregarNotificacoes();

    }
    private List<Notificacao> _listaBase = new();
    protected override void OnAppearing()
    {
        base.OnAppearing();
        CarregarNotificacoes();
    }

    private async void CarregarNotificacoes()
    {
        try
        {
            _listaBase = await App.Db.GetNotificacoes();
            ListaNotificacoes.ItemsSource = _listaBase;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }

    private async void ListaNotificacoes_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null)
            return;

        var notificacao = (Notificacao)e.SelectedItem;

        // Evita múltipla execução
        ((ListView)sender).SelectedItem = null;

        try
        {
            // Se ainda não estiver lida, marca como lida
            if (notificacao.StatusNotificacao != "lida")
            {
                await NotificacaoHelper.MarcarComoLida(notificacao);
            }

            // (Opcional) mostrar detalhe simples
            await DisplayAlert(
                "Notificação",
                notificacao.TituloNotificacao,
                "OK");

            // Recarrega lista atualizada
            CarregarNotificacoes();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }

    }
    private void BtnTodas_Clicked(object sender, EventArgs e)
    {
        ListaNotificacoes.ItemsSource = _listaBase;
    }
    private void BtnPendentes_Clicked(object sender, EventArgs e)
    {
        ListaNotificacoes.ItemsSource =
            _listaBase.Where(n => n.StatusNotificacao == "pendente").ToList();
    }
    private void BtnLidas_Clicked(object sender, EventArgs e)
    {
        ListaNotificacoes.ItemsSource =
            _listaBase.Where(n => n.StatusNotificacao == "lida").ToList();
    }
    private async void ButtonVoltar(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}