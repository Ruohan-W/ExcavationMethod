using Autodesk.Revit.DB;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcavationMethod.Revit.Application.Buttons.InstallPiles.DataContext
{
    public partial class WallWrapper : ObservableObject
    {
        public ElementId ElementId { get; }
        public Curve LocationCurve { get; }
        public double WallThickness { get; }

        public WallWrapper(Document doc, Face face)
        {
            FamilyInstance wallInstance = GetWallInstance(doc, face);
            ElementId = wallInstance.Id;
            LocationCurve = GetLocationCurveFromWall(wallInstance);
            WallThickness = GetWallWidth(doc, wallInstance);

        }

        public FamilyInstance GetWallInstance(Document doc, Face f)
        {
            FamilyInstance wallInstance = doc.GetElement(f.Reference) as FamilyInstance;
            return wallInstance;
        }

        public Curve GetLocationCurveFromWall(FamilyInstance wallInstance)
        {
            LocationCurve lc = (LocationCurve)wallInstance.Location;
            Curve c = lc.Curve;

            return c;
        }
        public double GetWallWidth(Document doc, FamilyInstance wallInstance)
        {
            WallType? wallType = doc.GetElement(wallInstance.GetTypeId()) as WallType;
            double nativeWidth = wallType.Width;
            double milimeterWidth = UnitUtils.ConvertFromInternalUnits(nativeWidth, UnitTypeId.Millimeters);

            // convert to inches for imperial unit system if needed.
            string documentUnit = doc.DisplayUnitSystem.ToString().ToLower();
            if (documentUnit.Contains("imperial"))
            {
                milimeterWidth = UnitUtils.ConvertFromInternalUnits(nativeWidth, UnitTypeId.Inches);
            }
             
            return milimeterWidth;
        }
    }
}
