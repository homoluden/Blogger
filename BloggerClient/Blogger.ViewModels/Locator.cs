using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blogger.ViewModels
{
    public sealed class Locator : BaseViewModel
    {
        #region Fields

        private AppViewModel _rootVm;

        #endregion


        #region Properties

        public AppViewModel RootViewModel
        {
            get { return _rootVm; }
            set 
            {
                _rootVm = value;
                RaisePropertyChanged("RootViewModel");
            }
        }

        #endregion

        #region Singleton impl.
        private static volatile Locator instance;
        private static object syncRoot = new Object();

        private Locator() 
        {
            
        }

        public static Locator Instance
        {
            get 
            {
                if (instance == null) 
                {
                    lock (syncRoot) 
                    {
                        if (instance == null)
                            instance = new Locator();
                    }
                }

                return instance;
            }
        }
        #endregion
    }
}
