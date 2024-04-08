using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcavationMethod.Revit.Utilities
{
    public class TaskDialogUtils
    {
        public static TaskDialogResult Info(string message)
        {
            return new TaskDialog("Info")
            {
                MainContent = message,
                MainIcon = TaskDialogIcon.TaskDialogIconInformation,
                CommonButtons = TaskDialogCommonButtons.Close
            }.Show();
        }

        public static TaskDialogResult Warning(string message)
        {
            return new TaskDialog("Warning")
            {
                MainContent = message,
                MainIcon = TaskDialogIcon.TaskDialogIconWarning,
                CommonButtons = TaskDialogCommonButtons.Close
            }.Show();
        }

        public static TaskDialogResult Error(string message)
        {
            return new TaskDialog("Error")
            {
                MainContent = message,
                MainIcon = TaskDialogIcon.TaskDialogIconError,
                CommonButtons = TaskDialogCommonButtons.Close
            }.Show();
        }
    }
}
