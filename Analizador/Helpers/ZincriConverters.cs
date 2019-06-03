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

        public static string DecimalAHexadecimal(int valor_a_extraer)
        {
            return string.Format("{0:x}", valor_a_extraer);
        }
        public static int NumeroZADecimal(string valor_a_extraer)
        {
            int valor = 0;
            switch (valor_a_extraer)
            {
                case ".":
                    valor = 1;
                    break;
                case ":":
                    valor = 2;
                    break;
                case ";":
                    valor = 3;
                    break;
                case ".;":
                    valor = 4;
                    break;
                case ":;":
                    valor = 5;
                    break;
                case ";;":
                    valor = 6;
                    break;
                case ".;;":
                    valor = 7;
                    break;
                case ":;;":
                    valor = 8;
                    break;
                case ";;;":
                    valor = 9;
                    break;
                case "¡":
                    valor = 10;
                    break;
            }

            return valor;
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

        public static string DecimalARomano(int valor_a_convertir)
        {
            string valor = string.Empty;
            switch (valor_a_convertir)
            {
                case 1:
                    valor = "I";
                    break;
                case 2:
                    valor = "II";
                    break;
                case 3:
                    valor = "III";
                    break;
                case 4:
                    valor = "IV";
                    break;
                case 5:
                    valor = "V";
                    break;
                case 6:
                    valor = "VI";
                    break;
                case 7:
                    valor = "VII";
                    break;
                case 8:
                    valor = "VIII";
                    break;
                case 9:
                    valor = "IX";
                    break;
                case 10:
                    valor = "X";
                    break;
            }

            return valor;
        }
    }
}
