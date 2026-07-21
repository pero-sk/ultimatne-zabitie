using HarmonyLib;
using TMPro;

namespace ultimatne_zabitie;

[HarmonyPatch(typeof(LevelNameFinder), "OnEnable")]
public static class LevelNameFinderPatch
{
    [HarmonyPostfix]
    public static void Postfix(TMP_Text ___txt2)
    {
        ___txt2.text = TranslationManager.Translate(___txt2.text);
    }
}