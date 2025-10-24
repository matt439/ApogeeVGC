//using ApogeeVGC.Sim.PokemonClasses;
//using ApogeeVGC.Sim.Stats;

//boost(
//		boost: SparseBoostsTable, target: Pokemon | null = null, source: Pokemon | null = null,
//		effect: Effect | null = null, isSecondary = false, isSelf = false
//	) {
//	if (this.event) {
//		target ||= this.event.target;
//		source ||= this.event.source;
//		effect ||= this.effect;
//	}
//	if (!target?.hp) return 0;
//	if (!target.isActive) return false;
//	if (this.gen > 5 && !target.side.foePokemonLeft()) return false;
//	boost = this.runEvent('ChangeBoost', target, source, effect, { ...boost });
//	boost = target.getCappedBoost(boost);
//	boost = this.runEvent('TryBoost', target, source, effect, { ...boost });
//	let success = null;
//	let boosted = isSecondary;
//	let boostName: BoostID;
//	for (boostName in boost)
//	{
//		const currentBoost: SparseBoostsTable = {
//			[boostName]: boost[boostName],
//			}
//		;
//		let boostBy = target.boostBy(currentBoost);
//		let msg = '-boost';
//		if (boost[boostName]! < 0 || target.boosts[boostName] === -6)
//		{
//			msg = '-unboost';
//			boostBy = -boostBy;
//		}
//		if (boostBy)
//		{
//			success = true;
//			switch (effect?.id)
//			{
//				case 'bellydrum':
//				case 'angerpoint':
//					this.add('-setboost', target, 'atk', target.boosts['atk'], '[from] ' + effect.fullname);
//					break;
//				case 'bellydrum2':
//					this.add(msg, target, boostName, boostBy, '[silent]');
//					this.hint("In Gen 2, Belly Drum boosts by 2 when it fails.");
//					break;
//				case 'zpower':
//					this.add(msg, target, boostName, boostBy, '[zeffect]');
//					break;
//				default:
//					if (!effect) break;
//					if (effect.effectType === 'Move')
//					{
//						this.add(msg, target, boostName, boostBy);
//					}
//					else if (effect.effectType === 'Item')
//					{
//						this.add(msg, target, boostName, boostBy, '[from] item: ' + effect.name);
//					}
//					else
//					{
//						if (effect.effectType === 'Ability' && !boosted)
//						{
//							this.add('-ability', target, effect.name, 'boost');
//							boosted = true;
//						}
//						this.add(msg, target, boostName, boostBy);
//					}
//					break;
//			}
//			this.runEvent('AfterEachBoost', target, source, effect, currentBoost);
//		}
//		else if (effect?.effectType === 'Ability')
//		{
//			if (isSecondary || isSelf) this.add(msg, target, boostName, boostBy);
//		}
//		else if (!isSecondary && !isSelf)
//		{
//			this.add(msg, target, boostName, boostBy);
//		}
//	}
//	this.runEvent('AfterBoost', target, source, effect, boost);
//	if (success)
//	{
//		if (Object.values(boost).some(x => x > 0)) target.statsRaisedThisTurn = true;
//		if (Object.values(boost).some(x => x < 0)) target.statsLoweredThisTurn = true;
//	}
//	return success;
//}