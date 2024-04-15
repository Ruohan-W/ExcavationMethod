using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcavationMethod.Revit.Application.Buttons.InstallPiles.Helpers
{
    public class InstallPilesHelper
    {
        #region helper functions for request handler
        // UI selection and filter
        // select one or multiple elements
        public void SelectElements(UIDocument uidoc)
        {
            Document doc = uidoc.Document;
            Selection choices = uidoc.Selection;
            WallsAndLinesSelectionFilter<Wall> selFilter = new WallsAndLinesSelectionFilter<Wall>(doc);

            #region pick single element
            /*
            // Pick one object from Revit.
            Reference pickedElement = choices.PickObject(ObjectType.Element, selFilter, "Select one element");
            if (pickedElement != null)
            {
                TaskDialog.Show("Revit", "One element selected.");
            }
            */
            #endregion


            // Pick multiple object from Revit.
            IList<Reference> pickedElementList = choices.PickObjects(ObjectType.PointOnElement, selFilter, "Select multiple elements");
            if (pickedElementList.Count > 0) 
            {
                IList<ElementId> pickedElemetnIdList = pickedElementList.Select(re => re.ElementId).ToList();
                TaskDialog.Show("Revit", string.Format("{0} element(s) added to selection.", pickedElemetnIdList.Count));
            }

            #region pick multiple elements
            /*
            // use rectangle picking tool 
            IList<Element> pickedElements = choices.PickElementsByRectangle(selFilter, "Select multiple elements by rectangle");
            if(pickedElements.Count > 0)
            {
                IList<ElementId> idsToSelect = pickedElements.Select(e => e.Id).ToList();

                // Update the current selection
                uidoc.Selection.SetElementIds(idsToSelect);
                TaskDialog.Show("Revit", string.Format("{0} element(s) added to selection.", idsToSelect.Count));
            }
            */
            #endregion
        }
        // 
        #endregion
    }
}
