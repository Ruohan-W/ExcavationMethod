using InnoSetup.ScriptBuilder;
using System.Collections.Generic;

namespace ExcavationMethod.Revit.Installer
{
    public class InstallerBuilder : IssBuilder
    {
        public InstallerBuilder(
            string appName,
            string outputFileName,
            string outputDir,
            string version,
            string icon,
            List<(string source, string destination)> folders
            ) 
        {
            Setup.Create(appName)
                .AppVersion(version)
                .OutputDir(outputDir)
                .OutputBaseFilename($"{outputFileName}-{version}")
                .AppVerName(appName)
                .DefaultDirName(@"{userappdata}\Autodesk\Revit\Addins")
                .PrivilegesRequired(PrivilegesRequired.Lowest)
                .SetupIconFile(icon)
                .UninstallDisplayIcon(icon)
                .DisableDirPage(YesNo.Yes)
                .WizardStyle(WizardStyle.Modern)
                .Compression("lzma")
                .SolidCompression(YesNo.Yes);

            foreach (var folder in folders)
            {
                Files.CreateEntry(
                    source: folder.source,
                    destDir: folder.destination)
                    .Flags(FileFlags.IgnoreVersion | FileFlags.RecurseSubdirs | FileFlags.CreateAllSubdirs);
            }
        }

    }
}
