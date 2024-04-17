using Autodesk.Revit.DB;
using Autodesk.Revit.Creation;

using NPOI.POIFS.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcavationMethod.Revit.Application.Buttons.InstallPiles.Helpers.Extensions
{
    public static class DocumentExtensions
    {
        public static DirectShape CreateDirectShape(
            this Autodesk.Revit.DB.Document doc, 
            List<GeometryObject> geometryObjects,
            BuiltInCategory builtInCategory = BuiltInCategory.OST_GenericModel)
        {
            var directShape = DirectShape.CreateElement(doc, new ElementId(builtInCategory));
            directShape.SetShape(geometryObjects);
            return directShape;
        }
    }
}
