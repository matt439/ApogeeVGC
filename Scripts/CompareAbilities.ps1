# Script to compare abilities between TS and C# implementations

# Read TypeScript abilities file
$tsFile = "c:\VSProjects\ApogeeVGC\pokemon-showdown\data\abilities.ts"
$tsContent = Get-Content $tsFile -Raw

# Extract ability names from TS (the keys in the object)
# Pattern: single tab followed by ability_name: {
# This ensures we only match top-level ability definitions, not nested properties
$tsAbilities = [regex]::Matches($tsContent, '^\t(\w+):\s*\{', [System.Text.RegularExpressions.RegexOptions]::Multiline) |
    ForEach-Object { $_.Groups[1].Value } |
    Where-Object { $_ -ne 'noability' } |
    Sort-Object -Unique

Write-Host "Found $($tsAbilities.Count) abilities in TypeScript file" -ForegroundColor Cyan
Write-Host ""

# Read C# AbilityId enum
$csEnumFile = "c:\VSProjects\ApogeeVGC\ApogeeVGC\Sim\Abilities\AbilityId.cs"
$csEnumContent = Get-Content $csEnumFile -Raw

# Extract ability IDs from C# enum (excluding None)
$csAbilities = [regex]::Matches($csEnumContent, '^\s+(\w+),?\s*$', [System.Text.RegularExpressions.RegexOptions]::Multiline) |
    ForEach-Object { $_.Groups[1].Value } |
    Where-Object { $_ -ne 'None' } |
    Sort-Object -Unique

Write-Host "Found $($csAbilities.Count) abilities in C# enum" -ForegroundColor Cyan
Write-Host ""

# Check for isNonstandard abilities in TS
# Read line by line to properly parse ability blocks
$lines = Get-Content $tsFile
$nonstandardAbilities = @()
$currentAbility = $null
$inAbilityBlock = $false

for ($i = 0; $i -lt $lines.Count; $i++) {
    $line = $lines[$i]

    # Check if this line starts a new ability definition (single tab, word, colon, brace)
    if ($line -match '^\t(\w+):\s*\{') {
        $currentAbility = $matches[1]
        $inAbilityBlock = $true
    }
    # Check if we're closing an ability block (single tab, closing brace)
    elseif ($line -match '^\t\},?\s*$') {
        $inAbilityBlock = $false
        $currentAbility = $null
    }
    # If we're in an ability block and find isNonstandard, mark it
    elseif ($inAbilityBlock -and $line -match 'isNonstandard:') {
        if ($currentAbility -and $currentAbility -ne 'noability') {
            $nonstandardAbilities += $currentAbility
        }
    }
}

Write-Host "Found $($nonstandardAbilities.Count) nonstandard abilities in TS (will be excluded):" -ForegroundColor Yellow
$nonstandardAbilities | Sort-Object | ForEach-Object { Write-Host "  - $_" -ForegroundColor Gray }
Write-Host ""

# Convert TS names to PascalCase to match C# naming
function Convert-ToPascalCase {
    param($name)
    # Split on underscores or treat as single word
    $words = $name -split '_'
    $result = ""
    foreach ($word in $words) {
        if ($word.Length -gt 0) {
            $result += $word.Substring(0,1).ToUpper() + $word.Substring(1).ToLower()
        }
    }
    return $result
}

# Find missing abilities
$missingAbilities = @()
foreach ($tsAbility in $tsAbilities) {
    # Skip nonstandard
    if ($nonstandardAbilities -contains $tsAbility) {
        continue
    }

    $pascalCase = Convert-ToPascalCase $tsAbility

    if ($csAbilities -notcontains $pascalCase) {
        $missingAbilities += @{
            TS = $tsAbility
            CS = $pascalCase
        }
    }
}

if ($missingAbilities.Count -eq 0) {
    Write-Host "All standard TS abilities are present in C#!" -ForegroundColor Green
} else {
    Write-Host "Abilities in TS but missing from C# ($($missingAbilities.Count)):" -ForegroundColor Red
    $missingAbilities | ForEach-Object {
        Write-Host "  - $($_.TS) -> $($_.CS)" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "Summary:" -ForegroundColor Cyan
Write-Host "  Total TS abilities: $($tsAbilities.Count)" -ForegroundColor White
Write-Host "  Nonstandard (excluded): $($nonstandardAbilities.Count)" -ForegroundColor White
Write-Host "  Standard TS abilities: $($tsAbilities.Count - $nonstandardAbilities.Count)" -ForegroundColor White
Write-Host "  C# abilities: $($csAbilities.Count)" -ForegroundColor White
Write-Host "  Missing: $($missingAbilities.Count)" -ForegroundColor $(if ($missingAbilities.Count -eq 0) { "Green" } else { "Red" })
