using MauiAppAMASBE.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiAppAMASBE.ViewModel
{
    public class HabitoViewModel : INotifyPropertyChanged
    {
        // 🔥 necessário pro Binding atualizar
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(
            [CallerMemberName] string nome = null)
        {
            PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(nome));
        }

        // 📌 Frequências
        public List<string> Frequencias { get; set; }

        private string frequenciaSelecionada;
        public string FrequenciaSelecionada
        {
            get => frequenciaSelecionada;
            set
            {
                frequenciaSelecionada = value;
                OnPropertyChanged();
            }
        }

        // 📌 Unidades
        public List<string> Unidades { get; set; }

        private string unidadeSelecionada;
        public string UnidadeSelecionada
        {
            get => unidadeSelecionada;
            set
            {
                unidadeSelecionada = value;
                OnPropertyChanged();
            }
        }

        // ✔️ Construtor
        public HabitoViewModel()
        {
            Frequencias = new List<string>
            {
                "Diário",
                "Semanal",
                "Quinzenal",
                "Mensal"
            };

            Unidades = new List<string>
            {
                "Litros",
                "Kilômetros",
                "Repetições",
                "Minutos",
                "Horas",
                "Exercícios"
            };
        }
    }
}