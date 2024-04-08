using Autodesk.Revit.UI;
using Autodesk.Windows;
using ExcavationMethod.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExcavationMethod.Revit.Utilities
{
    public static class RibbonUtils
    {
        public static Dictionary<string, ToolEntry> RibbonItemsToTools(string tabName, bool isThirdParty)
        {
            RibbonControl ribbon = ComponentManager.Ribbon;
            Dictionary<string, ToolEntry> tools = new Dictionary<string, ToolEntry>();

            foreach (RibbonTab tab in (Collection<RibbonTab>)(object)ribbon.Tabs)
            {
                if (tab.Name != tabName)
                { continue; }

                foreach (Autodesk.Windows.RibbonPanel panel in (Collection<Autodesk.Windows.RibbonPanel>)(object)tab.Panels)
                {
                    foreach (Autodesk.Windows.RibbonItem item in (Collection<Autodesk.Windows.RibbonItem>)(object)panel.Source.Items)
                    {
                        if (item.Id != "")
                        {
                            tools.Add(
                                item.Id,
                            new ToolEntry(
                                $"{tab.AutomationName.Replace(" ", "")}-" +
                                $"{panel.Source.Name.Replace(" ", "")}-" +
                                $"{item.AutomationName.Replace("\n", "").Replace("\r", "").Replace(" ", "")}",
                                isThirdParty
                                ));
                        }
                    }
                }
            }

            return tools;
        }

        public static Autodesk.Windows.RibbonItem? GetButton(string tabName, string panelName, string itemName)
        {
            RibbonControl ribbon = ComponentManager.Ribbon;

            foreach(RibbonTab tab in(Collection<RibbonTab>)(object)ribbon.Tabs)
            {
                if (!tab.AutomationName.Equals(tabName))
                { continue; }

                foreach(Autodesk.Windows.RibbonPanel panel in (Collection<Autodesk.Windows.RibbonPanel>)(object)tab.Panels)
                {
                    string selectedPanelName = panel.Source.Title;
                    if(selectedPanelName == panelName)
                    { 
                        return panel.Source.Items.FirstOrDefault(i => i.AutomationName.Equals(itemName)); 
                    }
                }
            }
            return null;
        }
        public static void CreateButton(Autodesk.Revit.UI.RibbonPanel panel, string itemName, string toolTip, string iconName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string? namespaceText = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            var button = new PushButtonData(
                MethodBase.GetCurrentMethod().DeclaringType?.Name,
                itemName,
                assembly.Location,
                MethodBase.GetCurrentMethod().DeclaringType?.FullName
                )
            {
                ToolTip = toolTip,
                LargeImage = ImageUtils.LoadImage(
                    assembly,
                    iconName)
            };
            panel.AddItem( button );
        }
    }
}
