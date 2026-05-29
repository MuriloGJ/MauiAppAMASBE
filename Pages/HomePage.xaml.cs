using MauiAppAMASBE.Helpers.CalculosHelpers;
using MauiAppAMASBE.Models;
using Microsoft.Maui.Controls;
using System;

namespace MauiAppAMASBE.Pages
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
            BindingContext = this;
           
        }
        //teste para habilitar frame apenas para Administrador
        protected override void OnAppearing()
        {
            base.OnAppearing();

            CardGerenciador.IsVisible =
                App.UsuarioLogado?.TipoUsuario == "Administrador";
        }
        public string ResultadoIMCTexto
        {
            get
            {
                CadastroSaudeUsuario usuario = App.UsuarioLogado;

                return CalculosHelper.ResultadoIMC(
                    usuario.Peso,
                    usuario.Altura
                );
            }
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

        // ✅ Maps UBS — navega para a página real
        private async void OnUbsTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new MapsUBSPage());
        }

        private async void OnBemEstarTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new BemEstarPage());
        }

        private async void OnDadosUsuarioTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new DadosUsuario());
        }
        private async void OnNotificacaoTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new NotificacaoPage());
        }
        private async void OnGerenciadorTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new GerenciadorPage());
        }

        private void Button_Logout(object sender, EventArgs e)
        {
            // limpa usuário logado
            App.UsuarioLogado = null;

            // limpa dados salvos
            Preferences.Remove("login");
            Preferences.Remove("senha");

            // volta pro login
            Application.Current.MainPage =
                new NavigationPage(new LoginPage());
        }

      
    }
}
