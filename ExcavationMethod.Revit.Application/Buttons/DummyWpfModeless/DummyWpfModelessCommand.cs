using System.Windows.Interop;
using System.Diagnostics;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

using ExcavationMethod.Revit.Application.Buttons.DummyWpfModeless.Model;
using ExcavationMethod.Revit.Application.Buttons.DummyWpfModeless.ViewModel;
using ExcavationMethod.Revit.Application.Buttons.DummyWpfModeless.View;
using System.Reflection;
using ExcavationMethod.Revit.Utilities;
using ExcavationMethod.Revit.Application.Resources;

namespace ExcavationMethod.Revit.Application.Buttons.DummyWpfModeless
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class DummyWpfModelessCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try 
            {
                // Initialize the linke between [model] => [view model] => view
                var uiApp = commandData.Application;
                var m = new DummyWpfModelessModel(uiApp);
                var vm = new DummyWpfModelessViewModel(m);
                var v = new DummyWpfModelessView
                {
                    DataContext = vm,
                };

                // close windows if Revit is closed
                var unused = new WindowInteropHelper(v)
                {
                    Owner = Process.GetCurrentProcess().MainWindowHandle
                };

                v.Show();

                return Result.Succeeded;
            }
            catch (Exception ex) 
            {
                return Result.Failed;
            }
        }

        public static void CreateButton(RibbonPanel panel)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            PushButtonData button = new PushButtonData(
                MethodBase.GetCurrentMethod().DeclaringType?.Name,
                "Dummy Wpf\nModeless",
                assembly.Location,
                MethodBase.GetCurrentMethod().DeclaringType?.FullName
                )
            {
                ToolTip = "toolTip goes here",
                LargeImage = ImageUtils.LoadImage(
                    assembly,
                    IconsPackX32.Placeholder)
            };
            panel.AddItem( button );
        }
    }
}
