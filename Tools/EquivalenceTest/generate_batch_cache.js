/**
 * Bulk fixture generator for equivalence testing.
 *
 * Generates all Showdown battle fixtures in a single Node.js process,
 * avoiding the ~200ms startup overhead per battle when spawning individually.
 *
 * Usage:
 *   node generate_batch_cache.js --count 100000 --format gen9randomdoublesbattle --outdir batch_cache
 *   node generate_batch_cache.js --count 100000 --start 50000  # resume from index 50000
 *
 * Writes to <outdir>/battle_NNNNNN.fixture.json and battle_NNNNNN.log
 * Skips battles that already have both files in the cache (for resumability).
 */

const path = require('path');
const fs = require('fs');

// Showdown setup
const showdownPath = fs.existsSync(path.resolve(__dirname, '../../../pokemon-showdown'))
    ? path.resolve(__dirname, '../../../pokemon-showdown')
    : path.resolve(__dirname, '../../pokemon-showdown');
const simPath = path.join(showdownPath, 'dist', 'sim');

if (!fs.existsSync(simPath)) {
    console.error(`ERROR: Showdown not built. Run: cd ${showdownPath} && node build`);
    process.exit(1);
}

const { BattleStream, getPlayerStreams } = require(path.join(simPath, 'index.js'));
const { RandomPlayerAI } = require(path.join(simPath, 'tools', 'random-player-ai.js'));
const { Teams } = require(path.join(simPath, 'index.js'));

function getSeedForIndex(i) {
    const s1 = (i * 7 + 1) % 65536;
    const s2 = (i * 13 + 2) % 65536;
    const s3 = (i * 19 + 3) % 65536;
    const s4 = (i * 31 + 4) % 65536;
    return `${s1},${s2},${s3},${s4}`;
}

function getP1SeedForIndex(i) {
    return `${(i * 41 + 10) % 65536},${(i * 43 + 20) % 65536},${(i * 47 + 30) % 65536},${(i * 53 + 40) % 65536}`;
}

function getP2SeedForIndex(i) {
    return `${(i * 59 + 50) % 65536},${(i * 61 + 60) % 65536},${(i * 67 + 70) % 65536},${(i * 71 + 80) % 65536}`;
}

async function generateBattle(formatid, i) {
    const seedStr = getSeedForIndex(i);
    const p1SeedStr = getP1SeedForIndex(i);
    const p2SeedStr = getP2SeedForIndex(i);

    const seed = seedStr.split(',').map(Number);
    const team1Seed = [seed[0] ^ 0x1234, seed[1] ^ 0x5678, seed[2] ^ 0x9ABC, seed[3] ^ 0xDEF0];
    const team2Seed = [seed[0] ^ 0xFEDC, seed[1] ^ 0xBA98, seed[2] ^ 0x7654, seed[3] ^ 0x3210];
    const team1 = Teams.pack(Teams.generate(formatid, { seed: team1Seed }));
    const team2 = Teams.pack(Teams.generate(formatid, { seed: team2Seed }));

    const stream = new BattleStream({ debug: false });
    const streams = getPlayerStreams(stream);

    // Collect protocol output
    const protocolLines = [];
    const outputDone = (async () => {
        for await (const chunk of streams.omniscient) {
            for (const line of chunk.split('\n')) {
                protocolLines.push(line);
            }
        }
    })();

    // Create seeded random players
    const p1 = new RandomPlayerAI(streams.p1, { seed: p1SeedStr });
    const p2 = new RandomPlayerAI(streams.p2, { seed: p2SeedStr });

    void p1.start();
    void p2.start();

    // Start battle
    const startCmd = JSON.stringify({ formatid, seed: seedStr });
    const p1Cmd = JSON.stringify({ name: 'Player1', team: team1 });
    const p2Cmd = JSON.stringify({ name: 'Player2', team: team2 });

    await streams.omniscient.write(
        `>start ${startCmd}\n>player p1 ${p1Cmd}\n>player p2 ${p2Cmd}`
    );

    // Instrument PRNG
    let prngCallCount = null;
    if (stream.battle && stream.battle.prng) {
        const origRandom = stream.battle.prng.random.bind(stream.battle.prng);
        stream.battle.prng._callCount = 0;
        stream.battle.prng.random = function (...args) {
            stream.battle.prng._callCount++;
            return origRandom(...args);
        };
    }

    await outputDone;

    // Get PRNG state
    let prngFinalSeed = null;
    try {
        prngFinalSeed = stream.battle.prng.getSeed();
        prngCallCount = stream.battle.prng._callCount || null;
    } catch (e) { /* ignore */ }

    // Build fixture
    const fixture = {
        formatid,
        seed: seedStr,
        p1Seed: p1SeedStr,
        p2Seed: p2SeedStr,
        p1TeamPacked: team1,
        p2TeamPacked: team2,
        p1Team: Teams.unpack(team1),
        p2Team: Teams.unpack(team2),
        inputLog: stream.battle.inputLog,
        prngFinalSeed,
        prngCallCount,
    };

    const log = protocolLines.join('\n') + '\n';

    return { fixture, log };
}

