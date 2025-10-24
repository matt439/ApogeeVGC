//spreadDamage(
//		damage: SpreadMoveDamage, targetArray: (false | Pokemon | null)[] | null = null,
//		source: Pokemon | null = null, effect: 'drain' | 'recoil' | Effect | null = null, instafaint = false
//	) {
//		if (!targetArray) return [0];
//		const retVals: (number | false | undefined)[] = [];
//		if (typeof effect === 'string' || !effect) effect = this.dex.conditions.getByID((effect || '') as ID);
//		for (const [i, curDamage] of damage.entries()) {
//			const target = targetArray[i];
//			let targetDamage = curDamage;
//			if (!(targetDamage || targetDamage === 0)) {
//				retVals[i] = targetDamage;
//				continue;
//			}
//			if (!target || !target.hp) {
//				retVals[i] = 0;
//				continue;
//			}
//			if (!target.isActive) {
//				retVals[i] = false;
//				continue;
//			}
//			if (targetDamage !== 0) targetDamage = this.clampIntRange(targetDamage, 1);

//			if (effect.id !== 'struggle-recoil') { // Struggle recoil is not affected by effects
//				if (effect.effectType === 'Weather' && !target.runStatusImmunity(effect.id)) {
//					this.debug('weather immunity');
//					retVals[i] = 0;
//					continue;
//				}
//				targetDamage = this.runEvent('Damage', target, source, effect, targetDamage, true);
//				if (!(targetDamage || targetDamage === 0)) {
//					this.debug('damage event failed');
//					retVals[i] = curDamage === true ? undefined : targetDamage;
//					continue;
//				}
//			}
//			if (targetDamage !== 0) targetDamage = this.clampIntRange(targetDamage, 1);

//			if (this.gen <= 1) {
//				if (this.dex.currentMod === 'gen1stadium' ||
//					!['recoil', 'drain', 'leechseed'].includes(effect.id) && effect.effectType !== 'Status') {
//					this.lastDamage = targetDamage;
//				}
//			}

//			retVals[i] = targetDamage = target.damage(targetDamage, source, effect);
//			if (targetDamage !== 0) target.hurtThisTurn = target.hp;
//			if (source && effect.effectType === 'Move') source.lastDamage = targetDamage;

//			const name = effect.fullname === 'tox' ? 'psn' : effect.fullname;
//			switch (effect.id) {
//			case 'partiallytrapped':
//				this.add('-damage', target, target.getHealth, '[from] ' + target.volatiles['partiallytrapped'].sourceEffect.fullname, '[partiallytrapped]');
//				break;
//			case 'powder':
//				this.add('-damage', target, target.getHealth, '[silent]');
//				break;
//			case 'confused':
//				this.add('-damage', target, target.getHealth, '[from] confusion');
//				break;
//			default:
//				if (effect.effectType === 'Move' || !name) {
//					this.add('-damage', target, target.getHealth);
//				} else if (source && (source !== target || effect.effectType === 'Ability')) {
//					this.add('-damage', target, target.getHealth, `[from] ${name}`, `[of] ${source}`);
//				} else {
//					this.add('-damage', target, target.getHealth, `[from] ${name}`);
//				}
//				break;
//			}

//			if (targetDamage && effect.effectType === 'Move') {
//				if (this.gen <= 1 && effect.recoil && source) {
//					if (this.dex.currentMod !== 'gen1stadium' || target.hp > 0) {
//						const amount = this.clampIntRange(Math.floor(targetDamage * effect.recoil[0] / effect.recoil[1]), 1);
//						this.damage(amount, source, target, 'recoil');
//					}
//				}
//				if (this.gen <= 4 && effect.drain && source) {
//					const amount = this.clampIntRange(Math.floor(targetDamage * effect.drain[0] / effect.drain[1]), 1);
//					// Draining can be countered in gen 1
//					if (this.gen <= 1) this.lastDamage = amount;
//					this.heal(amount, source, target, 'drain');
//				}
//				if (this.gen > 4 && effect.drain && source) {
//					const amount = Math.round(targetDamage * effect.drain[0] / effect.drain[1]);
//					this.heal(amount, source, target, 'drain');
//				}
//			}
//		}

//		if (instafaint) {
//			for (const [i, target] of targetArray.entries()) {
//				if (!retVals[i] || !target) continue;

//				if (target.hp <= 0) {
//					this.debug(`instafaint: ${this.faintQueue.map(entry => entry.target.name)}`);
//					this.faintMessages(true);
//					if (this.gen <= 2) {
//						target.faint();
//						if (this.gen <= 1) {
//							this.queue.clear();
//							// Fainting clears accumulated Bide damage
//							for (const pokemon of this.getAllActive()) {
//								if (pokemon.volatiles['bide']?.damage) {
//									pokemon.volatiles['bide'].damage = 0;
//									this.hint("Desync Clause Mod activated!");
//									this.hint("In Gen 1, Bide's accumulated damage is reset to 0 when a Pokemon faints.");
//								}
//							}
//						}
//					}
//				}
//			}
//		}

