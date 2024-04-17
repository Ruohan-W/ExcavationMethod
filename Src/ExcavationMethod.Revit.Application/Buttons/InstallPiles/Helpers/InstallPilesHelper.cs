using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CommunityToolkit.Mvvm.DependencyInjection;
using ExcavationMethod.Revit.Application.Buttons.InstallPiles.Helpers.Extensions;
using Microsoft.Graph.Models;
using NPOI.OpenXmlFormats.Dml;
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
        public void SelectElements(UIDocument uidoc, List<BuiltInCategory> bICList, Selection choices)
        {
            Document doc = uidoc.Document;
            ElementAndLinkElementSelectionFilter selFilter = new ElementAndLinkElementSelectionFilter(doc);
            selFilter.BuiltInCategoryMask.AddRange(bICList);

            // Pick multiple object from Revit.
            List<Reference> pickedElementList = choices.PickObjects(ObjectType.PointOnElement, selFilter, "Select wall or curve elements.").ToList();
            if (pickedElementList.Count > 0) 
            {
                List<ElementId> pickedElemetnIdList = pickedElementList.Select(re => re.ElementId).ToList();
                //HighlightElement(uidoc, pickedElemetnIdList);
                GetCurveFromReferences(doc, pickedElementList);
                TaskDialog.Show("Revit", string.Format("{0} element(s) added to selection.", pickedElemetnIdList.Count));
            }

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
        
        public void GetCurveFromReferences(Document doc, List<Reference> rList)
        {
            var rTypeNameList = rList.Select(r => r.ElementReferenceType.ToString())
                .Select(rTypeName => rTypeName.Substring(rTypeName.LastIndexOf("_")+1).ToLower())
                .ToList();

            for(int i = 0; i < rTypeNameList.Count; i++)
            {
                Reference r = rList[i];
                Element e = doc.GetElement(r.ElementId);
                BoundingBoxXYZ bb = e.get_BoundingBox(null);
                XYZ refPt = bb.Max;

                string rTName = rTypeNameList[i];
                if (rTName.Contains("surface"))
                {
                    Face face = (Face)doc.GetElement(r).GetGeometryObjectFromReference(r);
                    // get center line / location line from wall instance.
                    Curve c = (doc.GetElement(r).Location as LocationCurve)!.Curve;

                    XYZ projectedRefPt = c.Project(refPt).XYZPoint;
                    // XYZ refVector = (refPt - projectedRefPt).Normalize();
                    double offsetLength = (refPt - projectedRefPt).GetLength();
                    var offsetCurve = c.CreateOffset(offsetLength, XYZ.BasisZ);

                    // visualize all the elements above
                    using (var transaction = new Transaction(doc, "Visulaize elements"))
                    {
                        transaction.Start();
                        doc.CreateDirectShape(new List<GeometryObject>() { c, Point.Create(refPt), Point.Create(projectedRefPt) });
                        transaction.Commit();
                    }

                    // how to get to the curve? 
                    /*
                    // get all edges from face, then found the one with largest Z at its bouding box.
                    IList<Curve> curvesFromEdge = new List<Curve>();
                    foreach (EdgeArray eArr in face.EdgeLoops)
                    {
                        foreach(Autodesk.Revit.DB.Edge edge in eArr)
                        {
                            curvesFromEdge.Add(edge.AsCurve());
                        }
                    }
                    Curve topEdgeCurve = curvesFromEdge.Where(c => c.IsBound)
                        .OrderBy(c => c.GetEndPoint(0).Z > c.GetEndPoint(1).Z ? c.GetEndPoint(1) : c.GetEndPoint(0))
                        .Reverse()
                        .FirstOrDefault();
                    */

                    // hilghest point of the bounding box of wall.

                    // offset curve to the selected edage.

                }
                if(rTName == "linear")
                {
                    Curve? curve = doc.GetElement(r).GetGeometryObjectFromReference(r) as Curve;
                }

            }
            // return curve
        }

        #endregion
    }
}
