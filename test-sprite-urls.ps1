# Test script to verify Showdown sprite URLs are accessible
# Run this to test before downloading all sprites

Write-Host "Testing Pokémon Showdown sprite URLs..." -ForegroundColor Green

$testSprites = @{
"bulbasaur" = "bulbasaur"
    "miraidon" = "miraidon"
}

$baseUrl = "https://play.pokemonshowdown.com/sprites"

foreach ($key in $testSprites.Keys) {
    $name = $testSprites[$key]
    
    Write-Host "`nTesting: $name" -ForegroundColor Yellow
    
    # Test front sprite URLs
    $frontUrls = @(
        "$baseUrl/gen5ani/$name.png",
        "$baseUrl/dex/$name.png"
    )
    
    foreach ($url in $frontUrls) {
      try {
            $response = Invoke-WebRequest -Uri $url -Method Head -ErrorAction Stop
          Write-Host "  ? Front available: $url" -ForegroundColor Green
            break
        }
    catch {
            Write-Host "  ? Front not found: $url" -ForegroundColor Red
   }
    }
    
    # Test back sprite URLs
    $backUrls = @(
      "$baseUrl/gen5ani-back/$name.png",
 "$baseUrl/gen5-back/$name.png"
    )
    
    foreach ($url in $backUrls) {
try {
    $response = Invoke-WebRequest -Uri $url -Method Head -ErrorAction Stop
     Write-Host "  ? Back available: $url" -ForegroundColor Green
 break
      }
        catch {
            Write-Host "  ? Back not found: $url" -ForegroundColor Red
        }
    }
}

Write-Host "`nTest complete!" -ForegroundColor Green
Write-Host "If sprites are available, run: .\copy-sprites.ps1" -ForegroundColor Cyan
