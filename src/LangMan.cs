using Newtonsoft.Json.Linq;
using BepInEx;
using SettingsMenu.Models;

namespace ultimatne_zabitie;

public static class LanguageManager
{
    public static Dictionary<string, LanguageInfo> Languages = new();

    public static List<LanguageInfo> AvailableLanguages =>
        LanguageManager.Languages.Values
            .OrderBy(x => x.Code)
            .ToList();

    public static LanguageInfo CurrentLanguage { get; private set; } = null!;

    public static PreferenceKey LanguagePreference = new()
    {
        key = "language"
    };

    public static LanguageInfo DefaultLanguage { get; } = new()
    {
        Code = "en_us",
        DisplayName = "English (US)",
        Author = "Hakita, NewBlood",
        Version = "1.0.0",
        IsNative = true,
        FilePath = "built_in" // shitty code exists if this gets used for DefaultLanguage
    };

    public static List<string> GetLanguageNames()
    {
        return Languages.Values
            .Select(x => x.DisplayName)
            .ToList();
    }

    public static List<string> GetLanguageCodes()
    {
        return Languages.Keys.ToList();
    }

    public static void SetCurrentLanguage(LanguageInfo lang)
    {
        if (!Languages.ContainsKey(lang.Code))
        {
            Plugin.Log.LogError(
                $"Unknown language: {lang.Code}"
            );
            return;
        }

        CurrentLanguage = Languages[lang.Code];


        int index = AvailableLanguages
            .FindIndex(x => x.Code == lang.Code);

        if (LanguagePreference.IsValid())
        {
            LanguagePreference.SetValue(index);
        }

        TranslationManager.Load();
        TranslationManager.RefreshAllText();
    }

    public static void LoadLanguages(bool resetToDefault)
    {
        // Ensure default language is present (use indexer to avoid ArgumentException when already present)
        Languages[DefaultLanguage.Code] = DefaultLanguage;
        if (resetToDefault) {
            SetCurrentLanguage(DefaultLanguage);
        }

        string path = Path.Combine(
            Paths.PluginPath,
            "ultimatne-zabitie",
            "translations",
            "texts"
        );

        Plugin.Log.LogInfo($"Loading languages from: {path}");

        if (!Directory.Exists(path))
        {
            Plugin.Log.LogError("Translations directory missing!");
            return;
        }

        foreach (string file in Directory.GetFiles(path, "*.json"))
        {
            LanguageInfo? language = LoadMetadata(file);

            if (language == null)
                continue;

            Languages[language.Code] = language;

            Plugin.Log.LogInfo(
                $"Found language: {language.DisplayName} ({language.Code})"
            );
        }

        Plugin.Log.LogInfo(
            $"Loaded {Languages.Count} languages"
        );
    }


    private static LanguageInfo? LoadMetadata(string path)
    {
        try
        {
            string json = File.ReadAllText(path);

            JObject root = JObject.Parse(json);

            JObject? metadata = root["metadata"] as JObject;

            if (metadata == null)
            {
                Plugin.Log.LogWarning(
                    $"Missing metadata: {path}"
                );
                return null;
            }


            return new LanguageInfo
            {
                Code = metadata["lang-code"]?.ToString() ?? "",
                DisplayName = metadata["lang-display"]?.ToString() ?? "",
                Author = metadata["lang-author"]?.ToString() ?? "",
                Version = metadata["lang-version"]?.ToString() ?? "",
                IsNative = metadata["lang-native"]?.ToObject<bool>() ?? false,
                FilePath = path,
            };
        }
        catch (Exception e)
        {
            Plugin.Log.LogError(
                $"Failed reading language metadata: {path}"
            );

            Plugin.Log.LogError(e);
            return null;
        }
    }
}