using ExcavationMethod.Revit.Installer;
using InnoSetup.ScriptBuilder;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.InnoSetup;
using Serilog;
using System.IO.Compression;
using System.Security.Policy;

partial class Build
{
    string RevitAppName = "Excavation Method for Revit";
    string RevitAppNameShort = "ExcavationMethod-Revit";

    Target CreateArtifacts => _ => _
    .TriggeredBy(CompileRevit)
    .OnlyWhenStatic(() => IsLocalBuild || GitRepository.IsOnMainBranch())
    .Executes(() =>
    {
        Log.Information($"Creating the zip file ...");
        Directory.CreateDirectory(ArtifactsDirectory);
        RevitArtifactsDirectory.ZipTo(ArtifactsDirectory / $"{RevitAppNameShort}-{Version}.zip", compressionLevel: CompressionLevel.Optimal, fileMode: FileMode.CreateNew);

        Log.Information("Building installer for Revit");
        string[] folders = Directory.GetDirectories(RevitArtifactsDirectory);
        List<(string source, string destination)> setupFolders = new();

        foreach (var folder in folders)
        {
            string dirName = Path.GetFileName(folder);
            setupFolders.Add(($@"{dirName}\*", $@"{{userappdata}}\\Autodesk\Revit\Addins\{dirName}"));
        }

        var builder = new InstallerBuilder(
            appName: RevitAppName,
            version: Version,
            outputDir: ArtifactsDirectory,
            outputFileName: RevitAppNameShort,
            icon: RootDirectory / "media" / "aecom.ico",
            folders: setupFolders
            );

        var result = builder.ToString();
        var iss = RevitArtifactsDirectory / $"{RevitAppNameShort}-setup-config.iss";

        builder.Build(iss);

        InnoSetupTasks.InnoSetup(_ => _
        .SetScriptFile(iss)
        .SetProcessToolPath((AbsolutePath)Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) / "Inno Setup 6" / "ISCC.exe")
        );

        if (IsLocalBuild)
        {
            // Delete the local folders created by the build process.
            RevitVersionMatrix.ForEach(v =>
            {
                var path = ((AbsolutePath)Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) /
                "Autodesk" / "Revit" / "Addins" / v / AssemblyName;
                path.DeleteDirectory();

                var addinFilePath = ((AbsolutePath)Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) /
                "Autodesk" / "Revit" / "Addins" / v / AssemblyName + ".addin";
                addinFilePath.DeleteFile();
            });
        }
    });
}
