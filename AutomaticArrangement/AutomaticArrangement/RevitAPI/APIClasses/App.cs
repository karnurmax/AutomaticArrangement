#region Namespaces
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion


namespace AutomaticArrangement
{
    class App : IExternalApplication
    {
        public static PushButton AutomaticArrangementButton { get; set; }


        public Result OnStartup(UIControlledApplication application)
        {

            OnButtonCreate(application);
            application.ViewActivated += Application_ViewActivated;
            return Result.Succeeded;
        }

        private void Application_ViewActivated(object sender, Autodesk.Revit.UI.Events.ViewActivatedEventArgs e)
        {
            var app = (Autodesk.Revit.UI.UIApplication)sender;
            AutomaticArrangementUserControl.Instance.TextBlockProjectTitle.Text = app.ActiveUIDocument.Document.Title;
        }


        public Result OnShutdown(UIControlledApplication a)
        {
            AutomaticArrangementButton.Enabled = true;
            return Result.Succeeded;
        }

        //*****************************RibbonPanel()*****************************
        public RibbonPanel RibbonPanel(UIControlledApplication a)
        {
            string tab = Util.AddinRibbonTabName; // Archcorp
            string ribbonPanelText = Util.AddinRibbonPanel; // Architecture

            // Empty ribbon panel 
            RibbonPanel ribbonPanel = null;
            // Try to create ribbon tab. 
            try
            {
                a.CreateRibbonTab(tab);
            }
            catch { }
            // Try to create ribbon panel.
            try
            {
                RibbonPanel panel = a.CreateRibbonPanel(tab, ribbonPanelText);
            }
            catch { }
            // Search existing tab for your panel.
            List<RibbonPanel> panels = a.GetRibbonPanels(tab);
            foreach (RibbonPanel p in panels)
            {
                if (p.Name == ribbonPanelText)
                {
                    ribbonPanel = p;
                }
            }
            //return panel 
            return ribbonPanel;
        }


        /// <summary>
        /// Create a ribbon and panel and add a button for the revit DMS Plugin
        /// </summary>
        /// <param name="application"></param>
        private void OnButtonCreate(UIControlledApplication application)
        {
            string buttonText = Util.AddinButtonText;
            string buttonTooltip = Util.AddinButtonTooltip;

            string executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string dllLocation = Path.Combine(executableLocation, "AutomaticArrangement.dll");

            // Create two push buttons

            PushButtonData buttondata = new PushButtonData("AutomaticArrangementBtn", buttonText, dllLocation, "AutomaticArrangement.Command");
            buttondata.ToolTip = buttonTooltip;

            BitmapImage pb1Image = new BitmapImage(new Uri("pack://application:,,,/AutomaticArrangement;component/Resources/icon.ico"));
            buttondata.LargeImage = pb1Image;

            var ribbonPanel = RibbonPanel(application);


            AutomaticArrangementButton = ribbonPanel.AddItem(buttondata) as PushButton;

        }

    }

}
