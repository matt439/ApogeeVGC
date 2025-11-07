# PowerShell Script to Build MonoGame Content
# Alternative to using MGCB Editor GUI
# Run this from the ApogeeVGC root directory

param(
    [switch]$Clean,
    [switch]$Rebuild
)

$contentPath = ".\ApogeeVGC\Content"
$contentFile = "$contentPath\Content.mgcb"

if (-not (Test-Path $contentFile)) {
    Write-Host "Error: Content.mgcb not found at $contentFile" -ForegroundColor Red
    exit 1
}

Write-Host "MonoGame Content Builder" -ForegroundColor Cyan
Write-Host "========================" -ForegroundColor Cyan
Write-Host ""

# Change to content directory
Push-Location $contentPath

try {
    if ($Clean -or $Rebuild) {
 Write-Host "Cleaning content..." -ForegroundColor Yellow
        mgcb Content.mgcb /clean
     Write-Host ""
    }

    if (-not $Clean) {
  Write-Host "Building content..." -ForegroundColor Yellow
        mgcb Content.mgcb
        
        if ($LASTEXITCODE -eq 0) {
   Write-Host ""
Write-Host "? Build successful!" -ForegroundColor Green
      
 # Count built files
            $xnbFiles = Get-ChildItem ".\bin" -Recurse -Filter "*.xnb" -ErrorAction SilentlyContinue
      if ($xnbFiles) {
         Write-Host "  Built $($xnbFiles.Count) content files" -ForegroundColor Cyan
   
    # Show breakdown
       $fonts = ($xnbFiles | Where-Object { $_.FullName -like "*Fonts*" }).Count
                $sprites = ($xnbFiles | Where-Object { $_.FullName -like "*Sprites*" }).Count

   Write-Host "    - Fonts: $fonts" -ForegroundColor Gray
       Write-Host "    - Sprites: $sprites" -ForegroundColor Gray
            }
  }
        else {
            Write-Host ""
 Write-Host "? Build failed!" -ForegroundColor Red
   exit 1
        }
    }
}
finally {
    Pop-Location
}

Write-Host ""
Write-Host "Options:" -ForegroundColor Yellow
Write-Host "  .\build-content.ps1 - Build content (incremental)"
Write-Host "  .\build-content.ps1 -Clean   - Clean only"
Write-Host "  .\build-content.ps1 -Rebuild - Clean and rebuild all"
