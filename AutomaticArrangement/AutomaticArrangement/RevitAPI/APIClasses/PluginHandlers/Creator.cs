using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using AutomaticArrangement.RevitAPI.APIClasses.Classes;
using AutomaticArrangement.RevitAPI.APIClasses.CommonTools;
using AutomaticArrangement.RevitAPI.APIClasses.Interfaces;
using AutomaticArrangement.RevitAPI.APIClasses.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticArrangement.RevitAPI.APIClasses.PluginHandlers
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class Creator : IExternalEventHandler
    {
        public UIApplication App { get; private set; }
        public Document Doc { get; private set; }
        private IArrangementCalculator calculator = null;
        public void Execute(UIApplication app)
        {
            this.calculator = new SimpleCalculator();
            this.App = app;
            this.Doc = app.ActiveUIDocument.Document;
            var ids = app.ActiveUIDocument.Selection.GetElementIds().ToList();
            ids.ForEach(CreateDevices);
        }

        public string GetName()
        {
            return "AutomaticArrangement:Creator";
        }

        private void CreateDevices(ElementId obj)
        {
            var room = this.Doc.GetElement(obj) as Room;
            Rules rules = GetRulesForRoom(room);
            List<LocationPoint> locations = CalcLocations(room, rules);
            CreateInstancesAndSetLocations(locations);
        }

        private void CreateInstancesAndSetLocations(List<LocationPoint> locations)
        {

        }

        private List<LocationPoint> CalcLocations(Room room, Rules rules)
        {
            return this.calculator.Calculate(room, rules);
        }

        private Rules GetRulesForRoom(Room room)
        {
            double height = DimensionConverter.FeetToMeter(room.UnboundedHeight);
            TaskDialog.Show("height", height.ToString());
            return height < 3.5 ?
            new Rules
            {
                MaxBetweenDevices = 5.0,
                MaxBetweenDeviceAndWall = 2.5
            } : new Rules
            {
                MaxBetweenDevices = 4.5,
                MaxBetweenDeviceAndWall = 2.0
            };

        }


    }
}
