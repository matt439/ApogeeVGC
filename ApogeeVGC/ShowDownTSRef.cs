//setPlayer(slot: SideID, options: PlayerOptions) {
//    let side;
//    let didSomething = true;
//    const slotNum = parseInt(slot[1]) - 1;
//    if (!this.sides[slotNum])
//    {
//        // create player
//        const team = this.getTeam(options);
//        side = new Side(options.name || `Player ${ slotNum + 1 }`, this, slotNum, team);
//        if (options.avatar) side.avatar = `${ options.avatar}`;
//        this.sides[slotNum] = side;
//    }
//    else
//    {
//        // edit player
//        side = this.sides[slotNum];
//        didSomething = false;
//        if (options.name && side.name !== options.name)
//        {
//            side.name = options.name;
//            didSomething = true;
//        }
//        if (options.avatar && side.avatar !== `${ options.avatar}`) {
//            side.avatar = `${ options.avatar}`;
//            didSomething = true;
//        }
//        if (options.team) throw new Error(`Player ${ slot } already has a team!`);
//    }
//    if (options.team && typeof options.team !== 'string')
//    {
//        options.team = Teams.pack(options.team);
//    }
//    if (!didSomething) return;
//    this.inputLog.push(`> player ${ slot} ` +JSON.stringify(options));
//    this.add('player', side.id, side.name, side.avatar, options.rating || '');

//    // Start the battle if it's ready to start
//    if (this.sides.every(playerSide => !!playerSide) && !this.started) this.start();
//}