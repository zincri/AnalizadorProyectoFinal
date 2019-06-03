namespace Analizador.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;
    using System.Linq;
    using Analizador.Helpers;

    public class StartViewModel: INotifyPropertyChanged
    {
        #region Variables
        int[,] Matriz = new int[42, 21];//[5, 5];
        char[] array;
        int Indice = 0;
        int Estado = 0;
        int C = 0;
        int Contador = 0;
        int Nuevo_estado = 0;
        bool flagHex=false;
        int ultimo=1017;
        #endregion

        #region Properties
        private String cadena;
        public String Cadena
        {
            get
            {
                return cadena;
            }
            set
            {
                cadena = value;
                OnPropertyChanged();
            }
        }

        private String numeros;
        public String Numeros
        {
            get
            {
                return numeros;
            }
            set
            {
                numeros = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        public ICommand SetCadenaCommand
        {
            get
            {
                return new RelayCommand(SetCadena);
            }
        }

        #endregion

        #region Constructor
        public StartViewModel()
        {
            this.Numeros = "Sin numeros";
            InicializarMatriz();


        }

        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Este metodo se ejecuta cuando se pulsa el boton "Enviar Cadena"
        /// </summary>
        private async void SetCadena()
        {
            if (string.IsNullOrEmpty(this.Cadena))
            {
                await App.Current.MainPage.DisplayAlert(
                "Error",
                "El campo está vacio.",
                "Ok");
            }
            else
            {
                bool flag = false;
                int Token = 0;

                this.Numeros = String.Empty;
                array = (this.Cadena + " #").ToCharArray();
                List<int> listTokens = new List<int>();



                //me falto el indice correrlo a 0 otra vez
                Indice = 0;
                while (Token != 2000)
                {
                    Token = AnalizadorLexico();
                    this.Numeros = this.Numeros + "[" + Token.ToString() + "]";
                    if (Token == -1) { flag = true; break; }
                    //IF TOKEN ES 2000 QUE NO AGREGUE EL TOKEN
                    listTokens.Add(Token);

                }
                if (flag)
                {
                    this.Numeros = "Cadena no aceptada en el caracter: " + (Indice + 1);
                }
                else {
                    int band = verficaOrden(listTokens);
                    if (band == 1) 
                    {
                        //this.Numeros = "Cadena exitosa";
                        AnalizadorSemantico(listTokens);
                        
                    }
                    else { this.Numeros = "Cadena erronea"; }
                }

            }


        }
        /// <summary>
        /// Este metodo es el analizador semantico
        /// </summary>
        /// <param name="listTokens">List tokens.</param>
        private void AnalizadorSemantico(List<int> listTokens) {

            this.Numeros = String.Empty;
            List<char> listaTipos =  new List<char>();
            List<String> listaValores_sin_convertir;
            List<int> listaValores = new List<int>();
            foreach (var item in listTokens)
            {

                this.Numeros = Numeros + buscarTipo(item);
                if(buscarTipo(item)!=(char)00)
                    listaTipos.Add(buscarTipo(item));
            }
            string[] ValoresSeparados = Cadena.Split(' ');
            listaValores_sin_convertir = ValoresSeparados.OfType<String>().ToList();
            listaValores_sin_convertir.RemoveAt(listaValores_sin_convertir.Count-1);

            for (int i = 0; i < listaValores_sin_convertir.Count; i++)
            {
                listaValores.Add(ExtraerValor(listaTipos.ElementAt(i),listaValores_sin_convertir.ElementAt(i)));
            }

            List<int> pila_operandos = new List<int>();
            List<int> pila_operadores = new List<int>();
            bool bandera_operandos_operador=true;
            bool bandera_error_matematico = false;
            int operador_temporal;

            for (int i = 0; i < listaValores.Count; i++)
            {
                if (bandera_operandos_operador) 
                {
                    pila_operandos.Add(listaValores.ElementAt(i));
                    bandera_operandos_operador = false;
                }
                else 
                {
                    if (pila_operadores.Count == 0)
                        pila_operadores.Add(listaValores.ElementAt(i));
                    else
                    {
                        //AQUI
                        int prioridad_pila_operadores = pila_operadores.Last() == 1 || pila_operadores.Last() == 2 ? 1 : 2;
                        int prioridad_indice = listaValores.ElementAt(i) == 1 || listaValores.ElementAt(i) == 2 ? 1 : 2;

                        if (prioridad_pila_operadores < prioridad_indice)
                        {
                            pila_operadores.Add(listaValores.ElementAt(i));
                        }
                        else if (prioridad_pila_operadores > prioridad_indice) 
                        {
                            operador_temporal = pila_operadores.Last();
                            pila_operadores.RemoveAt(pila_operadores.Count - 1);
                            int Top = pila_operandos.Last();
                            pila_operandos.RemoveAt(pila_operandos.Count-1);
                            int TopMenos1 = pila_operandos.Last();
                            pila_operandos.RemoveAt(pila_operandos.Count - 1);
                            int resultado = RealizarOperacionTipoCuadruplo(TopMenos1,Top,operador_temporal);
                            if (resultado == -1) { bandera_error_matematico = true; break; }
                            pila_operandos.Add(resultado);
                            pila_operadores.Add(listaValores.ElementAt(i));
                        }
                        else if (prioridad_pila_operadores == prioridad_indice)
                        {
                            operador_temporal = pila_operadores.Last();
                            pila_operadores.RemoveAt(pila_operadores.Count - 1);
                            int Top = pila_operandos.Last();
                            pila_operandos.RemoveAt(pila_operandos.Count - 1);
                            int TopMenos1 = pila_operandos.Last();
                            pila_operandos.RemoveAt(pila_operandos.Count - 1);
                            int resultado = RealizarOperacionTipoCuadruplo(TopMenos1, Top, operador_temporal);
                            if (resultado == -1) { bandera_error_matematico = true; break; }
                            pila_operandos.Add(resultado);
                            pila_operadores.Add(listaValores.ElementAt(i));
                        }

                    }
                    bandera_operandos_operador = true;
                }


            }

            while (pila_operadores.Count != 0)
            {
                operador_temporal = pila_operadores.Last();
                pila_operadores.RemoveAt(pila_operadores.Count - 1);
                int Top = pila_operandos.Last();
                pila_operandos.RemoveAt(pila_operandos.Count - 1);
                int TopMenos1 = pila_operandos.Last();
                pila_operandos.RemoveAt(pila_operandos.Count - 1);
                int resultado = RealizarOperacionTipoCuadruplo(TopMenos1, Top, operador_temporal);
                if (resultado == -1) { bandera_error_matematico=true; break;}

                pila_operandos.Add(resultado);
            }
            if (bandera_error_matematico) 
            {
                App.Current.MainPage.DisplayAlert("Error","Error matematico, divicion entre 0","Ok");
                this.Numeros = "Error";//String.Empty;
                return;
            }



            /*__________________________________________________*/


            int resultado_que_esta_en_top = pila_operandos.Last();

            if (ultimo == 1019)
            {
                if (resultado_que_esta_en_top > 10 || resultado_que_esta_en_top <=0)
                {
                    this.Numeros = "El valor exede la represacion Romana.";
                }
                else
                {
                    //Convertir a romano 
                    this.Numeros = "El resultado es: " + ZincriConverters.DecimalARomano(resultado_que_esta_en_top);
                }

            }
            else if (ultimo == 1018)
            {
                if (resultado_que_esta_en_top > 65535)
                {
                    this.Numeros = "El valor exede la represacion Hexadecimal.";
                }
                else
                {
                    //Convertir a HEX
                    this.Numeros = "El resultado es: " + ZincriConverters.DecimalAHexadecimal(resultado_que_esta_en_top);

                }
            }
            else if (ultimo == 1017)
            {
                this.Numeros = "El resultado es: " + pila_operandos.Last().ToString();
            }
            /*__________________________________________________*/


            /* Descomenta cuando acabez para saber si todo sale bien ///simon ya!
            this.Numeros = Numeros + "\n" + this.Cadena + "\n"; // Esta linea es solo para saber si si va todo chido

            foreach (var item in ValoresSeparados)
            {
                this.Numeros = Numeros + item + "";
            }
            */
        }

        private int RealizarOperacionTipoCuadruplo(int TopMenos1,int Top, int Operacion) 
        {
            int resultado = -1;
            switch(Operacion)
            {
                case 1:
                    resultado = TopMenos1 + Top;
                    break;
                case 2:
                    resultado = TopMenos1 - Top;
                    break;
                case 3:
                    resultado = TopMenos1 * Top;
                    break;
                case 4:
                    if (Top == 0) { break; }
                    resultado = TopMenos1 / Top;
                    break;
            }
            return resultado; 
        }

        /// <summary>
        /// Se puede extraer el valor en los valores que son convertibles,
        /// en caso de que el parametro valor sea un operador solo se le asigna una prioridad.
        /// </summary>
        /// <returns>The valor.</returns>
        /// <param name="tipo">Tipo.</param>
        /// <param name="valor_a_extraer">Valor a extraer.</param>
        private int ExtraerValor(char tipo, string valor_a_extraer)
        {
            int valor = 0;
            switch (tipo) 
            {
                case 'B':
                    valor=ZincriConverters.BinarioADecimal(valor_a_extraer);
                    break;
                case 'H':
                    valor_a_extraer=valor_a_extraer.Substring(1);
                    valor = ZincriConverters.HexadecimalADecimal(valor_a_extraer);
                    break;
                case '+':
                    valor = 1;
                    break;
                case '-':
                    valor = 2;
                    break;
                case '*':
                    valor = 3;
                    break;
                case '/':
                    valor = 4;
                    break;
                case 'Z':
                    valor_a_extraer = valor_a_extraer.Substring(1);
                    valor = ZincriConverters.NumeroZADecimal(valor_a_extraer);
                    break;
                case 'R':
                    valor = ZincriConverters.RomanoADecimal(valor_a_extraer);
                    break;
            }

            return valor;
        }

        /// <summary>
        /// En esta funcion se recibe un token y dependiendo el rango o numero que sea el token,
        /// ésta funcion devolvera el tipo de dato representativo (para nuestra perspectiva). 
        /// </summary>
        /// <returns>The tipo.</returns>
        /// <param name="token">Token.</param>
        private char buscarTipo(int token)
        {
            char tipo=' ';
            switch (token) {
                case 1000:
                    tipo = 'B';
                    break;
                case 1001:
                    tipo = 'H';
                    break;
                case 1002:
                    tipo = '+';
                    break;
                case 1003:
                    tipo = '-';
                    break;
                case 1004:
                    tipo = '*';
                    break;
                case 1005:
                    tipo = '/';
                    break;
                case 1006:
                case 1023:
                case 1024:
                case 1025:
                    tipo = 'Z';
                    break;
                case 1007:
                case 1008:
                case 1009:
                case 1010:
                case 1011:
                case 1012:
                case 1013:
                case 1014:
                case 1015:
                case 1016:
                    tipo = 'R';
                    break;

                #region Inservibles
                /*AQUI SE PUEDEN QUITAR YA QUE NO SE TOMA EN CUENTA EL SIMBOLO DE RESULTADO EN ESTA PARTE*/
                case 1017:
                    tipo = 'D';
                    break;
                case 1018:
                    tipo = 'X';
                    break;
                case 1019:
                    tipo = 'S';
                    break;
                #endregion
                case 1020:
                    tipo = (char)00;
                    break;
                

            }
            return tipo;
             
        }

        #region Sintactico
        /// <summary>
        /// Este Metodo verifica que el orden de la cadena sea correcto y
        /// cumpla con las caracteristicas sintacticas.
        /// </summary>
        /// <returns>The orden.</returns>
        /// <param name="listTokens">List tokens.</param>
        private int verficaOrden(List<int> listTokens)
        {
            List<int> vOperandos = new List<int>();
            vOperandos.Add(1000);
            vOperandos.Add(1001);
            vOperandos.Add(1006);
            vOperandos.Add(1007);
            vOperandos.Add(1008);
            vOperandos.Add(1009);
            vOperandos.Add(1010);
            vOperandos.Add(1011);
            vOperandos.Add(1012);
            vOperandos.Add(1013);
            vOperandos.Add(1014);
            vOperandos.Add(1015);
            vOperandos.Add(1016);
            //Numero Z, ultimos agregados.
            vOperandos.Add(1023);
            vOperandos.Add(1024);
            vOperandos.Add(1025);

            List<int> vOperadores = new List<int>();
            vOperadores.Add(1002);
            vOperadores.Add(1003);
            vOperadores.Add(1004);
            vOperadores.Add(1005);

            List<int> vResultados = new List<int>();
            vResultados.Add(1017);
            vResultados.Add(1018);
            vResultados.Add(1019);

            int Espacio = 1020;

            listTokens.RemoveAt(listTokens.Count - 1);
            listTokens.RemoveAt(listTokens.Count - 1);
            int[] arrayANA = listTokens.ToArray();
            ultimo = arrayANA[listTokens.Count - 1];
            listTokens.RemoveAt(listTokens.Count - 1);
            arrayANA = listTokens.ToArray();
            bool flagOp = false;

            for (int i = 0; i < listTokens.Count; i++)
            {
                if (i == 0) { flagOp = true; }

                if (i % 2 == 0)
                {

                    if (flagOp == true)
                    {
                        if (!vOperandos.Contains(arrayANA[i]))
                        {
                            return 0;
                        }
                        flagOp = false;

                    }
                    else
                    {
                        if (!vOperadores.Contains(arrayANA[i]))
                        {
                            return 0;
                        }
                        flagOp = true;


                    }
                }
                else if (i % 2 != 0)
                {
                    if (arrayANA[i] != Espacio)
                        return 0;
                }
            }
            if (!vResultados.Contains(ultimo))
            {
                return 0;
            }
            return 1;

        }
        #endregion

        #region Lexico
        /// <summary>
        /// Esta funcion es el algoritmo de nuestro analizador lexico
        /// </summary>
        /// <returns>The lexico.</returns>
        private int AnalizadorLexico()
        {
            Estado = 0;
            C = Inspeccionar(array[Indice]);

            if (C == 2)
            {
                flagHex = true;
            }
            if (C == 17)
            {
                flagHex = false;
            }
            if (C == 22)
            {
                return 2000;
            }
            if (C == 21)
            {
                return -1;
            }

            Contador = 0;
            Nuevo_estado = Matriz[Estado, C];
            if (Nuevo_estado == -1)
            {
                return -1;
            }

            while (Nuevo_estado < 1000 && Nuevo_estado != -1)
            {
                if (Indice == (array.Length - 1))
                    break;
                Avanzar();
                C = Inspeccionar(array[Indice]);
                if (C == 2)
                {
                    flagHex = true;
                }
                if (C == 17)
                {
                    flagHex = false;
                }
                if (C == 22)
                {
                    return 2000;
                }
                if (C == 21)
                {
                    return -1;
                }
                Estado = Nuevo_estado;
                Nuevo_estado = Matriz[Estado, C];
                Contador++;
            }
            if (Contador == 0 && Indice != (array.Length - 1))
                Avanzar();
            return Nuevo_estado;

        }
        #endregion

        private void Avanzar()
        {
            Indice++;
        }

        #region Inspeccionar
        /// <summary>
        /// Esta funcion nos devuelve la columna dada por el caracter que se ingrese.
        /// </summary>
        /// <returns>The inspeccionar.</returns>
        /// <param name="Caracter">Caracter.</param>
        private int Inspeccionar(char Caracter)
        {
            int valor = 0;
            if (((int)Caracter >= 65) && ((int)Caracter <= 70) || ((int)Caracter >= 97) && ((int)Caracter <= 102))
            {
                valor = 3;
            }
            else
            {
                if (flagHex && ((int)Caracter >= 48) && ((int)Caracter <= 57))
                {
                    valor = 4;
                }

                else
                {
                    switch (Caracter)
                    {

                        case '0':
                            valor = 0;
                            break;
                        case '1':
                            valor = 1;
                            break;
                        case 'H':
                            valor = 2;
                            break;
                        case '+':
                            valor = 5;
                            break;

                        case '-':
                            valor = 6;
                            break;


                        case '*':
                            valor = 7;
                            break;


                        case '/':
                            valor = 8;
                            break;


                        case 'Z':
                            valor = 9;
                            break;


                        case '.':
                            valor = 10;
                            break;


                        case ':':
                            valor = 11;
                            break;


                        case ';':
                            valor = 12;
                            break;

                        case '¡':
                            valor = 13;
                            break;

                        case 'I':
                            valor = 14;
                            break;

                        case 'V':
                            valor = 15;
                            break;

                        case 'X':
                            valor = 16;
                            break;

                        case ' ':
                            valor = 17;
                            break;

                        case '=':
                            valor = 18;
                            break;
                        case '&':
                            valor = 19;
                            break;

                        case '@':
                            valor = 20;
                            break;

                        case '#':
                            valor = 22;
                            break;


                        default:
                            valor = 21;
                            break;
                    }
                }
            }

            return valor;
        }
        #endregion


        #region Matriz
        /// <summary>
        /// Este metodo llena la matriz o tabla de transicion
        /// </summary>
        private void InicializarMatriz()
        {
            Matriz[0, 0] = 1;
            Matriz[0, 1] = 1;
            Matriz[0, 2] = 9;
            Matriz[0, 3] = -1;
            Matriz[0, 4] = -1;
            Matriz[0, 5] = 14;
            Matriz[0, 6] = 15;
            Matriz[0, 7] = 16;
            Matriz[0, 8] = 17;
            Matriz[0, 9] = 18;
            Matriz[0, 10] = -1;
            Matriz[0, 11] = -1;
            Matriz[0, 12] = -1;
            Matriz[0, 13] = -1;
            Matriz[0, 14] = 20;
            Matriz[0, 15] = 25;
            Matriz[0, 16] = 29;
            Matriz[0, 17] = 1020;
            Matriz[0, 18] = 1017;
            Matriz[0, 19] = 1018;
            Matriz[0, 20] = 1019;

            Matriz[1, 0] = 2;
            Matriz[1, 1] = 2;
            Matriz[1, 2] = -1;
            Matriz[1, 3] = -1;
            Matriz[1, 4] = -1;
            Matriz[1, 5] = -1;
            Matriz[1, 6] = -1;
            Matriz[1, 7] = -1;
            Matriz[1, 8] = -1;
            Matriz[1, 9] = -1;
            Matriz[1, 10] = -1;
            Matriz[1, 11] = -1;
            Matriz[1, 12] = -1;
            Matriz[1, 13] = -1;
            Matriz[1, 14] = -1;
            Matriz[1, 15] = -1;
            Matriz[1, 16] = -1;
            Matriz[1, 17] = 1000;
            Matriz[1, 18] = -1;
            Matriz[1, 19] = -1;
            Matriz[1, 20] = -1;

            Matriz[2, 0] = 3;
            Matriz[2, 1] = 3;
            Matriz[2, 2] = -1;
            Matriz[2, 3] = -1;
            Matriz[2, 4] = -1;
            Matriz[2, 5] = -1;
            Matriz[2, 6] = -1;
            Matriz[2, 7] = -1;
            Matriz[2, 8] = -1;
            Matriz[2, 9] = -1;
            Matriz[2, 10] = -1;
            Matriz[2, 11] = -1;
            Matriz[2, 12] = -1;
            Matriz[2, 13] = -1;
            Matriz[2, 14] = -1;
            Matriz[2, 15] = -1;
            Matriz[2, 16] = -1;
            Matriz[2, 17] = 1000;
            Matriz[2, 18] = -1;
            Matriz[2, 19] = -1;
            Matriz[2, 20] = -1;

            Matriz[3, 0] = 4;
            Matriz[3, 1] = 4;
            Matriz[3, 2] = -1;
            Matriz[3, 3] = -1;
            Matriz[3, 4] = -1;
            Matriz[3, 5] = -1;
            Matriz[3, 6] = -1;
            Matriz[3, 7] = -1;
            Matriz[3, 8] = -1;
            Matriz[3, 9] = -1;
            Matriz[3, 10] = -1;
            Matriz[3, 11] = -1;
            Matriz[3, 12] = -1;
            Matriz[3, 13] = -1;
            Matriz[3, 14] = -1;
            Matriz[3, 15] = -1;
            Matriz[3, 16] = -1;
            Matriz[3, 17] = 1000;
            Matriz[3, 18] = -1;
            Matriz[3, 19] = -1;
            Matriz[3, 20] = -1;

            Matriz[4, 0] = 5;
            Matriz[4, 1] = 5;
            Matriz[4, 2] = -1;
            Matriz[4, 3] = -1;
            Matriz[4, 4] = -1;
            Matriz[4, 5] = -1;
            Matriz[4, 6] = -1;
            Matriz[4, 7] = -1;
            Matriz[4, 8] = -1;
            Matriz[4, 9] = -1;
            Matriz[4, 10] = -1;
            Matriz[4, 11] = -1;
            Matriz[4, 12] = -1;
            Matriz[4, 13] = -1;
            Matriz[4, 14] = -1;
            Matriz[4, 15] = -1;
            Matriz[4, 16] = -1;
            Matriz[4, 17] = 1000;
            Matriz[4, 18] = -1;
            Matriz[4, 19] = -1;
            Matriz[4, 20] = -1;

            Matriz[5, 0] = 6;
            Matriz[5, 1] = 6;
            Matriz[5, 2] = -1;
            Matriz[5, 3] = -1;
            Matriz[5, 4] = -1;
            Matriz[5, 5] = -1;
            Matriz[5, 6] = -1;
            Matriz[5, 7] = -1;
            Matriz[5, 8] = -1;
            Matriz[5, 9] = -1;
            Matriz[5, 10] = -1;
            Matriz[5, 11] = -1;
            Matriz[5, 12] = -1;
            Matriz[5, 13] = -1;
            Matriz[5, 14] = -1;
            Matriz[5, 15] = -1;
            Matriz[5, 16] = -1;
            Matriz[5, 17] = 1000;
            Matriz[5, 18] = -1;
            Matriz[5, 19] = -1;
            Matriz[5, 20] = -1;

            Matriz[6, 0] = 7;
            Matriz[6, 1] = 7;
            Matriz[6, 2] = -1;
            Matriz[6, 3] = -1;
            Matriz[6, 4] = -1;
            Matriz[6, 5] = -1;
            Matriz[6, 6] = -1;
            Matriz[6, 7] = -1;
            Matriz[6, 8] = -1;
            Matriz[6, 9] = -1;
            Matriz[6, 10] = -1;
            Matriz[6, 11] = -1;
            Matriz[6, 12] = -1;
            Matriz[6, 13] = -1;
            Matriz[6, 14] = -1;
            Matriz[6, 15] = -1;
            Matriz[6, 16] = -1;
            Matriz[6, 17] = 1000;
            Matriz[6, 18] = -1;
            Matriz[6, 19] = -1;
            Matriz[6, 20] = -1;

            Matriz[7, 0] = 8;
            Matriz[7, 1] = 8;
            Matriz[7, 2] = -1;
            Matriz[7, 3] = -1;
            Matriz[7, 4] = -1;
            Matriz[7, 5] = -1;
            Matriz[7, 6] = -1;
            Matriz[7, 7] = -1;
            Matriz[7, 8] = -1;
            Matriz[7, 9] = -1;
            Matriz[7, 10] = -1;
            Matriz[7, 11] = -1;
            Matriz[7, 12] = -1;
            Matriz[7, 13] = -1;
            Matriz[7, 14] = -1;
            Matriz[7, 15] = -1;
            Matriz[7, 16] = -1;
            Matriz[7, 17] = 1000;
            Matriz[7, 18] = -1;
            Matriz[7, 19] = -1;
            Matriz[7, 20] = -1;

            Matriz[8, 0] = -1;
            Matriz[8, 1] = -1;
            Matriz[8, 2] = -1;
            Matriz[8, 3] = -1;
            Matriz[8, 4] = -1;
            Matriz[8, 5] = -1;
            Matriz[8, 6] = -1;
            Matriz[8, 7] = -1;
            Matriz[8, 8] = -1;
            Matriz[8, 9] = -1;
            Matriz[8, 10] = -1;
            Matriz[8, 11] = -1;
            Matriz[8, 12] = -1;
            Matriz[8, 13] = -1;
            Matriz[8, 14] = -1;
            Matriz[8, 15] = -1;
            Matriz[8, 16] = -1;
            Matriz[8, 17] = 1000;
            Matriz[8, 18] = -1;
            Matriz[8, 19] = -1;
            Matriz[8, 20] = -1;

            Matriz[9, 0] = 10;
            Matriz[9, 1] = 10;
            Matriz[9, 2] = -1;
            Matriz[9, 3] = 10;
            Matriz[9, 4] = 10;
            Matriz[9, 5] = -1;
            Matriz[9, 6] = -1;
            Matriz[9, 7] = -1;
            Matriz[9, 8] = -1;
            Matriz[9, 9] = -1;
            Matriz[9, 10] = -1;
            Matriz[9, 11] = -1;
            Matriz[9, 12] = -1;
            Matriz[9, 13] = -1;
            Matriz[9, 14] = -1;
            Matriz[9, 15] = -1;
            Matriz[9, 16] = -1;
            Matriz[9, 17] = 1001;
            Matriz[9, 18] = -1;
            Matriz[9, 19] = -1;
            Matriz[9, 20] = -1;

            Matriz[10, 0] = 11;
            Matriz[10, 1] = 11;
            Matriz[10, 2] = -1;
            Matriz[10, 3] = 11;
            Matriz[10, 4] = 11;
            Matriz[10, 5] = -1;
            Matriz[10, 6] = -1;
            Matriz[10, 7] = -1;
            Matriz[10, 8] = -1;
            Matriz[10, 9] = -1;
            Matriz[10, 10] = -1;
            Matriz[10, 11] = -1;
            Matriz[10, 12] = -1;
            Matriz[10, 13] = -1;
            Matriz[10, 14] = -1;
            Matriz[10, 15] = -1;
            Matriz[10, 16] = -1;
            Matriz[10, 17] = 1001;
            Matriz[10, 18] = -1;
            Matriz[10, 19] = -1;
            Matriz[10, 20] = -1;

            Matriz[11, 0] = 12;
            Matriz[11, 1] = 12;
            Matriz[11, 2] = -1;
            Matriz[11, 3] = 12;
            Matriz[11, 4] = 12;
            Matriz[11, 5] = -1;
            Matriz[11, 6] = -1;
            Matriz[11, 7] = -1;
            Matriz[11, 8] = -1;
            Matriz[11, 9] = -1;
            Matriz[11, 10] = -1;
            Matriz[11, 11] = -1;
            Matriz[11, 12] = -1;
            Matriz[11, 13] = -1;
            Matriz[11, 14] = -1;
            Matriz[11, 15] = -1;
            Matriz[11, 16] = -1;
            Matriz[11, 17] = 1001;
            Matriz[11, 18] = -1;
            Matriz[11, 19] = -1;
            Matriz[11, 20] = -1;

            Matriz[12, 0] = 13;
            Matriz[12, 1] = 13;
            Matriz[12, 2] = -1;
            Matriz[12, 3] = 13;
            Matriz[12, 4] = 13;
            Matriz[12, 5] = -1;
            Matriz[12, 6] = -1;
            Matriz[12, 7] = -1;
            Matriz[12, 8] = -1;
            Matriz[12, 9] = -1;
            Matriz[12, 10] = -1;
            Matriz[12, 11] = -1;
            Matriz[12, 12] = -1;
            Matriz[12, 13] = -1;
            Matriz[12, 14] = -1;
            Matriz[12, 15] = -1;
            Matriz[12, 16] = -1;
            Matriz[12, 17] = 1001;
            Matriz[12, 18] = -1;
            Matriz[12, 19] = -1;
            Matriz[12, 20] = -1;

            Matriz[13, 0] = -1;
            Matriz[13, 1] = -1;
            Matriz[13, 2] = -1;
            Matriz[13, 3] = -1;
            Matriz[13, 4] = -1;
            Matriz[13, 5] = -1;
            Matriz[13, 6] = -1;
            Matriz[13, 7] = -1;
            Matriz[13, 8] = -1;
            Matriz[13, 9] = -1;
            Matriz[13, 10] = -1;
            Matriz[13, 11] = -1;
            Matriz[13, 12] = -1;
            Matriz[13, 13] = -1;
            Matriz[13, 14] = -1;
            Matriz[13, 15] = -1;
            Matriz[13, 16] = -1;
            Matriz[13, 17] = 1001;
            Matriz[13, 18] = -1;
            Matriz[13, 19] = -1;
            Matriz[13, 20] = -1;

            Matriz[14, 0] = -1;
            Matriz[14, 1] = -1;
            Matriz[14, 2] = -1;
            Matriz[14, 3] = -1;
            Matriz[14, 4] = -1;
            Matriz[14, 5] = -1;
            Matriz[14, 6] = -1;
            Matriz[14, 7] = -1;
            Matriz[14, 8] = -1;
            Matriz[14, 9] = -1;
            Matriz[14, 10] = -1;
            Matriz[14, 11] = -1;
            Matriz[14, 12] = -1;
            Matriz[14, 13] = -1;
            Matriz[14, 14] = -1;
            Matriz[14, 15] = -1;
            Matriz[14, 16] = -1;
            Matriz[14, 17] = 1002;
            Matriz[14, 18] = -1;
            Matriz[14, 19] = -1;
            Matriz[14, 20] = -1;

            Matriz[15, 0] = -1;
            Matriz[15, 1] = -1;
            Matriz[15, 2] = -1;
            Matriz[15, 3] = -1;
            Matriz[15, 4] = -1;
            Matriz[15, 5] = -1;
            Matriz[15, 6] = -1;
            Matriz[15, 7] = -1;
            Matriz[15, 8] = -1;
            Matriz[15, 9] = -1;
            Matriz[15, 10] = -1;
            Matriz[15, 11] = -1;
            Matriz[15, 12] = -1;
            Matriz[15, 13] = -1;
            Matriz[15, 14] = -1;
            Matriz[15, 15] = -1;
            Matriz[15, 16] = -1;
            Matriz[15, 17] = 1003;
            Matriz[15, 18] = -1;
            Matriz[15, 19] = -1;
            Matriz[15, 20] = -1;

            Matriz[16, 0] = -1;
            Matriz[16, 1] = -1;
            Matriz[16, 2] = -1;
            Matriz[16, 3] = -1;
            Matriz[16, 4] = -1;
            Matriz[16, 5] = -1;
            Matriz[16, 6] = -1;
            Matriz[16, 7] = -1;
            Matriz[16, 8] = -1;
            Matriz[16, 9] = -1;
            Matriz[16, 10] = -1;
            Matriz[16, 11] = -1;
            Matriz[16, 12] = -1;
            Matriz[16, 13] = -1;
            Matriz[16, 14] = -1;
            Matriz[16, 15] = -1;
            Matriz[16, 16] = -1;
            Matriz[16, 17] = 1004;
            Matriz[16, 18] = -1;
            Matriz[16, 19] = -1;
            Matriz[16, 20] = -1;

            Matriz[17, 0] = -1;
            Matriz[17, 1] = -1;
            Matriz[17, 2] = -1;
            Matriz[17, 3] = -1;
            Matriz[17, 4] = -1;
            Matriz[17, 5] = -1;
            Matriz[17, 6] = -1;
            Matriz[17, 7] = -1;
            Matriz[17, 8] = -1;
            Matriz[17, 9] = -1;
            Matriz[17, 10] = -1;
            Matriz[17, 11] = -1;
            Matriz[17, 12] = -1;
            Matriz[17, 13] = -1;
            Matriz[17, 14] = -1;
            Matriz[17, 15] = -1;
            Matriz[17, 16] = -1;
            Matriz[17, 17] = 1005;
            Matriz[17, 18] = -1;
            Matriz[17, 19] = -1;
            Matriz[17, 20] = -1;

            Matriz[18, 0] = -1;
            Matriz[18, 1] = -1;
            Matriz[18, 2] = -1;
            Matriz[18, 3] = -1;
            Matriz[18, 4] = -1;
            Matriz[18, 5] = -1;
            Matriz[18, 6] = -1;
            Matriz[18, 7] = -1;
            Matriz[18, 8] = -1;
            Matriz[18, 9] = -1;
            Matriz[18, 10] = 19;
            Matriz[18, 11] = 35;
            Matriz[18, 12] = 38;
            Matriz[18, 13] = 41;
            Matriz[18, 14] = -1;
            Matriz[18, 15] = -1;
            Matriz[18, 16] = -1;
            Matriz[18, 17] = -1;
            Matriz[18, 18] = -1;
            Matriz[18, 19] = -1;
            Matriz[18, 20] = -1;

            Matriz[19, 0] = -1;
            Matriz[19, 1] = -1;
            Matriz[19, 2] = -1;
            Matriz[19, 3] = -1;
            Matriz[19, 4] = -1;
            Matriz[19, 5] = -1;
            Matriz[19, 6] = -1;
            Matriz[19, 7] = -1;
            Matriz[19, 8] = -1;
            Matriz[19, 9] = -1;
            Matriz[19, 10] = -1;
            Matriz[19, 11] = -1;
            Matriz[19, 12] = 33;
            Matriz[19, 13] = -1;
            Matriz[19, 14] = -1;
            Matriz[19, 15] = -1;
            Matriz[19, 16] = -1;
            Matriz[19, 17] = 1006;
            Matriz[19, 18] = -1;
            Matriz[19, 19] = -1;
            Matriz[19, 20] = -1;

            Matriz[20, 0] = -1;
            Matriz[20, 1] = -1;
            Matriz[20, 2] = -1;
            Matriz[20, 3] = -1;
            Matriz[20, 4] = -1;
            Matriz[20, 5] = -1;
            Matriz[20, 6] = -1;
            Matriz[20, 7] = -1;
            Matriz[20, 8] = -1;
            Matriz[20, 9] = -1;
            Matriz[20, 10] = -1;
            Matriz[20, 11] = -1;
            Matriz[20, 12] = -1;
            Matriz[20, 13] = -1;
            Matriz[20, 14] = 21;
            Matriz[20, 15] = 23;
            Matriz[20, 16] = 24;
            Matriz[20, 17] = 1007;
            Matriz[20, 18] = -1;
            Matriz[20, 19] = -1;
            Matriz[20, 20] = -1;

            Matriz[21, 0] = -1;
            Matriz[21, 1] = -1;
            Matriz[21, 2] = -1;
            Matriz[21, 3] = -1;
            Matriz[21, 4] = -1;
            Matriz[21, 5] = -1;
            Matriz[21, 6] = -1;
            Matriz[21, 7] = -1;
            Matriz[21, 8] = -1;
            Matriz[21, 9] = -1;
            Matriz[21, 10] = -1;
            Matriz[21, 11] = -1;
            Matriz[21, 12] = -1;
            Matriz[21, 13] = -1;
            Matriz[21, 14] = 22;
            Matriz[21, 15] = -1;
            Matriz[21, 16] = -1;
            Matriz[21, 17] = 1008;
            Matriz[21, 18] = -1;
            Matriz[21, 19] = -1;
            Matriz[21, 20] = -1;

            Matriz[22, 0] = -1;
            Matriz[22, 1] = -1;
            Matriz[22, 2] = -1;
            Matriz[22, 3] = -1;
            Matriz[22, 4] = -1;
            Matriz[22, 5] = -1;
            Matriz[22, 6] = -1;
            Matriz[22, 7] = -1;
            Matriz[22, 8] = -1;
            Matriz[22, 9] = -1;
            Matriz[22, 10] = -1;
            Matriz[22, 11] = -1;
            Matriz[22, 12] = -1;
            Matriz[22, 13] = -1;
            Matriz[22, 14] = -1;
            Matriz[22, 15] = -1;
            Matriz[22, 16] = -1;
            Matriz[22, 17] = 1009;
            Matriz[22, 18] = -1;
            Matriz[22, 19] = -1;
            Matriz[22, 20] = -1;

            Matriz[23, 0] = -1;
            Matriz[23, 1] = -1;
            Matriz[23, 2] = -1;
            Matriz[23, 3] = -1;
            Matriz[23, 4] = -1;
            Matriz[23, 5] = -1;
            Matriz[23, 6] = -1;
            Matriz[23, 7] = -1;
            Matriz[23, 8] = -1;
            Matriz[23, 9] = -1;
            Matriz[23, 10] = -1;
            Matriz[23, 11] = -1;
            Matriz[23, 12] = -1;
            Matriz[23, 13] = -1;
            Matriz[23, 14] = -1;
            Matriz[23, 15] = -1;
            Matriz[23, 16] = -1;
            Matriz[23, 17] = 1010;
            Matriz[23, 18] = -1;
            Matriz[23, 19] = -1;
            Matriz[23, 20] = -1;

            Matriz[24, 0] = -1;
            Matriz[24, 1] = -1;
            Matriz[24, 2] = -1;
            Matriz[24, 3] = -1;
            Matriz[24, 4] = -1;
            Matriz[24, 5] = -1;
            Matriz[24, 6] = -1;
            Matriz[24, 7] = -1;
            Matriz[24, 8] = -1;
            Matriz[24, 9] = -1;
            Matriz[24, 10] = -1;
            Matriz[24, 11] = -1;
            Matriz[24, 12] = -1;
            Matriz[24, 13] = -1;
            Matriz[24, 14] = -1;
            Matriz[24, 15] = -1;
            Matriz[24, 16] = -1;
            Matriz[24, 17] = 1011;
            Matriz[24, 18] = -1;
            Matriz[24, 19] = -1;
            Matriz[24, 20] = -1;

            Matriz[25, 0] = -1;
            Matriz[25, 1] = -1;
            Matriz[25, 2] = -1;
            Matriz[25, 3] = -1;
            Matriz[25, 4] = -1;
            Matriz[25, 5] = -1;
            Matriz[25, 6] = -1;
            Matriz[25, 7] = -1;
            Matriz[25, 8] = -1;
            Matriz[25, 9] = -1;
            Matriz[25, 10] = -1;
            Matriz[25, 11] = -1;
            Matriz[25, 12] = -1;
            Matriz[25, 13] = -1;
            Matriz[25, 14] = 26;
            Matriz[25, 15] = -1;
            Matriz[25, 16] = -1;
            Matriz[25, 17] = 1012;
            Matriz[25, 18] = -1;
            Matriz[25, 19] = -1;
            Matriz[25, 20] = -1;

            Matriz[26, 0] = -1;
            Matriz[26, 1] = -1;
            Matriz[26, 2] = -1;
            Matriz[26, 3] = -1;
            Matriz[26, 4] = -1;
            Matriz[26, 5] = -1;
            Matriz[26, 6] = -1;
            Matriz[26, 7] = -1;
            Matriz[26, 8] = -1;
            Matriz[26, 9] = -1;
            Matriz[26, 10] = -1;
            Matriz[26, 11] = -1;
            Matriz[26, 12] = -1;
            Matriz[26, 13] = -1;
            Matriz[26, 14] = 27;
            Matriz[26, 15] = -1;
            Matriz[26, 16] = -1;
            Matriz[26, 17] = 1013;
            Matriz[26, 18] = -1;
            Matriz[26, 19] = -1;
            Matriz[26, 20] = -1;

            Matriz[27, 0] = -1;
            Matriz[27, 1] = -1;
            Matriz[27, 2] = -1;
            Matriz[27, 3] = -1;
            Matriz[27, 4] = -1;
            Matriz[27, 5] = -1;
            Matriz[27, 6] = -1;
            Matriz[27, 7] = -1;
            Matriz[27, 8] = -1;
            Matriz[27, 9] = -1;
            Matriz[27, 10] = -1;
            Matriz[27, 11] = -1;
            Matriz[27, 12] = -1;
            Matriz[27, 13] = -1;
            Matriz[27, 14] = 28;
            Matriz[27, 15] = -1;
            Matriz[27, 16] = -1;
            Matriz[27, 17] = 1014;
            Matriz[27, 18] = -1;
            Matriz[27, 19] = -1;
            Matriz[27, 20] = -1;

            Matriz[28, 0] = -1;
            Matriz[28, 1] = -1;
            Matriz[28, 2] = -1;
            Matriz[28, 3] = -1;
            Matriz[28, 4] = -1;
            Matriz[28, 5] = -1;
            Matriz[28, 6] = -1;
            Matriz[28, 7] = -1;
            Matriz[28, 8] = -1;
            Matriz[28, 9] = -1;
            Matriz[28, 10] = -1;
            Matriz[28, 11] = -1;
            Matriz[28, 12] = -1;
            Matriz[28, 13] = -1;
            Matriz[28, 14] = -1;
            Matriz[28, 15] = -1;
            Matriz[28, 16] = -1;
            Matriz[28, 17] = 1015;
            Matriz[28, 18] = -1;
            Matriz[28, 19] = -1;
            Matriz[28, 20] = -1;

            Matriz[29, 0] = -1;
            Matriz[29, 1] = -1;
            Matriz[29, 2] = -1;
            Matriz[29, 3] = -1;
            Matriz[29, 4] = -1;
            Matriz[29, 5] = -1;
            Matriz[29, 6] = -1;
            Matriz[29, 7] = -1;
            Matriz[29, 8] = -1;
            Matriz[29, 9] = -1;
            Matriz[29, 10] = -1;
            Matriz[29, 11] = -1;
            Matriz[29, 12] = -1;
            Matriz[29, 13] = -1;
            Matriz[29, 14] = -1;
            Matriz[29, 15] = -1;
            Matriz[29, 16] = -1;
            Matriz[29, 17] = 1016;
            Matriz[29, 18] = -1;
            Matriz[29, 19] = -1;
            Matriz[29, 20] = -1;

            Matriz[30, 0] = -1;
            Matriz[30, 1] = -1;
            Matriz[30, 2] = -1;
            Matriz[30, 3] = -1;
            Matriz[30, 4] = -1;
            Matriz[30, 5] = -1;
            Matriz[30, 6] = -1;
            Matriz[30, 7] = -1;
            Matriz[30, 8] = -1;
            Matriz[30, 9] = -1;
            Matriz[30, 10] = -1;
            Matriz[30, 11] = -1;
            Matriz[30, 12] = -1;
            Matriz[30, 13] = -1;
            Matriz[30, 14] = -1;
            Matriz[30, 15] = -1;
            Matriz[30, 16] = -1;
            Matriz[30, 17] = 1016;
            Matriz[30, 18] = -1;
            Matriz[30, 19] = -1;
            Matriz[30, 20] = -1;

            Matriz[31, 0] = -1;
            Matriz[31, 1] = -1;
            Matriz[31, 2] = -1;
            Matriz[31, 3] = -1;
            Matriz[31, 4] = -1;
            Matriz[31, 5] = -1;
            Matriz[31, 6] = -1;
            Matriz[31, 7] = -1;
            Matriz[31, 8] = -1;
            Matriz[31, 9] = -1;
            Matriz[31, 10] = -1;
            Matriz[31, 11] = -1;
            Matriz[31, 12] = -1;
            Matriz[31, 13] = -1;
            Matriz[31, 14] = -1;
            Matriz[31, 15] = -1;
            Matriz[31, 16] = -1;
            Matriz[31, 17] = 1016;
            Matriz[31, 18] = -1;
            Matriz[31, 19] = -1;
            Matriz[31, 20] = -1;

            Matriz[32, 0] = -1;
            Matriz[32, 1] = -1;
            Matriz[32, 2] = -1;
            Matriz[32, 3] = -1;
            Matriz[32, 4] = -1;
            Matriz[32, 5] = -1;
            Matriz[32, 6] = -1;
            Matriz[32, 7] = -1;
            Matriz[32, 8] = -1;
            Matriz[32, 9] = -1;
            Matriz[32, 10] = -1;
            Matriz[32, 11] = -1;
            Matriz[32, 12] = -1;
            Matriz[32, 13] = -1;
            Matriz[32, 14] = -1;
            Matriz[32, 15] = -1;
            Matriz[32, 16] = -1;
            Matriz[32, 17] = 1016;
            Matriz[32, 18] = -1;
            Matriz[32, 19] = -1;
            Matriz[32, 20] = -1;
            //Nuevos agregados
            Matriz[33, 0] = -1;
            Matriz[33, 1] = -1;
            Matriz[33, 2] = -1;
            Matriz[33, 3] = -1;
            Matriz[33, 4] = -1;
            Matriz[33, 5] = -1;
            Matriz[33, 6] = -1;
            Matriz[33, 7] = -1;
            Matriz[33, 8] = -1;
            Matriz[33, 9] = -1;
            Matriz[33, 10] = -1;
            Matriz[33, 11] = -1;
            Matriz[33, 12] = 34;
            Matriz[33, 13] = -1;
            Matriz[33, 14] = -1;
            Matriz[33, 15] = -1;
            Matriz[33, 16] = -1;
            Matriz[33, 17] = 1006;
            Matriz[33, 18] = -1;
            Matriz[33, 19] = -1;
            Matriz[33, 20] = -1;

            Matriz[34, 0] = -1;
            Matriz[34, 1] = -1;
            Matriz[34, 2] = -1;
            Matriz[34, 3] = -1;
            Matriz[34, 4] = -1;
            Matriz[34, 5] = -1;
            Matriz[34, 6] = -1;
            Matriz[34, 7] = -1;
            Matriz[34, 8] = -1;
            Matriz[34, 9] = -1;
            Matriz[34, 10] = -1;
            Matriz[34, 11] = -1;
            Matriz[34, 12] = -1;
            Matriz[34, 13] = -1;
            Matriz[34, 14] = -1;
            Matriz[34, 15] = -1;
            Matriz[34, 16] = -1;
            Matriz[34, 17] = 1006;
            Matriz[34, 18] = -1;
            Matriz[34, 19] = -1;
            Matriz[34, 20] = -1;

            Matriz[35, 0] = -1;
            Matriz[35, 1] = -1;
            Matriz[35, 2] = -1;
            Matriz[35, 3] = -1;
            Matriz[35, 4] = -1;
            Matriz[35, 5] = -1;
            Matriz[35, 6] = -1;
            Matriz[35, 7] = -1;
            Matriz[35, 8] = -1;
            Matriz[35, 9] = -1;
            Matriz[35, 10] = -1;
            Matriz[35, 11] = -1;
            Matriz[35, 12] = 36;
            Matriz[35, 13] = -1;
            Matriz[35, 14] = -1;
            Matriz[35, 15] = -1;
            Matriz[35, 16] = -1;
            Matriz[35, 17] = 1023;
            Matriz[35, 18] = -1;
            Matriz[35, 19] = -1;
            Matriz[35, 20] = -1;

            Matriz[36, 0] = -1;
            Matriz[36, 1] = -1;
            Matriz[36, 2] = -1;
            Matriz[36, 3] = -1;
            Matriz[36, 4] = -1;
            Matriz[36, 5] = -1;
            Matriz[36, 6] = -1;
            Matriz[36, 7] = -1;
            Matriz[36, 8] = -1;
            Matriz[36, 9] = -1;
            Matriz[36, 10] = -1;
            Matriz[36, 11] = -1;
            Matriz[36, 12] = 37;
            Matriz[36, 13] = -1;
            Matriz[36, 14] = -1;
            Matriz[36, 15] = -1;
            Matriz[36, 16] = -1;
            Matriz[36, 17] = 1023;
            Matriz[36, 18] = -1;
            Matriz[36, 19] = -1;
            Matriz[36, 20] = -1;

            Matriz[37, 0] = -1;
            Matriz[37, 1] = -1;
            Matriz[37, 2] = -1;
            Matriz[37, 3] = -1;
            Matriz[37, 4] = -1;
            Matriz[37, 5] = -1;
            Matriz[37, 6] = -1;
            Matriz[37, 7] = -1;
            Matriz[37, 8] = -1;
            Matriz[37, 9] = -1;
            Matriz[37, 10] = -1;
            Matriz[37, 11] = -1;
            Matriz[37, 12] = -1;
            Matriz[37, 13] = -1;
            Matriz[37, 14] = -1;
            Matriz[37, 15] = -1;
            Matriz[37, 16] = -1;
            Matriz[37, 17] = 1023;
            Matriz[37, 18] = -1;
            Matriz[37, 19] = -1;
            Matriz[37, 20] = -1;

            Matriz[38, 0] = -1;
            Matriz[38, 1] = -1;
            Matriz[38, 2] = -1;
            Matriz[38, 3] = -1;
            Matriz[38, 4] = -1;
            Matriz[38, 5] = -1;
            Matriz[38, 6] = -1;
            Matriz[38, 7] = -1;
            Matriz[38, 8] = -1;
            Matriz[38, 9] = -1;
            Matriz[38, 10] = -1;
            Matriz[38, 11] = -1;
            Matriz[38, 12] = 39;
            Matriz[38, 13] = -1;
            Matriz[38, 14] = -1;
            Matriz[38, 15] = -1;
            Matriz[38, 16] = -1;
            Matriz[38, 17] = 1024;
            Matriz[38, 18] = -1;
            Matriz[38, 19] = -1;
            Matriz[38, 20] = -1;

            Matriz[39, 0] = -1;
            Matriz[39, 1] = -1;
            Matriz[39, 2] = -1;
            Matriz[39, 3] = -1;
            Matriz[39, 4] = -1;
            Matriz[39, 5] = -1;
            Matriz[39, 6] = -1;
            Matriz[39, 7] = -1;
            Matriz[39, 8] = -1;
            Matriz[39, 9] = -1;
            Matriz[39, 10] = -1;
            Matriz[39, 11] = -1;
            Matriz[39, 12] = 40;
            Matriz[39, 13] = -1;
            Matriz[39, 14] = -1;
            Matriz[39, 15] = -1;
            Matriz[39, 16] = -1;
            Matriz[39, 17] = 1024;
            Matriz[39, 18] = -1;
            Matriz[39, 19] = -1;
            Matriz[39, 20] = -1;

            Matriz[40, 0] = -1;
            Matriz[40, 1] = -1;
            Matriz[40, 2] = -1;
            Matriz[40, 3] = -1;
            Matriz[40, 4] = -1;
            Matriz[40, 5] = -1;
            Matriz[40, 6] = -1;
            Matriz[40, 7] = -1;
            Matriz[40, 8] = -1;
            Matriz[40, 9] = -1;
            Matriz[40, 10] = -1;
            Matriz[40, 11] = -1;
            Matriz[40, 12] = -1;
            Matriz[40, 13] = -1;
            Matriz[40, 14] = -1;
            Matriz[40, 15] = -1;
            Matriz[40, 16] = -1;
            Matriz[40, 17] = 1024;
            Matriz[40, 18] = -1;
            Matriz[40, 19] = -1;
            Matriz[40, 20] = -1;

            Matriz[41, 0] = -1;
            Matriz[41, 1] = -1;
            Matriz[41, 2] = -1;
            Matriz[41, 3] = -1;
            Matriz[41, 4] = -1;
            Matriz[41, 5] = -1;
            Matriz[41, 6] = -1;
            Matriz[41, 7] = -1;
            Matriz[41, 8] = -1;
            Matriz[41, 9] = -1;
            Matriz[41, 10] = -1;
            Matriz[41, 11] = -1;
            Matriz[41, 12] = -1;
            Matriz[41, 13] = -1;
            Matriz[41, 14] = -1;
            Matriz[41, 15] = -1;
            Matriz[41, 16] = -1;
            Matriz[41, 17] = 1025;
            Matriz[41, 18] = -1;
            Matriz[41, 19] = -1;
            Matriz[41, 20] = -1;


        }
        #endregion
        #endregion
    }
}
