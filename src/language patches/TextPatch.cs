using HarmonyLib;
using TMPro;

namespace ultimatne_zabitie;

[HarmonyPatch(typeof(TMP_Text), "set_text")]
public static class TMPTextPatch
{
    [HarmonyPrefix]
    public static void Prefix(TMP_Text __instance, ref string value)
    {
        if (LanguageManager.CurrentLanguage.IsNative)
        {
            return;
        }

        string original = value;

        string translated = TranslationManager.Translate(original);

        if (original != translated)
        {
            Plugin.Log.LogInfo(
                $"Translated: {original} -> {translated}"
            );
        }

        value = translated;
    }
}