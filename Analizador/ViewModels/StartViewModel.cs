namespace Analizador.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;

    public class StartViewModel: INotifyPropertyChanged
    {
        #region Variables
        int[,] Matriz = new int[30, 19];//[5, 5];
        char[] array;
        int Indice = 0;
        int Estado = 0;
        int C = 0;
        int Contador = 0;
        int Nuevo_estado = 0;
        bool flagHex=false;
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
        private void SetCadena()
        {
            bool flag= false;
            int Token = 0;
            
            this.Numeros = String.Empty;
            array = (this.Cadena+"#").ToCharArray();

            //me falto el indice correrlo a 0 otra vez
            Indice = 0;
            while (Token!=504)
            {
                Token = AnalizadorLexico();
                this.Numeros = this.Numeros + "[" + Token.ToString() + "]";
                if (Token == -1) { flag = true; break; }
            }
            if (flag) {
                this.Numeros = "Cadena no aceptada en el caracter: "+(Indice+1);
            }


        }

        private int AnalizadorLexico()
        {
            Estado = 0;
            C = Inspeccionar(array[Indice]);
            if (C == 2)
            {
            flagHex=true; 
            }
            if (C == 17)
            {
                flagHex = false;
            }
            Contador = 0;
            Nuevo_estado = Matriz[Estado, C];

            //this.Numeros = this.Numeros + "," + Nuevo_estado.ToString();

            while (Nuevo_estado < 500 && Nuevo_estado!=-1)
            {
                if (Indice == (array.Length - 1))
                    break;
                Avanzar();
                C = Inspeccionar(array[Indice]);
                if(C==2)
                {
                    flagHex=true; 
                }
                if(C==17)
                {
                    flagHex = false;
                }
                Estado = Nuevo_estado;
                Nuevo_estado = Matriz[Estado, C];
                Contador++;
                //this.Numeros = this.Numeros + "," + Nuevo_estado.ToString();
            }
            if (Contador == 0 && Indice != (array.Length - 1))
                Avanzar();
            return Nuevo_estado;
            
        }

        private void Avanzar()
        {
            Indice++;
        }

        private int Inspeccionar(char Caracter)
        {
            int valor = 0;
            if (((int)Caracter >= 65) && ((int)Caracter <= 70))


            {
                valor = 3;
            }
            else
            {
                if (((int)Caracter >= 48) && ((int)Caracter <= 57))
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
                            valor = 4;
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

                        case 'v':
                            valor = 15;
                            break;

                        case 'X':
                            valor = 16;
                            break;

                        case ' ':
                            valor = 17;
                            break;

                        default:
                            valor = 18;
                            break;
                    }
                }
            }

            return valor;
        }


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
            Matriz[0, 17] = 1017;
            Matriz[0, 18] = -1;


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
            Matriz[18, 11] = 19;
            Matriz[18, 12] = 19;
            Matriz[18, 13] = 19;
            Matriz[18, 14] = -1;
            Matriz[18, 15] = -1;
            Matriz[18, 16] = -1;
            Matriz[18, 17] = -1;
            Matriz[18, 18] = -1;

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
            Matriz[19, 10] = 19;
            Matriz[19, 11] = 19;
            Matriz[19, 12] = 19;
            Matriz[19, 13] = 19;
            Matriz[19, 14] = -1;
            Matriz[19, 15] = -1;
            Matriz[19, 16] = -1;
            Matriz[19, 17] = 1006;
            Matriz[19, 18] = -1;

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
        }
        #endregion
    }
}

