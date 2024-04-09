using System.Windows.Interop;
using System.Diagnostics;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

using System.Reflection;
using ExcavationMethod.Revit.Utilities;
using ExcavationMethod.Revit.Application.Resources;
using ExcavationMethod.Revit.Application.Buttons.InstallPiles.Model;
using ExcavationMethod.Revit.Application.Buttons.InstallPiles.ViewModel;
using ExcavationMethod.Revit.Application.Buttons.InstallPiles.View;

namespace ExcavationMethod.Revit.Application.Buttons.InstallPiles
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class InstallPilesCommand : IExternalCommand
    {
        readonly string ButtonName = "Install Piles";
        readonly string ButtonDescription = "Install Piles with equal spacing along the reference";
        readonly string ButtonIcon = IconsPackX32.InstallPiles;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                // initialize the link between [model] => [view model] => [view]
                var uiApp = commandData.Application;
                var m = new InstallPilesModel(uiApp);
                var vm = new InstallPilesViewModel(m);
                var v = new InstallPilesView
                {
                    DataContext = vm,
                };

                v.Show();

                // close when Revit is closed
                var unused = new WindowInteropHelper(v)
                {
                    Owner = Process.GetCurrentProcess().MainWindowHandle
                };

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                return Result.Failed;
            }
        }

        public static void CreateButton(RibbonPanel panel)
        {
            InstallPilesCommand command = new InstallPilesCommand();
            string buttonName = command.ButtonName;
            string buttonDescription = command.ButtonDescription;
            string buttonIcon = command.ButtonIcon;

            Assembly assembly = Assembly.GetExecutingAssembly();
            string? methodName = MethodBase.GetCurrentMethod().DeclaringType?.Name;
            string? methodFullName = MethodBase.GetCurrentMethod().DeclaringType?.FullName;

            if (methodName != null && methodFullName != null)
            {
                PushButtonData button = RibbonUtils.FillPushButtonData(assembly, methodName, buttonName, methodFullName, buttonDescription, buttonIcon);
                panel.AddItem(button);
            }
        }
    }
}
