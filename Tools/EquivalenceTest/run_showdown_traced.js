/**
 * Showdown battle runner with RNG tracing — logs every PRNG.next() call for
 * debugging divergence with the C# sim.
 *
 * Usage: node run_showdown_traced.js <fixture.json>
 * Output: RNG trace to stderr, protocol to stdout
 */

const path = require('path');
const fs = require('fs');

const showdownPath = fs.existsSync(path.resolve(__dirname, '../../../pokemon-showdown'))
    ? path.resolve(__dirname, '../../../pokemon-showdown')
    : path.resolve(__dirname, '../../pokemon-showdown');
const simPath = path.join(showdownPath, 'dist', 'sim');

const { BattleStream, getPlayerStreams } = require(path.join(simPath, 'index.js'));
const { PRNG, Gen5RNG } = require(path.join(simPath, 'prng.js'));

// Monkey-patch Gen5RNG.next() to log every call with caller info
let callCount = 0;
const origNext = Gen5RNG.prototype.next;
Gen5RNG.prototype.next = function() {
    const result = origNext.call(this);
    callCount++;
    // Get caller from stack trace
    const stack = new Error().stack.split('\n');
    const caller = stack[2] ? stack[2].trim().replace(/^at /, '').split(' (')[0] : '?';
    console.error(`[RNG#${callCount}] raw=${result} caller=${caller}`);
    return result;
};

// Also patch PRNG.random to see the from/to parameters
const origRandom = PRNG.prototype.random;
PRNG.prototype.random = function(from, to) {
    const result = origRandom.call(this, from, to);
    const args = from !== undefined ? (to !== undefined ? `(${from},${to})` : `(${from})`) : '()';
    console.error(`  -> random${args} = ${result}`);
    return result;
};

async function main() {
    const fixturePath = process.argv[2];
    if (!fixturePath) {
        console.error('Usage: node run_showdown_traced.js <fixture.json>');
        process.exit(1);
    }

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
    const p1Packed = fixture.p1TeamPacked;
    const p2Packed = fixture.p2TeamPacked;
    const p1Cmd = JSON.stringify({ name: 'Player1', team: p1Packed });
    const p2Cmd = JSON.stringify({ name: 'Player2', team: p2Packed });

    await streams.omniscient.write(
        `>start ${startCmd}\n>player p1 ${p1Cmd}\n>player p2 ${p2Cmd}`
    );

    // Feed choices from input log
    const choices = fixture.inputLog || [];
    for (const entry of choices) {
        if (typeof entry === 'string') {
            if (entry.startsWith('>p1 ') || entry.startsWith('>p2 ')) {
                await streams.omniscient.write(entry);
            }
        }
    }

    await outputDone;

    for (const line of protocolLines) {
        console.log(line);
    }

    console.error(`\nTotal RNG calls: ${callCount}`);
}

main().catch(err => {
    console.error('Fatal error:', err);
    process.exit(1);
});
