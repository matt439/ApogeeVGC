const fs = require('fs');
const path = require('path');

// Read the pokedex.ts file
const filePath = path.join(__dirname, '..', 'pokemon-showdown', 'data', 'pokedex.ts');
const content = fs.readFileSync(filePath, 'utf8');

// Strip the TypeScript export/type annotation to get valid JS object literal
// Replace the export line with a variable assignment
const jsContent = content
  .replace(/^export const Pokedex:.*?= \{/, 'var Pokedex = {')
  // Remove any trailing semicolons after the closing brace
  .replace(/\};\s*$/, '};');

// Evaluate to get the object
eval(jsContent);

// Convert a Pokemon name to PascalCase enum style
// e.g. "Bulbasaur" -> "Bulbasaur", "Mr. Mime" -> "MrMime", "Roaring Moon" -> "RoaringMoon"
function toPascalCase(name) {
  // Remove special characters and split on spaces/hyphens/dots
  return name
    .replace(/['.:%]/g, '')
    .split(/[\s\-]+/)
    .map(word => word.charAt(0).toUpperCase() + word.slice(1))
    .join('');
}

// Output prevo relationships
console.log("=== PREVO RELATIONSHIPS ===");
for (const [id, data] of Object.entries(Pokedex)) {
  if (data.prevo) {
    const pokeName = toPascalCase(data.name);
    const prevoName = toPascalCase(data.prevo);
    console.log(`${pokeName} ${prevoName}`);
  }
}

console.log("\n=== EVOS RELATIONSHIPS ===");
for (const [id, data] of Object.entries(Pokedex)) {
  if (data.evos && data.evos.length > 0) {
    const pokeName = toPascalCase(data.name);
    const evoNames = data.evos.map(e => toPascalCase(e)).join(',');
    console.log(`${pokeName} evos ${evoNames}`);
  }
}
