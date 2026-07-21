using HarmonyLib;
using TMPro;

namespace ultimatne_zabitie;

[HarmonyPatch(typeof(TMP_Text), "set_text")]
public static class TMPTextPatch
{
    private static Dictionary<TMP_Text, string> originals = new();

    [HarmonyPrefix]
    public static void Prefix(TMP_Text __instance, ref string value)
    {
        if (!originals.ContainsKey(__instance))
        {
            originals[__instance] = value;
        }

        string original = originals[__instance];

        if (LanguageManager.CurrentLanguage.IsNative)
        {
            value = original;
            return;
        }

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