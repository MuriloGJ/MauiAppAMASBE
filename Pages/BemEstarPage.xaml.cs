using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace MauiAppAMASBE.Pages
{
    public partial class BemEstarPage : ContentPage
    {
        public BemEstarPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Carrega texto salvo
            entradaTexto.Text = Preferences.Get("bem_estar_texto", "");
        }

        private async void OnSalvarClicked(object sender, EventArgs e)
        {
            Preferences.Set("bem_estar_texto", entradaTexto.Text);

            await DisplayAlert("Salvo 💾", "Seu registro foi guardado!", "OK");
        }

        private async void OnAguaClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Água 💧", "Beber água é essencial para sua saúde!", "OK");
        }

        private async void OnSonoClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Sono 😴", "Dormir bem melhora sua energia e foco!", "OK");
        }

        private async void OnExercicioClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Exercício 🚶", "Mexer o corpo faz muito bem pra mente!", "OK");
        }

        private async void OnTempoClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Autocuidado 🧘", "Tire um tempo só pra você hoje!", "OK");
        }
        private async void ButtonVoltar(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}