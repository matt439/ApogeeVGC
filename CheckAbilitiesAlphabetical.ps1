# Script to check that abilities are in alphabetical order within each file

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

$allGood = $true

foreach ($file in $abilityDataFiles) {
    $fileName = Split-Path $file -Leaf
    Write-Host ""
    Write-Host "Checking: $fileName" -ForegroundColor Cyan

    $content = Get-Content $file -Raw

    # Extract all ability entries from the file (only dictionary entries, not references)
    $abilityEntries = [regex]::Matches($content, '\[AbilityId\.(\w+)\]\s*=\s*new\(\)') |
        ForEach-Object { $_.Groups[1].Value }

    if ($abilityEntries.Count -eq 0) {
        Write-Host "  No abilities found" -ForegroundColor Yellow
        continue
    }

    # Check if sorted
    $sortedEntries = $abilityEntries | Sort-Object
    $outOfOrder = @()

    for ($i = 0; $i -lt $abilityEntries.Count; $i++) {
        if ($abilityEntries[$i] -ne $sortedEntries[$i]) {
            $outOfOrder += "  Position $($i + 1): Found '$($abilityEntries[$i])', Expected '$($sortedEntries[$i])'"
        }
    }

    if ($outOfOrder.Count -eq 0) {
        Write-Host "  All $($abilityEntries.Count) abilities are in alphabetical order" -ForegroundColor Green
    } else {
        Write-Host "  Abilities are NOT in alphabetical order:" -ForegroundColor Red
        $outOfOrder | ForEach-Object { Write-Host $_ -ForegroundColor Yellow }
        $allGood = $false

        Write-Host ""
        Write-Host "  Current order:" -ForegroundColor Yellow
        $abilityEntries | ForEach-Object { Write-Host "    - $_" -ForegroundColor Gray }
        Write-Host ""
        Write-Host "  Expected order:" -ForegroundColor Green
        $sortedEntries | ForEach-Object { Write-Host "    - $_" -ForegroundColor Gray }
    }
}

Write-Host ""
if ($allGood) {
    Write-Host "All ability files are properly alphabetized!" -ForegroundColor Green
} else {
    Write-Host "Some files have abilities out of order" -ForegroundColor Red
}
