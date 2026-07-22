using HarmonyLib;
using TMPro;

namespace ultimatne_zabitie.Harmony_Patches;

[HarmonyPatch(typeof(CutsceneSkipText), "Show")]
public static class CutsceneSkipTextPatch
{
    [HarmonyPostfix]
    public static void CutsceneSkipText_Postfix(CutsceneSkipText __instance, ref TMP_Text ___txt)
    {
        string scope_header = "Cutscene";
        
        if (LanguageManager.CurrentLanguage.IsNative)
            return;

        if (___txt != null)
        {
            string? id = EnglishMappingManager.GetId("Press any key to skip", scope_header);
            if (id != null)
            {
                string translated = TranslationManager.Get(id);
                if (translated != id)
                {
                    ___txt.text = translated;
                }
            }
        }
    }
}