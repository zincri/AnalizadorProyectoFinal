using System;
namespace Analizador.Helpers
{
    public class ZincriConverters
    {
        public ZincriConverters()
        {
        }
        public static int BinarioADecimal(string valor_a_extraer)
        {
            char[] array = valor_a_extraer.ToCharArray();
            // Invertido pues los valores van incrementandose de derecha a izquierda: 16-8-4-2-1
            Array.Reverse(array);
            int sum = 0;

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == '1')
                {
                    // Usamos la potencia de 2, según la posición
                    sum += (int)Math.Pow(2, i);
                }
            }
            return sum;
        }
        public static int HexadecimalADecimal(string valor_a_extraer)
        {
            return int.Parse(valor_a_extraer, System.Globalization.NumberStyles.HexNumber);
        }
        public static int NumeroZADecimal(string valor_a_extraer)
        {
            char[] array = valor_a_extraer.ToCharArray();
            /*PENDIENTE
            switch (indice) 
            {
                case  
            }
            */
            return 0;
        }
        public static int RomanoADecimal(string valor_a_extraer)
        {
            int valor = 0;
            switch (valor_a_extraer) 
            {
                case "I":
                    valor = 1;
                    break;
                case "II":
                    valor = 2;
                    break;
                case "III":
                    valor = 3;
                    break;
                case "IV":
                    valor = 4;
                    break;
                case "V":
                    valor = 5;
                    break;
                case "VI":
                    valor = 6;
                    break;
                case "VII":
                    valor = 7;
                    break;
                case "VIII":
                    valor = 8;
                    break;
                case "IX":
                    valor = 9;
                    break;
                case "X":
                    valor = 10;
                    break;
            }

            return valor;
        }
    }
}
