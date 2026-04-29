using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using MauiAppAMASBE.ViewModel;
using System.ComponentModel;

namespace MauiAppAMASBE.Models
{
    public class Habito : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        [PrimaryKey, AutoIncrement]
        public int IdHabito { get; set; }

        [Indexed]
        public int IdCadastro { get; set; }

        public string NomeHabito { get; set; }
        public string TipoHabito { get; set; }
        public string DescricaoHabito { get; set; }

        public double MetaValor { get; set; }
        public string MetaUnidade { get; set; }
        public string FrequenciaHabito { get; set; }

        public TimeSpan HorarioHabito { get; set; }

        private double valorAtual;
        public double ValorAtual
        {
            get => valorAtual;
            set
            {
                valorAtual = value;
                OnPropertyChanged(nameof(ValorAtual));
                OnPropertyChanged(nameof(Progresso));
            }
        }

        public double Progresso => MetaValor == 0 ? 0 : (ValorAtual / MetaValor) * 100;
        protected void OnPropertyChanged(string nome)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nome));
        }
    }

    //public string StatusHabito { get; set; } = "pendente";


}

