# PowerShell Script to Download Sprites from Pokemon Showdown Server
# Run this from the ApogeeVGC root directory

param(
    [string]$TargetPath = ".\ApogeeVGC\Content\Sprites",
    [string]$SpriteServer = "https://play.pokemonshowdown.com/sprites"
)

Write-Host "Downloading Pokemon sprites from Showdown server..." -ForegroundColor Green

# Create target directories
New-Item -ItemType Directory -Force -Path "$TargetPath\Front" | Out-Null
New-Item -ItemType Directory -Force -Path "$TargetPath\Back" | Out-Null

# Define sprite mappings (SpecieId -> showdown filename)
$spriteMap = @{
    "bulbasaur" = "bulbasaur"
    "calyrexice" = "calyrex-ice"
    "miraidon" = "miraidon"
"ursaluna" = "ursaluna"
    "volcarona" = "volcarona"
    "grimmsnarl" = "grimmsnarl"
    "ironhands" = "ironhands"
    "calyrex" = "calyrex"
    "shayminsky" = "shaymin-sky"
    "shaymin" = "shaymin"
  "greninjasbond" = "greninja-bond"
    "rockruffdusk" = "rockruff-dusk"
    "terapagosterastal" = "terapagos-terastal"
    "terapagosstellar" = "terapagos-stellar"
    "zacian" = "zacian"
    "zaciancrowned" = "zacian-crowned"
    "zamazenta" = "zamazenta"
    "zamazentacrowned" = "zamazenta-crowned"
    "ogerpon" = "ogerpon"
    "ogerpontealtera" = "ogerpon-tealtera"
    "terapagos" = "terapagos"
    "eternatuseeternamax" = "eternatus-eternamax"
    "morpeko" = "morpeko"
    "xerneas" = "xerneas"
    "xerneasneutral" = "xerneas-neutral"
    "xerneasactive" = "xerneas-active"
}

$successCount = 0
$failCount = 0

# Download front sprites (gen5ani or dex as fallback)
Write-Host "`nDownloading front sprites..." -ForegroundColor Yellow
foreach ($key in $spriteMap.Keys) {
    $showdownName = $spriteMap[$key]
    $targetName = "$key.png"
    
    # Try gen5ani first (animated), fall back to dex (static)
    $frontUrls = @(
        "$SpriteServer/gen5ani/$showdownName.png",
        "$SpriteServer/dex/$showdownName.png"
    )
    
    $downloaded = $false
    foreach ($url in $frontUrls) {
     try {
            Invoke-WebRequest -Uri $url -OutFile "$TargetPath\Front\$targetName" -ErrorAction Stop
       Write-Host "  ? Downloaded front sprite: $targetName" -ForegroundColor Green
     $successCount++
       $downloaded = $true
       break
     }
        catch {
            # Try next URL
        }
    }
    
    if (-not $downloaded) {
     Write-Host "  ? Failed to download front sprite: $targetName" -ForegroundColor Red
     $failCount++
    }
}

# Download back sprites (gen5ani-back or gen5-back as fallback)
Write-Host "`nDownloading back sprites..." -ForegroundColor Yellow
foreach ($key in $spriteMap.Keys) {
    $showdownName = $spriteMap[$key]
    $targetName = "$key.png"
    
    # Try gen5ani-back first (animated), fall back to gen5-back (static)
    $backUrls = @(
     "$SpriteServer/gen5ani-back/$showdownName.png",
        "$SpriteServer/gen5-back/$showdownName.png"
    )
    
    $downloaded = $false
foreach ($url in $backUrls) {
        try {
            Invoke-WebRequest -Uri $url -OutFile "$TargetPath\Back\$targetName" -ErrorAction Stop
            Write-Host "  ? Downloaded back sprite: $targetName" -ForegroundColor Green
   $successCount++
 $downloaded = $true
  break
        }
   catch {
            # Try next URL
        }
  }
    
    if (-not $downloaded) {
   Write-Host "  ? Failed to download back sprite: $targetName" -ForegroundColor Red
        $failCount++
    }
}

Write-Host "`nSprite download complete!" -ForegroundColor Green
Write-Host "Success: $successCount sprites" -ForegroundColor Green
Write-Host "Failed: $failCount sprites" -ForegroundColor $(if ($failCount -gt 0) { "Red" } else { "Green" })

Write-Host "`nNext steps:" -ForegroundColor Yellow
Write-Host "1. Run: .\generate-sprite-content.ps1" -ForegroundColor Cyan
Write-Host "2. Append the contents of 'sprite-content-entries.txt' to Content.mgcb" -ForegroundColor Cyan
Write-Host "3. Build the content project" -ForegroundColor Cyan
