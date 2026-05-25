using System;
using System.Collections.Generic;
using System.Text;
using MauiAppAMASBE.Models;

namespace MauiAppAMASBE.Helpers.CalculosHelpers
{
    public class CalculosHelper
    {
        public static double CalcularIMC(double peso, double altura)
        {
            if (altura <= 0)
                return 0;

            altura = altura / 100.0;

            return peso / (altura * altura);
        }

        public static string ClassificarIMC(double imc)
        {
            if (imc < 18.5)
                return "Abaixo do peso";

            if (imc < 25)
                return "Normal";

            if (imc < 30)
                return "Sobrepeso";

            return "Obesidade";
        }

        public static string ResultadoIMC(double peso, double altura)
        {
            double imc = CalcularIMC(peso, altura);

            return $"IMC: {imc:F2} - {ClassificarIMC(imc)}";
        }
       
    }
}