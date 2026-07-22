using HarmonyLib;
using TMPro;
using SettingsMenu.Components;

namespace ultimatne_zabitie.Harmony_Patches;

[HarmonyPatch(typeof(SettingsMenu.Components.SettingsMenu), "Initialize")]
public static class LocalizeSettingsMenu
{
    [HarmonyPostfix]
    public static void Initialize_Postfix(SettingsMenu.Components.SettingsMenu __instance)
    {
        string scope_header = "settings";
        if (LanguageManager.CurrentLanguage.IsNative)
            return;

        // Find all text elements in the settings menu
        foreach (var text in __instance.gameObject.GetComponentsInChildren<TMP_Text>(true))
        {
            if (text == null || string.IsNullOrEmpty(text.text))
                continue;

            string original = text.text;
            string clean = System.Text.RegularExpressions.Regex.Replace(original, "<.*?>", "");
            
            string? id = EnglishMappingManager.GetId(clean, scope_header);
            if (id != null)
            {
                string translated = TranslationManager.Get(id);
                if (translated != id)
                {
                    text.text = original.Replace(clean, translated);
                }
            }
        }
    }
}