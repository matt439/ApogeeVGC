# PowerShell script to extract and compare just the battle protocol output

$outputDir = ".\determinism_test_outputs"

# Function to extract just the lines that start with |
function Extract-BattleLines {
    param(
        [string]$filePath
    )
    
    Get-Content -Path $filePath | Where-Object { $_ -match '^\|' }
}

Write-Host "Extracting battle protocol lines from all runs..." -ForegroundColor Cyan
Write-Host ""

# Extract battle lines from each run
for ($i = 1; $i -le 3; $i++) {
    $inputFile = Join-Path $outputDir "run_$i.txt"
    $outputFile = Join-Path $outputDir "battle_$i.txt"
    
    Extract-BattleLines -filePath $inputFile | Out-File -FilePath $outputFile -Encoding UTF8
    
    Write-Host "Extracted battle lines from run $i" -ForegroundColor Green
}

Write-Host ""
Write-Host "Comparing battle protocol lines..." -ForegroundColor Cyan
Write-Host ""

$allMatch = $true
$baselineFile = Join-Path $outputDir "battle_1.txt"

for ($i = 2; $i -le 3; $i++) {
    $compareFile = Join-Path $outputDir "battle_$i.txt"
    
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
     Write-Host "  Run ${runNum}: $($compareLines[$j])" -ForegroundColor White
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
    Write-Host "SUCCESS: All runs produced identical battle protocol output!" -ForegroundColor Green
    Write-Host "The battle simulation is deterministic with fixed seeds." -ForegroundColor Green
    Write-Host ""
    Write-Host "Note: Console debug output may vary in order due to async execution," -ForegroundColor Yellow
    Write-Host "but the actual battle protocol (game logic) is deterministic." -ForegroundColor Yellow
} else {
    Write-Host "FAILURE: Runs produced different battle protocol outputs!" -ForegroundColor Red
    Write-Host "The battle simulation is NOT deterministic." -ForegroundColor Red
    Write-Host "Check the battle_*.txt files in $outputDir for details." -ForegroundColor Red
}
Write-Host "========================================" -ForegroundColor Cyan
