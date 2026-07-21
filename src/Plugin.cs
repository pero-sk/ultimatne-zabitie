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