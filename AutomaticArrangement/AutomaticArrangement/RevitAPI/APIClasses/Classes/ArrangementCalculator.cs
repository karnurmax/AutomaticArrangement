using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using AutomaticArrangement.RevitAPI.APIClasses.Interfaces;
using AutomaticArrangement.RevitAPI.APIClasses.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticArrangement.RevitAPI.APIClasses.Classes
{
    public class SimpleCalculator : IArrangementCalculator
    {
        public List<LocationPoint> Calculate(Room room, Rules rules)
        {
            throw new NotImplementedException();
        }
    }
}
