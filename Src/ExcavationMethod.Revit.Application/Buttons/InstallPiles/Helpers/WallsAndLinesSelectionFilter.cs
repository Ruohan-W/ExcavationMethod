using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcavationMethod.Revit.Application.Buttons.InstallPiles.Helpers
{
    public class WallsAndLinesSelectionFilter<T> : ISelectionFilter where T : Element
    {
        private Autodesk.Revit.DB.Document _doc;
        public Autodesk.Revit.DB.Document? LinkedDocument { get; private set; } = null;

        public WallsAndLinesSelectionFilter(Autodesk.Revit.DB.Document doc)
        {
            _doc = doc;
        }

        public bool FromLink
        {  
            get { return null != LinkedDocument; }
        }
        public bool AllowElement(Element element)
        {
            return true;
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            LinkedDocument = null;
            Element e = _doc.GetElement(refer);

            bool result = false;

            if ( e is RevitLinkInstance)
            {
                RevitLinkInstance linkInstance = (RevitLinkInstance) e;
                LinkedDocument = linkInstance.GetLinkDocument();

                e = LinkedDocument.GetElement(refer.LinkedElementId);
            }

            /*
            if (e is FamilyInstance)
            {
                FamilyInstance eInstance = (FamilyInstance) e;
                result = eInstance.Category.BuiltInCategory is BuiltInCategory.OST_Walls or BuiltInCategory.OST_Lines;
            }
            */
            T? selInstance  = e as T;
            if ( selInstance != null ) 
            {
                result = true;
            }

            return result;
        }
    }
}
