using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using AutomaticArrangement.RevitAPI.APIClasses.CommonTools;
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
        public List<XYZ> Calculate(Room room, Rules rules, FamilySymbol fs)
        {
            XYZ firstPoint;
            double roomWidth, roomHeight;
            (firstPoint, roomWidth, roomHeight) = GetFirstPointAndSizes(room);

            //roomHeight /= 10;
            //roomWidth /= 10;

            var fsDiameter = fs.LookupParameter("ADSK_Размер_Диаметр").AsDouble();
            CalculateCountAndIntervalsByVertical(roomHeight, rules, fsDiameter, out double vOffsetFromWall, out int vCount, out double vInterval);
            CalculateCountAndIntervalsByHorizontal(roomWidth, rules, fsDiameter, out double hOffsetFromWall, out int hCount, out double hInterval);

            List<XYZ> result = new List<XYZ>();

            //result.Add(firstPoint);
            //return result;

            double lastY = vCount != 1
                ? firstPoint.Y - vOffsetFromWall
                : (firstPoint.Y - (roomHeight / 2));
            var z = firstPoint.Z;


            for (int i = 0; i < vCount; i++)
            {

                double lastX = vCount != 1
                    ? firstPoint.X + hOffsetFromWall
                    : (firstPoint.X + (roomWidth / 2));
                for (int j = 0; j < hCount; j++)
                {
                    result.Add(new XYZ(lastX, lastY, z));
                    lastX += hInterval;
                }
                lastY -= vInterval;
            }
            return result;

        }

        private (XYZ firstPoint, double roomWidth, double roomHeight) GetFirstPointAndSizes(Room room)
        {
            var allPoints = room.GetBoundarySegments(new SpatialElementBoundaryOptions
            {
                StoreFreeBoundaryFaces = true,
                SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Finish
            })[0]
            .Select(s => s.GetCurve())
            .Aggregate(new List<XYZ>(),
                (a, b) =>
                  {
                      a.Add(b.GetEndPoint(0));
                      return a;
                  });
            var _p = allPoints[0];
            double minX = _p.X, minY = _p.Y, maxX = _p.X, maxY = _p.Y;
            for (int i = 1; i < allPoints.Count; i++)
            {
                var p = allPoints[i];
                var x = p.X;
                var y = p.Y;
                if (x < minX)
                    minX = x;
                if (x > maxX)
                    maxX = x;
                if (y < minY)
                    minY = y;
                if (y > maxY) maxY = y;
            }
            var roomWidth = maxX - minX;
            var roomHeight = maxY - minY;
            return (firstPoint: new XYZ(minX, maxY, _p.Z), roomWidth, roomHeight);
        }

        private void CalculateCountAndIntervalsByVertical(double roomHeight, Rules rules, double elementBBoxHeight, out double vOffsetFromWall, out int vCount, out double vInterval)
        {
            double minArea = roomHeight - (rules.MaxBetweenDeviceAndWall * 2);
            if (minArea <= 0)
            {
                vCount = 1;
                vOffsetFromWall = roomHeight / 2;
                vInterval = 0;
                return;
            }
            double neededAsMinimum = minArea / (10 * (rules.MaxBetweenDevices));
            vCount = (int)neededAsMinimum;
            if (vCount != neededAsMinimum)
                vCount += 1;
            vCount++;
            vOffsetFromWall = rules.MaxBetweenDeviceAndWall;
            vInterval = minArea / (vCount - 1);
        }

        private void CalculateCountAndIntervalsByHorizontal(double roomWidth, Rules rules, double elementBBoxWidth, out double hOffsetFromWall, out int hCount, out double hInterval)
        {
            double minArea = roomWidth - (rules.MaxBetweenDeviceAndWall * 2);
            if (minArea <= 0)
            {
                hCount = 1;
                hOffsetFromWall = roomWidth / 2;
                hInterval = 0;
                return;
            }
            double neededAsMinimum = minArea / (10 * (rules.MaxBetweenDevices));
            hCount = (int)neededAsMinimum;
            if (hCount != neededAsMinimum)
                hCount += 1;
            hCount++;
            hOffsetFromWall = rules.MaxBetweenDeviceAndWall;
            hInterval = minArea / (hCount - 1);

        }
    }
}
