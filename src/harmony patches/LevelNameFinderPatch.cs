using HarmonyLib;
using TMPro;

namespace ultimatne_zabitie;

[HarmonyPatch(typeof(LevelNameFinder), "OnEnable")]
public static class LevelNameFinderPatch
{
    [HarmonyPostfix]
    public static void Postfix(TMP_Text ___txt2)
    {
        Plugin.Log.LogInfo($"LevelNameFinderPatch.Postfix called with text: {___txt2.text}");
        ___txt2.text = TranslationManager.Translate(___txt2.text);
        Plugin.Log.LogInfo($"LevelNameFinderPatch.Postfix updated text to: {___txt2.text}");
    }
}