using HarmonyLib;
using SettingsMenu.Components;
using System.Reflection;
using TMPro;

namespace ultimatne_zabitie;

[HarmonyPatch(typeof(SettingsDropdown), "ConfigureFrom")]
public static class LanguageDropdownPatch
{
    public static void Postfix(SettingsDropdown __instance, SettingsItemBuilder itemBuilder)
    {
        if (itemBuilder.asset.name != "languageSelector")
            return;

        var dropdownField = typeof(SettingsDropdown)
            .GetField("dropdown", BindingFlags.Instance | BindingFlags.NonPublic);

        if (dropdownField == null)
        {
            Plugin.Log.LogError("Could not find dropdown field");
            return;
        }

        var dropdown = dropdownField.GetValue(__instance) as TMP_Dropdown;

        if (dropdown == null)
        {
            Plugin.Log.LogError("Dropdown field was null");
            return;
        }

        int index = LanguageManager.AvailableLanguages.FindIndex(
            x => x.Code == LanguageManager.CurrentLanguage.Code
        );

        dropdown.SetValueWithoutNotify(index);
        dropdown.RefreshShownValue();

        dropdown.onValueChanged.AddListener(OnChanged);

        Plugin.Log.LogInfo(
            $"Language dropdown initialized: {index} ({LanguageManager.CurrentLanguage.DisplayName})"
        );
    }


    private static void OnChanged(int value)
    {
        var languages = LanguageManager.AvailableLanguages;

        if (value < 0 || value >= languages.Count)
        {
            Plugin.Log.LogError($"Invalid language index: {value}");
            return;
        }

        var language = languages[value];

        LanguageManager.SetCurrentLanguage(language);

        Plugin.Log.LogInfo(
            $"Changed language to {language.DisplayName}"
        );
    }
}