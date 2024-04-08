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
        readonly string ButtonName = "Dummy Wpf\nModeless";
        readonly string ButtonDescription = "toolTip description goes here";
        readonly string ButtonIcon = IconsPackX32.Placeholder;
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
            DummyWpfModelessCommand command = new DummyWpfModelessCommand();
            string buttonName = command.ButtonName;
            string buttonDescription = command.ButtonDescription;
            string buttonIcon = command.ButtonIcon;

            Assembly assembly = Assembly.GetExecutingAssembly();
            string? methodName = MethodBase.GetCurrentMethod().DeclaringType?.Name;
            string? methodFullName = MethodBase.GetCurrentMethod().DeclaringType?.FullName;

            if(methodName != null && methodFullName != null)
            {
                PushButtonData button = RibbonUtils.FillPushButtonData(assembly, methodName, buttonName, methodFullName, buttonDescription, buttonIcon);
                panel.AddItem(button);
            }
        }
    }
}
