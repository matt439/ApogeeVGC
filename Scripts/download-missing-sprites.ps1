# PowerShell Script to Download Missing Sprites
# Checks which sprites are missing and downloads them

param(
  [string]$TargetPath = ".\ApogeeVGC\Content\Sprites",
    [string]$SpriteServer = "https://play.pokemonshowdown.com/sprites"
)

Write-Host "Checking for missing sprites..." -ForegroundColor Cyan

# Get current sprites
$frontSprites = @()
$backSprites = @()

if (Test-Path "$TargetPath\Front") {
    $frontSprites = (Get-ChildItem "$TargetPath\Front\*.png" -ErrorAction SilentlyContinue).BaseName
}

if (Test-Path "$TargetPath\Back") {
  $backSprites = (Get-ChildItem "$TargetPath\Back\*.png" -ErrorAction SilentlyContinue).BaseName
}

Write-Host "Current sprites:" -ForegroundColor Yellow
Write-Host "  Front: $($frontSprites.Count)" -ForegroundColor Cyan
Write-Host "  Back: $($backSprites.Count)" -ForegroundColor Cyan

# Find missing
$missingFront = $backSprites | Where-Object { $_ -notin $frontSprites }
$missingBack = $frontSprites | Where-Object { $_ -notin $backSprites }

if (-not $missingFront -and -not $missingBack) {
    Write-Host "`n? All sprites are complete!" -ForegroundColor Green
    exit 0
}

# Sprite name mappings (from your copy-sprites.ps1)
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

$downloadCount = 0

# Download missing front sprites
if ($missingFront) {
    Write-Host "`nDownloading missing FRONT sprites..." -ForegroundColor Yellow
    foreach ($sprite in $missingFront) {
     $showdownName = $spriteMap[$sprite]
    if (-not $showdownName) {
Write-Host "  ? Unknown sprite mapping: $sprite" -ForegroundColor Magenta
        continue
        }
     
        $targetFile = "$TargetPath\Front\$sprite.png"
   $urls = @(
          "$SpriteServer/gen5ani/$showdownName.png",
         "$SpriteServer/dex/$showdownName.png"
        )
        
        $downloaded = $false
        foreach ($url in $urls) {
     try {
       Invoke-WebRequest -Uri $url -OutFile $targetFile -ErrorAction Stop
    Write-Host "  ? Downloaded: $sprite.png" -ForegroundColor Green
           $downloadCount++
     $downloaded = $true
     break
      }
   catch {
  # Try next URL
 }
        }
    
        if (-not $downloaded) {
            Write-Host "  ? Failed: $sprite.png" -ForegroundColor Red
 }
    }
}

# Download missing back sprites
if ($missingBack) {
    Write-Host "`nDownloading missing BACK sprites..." -ForegroundColor Yellow
    foreach ($sprite in $missingBack) {
        $showdownName = $spriteMap[$sprite]
        if (-not $showdownName) {
            Write-Host "  ? Unknown sprite mapping: $sprite" -ForegroundColor Magenta
            continue
    }
        
        $targetFile = "$TargetPath\Back\$sprite.png"
   $urls = @(
    "$SpriteServer/gen5ani-back/$showdownName.png",
       "$SpriteServer/gen5-back/$showdownName.png"
 )
        
 $downloaded = $false
        foreach ($url in $urls) {
try {
        Invoke-WebRequest -Uri $url -OutFile $targetFile -ErrorAction Stop
     Write-Host "  ? Downloaded: $sprite.png" -ForegroundColor Green
                $downloadCount++
         $downloaded = $true
         break
}
catch {
                # Try next URL
          }
  }
  
        if (-not $downloaded) {
Write-Host "  ? Failed: $sprite.png" -ForegroundColor Red
      }
    }
}

Write-Host "`n? Downloaded $downloadCount sprites!" -ForegroundColor Green

if ($downloadCount -gt 0) {
    Write-Host "`nNext steps:" -ForegroundColor Yellow
    Write-Host "1. Run: .\generate-sprite-content.ps1" -ForegroundColor Cyan
    Write-Host "2. Replace the sprites section in Content.mgcb with sprite-content-entries.txt" -ForegroundColor Cyan
    Write-Host "3. Run: .\build-content.ps1" -ForegroundColor Cyan
}
