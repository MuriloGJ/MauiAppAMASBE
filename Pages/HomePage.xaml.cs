using System;
using Microsoft.Maui.Controls;

namespace MauiAppAMASBE.Pages
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private async void OnHabitosTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new HabitosPage());
        }

        private async void OnLembretesTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new LembretesPage());
        }

        private async void OnConteudosTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new ConteudosPage());
        }

        private async void OnFaqTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new FAQPage());
        }

        private async void OnUbsTapped(object sender, TappedEventArgs e)
        {
            await DisplayAlert("Em desenvolvimento", "A tela de UBS será implementada nas próximas etapas.", "OK");
        }

        private async void OnBemEstarTapped(object sender, TappedEventArgs e)
        {
            await DisplayAlert("Em desenvolvimento", "A tela de bem-estar será implementada nas próximas etapas.", "OK");
        }
    }
}