/**
 * One-time migration script: ingests existing batch_cache flat files into SQLite databases.
 *
 * Usage:
 *   cd Tools/EquivalenceTest
 *   npm install better-sqlite3   (if not already installed)
 *   node migrate_to_sqlite.js [--format gen9randombattle] [--format gen9randomdoublesbattle]
 *
 * If no --format is given, migrates all subdirectories found in batch_cache/.
 * Creates batch_cache/<format>.db for each format directory.
 * After verifying the .db files, you can delete the flat-file subdirectories.
 */

const path = require('path');
const fs = require('fs');
const Database = require('better-sqlite3');

function migrate(formatDir, dbPath) {
    const formatName = path.basename(formatDir);
    console.log(`Migrating ${formatName} -> ${dbPath}`);

    // Discover all battle indices
    const files = fs.readdirSync(formatDir);
    const indices = new Set();
    for (const f of files) {
        const m = f.match(/^battle_(\d+)\./);
        if (m) indices.add(parseInt(m[1], 10));
    }

    const sorted = [...indices].sort((a, b) => a - b);
    console.log(`  Found ${sorted.length} battles (indices ${sorted[0]}..${sorted[sorted.length - 1]})`);

    const db = new Database(dbPath);
    db.pragma('journal_mode = WAL');
    db.pragma('synchronous = OFF');     // safe for bulk import
    db.pragma('cache_size = -64000');   // 64 MB cache

    db.exec(`
        CREATE TABLE IF NOT EXISTS battles (
            id INTEGER PRIMARY KEY,
            fixture_json TEXT NOT NULL,
            log TEXT NOT NULL,
            inputlog TEXT
        );
        CREATE TABLE IF NOT EXISTS metadata (
            key TEXT PRIMARY KEY,
            value TEXT NOT NULL
        );
    `);

    const insert = db.prepare(`
        INSERT OR IGNORE INTO battles (id, fixture_json, log, inputlog)
        VALUES (?, ?, ?, ?)
    `);

    // Migrate showdown_version.txt if present
    const versionFile = path.join(formatDir, 'showdown_version.txt');
    if (fs.existsSync(versionFile)) {
        const version = fs.readFileSync(versionFile, 'utf-8').trim();
        db.prepare(`INSERT OR REPLACE INTO metadata (key, value) VALUES (?, ?)`)
            .run('showdown_version', version);
        console.log(`  Stored showdown_version: ${version}`);
    }

    // Batch insert in transactions of 1000
    const BATCH_SIZE = 1000;
    let migrated = 0;
    let skipped = 0;

    const insertBatch = db.transaction((batch) => {
        for (const entry of batch) {
            insert.run(entry.id, entry.fixture, entry.log, entry.inputlog);
        }
    });

    let batch = [];

    for (const idx of sorted) {
        const base = path.join(formatDir, `battle_${String(idx).padStart(6, '0')}`);
        const fixturePath = base + '.fixture.json';
        const logPath = base + '.log';

        if (!fs.existsSync(fixturePath) || !fs.existsSync(logPath)) {
            skipped++;
            continue;
        }

        const fixture = fs.readFileSync(fixturePath, 'utf-8');
        const log = fs.readFileSync(logPath, 'utf-8');
        const inputlogPath = base + '.inputlog';
        const inputlog = fs.existsSync(inputlogPath)
            ? fs.readFileSync(inputlogPath, 'utf-8')
            : null;

        batch.push({ id: idx, fixture, log, inputlog });

        if (batch.length >= BATCH_SIZE) {
            insertBatch(batch);
            migrated += batch.length;
            batch = [];

            if (migrated % 10000 === 0) {
                console.log(`  ${migrated}/${sorted.length} migrated...`);
            }
        }
    }

    // Flush remaining
    if (batch.length > 0) {
        insertBatch(batch);
        migrated += batch.length;
    }

    db.pragma('synchronous = NORMAL');
    db.close();

    console.log(`  Done: ${migrated} migrated, ${skipped} skipped (missing files)`);
    console.log(`  Database: ${dbPath}`);
}

function main() {
    const args = process.argv.slice(2);
    const cacheDir = path.join(__dirname, 'batch_cache');

    // Collect --format arguments
    const formats = [];
    for (let i = 0; i < args.length; i++) {
        if (args[i] === '--format' && i + 1 < args.length) {
            formats.push(args[++i]);
        }
    }

    // If none specified, auto-detect from subdirectories
    let targets = formats;
    if (targets.length === 0) {
        targets = fs.readdirSync(cacheDir).filter(f => {
            const full = path.join(cacheDir, f);
            return fs.statSync(full).isDirectory();
        });
    }

    if (targets.length === 0) {
        console.log('No format directories found in batch_cache/');
        return;
    }

    console.log(`Formats to migrate: ${targets.join(', ')}`);
    console.log();

    for (const fmt of targets) {
        const formatDir = path.join(cacheDir, fmt);
        const dbPath = path.join(cacheDir, `${fmt}.db`);

        if (!fs.existsSync(formatDir) || !fs.statSync(formatDir).isDirectory()) {
            console.log(`Skipping ${fmt}: directory not found`);
            continue;
        }

        migrate(formatDir, dbPath);
        console.log();
    }

    console.log('Migration complete.');
    console.log('After verifying the .db files work correctly, you can delete the flat-file subdirectories:');
    for (const fmt of targets) {
        console.log(`  rm -rf batch_cache/${fmt}/`);
    }
}

main();
