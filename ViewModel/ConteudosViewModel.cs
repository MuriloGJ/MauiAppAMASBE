using MauiAppAMASBE.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace MauiAppAMASBE.ViewModel
{
    public class ConteudosViewModel : INotifyPropertyChanged
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
        public List<string> Categorias { get; set; }

        private string categoriaSelecionada;
        public string CategoriaSelecionada
        {
            get => categoriaSelecionada;
            set
            {
                categoriaSelecionada = value;
                OnPropertyChanged();
            }
        }

       
        // ✔️ Construtor
        public ConteudosViewModel()
        {
            Categorias = new List<string>
            {
                "🍎 Alimentação",
                "🏃 Atividade Física",
                "🧠 Saúde Mental",
                "💧 Hidratação",
                "😴 Sono",
                "❤️ Bem-estar",
                "💊 Medicamentos",
                "🏥 SUS"

            };

          
        }
    }
}