//		return retVals;
//	}spreadDamage(
//		damage: SpreadMoveDamage, targetArray: (false | Pokemon | null)[] | null = null,
//		source: Pokemon | null = null, effect: 'drain' | 'recoil' | Effect | null = null, instafaint = false
//	) {
//		if (!targetArray) return [0];
//		const retVals: (number | false | undefined)[] = [];
//		if (typeof effect === 'string' || !effect) effect = this.dex.conditions.getByID((effect || '') as ID);
//		for (const [i, curDamage] of damage.entries()) {
//			const target = targetArray[i];
//			let targetDamage = curDamage;
//			if (!(targetDamage || targetDamage === 0)) {
//				retVals[i] = targetDamage;
//				continue;
//			}
//			if (!target || !target.hp) {
//				retVals[i] = 0;
//				continue;
//			}
//			if (!target.isActive) {
//				retVals[i] = false;
//				continue;
//			}
//			if (targetDamage !== 0) targetDamage = this.clampIntRange(targetDamage, 1);

//			if (effect.id !== 'struggle-recoil') { // Struggle recoil is not affected by effects
//				if (effect.effectType === 'Weather' && !target.runStatusImmunity(effect.id)) {
//					this.debug('weather immunity');
//					retVals[i] = 0;
//					continue;
//				}
//				targetDamage = this.runEvent('Damage', target, source, effect, targetDamage, true);
//				if (!(targetDamage || targetDamage === 0)) {
//					this.debug('damage event failed');
//					retVals[i] = curDamage === true ? undefined : targetDamage;
//					continue;
//				}
//			}
//			if (targetDamage !== 0) targetDamage = this.clampIntRange(targetDamage, 1);

//			if (this.gen <= 1) {
//				if (this.dex.currentMod === 'gen1stadium' ||
//					!['recoil', 'drain', 'leechseed'].includes(effect.id) && effect.effectType !== 'Status') {
//					this.lastDamage = targetDamage;
//				}
//			}

//			retVals[i] = targetDamage = target.damage(targetDamage, source, effect);
//			if (targetDamage !== 0) target.hurtThisTurn = target.hp;
//			if (source && effect.effectType === 'Move') source.lastDamage = targetDamage;

//			const name = effect.fullname === 'tox' ? 'psn' : effect.fullname;
//			switch (effect.id) {
//			case 'partiallytrapped':
//				this.add('-damage', target, target.getHealth, '[from] ' + target.volatiles['partiallytrapped'].sourceEffect.fullname, '[partiallytrapped]');
//				break;
//			case 'powder':
//				this.add('-damage', target, target.getHealth, '[silent]');
//				break;
//			case 'confused':
//				this.add('-damage', target, target.getHealth, '[from] confusion');
//				break;
//			default:
//				if (effect.effectType === 'Move' || !name) {
//					this.add('-damage', target, target.getHealth);
//				} else if (source && (source !== target || effect.effectType === 'Ability')) {
//					this.add('-damage', target, target.getHealth, `[from] ${name}`, `[of] ${source}`);
//				} else {
//					this.add('-damage', target, target.getHealth, `[from] ${name}`);
//				}
//				break;
//			}

//			if (targetDamage && effect.effectType === 'Move') {
//				if (this.gen <= 1 && effect.recoil && source) {
//					if (this.dex.currentMod !== 'gen1stadium' || target.hp > 0) {
//						const amount = this.clampIntRange(Math.floor(targetDamage * effect.recoil[0] / effect.recoil[1]), 1);
//						this.damage(amount, source, target, 'recoil');
//					}
//				}
//				if (this.gen <= 4 && effect.drain && source) {
//					const amount = this.clampIntRange(Math.floor(targetDamage * effect.drain[0] / effect.drain[1]), 1);
//					// Draining can be countered in gen 1
//					if (this.gen <= 1) this.lastDamage = amount;
//					this.heal(amount, source, target, 'drain');
//				}
//				if (this.gen > 4 && effect.drain && source) {
//					const amount = Math.round(targetDamage * effect.drain[0] / effect.drain[1]);
//					this.heal(amount, source, target, 'drain');
//				}
//			}
//		}

//		if (instafaint) {
//			for (const [i, target] of targetArray.entries()) {
//				if (!retVals[i] || !target) continue;

//				if (target.hp <= 0) {
//					this.debug(`instafaint: ${this.faintQueue.map(entry => entry.target.name)}`);
//					this.faintMessages(true);
//					if (this.gen <= 2) {
//						target.faint();
//						if (this.gen <= 1) {
//							this.queue.clear();
//							// Fainting clears accumulated Bide damage
//							for (const pokemon of this.getAllActive()) {
//								if (pokemon.volatiles['bide']?.damage) {
//									pokemon.volatiles['bide'].damage = 0;
//									this.hint("Desync Clause Mod activated!");
//									this.hint("In Gen 1, Bide's accumulated damage is reset to 0 when a Pokemon faints.");
//								}
//							}
//						}
//					}
//				}
//			}
//		}

//		return retVals;
//	}