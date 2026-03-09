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
const showdownPath = path.resolve(__dirname, '../../pokemon-showdown');
const simPath = path.join(showdownPath, '.sim-dist');

if (!fs.existsSync(simPath)) {
    console.error(`ERROR: Showdown not built. Run: cd ${showdownPath} && node build`);
    process.exit(1);
}

const { BattleStream, getPlayerStreams } = require(path.join(simPath, 'index.js'));
const { RandomPlayerAI } = require(path.join(simPath, 'tools', 'random-player-ai.js'));
const { Teams } = require(path.join(simPath, 'index.js'));

async function runRandom(formatid, seedStr, p1SeedStr, p2SeedStr, outputPath) {
    const stream = new BattleStream({ debug: false });
    const streams = getPlayerStreams(stream);

    // Generate random teams using Showdown's team generator
    const team1 = Teams.pack(Teams.generate(formatid));
    const team2 = Teams.pack(Teams.generate(formatid));

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

    await outputDone;

    // Write protocol to stdout
    for (const line of protocolLines) {
        console.log(line);
    }

    // Write input log to file
    if (stream.battle && outputPath) {
        const inputLog = stream.battle.inputLog.join('\n');
        fs.writeFileSync(outputPath + '.inputlog', inputLog);

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
