//tiebreak() {
//    if (this.ended) return false;

//    this.inputLog.push(`> tiebreak`);
//    this.add('message', "Time's up! Going to tiebreaker...");
//    const notFainted = this.sides.map(side => (
//        side.pokemon.filter(pokemon => !pokemon.fainted).length
//    ));
//    this.add('-message', this.sides.map((side, i) => (
//        `${ side.name}: ${ notFainted[i]}
//    Pokemon left`
//        )).join('; '));
//    const maxNotFainted = Math.max(...notFainted);
//    let tiedSides = this.sides.filter((side, i) => notFainted[i] === maxNotFainted);
//    if (tiedSides.length <= 1)
//    {
//        return this.win(tiedSides[0]);
//    }

//    const hpPercentage = tiedSides.map(side => (
//        side.pokemon.map(pokemon => pokemon.hp / pokemon.maxhp).reduce((a, b) => a + b) * 100 / 6
//    ));
//    this.add('-message', tiedSides.map((side, i) => (
//        `${ side.name}: ${ Math.round(hpPercentage[i])}% total HP left`
//        )).join('; '));
//    const maxPercentage = Math.max(...hpPercentage);
//    tiedSides = tiedSides.filter((side, i) => hpPercentage[i] === maxPercentage);
//    if (tiedSides.length <= 1)
//    {
//        return this.win(tiedSides[0]);
//    }

//    const hpTotal = tiedSides.map(side => (
//        side.pokemon.map(pokemon => pokemon.hp).reduce((a, b) => a + b)
//    ));
//    this.add('-message', tiedSides.map((side, i) => (
//        `${ side.name}: ${ Math.round(hpTotal[i])}
//    total HP left`
//        )).join('; '));
//    const maxTotal = Math.max(...hpTotal);
//    tiedSides = tiedSides.filter((side, i) => hpTotal[i] === maxTotal);
//    if (tiedSides.length <= 1)
//    {
//        return this.win(tiedSides[0]);
//    }
//    return this.tie();
//}