using System.Windows.Interop;
using System.Diagnostics;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

using System.Reflection;
using ExcavationMethod.Revit.Utilities;
using ExcavationMethod.Revit.Application.Resources;

namespace ExcavationMethod.Revit.Application.Buttons.InstallPiles
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class InstallPilesCommand : IExternalCommand
    {
        readonly string ButtonName = "Install Piles";
        readonly string ButtonDescription = "Install Piles with equal spacing along the reference";
        readonly string ButtonIcon = IconsPackX32.Placeholder;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
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
