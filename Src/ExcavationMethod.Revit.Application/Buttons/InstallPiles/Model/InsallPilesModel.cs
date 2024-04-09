using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcavationMethod.Revit.Application.Buttons.InstallPiles.Model
{
    public class InstallPilesModel(UIApplication uiApp)
    {
        public UIApplication UiApp = uiApp;

        public void InstallPiles()
        {
            InstallPilesRequestHandler.RequestId = RequestId.InstallPiles;
            InstallPilesRequestHandler.ExternalEventHandler?.Raise();
        }
    }
}
