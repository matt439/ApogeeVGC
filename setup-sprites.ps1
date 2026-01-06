# Complete Sprite Setup Workflow
# Automates the entire process of setting up sprites

Write-Host "???????????????????????????????????????" -ForegroundColor Cyan
Write-Host "  ApogeeVGC Sprite Setup Wizard" -ForegroundColor Cyan
Write-Host "???????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

# Step 1: Check current status
Write-Host "Step 1: Checking current sprite status..." -ForegroundColor Yellow
$frontCount = (Get-ChildItem ".\ApogeeVGC\Content\Sprites\Front\*.png" -ErrorAction SilentlyContinue).Count
$backCount = (Get-ChildItem ".\ApogeeVGC\Content\Sprites\Back\*.png" -ErrorAction SilentlyContinue).Count

Write-Host "  Current: $frontCount front, $backCount back sprites" -ForegroundColor Cyan

if ($frontCount -eq 0 -and $backCount -eq 0) {
    Write-Host "  ? No sprites found. Will download all." -ForegroundColor Magenta
    
    $response = Read-Host "`nDownload all sprites from Pokémon Showdown? (y/n)"
    if ($response -eq 'y') {
        Write-Host "`nDownloading sprites..." -ForegroundColor Yellow
        & ".\copy-sprites.ps1"
}
    else {
        Write-Host "Skipping download. Exiting." -ForegroundColor Red
        exit
    }
}
else {
    Write-Host "  ? Checking for missing sprites..." -ForegroundColor Magenta
    
$frontSprites = (Get-ChildItem ".\ApogeeVGC\Content\Sprites\Front\*.png" -ErrorAction SilentlyContinue).BaseName
    $backSprites = (Get-ChildItem ".\ApogeeVGC\Content\Sprites\Back\*.png" -ErrorAction SilentlyContinue).BaseName
    
    $missingFront = $backSprites | Where-Object { $_ -notin $frontSprites }
    $missingBack = $frontSprites | Where-Object { $_ -notin $backSprites }
    
    if ($missingFront -or $missingBack) {
      Write-Host "`n? Found missing sprites:" -ForegroundColor Yellow
 if ($missingFront) { Write-Host "    Missing front: $($missingFront.Count)" -ForegroundColor Red }
        if ($missingBack) { Write-Host "    Missing back: $($missingBack.Count)" -ForegroundColor Red }
     
     $response = Read-Host "`nDownload missing sprites? (y/n)"
        if ($response -eq 'y') {
     Write-Host "`nDownloading missing sprites..." -ForegroundColor Yellow
     & ".\download-missing-sprites.ps1"
        }
    }
    else {
      Write-Host "  ? All sprites present!" -ForegroundColor Green
    }
}

# Step 2: Generate Content.mgcb entries
Write-Host "`nStep 2: Generating Content.mgcb entries..." -ForegroundColor Yellow
& ".\generate-sprite-content.ps1"

if (Test-Path "sprite-content-entries.txt") {
    Write-Host "  ? Generated sprite-content-entries.txt" -ForegroundColor Green
    
    Write-Host "`n  ? MANUAL STEP REQUIRED:" -ForegroundColor Yellow
    Write-Host "    1. Open ApogeeVGC\Content\Content.mgcb in a text editor" -ForegroundColor Cyan
    Write-Host "    2. Find the '# Sprites' section" -ForegroundColor Cyan
    Write-Host "    3. Replace sprite entries with contents of sprite-content-entries.txt" -ForegroundColor Cyan
    Write-Host ""
    
    $response = Read-Host "Have you updated Content.mgcb? (y/n)"
    if ($response -ne 'y') {
        Write-Host "`nPlease update Content.mgcb and run: .\build-content.ps1" -ForegroundColor Yellow
        Write-Host "Then run your game!" -ForegroundColor Green
        exit
    }
}

# Step 3: Build content
Write-Host "`nStep 3: Building MonoGame content..." -ForegroundColor Yellow
& ".\build-content.ps1"

if ($LASTEXITCODE -eq 0) {
  Write-Host "`n???????????????????????????????????????" -ForegroundColor Green
    Write-Host "  ? Setup Complete!" -ForegroundColor Green
    Write-Host "???????????????????????????????????????" -ForegroundColor Green
    Write-Host ""
 Write-Host "Your sprites are ready to use!" -ForegroundColor Cyan
    Write-Host "Run your game to see them in action." -ForegroundColor Cyan
}
else {
    Write-Host "`n? Build failed. Check errors above." -ForegroundColor Red
}
