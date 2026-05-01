using MauiAppAMASBE.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MauiAppAMASBE.ViewModel
{
    public class CadastroViewModel : INotifyPropertyChanged
    {
        private CadastroSaudeUsuario usuario;
        public CadastroSaudeUsuario Usuario
        {
            get => usuario;
            set
            {
                usuario = value;
                OnPropertyChanged(nameof(Usuario));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        // 🔹 LISTA DE SEXO
        public List<string> Sexos { get; set; }

        private string sexoSelecionado;
        public string SexoSelecionado
        {
            get => sexoSelecionado;
            set
            {
                sexoSelecionado = value;
                OnPropertyChanged(nameof(SexoSelecionado));
            }
        }

        // 🔹 LISTA DE TIPO SANGUÍNEO
        public List<string> TiposSanguineos { get; set; }

        private string tipoSanguineoSelecionado;
        public string TipoSanguineoSelecionado
        {
            get => tipoSanguineoSelecionado;
            set
            {
                tipoSanguineoSelecionado = value;
                OnPropertyChanged(nameof(TipoSanguineoSelecionado));
            }
        }

        // 🔧 CONSTRUTOR
        public CadastroViewModel()
        {
            Sexos = new List<string>
            {
                "Masculino",
                "Feminino",
                "Outro",
                "Não Informar"
            };

            TiposSanguineos = new List<string>
            {
                "A+","A-",
                "B+","B-",
                "AB+","AB-",
                "O+","O-"
            };
            Estados = new List<string>
{
                        "AC", // Acre
                        "AL", // Alagoas
                        "AP", // Amapá
                        "AM", // Amazonas
                        "BA", // Bahia
                        "CE", // Ceará
                        "DF", // Distrito Federal
                        "ES", // Espírito Santo
                        "GO", // Goiás
                        "MA", // Maranhão
                        "MT", // Mato Grosso
                        "MS", // Mato Grosso do Sul
                        "MG", // Minas Gerais
                        "PA", // Pará
                        "PB", // Paraíba
                        "PR", // Paraná
                        "PE", // Pernambuco
                        "PI", // Piauí
                        "RJ", // Rio de Janeiro
                        "RN", // Rio Grande do Norte
                        "RS", // Rio Grande do Sul
                        "RO", // Rondônia
                        "RR", // Roraima
                        "SC", // Santa Catarina
                        "SP", // São Paulo
                        "SE", // Sergipe
                        "TO"  // Tocantins
                    };
        }
        private DateTime dataNascimento;
        public DateTime DataNascimento
        {
            get => dataNascimento;
            set
            {
                dataNascimento = value;
                OnPropertyChanged(nameof(DataNascimento));
            }
        }
        // 🔹 LISTA DE ESTADOS
        public List<string> Estados { get; set; }

        private string estadoSelecionado;
        public string EstadoSelecionado
        {
            get => estadoSelecionado;
            set
            {
                estadoSelecionado = value;
                OnPropertyChanged(nameof(EstadoSelecionado));
            }
        }

        // 🔧 Método para notificar mudança
        void OnPropertyChanged(string nome)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nome));
        }
        
    }
}