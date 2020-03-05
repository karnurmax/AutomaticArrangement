﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;

namespace AutomaticArrangement
{
    class WpfWindowController : IExternalApplication
    {
        // class instance
        public static WpfWindowController Instance;
        // ModelessForm instance
        private AutomaticArrangementUserControl _AutomaticArrangementUserControl;

        public System.Windows.Window Window = new System.Windows.Window();


        public Result OnStartup(UIControlledApplication application)
        {

            _AutomaticArrangementUserControl = null;
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            if (Window != null)
            {
                Window.Close();
            }

            return Result.Succeeded;

        }

        public void ShowForm(UIApplication uiapp)
        {
            if (_AutomaticArrangementUserControl == null)
            {
                if (Instance == null)
                {
                    Instance = this;
                }

                // A new handler to handle request posting by the dialog
                RevitHandler handler = new RevitHandler();

                // External Event for the dialog to use (to post requests)
                ExternalEvent exEvent = ExternalEvent.Create(handler);

                // We give the objects to the new dialog;
                // The dialog becomes the owner responsible for disposing them, eventually.


                _AutomaticArrangementUserControl = new AutomaticArrangementUserControl(exEvent);
                BitmapImage pb1Image = new BitmapImage(new Uri("pack://application:,,,/AutomaticArrangement;component/Resources/icon.ico"));

                Window.Content = _AutomaticArrangementUserControl;
                Window.Icon = pb1Image;
                Window.Title = Util.ApplicationWindowTitle;
                Window.Height = Util.ApplicationWindowHeight;
                Window.Topmost = Util.IsApplicationWindowTopMost;
                Window.Width = Util.ApplicationWindowWidth;
                Window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                Window.Show();

                Window.Closed += OnClosing;
                App.AutomaticArrangementButton.Enabled = false;


            }
        }

        public void OnClosing(object sender, EventArgs e)
        {
            App.AutomaticArrangementButton.Enabled = true;
            WpfWindowController.Instance = null;
            AutomaticArrangementUserControl.Instance = null;
        }
    }

}
