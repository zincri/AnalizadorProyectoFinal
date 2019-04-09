using System;
namespace Analizador.ViewModels
{
    public class MainViewModel
    {
        #region Properties
        public StartViewModel Start
        {
            get;
            set;
        }
        #endregion
        #region Constructor
        public MainViewModel()
        {
            this.Start = new StartViewModel();
        }
        #endregion
    }
}
