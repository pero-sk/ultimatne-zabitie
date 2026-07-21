# Ultimatné Zabitie

A [BepInEx](https://github.com/BepInEx/BepInEx) plugin that adds Slovak (and, more generally, additional languages) localization support to [ULTRAKILL](https://ultrakill.fandom.com/wiki/ULTRAKILL).

It works by intercepting the game's `TextMeshPro` text as it's set, mapping the original English string to a translation ID, and swapping in the translated string for the currently selected language — without touching the game's own assemblies.

## Features

- In-game language switcher added to the settings menu
- Live text translation via a Harmony patch on `TMP_Text.set_text` — no need to restart the game after switching language
- Level name translation (title screen / level select)
- JSON-based translation files, so new languages can be added without touching any code
- Falls back to the game's native English text for any string that has no translation

## Installing (as a player)

1. Install [BepInEx 5.x](https://github.com/BepInEx/BepInEx/releases) for ULTRAKILL if you haven't already.
2. Grab the ultimatnie-zabite.dll file from the [Latest Release](https://github.com/pero-sk/ultimatne-zabitie/releases).
3. Put it in your BepInEx's plugin folder under a subdirectory called "ultimatne-zabitie" (pathto_ULTRAKILL/BepInEx/plugins/ultimatne-zabitite/)
4. Launch the game. Open **Settings** to pick your language from the dropdown.

## Building from source

This repo does **not** include ULTRAKILL's assemblies — you need to supply your own copies (from your own legitimate install of the game) since redistributing them isn't allowed.

1. Create a `ref/` folder in the project root.
2. Copy the following DLLs into `ref/` from your `ULTRAKILL/ULTRAKILL_Data/Managed/` folder (plus BepInEx's `0Harmony.dll` from your BepInEx install):
   - `Assembly-CSharp.dll`
   - `BepInEx.dll`
   - `UnityEngine.dll`
   - `UnityEngine.CoreModule.dll`
   - `Unity.TextMeshPro.dll`
   - `UnityEngine.UI.dll`
   - `Newtonsoft.Json.dll`
   - `0Harmony.dll`
3. Edit `build.ps1` and update `$UltrakillPluginsDir` to point at your own ULTRAKILL install.
4. Run the build script from a PowerShell prompt in the project root:
   ```powershell
   ./build.ps1
   ```
   This cleans, builds in Release mode, and copies the resulting plugin straight into your ULTRAKILL `BepInEx/plugins` folder.

Requires the .NET SDK (targets `net48`) and PowerShell.

## Project layout

```
src/
  Plugin.cs                     Harmony bootstrap / plugin entry point
  LangMan.cs                    Tracks available languages and the current language
  LangInfo.cs                   Language metadata model (code, display name, author, etc.)
  TransMan.cs                   Loads translation files and resolves strings by ID
  engman.cs                     Maps original English text -> translation IDs
  language patches/
    TextPatch.cs                 Harmony patch: translates TMP_Text as it's set
    LevelNameFinderPatch.cs       Harmony patch: translates level names on the level select screen
  settings/
    LangSettings.cs               Config binding for the selected language
    LangDropdownPatch.cs           Adds the language dropdown to the settings menu
    SettingsMenuPatch.cs           Hooks into the settings menu UI

assets/translations/
  mappings/en_us.json           English text -> translation ID map (the "source of truth" for keys)
  texts/<lang_code>.json        Per-language translation files (e.g. sk_sk.json)
  texts/manifest.json           Contains every language file in texts/ for easy downloading of them from the mod
```

## Adding a new language

1. Create a new file at `assets/translations/texts/<your_lang_code>.json`.
2. Add a `metadata` block:
   ```json
   {
     "metadata": {
       "lang-name": "xx_XX",
       "lang-author": "Your Name",
       "lang-version": "1.0.0",
       "lang-display": "Language Name (English)",
       "lang-code": "xx_xx",
       "lang-rtl": false,
       "versions": { "min": "0.1.0", "max": "0.1.0" }
     },
     "translations": {
       "0-1.levelName": "..."
     }
   }
   ```
3. Fill in `translations` using the IDs found in `assets/translations/mappings/en_us.json` (each English string maps to an ID like `0-1.levelName`, `mainMenu.title`, etc.) — you don't need every ID, just the ones you've translated so far.
4. Drop the file in `assets/translations/texts/`, rebuild, and your language will show up in the in-game dropdown automatically.
5. To provide the language globally for everyone, fork this repo, add your <your_lang_code>.json to assets/translations/texts/... and into manifest.json.

## License

Apache License 2.0. See [LICENSE.md](LICENSE.md).

ULTRAKILL is © 3D Realms / New Blood Interactive / Hakita. This project is an unofficial fan-made mod and is not affiliated with or endorsed by them. No game assets or decompiled source code are distributed with this repository.

## Future plans

### Context-scoped mappings ("headers")

Right now `en_us.json` is a single flat map from English text to translation ID:

```json
{
  "RIGHT": "option.right"
}
```

This breaks down whenever the same English string means different things in different places. A great example being **"RIGHT"**, which could mean "correct" (e.g. prompt confirmation) or a direction (e.g. Weapon position right). Right now both would collide on the same key and be forced to share one translation, even though Slovak (and most languages) needs different words for each.

The plan is to split the mapping into **headers**, grouping strings by the screen/context they actually appear in, so the same English text can resolve to different IDs — and therefore different translations — depending on where it shows up:

```json
{
  "options": {
    "RIGHT": "options.direction.right"
  },
  "levelEnd": {
    "RIGHT": "levelEnd.correct"
  },
  "mainMenu": {
    "PLAY": "mainMenu.play"
  }
}
```

Concretely this means:

- `EnglishMappingManager` will need to know which header/context it's resolving in, not just the raw text, so it can look up `header.text` instead of just `text`.
- Each Harmony patch (or the general `TMPTextPatch`) will need to supply that context — likely derived from the scene name, the containing menu/canvas, or a small explicit map of component -> header, since generic `TMP_Text.set_text` doesn't inherently know what screen it's on.
- `sk_sk.json` and future language files will mirror the same header structure, so translators can see at a glance which section of the game a string belongs to.
- Where no header-specific match exists, we'll fall back to a `"default"` header (today's flat behavior) so translators aren't forced to duplicate every string across every context up front — headers are opt-in for strings that actually need disambiguation.

This should also make the translation files easier for contributors to navigate — grouped by menu/context instead of one giant alphabetical-ish list.
