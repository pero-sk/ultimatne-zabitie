using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace ultimatne_zabitie;

[BepInPlugin(
    "sk.ultimatnezabitie",
    "Ultimatné Zabitie",
    "0.1.0"
)]
public class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource Log = null!;

    internal static ConfigEntry<string> LanguageCode = null!;

    internal static ConfigEntry<bool> RemoteTranslationsEnabled = null!;
    internal static ConfigEntry<string> GithubOwner = null!;
    internal static ConfigEntry<string> GithubRepo = null!;
    internal static ConfigEntry<string> GithubBranch = null!;
    internal static ConfigEntry<string> GithubTextsPath = null!;
    internal static ConfigEntry<string> GithubManifestFile = null!;
    internal static ConfigEntry<string> GithubMappingsPath = null!;


    private void Awake()
    {
        Log = Logger;

        try
        {
            Logger.LogInfo("AWAKE START");

            var harmony = new Harmony("sk.ultimatnezabitie");

            Logger.LogInfo("Patching Harmony types individually...");
            var asm = typeof(Plugin).Assembly;
            foreach (var type in asm.GetTypes())
            {
                if (type.GetCustomAttributes(typeof(HarmonyPatch), false).Length == 0)
                    continue;

                Logger.LogInfo($"Patching type: {type.FullName}");
                try
                {
                    var processor = new HarmonyLib.PatchClassProcessor(harmony, type);
                    processor.Patch();
                    Logger.LogInfo($"Patched {type.FullName}");
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Failed patching {type.FullName}: {ex}");
                }
            }
            Logger.LogInfo("Harmony patching complete");

            LanguageCode = Config.Bind(
                "General",                // Section
                "Language",               // Key
                "en_us",                  // Default value
                "Selected language"       // Description
            );

            RemoteTranslationsEnabled = Config.Bind(
                "Remote",
                "Enabled",
                true,
                "Fetch translation files from GitHub on startup instead of only using the copies bundled with the plugin"
            );

            GithubOwner = Config.Bind(
                "Remote",
                "GithubOwner",
                "pero-sk",
                "GitHub account/organization that hosts the translation files"
            );

            GithubRepo = Config.Bind(
                "Remote",
                "GithubRepo",
                "ultimatne-zabitie",
                "GitHub repository that hosts the translation files"
            );

            GithubBranch = Config.Bind(
                "Remote",
                "GithubBranch",
                "main",
                "Branch to pull translation files from"
            );

            GithubTextsPath = Config.Bind(
                "Remote",
                "GithubTextsPath",
                "assets/translations/texts",
                "Repo path containing the per-language translation JSON files"
            );

            GithubManifestFile = Config.Bind(
                "Remote",
                "GithubManifestFile",
                "manifest.json",
                "Filename (inside GithubTextsPath) listing which language JSON files exist, e.g. [\"sk_sk.json\"]"
            );

            GithubMappingsPath = Config.Bind(
                "Remote",
                "GithubMappingsPath",
                "assets/translations/mappings/en_us.json",
                "Repo path to the English text -> translation ID mapping file"
            );

            Logger.LogInfo("Syncing translations from GitHub...");
            GithubSync.SyncAll();
            Logger.LogInfo("GitHub sync complete");

            Logger.LogInfo("Loading languages...");
            LanguageManager.LoadLanguages();
            Logger.LogInfo("Languages loaded");

            Logger.LogInfo("Loading English mappings...");
            EnglishMappingManager.Load();
            Logger.LogInfo("English mappings loaded");

            Logger.LogInfo("Loading translations...");
            TranslationManager.Load();
            Logger.LogInfo("Translations loaded");
        }
        catch (Exception e)
        {
            Logger.LogError("Plugin Awake threw an exception:");
            Logger.LogError(e);
        }
    }
}