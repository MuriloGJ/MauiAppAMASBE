using System;
using System.Collections.Generic;
using System.Text;

namespace MauiAppAMASBE.ViewModel
{
    //classe criada para usar o picker e instanciar os atributos
    public class LembreteViewModel
    {
        // 📌 Frequência
        public List<string> TiposL { get; set; }
        public string TipoLSelecionada { get; set; }

        // 📌 Unidade
        public List<string> FrequenciaL { get; set; }
        public string FrequenciaLSelecionada { get; set; }

        // ✔️ ÚNICO construtor
        public LembreteViewModel()
        {
            TiposL = new List<string>
            {
                    "Saúde",
                    "Alimentação",
                     "Exercício",
                    "Bem-estar",
                     "Rotina"
                   
            };

            FrequenciaL = new List<string>
            {
                "Uma vez",
                "Diario",
                "Semanal",
                "Mensal",
                "Anual"
                
            };
        }
    }
}