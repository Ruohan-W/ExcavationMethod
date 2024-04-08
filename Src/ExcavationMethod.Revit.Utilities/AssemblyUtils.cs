using System.Diagnostics;
using System.Reflection;

namespace ExcavationMethod.Revit.Utilities
{
    public static class AssemblyUtils
    {
        public static string GetAddinVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fileVersionInfo.FileVersion;
        }
    }
}