#region OLDCLASS
/*
 namespace Analizador.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;

    public class StartViewModel: INotifyPropertyChanged
    {
        #region Variables
        int[,] Matriz = new int[9, 9];
        char[] array;
        int Indice = 0;
        int Estado = 0;
        int C = 0;
        int Contador = 0;
        int Nuevo_estado = 0;
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
        private void SetCadena()
        {
            int Token = 0;
            this.Numeros = String.Empty;
            array = (this.Cadena+"#").ToCharArray();


            while(Token!=506)
            {
                Token = AnalizadorLexico();
                this.Numeros = this.Numeros + "[[" + Token.ToString() + "]]"; 
            }

        }

        private int AnalizadorLexico()
        {
            Estado = 0;
            C = Inspeccionar(array[Indice]);
            Contador = 0;
            Nuevo_estado = Matriz[Estado, C];

            this.Numeros = this.Numeros + "," + Nuevo_estado.ToString();

            while (Nuevo_estado < 500)
            {
                if (Indice == (array.Length - 1))
                    break;
                Avanzar();
                C = Inspeccionar(array[Indice]);
                Estado = Nuevo_estado;
                Nuevo_estado = Matriz[Estado, C];
                Contador++;
                //this.Numeros = this.Numeros + "," + Nuevo_estado.ToString();
            }
            if (Contador == 0 && Indice != (array.Length - 1))
                Avanzar();
            return Nuevo_estado;
            
        }

        private void Avanzar()
        {
            Indice++;
        }

        private int Inspeccionar(char Caracter)
        {
            int valor=0;
            if(((int)Caracter >= 65) && ((int)Caracter <= 122) && ((int)Caracter != 91)
               && ((int)Caracter != 92) && ((int)Caracter != 93) && ((int)Caracter != 94)
               && ((int)Caracter != 95 && ((int)Caracter != 96)))
            {
                    valor = 0; 
            }
            else
            {
                if (((int)Caracter >= 48) && ((int)Caracter <= 57))
                    valor = 4;
                else
                switch (Caracter)
                {
                    
                    case '(':
                        valor = 1;
                        break;
                    case ')':
                            valor = 2;
                            break;
                    case ';':
                            valor = 3;
                            break;
                    case '+':
                            valor = 5;
                            break;
                    case '-':
                            valor = 6;
                            break;
                    case '.':
                            valor = 7;
                            break;
                    case '#':
                            valor = 8;
                            break;
                     
                    default:
                        valor = 8;
                        break;
                }
            }

            return valor;
        }


        private void InicializarMatriz()
        {
            Matriz[0, 0] = 1;
            Matriz[0, 1] = 2;
            Matriz[0, 2] = 3;
            Matriz[0, 3] = 4;
            Matriz[0, 4] = 5;
            Matriz[0, 5] = 6;
            Matriz[0, 6] = 6;
            Matriz[0, 7] = 7;
            Matriz[0, 8] = 506;

            Matriz[1, 0] = 1;
            Matriz[1, 1] = 500;
            Matriz[1, 2] = 500;
            Matriz[1, 3] = 500;
            Matriz[1, 4] = 500;
            Matriz[1, 5] = 500;
            Matriz[1, 6] = 500;
            Matriz[1, 7] = 500;
            Matriz[1, 8] = 500;

            Matriz[2, 0] = 501;
            Matriz[2, 1] = 501;
            Matriz[2, 2] = 501;
            Matriz[2, 3] = 501;
            Matriz[2, 4] = 501;
            Matriz[2, 5] = 501;
            Matriz[2, 6] = 501;
            Matriz[2, 7] = 501;
            Matriz[2, 8] = 501;

            Matriz[3, 0] = 502;
            Matriz[3, 1] = 502;
            Matriz[3, 2] = 502;
            Matriz[3, 3] = 502;
            Matriz[3, 4] = 502;
            Matriz[3, 5] = 502;
            Matriz[3, 6] = 502;
            Matriz[3, 7] = 502;
            Matriz[3, 8] = 502;

            Matriz[4, 0] = 503;
            Matriz[4, 1] = 503;
            Matriz[4, 2] = 503;
            Matriz[4, 3] = 503;
            Matriz[4, 4] = 503;
            Matriz[4, 5] = 503;
            Matriz[4, 6] = 503;
            Matriz[4, 7] = 503;
            Matriz[4, 8] = 503;

            Matriz[5, 0] = 504;
            Matriz[5, 1] = 504;
            Matriz[5, 2] = 504;
            Matriz[5, 3] = 504;
            Matriz[5, 4] = 5;
            Matriz[5, 5] = 504;
            Matriz[5, 6] = 504;
            Matriz[5, 7] = 7;
            Matriz[5, 8] = 504;

            Matriz[6, 0] = 506;
            Matriz[6, 1] = 506;
            Matriz[6, 2] = 506;
            Matriz[6, 3] = 506;
            Matriz[6, 4] = 5;
            Matriz[6, 5] = 506;
            Matriz[6, 6] = 506;
            Matriz[6, 7] = 506;
            Matriz[6, 8] = 506;

            Matriz[7, 0] = 506;
            Matriz[7, 1] = 506;
            Matriz[7, 2] = 506;
            Matriz[7, 3] = 506;
            Matriz[7, 4] = 8;
            Matriz[7, 5] = 506;
            Matriz[7, 6] = 506;
            Matriz[7, 7] = 506;
            Matriz[7, 8] = 506;

            Matriz[8, 0] = 505;
            Matriz[8, 1] = 505;
            Matriz[8, 2] = 505;
            Matriz[8, 3] = 505;
            Matriz[8, 4] = 8;
            Matriz[8, 5] = 505;
            Matriz[8, 6] = 505;
            Matriz[8, 7] = 505;
            Matriz[8, 8] = 505;


        }
        #endregion
    }

*/
#endregion