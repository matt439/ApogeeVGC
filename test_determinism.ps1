# PowerShell script to test determinism of the battle simulation
# Runs the program multiple times and compares outputs

$numRuns = 3
$outputDir = ".\determinism_test_outputs"

# Create output directory
if (Test-Path $outputDir) {
    Remove-Item -Path $outputDir -Recurse -Force
}
New-Item -ItemType Directory -Path $outputDir | Out-Null

Write-Host "Running battle simulation $numRuns times to test determinism..." -ForegroundColor Cyan
Write-Host ""

# Run the program multiple times and capture outputs
for ($i = 1; $i -le $numRuns; $i++) {
    Write-Host "Run $i of $numRuns..." -ForegroundColor Yellow
    
    $outputFile = Join-Path $outputDir "run_$i.txt"
    
    # Run the program and capture output
    dotnet run --project .\ApogeeVGC\ApogeeVGC.csproj 2>&1 | Out-File -FilePath $outputFile -Encoding UTF8
    
    Write-Host "Output saved to $outputFile" -ForegroundColor Green
    Write-Host ""
}

# Compare outputs
Write-Host "Comparing outputs..." -ForegroundColor Cyan
Write-Host ""

$allMatch = $true
$baselineFile = Join-Path $outputDir "run_1.txt"

for ($i = 2; $i -le $numRuns; $i++) {
    $compareFile = Join-Path $outputDir "run_$i.txt"
    
    Write-Host "Comparing run 1 with run $i..." -ForegroundColor Yellow
  
    $baseline = Get-Content -Path $baselineFile -Raw
    $compare = Get-Content -Path $compareFile -Raw
    
    if ($baseline -eq $compare) {
        Write-Host "? Run $i matches run 1" -ForegroundColor Green
    } else {
      Write-Host "? Run $i differs from run 1" -ForegroundColor Red
        $allMatch = $false
   
        # Show first difference
        $baselineLines = Get-Content -Path $baselineFile
     $compareLines = Get-Content -Path $compareFile
   
        $diffFound = $false
        for ($j = 0; $j -lt [Math]::Min($baselineLines.Count, $compareLines.Count); $j++) {
            if ($baselineLines[$j] -ne $compareLines[$j]) {
  Write-Host "  First difference at line $($j + 1):" -ForegroundColor Yellow
  Write-Host "    Run 1: $($baselineLines[$j])" -ForegroundColor White
        $runNum = $i
   Write-Host "    Run ${runNum}: $($compareLines[$j])" -ForegroundColor White
        $diffFound = $true
       break
     }
        }
   
        if (-not $diffFound) {
  Write-Host "  Files have different lengths:" -ForegroundColor Yellow
    Write-Host "    Run 1: $($baselineLines.Count) lines" -ForegroundColor White
            $runNum = $i
        Write-Host "    Run ${runNum}: $($compareLines.Count) lines" -ForegroundColor White
        }
    }
    Write-Host ""
}

# Summary
Write-Host "========================================" -ForegroundColor Cyan
if ($allMatch) {
    Write-Host "SUCCESS: All runs produced identical output!" -ForegroundColor Green
    Write-Host "The simulation is deterministic with fixed seeds." -ForegroundColor Green
} else {
    Write-Host "FAILURE: Runs produced different outputs!" -ForegroundColor Red
    Write-Host "The simulation is NOT deterministic." -ForegroundColor Red
    Write-Host "Check the output files in $outputDir for details." -ForegroundColor Red
}
Write-Host "========================================" -ForegroundColor Cyan
