using NuGet.Common;
using Nuke.Common.ProjectModel;
using System.IO.Enumeration;

partial class Build
{
    public Project BuildProject { get; private set; }
    public Project InstallerProject { get; private set; }
    public IEnumerable<string> SolutionConfigurations { get; private set; }

    public string AssemblyName = "ExcavationMethod.Revit.Application";
    public List<string> RevitVersionMatrix = new List<string>()
    {
        "2023",
        "2024",
    };

    protected override void OnBuildInitialized()
    {
        Configurations = new[]
        {
            "Release*",
        };

        BuildProject = Solution.AllProjects.Where(prj => prj.Name.Contains("Build")).FirstOrDefault();
        InstallerProject = Solution.AllProjects.Where(prj => prj.Name.Contains("Installer")).FirstOrDefault();
        SolutionConfigurations = GlobBuildConfigurations();

        IEnumerable<string> GlobBuildConfigurations()
        {
            var solConf = Solution.Configurations;
            var configurations = Solution.Configurations
                .Select(pair => pair.Key)
                .Select(config => config.Remove(config.LastIndexOf('|')))
                .Where(config => config.Contains(Configuration))
                .ToList();

            if (configurations.Count == 0)
                throw new Exception($"No solution configurations have been found. Pattern: {string.Join("|", Configurations)}");

            return configurations;
        }

    }

}
