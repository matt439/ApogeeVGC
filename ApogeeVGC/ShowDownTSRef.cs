using ApogeeVGC.Sim.GameObjects;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;

//start() {
//	// Deserialized games should use restart()
//	if (this.deserialized) return;
//	// need all players to start
//	if (!this.sides.every(side => !!side)) throw new Error(`Missing sides: ${ this.sides }`);

//	if (this.started) throw new Error(`Battle already started`);

//	const format = this.format;
//	this.started = true;
//	if (this.gameType === 'multi')
//	{
//		this.sides[1].foe = this.sides[2]!;
//		this.sides[0].foe = this.sides[3]!;
//		this.sides[2]!.foe = this.sides[1];
//		this.sides[3]!.foe = this.sides[0];
//		this.sides[1].allySide = this.sides[3]!;
//		this.sides[0].allySide = this.sides[2]!;
//		this.sides[2]!.allySide = this.sides[0];
//		this.sides[3]!.allySide = this.sides[1];
//		// sync side conditions
//		this.sides[2]!.sideConditions = this.sides[0].sideConditions;
//		this.sides[3]!.sideConditions = this.sides[1].sideConditions;
//	}
//	else
//	{
//		this.sides[1].foe = this.sides[0];
//		this.sides[0].foe = this.sides[1];
//		if (this.sides.length > 2)
//		{ // ffa
//			this.sides[2]!.foe = this.sides[3]!;
//			this.sides[3]!.foe = this.sides[2]!;
//		}
//	}

//	this.add('gen', this.gen);

//	this.add('tier', format.name);
//	if (this.rated)
//	{
//		if (this.rated === 'Rated battle') this.rated = true;
//		this.add('rated', typeof this.rated === 'string' ? this.rated : '');
//	}

//	format.onBegin?.call(this);
//	for (const rule of this.ruleTable.keys()) {
//		if ('+*-!'.includes(rule.charAt(0))) continue;
//		const subFormat = this.dex.formats.get(rule);
//		subFormat.onBegin?.call(this);
//	}

//	if (this.sides.some(side => !side.pokemon[0]))
//	{
//		throw new Error('Battle not started: A player has an empty team.');
//	}

//	if (this.debugMode)
//	{
//		this.checkEVBalance();
//	}

//	if (format.customRules)
//	{
//		const plural = format.customRules.length === 1 ? '' : 's';
//		const open = format.customRules.length <= 5 ? ' open' : '';
//		this.add(`raw |< div class= "infobox" >< details class= "readmore"${ open}>< summary >< strong >${ format.customRules.length}
//custom rule${plural}:</ strong ></ summary > ${ format.customRules.join(', ')}</ details ></ div >`);
//		}

//		this.runPickTeam();
//this.queue.addChoice({ choice: 'start' });
//this.midTurn = true;
//if (!this.requestState) this.turnLoop();
//	}