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
        public void SelectElements(
            UIDocument uidoc, 
            Selection choices, 
            List<BuiltInCategory>? bICList = null, 
            List<ElementReferenceType>? eRTList = null)
        {
            Document doc = uidoc.Document;
            ElementAndLinkElementSelectionFilter selFilter = new ElementAndLinkElementSelectionFilter(doc);
            if(bICList != null)
            {
                selFilter.ElementReferenceTypeMask.AddRange(eRTList);
            }
            if(eRTList !=null)
            {
                selFilter.BuiltInCategoryMask.AddRange(bICList);
            }    

            // Pick multiple object from Revit.
            List<Reference> pickedElementList = choices.PickObjects(ObjectType.PointOnElement, selFilter, "Select wall or curve elements.").ToList();
            if (pickedElementList.Count > 0) 
            {
                List<ElementId> pickedElemetnIdList = pickedElementList.Select(re => re.ElementId).ToList();
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
                    Curve topCurve = GetTopEdgeFromSurfaceReference(r, doc);
                    // visualize all the elements above
                    using (var transaction = new Transaction(doc, "Visulaize elements"))
                    {
                        transaction.Start();
                        DirectShape directShape = doc.CreateDirectShape([topCurve]);
                        SetElementColor(doc, new List<ElementId>() { directShape.Id }, new Color(255, 0, 0));
                        transaction.Commit();
                    }
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
                .GroupBy(c => Math.Round(Math.Min(c.GetEndPoint(0).Z, c.GetEndPoint(1).Z),6))
                .OrderBy(g => g.Key)
                .LastOrDefault()
                .Select(g => g)
                .ToList();

            if (topEdgesAsCurves.Count == 1)
            {
                return topEdgesAsCurves[0];
            }
            else
            {
                // get center line / location line from wall instance.
                Curve locationCurve = (doc.GetElement(r).Location as LocationCurve)!.Curve;
                if (locationCurve is Line)
                {
                    List<XYZ> endPoints = topEdgesAsCurves
                        .SelectMany(c => GetEndPoints(c))
                        .ToList();                
                    endPoints = RemoveDuplicatedXYZ(endPoints); // get rid of duplicate XYZ from list

                    double highestElevation = endPoints.Max(c => c.Z);
                    List<XYZ> coords = GetFarestCoordinateFromPoints(endPoints)
                        .Select(pt =>  new XYZ(pt.X, pt.Y, highestElevation))
                        .ToList();
                    Curve simplifiedTopEdgeLine = Line.CreateBound(coords[0], coords[1]);
                    return simplifiedTopEdgeLine;
                }
                else
                {                 
                    List<Arc> arcList = topEdgesAsCurves.Select(c => (Arc)c).ToList();
                    Arc simplifiedArc = GetArcFromArcSegement(arcList, doc, locationCurve);
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
        public Arc GetArcFromArcSegement(List<Arc> arcList, Document doc, Curve locationCurve)
        {
            XYZ center = ((Arc)locationCurve).Center;
            List<XYZ> endCoords = arcList.SelectMany(c => GetEndPoints(c)).ToList();
            endCoords = RemoveDuplicatedXYZ(endCoords)
                .OrderBy(c => locationCurve.Project(c).Parameter)
                .ToList();

            double highestElevation = endCoords.Max(c => c.Z);
            List<XYZ> pontsOnArcList = [endCoords.First(), endCoords.Last(), endCoords[1]];
            List<XYZ> pointsOnArcAtHighestElevation = pontsOnArcList.Select(pt => new XYZ(pt.X, pt.Y, highestElevation)).ToList();

            Arc simplifiedArc = Arc.Create(pointsOnArcAtHighestElevation[0], pointsOnArcAtHighestElevation[1], pointsOnArcAtHighestElevation[2]);

            return simplifiedArc;
        }
        public void SetElementColor(Document doc, List<ElementId> elementIds, Color? color)
        {
            color ??= new Color(255, 0, 0);
            OverrideGraphicSettings ogs = new OverrideGraphicSettings();
            ogs.SetProjectionLineColor(color);
            ogs.SetProjectionLineWeight(8);
            ogs.SetSurfaceForegroundPatternColor(color);
            ogs.SetCutLineColor(color);
            ogs.SetCutLineWeight(8);
            ogs.SetCutForegroundPatternColor(color);

            elementIds.ForEach(e => doc.ActiveView.SetElementOverrides(e, ogs));
        }
        public List<XYZ> RemoveDuplicatedXYZ(List<XYZ> xYZList)
        {
            List<XYZ> xYZWithoutDupList = [xYZList[0]];
            for(int i = 1; i < xYZList.Count; i++)
            {
                XYZ coordA = xYZList[i];
                bool containSameXYZ = xYZWithoutDupList.Select(coord => coord.IsAlmostEqualTo(coordA, 1.0e-6))
                    .Contains(true);
                if(!containSameXYZ)
                {
                    xYZWithoutDupList.Add(coordA);
                }
            }
            return xYZWithoutDupList;
        }

        #endregion
    }
}