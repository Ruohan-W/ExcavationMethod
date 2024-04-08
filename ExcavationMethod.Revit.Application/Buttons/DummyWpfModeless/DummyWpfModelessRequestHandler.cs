using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ExcavationMethod.Revit.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcavationMethod.Revit.Application.Buttons.DummyWpfModeless
{
    public enum RequestId
    {
        None,
        DoSomething
    }
    public class DummyWpfModelessRequestHandler : IExternalEventHandler
    {
        public static RequestId RequestId { get; set; }
        public static DummyWpfModelessRequestHandler? RequestHandler { get; set; }
        public static ExternalEvent? ExternalEventHandler { get; set; }
        public void Execute(UIApplication app)
        {
            try 
            {
                switch(RequestId)
                {
                    case RequestId.None:
                        return;
                    case RequestId.DoSomething:
                        DoSomething(app);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception ex) 
            { 
                // ignore
            }
        }

        public string GetName() => nameof(DummyWpfModelessRequestHandler);

        public static void RegisterHandler()
        {
            RequestHandler = new DummyWpfModelessRequestHandler();
            ExternalEventHandler = ExternalEvent.Create(RequestHandler);
        }

        private void DoSomething(UIApplication app) 
        {
            var doc = app.ActiveUIDocument.Document;
            var count = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Walls)
                .WhereElementIsNotElementType()
                .ToElementIds()
                .Count();

            TaskDialogUtils.Info($"We have {count} walls in the model");
        }
    }
}
