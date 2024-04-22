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
    public class ElementAndLinkElementSelectionFilter : ISelectionFilter
    {
        private Autodesk.Revit.DB.Document _doc;
        public List<BuiltInCategory> BuiltInCategoryMask { get; set; }
        public List<ElementReferenceType> ElementReferenceTypeMask { get; set; }
        public Autodesk.Revit.DB.Document? LinkedDocument { get; private set; } = null;

        public bool FromLink
        {
            get { return null != LinkedDocument; }
        }

        public ElementAndLinkElementSelectionFilter(Autodesk.Revit.DB.Document doc)
        {
            _doc = doc;
            BuiltInCategoryMask = new List<BuiltInCategory>();
            ElementReferenceTypeMask = new List<ElementReferenceType>();
        }

        public bool AllowElement(Element element)
        {
            return true;
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            bool result = false;

            LinkedDocument = null;
            Element e = _doc.GetElement(refer);

            ElementReferenceType eleRT = refer.ElementReferenceType;
            if (ElementReferenceTypeMask.Count > 0 && !ElementReferenceTypeMask.Contains(eleRT))
            {
                return result;
            }

            if ( e != null && e is RevitLinkInstance)
            {
                RevitLinkInstance linkInstance = (RevitLinkInstance) e;
                LinkedDocument = linkInstance.GetLinkDocument();

                e = LinkedDocument.GetElement(refer.LinkedElementId);
            }

            if (e?.Category == null)
            {
                return result; 
            }

            BuiltInCategory elemBIC = e.Category.BuiltInCategory;
            if(BuiltInCategoryMask.Count>0 && BuiltInCategoryMask.Contains(elemBIC))
            {
                result = true;
            }

            return result;
        }
    }
}
