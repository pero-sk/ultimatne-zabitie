using System.Collections.Generic;
using Newtonsoft.Json;
using BepInEx;

namespace ultimatne_zabitie;

public static class EnglishMappingManager
{
    private static Dictionary<string, string> mappings = new();

    public static void Load()
    {
        string path = Path.Combine(
            Paths.PluginPath,
            "ultimatne-zabitie",
            "translations",
            "mappings",
            "en_us.json"
        );

        Plugin.Log.LogInfo($"Loading English mappings: {path}");

        if (!File.Exists(path))
        {
            Plugin.Log.LogError("English mapping file missing!");
            return;
        }

        string json = File.ReadAllText(path);

        mappings =
            JsonConvert.DeserializeObject<Dictionary<string,string>>(json)
            ?? new();

        Plugin.Log.LogInfo(
            $"Loaded {mappings.Count} English mappings"
        );
    }


    public static string? GetId(string englishText)
    {
        if (mappings.TryGetValue(englishText, out var id))
            return id;

        return null;
    }
}