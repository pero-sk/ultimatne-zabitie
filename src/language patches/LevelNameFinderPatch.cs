using HarmonyLib;
using TMPro;

namespace ultimatne_zabitie;

[HarmonyPatch(typeof(LevelNameFinder), "OnEnable")]
public static class LevelNameFinderPatch
{
    [HarmonyPostfix]
    public static void Postfix(TMP_Text ___txt2)
    {
        string scope_header = "levelName";

        if (LanguageManager.CurrentLanguage.IsNative)
            return;

        string original = ___txt2.text;
        string clean = System.Text.RegularExpressions.Regex.Replace(original, "<.*?>", "");
        
        string? id = EnglishMappingManager.GetId(clean, scope_header);
        
        if (id != null)
        {
            string translated = TranslationManager.Get(id);
            if (translated != id)
            {
                ___txt2.text = original.Replace(clean, translated);
            }
        }
    }
}