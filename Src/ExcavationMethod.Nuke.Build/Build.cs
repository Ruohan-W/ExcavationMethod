using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;

partial class Build : NukeBuild
{
    string[] Configurations;
    Dictionary<Project, Project> InstallersMap;
    Dictionary<string, string> VersionMap;
    [Parameter("The build version")] string Version;
    [Parameter("Debug | Release")] string Configuration;

    readonly AbsolutePath ArtifactsDirectory = RootDirectory / "artifacts";
    readonly AbsolutePath DistDirectory = RootDirectory / "dist";
    readonly AbsolutePath RevitArtifactsDirectory = RootDirectory / "dist" / "Revit";
    readonly AbsolutePath ChangeLogPath = RootDirectory / "Changelog.md";

    [GitRepository] readonly GitRepository GitRepository;
    [Parameter] string GitHubToken { get; set; }
    //[GitVersion(NoFetch = true)] readonly GitVersion GitVersion;

    [Solution(GenerateProjects = true)] Solution Solution;

    public static int Main() => Execute<Build>(x => x.Clean);
}
