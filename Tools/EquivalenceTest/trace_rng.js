/**
 * Trace every PRNG call during battle replay to compare with C# RNG trace.
 * Usage: node trace_rng.js
 */
const path = require('path');
const fs = require('fs');

const showdownPath = fs.existsSync(path.resolve(__dirname, '../../../pokemon-showdown'))
    ? path.resolve(__dirname, '../../../pokemon-showdown')
    : path.resolve(__dirname, '../../pokemon-showdown');
const simPath = path.join(showdownPath, 'dist', 'sim');

const { BattleStream, getPlayerStreams } = require(path.join(simPath, 'index.js'));
const { Teams } = require(path.join(simPath, 'index.js'));

async function main() {
    const fixturePath = path.join(__dirname, 'battle_output.fixture.json');
    const fixture = JSON.parse(fs.readFileSync(fixturePath, 'utf8'));
    const seedStr = Array.isArray(fixture.seed) ? fixture.seed.join(',') : fixture.seed;

    const stream = new BattleStream({ debug: false });
    const streams = getPlayerStreams(stream);

    const protocolLines = [];
    const outputDone = (async () => {
        for await (const chunk of streams.omniscient) {
            for (const line of chunk.split('\n')) {
                protocolLines.push(line);
            }
        }
    })();

    const startCmd = JSON.stringify({ formatid: fixture.formatid, seed: seedStr });
    const p1Cmd = JSON.stringify({ name: 'Player1', team: fixture.p1Team || fixture.p1?.team });
    const p2Cmd = JSON.stringify({ name: 'Player2', team: fixture.p2Team || fixture.p2?.team });

    await streams.omniscient.write(
        `>start ${startCmd}\n>player p1 ${p1Cmd}\n>player p2 ${p2Cmd}`
    );

    // Patch the battle's PRNG to trace calls
    // We need to wait for the battle to be created
    await new Promise(r => setTimeout(r, 100));

    const battle = stream.battle;
    if (!battle) {
        console.error('No battle object found');
        process.exit(1);
    }

    let callNum = 0;
    const origRandom = battle.prng.random.bind(battle.prng);
    battle.prng.random = function(from, to) {
        const result = origRandom(from, to);
        // Get a short stack trace
        const stack = new Error().stack.split('\n').slice(2, 5)
            .map(l => l.trim().replace(/^at /, '').replace(/\(.*[\/\\]dist[\/\\]sim[\/\\]/g, '(').replace(/\(.*[\/\\]sim[\/\\]/g, '('))
            .filter(l => !l.includes('node_modules'));
        const caller = stack.slice(0, 3).join(' <- ') || '?';
        const args = to !== undefined ? `${from},${to}` : from !== undefined ? `${from}` : '';
        console.error(`RNG#${callNum}: random(${args})=${result} [${caller}]`);
        callNum++;
        return result;
    };

    // Also patch randomChance and sample
    const origRandomChance = battle.prng.randomChance.bind(battle.prng);
    battle.prng.randomChance = function(num, den) {
        const result = origRandomChance(num, den);
        return result; // random() inside will already be traced
    };

    // Patch shuffle to add context
    const origShuffle = battle.prng.shuffle.bind(battle.prng);
    battle.prng.shuffle = function(items, start, end) {
        const stack = new Error().stack.split('\n').slice(2, 5)
            .map(l => l.trim().replace(/^at /, '').replace(/\(.*[\/\\]dist[\/\\]sim[\/\\]/g, '(').replace(/\(.*[\/\\]sim[\/\\]/g, '('));
        console.error(`  [SHUFFLE] ${items.length} items, start=${start||0}, end=${end||items.length}, before RNG#${callNum} [${stack[0]}]`);
        return origShuffle(items, start, end);
    };

    // Patch battle.add to catch turn markers
    const origAdd = battle.add.bind(battle);
    battle.add = function(...args) {
        if (args[0] === 'turn') {
            console.error(`--- TURN ${args[1]} --- (after RNG#${callNum - 1})`);
        }
        return origAdd(...args);
    };

    // Feed choices
    const choices = fixture.inputLog || fixture.choices;
    if (choices) {
        for (const entry of choices) {
            if (typeof entry === 'string') {
                if (entry.startsWith('>p1 ') || entry.startsWith('>p2 ')) {
                    await streams.omniscient.write(entry);
                }
            }
        }
    }

    await outputDone;
}

main().catch(err => {
    console.error('Fatal error:', err);
    process.exit(1);
});
