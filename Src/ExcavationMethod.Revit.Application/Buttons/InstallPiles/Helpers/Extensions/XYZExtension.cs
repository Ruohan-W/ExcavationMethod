using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace ExcavationMethod.Revit.Application.Buttons.InstallPiles.Helpers.Extensions
{
    public static class XYZExtension
    {
        public static Autodesk.Revit.DB.Line VisualizeVectorAsLine(
            this XYZ vector, Document document, XYZ? origin = null)
        {
            origin ??= XYZ.Zero;
            var endPoint = origin + vector;
            Autodesk.Revit.DB.Line line = Autodesk.Revit.DB.Line.CreateBound(origin, vector);
            document.CreateDirectShape(new List<GeometryObject>() { line, Point.Create(endPoint) });
            return line;
        }
    }
}