async function main() {
    const args = process.argv.slice(2);

    const countIdx = args.indexOf('--count');
    const formatIdx = args.indexOf('--format');
    const outdirIdx = args.indexOf('--outdir');
    const startIdx = args.indexOf('--start');
    const concurrencyIdx = args.indexOf('--concurrency');

    const count = countIdx >= 0 ? parseInt(args[countIdx + 1]) : 100000;
    const formatid = formatIdx >= 0 ? args[formatIdx + 1] : 'gen9randomdoublesbattle';
    const outdir = outdirIdx >= 0 ? args[outdirIdx + 1] : 'batch_cache';
    const startFrom = startIdx >= 0 ? parseInt(args[startIdx + 1]) : 0;
    const concurrency = concurrencyIdx >= 0 ? parseInt(args[concurrencyIdx + 1]) : 64;

    fs.mkdirSync(outdir, { recursive: true });

    console.log(`Generating ${count} battles (${formatid})`);
    console.log(`Output: ${outdir}/`);
    console.log(`Starting from index: ${startFrom}`);
    console.log(`Concurrency: ${concurrency}`);

    let generated = 0;
    let skipped = 0;
    let errors = 0;
    const startTime = Date.now();

    // Process in batches for concurrency control
    for (let batchStart = startFrom; batchStart < count; batchStart += concurrency) {
        const batchEnd = Math.min(batchStart + concurrency, count);
        const promises = [];

        for (let i = batchStart; i < batchEnd; i++) {
            const base = path.join(outdir, `battle_${String(i).padStart(6, '0')}`);
            const fixturePath = base + '.fixture.json';
            const logPath = base + '.log';

            // Skip if already cached
            if (fs.existsSync(fixturePath) && fs.existsSync(logPath)) {
                skipped++;
                continue;
            }

            promises.push(
                generateBattle(formatid, i)
                    .then(({ fixture, log }) => {
                        fs.writeFileSync(fixturePath, JSON.stringify(fixture, null, 2));
                        fs.writeFileSync(logPath, log);
                        generated++;
                    })
                    .catch(err => {
                        console.error(`  ERROR battle ${i}: ${err.message}`);
                        errors++;
                    })
            );
        }

        await Promise.all(promises);

        // Progress update every 1000 battles
        const total = generated + skipped + errors;
        if (total % 1000 < concurrency || batchStart + concurrency >= count) {
            const elapsed = (Date.now() - startTime) / 1000;
            const rate = generated > 0 ? (generated / elapsed).toFixed(1) : '?';
            const eta = generated > 0 ? (((count - startFrom - total) / (total / elapsed)) / 60).toFixed(1) : '?';
            console.log(`  [${total}/${count - startFrom}] generated=${generated} skipped=${skipped} errors=${errors} rate=${rate}/s ETA=${eta}min`);
        }
    }

    const elapsed = (Date.now() - startTime) / 1000;
    console.log();
    console.log(`Done in ${elapsed.toFixed(1)}s`);
    console.log(`  Generated: ${generated}`);
    console.log(`  Skipped (cached): ${skipped}`);
    console.log(`  Errors: ${errors}`);
}

main().catch(err => {
    console.error('Fatal error:', err);
    process.exit(1);
});
