// Translation Manager

using Newtonsoft.Json;
using BepInEx;
using System.Text.RegularExpressions;
using TMPro;

namespace ultimatne_zabitie;

public static class TranslationManager
{
    private static Dictionary<string, string> translations = new();

public static void RefreshAllText()
{
    foreach (var text in UnityEngine.Object.FindObjectsOfType<TMP_Text>(true))
    {
        text.text = text.text;
    }
}

    public static string Translate(string text)
    {
        if (LanguageManager.CurrentLanguage.IsNative)
            return text;

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

        if (LanguageManager.CurrentLanguage.IsNative)
        {
            Plugin.Log.LogInfo(
                "Current language is native, skipping translations"
            );

            translations = new();
            return;
        }

        string path = LanguageManager.CurrentLanguage.FilePath;

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