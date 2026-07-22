using HarmonyLib;
using TMPro;

namespace ultimatne_zabitie.Harmony_Patches;

[HarmonyPatch(typeof(HudMessage), "PlayMessage")]
public static class LocalizeHudMessage
{
    [HarmonyPostfix]
    public static void PlayMessage_Postfix(HudMessage __instance, HudMessageReceiver ___messageHud, TMP_Text ___text)
    {
        string scope_header = "HUD";
        
        if (LanguageManager.CurrentLanguage.IsNative)
            return;

        if (___text == null)
            return;

        string original = ___text.text;
        string clean = System.Text.RegularExpressions.Regex.Replace(original, "<.*?>", "");
        
        string? id = EnglishMappingManager.GetId(clean, scope_header);
        if (id != null)
        {
            string translated = TranslationManager.Get(id);
            if (translated != id)
            {
                ___text.text = original.Replace(clean, translated);
                ___text.text = ___text.text.Replace('$', '\n');
            }
        }
    }
}

[HarmonyPatch(typeof(HudMessageReceiver), "SendHudMessage")]
public static class SendHudMessagePatch
{
    [HarmonyPrefix]
    public static bool SendHudMessage_Prefix(ref string newmessage, string newinput = "", string newmessage2 = "", int delay = 0, bool silent = false)
    {
        string scope_header = "HUD";
        if (!LanguageManager.CurrentLanguage.IsNative)
        {
            string? id = EnglishMappingManager.GetId(newmessage, scope_header);
            if (id != null)
            {
                string translated = TranslationManager.Get(id);
                if (translated != id)
                {
                    newmessage = translated;
                }
            }
        }
        return true;
    }
}