# PowerShell script to generate remaining EventHandlerInfo records
# This will create all missing Foe/Source/Ally/Any prefix variants

Write-Host "EventHandlerInfo Generator Script" -ForegroundColor Cyan
Write-Host "=================================" -ForegroundColor Cyan
Write-Host ""

# Check if dotnet-script is installed
$scriptInstalled = Get-Command dotnet-script -ErrorAction SilentlyContinue

if (-not $scriptInstalled) {
    Write-Host "??  dotnet-script not found. Installing..." -ForegroundColor Yellow
    dotnet tool install -g dotnet-script
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "? Failed to install dotnet-script" -ForegroundColor Red
      Write-Host "Please run: dotnet tool install -g dotnet-script" -ForegroundColor Yellow
        exit 1
  }
}

Write-Host "??  Running generator..." -ForegroundColor Green
Write-Host ""

# Run the C# script
dotnet script Scripts\GenerateEventHandlerInfoRecords.csx

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "? Generation completed successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "?? Next steps:" -ForegroundColor Cyan
    Write-Host "   1. Review the generated files in ApogeeVGC\Sim\Events\Handlers\EventMethods\" -ForegroundColor White
    Write-Host "   2. Run: dotnet build" -ForegroundColor White
    Write-Host "   3. Verify all files compile successfully" -ForegroundColor White
} else {
    Write-Host ""
    Write-Host "? Generation failed with errors" -ForegroundColor Red
    exit 1
}
