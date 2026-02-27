# PowerShell Script to Generate Content.mgcb Sprite Entries
# Run this after copying sprites to generate the content entries

param(
    [string]$SpritePath = ".\ApogeeVGC\Content\Sprites"
)

Write-Host "Generating Content.mgcb entries..." -ForegroundColor Green

$output = @()
$output += ""
$output += "#-------------------------------- Sprites -----------------------------------#"
$output += ""

# Process Front sprites
if (Test-Path "$SpritePath\Front") {
    $frontSprites = Get-ChildItem "$SpritePath\Front\*.png" -File
  
    foreach ($sprite in $frontSprites) {
     $spriteName = $sprite.BaseName
        $output += "#begin Sprites/Front/$($sprite.Name)"
   $output += "/importer:TextureImporter"
        $output += "/processor:TextureProcessor"
    $output += "/processorParam:ColorKeyColor=255,0,255,255"
      $output += "/processorParam:ColorKeyEnabled=True"
    $output += "/processorParam:GenerateMipmaps=False"
     $output += "/processorParam:PremultiplyAlpha=True"
   $output += "/processorParam:ResizeToPowerOfTwo=False"
        $output += "/processorParam:MakeSquare=False"
  $output += "/processorParam:TextureFormat=Color"
        $output += "/build:Sprites/Front/$($sprite.Name)"
     $output += ""
    }
}

# Process Back sprites
if (Test-Path "$SpritePath\Back") {
  $backSprites = Get-ChildItem "$SpritePath\Back\*.png" -File
    
    foreach ($sprite in $backSprites) {
        $spriteName = $sprite.BaseName
    $output += "#begin Sprites/Back/$($sprite.Name)"
        $output += "/importer:TextureImporter"
        $output += "/processor:TextureProcessor"
        $output += "/processorParam:ColorKeyColor=255,0,255,255"
    $output += "/processorParam:ColorKeyEnabled=True"
     $output += "/processorParam:GenerateMipmaps=False"
        $output += "/processorParam:PremultiplyAlpha=True"
  $output += "/processorParam:ResizeToPowerOfTwo=False"
   $output += "/processorParam:MakeSquare=False"
        $output += "/processorParam:TextureFormat=Color"
   $output += "/build:Sprites/Back/$($sprite.Name)"
      $output += ""
    }
}

# Save to file
$outputFile = "sprite-content-entries.txt"
$output | Out-File $outputFile -Encoding UTF8

Write-Host "`nGenerated content entries saved to: $outputFile" -ForegroundColor Green
Write-Host "Total sprites found:" -ForegroundColor Yellow
Write-Host "  Front: $($frontSprites.Count)" -ForegroundColor Cyan
Write-Host "  Back: $($backSprites.Count)" -ForegroundColor Cyan
Write-Host "`nCopy the contents of $outputFile and append to Content.mgcb" -ForegroundColor Yellow
