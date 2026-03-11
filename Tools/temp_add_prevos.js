const fs = require('fs');
const path = require('path');

const ROOT = path.resolve(__dirname, '..');

// ============================================================
// 1. Re-derive missing Prevo assignments (same as temp_find_missing_prevos.js)
// ============================================================

const specieIdFile = fs.readFileSync(path.join(ROOT, 'ApogeeVGC/Sim/SpeciesClasses/SpecieId.cs'), 'utf8');
const specieIds = new Set();
for (const m of specieIdFile.matchAll(/^\s+(\w+),?\s*$/gm)) {
    specieIds.add(m[1]);
}

const dataDir = path.join(ROOT, 'ApogeeVGC/Data/SpeciesData');
const dataFiles = fs.readdirSync(dataDir).filter(f => f.endsWith('.cs') && f !== 'SpeciesData.cs');

const existingPrevos = new Set();

for (const file of dataFiles) {
    const content = fs.readFileSync(path.join(dataDir, file), 'utf8');
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

const pokedexFile = fs.readFileSync(path.join(ROOT, 'pokemon-showdown/data/pokedex.ts'), 'utf8');

function showdownNameToCSharp(name) {
    return name
        .replace(/[''']/g, '')
        .replace(/[éè]/g, 'e')
        .replace(/:/g, '')
        .replace(/\./g, '')
        .replace(/[ -]/g, '');
}

const showdownPrevos = [];
const lines2 = pokedexFile.split('\n');
let currentEntry = null;
let currentName = null;
let currentPrevo = null;
let currentNum = null;
let braceDepth = 0;

for (const line of lines2) {
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

const missing = [];
for (const entry of showdownPrevos) {
    const { species, prevo, num, rawName, rawPrevo } = entry;
    if (!specieIds.has(species)) continue;
    if (!specieIds.has(prevo)) continue;
    if (existingPrevos.has(species)) continue;
    missing.push({ species, prevo, num, rawName, rawPrevo });
}

function getFileForNum(num) {
    if (num < 1) return null;
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

const sortedFiles = Object.keys(grouped).sort();
for (const file of sortedFiles) {
    grouped[file].sort((a, b) => a.num - b.num || a.species.localeCompare(b.species));
}

console.log(`Total existing Prevo assignments: ${existingPrevos.size}`);
console.log(`Total Showdown species with prevo: ${showdownPrevos.length}`);
console.log(`Missing Prevo assignments to add: ${missing.length}\n`);

// ============================================================
// 2. Edit C# files to add missing Prevo assignments
// ============================================================

const INDENT = '                '; // 16 spaces

/**
 * Find the line range of a species entry block using brace-depth tracking.
 * Returns { startLine, endLine } (inclusive) where startLine is the
 * `[SpecieId.X] = new()` line and endLine is the outer closing `},` line.
 */
function findSpeciesBlock(lines, species) {
    // Find the entry header line
    let headerIdx = -1;
    for (let i = 0; i < lines.length; i++) {
        if (lines[i].match(new RegExp(`\\[SpecieId\\.${species}\\]\\s*=\\s*new`))) {
            headerIdx = i;
            break;
        }
    }
    if (headerIdx === -1) return null;

    // Find the opening { (should be the next line or on the same line)
    let openIdx = headerIdx;
    let depth = 0;
    for (let i = headerIdx; i < lines.length; i++) {
        for (const ch of lines[i]) {
            if (ch === '{') depth++;
            if (ch === '}') depth--;
        }
        if (depth === 0 && i > headerIdx) {
            return { startLine: headerIdx, endLine: i };
        }
    }
    return null;
}

let totalAdded = 0;

for (const file of sortedFiles) {
    const filePath = path.join(dataDir, file);
    let content = fs.readFileSync(filePath, 'utf8');
    let lines = content.split('\n');
    let addedInFile = 0;

    for (const entry of grouped[file]) {
        const { species, prevo } = entry;

        const block = findSpeciesBlock(lines, species);
        if (!block) {
            console.log(`  WARNING: Could not find block for SpecieId.${species} in ${file}`);
            continue;
        }

        // Check if already has Prevo
        const blockLines = lines.slice(block.startLine, block.endLine + 1);
        if (blockLines.some(l => l.match(/\bPrevo\s*=/))) {
            console.log(`  SKIP: SpecieId.${species} already has Prevo in ${file}`);
            continue;
        }

        const prevoLine = `${INDENT}Prevo = SpecieId.${prevo},`;

        // Find insertion point: after Color line at outer depth (depth 1 within block)
        // We need to track depth within the block to only match Color at the outermost property level
        let insertIdx = -1;
        let depth = 0;
        for (let i = block.startLine; i <= block.endLine; i++) {
            for (const ch of lines[i]) {
                if (ch === '{') depth++;
                if (ch === '}') depth--;
            }
            // At depth 1, we're at the top-level properties of this entry
            if (depth === 1 && lines[i].match(/^\s*Color\s*=\s*"[^"]*",\s*$/)) {
                insertIdx = i;
                break;
            }
        }

        if (insertIdx !== -1) {
            // Insert after Color line
            lines.splice(insertIdx + 1, 0, prevoLine);
        } else {
            // No Color line found - insert before the closing `},` of the outer block
            // The endLine is the closing `},`
            lines.splice(block.endLine, 0, prevoLine);
        }

        addedInFile++;
        // Re-derive lines since we modified the array (indices shifted)
        // No need to re-read; lines array is already updated
    }

    if (addedInFile > 0) {
        fs.writeFileSync(filePath, lines.join('\n'), 'utf8');
        console.log(`${file}: added ${addedInFile} Prevo assignments`);
    }
    totalAdded += addedInFile;
}

console.log(`\nTotal Prevo assignments added: ${totalAdded}`);

// ============================================================
// 3. Verification: spot-check a few edits
// ============================================================

console.log('\n=== Spot-check verification ===\n');

const toVerify = missing.slice(0, 8);
for (const entry of toVerify) {
    const file = getFileForNum(entry.num);
    if (!file) continue;
    const filePath = path.join(dataDir, file);
    const content = fs.readFileSync(filePath, 'utf8');
    const fileLines = content.split('\n');

    const block = findSpeciesBlock(fileLines, entry.species);
    if (!block) {
        console.log(`FAIL: Could not find block for SpecieId.${entry.species} in ${file}`);
        continue;
    }

    const blockLines = fileLines.slice(block.startLine, block.endLine + 1);
    const hasPrevo = blockLines.some(l => l.includes(`Prevo = SpecieId.${entry.prevo}`));

    if (hasPrevo) {
        console.log(`OK: SpecieId.${entry.species} -> Prevo = SpecieId.${entry.prevo}`);
        // Show the Prevo line with 1 line of context
        for (let i = 0; i < blockLines.length; i++) {
            if (blockLines[i].includes('Prevo =')) {
                const start = Math.max(0, i - 1);
                const end = Math.min(blockLines.length - 1, i + 1);
                for (let j = start; j <= end; j++) {
                    console.log(`  | ${blockLines[j]}`);
                }
                console.log('');
                break;
            }
        }
    } else {
        console.log(`FAIL: SpecieId.${entry.species} missing Prevo = SpecieId.${entry.prevo}`);
    }
}

// Final count verification
let totalPrevosAfter = 0;
for (const file of dataFiles) {
    const content = fs.readFileSync(path.join(dataDir, file), 'utf8');
    const matches = content.match(/Prevo\s*=\s*SpecieId\./g);
    if (matches) totalPrevosAfter += matches.length;
}
console.log(`Total Prevo assignments in C# files after edits: ${totalPrevosAfter}`);
console.log(`(was ${existingPrevos.size} before, added ${totalAdded})`);
