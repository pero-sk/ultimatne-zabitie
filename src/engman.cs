// ID Mapper
using Newtonsoft.Json;
using BepInEx;

namespace ultimatne_zabitie;

public static class EnglishMappingManager
{
    private static Dictionary<string, Dictionary<string, string>> scopedMappings = new();
    
    private static Dictionary<string, string> defaultMappings = new();

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
        
        try
        {
            var raw = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            
            if (raw != null && raw.TryGetValue("scoped", out var scopedObj))
            {
                scopedMappings = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(scopedObj.ToString()) ?? new();
                Plugin.Log.LogInfo($"Loaded {scopedMappings.Count} scoped mapping sections");
            }
            else
            {
                defaultMappings = JsonConvert.DeserializeObject<Dictionary<string, string>>(json) ?? new();
                Plugin.Log.LogInfo($"Loaded {defaultMappings.Count} default mappings (legacy format)");
            }
        }
        catch
        {
            defaultMappings = JsonConvert.DeserializeObject<Dictionary<string, string>>(json) ?? new();
            Plugin.Log.LogInfo($"Loaded {defaultMappings.Count} default mappings (fallback)");
        }
    }

    public static string? GetId(string englishText, string scope = "default")
    {
        if (scopedMappings.TryGetValue(scope, out var scopeDict))
        {
            if (scopeDict.TryGetValue(englishText, out var id))
                return id;
        }
        
        if (defaultMappings.TryGetValue(englishText, out var defaultId))
            return defaultId;
            
        foreach (var kvp in scopedMappings)
        {
            if (kvp.Value.TryGetValue(englishText, out var id))
                return id;
        }

        return null;
    }
}