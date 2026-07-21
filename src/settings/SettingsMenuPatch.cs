using HarmonyLib;
using SettingsMenu.Components;
using SettingsMenu.Models;
using UnityEngine;

namespace ultimatne_zabitie;

[HarmonyPatch(typeof(SettingsMenu.Components.SettingsMenu), "Initialize")]
public static class SettingsMenuPatch
{
    public static void Prefix()
    {
        foreach (var page in Resources.FindObjectsOfTypeAll<SettingsPage>())
        {
            if (page.name != "General")
                continue;
                
            if (page.categories.Any(x => x.title == "LANGUAGE"))
                continue;

            var categories = page.categories.ToList();
            categories.Add(LanguageSettings.Create());

            page.categories = categories.ToArray();

            Plugin.Log.LogInfo(
                $"Added language category to {page.name}"
            );
        }
    }
}