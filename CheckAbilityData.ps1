# Script to check that every AbilityId has a corresponding data entry

# Read the AbilityId enum
$abilityIdPath = "c:\VSProjects\ApogeeVGC\ApogeeVGC\Sim\Abilities\AbilityId.cs"
$content = Get-Content $abilityIdPath -Raw

# Extract all ability IDs from the enum (excluding None)
$abilityIds = [regex]::Matches($content, '^\s+(\w+),?\s*$', [System.Text.RegularExpressions.RegexOptions]::Multiline) |
    ForEach-Object { $_.Groups[1].Value } |
    Where-Object { $_ -ne 'None' }

Write-Host "Found $($abilityIds.Count) abilities in AbilityId enum (excluding 'None')" -ForegroundColor Cyan
Write-Host ""

# Read all ability data files
$abilityDataFiles = @(
    "c:\VSProjects\ApogeeVGC\ApogeeVGC\Data\Abilities\AbilitiesABC.cs",
    "c:\VSProjects\ApogeeVGC\ApogeeVGC\Data\Abilities\AbilitiesDEF.cs",
    "c:\VSProjects\ApogeeVGC\ApogeeVGC\Data\Abilities\AbilitiesGHI.cs",
    "c:\VSProjects\ApogeeVGC\ApogeeVGC\Data\Abilities\AbilitiesJKL.cs",
    "c:\VSProjects\ApogeeVGC\ApogeeVGC\Data\Abilities\AbilitiesMNO.cs",
    "c:\VSProjects\ApogeeVGC\ApogeeVGC\Data\Abilities\AbilitiesPQR.cs",
    "c:\VSProjects\ApogeeVGC\ApogeeVGC\Data\Abilities\AbilitiesSTU.cs",
    "c:\VSProjects\ApogeeVGC\ApogeeVGC\Data\Abilities\AbilitiesVWX.cs",
    "c:\VSProjects\ApogeeVGC\ApogeeVGC\Data\Abilities\AbilitiesYZ.cs"
)

# Combine all ability data content
$allDataContent = ""
foreach ($file in $abilityDataFiles) {
    $allDataContent += Get-Content $file -Raw
    $allDataContent += "`n"
}

# Extract all ability entries from the data files
$dataEntries = [regex]::Matches($allDataContent, '\[AbilityId\.(\w+)\]') |
    ForEach-Object { $_.Groups[1].Value } |
    Sort-Object -Unique

Write-Host "Found $($dataEntries.Count) ability data entries in Abilities files" -ForegroundColor Cyan
Write-Host ""

# Find abilities in enum but not in data
$missingInData = $abilityIds | Where-Object { $_ -notin $dataEntries }

# Find abilities in data but not in enum
$missingInEnum = $dataEntries | Where-Object { $_ -notin $abilityIds }

# Results
if ($missingInData.Count -eq 0 -and $missingInEnum.Count -eq 0) {
    Write-Host "All abilities match! Every AbilityId has a data entry." -ForegroundColor Green
} else {
    if ($missingInData.Count -gt 0) {
        Write-Host "Abilities in AbilityId but MISSING from data files:" -ForegroundColor Red
        $missingInData | Sort-Object | ForEach-Object { Write-Host "  - $_" -ForegroundColor Yellow }
        Write-Host ""
    }

    if ($missingInEnum.Count -gt 0) {
        Write-Host "Abilities in data files but NOT in AbilityId enum:" -ForegroundColor Red
        $missingInEnum | Sort-Object | ForEach-Object { Write-Host "  - $_" -ForegroundColor Yellow }
        Write-Host ""
    }
}

Write-Host ""
Write-Host "Summary:" -ForegroundColor Cyan
Write-Host "  Total in AbilityId enum: $($abilityIds.Count)"
Write-Host "  Total in data files: $($dataEntries.Count)"
$missingColor = if ($missingInData.Count -eq 0) { "Green" } else { "Red" }
$extraColor = if ($missingInEnum.Count -eq 0) { "Green" } else { "Red" }
Write-Host "  Missing from data: $($missingInData.Count)" -ForegroundColor $missingColor
Write-Host "  Extra in data: $($missingInEnum.Count)" -ForegroundColor $extraColor
