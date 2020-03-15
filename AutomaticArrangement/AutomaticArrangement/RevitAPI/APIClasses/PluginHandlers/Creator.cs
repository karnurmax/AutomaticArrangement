using Autodesk.Revit.Attributes;
using Autodesk.Revit.Creation;
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
        public Autodesk.Revit.DB.Document Doc { get; private set; }
        public string FamilySymbolName { get; private set; } = "ИП 212-64 прот. R3 ПАСН.425232.038";

        private IArrangementCalculator calculator = null;
        FamilySymbol symbol;
        public void Execute(UIApplication app)
        {

            Transaction transaction = null;
            try
            {
                this.calculator = new SimpleCalculator();

                this.App = app;
                this.Doc = app.ActiveUIDocument.Document;
                using (transaction = new Transaction(Doc))
                {
                    transaction.Start("auto arrangement");
                    this.symbol = GetFamilySymbol();
                    var ids = app.ActiveUIDocument.Selection.GetElementIds().ToList();
                    if (ids == null || ids.Count == 0)
                    {
                        TaskDialog.Show("Ошибка", "Помещения не выбраны");
                        return;
                    }
                    if (symbol == null)
                    {
                        var dr = TaskDialog.Show("Ошибка загрузки", "Семейство не найдено. Выберите путь...", TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.Cancel, TaskDialogResult.Ok);
                        if (dr == TaskDialogResult.Ok)
                        {
                            FileOpenDialog fileOpenDialog = new FileOpenDialog("revit files (*.rfa)|*.rfa|All files (*.*)|*.*");
                            var fodr = fileOpenDialog.Show();
                            if (fodr == ItemSelectionDialogResult.Confirmed)
                            {
                                var model = fileOpenDialog.GetSelectedModelPath();
                                var path = ModelPathUtils.ConvertModelPathToUserVisiblePath(model);
                                var loaded = Doc.LoadFamilySymbol(path, this.FamilySymbolName, out FamilySymbol loadedFS);
                                if (!loaded)
                                {
                                    TaskDialog.Show("Ошибка загрузки семейства", "Не найдено семейство с названием \"ИП 212-64 прот. R3 ПАСН.425232.038\"");
                                    return;
                                }
                                else
                                {
                                    this.symbol = loadedFS;
                                }
                            }
                        }
                    }
                    if (!this.symbol.IsActive)
                        this.symbol.Activate();
                    ids.ForEach(CreateDevices);
                    transaction.Commit();
                }

            }
            catch (Exception ex)
            {
                var s = ex.Message;
                if (transaction != null && transaction.GetStatus() != TransactionStatus.Uninitialized)
                    transaction.RollBack();
            }

        }

        private FamilySymbol GetFamilySymbol()
        {
            FilteredElementCollector collector1 = new FilteredElementCollector(Doc);
            ICollection<Element> collection = collector1.WhereElementIsNotElementType().ToElements();

            return new FilteredElementCollector(Doc)
                         .OfClass(typeof(FamilySymbol))
                         .Cast<FamilySymbol>()
                         .Where(fs => fs.Name.Contains("212-64"))
                         .FirstOrDefault();
        }

        public string GetName()
        {
            return "AutomaticArrangement:Creator";
        }

        private void CreateDevices(ElementId obj)
        {
            var room = this.Doc.GetElement(obj) as Room;
            Rules rules = GetRulesForRoom(room);
            List<XYZ> locations = CalcLocations(room, rules);
            CreateInstancesAndSetLocations(locations, room);
        }

        private void CreateInstancesAndSetLocations(List<XYZ> locations, Room room)
        {
            FilteredElementCollector collector = new FilteredElementCollector(Doc);
            Func<View3D, bool> isNotTemplate = v3 => !(v3.IsTemplate);
            View3D view3D = collector.OfClass(typeof(View3D)).Cast<View3D>().First<View3D>(isNotTemplate);

            BoundingBoxXYZ box = room.get_BoundingBox(view3D);
            XYZ center = box.Min.Add(box.Max).Multiply(0.5);

            // Project in the negative Z direction down to the floor.
            XYZ rayDirection = new XYZ(0, 0, 1);

            ElementClassFilter filter = new ElementClassFilter(typeof(Floor));

            ReferenceIntersector refIntersector = new ReferenceIntersector(filter, FindReferenceTarget.Face, view3D);
            ReferenceWithContext referenceWithContext = refIntersector.FindNearest(center, rayDirection);

            Reference reference = referenceWithContext.GetReference();

            var el = Doc.GetElement(reference);
            var ids = string.Empty;
            var vector = new XYZ(1, 0, 0);
            for (int i = 0; i < locations.Count; i++)
            {
                var fi = App.ActiveUIDocument.Document.Create.NewFamilyInstance(reference, locations[i], vector, this.symbol);
                ids += fi.Id.IntegerValue.ToString() + ";";
            }

        }

        private List<XYZ> CalcLocations(Room room, Rules rules)
        {
            return this.calculator.Calculate(room, rules, this.symbol);
        }

        private Rules GetRulesForRoom(Room room)
        {
            double height = DimensionConverter.FeetToMeter(room.UnboundedHeight) / 10;
            return height < 3.5 ?
            new Rules
            {
                MaxBetweenDevices = DimensionConverter.MeterToFeet(5.0) * 10,
                MaxBetweenDeviceAndWall = DimensionConverter.MeterToFeet(2.5) * 10
            } : new Rules
            {
                MaxBetweenDevices = DimensionConverter.MeterToFeet(4.5) * 10,
                MaxBetweenDeviceAndWall = DimensionConverter.MeterToFeet(2.0) * 10
            };

        }


    }
}
