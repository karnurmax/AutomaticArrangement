using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomaticArrangement.RevitAPI.APIClasses.CommonTools
{
    public class DimensionConverter
    {
        public static double FeetToMeter(double d)
        {
            return d * 0.3048;
        }
        public static double MeterToFeet(double d)
        {
            return d * 3.28084;
        }
    }
}
