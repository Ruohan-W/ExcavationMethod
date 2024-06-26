﻿using ExcavationMethod.Revit.Application.Buttons.InstallPiles.Helpers;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CommunityToolkit.Mvvm.DependencyInjection;
using ExcavationMethod.Revit.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcavationMethod.Revit.Application.Buttons.InstallPiles
{
    public enum RequestId
    {
        None,
        InstallPiles
    }
    public class InstallPilesRequestHandler : IExternalEventHandler
    {
        public static RequestId RequestId { get; set; }
        public static InstallPilesRequestHandler? RequestHandler { get; set; }
        public static ExternalEvent? ExternalEventHandler { get; set; }

        public InstallPilesHelper Helper = new();
        public void Execute(UIApplication app)
        {
            try
            {
                switch(RequestId)
                {
                    case RequestId.None:
                        return;
                    case RequestId.InstallPiles:
                        InstallPiles(app);
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

        public string GetName() => nameof(InstallPilesRequestHandler);
        public static void RegisterHandler()
        {
            RequestHandler = new InstallPilesRequestHandler();
            ExternalEventHandler = ExternalEvent.Create(RequestHandler);
        }

        private void InstallPiles(UIApplication app) 
        {
            Document doc = app.ActiveUIDocument.Document;

            // select wall from linked file
            // or select 2D or 3D lines from current file

            UIDocument uidoc = new UIDocument(doc);
            Selection choices = uidoc.Selection;
            Helper.SelectElements(uidoc);

            // select available pile familiy from drop down list

            // set the offset and spacing
            // calculate the length + pile size

            // install piles
        }
    }
}
