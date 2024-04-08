using Microsoft.Graph;
using Microsoft.Graph.Models;
using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graph.Reports.GetOneDriveActivityFileCountsWithPeriod;

namespace ExcavationMethod.Updates
{
    public class SharePointDownloader
    {
        private const string SiteUrl = "";
        private const string VersionFilePath = "";
        
        private readonly GraphServiceClient _grServiceClient;

        public SharePointDownloader(IntPtr windowHandle)
        {
            var credential = new CustomCredential(windowHandle);
            var scopes = new[] { "https://graph.microsoft.com/.default" };

            _grServiceClient = new GraphServiceClient(credential, scopes);

        }

        public async Task<VersionInfo?> GetLatestVersion()
        {
            var drive = await GetDrive();

            var versionFile = await _grServiceClient
                .Drives[drive.Id]
                .Root
                .ItemWithPath(VersionFilePath)
                .Content
                .GetAsync()
                .ConfigureAwait(false);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var versionInfo = await JsonSerializer.DeserializeAsync<VersionInfo>(versionFile, options);
            return versionInfo;
        }

        public async Task DownloadUpdate(string remoteFilePath, string localFilePath)
        {
            var drive = await GetDrive();

            var updateFile = await _grServiceClient
                .Drives[drive.Id]
                .Root
                .ItemWithPath(remoteFilePath)
                .Content
                .GetAsync()
                .ConfigureAwait(false);

            using var fileStream = File.OpenWrite(localFilePath);
            await updateFile.CopyToAsync(fileStream);
        }
        private async Task<Drive> GetDrive()
        {
            var site = await _grServiceClient
                .Sites[SiteUrl]
                .GetAsync()
                .ConfigureAwait(false);

            var drive = await _grServiceClient
                .Sites[site.Id]
                .Drive
                .GetAsync()
                .ConfigureAwait(false);

            return drive;
        }

    }
}
