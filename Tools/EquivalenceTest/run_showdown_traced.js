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
    const caller = stack[2].trim().replace(/^at /, '').split(' (')[0];
    console.error(`[RNG#${callCount}] raw=${result} caller=${caller}`);
    return result;
};

// Also patch PRNG.random to see the from/to parameters and deep caller
const origRandom = PRNG.prototype.random;
PRNG.prototype.random = function(from, to) {
    const result = origRandom.call(this, from, to);
    const args = from !== undefined ? (to !== undefined ? `(${from},${to})` : `(${from})`) : '()';
    // Get deep caller stack (skip Error, PRNG.random, PRNG.random wrapper)
    const stack = new Error().stack.split('\n');
    const callers = stack.slice(2, 8).map(s => s.trim().replace(/^at /, '').split(' (')[0]).join(' <- ');
    console.error(`  -> random${args} = ${result}  [${callers}]`);
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

    // Monkey-patch Pokemon.addVolatile to trace stall volatile changes
    const { Pokemon } = require(path.join(simPath, 'pokemon.js'));
    const origAddVolatile = Pokemon.prototype.addVolatile;
    Pokemon.prototype.addVolatile = function(status, source, sourceEffect, linkedStatus) {
        const statusId = typeof status === 'string' ? status : status.id;
        if (statusId === 'stall') {
            const existing = this.volatiles['stall'];
            if (existing) {
                console.error(`[STALL] ${this.name}: addVolatile('stall') RESTART - counter=${existing.counter}, duration=${existing.duration}`);
            } else {
                console.error(`[STALL] ${this.name}: addVolatile('stall') NEW`);
            }
        }
        const result = origAddVolatile.call(this, status, source, sourceEffect, linkedStatus);
        if (statusId === 'stall' && this.volatiles['stall']) {
            console.error(`[STALL] ${this.name}: after addVolatile - counter=${this.volatiles['stall'].counter}, duration=${this.volatiles['stall'].duration}`);
        }
        return result;
    };

    // Monkey-patch Battle.onStallMove to trace which Pokemon the check is for
    const { Battle } = require(path.join(simPath, 'battle.js'));
    const origRunEvent = Battle.prototype.runEvent;
    Battle.prototype.runEvent = function(eventid, target, source, sourceEffect, relayVar) {
        if (eventid === 'StallMove') {
            const pokeName = target?.name || target?.toString() || 'unknown';
            const hasStall = target?.volatiles?.['stall'] ? 'YES' : 'NO';
            const counter = target?.volatiles?.['stall']?.counter || 'N/A';
            console.error(`[STALLMOVE-CHECK] ${pokeName}: hasStall=${hasStall}, counter=${counter}, turn=${this.turn}`);
        }
        return origRunEvent.call(this, eventid, target, source, sourceEffect, relayVar);
    };

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
