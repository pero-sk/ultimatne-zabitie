using HarmonyLib;
using SettingsMenu.Components;

namespace ultimatne_zabitie;

[HarmonyPatch(typeof(SettingsDropdown), "ConfigureFrom")]
public static class LanguageDropdownPatch
{
    public static void Postfix(SettingsDropdown __instance, SettingsItemBuilder itemBuilder)
    {
        if (itemBuilder.asset.name != "languageSelector")
            return;

        __instance.onValueChanged.AddListener(OnChanged);

        Plugin.Log.LogInfo("Language dropdown hooked");
    }

    private static void OnChanged(int value)
    {
        var language = LanguageManager.Languages.Values
            .OrderBy(x => x.Code)
            .ElementAt(value);

        LanguageManager.SetCurrentLanguage(language);

        Plugin.Log.LogInfo(
            $"Changed language to {language.DisplayName}"
        );
    }

    public static void OnValueChangedInt(int value)
    {
        Plugin.Log.LogInfo($"(ReflectiveHook) Dropdown changed -> {value}");

        var prefs = MonoSingleton<PrefsManager>.Instance;

        if (prefs != null)
        {
            prefs.SetInt("language", value);
        }

        var language = LanguageManager.AvailableLanguages[value];

        LanguageManager.SetCurrentLanguage(language);
    }
}