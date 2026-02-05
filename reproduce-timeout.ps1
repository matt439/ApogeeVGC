# Reproduce timeout issue with specific seeds
# Team 1 Seed:   56202
# Team 2 Seed:   69772
# Player 1 Seed: 14228
# Player 2 Seed: 3702
# Battle Seed:   11761

Write-Host "Building solution..." -ForegroundColor Yellow
dotnet build --configuration Debug

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "Running battle with problematic seeds..." -ForegroundColor Cyan
dotnet run --project ApogeeVGC --configuration Debug
