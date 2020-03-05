using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using AutomaticArrangement.MVVM.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using AutomaticArrangement.RevitAPI.APIClasses.PluginHandlers;

namespace AutomaticArrangement
{
    public class ViewModel : Singleton<ViewModel>, INotifyPropertyChanged
    {
        public ViewModel()
        {
            Creator handler = new Creator();
            ExternalEvent exEvent = ExternalEvent.Create(handler);
            this._event = exEvent;
        }
        private ExternalEvent _event;
        private RelayCommand calcCommand;
        public RelayCommand CalcCommand
        {
            get
            {
                return calcCommand ??
                  (calcCommand = new RelayCommand(obj =>
                  {
                      _event.Raise();
                  }));
            }
        }
        private RelayCommand checkCommand;

        public RelayCommand CheckCommand
        {
            get
            {
                return checkCommand ??
                  (checkCommand = new RelayCommand(obj =>
                  {

                  }));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }

}
