using ApogeeVGC_CS.sim;

namespace ApogeeVGC_CS.data
{
    public static class Moves
    {
        public static MoveDataTable MoveData { get; } = new()
        {
            [new IdEntry("glaciallance")] = new MoveData
            {
                Num = 824,
                Accuracy = 100,
                BasePower = 120,
                Category = MoveCategory.Physical,
                Name = "Glacial Lance",
                Pp = 5,
                Priority = 0,
                Flags = new MoveFlags() { Protect = true, Mirror = true },
                Secondary = null,
                Target = MoveTarget.AllAdjacentFoes,
                Type = PokemonType.Ice,
                EffectType = EffectType.Move,
            },
            [new IdEntry("leechseed")] = new MoveData
            {
                Num = 73,
                Accuracy = 90,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Leech Seed",
                Pp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    Metronome = true
                },
                VolatileStatus = new Id("leechseed"),
                Condition = new Condition
                {
                    Name = "Leech Seed",
                    Fullname = string.Empty,
                    EffectType = EffectType.Condition,
                    Exists = true,
                    Num = 0,
                    Gen = 0,
                    NoCopy = false,
                    AffectsFainted = false,
                    SourceEffect = "leechseed",
                    OnStart = (battle, target, unusedSource, unusedEffect) =>
                    {
                        battle.Add("-start", target, "move: Leech Seed");
                        return true;
                    },
                    OnResidualOrder = 8,
                    OnResidual = static (battle2, target2, unusedSource2, unusedEffect2) =>
                    {
                        var sourceSlot = target2.Volatiles["leechseed"].ExtraData["sourceSlot"];
                        if (sourceSlot == null)
                        {
                            battle2.Debug("Leech Seed source slot is null");
                            return;
                        }
                        if (sourceSlot is not PokemonSlot pokemonSlot)
                        {
                            battle2.Debug("Leech Seed source slot is not a PokemonSlot");
                            return;
                        }

                        Pokemon? foundTarget = battle2.GetAtSlot(pokemonSlot);
                        if (foundTarget == null || foundTarget.Fainted || foundTarget.Hp <= 0)
                        {
                            battle2.Debug("Nothing to leech into");
                            return;
                        }
                        IntFalseUnion? damage = battle2.Damage(target2.BaseMaxHp / 8, target2, foundTarget);
                        if (damage == null)
                        {
                            battle2.Debug("Leech Seed damage is null");
                            return;
                        }
                        switch (damage)
                        {
                            case IntIntFalseUnion(int intDamage):
                                if (intDamage > 0)
                                {
                                    battle2.Heal(intDamage, foundTarget, target2);
                                }
                                break;
                            case FalseIntFalseUnion falseDamage:
                                damage = 0;
                                break;
                            default:
                                throw new Exception("Unexpected damage type in Leech Seed residual");
                        }
                        
                    }
                },
                OnTryImmunity = (unusedBattle, target3, unusedSource, unusedActiveMove) =>
                                                                !target3.HasType(PokemonType.Grass),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = PokemonType.Grass,
                ZMove = new ZMoveData { Effect = "clearnegativeboost" },
                ContestType = ContestType.Clever,
                EffectType = EffectType.Move,
            },
            [new IdEntry("trickroom")] = new MoveData
            {
                Num = 433,
                Accuracy = true,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Trick Room",
                Pp = 5,
                Priority = -7,
                Flags = new MoveFlags
                {
                    Mirror = true,
                    Metronome = true
                },
                PseudoWeather = "trickroom",
                Condition = new Condition
                {
                    Name = "Trick Room",
                    Fullname = "move: Trick Room",
                    EffectType = EffectType.Condition,
                    Exists = true,
                    Num = 433,
                    Gen = 4,
                    NoCopy = false,
                    AffectsFainted = false,
                    SourceEffect = "trickroom",
                    Duration = 5,
                    DurationCallback = (battle, target, source, effect) =>
                    {
                        if (source != null && source.HasAbility("persistent"))
                        {
                            battle.Add("-activate", source, "ability: Persistent", "[move] Trick Room");
                            return 7;
                        }
                        return 5;
                    },
                    OnFieldStart = (battle, targetField, sourcePokemon, sourceEffect) =>
                    {
                        if (sourcePokemon != null && sourcePokemon.HasAbility("persistent"))
                        {
                            battle.Add("-fieldstart", "move: Trick Room", $"[of] {sourcePokemon}", "[persistent]");
                        }
                        else
                        {
                            battle.Add("-fieldstart", "move: Trick Room", $"[of] {sourcePokemon}");
                        }
                    },
                    OnFieldRestart = (battle, targetField, sourcePokemon, sourceEffect) =>
                    {
                        battle.Field.RemovePseudoWeather("trickroom");
                    },
                    OnFieldResidualOrder = 27,
                    OnFieldResidualSubOrder = 1,
                    OnFieldEnd = (battle, targetField) =>
                    {
                        battle.Add("-fieldend", "move: Trick Room");
                    }
                },
                Secondary = null,
                Target = MoveTarget.All,
                Type = PokemonType.Psychic,
                ZMove = new ZMoveData { Boost = new() { [BoostId.Accuracy] = 1 } },
                ContestType = ContestType.Clever,
                EffectType = EffectType.Move,
            },
            [new IdEntry("protect")] = new MoveData
            {
                Num = 182,
                Accuracy = true,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Protect",
                Pp = 10,
                Priority = 4,
                Flags = new MoveFlags
                {
                    NoAssist = true,
                    FailCopycat = true
                },
                StallingMove = true,
                VolatileStatus = new Id("protect"),
                OnPrepareHit = (battle, targetPokemon, sourcePokemon, move) =>
                {
                    // Only allow if the user will act and the stalling event passes
                    object runEventReturn = battle.RunEvent("StallMove", targetPokemon);
                    bool runEventResult = runEventReturn is bool result && result;
                    return (battle.Queue.WillAct() != null) && runEventResult;
                },
                OnHit = (battle, targetPokemon, sourcePokemon, move) =>
                {
                    // Add the "stall" volatile to the user
                    sourcePokemon.AddVolatile("stall");
                    return true;
                },
                Condition = new Condition
                {
                    Name = "Protect",
                    Duration = 1,
                    OnStart = (battle, target, unusedSource, unusedEffect) =>
                    {
                        battle.Add("-singleturn", target, "Protect");
                        return true;
                    },
                    OnTryHitPriority = 3,
                    OnTryHit = (battle, target, source, move) =>
                    {
                        // Block moves without the "protect" flag, except for certain moves
                        if (!move.Flags.Protect ?? false)
                        {
                            if (move.Id == "gmaxoneblow" || move.Id == "gmaxrapidflow")
                                return false;
                            if (move.IsZ != null || move.IsMax != null)
                                target.GetMoveHitData(move).ZBrokeProtect = true;
                            return false;
                        }
                        if (move.SmartTarget ?? false)
                        {
                            move.SmartTarget = false;
                        }
                        else
                        {
                            battle.Add("-activate", target, "move: Protect");
                        }
                        var lockedMove = source.GetVolatile("lockedmove");
                        if (lockedMove != null && lockedMove.Duration == 2)
                        {
                            source.RemoveVolatile("lockedmove");
                        }
                        return Battle.NotFail;
                    },
                    Fullname = string.Empty,
                    EffectType = EffectType.Condition,
                    Exists = true,
                    Num = 0,
                    Gen = 0,
                    NoCopy = false,
                    AffectsFainted = false,
                    SourceEffect = "protect",
                },
                Secondary = null,
                Target = MoveTarget.Self,
                Type = PokemonType.Normal,
                ZMove = new ZMoveData { Effect = "clearnegativeboost" },
                ContestType = ContestType.Cute,
                EffectType = EffectType.Move,
            },
            [new IdEntry("voltswitch")] = new MoveData
            {
                Num = 521,
                Accuracy = 100,
                BasePower = 70,
                Category = MoveCategory.Special,
                Name = "Volt Switch",
                Pp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true
                },
                SelfSwitch = true,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = PokemonType.Electric,
                ContestType = ContestType.Cool,
                EffectType = EffectType.Move,
            },
            [new IdEntry("dazzlinggleam")] = new MoveData
            {
                Num = 605,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Special,
                Name = "Dazzling Gleam",
                Pp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true
                },
                Secondary = null,
                Target = MoveTarget.AllAdjacentFoes,
                Type = PokemonType.Fairy,
                ContestType = ContestType.Beauty,
                EffectType = EffectType.Move,
            },
            [new IdEntry("electrodrift")] = new MoveData
            {
                Num = 879,
                Accuracy = 100,
                BasePower = 100,
                Category = MoveCategory.Special,
                Name = "Electro Drift",
                Pp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true
                },
                OnBasePower = (battle, basePower, source, target, move) =>
                {
                    // If the move is super effective, multiply base power by 1.33
                    if (target.RunEffectiveness(move) > 0)
                    {

                        battle.ChainModify([5461, 4096]);
                        return 5461 * basePower / 4096;
                    }
                    return basePower;
                },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = PokemonType.Electric,
                ContestType = ContestType.Cool,
                EffectType = EffectType.Move,
            },
            [new IdEntry("dracometeor")] = new MoveData
            {
                Num = 434,
                Accuracy = 90,
                BasePower = 130,
                Category = MoveCategory.Special,
                Name = "Draco Meteor",
                Pp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true
                },
                Self = new SecondaryEffect
                {
                    Boosts = new SparseBoostsTable
                    {
                        [BoostId.Spa] = -2
                    }
                },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = PokemonType.Dragon,
                ContestType = ContestType.Beauty,
                EffectType = EffectType.Move,
            },
            [new IdEntry("facade")] = new MoveData
            {
                Num = 263,
                Accuracy = 100,
                BasePower = 70,
                Category = MoveCategory.Physical,
                Name = "Facade",
                Pp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true
                },
                OnBasePower = (battle, basePower, pokemon, target, move) =>
                {
                    // If the user is statused and not asleep, double the base power
                    if (pokemon.Status != "slp")
                    {
                        return basePower * 2;
                    }
                    return basePower;
                },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = PokemonType.Normal,
                ContestType = ContestType.Cute,
                EffectType = EffectType.Move,
            },
            [new IdEntry("crunch")] = new MoveData
            {
                Num = 242,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Physical,
                Name = "Crunch",
                Pp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                    Bite = true
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 20,
                    Boosts = new SparseBoostsTable
                    {
                        [BoostId.Def] = -1
                    }
                },
                Target = MoveTarget.Normal,
                Type = PokemonType.Dark,
                ContestType = ContestType.Tough,
                EffectType = EffectType.Move,
            },
            [new IdEntry("headlongrush")] = new MoveData
            {
                Num = 838,
                Accuracy = 100,
                BasePower = 120,
                Category = MoveCategory.Physical,
                Name = "Headlong Rush",
                Pp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Punch = true,
                    Metronome = true
                },
                Self = new SecondaryEffect
                {
                    Boosts = new SparseBoostsTable
                    {
                        [BoostId.Def] = -1,
                        [BoostId.Spd] = -1
                    }
                },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = PokemonType.Ground,
                EffectType = EffectType.Move,
            },
            [new IdEntry("strugglebug")] = new MoveData
            {
                Num = 522,
                Accuracy = 100,
                BasePower = 50,
                Category = MoveCategory.Special,
                Name = "Struggle Bug",
                Pp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    Boosts = new SparseBoostsTable
                    {
                        [BoostId.Spa] = -1
                    }
                },
                Target = MoveTarget.AllAdjacentFoes,
                Type = PokemonType.Bug,
                ContestType = ContestType.Cute,
                EffectType = EffectType.Move,
            },
            [new IdEntry("overheat")] = new MoveData
            {
                Num = 315,
                Accuracy = 90,
                BasePower = 130,
                Category = MoveCategory.Special,
                Name = "Overheat",
                Pp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true
                },
                Self = new SecondaryEffect
                {
                    Boosts = new SparseBoostsTable
                    {
                        [BoostId.Spa] = -2
                    }
                },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = PokemonType.Fire,
                ContestType = ContestType.Beauty,
                EffectType = EffectType.Move,
            },
            [new IdEntry("ragepowder")] = new MoveData
            {
                Num = 476,
                Accuracy = true,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Rage Powder",
                Pp = 20,
                Priority = 2,
                Flags = new MoveFlags
                {
                    NoAssist = true,
                    FailCopycat = true,
                    Powder = true
                },
                VolatileStatus = new Id("ragepowder"),
                OnTry = (battle, source, target, move) =>
                {
                    // Only works if there is more than one active Pokémon per side
                    return battle.ActivePerHalf > 1;
                },
                Condition = new Condition
                {
                    Name = "Rage Powder",
                    Duration = 1,
                    OnStart = (battle, pokemon, unusedSource, unusedEffect) =>
                    {
                        battle.Add("-singleturn", pokemon, "move: Rage Powder");
                        return true;
                    },
                    OnFoeRedirectTargetPriority = 1,
                    OnFoeRedirectTarget = (battle, target, source, source2, move) =>
                    {
                        var ragePowderUser = battle.EffectState.ExtraData["target"];
                        if (ragePowderUser is not Pokemon pokemonUser)
                        {
                            battle.Debug("Rage Powder user is not a Pokemon");
                            return null;
                        }
                        if (pokemonUser.IsSkyDropped())
                            return null;

                        if (source.RunStatusImmunity("powder") &&
                            battle.ValidTarget(pokemonUser, source, move.Target))
                        {
                            if (move.SmartTarget ?? false)
                                move.SmartTarget = false;
                            battle.Debug("Rage Powder redirected target of move");
                            return pokemonUser;
                        }
                        return null;
                    },
                    Fullname = string.Empty,
                    EffectType = EffectType.Condition,
                    Exists = true,
                    Num = 0,
                    Gen = 0,
                    NoCopy = false,
                    AffectsFainted = false,
                    SourceEffect = "ragepowder",
                },
                Secondary = null,
                Target = MoveTarget.Self,
                Type = PokemonType.Bug,
                ZMove = new ZMoveData { Effect = "clearnegativeboost" },
                ContestType = ContestType.Clever,
                EffectType = EffectType.Move,
            },
            [new IdEntry("tailwind")] = new MoveData
            {
                Num = 366,
                Accuracy = true,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Tailwind",
                Pp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Metronome = true,
                    Wind = true
                },
                SideCondition = "tailwind",
                Condition = new Condition
                {
                    Name = "Tailwind",
                    Duration = 4,
                    DurationCallback = (battle, target, source, effect) =>
                    {
                        if (source != null && source.HasAbility("persistent"))
                        {
                            battle.Add("-activate", source, "ability: Persistent", "[move] Tailwind");
                            return 6;
                        }
                        return 4;
                    },
                    OnSideStart = (battle, side, source, effect) =>
                    {
                        if (source != null && source.HasAbility("persistent"))
                        {
                            battle.Add("-sidestart", side, "move: Tailwind", "[persistent]");
                        }
                        else
                        {
                            battle.Add("-sidestart", side, "move: Tailwind");
                        }
                    },
                    OnModifySpe = (battle, spe, pokemon) =>
                    {
                        // Doubles the Speed stat
                        battle.ChainModify(2);
                        return spe * 2;
                    },
                    OnSideResidualOrder = 26,
                    OnSideResidualSubOrder = 5,
                    OnSideEnd = (battle, side) =>
                    {
                        battle.Add("-sideend", side, "move: Tailwind");
                    },
                    Fullname = string.Empty,
                    EffectType = EffectType.Condition,
                    Exists = true,
                    Num = 0,
                    Gen = 0,
                    NoCopy = false,
                    AffectsFainted = false,
                    SourceEffect = "tailwind",
                },
                Secondary = null,
                Target = MoveTarget.AllySide,
                Type = PokemonType.Flying,
                ZMove = new ZMoveData { Effect = "crit2" },
                ContestType = ContestType.Cool,
                EffectType = EffectType.Move,
            },
            [new IdEntry("spiritbreak")] = new MoveData
            {
                Num = 789,
                Accuracy = 100,
                BasePower = 75,
                Category = MoveCategory.Physical,
                Name = "Spirit Break",
                Pp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    Boosts = new SparseBoostsTable
                    {
                        [BoostId.Spa] = -1
                    }
                },
                Target = MoveTarget.Normal,
                Type = PokemonType.Fairy,
                EffectType = EffectType.Move,
            },
            [new IdEntry("thunderwave")] = new MoveData
            {
                Num = 86,
                Accuracy = 90,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Thunder Wave",
                Pp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    Metronome = true
                },
                Status = new Id("par"),
                IgnoreImmunity = false,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = PokemonType.Electric,
                ZMove = new ZMoveData { Boost = new() { [BoostId.Spd] = 1 } },
                ContestType = ContestType.Cool,
                EffectType = EffectType.Move,
            },
            [new IdEntry("reflect")] = new MoveData
            {
                Num = 115,
                Accuracy = true,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Reflect",
                Pp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Metronome = true
                },
                SideCondition = "reflect",
                Condition = new Condition
                {
                    Name = "Reflect",
                    Duration = 5,
                    DurationCallback = (battle, target, source, effect) =>
                    {
                        if (source != null && source.HasItem("lightclay"))
                        {
                            return 8;
                        }
                        return 5;
                    },
                    OnAnyModifyDamage = (battle, damage, source, target, move) =>
                    {
                        var sideTarget = battle.EffectState.ExtraData["target"];
                        if (sideTarget is not Side side)
                        {
                            battle.Debug("Reflect target is not a Side");
                            return damage;
                        }

                        if (target != source && side.HasAlly(target) &&
                            move.Category == MoveCategory.Physical)
                        {
                            if (!(target.GetMoveHitData(move)?.Crit ?? false) && !(move.Infiltrates ?? false))
                            {
                                battle.Debug("Reflect weaken");
                                if (battle.ActivePerHalf > 1)
                                {
                                    battle.ChainModify([2732, 4096]);
                                    return damage * 2732 / 4096;
                                }
                                battle.ChainModify(1, 2);
                                return damage / 2;
                            }
                        }
                        return damage;
                    },
                    OnSideStart = (battle, side, source, effect) =>
                    {
                        battle.Add("-sidestart", side, "Reflect");
                    },
                    OnSideResidualOrder = 26,
                    OnSideResidualSubOrder = 1,
                    OnSideEnd = (battle, side) =>
                    {
                        battle.Add("-sideend", side, "Reflect");
                    },
                    EffectType = EffectType.Condition,
                    Exists = true,
                    SourceEffect = "reflect",
                    Fullname = string.Empty,
                    Num = 0,
                    Gen = 0,
                    NoCopy = false,
                    AffectsFainted = false,
                },
                Secondary = null,
                Target = MoveTarget.AllySide,
                Type = PokemonType.Psychic,
                ZMove = new ZMoveData { Boost = new() { [BoostId.Def] = 1 } },
                ContestType = ContestType.Clever,
                EffectType = EffectType.Move,
            },
            [new IdEntry("lightscreen")] = new MoveData
            {
                Num = 113,
                Accuracy = true,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Light Screen",
                Pp = 30,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Metronome = true
                },
                SideCondition = "lightscreen",
                Condition = new Condition
                {
                    Name = "Light Screen",
                    Duration = 5,
                    DurationCallback = (battle, target, source, effect) =>
                    {
                        if (source != null && source.HasItem("lightclay"))
                        {
                            return 8;
                        }
                        return 5;
                    },
                    OnAnyModifyDamage = (battle, damage, source, target, move) =>
                    {
                        var sideTarget = battle.EffectState.ExtraData["target"];
                        if (sideTarget is not Side side)
                        {
                            battle.Debug("Light Screen target is not a Side");
                            return damage;
                        }

                        if (target != source && side.HasAlly(target) &&
                            move.Category == MoveCategory.Special)
                        {
                            if (!(target.GetMoveHitData(move)?.Crit ?? false) && !(move.Infiltrates ?? false))
                            {
                                battle.Debug("Light Screen weaken");
                                if (battle.ActivePerHalf > 1)
                                {
                                    battle.ChainModify([2732, 4096]);
                                    return damage * 2732 / 4096;
                                }
                                battle.ChainModify(1, 2);
                                return damage / 2;
                            }
                        }
                        return damage;
                    },
                    OnSideStart = (battle, side, source, effect) =>
                    {
                        battle.Add("-sidestart", side, "move: Light Screen");
                    },
                    OnSideResidualOrder = 26,
                    OnSideResidualSubOrder = 2,
                    OnSideEnd = (battle, side) =>
                    {
                        battle.Add("-sideend", side, "move: Light Screen");
                    },
                    EffectType = EffectType.Condition,
                    Exists = true,
                    SourceEffect = "lightscreen",
                    Fullname = string.Empty,
                    Num = 0,
                    Gen = 0,
                    NoCopy = false,
                    AffectsFainted = false,
                },
                Secondary = null,
                Target = MoveTarget.AllySide,
                Type = PokemonType.Psychic,
                ZMove = new ZMoveData { Boost = new() { [BoostId.Spd] = 1 } },
                ContestType = ContestType.Beauty,
                EffectType = EffectType.Move,
            },
            [new IdEntry("fakeout")] = new MoveData
            {
                Num = 252,
                Accuracy = 100,
                BasePower = 40,
                Category = MoveCategory.Physical,
                Name = "Fake Out",
                Pp = 10,
                Priority = 3,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true
                },
                OnTry = (battle, source, target, move) =>
                {
                    if (source.ActiveMoveActions > 1)
                    {
                        battle.Hint("Fake Out only works on your first turn out.");
                        return false;
                    }
                    return true;
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    VolatileStatus = new Id("flinch")
                },
                Target = MoveTarget.Normal,
                Type = PokemonType.Normal,
                ContestType = ContestType.Cute,
                EffectType = EffectType.Move,
            },
            [new IdEntry("heavyslam")] = new MoveData
            {
                Num = 484,
                Accuracy = 100,
                BasePower = 0, // Calculated dynamically
                Category = MoveCategory.Physical,
                Name = "Heavy Slam",
                Pp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    NonSky = true,
                    Metronome = true
                },
                OnBasePower = (battle, basePower, pokemon, target, move) =>
                {
                    double targetWeight = target.GetWeight();
                    double pokemonWeight = pokemon.GetWeight();
                    int bp;
                    if (pokemonWeight >= targetWeight * 5)
                        bp = 120;
                    else if (pokemonWeight >= targetWeight * 4)
                        bp = 100;
                    else if (pokemonWeight >= targetWeight * 3)
                        bp = 80;
                    else if (pokemonWeight >= targetWeight * 2)
                        bp = 60;
                    else
                        bp = 40;
                    battle.Debug($"BP: {bp}");
                    return bp;
                },
                OnTryHit = (battle, target, pokemon, move) =>
                {
                    if (target.Volatiles.ContainsKey("dynamax"))
                    {
                        battle.Add("-fail", pokemon, "Dynamax");
                        battle.AttrLastMove("[still]");
                        return false;
                    }
                    return true;
                },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = PokemonType.Steel,
                ZMove = new ZMoveData { BasePower = 160 },
                MaxMove = new MaxMoveData { BasePower = 130 },
                ContestType = ContestType.Tough,
                EffectType = EffectType.Move,
            },
            [new IdEntry("lowkick")] = new MoveData
            {
                Num = 67,
                Accuracy = 100,
                BasePower = 0, // Calculated dynamically
                Category = MoveCategory.Physical,
                Name = "Low Kick",
                Pp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true
                },
                OnBasePower = (battle, basePower, pokemon, target, move) =>
                {
                    double targetWeight = target.GetWeight();
                    int bp;
                    if (targetWeight >= 2000)
                        bp = 120;
                    else if (targetWeight >= 1000)
                        bp = 100;
                    else if (targetWeight >= 500)
                        bp = 80;
                    else if (targetWeight >= 250)
                        bp = 60;
                    else if (targetWeight >= 100)
                        bp = 40;
                    else
                        bp = 20;
                    battle.Debug($"BP: {bp}");
                    return bp;
                },
                OnTryHit = (battle, target, pokemon, move) =>
                {
                    if (target.Volatiles.ContainsKey("dynamax"))
                    {
                        battle.Add("-fail", pokemon, "Dynamax");
                        battle.AttrLastMove("[still]");
                        return false;
                    }
                    return true;
                },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = PokemonType.Fighting,
                ZMove = new ZMoveData { BasePower = 160 },
                ContestType = ContestType.Tough,
                EffectType = EffectType.Move,
            },
            [new IdEntry("wildcharge")] = new MoveData
            {
                Num = 528,
                Accuracy = 100,
                BasePower = 90,
                Category = MoveCategory.Physical,
                Name = "Wild Charge",
                Pp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true
                },
                Recoil = (1, 4), // User takes 1/4 of damage dealt as recoil
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = PokemonType.Electric,
                ContestType = ContestType.Tough,
                EffectType = EffectType.Move,
            },
        };
    }
}