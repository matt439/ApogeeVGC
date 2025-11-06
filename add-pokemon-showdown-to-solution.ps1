# PowerShell script to add pokemon-showdown folders to the solution
# Close the solution in Visual Studio before running this script

$solutionFile = "ApogeeVGC.sln"

# Read the current solution content
$content = Get-Content $solutionFile -Raw

# Find the insertion point (after the last Project line but before Global)
$insertionPoint = $content.IndexOf("Global")

# Define the new solution folders
$newContent = @"
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "pokemon-showdown", "pokemon-showdown", "{A1B2C3D4-E5F6-7890-ABCD-EF1234567890}"
EndProject
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "sim", "sim", "{B2C3D4E5-F6A7-8901-BCDE-F12345678901}"
	ProjectSection(SolutionItems) = preProject
		pokemon-showdown\sim\battle-actions.ts = pokemon-showdown\sim\battle-actions.ts
		pokemon-showdown\sim\battle-queue.ts = pokemon-showdown\sim\battle-queue.ts
		pokemon-showdown\sim\battle-stream.ts = pokemon-showdown\sim\battle-stream.ts
		pokemon-showdown\sim\battle.ts = pokemon-showdown\sim\battle.ts
		pokemon-showdown\sim\dex-abilities.ts = pokemon-showdown\sim\dex-abilities.ts
		pokemon-showdown\sim\dex-conditions.ts = pokemon-showdown\sim\dex-conditions.ts
		pokemon-showdown\sim\dex-data.ts = pokemon-showdown\sim\dex-data.ts
		pokemon-showdown\sim\dex-formats.ts = pokemon-showdown\sim\dex-formats.ts
		pokemon-showdown\sim\dex-items.ts = pokemon-showdown\sim\dex-items.ts
		pokemon-showdown\sim\dex-moves.ts = pokemon-showdown\sim\dex-moves.ts
		pokemon-showdown\sim\dex-species.ts = pokemon-showdown\sim\dex-species.ts
		pokemon-showdown\sim\DEX.md = pokemon-showdown\sim\DEX.md
		pokemon-showdown\sim\dex.ts = pokemon-showdown\sim\dex.ts
		pokemon-showdown\sim\field.ts = pokemon-showdown\sim\field.ts
		pokemon-showdown\sim\global-types.ts = pokemon-showdown\sim\global-types.ts
		pokemon-showdown\sim\index.ts = pokemon-showdown\sim\index.ts
		pokemon-showdown\sim\NONSTANDARD.md = pokemon-showdown\sim\NONSTANDARD.md
		pokemon-showdown\sim\pokemon.ts = pokemon-showdown\sim\pokemon.ts
		pokemon-showdown\sim\prng.ts = pokemon-showdown\sim\prng.ts
		pokemon-showdown\sim\README.md = pokemon-showdown\sim\README.md
		pokemon-showdown\sim\side.ts = pokemon-showdown\sim\side.ts
		pokemon-showdown\sim\TEAMS.md = pokemon-showdown\sim\TEAMS.md
		pokemon-showdown\sim\team-validator.ts = pokemon-showdown\sim\team-validator.ts
	EndProjectSection
EndProject
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "data", "data", "{C3D4E5F6-A7B8-9012-CDEF-123456789012}"
	ProjectSection(SolutionItems) = preProject
		pokemon-showdown\data\abilities.ts = pokemon-showdown\data\abilities.ts
		pokemon-showdown\data\aliases.ts = pokemon-showdown\data\aliases.ts
		pokemon-showdown\data\conditions.ts = pokemon-showdown\data\conditions.ts
		pokemon-showdown\data\formats-data.ts = pokemon-showdown\data\formats-data.ts
		pokemon-showdown\data\FORMES.md = pokemon-showdown\data\FORMES.md
		pokemon-showdown\data\items.ts = pokemon-showdown\data\items.ts
		pokemon-showdown\data\learnsets.ts = pokemon-showdown\data\learnsets.ts
		pokemon-showdown\data\moves.ts = pokemon-showdown\data\moves.ts
		pokemon-showdown\data\natures.ts = pokemon-showdown\data\natures.ts
		pokemon-showdown\data\pokedex.ts = pokemon-showdown\data\pokedex.ts
		pokemon-showdown\data\pokemongo.ts = pokemon-showdown\data\pokemongo.ts
		pokemon-showdown\data\rulesets.ts = pokemon-showdown\data\rulesets.ts
		pokemon-showdown\data\scripts.ts = pokemon-showdown\data\scripts.ts
		pokemon-showdown\data\tags.ts = pokemon-showdown\data\tags.ts
		pokemon-showdown\data\typechart.ts = pokemon-showdown\data\typechart.ts
	EndProjectSection
EndProject

"@

# Insert the new content
$before = $content.Substring(0, $insertionPoint)
$after = $content.Substring($insertionPoint)
$newSolutionContent = $before + $newContent + $after

# Find the NestedProjects section or create it
if ($newSolutionContent -match "GlobalSection\(NestedProjects\) = preSolution") {
    # Add to existing NestedProjects section
    $nestedProjectsEnd = $newSolutionContent.IndexOf("EndGlobalSection", $newSolutionContent.IndexOf("GlobalSection(NestedProjects)"))
    $nestingEntries = @"
		{B2C3D4E5-F6A7-8901-BCDE-F12345678901} = {A1B2C3D4-E5F6-7890-ABCD-EF1234567890}
		{C3D4E5F6-A7B8-9012-CDEF-123456789012} = {A1B2C3D4-E5F6-7890-ABCD-EF1234567890}

"@
    $before = $newSolutionContent.Substring(0, $nestedProjectsEnd)
    $after = $newSolutionContent.Substring($nestedProjectsEnd)
    $newSolutionContent = $before + $nestingEntries + $after
} else {
    # Create NestedProjects section
    $extensibilityGlobalsStart = $newSolutionContent.IndexOf("GlobalSection(ExtensibilityGlobals)")
    $nestedProjectsSection = @"
	GlobalSection(NestedProjects) = preSolution
		{B2C3D4E5-F6A7-8901-BCDE-F12345678901} = {A1B2C3D4-E5F6-7890-ABCD-EF1234567890}
		{C3D4E5F6-A7B8-9012-CDEF-123456789012} = {A1B2C3D4-E5F6-7890-ABCD-EF1234567890}
	EndGlobalSection

"@
    $before = $newSolutionContent.Substring(0, $extensibilityGlobalsStart)
    $after = $newSolutionContent.Substring($extensibilityGlobalsStart)
    $newSolutionContent = $before + $nestedProjectsSection + $after
}

# Write the updated content
Set-Content -Path $solutionFile -Value $newSolutionContent

Write-Host "Successfully added pokemon-showdown solution folder with /sim and /data subdirectories!"
Write-Host "Please reopen the solution in Visual Studio to see the changes."
