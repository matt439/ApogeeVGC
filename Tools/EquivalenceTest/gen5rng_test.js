/**
 * Standalone Gen5RNG test — outputs random sequences for comparison with C# port.
 *
 * Usage: node gen5rng_test.js
 *
 * Outputs 100 raw next() values and 100 random(256) values for seed [1, 2, 3, 4].
 * Compare with C# Gen5RngVerificationTest output.
 */

// Inline Gen5RNG implementation (same as Showdown's prng.ts)
class Gen5RNG {
    constructor(seed) {
        this.seed = [...seed];
    }

    next() {
        this.seed = this.nextFrame(this.seed);
        return (this.seed[0] << 16 >>> 0) + this.seed[1];
    }

    random(from, to) {
        const result = this.next();
        if (from === undefined) {
            return result / 2 ** 32;
        } else if (to === undefined) {
            return Math.floor(result * from / 2 ** 32);
        } else {
            return Math.floor(result * (to - from) / 2 ** 32) + from;
        }
    }

    multiplyAdd(a, b, c) {
        const out = [0, 0, 0, 0];
        let carry = 0;
        for (let outIndex = 3; outIndex >= 0; outIndex--) {
            for (let bIndex = outIndex; bIndex < 4; bIndex++) {
                const aIndex = 3 - (bIndex - outIndex);
                carry += a[aIndex] * b[bIndex];
            }
            carry += c[outIndex];
            out[outIndex] = carry & 0xFFFF;
            carry >>>= 16;
        }
        return out;
    }

    nextFrame(seed, framesToAdvance = 1) {
        const a = [0x5D58, 0x8B65, 0x6C07, 0x8965];
        const c = [0, 0, 0x26, 0x9EC3];
        for (let i = 0; i < framesToAdvance; i++) {
            seed = this.multiplyAdd(seed, a, c);
        }
        return seed;
    }
}

// Test with seed [1, 2, 3, 4]
const rng = new Gen5RNG([1, 2, 3, 4]);

console.log('=== Raw next() values (100) ===');
const rawValues = [];
for (let i = 0; i < 100; i++) {
    const val = rng.next();
    rawValues.push(val);
    console.log(`${i}: ${val}`);
}

// Reset and test random(N)
const rng2 = new Gen5RNG([1, 2, 3, 4]);
console.log('\n=== random(256) values (100) ===');
for (let i = 0; i < 100; i++) {
    const val = rng2.random(256);
    console.log(`${i}: ${val}`);
}

// Reset and test random(min, max)
const rng3 = new Gen5RNG([1, 2, 3, 4]);
console.log('\n=== random(10, 20) values (50) ===');
for (let i = 0; i < 50; i++) {
    const val = rng3.random(10, 20);
    console.log(`${i}: ${val}`);
}
