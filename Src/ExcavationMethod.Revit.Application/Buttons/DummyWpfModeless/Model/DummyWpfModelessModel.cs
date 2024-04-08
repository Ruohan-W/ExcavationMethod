using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcavationMethod.Revit.Application.Buttons.DummyWpfModeless.Model
{
    public class DummyWpfModelessModel(UIApplication uiApp)
    {
        public UIApplication UiApp = uiApp;

        public void DoSomething() 
        {
            DummyWpfModelessRequestHandler.RequestId = RequestId.DoSomething;
            DummyWpfModelessRequestHandler.ExternalEventHandler.Raise();
        }
    }
}
