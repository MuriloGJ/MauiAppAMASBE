using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System;

namespace MauiAppAMASBE.Pages
{
    public partial class ConteudosPage : ContentPage
    {
        public ConteudosPage()
        {
            InitializeComponent();
        }

        private async void OnAbrirArtigoAlimentacaoClicked(object sender, EventArgs e)
        {
            await Launcher.OpenAsync("https://www.scielosp.org/article/csc/2025.v30n2/e17962024/pt/");
        }

        private async void OnAbrirVideoAtividadeClicked(object sender, EventArgs e)
        {
            await Launcher.OpenAsync("https://www.youtube.com/watch?v=i7QwQPiAa0A");
        }

        private async void OnAbrirArtigoHidratacaoClicked(object sender, EventArgs e)
        {
            await Launcher.OpenAsync("https://viverbem.unimedbh.com.br/prevencao-e-controle/hidratacao/");
        }

        private async void OnAbrirVideoSaudeMentalClicked(object sender, EventArgs e)
        {
            await Launcher.OpenAsync("https://www.youtube.com/watch?v=t354E2Ot9eA");
        }
        private async void ButtonVoltar(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}