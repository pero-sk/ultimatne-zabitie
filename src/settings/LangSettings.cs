using SettingsMenu.Models;
using UnityEngine;

namespace ultimatne_zabitie;

public static class LanguageSettings
{
    public static SettingsCategory Create()
    {
        var category = ScriptableObject.CreateInstance<SettingsCategory>();

        category.title = "LANGUAGE";
        category.titleDecorator = "--";
        category.items = new List<SettingsItem>();

        var item = ScriptableObject.CreateInstance<SettingsItem>();

        item.name = "languageSelector";
        item.label = "LANGUAGE";
        item.itemType = SettingsItemType.Dropdown;
        item.dropdownType = SettingsDropdownType.List;

        item.preferenceKey = LanguageManager.LanguagePreference;

        var languages = LanguageManager.Languages.Values
            .OrderBy(x => x.Code)
            .ToList();

        item.dropdownList = languages
            .Select(x => x.DisplayName)
            .ToArray();

        item.valueType = SettingsMenu.Models.ValueType.Int;

        category.items.Add(item);

        return category;
    }
}