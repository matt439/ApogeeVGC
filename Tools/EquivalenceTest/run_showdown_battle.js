/**
 * Deterministic Showdown battle runner for equivalence testing.
 *
 * Two modes:
 *
 * 1. Random-player mode (no fixture needed):
 *    node run_showdown_battle.js --random --format gen9vgc2024regg --seed 1,2,3,4
 *    Runs a full battle with RandomPlayerAI on both sides.
 *    Outputs protocol log to stdout, input log to a .inputlog file.
 *
 * 2. Replay mode (fixture with pre-scripted choices):
 *    node run_showdown_battle.js <fixture.json>
 *
 * Output:
 *   stdout: full battle protocol log (one line per message)
 *   The input log is written to <output>.inputlog alongside the protocol.
 */

const path = require('path');
const fs = require('fs');

// Showdown must be built first: cd pokemon-showdown && node build
// Try sibling of ApogeeVGC repo first, then sibling within repo
const showdownPath = fs.existsSync(path.resolve(__dirname, '../../../pokemon-showdown'))
    ? path.resolve(__dirname, '../../../pokemon-showdown')
    : path.resolve(__dirname, '../../pokemon-showdown');
// Showdown builds to dist/sim/ (not .sim-dist)
const simPath = path.join(showdownPath, 'dist', 'sim');

if (!fs.existsSync(simPath)) {
    console.error(`ERROR: Showdown not built. Run: cd ${showdownPath} && node build`);
    process.exit(1);
}

const { BattleStream, getPlayerStreams } = require(path.join(simPath, 'index.js'));
const { RandomPlayerAI } = require(path.join(simPath, 'tools', 'random-player-ai.js'));
const { Teams } = require(path.join(simPath, 'index.js'));

function instrumentPrng(battle) {
    // Monkey-patch the PRNG to count calls
    if (battle && battle.prng) {
        const origRandom = battle.prng.random.bind(battle.prng);
        battle.prng._callCount = 0;
        battle.prng.random = function(...args) {
            battle.prng._callCount++;
            return origRandom(...args);
        };
    }
}

async function runRandom(formatid, seedStr, p1SeedStr, p2SeedStr, outputPath) {
    const stream = new BattleStream({ debug: false });
    const streams = getPlayerStreams(stream);

    // Generate random teams using Showdown's team generator with deterministic seeds
    // Derive team seeds from the battle seed to ensure reproducibility
    const seed = seedStr.split(',').map(Number);
    const team1Seed = [seed[0] ^ 0x1234, seed[1] ^ 0x5678, seed[2] ^ 0x9ABC, seed[3] ^ 0xDEF0];
    const team2Seed = [seed[0] ^ 0xFEDC, seed[1] ^ 0xBA98, seed[2] ^ 0x7654, seed[3] ^ 0x3210];
    const team1 = Teams.pack(Teams.generate(formatid, { seed: team1Seed }));
    const team2 = Teams.pack(Teams.generate(formatid, { seed: team2Seed }));

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

    // Instrument PRNG after battle is created (small delay for initialization)
    if (stream.battle) instrumentPrng(stream.battle);

    await outputDone;

    // Write protocol to stdout
    for (const line of protocolLines) {
        console.log(line);
    }

    // Dump PRNG call trace if requested
    if (stream.battle && process.env.PRNG_TRACE) {
        // The PRNG calls were already logged by our monkey-patch
    }

    // Write input log to file
    if (stream.battle && outputPath) {
        const inputLog = stream.battle.inputLog.join('\n');
        fs.writeFileSync(outputPath + '.inputlog', inputLog);

        // Get final PRNG state for equivalence verification
        let prngFinalSeed = null;
        let prngCallCount = null;
        try {
            prngFinalSeed = stream.battle.prng.getSeed();
            // Try to get call count if we instrumented the PRNG
            prngCallCount = stream.battle.prng._callCount || null;
        } catch (e) { /* ignore */ }

        // Write teams as unpacked JSON so C# can reconstruct them
        const teamsData = {
            formatid,
            seed: seedStr,
            p1Seed: p1SeedStr,
            p2Seed: p2SeedStr,
            p1TeamPacked: team1,
            p2TeamPacked: team2,
            p1Team: Teams.unpack(team1),
            p2Team: Teams.unpack(team2),
            inputLog: stream.battle.inputLog,
            prngFinalSeed: prngFinalSeed,
            prngCallCount: prngCallCount,
        };
        fs.writeFileSync(outputPath + '.fixture.json', JSON.stringify(teamsData, null, 2));
    }
}

async function runFixture(fixturePath) {
    const fixture = JSON.parse(fs.readFileSync(fixturePath, 'utf8'));
    const seedStr = Array.isArray(fixture.seed) ? fixture.seed.join(',') : fixture.seed;

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

    // Start battle
    const startCmd = JSON.stringify({ formatid: fixture.formatid, seed: seedStr });
    const p1Cmd = JSON.stringify({ name: fixture.p1?.name || 'Player1', team: fixture.p1Team || fixture.p1?.team });
    const p2Cmd = JSON.stringify({ name: fixture.p2?.name || 'Player2', team: fixture.p2Team || fixture.p2?.team });

    await streams.omniscient.write(
        `>start ${startCmd}\n>player p1 ${p1Cmd}\n>player p2 ${p2Cmd}`
    );

    // Feed choices from input log or choices array
    const choices = fixture.inputLog || fixture.choices;
    if (choices) {
        for (const entry of choices) {
            if (typeof entry === 'string') {
                // Input log format: ">p1 move fakeout 1, move protect"
                if (entry.startsWith('>p1 ') || entry.startsWith('>p2 ')) {
                    await streams.omniscient.write(entry);
                }
            } else {
                // Choices array format: { p1: "...", p2: "..." }
                if (entry.p1) await streams.omniscient.write(`>p1 ${entry.p1}`);
                if (entry.p2) await streams.omniscient.write(`>p2 ${entry.p2}`);
            }
        }
    }

    await outputDone;

    for (const line of protocolLines) {
        console.log(line);
    }
}

async function main() {
    const args = process.argv.slice(2);

    if (args.includes('--random')) {
        const formatIdx = args.indexOf('--format');
        const seedIdx = args.indexOf('--seed');
        const p1SeedIdx = args.indexOf('--p1seed');
        const p2SeedIdx = args.indexOf('--p2seed');
        const outIdx = args.indexOf('--out');

        const formatid = formatIdx >= 0 ? args[formatIdx + 1] : 'gen9vgc2024regg';
        const seed = seedIdx >= 0 ? args[seedIdx + 1] : '1,2,3,4';
        const p1Seed = p1SeedIdx >= 0 ? args[p1SeedIdx + 1] : '10,20,30,40';
        const p2Seed = p2SeedIdx >= 0 ? args[p2SeedIdx + 1] : '50,60,70,80';
        const output = outIdx >= 0 ? args[outIdx + 1] : 'battle_output';

        await runRandom(formatid, seed, p1Seed, p2Seed, output);
    } else if (args.length > 0) {
        await runFixture(args[0]);
    } else {
        console.error('Usage:');
        console.error('  Random: node run_showdown_battle.js --random --format gen9vgc2024regg --seed 1,2,3,4');
        console.error('  Fixture: node run_showdown_battle.js <fixture.json>');
        process.exit(1);
    }
}

main().catch(err => {
    console.error('Fatal error:', err);
    process.exit(1);
});
