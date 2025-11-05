# Script to add pokemon-showdown-client solution folder to ApogeeVGC.sln
# Make sure the solution is closed in Visual Studio before running this script

$solutionFile = "ApogeeVGC.sln"

# Read the solution file
$content = Get-Content $solutionFile -Raw

# Define the new projects to add (before the Global section)
$newProjects = @"
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "pokemon-showdown-client", "pokemon-showdown-client", "{A1B2C3D4-E5F6-7890-ABCD-EF1234567890}"
	ProjectSection(SolutionItems) = preProject
		pokemon-showdown-client\.gitignore = pokemon-showdown-client\.gitignore
		pokemon-showdown-client\package.json = pokemon-showdown-client\package.json
		pokemon-showdown-client\README.md = pokemon-showdown-client\README.md
	EndProjectSection
EndProject
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "js", "js", "{B2C3D4E5-F6A7-8901-BCDE-F12345678901}"
	ProjectSection(SolutionItems) = preProject
		pokemon-showdown-client\play.pokemonshowdown.com\js\client-battle.js = pokemon-showdown-client\play.pokemonshowdown.com\js\client-battle.js
		pokemon-showdown-client\play.pokemonshowdown.com\js\battle.js = pokemon-showdown-client\play.pokemonshowdown.com\js\battle.js
		pokemon-showdown-client\play.pokemonshowdown.com\js\battle-animations.js = pokemon-showdown-client\play.pokemonshowdown.com\js\battle-animations.js
		pokemon-showdown-client\play.pokemonshowdown.com\js\battle-animations-moves.js = pokemon-showdown-client\play.pokemonshowdown.com\js\battle-animations-moves.js
		pokemon-showdown-client\play.pokemonshowdown.com\js\battle-choices.js = pokemon-showdown-client\play.pokemonshowdown.com\js\battle-choices.js
		pokemon-showdown-client\play.pokemonshowdown.com\js\battle-dex.js = pokemon-showdown-client\play.pokemonshowdown.com\js\battle-dex.js
		pokemon-showdown-client\play.pokemonshowdown.com\js\battle-dex-data.js = pokemon-showdown-client\play.pokemonshowdown.com\js\battle-dex-data.js
		pokemon-showdown-client\play.pokemonshowdown.com\js\battle-log.js = pokemon-showdown-client\play.pokemonshowdown.com\js\battle-log.js
		pokemon-showdown-client\play.pokemonshowdown.com\js\battle-scene-stub.js = pokemon-showdown-client\play.pokemonshowdown.com\js\battle-scene-stub.js
		pokemon-showdown-client\play.pokemonshowdown.com\js\battle-sound.js = pokemon-showdown-client\play.pokemonshowdown.com\js\battle-sound.js
		pokemon-showdown-client\play.pokemonshowdown.com\js\battle-text-parser.js = pokemon-showdown-client\play.pokemonshowdown.com\js\battle-text-parser.js
		pokemon-showdown-client\play.pokemonshowdown.com\js\battle-tooltips.js = pokemon-showdown-client\play.pokemonshowdown.com\js\battle-tooltips.js
		pokemon-showdown-client\play.pokemonshowdown.com\js\battledata.js = pokemon-showdown-client\play.pokemonshowdown.com\js\battledata.js
		pokemon-showdown-client\play.pokemonshowdown.com\js\client.js = pokemon-showdown-client\play.pokemonshowdown.com\js\client.js
		pokemon-showdown-client\play.pokemonshowdown.com\js\client-connection.js = pokemon-showdown-client\play.pokemonshowdown.com\js\client-connection.js
		pokemon-showdown-client\play.pokemonshowdown.com\js\client-core.js = pokemon-showdown-client\play.pokemonshowdown.com\js\client-core.js
		pokemon-showdown-client\play.pokemonshowdown.com\js\client-main.js = pokemon-showdown-client\play.pokemonshowdown.com\js\client-main.js
	EndProjectSection
EndProject
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "client-data", "client-data", "{C3D4E5F6-A7B8-9012-CDEF-123456789012}"
	ProjectSection(SolutionItems) = preProject
		pokemon-showdown-client\play.pokemonshowdown.com\data\abilities.js = pokemon-showdown-client\play.pokemonshowdown.com\data\abilities.js
		pokemon-showdown-client\play.pokemonshowdown.com\data\aliases.js = pokemon-showdown-client\play.pokemonshowdown.com\data\aliases.js
		pokemon-showdown-client\play.pokemonshowdown.com\data\formats-data.js = pokemon-showdown-client\play.pokemonshowdown.com\data\formats-data.js
		pokemon-showdown-client\play.pokemonshowdown.com\data\items.js = pokemon-showdown-client\play.pokemonshowdown.com\data\items.js
		pokemon-showdown-client\play.pokemonshowdown.com\data\moves.js = pokemon-showdown-client\play.pokemonshowdown.com\data\moves.js
		pokemon-showdown-client\play.pokemonshowdown.com\data\pokedex.js = pokemon-showdown-client\play.pokemonshowdown.com\data\pokedex.js
		pokemon-showdown-client\play.pokemonshowdown.com\data\typechart.js = pokemon-showdown-client\play.pokemonshowdown.com\data\typechart.js
	EndProjectSection
EndProject
"@

# Add new nested projects section
$newNestingEntries = @"
		{B2C3D4E5-F6A7-8901-BCDE-F12345678901} = {A1B2C3D4-E5F6-7890-ABCD-EF1234567890}
		{C3D4E5F6-A7B8-9012-CDEF-123456789012} = {A1B2C3D4-E5F6-7890-ABCD-EF1234567890}
"@

# Insert new projects before the Global section
$content = $content -replace "(EndProject\s+Global)", "$newProjects`r`nGlobal"

# Insert new nesting entries in the NestedProjects section
$content = $content -replace "(\s+\{711F7807-C3DF-4241-A073-EB51552D8865\} = \{FAB885D4-E08F-4809-9763-9B97788E28E5\})", "`$1`r`n$newNestingEntries"

# Save the modified content
Set-Content $solutionFile $content -NoNewline

Write-Host "Successfully added pokemon-showdown-client solution folder!" -ForegroundColor Green
Write-Host ""
Write-Host "Added folders:" -ForegroundColor Cyan
Write-Host "  - pokemon-showdown-client (root)" -ForegroundColor White
Write-Host "    - js (client-battle.js and other battle/client JS files)" -ForegroundColor White
Write-Host "    - client-data (data files like abilities, moves, pokedex, etc.)" -ForegroundColor White
Write-Host ""
Write-Host "You can now reopen the solution in Visual Studio." -ForegroundColor Green
