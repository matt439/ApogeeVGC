# PowerShell Script to Download Sprites from PokeAPI
# Alternative source if Showdown sprites don't work
# Run this from the ApogeeVGC root directory

param(
    [string]$TargetPath = ".\ApogeeVGC\Content\Sprites"
)

Write-Host "Downloading Pokemon sprites from PokeAPI..." -ForegroundColor Green

# Create target directories
New-Item -ItemType Directory -Force -Path "$TargetPath\Front" | Out-Null
New-Item -ItemType Directory -Force -Path "$TargetPath\Back" | Out-Null

# Define sprite mappings with PokeAPI IDs
# Format: SpecieId -> @{ name = "name"; id = dex_number }
$spriteMap = @{
    "bulbasaur" = @{ name = "bulbasaur"; id = 1 }
    "calyrexice" = @{ name = "calyrex-ice"; id = 898 }
  "miraidon" = @{ name = "miraidon"; id = 1008 }
    "ursaluna" = @{ name = "ursaluna"; id = 901 }
    "volcarona" = @{ name = "volcarona"; id = 637 }
    "grimmsnarl" = @{ name = "grimmsnarl"; id = 861 }
  "ironhands" = @{ name = "iron-hands"; id = 992 }
    "calyrex" = @{ name = "calyrex"; id = 898 }
    "shayminsky" = @{ name = "shaymin-sky"; id = 492 }
    "shaymin" = @{ name = "shaymin"; id = 492 }
    "greninjasbond" = @{ name = "greninja-bond"; id = 658 }
    "rockruffdusk" = @{ name = "rockruff"; id = 744 }
    "terapagosterastal" = @{ name = "terapagos-terastal"; id = 1024 }
    "terapagosstellar" = @{ name = "terapagos-stellar"; id = 1024 }
    "zacian" = @{ name = "zacian"; id = 888 }
    "zaciancrowned" = @{ name = "zacian-crowned"; id = 888 }
    "zamazenta" = @{ name = "zamazenta"; id = 889 }
    "zamazentacrowned" = @{ name = "zamazenta-crowned"; id = 889 }
    "ogerpon" = @{ name = "ogerpon"; id = 1017 }
    "ogerpontealtera" = @{ name = "ogerpon-teal"; id = 1017 }
    "terapagos" = @{ name = "terapagos"; id = 1024 }
    "eternatuseeternamax" = @{ name = "eternatus-eternamax"; id = 890 }
    "morpeko" = @{ name = "morpeko"; id = 877 }
    "xerneas" = @{ name = "xerneas"; id = 716 }
    "xerneasneutral" = @{ name = "xerneas-neutral"; id = 716 }
"xerneasactive" = @{ name = "xerneas-active"; id = 716 }
}

$successCount = 0
$failCount = 0

# PokeAPI sprite URLs
$baseUrl = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon"

Write-Host "`nDownloading sprites from PokeAPI..." -ForegroundColor Yellow
foreach ($key in $spriteMap.Keys) {
    $info = $spriteMap[$key]
    $pokemonId = $info.id
    $targetName = "$key.png"
    
    # Download front sprite
    $frontUrl = "$baseUrl/$pokemonId.png"
    try {
        Invoke-WebRequest -Uri $frontUrl -OutFile "$TargetPath\Front\$targetName" -ErrorAction Stop
        Write-Host "  ? Downloaded front: $targetName (ID: $pokemonId)" -ForegroundColor Green
        $successCount++
    }
    catch {
      Write-Host "  ? Failed front: $targetName (ID: $pokemonId)" -ForegroundColor Red
        $failCount++
 }
    
    # Download back sprite
    $backUrl = "$baseUrl/back/$pokemonId.png"
    try {
   Invoke-WebRequest -Uri $backUrl -OutFile "$TargetPath\Back\$targetName" -ErrorAction Stop
     Write-Host "  ? Downloaded back: $targetName (ID: $pokemonId)" -ForegroundColor Green
        $successCount++
    }
    catch {
        Write-Host "  ? Failed back: $targetName (ID: $pokemonId)" -ForegroundColor Red
        $failCount++
    }
    
    # Small delay to avoid rate limiting
    Start-Sleep -Milliseconds 100
}

Write-Host "`nDownload complete!" -ForegroundColor Green
Write-Host "Success: $successCount sprites" -ForegroundColor Green
Write-Host "Failed: $failCount sprites" -ForegroundColor $(if ($failCount -gt 0) { "Red" } else { "Green" })

if ($failCount -gt 0) {
    Write-Host "`nNote: Some sprites may not be available in PokeAPI format." -ForegroundColor Yellow
    Write-Host "Try using copy-sprites.ps1 (Showdown) instead for better coverage." -ForegroundColor Yellow
}

Write-Host "`nNext steps:" -ForegroundColor Cyan
Write-Host "1. Run: .\generate-sprite-content.ps1"
Write-Host "2. Append 'sprite-content-entries.txt' to Content.mgcb"
Write-Host "3. Build the content project"
