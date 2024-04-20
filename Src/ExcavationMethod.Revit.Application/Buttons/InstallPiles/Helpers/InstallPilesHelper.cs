using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CommunityToolkit.Mvvm.DependencyInjection;
using ExcavationMethod.Revit.Application.Buttons.InstallPiles.Helpers.Extensions;
using Microsoft.Graph.Models;
using NPOI.OpenXmlFormats.Dml;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
                string rTName = rTypeNameList[i];
                if (rTName.Contains("surface"))
                {
                    /*
                    // get center line / location line from wall instance.
                    Curve c = (doc.GetElement(r).Location as LocationCurve)!.Curve;

                    // get all edges from face, then found the one with largest Z at its bouding box.
                    Face face = (Face)doc.GetElement(r).GetGeometryObjectFromReference(r);
                    IList<Curve> curvesFromEdge = new List<Curve>();
                    foreach (EdgeArray eArr in face.EdgeLoops)
                    {
                        foreach (Autodesk.Revit.DB.Edge edge in eArr)
                        {
                            curvesFromEdge.Add(edge.AsCurve());
                        }
                    }
                    XYZ endPointAtTopEdgeCurve = curvesFromEdge.Where(c => c.IsBound)
                        .OrderBy(c => c.GetEndPoint(0).Z > c.GetEndPoint(1).Z ? c.GetEndPoint(1).Z : c.GetEndPoint(0).Z)
                        .Reverse()
                        .FirstOrDefault()
                        .GetEndPoint(0);


                    double curveZ = c.GetEndPoint(0).Z;
                    double endPointAtTopEdgeCurveZ = endPointAtTopEdgeCurve.Z;
                    XYZ transferVector = new XYZ(0, 0, curveZ - endPointAtTopEdgeCurveZ);
                    XYZ refPt = endPointAtTopEdgeCurve + transferVector;

                    XYZ projectedRefPt = c.Project(refPt).XYZPoint;
                    // XYZ refVector = (refPt - projectedRefPt).Normalize();
                    double offsetLength = (refPt - projectedRefPt).GetLength();
                    var offsetCurve = c.CreateOffset(offsetLength, XYZ.BasisZ);
                    */

                    Curve topCurve = GetTopEdgeFromSurfaceReference(r, doc);
                    // visualize all the elements above
                    using (var transaction = new Transaction(doc, "Visulaize elements"))
                    {
                        transaction.Start();
                        doc.CreateDirectShape([topCurve]);
                        transaction.Commit();
                    }

                    // how to get to the curve? 
                    /*

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

        public Curve GetTopEdgeFromSurfaceReference(Reference r, Document doc)
        {
            EdgeArrayArray edgeArrayArray = ((Face)doc.GetElement(r).GetGeometryObjectFromReference(r))!.EdgeLoops;
            IList<Curve> edgesAsCurves = new List<Curve>();
            foreach (EdgeArray eArr in edgeArrayArray)
            {
                foreach (Autodesk.Revit.DB.Edge edge in eArr)
                {
                    edgesAsCurves.Add(edge.AsCurve());
                }
            }
            List<Curve> topEdgesAsCurves = edgesAsCurves.Where(c => c.IsBound)
                .GroupBy(c => c.GetEndPoint(0).Z > c.GetEndPoint(1).Z ? c.GetEndPoint(1).Z : c.GetEndPoint(0).Z)
                .OrderBy(g => g.Key)
                .FirstOrDefault()
                .Select(g => g)
                .ToList();

            if (topEdgesAsCurves.Count == 1)
            {
                return topEdgesAsCurves[0];
            }
            else
            {     
                Curve firstCurve = topEdgesAsCurves[0]; 
                if(firstCurve is Line)
                {
                    List<XYZ> endPoints = topEdgesAsCurves.SelectMany(c => GetEndPoints(c)).ToList();
                    List<XYZ> coords = GetFarestCoordinateFromPoints(endPoints);
                    Curve simplifiedTopEdgeLine = Line.CreateBound(coords[0], coords[1]);
                    return simplifiedTopEdgeLine;
                }
                else
                {
                    List<Arc> arcList = topEdgesAsCurves.Select(c => (Arc)c).ToList();
                    Arc simplifiedArc = GetArcFromArcSegement(arcList);
                    return simplifiedArc as Curve;
                }
            }
        }

        public List<XYZ> GetEndPoints( Curve c)
        {
            return [c.GetEndPoint(0), c.GetEndPoint(1)];
        }

        public List<XYZ> GetFarestCoordinateFromPoints(List<XYZ> coordList)
        {
            List<Line> lineList = new List<Line>();
            for(int i = 0; i < coordList.Count - 1; i++)
            {
                XYZ coordStart = coordList[i];
                for (int j = i+1; j < coordList.Count; j++)
                {
                    XYZ coordEnd = coordList[j];
                    Line line = Line.CreateBound(coordStart, coordEnd);
                    lineList.Add(line);
                }
            }

            Line longestLine = lineList.OrderBy(l => l.Length)
                .LastOrDefault();
            List<XYZ> coords = GetEndPoints(longestLine);

            return coords;
        }

        public Arc GetArcFromArcSegement(List<Arc> arcList)
        {
            XYZ center = arcList[0].Center;
            List<XYZ> endCoords = arcList.SelectMany(c => GetEndPoints(c))
                .Select(pt=> pt - center)
                .ToList();

            List<Tuple<double, List<XYZ>>> anglesAndCoords = new List<Tuple<double, List<XYZ>>>();  
            for(int i  = 0; i < endCoords.Count - 1; i++)
            {
                XYZ endCoordA = endCoords[i];
                XYZ vectorA = endCoordA - center;
                for (int j = i+1; j < endCoords.Count; j++)
                {
                    XYZ endCoordB = endCoords[j];
                    XYZ vectorB = endCoordB - center;

                    double angle = vectorA.AngleTo(vectorB);
                    List<XYZ> startPtAndEndPt = [endCoordA, endCoordB];
                    anglesAndCoords.Add( new Tuple<double, List<XYZ>>(angle, startPtAndEndPt));
                }
            }
            List<XYZ> startAndEndOfSimplifiedArc = anglesAndCoords.OrderBy(t => t.Item1)
                .LastOrDefault()
                .Item2;

            Arc simplifiedArc = Arc.Create(startAndEndOfSimplifiedArc[0], center, startAndEndOfSimplifiedArc[1]);
            return simplifiedArc;
        }

        #endregion
    }
}