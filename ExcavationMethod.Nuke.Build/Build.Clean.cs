using Nuke.Common;
using Nuke.Common.IO;
using Serilog;
using static Nuke.Common.IO.FileSystemTasks;

partial class Build
{
    Target Clean => _ => _
    .OnlyWhenStatic(() => IsLocalBuild)
    .Executes(() =>
    {
        ArtifactsDirectory.CreateOrCleanDirectory();
        RevitArtifactsDirectory.CreateOrCleanDirectory();

        foreach (var project in Solution.AllProjects.Where(project => project != Solution.ExcavationMethod_Nuke_Build))
            CleanDirectory(project.Directory / "bin");
    });

    static void CleanDirectory(AbsolutePath path)
    {
        Log.Information("Cleaning directory: {Directory}", path);
        path.CreateOrCleanDirectory();
    }
}
