using System;
using System.Collections.Generic;
using System.Text;

namespace MauiAppAMASBE.Helpers.CalculosHelpers
{
    public static class CalculosHelper
    {
        public static double CalcularIMC(double peso, double altura)
        {
            //double imc = CalculosHelper.CalcularIMC(usuario.Peso, usuario.Altura);// vai entrar na mainpage
            if (altura <= 0) return 0;

            return peso / (altura * altura);
        }
        public static string ClassificarIMC(double imc)
        {
            if (imc < 18.5) return "Abaixo do peso";
            if (imc < 25) return "Normal";
            if (imc < 30) return "Sobrepeso";
            return "Obesidade";
        }
        public static string ResultadoIMC(double peso, double altura)
        {
            double imc = CalcularIMC(peso, altura);
            return $"IMC: {imc:F2} - {ClassificarIMC(imc)}";
        }
    }
}
