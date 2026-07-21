// Translation Manager

using Newtonsoft.Json;
using BepInEx;
using System.Text.RegularExpressions;

namespace ultimatne_zabitie;

public static class TranslationManager
{
    private static Dictionary<string, string> translations = new();

    public static string Translate(string text)
    {
        string clean = Regex.Replace(text, "<.*?>", "");

        string? id = EnglishMappingManager.GetId(clean);

        if (id == null)
            return text;

        string translated = Get(id);

        return text.Replace(clean, translated);
    }

    public static void Load()
    {
        Plugin.Log.LogInfo("TranslationManager.Load() started");

        string path = Path.Combine(
            Paths.PluginPath,
            "ultimatne-zabitie",
            "translations",
            "texts",
            "sk_sk.json"
        );

        Plugin.Log.LogInfo($"Path: {path}");

        if (!File.Exists(path))
        {
            Plugin.Log.LogWarning("Translation file missing!");
            return;
        }

        Plugin.Log.LogInfo("File exists");

        string json = File.ReadAllText(path);

        var root =
            JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

        if (root != null && root.TryGetValue("translations", out var translationData))
        {
            translations =
                JsonConvert.DeserializeObject<Dictionary<string,string>>(
                    translationData.ToString()!
                )
                ?? new();
        }

        Plugin.Log.LogInfo(
            $"Loaded {translations.Count} translations"
        );
    }

    public static string Get(string key)
    {
        if (translations.TryGetValue(key, out var value))
            return value;

        return key;
    }
}