namespace MauiAppAMASBE.Pages;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
    }

    private async Task MostrarAvisoAsync(string nomeFuncionalidade)
    {
        await DisplayAlert(
            "Em desenvolvimento",
            $"A funcionalidade \"{nomeFuncionalidade}\" ainda está em desenvolvimento.",
            "OK");
    }

    private async void OnEspecialistasTapped(object sender, TappedEventArgs e)
    {
        await MostrarAvisoAsync("Especialistas");
    }

    private async void OnVacinasTapped(object sender, TappedEventArgs e)
    {
        await MostrarAvisoAsync("Vacinas");
    }

    private async void OnExamesTapped(object sender, TappedEventArgs e)
    {
        await MostrarAvisoAsync("Exames");
    }

    private async void OnMedicamentosTapped(object sender, TappedEventArgs e)
    {
        await MostrarAvisoAsync("Medicamentos");
    }

    private async void OnHabitosTapped(object sender, TappedEventArgs e)
    {
        await MostrarAvisoAsync("Hábitos");
    }

    private async void OnUbsTapped(object sender, TappedEventArgs e)
    {
        await MostrarAvisoAsync("UBS");
    }

    private async void OnConteudosTapped(object sender, TappedEventArgs e)
    {
        await MostrarAvisoAsync("Conteúdos");
    }

    private async void OnLembretesTapped(object sender, TappedEventArgs e)
    {
        await MostrarAvisoAsync("Lembretes");
    }

    private async void OnBemEstarTapped(object sender, TappedEventArgs e)
    {
        await MostrarAvisoAsync("Bem-estar");
    }
}