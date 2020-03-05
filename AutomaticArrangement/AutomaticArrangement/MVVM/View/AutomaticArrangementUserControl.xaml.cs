using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autodesk.Revit.UI;

namespace AutomaticArrangement
{
    /// <summary>
    /// UI Events
    /// </summary>
    public partial class AutomaticArrangementUserControl : UserControl
    {
        public static AutomaticArrangementUserControl Instance;

        public ViewModel ViewModel;
        private ExternalEvent _event;

        public AutomaticArrangementUserControl(ExternalEvent exEvent)
        {
            InitializeComponent();

            Instance = this;
            ViewModel = new ViewModel();
            DataContext = ViewModel;
            _event = exEvent;
            _event.Raise();
        }
    }
}
