using Microsoft.Maui.Controls;
using System;

namespace MauiAppAMASBE.Pages
{
    public partial class HabitosPage : ContentPage
    {
        public HabitosPage()
        {
            InitializeComponent();
        }

        private void OnNovoHabitoClicked(object sender, EventArgs e)
        {
            CadastroCard.IsVisible = true;
            EmptyState.IsVisible = false;
        }

        private async void OnSalvarHabitoClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Sucesso", "Hábito cadastrado", "OK");

            // Limpa os campos
            NomeEntry.Text = "";
            TipoEntry.Text = "";
            MetaEntry.Text = "";

            CadastroCard.IsVisible = false;
            EmptyState.IsVisible = true;
        }

        private void OnCancelarClicked(object sender, EventArgs e)
        {
            CadastroCard.IsVisible = false;
            EmptyState.IsVisible = true;
        }
    }
}