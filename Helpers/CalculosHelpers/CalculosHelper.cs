using MauiAppAMASBE.Models;

namespace MauiAppAMASBE.Helpers.CalculosHelpers
{
    /// <summary>
    /// Helpers de cálculos de saúde.
    /// IMPORTANTE: altura deve ser informada em centímetros (ex: 175).
    /// A divisão por 100 é feita internamente.
    /// </summary>
    public class CalculosHelper
    {
        /// <param name="peso">Peso em kg</param>
        /// <param name="altura">Altura em centímetros (ex: 175)</param>
        public static double CalcularIMC(double peso, double altura)
        {
            if (altura <= 0) return 0;
            double alturaM = altura / 100.0;
            return peso / (alturaM * alturaM);
        }

        public static string ClassificarIMC(double imc)
        {
            if (imc < 18.5) return "Abaixo do peso";
            if (imc < 25)   return "Normal";
            if (imc < 30)   return "Sobrepeso";
            if (imc < 35)   return "Obesidade grau I";
            if (imc < 40)   return "Obesidade grau II";
            return "Obesidade grau III";
        }

        public static string ResultadoIMC(double peso, double altura)
        {
            double imc = CalcularIMC(peso, altura);
            return $"IMC: {imc:F2} — {ClassificarIMC(imc)}";
        }

        public static int CalcularIdade(DateTime dataNascimento)
        {
            var hoje = DateTime.Today;
            int idade = hoje.Year - dataNascimento.Year;
            if (dataNascimento.Date > hoje.AddYears(-idade)) idade--;
            return idade;
        }
    }
}
