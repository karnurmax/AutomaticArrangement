using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using AutomaticArrangement.MVVM.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticArrangement
{
    public class ViewModel : Singleton<ViewModel>, INotifyPropertyChanged
    {
        public ViewModel()
        {
        }
        private RelayCommand calcCommand;
        public RelayCommand CalcCommand
        {
            get
            {
                return calcCommand ??
                  (calcCommand = new RelayCommand(obj =>
                  {

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
