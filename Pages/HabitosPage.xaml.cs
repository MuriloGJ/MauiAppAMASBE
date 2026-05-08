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
            CadastroSaudeUsuario usuario = App.UsuarioLogado;

            try
            {
                lista.Clear();

                List<Habito> tmp = await App.Db.GetHabitosPorUsuario(usuario.IdCadastro);
                tmp.ForEach(i => lista.Add(i));
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Ops", ex.Message, "OK");
            }
        }

        private void OnNovoHabitoClicked(object sender, EventArgs e)
        {
            CadastroSaudeUsuario usuario = App.UsuarioLogado;

            CadastroCard.IsVisible = true;
            EmptyState.IsVisible = false;
        }

        private async void OnSalvarHabitoClicked(object sender, EventArgs e)
        {

           
            try
            {
                CadastroSaudeUsuario usuario = App.UsuarioLogado;

                if (usuario == null)
                {
                    await DisplayAlert("Erro", "Usuário não está logado", "OK");
                    return;
                }




                #region validação_das_entradas
                // 🔹 NOME
                var lista = await App.Db.GetHabitosPorUsuario(usuario.IdCadastro);

                if (lista.Any(h => h.NomeHabito == NomeEntry.Text))
                {
                    await DisplayAlert("Erro", "Você já tem um hábito com esse nome", "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(NomeEntry.Text))
                {
                    await DisplayAlert("Erro", "Nome do hábito é obrigatório", "OK");
                    return;
                }
                //Meta
                double meta;

                if (!double.TryParse(MetaEntry.Text, out meta) || meta <= 0)
                {
                    await DisplayAlert("Erro", "Meta inválida", "OK");
                    return;
                }

                // 🔹 FREQUÊNCIA
                if (string.IsNullOrEmpty(viewModel.FrequenciaSelecionada))
                {
                    await DisplayAlert("Erro", "Selecione a frequência", "OK");
                    return;
                }

                // 🔹 UNIDADE
                if (string.IsNullOrEmpty(viewModel.UnidadeSelecionada))
                {
                    await DisplayAlert("Erro", "Selecione a unidade", "OK");
                    return;
                }

                // 🔹 HORÁRIO (opcional mas seguro)
                var horario = timeHorario.Time ?? TimeSpan.Zero;
#endregion
                Habito habito = new Habito
                {
                    NomeHabito = NomeEntry.Text,
                    
                    TipoHabito = TipoEntry.Text,
                    DescricaoHabito = DescricaoEntry.Text,
                    HorarioHabito = timeHorario.Time ?? TimeSpan.Zero,
                    FrequenciaHabito = viewModel.FrequenciaSelecionada,

                    IdCadastro = usuario.IdCadastro,
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
            CadastroSaudeUsuario usuario = App.UsuarioLogado;
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
            CadastroSaudeUsuario usuario = App.UsuarioLogado;
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
            CadastroSaudeUsuario usuario = App.UsuarioLogado;
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
            Edit_NomeEntry.Text = habitoSelecionado.NomeHabito;
            Edit_DescricaoEntry.Text = habitoSelecionado.DescricaoHabito;
            Edit_MetaEntry.Text = habitoSelecionado.MetaValor.ToString();
            Edit_timeHorario.Time = habitoSelecionado.HorarioHabito;

            viewModel.FrequenciaSelecionada = habitoSelecionado.FrequenciaHabito;
        }
        private async void Button_Editar_Habito(object sender, EventArgs e)
        {
            base.OnAppearing();
            CadastroSaudeUsuario usuario = App.UsuarioLogado;
            if (usuario == null)
            {
                await DisplayAlert("Erro", "Usuário não está logado", "OK");
                return;
                
            }

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