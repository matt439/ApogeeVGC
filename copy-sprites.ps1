# PowerShell Script to Copy Sprites from Pokemon Showdown
# Run this from the ApogeeVGC root directory

param(
    [string]$ShowdownPath = ".\pokemon-showdown\public\sprites",
    [string]$TargetPath = ".\ApogeeVGC\Content\Sprites"
)

Write-Host "Copying Pokemon sprites from Showdown..." -ForegroundColor Green

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
    "greninbond" = "greninja-bond"
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
    "eternatuse eternamax" = "eternatus-eternamax"
    "morpeko" = "morpeko"
    "xerneas" = "xerneas"
    "xerneasneutral" = "xerneas-neutral"
    "xerneasactive" = "xerneas-active"
}

# Copy front sprites (gen5 animated or gen5)
foreach ($key in $spriteMap.Keys) {
    $showdownName = $spriteMap[$key]
    $targetName = "$key.png"
 
    # Try gen5ani first, fall back to gen5
    $frontSource = "$ShowdownPath\gen5ani\$showdownName.png"
    if (-not (Test-Path $frontSource)) {
        $frontSource = "$ShowdownPath\gen5\$showdownName.png"
    }
    
    if (Test-Path $frontSource) {
      Copy-Item $frontSource "$TargetPath\Front\$targetName" -Force
        Write-Host "Copied front sprite: $targetName" -ForegroundColor Cyan
    } else {
        Write-Host "Warning: Front sprite not found for $showdownName" -ForegroundColor Yellow
    }
}

# Copy back sprites (gen5ani-back or gen5-back)
foreach ($key in $spriteMap.Keys) {
    $showdownName = $spriteMap[$key]
    $targetName = "$key.png"
    
    # Try gen5ani-back first, fall back to gen5-back
    $backSource = "$ShowdownPath\gen5ani-back\$showdownName.png"
    if (-not (Test-Path $backSource)) {
   $backSource = "$ShowdownPath\gen5-back\$showdownName.png"
    }
    
    if (Test-Path $backSource) {
        Copy-Item $backSource "$TargetPath\Back\$targetName" -Force
        Write-Host "Copied back sprite: $targetName" -ForegroundColor Cyan
  } else {
        Write-Host "Warning: Back sprite not found for $showdownName" -ForegroundColor Yellow
    }
}

Write-Host "`nSprite copy complete!" -ForegroundColor Green
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Open Content.mgcb in MGCB Editor"
Write-Host "2. Add the sprites from Content/Sprites/Front and Content/Sprites/Back"
Write-Host "3. Build the content project"
