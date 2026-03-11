const fs = require('fs');
const path = require('path');

const ROOT = path.resolve(__dirname, '..');

// 1. Read all SpecieId enum values
const specieIdFile = fs.readFileSync(path.join(ROOT, 'ApogeeVGC/Sim/SpeciesClasses/SpecieId.cs'), 'utf8');
const specieIds = new Set();
for (const m of specieIdFile.matchAll(/^\s+(\w+),?\s*$/gm)) {
    specieIds.add(m[1]);
}

// 2. Read existing Prevo assignments from C# data files
// We need to find which SpecieId has a Prevo already set.
// Pattern: [SpecieId.Foo] = new() { ... Prevo = SpecieId.Bar ... }
const dataDir = path.join(ROOT, 'ApogeeVGC/Data/SpeciesData');
const dataFiles = fs.readdirSync(dataDir).filter(f => f.endsWith('.cs') && f !== 'SpeciesData.cs');

const existingPrevos = new Set(); // species that already have Prevo set

for (const file of dataFiles) {
    const content = fs.readFileSync(path.join(dataDir, file), 'utf8');
    // Find each species block and check if it has Prevo
    // We track the current species context
    const lines = content.split('\n');
    let currentSpecies = null;
    for (const line of lines) {
        const specMatch = line.match(/\[SpecieId\.(\w+)\]\s*=\s*new/);
        if (specMatch) {
            currentSpecies = specMatch[1];
        }
        const prevoMatch = line.match(/Prevo\s*=\s*SpecieId\.(\w+)/);
        if (prevoMatch && currentSpecies) {
            existingPrevos.add(currentSpecies);
        }
    }
}

// 3. Parse Showdown pokedex.ts for prevo fields
const pokedexFile = fs.readFileSync(path.join(ROOT, 'pokemon-showdown/data/pokedex.ts'), 'utf8');

// Convert Showdown name to C# SpecieId format
function showdownNameToCSharp(name) {
    // e.g. "Ivysaur" -> "Ivysaur", "Venusaur-Mega" -> "VenusaurMega",
    // "Rattata-Alola" -> "RattataAlola", "Wormadam-Sandy" -> "WormadamSandy"
    // "Nidoran-F" -> "NidoranF", "Mr. Mime" -> "MrMime"
    // "Farfetch'd" -> "Farfetchd", "Farfetch'd-Galar" -> "FarfetchdGalar"
    // "Ho-Oh" -> "HoOh", "Porygon-Z" -> "PorygonZ"
    // "Jangmo-o" -> "Jangmoo", "Type: Null" -> "TypeNull"
    // "Tapu Koko" -> "TapuKoko", "Flabébé" -> "Flabebe"
    return name
        .replace(/[''']/g, '')          // Remove apostrophes
        .replace(/[éè]/g, 'e')          // Normalize accented chars
        .replace(/:/g, '')              // Remove colons (Type: Null)
        .replace(/\./g, '')             // Remove periods (Mr. Mime, Jr.)
        .replace(/[ -]/g, '')           // Remove spaces and hyphens
        ;
}

// Parse the pokedex - extract species with prevo field using line-by-line parsing
const showdownPrevos = []; // { species: CSharpName, prevo: CSharpName, num: number }

const lines2 = pokedexFile.split('\n');
let currentEntry = null;
let currentName = null;
let currentPrevo = null;
let currentNum = null;
let braceDepth = 0;

for (const line of lines2) {
    // Top-level entry start: single tab, word, colon, opening brace
    const entryStart = line.match(/^\t(\w+):\s*\{/);
    if (entryStart && braceDepth === 0) {
        currentEntry = entryStart[1];
        currentName = null;
        currentPrevo = null;
        currentNum = null;
        braceDepth = 1;
        continue;
    }

    if (braceDepth >= 1) {
        // Count braces
        for (const ch of line) {
            if (ch === '{') braceDepth++;
            if (ch === '}') braceDepth--;
        }

        const nm = line.match(/\bname:\s*"([^"]+)"/);
        if (nm) currentName = nm[1];

        const pm = line.match(/\bprevo:\s*"([^"]+)"/);
        if (pm) currentPrevo = pm[1];

        const numm = line.match(/\bnum:\s*(-?\d+)/);
        if (numm) currentNum = parseInt(numm[1]);

        // Entry closed
        if (braceDepth === 0) {
            if (currentName && currentPrevo && currentNum !== null) {
                showdownPrevos.push({
                    species: showdownNameToCSharp(currentName),
                    prevo: showdownNameToCSharp(currentPrevo),
                    num: currentNum,
                    rawName: currentName,
                    rawPrevo: currentPrevo,
                });
            }
            currentEntry = null;
        }
    }
}

// 4. Cross-reference: find missing prevos
const missing = [];
for (const entry of showdownPrevos) {
    const { species, prevo, num, rawName, rawPrevo } = entry;

    // Both must exist in C# enum
    if (!specieIds.has(species)) continue;
    if (!specieIds.has(prevo)) continue;

    // Must not already have Prevo set
    if (existingPrevos.has(species)) continue;

    missing.push({ species, prevo, num, rawName, rawPrevo });
}

// 5. Group by file
function getFileForNum(num) {
    if (num < 1) return null;
    // Files are in ranges of 50: 0001to0050, 0051to0100, etc.
    // Special case: 1001To1050 uses capital T
    const start = Math.floor((num - 1) / 50) * 50 + 1;
    const end = start + 49;
    const pad = (n) => String(n).padStart(4, '0');
    if (start >= 1001) {
        return `SpeciesData${pad(start)}To${pad(end)}.cs`;
    }
    return `SpeciesData${pad(start)}to${pad(end)}.cs`;
}

const grouped = {};
for (const entry of missing) {
    const file = getFileForNum(entry.num);
    if (!file) continue;
    if (!grouped[file]) grouped[file] = [];
    grouped[file].push(entry);
}

// Sort files by number, entries within each file by num
const sortedFiles = Object.keys(grouped).sort();
for (const file of sortedFiles) {
    grouped[file].sort((a, b) => a.num - b.num || a.species.localeCompare(b.species));
}

// Output
console.log(`\nTotal existing Prevo assignments: ${existingPrevos.size}`);
console.log(`Total Showdown species with prevo: ${showdownPrevos.length}`);
console.log(`Missing Prevo assignments (both species exist in C# enum): ${missing.length}\n`);

for (const file of sortedFiles) {
    console.log(`=== ${file} ===`);
    for (const entry of grouped[file]) {
        console.log(`  SpecieId.${entry.species} -> Prevo = SpecieId.${entry.prevo}`);
    }
    console.log('');
}
