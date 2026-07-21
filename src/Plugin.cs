using BepInEx;
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

    private void Awake()
    {
        Log = Logger;

        Logger.LogInfo("AWAKE START");

        var harmony = new Harmony("sk.ultimatnezabitie");
        harmony.PatchAll();

        Logger.LogInfo("EnglishMappingManager.Load() started");
        EnglishMappingManager.Load();

        Logger.LogInfo("EnglishMappingManager.Load() finished");

        // should log "0-1.levelName"
        string? id = EnglishMappingManager.GetId("INTO THE FIRE");

        Logger.LogInfo(id);

        Logger.LogInfo("TranslationManager.Load() started");
        TranslationManager.Load();
        Logger.LogInfo("TranslationManager.Load() finished");
    }
}