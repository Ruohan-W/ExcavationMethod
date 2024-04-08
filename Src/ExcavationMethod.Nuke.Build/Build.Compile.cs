using ExcavationMethod.Revit.PackageBuilder;
using Nuke.Common;
using Nuke.Common.Tools.MSBuild;
using Serilog;

partial class Build
{
    Target CompileRevit => _ => _
    .TriggeredBy(Clean)
    .Executes(() =>
    {
        AddinBuilder.Create(
            name: "Excavation Method Revit",
            path: Solution.ExcavationMethod_Revit_Application.Directory / "Manifests" / "ExcavationMethod.Revit.Application.addin",
            guid: "6892AA63-C358-41D0-96CE-C834FC43A3C5",
            assembly: $"ExcavationMethod.Revit.Application/{Version}/ExcavationMethod.Revit.Application.dll",
            fullClassName: "ExcavationMethod.Revit.Application.AppCommand"
            );

        foreach (var configuration in SolutionConfigurations)
        {
            if(!configuration.Contains("R2"))
                continue;

            Log.Debug("Building configuration {Configuration}", configuration);

            MSBuildTasks.MSBuild(settings => settings
            .SetTargets("Rebuild")
            .SetConfiguration(configuration)
            .SetProjectFile(Solution.ExcavationMethod_Revit_Application)
            .SetAssemblyVersion(Version)
            .SetVerbosity(MSBuildVerbosity.Minimal)
            .DisableNodeReuse()
            .EnableRestore());
        }
    });
}
