using System;
using Microsoft.Maui.Controls;

namespace MauiAppAMASBE.Pages
{
    public partial class LembretesPage : ContentPage
    {
        public LembretesPage()
        {
            InitializeComponent();
        }

        private void OnNovoLembreteClicked(object sender, EventArgs e)
        {
            CadastroCard.IsVisible = true;
            EmptyState.IsVisible = false;
        }

        private async void OnSalvarLembreteClicked(object sender, EventArgs e)
        {
            if (TipoPicker.SelectedItem == null ||
                string.IsNullOrWhiteSpace(DescricaoEntry.Text) ||
                string.IsNullOrWhiteSpace(DataEntry.Text) ||
                string.IsNullOrWhiteSpace(HoraEntry.Text))
            {
                await DisplayAlert("Atenção", "Preencha os campos principais do lembrete.", "OK");
                return;
            }

            string tipo = TipoPicker.SelectedItem.ToString();

            await DisplayAlert("Sucesso", $"Lembrete de {tipo} cadastrado com sucesso!", "OK");

            TipoPicker.SelectedItem = null;
            DescricaoEntry.Text = "";
            DataEntry.Text = "";
            HoraEntry.Text = "";
            FrequenciaEntry.Text = "";

            CadastroCard.IsVisible = false;
            EmptyState.IsVisible = true;
        }

        private void OnCancelarClicked(object sender, EventArgs e)
        {
            TipoPicker.SelectedItem = null;
            DescricaoEntry.Text = "";
            DataEntry.Text = "";
            HoraEntry.Text = "";
            FrequenciaEntry.Text = "";

            CadastroCard.IsVisible = false;
            EmptyState.IsVisible = true;
        }
    }
}