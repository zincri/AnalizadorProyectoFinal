using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Analizador
{
    public partial class MainPage : ContentPage
    {
        public string msj
        {
            get;
            set;
        }

        public MainPage()
        {
            InitializeComponent();
            msj = "Zincri";
        }
        //Button button = FindViewById<Button>(Resource.Id.boton);
    }
}
