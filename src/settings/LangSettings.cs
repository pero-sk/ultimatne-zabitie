using SettingsMenu.Models;
using UnityEngine;

namespace ultimatne_zabitie;

public static class LanguageSettings
{
    public static SettingsCategory Create()
    {
        Plugin.Log.LogInfo(
            $"Creating language settings, current = {LanguageManager.CurrentLanguage.Code}"
        );

        var category = ScriptableObject.CreateInstance<SettingsCategory>();

        category.title = "LANGUAGE";
        category.titleDecorator = "--";
        category.items = new List<SettingsItem>();

        var item = ScriptableObject.CreateInstance<SettingsItem>();

        item.name = "languageSelector";
        item.label = "LANGUAGE";
        item.itemType = SettingsItemType.Dropdown;
        item.dropdownType = SettingsDropdownType.List;

        var languages = LanguageManager.AvailableLanguages;

        item.dropdownList = languages
            .Select(x => x.DisplayName)
            .ToArray();

        item.defaultCombination = languages.FindIndex(
            x => x.Code == LanguageManager.CurrentLanguage.Code
        );

        item.valueType = SettingsMenu.Models.ValueType.Int;

        category.items.Add(item);

        return category;
    }
}