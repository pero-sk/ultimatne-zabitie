using HarmonyLib;
using TMPro;

namespace ultimatne_zabitie.Harmony_Patches;

[HarmonyPatch(typeof(DifficultyTitle), "Check")]
public static class LocalizeDifficultyTitle
{
    [HarmonyPrefix]
    public static bool Check_Prefix(DifficultyTitle __instance, ref TMP_Text ___txt2)
    {
        if (LanguageManager.CurrentLanguage.IsNative)
            return true;

        try
        {
            string scope_header = "difficultyTitle";
            
            var prefs = MonoSingleton<PrefsManager>.Instance;
            int difficulty = prefs?.GetInt("difficulty", 0) ?? 0;
            string text = __instance.lines ? "-- " : "";
            
            string difficultyKey = difficulty switch
            {
                0 => "harmless",
                1 => "lenient", 
                2 => "standard",
                3 => "violent",
                4 => "brutal",
                5 => "umd",
                _ => "standard"
            };
            
            string? id = EnglishMappingManager.GetId(difficultyKey, scope_header);
            if (id != null)
            {
                text += TranslationManager.Get(id);
            }
            else
            {
                text += difficultyKey.ToUpper();
            }
            
            if (__instance.lines)
                text += " --";

            if (___txt2 == null)
                ___txt2 = __instance.GetComponent<TMP_Text>();

            if (___txt2 != null)
            {
                ___txt2.text = text;
                return false;
            }
        }
        catch (System.Exception e)
        {
            Plugin.Log.LogError($"Failed to localize difficulty: {e.Message}");
        }
        
        return true;
    }
}