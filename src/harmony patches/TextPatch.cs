using HarmonyLib;
using TMPro;

namespace ultimatne_zabitie;

[HarmonyPatch(typeof(TMP_Text), "set_text")]
public static class TMPTextDebugPatch
{
    [HarmonyPrefix]
    public static void Prefix(ref string value)
    {
        string original = value;

        if (value.Contains("FULL INTRO"))
        {
            Plugin.Log.LogInfo($"FOUND FULL INTRO: {value}");
        }

        value = TranslationManager.Translate(value);

        if (original != value)
        {
            Plugin.Log.LogInfo($"Translated: {original} -> {value}");
        } else
        {
            Plugin.Log.LogInfo($"No translation found for: {original}");
        }
    }
}