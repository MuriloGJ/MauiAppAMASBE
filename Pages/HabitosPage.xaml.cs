using MauiAppAMASBE.Models;
using MauiAppAMASBE.ViewModel;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace MauiAppAMASBE.Pages
{


    
    public partial class HabitosPage : ContentPage
    {
        ObservableCollection<Habito> lista = new ObservableCollection<Habito>();

        HabitoViewModel viewModel = new HabitoViewModel();
        Habito habitoSelecionado;

        public HabitosPage()
        {
            InitializeComponent();
            BindingContext = viewModel;

            lst_habito.ItemsSource = lista;

        }
        protected async override void OnAppearing()
        {


            try
            {
                lista.Clear();

                List<Habito> tmp = await App.Db.GetHabitos();
                tmp.ForEach(i => lista.Add(i));
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Ops", ex.Message, "OK");
            }
        }

        private void OnNovoHabitoClicked(object sender, EventArgs e)
        {


            CadastroCard.IsVisible = true;
            EmptyState.IsVisible = false;
        }

        private async void OnSalvarHabitoClicked(object sender, EventArgs e)
        {
           
            try
            {
                double meta;

                if (!double.TryParse(
                    MetaEntry.Text,
                    NumberStyles.Any,
                    CultureInfo.CurrentCulture,
                    out meta))
                {
                    await DisplayAlert("Erro", "Digite uma meta válida", "OK");
                    return;
                }

                Habito habito = new Habito
                {
                    NomeHabito = NomeEntry.Text,
                    TipoHabito = TipoEntry.Text,
                    DescricaoHabito = DescricaoEntry.Text,
                    HorarioHabito = timeHorario.Time ?? TimeSpan.Zero,
                    FrequenciaHabito = viewModel.FrequenciaSelecionada,

                    
                    MetaValor = meta,
                    MetaUnidade = viewModel.UnidadeSelecionada,
                };

                await App.Db.InsertHabito(habito);

                await DisplayAlert("Sucesso!", "Hábito Inserido", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }

            // limpar campos
            NomeEntry.Text = "";
            TipoEntry.Text = "";
            DescricaoEntry.Text = "";
            MetaEntry.Text = "";

            CadastroCard.IsVisible = false;
            EmptyState.IsVisible = true;

            // 🔥 importante pra atualizar lista
            OnAppearing();
        }

        private void OnCancelarClicked(object sender, EventArgs e)
        {
            CadastroCard.IsVisible = false;
            EmptyState.IsVisible = true;
        }
        private async void lst_habito_Refreshing(object sender, EventArgs e)
        {
            try
            {
                lista.Clear();

                List<Habito> tmp = await App.Db.GetHabitos();
                tmp.ForEach(i => lista.Add(i));
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Ops", ex.Message, "OK");
            }
            finally
            {
                lst_habito.IsRefreshing = false;
            }
        }
        private void lst_habito_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                Habito h = e.SelectedItem as Habito;

                Navigation.PushAsync(new Pages.HabitosPage
                { BindingContext = h, });


            }
            catch (Exception ex)
            {
                DisplayAlertAsync("Ops", ex.Message, "OK");
            }
        }
        private async void MenuItem_Remover_Habito(object sender, EventArgs e)
        {
            try
            {
                MenuItem item = sender as MenuItem;

                Habito h = item.BindingContext as Habito;

                bool confirma = await DisplayAlertAsync("Tem Certeza?", $"Remover {h.NomeHabito}", "Sim", "Não");

                if (confirma)
                {
                    await App.Db.DeleteHabito(h.IdHabito);
                    lista.Remove(h);
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

        private void MenuItem_Editar_Habito(object sender, EventArgs e)
        {
            BindingContext = null;
            BindingContext = viewModel;
            var menuItem = sender as MenuItem;
            habitoSelecionado = menuItem.BindingContext as Habito;

            EditCard.IsVisible = true;

            // Preenche campos
            
            Edit_MetaEntry.Text = habitoSelecionado.MetaValor.ToString();
            Edit_timeHorario.Time = habitoSelecionado.HorarioHabito;

            viewModel.FrequenciaSelecionada = habitoSelecionado.FrequenciaHabito;
        }
        private async void Button_Editar_Habito(object sender, EventArgs e)
        {
            if (habitoSelecionado == null)
                return;

            double meta;

            if (!double.TryParse(Edit_MetaEntry.Text, out meta))
            {
                await DisplayAlert("Erro", "Meta inválida", "OK");
                return;
            }

            habitoSelecionado.MetaValor = meta;
            habitoSelecionado.HorarioHabito = Edit_timeHorario.Time ?? TimeSpan.Zero;
            habitoSelecionado.FrequenciaHabito = viewModel.FrequenciaSelecionada;

            await App.Db.UpdateHabito(habitoSelecionado);

            EditCard.IsVisible = false;

            await DisplayAlert("Sucesso", "Hábito atualizado!", "OK");
        
    }

        private void Button_Cancelar_edicao(object sender, EventArgs e)
        {
            EditCard.IsVisible = false;
            EmptyState.IsVisible = true;
        }

        private async void Stepper_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            var stepper = sender as Stepper;
            var habito = stepper?.BindingContext as Habito;

            if (habito == null)
                return;

            // Atualiza o valor atual
            habito.ValorAtual = e.NewValue;

          
            await App.Db.UpdateHabito(habito);
        }
    }
}