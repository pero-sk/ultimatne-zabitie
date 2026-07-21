// Pulls translation content (mappings + per-language texts) from GitHub at
// startup so translators can push updates without a new plugin release.
//
// This intentionally writes into the exact same local paths that
// LanguageManager / EnglishMappingManager already read from, so nothing else
// in the codebase needs to know or care that the files came from GitHub.
// If a fetch fails for any reason (offline, rate-limited, repo down, etc.)
// we just log a warning and fall back to whatever's already on disk -
// either a previous successful sync, or the copy bundled with the plugin.
using System.Net;
using Newtonsoft.Json.Linq;
using BepInEx;

namespace ultimatne_zabitie;

public static class GithubSync
{
    private sealed class TimeoutWebClient : WebClient
    {
        public int TimeoutMs { get; set; } = 8000;

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            if (request != null)
                request.Timeout = TimeoutMs;
            return request!;
        }
    }

    private static readonly TimeoutWebClient client = new();

    private static string RawBase =>
        $"https://raw.githubusercontent.com/{Plugin.GithubOwner.Value}/{Plugin.GithubRepo.Value}/{Plugin.GithubBranch.Value}";

    public static void SyncAll()
    {
        if (!Plugin.RemoteTranslationsEnabled.Value)
        {
            Plugin.Log.LogInfo("Remote translations disabled (see config), skipping GitHub sync");
            return;
        }

        try
        {
            SyncMapping();
        }
        catch (Exception e)
        {
            Plugin.Log.LogWarning("Failed to sync English mappings from GitHub, using local/cached copy instead.");
            Plugin.Log.LogWarning(e);
        }

        try
        {
            SyncTexts();
        }
        catch (Exception e)
        {
            Plugin.Log.LogWarning("Failed to sync language files from GitHub, using local/cached copies instead.");
            Plugin.Log.LogWarning(e);
        }
    }

    private static void SyncMapping()
    {
        string url = $"{RawBase}/{Plugin.GithubMappingsPath.Value}";

        string localPath = Path.Combine(
            Paths.PluginPath,
            "ultimatne-zabitie",
            "translations",
            "mappings",
            "en_us.json"
        );

        DownloadFile(url, localPath);
    }
    private static void SyncTexts()
    {
        string manifestUrl = $"{RawBase}/{Plugin.GithubTextsPath.Value}/{Plugin.GithubManifestFile.Value}";

        Plugin.Log.LogInfo($"Checking language manifest: {manifestUrl}");

        string manifestJson = client.DownloadString(manifestUrl);
        JArray fileNames = JArray.Parse(manifestJson);

        string localDir = Path.Combine(
            Paths.PluginPath,
            "ultimatne-zabitie",
            "translations",
            "texts"
        );

        Directory.CreateDirectory(localDir);

        foreach (var entry in fileNames)
        {
            string? name = entry.ToString();

            if (string.IsNullOrEmpty(name) || !name.EndsWith(".json"))
                continue;

            string url = $"{RawBase}/{Plugin.GithubTextsPath.Value}/{name}";
            string localPath = Path.Combine(localDir, name);

            DownloadFile(url, localPath);
        }
    }

    private static void DownloadFile(string url, string localPath)
    {
        try
        {
            string content = client.DownloadString(url);

            Directory.CreateDirectory(Path.GetDirectoryName(localPath)!);
            File.WriteAllText(localPath, content);

            Plugin.Log.LogInfo($"Synced: {url} -> {localPath}");
        }
        catch (Exception e)
        {
            Plugin.Log.LogWarning($"Failed to sync {url}, keeping existing local copy (if any).");
            Plugin.Log.LogWarning(e);
        }
    }
}