using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.Tools.GitHub;
using Octokit;
using Serilog;

partial class Build
{
    Target PublishrevitGitHubRelease => _ => _
    .Requires(() => GitHubToken)
    .Requires(() => GitRepository)
    .OnlyWhenStatic(() => IsServerBuild && GitRepository.IsOnMainOrMasterBranch())
    .TriggeredBy(CreateArtifacts)
    .Executes(() =>
    {
        Log.Information("Publish a release of the installer on GitHub");
        GitHubTasks.GitHubClient = new GitHubClient(new ProductHeaderValue(Solution.Name))
        {
            Credentials = new Credentials(GitHubToken)
        };

        var gitHubName = GitRepository.GetGitHubName();
        var gitHubOwner = GitRepository.GetGitHubOwner();
        var artifacts = Directory.GetFiles(ArtifactsDirectory, "*");

        Log.Information($"Detected Version: {Version}");
        var newRelease = new NewRelease(Version)
        {
            Name = Version,
            Draft = true,
            GenerateReleaseNotes = true,
        };
        var draft = CreatedDraft(gitHubOwner, gitHubName, newRelease);
    });

    static Release CreatedDraft(string gitHubOwner, string gitHubName, NewRelease newRelease) =>
        GitHubTasks.GitHubClient.Repository.Release
        .Create(gitHubOwner, gitHubName, newRelease)
        .Result;

    static void UploadArtifacts(Release createdRelease, IEnumerable<string> artifacts)
    {
        foreach (var file in artifacts)
        {
            var releaseAssetUpload = new ReleaseAssetUpload
            {
                ContentType = "application/x-binary",
                FileName = Path.GetFileName(file),
                RawData = File.OpenRead(file)
            };
            var _ = GitHubTasks.GitHubClient.Repository.Release.UploadAsset(createdRelease, releaseAssetUpload).Result;
            Log.Information("Added artifact: {Path}", file);
        }
    }

    static void ReleaseDraft(string gitHubOwner, string gitHubName, Release draft)
    {
        var _ = GitHubTasks.GitHubClient.Repository.Release
            .Edit(gitHubOwner, gitHubName, draft.Id, new ReleaseUpdate { Draft = false})
            .Result;
    }
}
