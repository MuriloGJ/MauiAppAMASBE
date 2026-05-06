using MauiAppAMASBE.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiAppAMASBE.ViewModel
{
    //classe criada para usar o picker e instanciar os atributos
    public class HabitoViewModel
    {
        // 📌 Frequência
        public List<string> Frequencias { get; set; }
        public string FrequenciaSelecionada { get; set; }

        // 📌 Unidade
        public List<string> Unidades { get; set; }
        public string UnidadeSelecionada { get; set; }

        // ✔️ ÚNICO construtor
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
                "Kilometros",
                "Repetições",
                "Minutos",
                "Horas",
                "Exercícios"
            };
        }
    }
}