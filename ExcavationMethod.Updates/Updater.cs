using System;
using System.Threading.Tasks;

namespace ExcavationMethod.Updates
{
    public static class Updater
    {
        public static async Task<bool> CheckUpdateIsAvailble(string currentVersion, IntPtr windowHandle)
        {
            var downloader = new SharePointDownloader(windowHandle);
            var latestVersion = await downloader.GetLatestVersion();
            var existingVersion = new Version(currentVersion);

            if (latestVersion == null)
                return false;
            return latestVersion.Version > existingVersion;
        }

    }
}
