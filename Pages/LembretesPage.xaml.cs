using MauiAppAMASBE.Models;
using MauiAppAMASBE.ViewModel;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MauiAppAMASBE.Pages
{
    public partial class LembretesPage : ContentPage
    {
        ObservableCollection<Lembrete> lista = new ObservableCollection<Lembrete>();

        LembreteViewModel viewModel = new LembreteViewModel();
        Lembrete LembreteSelecionado;
        public LembretesPage()
        {
            InitializeComponent();
            BindingContext = viewModel;

            lst_lembrete.ItemsSource = lista;
        }
        protected async override void OnAppearing()
        {
            CadastroSaudeUsuario usuario = App.UsuarioLogado;

            try
            {
                lista.Clear();

                List<Lembrete> tmp = await App.Db.GetLembretePorUsuario(usuario.IdCadastro);
                tmp.ForEach(i => lista.Add(i));
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Ops", ex.Message, "OK");
            }
        }

        private async void OnNovoLembreteClicked(object sender, EventArgs e)
        {
            CadastroSaudeUsuario usuario = App.UsuarioLogado;
            if (usuario == null)
            {
                await DisplayAlert("Erro", "Usuário não está logado", "OK");
                return;
            }


            CadastroCard.IsVisible = true;
            EmptyState.IsVisible = false;
        }

        private async void OnSalvarLembreteClicked(object sender, EventArgs e)
        {

            try
            {
                CadastroSaudeUsuario usuario = App.UsuarioLogado;

                if (usuario == null)
                {
                    await DisplayAlert("Erro", "Usuário não está logado", "OK");
                    return;
                }
                if (dtp_lembrete.Date == null)
                {
                    await DisplayAlert("Erro", "Selecione uma data", "OK");
                    return;
                }

                Lembrete lembrete = new Lembrete
                {
                    TituloLembrete = TituloEntry.Text,
                    DataLembrete = dtp_lembrete.Date.Value,
                    HorarioLembrete = HoraLembrete.Time ?? TimeSpan.Zero,

                    TipoLembrete = viewModel.TipoLSelecionada,
                    FrequenciaLembrete = viewModel.FrequenciaLSelecionada,

                    IdCadastro = usuario.IdCadastro,


                };

                await App.Db.InsertLembrete(lembrete);

                await DisplayAlert("Sucesso!", "Lembrete Inserido", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }

            TituloEntry.Text = "";


            CadastroCard.IsVisible = false;
            EmptyState.IsVisible = true;
            OnAppearing();
        }

        private void OnCancelarClicked(object sender, EventArgs e)
        {


            CadastroCard.IsVisible = false;
            EmptyState.IsVisible = true;
        }

        private async void lst_lembrete_Refreshing(object sender, EventArgs e)
        {
            CadastroSaudeUsuario usuario = App.UsuarioLogado;
            try
            {
                lista.Clear();

                List<Lembrete> tmp = await App.Db.GetLembrete();
                tmp.ForEach(i => lista.Add(i));
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Ops", ex.Message, "OK");
            }
            finally
            {
                lst_lembrete.IsRefreshing = false;
            }
        }

        private void lst_lembrete_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            CadastroSaudeUsuario usuario = App.UsuarioLogado;
            try
            {
                Lembrete l = e.SelectedItem as Lembrete;

                Navigation.PushAsync(new Pages.LembretesPage
                { BindingContext = l, });


            }
            catch (Exception ex)
            {
                DisplayAlertAsync("Ops", ex.Message, "OK");
            }
        }

        private async void MenuItem_RemoverLembrete(object sender, EventArgs e)
        {
            CadastroSaudeUsuario usuario = App.UsuarioLogado;
            try
            {
                MenuItem item = sender as MenuItem;

                Lembrete l = item.BindingContext as Lembrete;

                bool confirma = await DisplayAlertAsync("Tem Certeza?", $"Remover {l.TituloLembrete}", "Sim", "Não");

                if (confirma)
                {
                    await App.Db.DeleteLembrete(l.IdLembrete);
                    lista.Remove(l);
                    await DisplayAlertAsync("Sucesso!", "Registro Apagado", "OK");

                }
                else
                {
                    await DisplayAlertAsync("Falha!", "Registro Mantido", "OK");
                }

            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Ops", ex.Message, "OK");
            }

        }



        private async void MenuItem_EditarLembrete(object sender, EventArgs e)
        {
            BindingContext = null;
            BindingContext = viewModel;
         
            CadastroSaudeUsuario usuario = App.UsuarioLogado;

            if (usuario == null)
            {
                await DisplayAlert("Erro", "Usuário não está logado", "OK");
                return;
            }

            var menuItem = sender as MenuItem;

            LembreteSelecionado = menuItem.BindingContext as Lembrete;

            // PREENCHE CAMPOS
            TituloEntryEdit.Text = LembreteSelecionado.TituloLembrete;

            Edit_timeHorario.Time = LembreteSelecionado.HorarioLembrete;

            // PICKERS
            viewModel.FrequenciaLSelecionada =
                LembreteSelecionado.FrequenciaLembrete;

            viewModel.TipoLSelecionada =
                LembreteSelecionado.TipoLembrete;

            EditCard.IsVisible = true;

            EmptyState.IsVisible = false;
        }
        private async void Button_EditarLembrete(object sender, EventArgs e)
        {
            CadastroSaudeUsuario usuario = App.UsuarioLogado;
            if (usuario == null)
            {
                await DisplayAlert("Erro", "Usuário não está logado", "OK");
                return;

            }

            LembreteSelecionado.TituloLembrete = TituloEntryEdit.Text;
            LembreteSelecionado.TipoLembrete = viewModel.TipoLSelecionada;
            LembreteSelecionado.HorarioLembrete = Edit_timeHorario.Time ?? TimeSpan.Zero;
            LembreteSelecionado.HorarioLembrete = Edit_timeHorario.Time ?? TimeSpan.Zero;
            LembreteSelecionado.FrequenciaLembrete = viewModel.FrequenciaLSelecionada;

            await App.Db.UpdateLembrete(LembreteSelecionado);

            EditCard.IsVisible = false;

            await DisplayAlert("Sucesso", "Lembrete atualizado!", "OK");

        }

        private void Button_Cancelar(object sender, EventArgs e)
        {
            EditCard.IsVisible = false;
            EmptyState.IsVisible = true;
        }
        private async void ButtonVoltar(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
    }
