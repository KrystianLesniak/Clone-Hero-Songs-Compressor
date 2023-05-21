using Octokit;
using SongsCompressor.Common.Consts;
using System.Text.RegularExpressions;

namespace SongsCompressor.Services.Services
{
    public class UpdateChecker
    {
        public string LatestVersionString { get; set; } = string.Empty;

        private readonly Version localVersion;
        public UpdateChecker(Version localVersion)
        {
            this.localVersion = localVersion ?? throw new ArgumentNullException(nameof(localVersion));
        }

        public async Task<bool> CheckForNewGitHubVersion()
        {
            GitHubClient client = new(new ProductHeaderValue("clone-hero-songs-compressor-update-check"));
            Release latestRelease = await client.Repository.Release.GetLatest(AppInfoConsts.RepositoryAuthor, AppInfoConsts.RepositoryName);

            if(latestRelease is null)
                return false;

            LatestVersionString = Regex.Replace(latestRelease.TagName, "[^0-9.]", "");

            Version latestGitHubVersion = new(LatestVersionString);
            int versionComparison = localVersion.CompareTo(latestGitHubVersion);

            if (versionComparison < 0)
            {
                return true;
            }
            return false;
        }
    }
}
