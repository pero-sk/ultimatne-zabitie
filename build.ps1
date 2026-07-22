$Project = "ultimatne-zabitie.csproj"
$Output = "bin/Release/net48"
$PluginName = "ultimatne-zabitie"

# Change this to your ULTRAKILL install path
$UltrakillPlugins = "C:\Program Files (x86)\Steam\steamapps\common\ULTRAKILL\BepInEx\plugins\$PluginName"
$UltrakillPluginsDir = "C:\Program Files (x86)\Steam\steamapps\common\ULTRAKILL\BepInEx\plugins\"

Write-Host "Cleaning..."
dotnet clean $Project

Write-Host "Building..."
dotnet build $Project -c Release

if (!(Test-Path "$Output\$PluginName.dll")) {
    Write-Error "Build failed: DLL not found"
    exit 1
}

Write-Host "Preparing plugin folder..."

if (Test-Path "dist") {
    Remove-Item "dist" -Recurse -Force
}

New-Item "dist\$PluginName" -ItemType Directory | Out-Null

Copy-Item "$Output\$PluginName.dll" "dist\$PluginName\"

Write-Host "Copying to ULTRAKILL..."

if (Test-Path $UltrakillPluginsDir) {
    if (Test-Path $UltrakillPlugins) {
        Remove-Item $UltrakillPlugins -Recurse -Force
    }

    New-Item $UltrakillPlugins -ItemType Directory | Out-Null
    Copy-Item "dist\$PluginName\*" $UltrakillPlugins -Recurse

    Write-Host "Installed!"
}
else {
    Write-Warning "ULTRAKILL plugin folder not found"
}

Write-Host "Done!"