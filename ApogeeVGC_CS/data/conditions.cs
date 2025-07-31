using ApogeeVGC_CS.sim;

namespace ApogeeVGC_CS.data
{
    public static class Conditions
    {
        public static ConditionDataTable ConditionData { get; } = new()
        {
            [new IdEntry("brn")] = new Condition
            {
                Name = "brn",
                EffectType = EffectType.Status,
                OnStart = (battle, target, source, sourceEffect) =>
                {
                    if (sourceEffect?.Id.ToString() == "flameorb")
                    {
                        battle.Add("-status", target, "brn", "[from] item: Flame Orb");
                    }
                    else if (sourceEffect?.EffectType == EffectType.Ability)
                    {
                        battle.Add("-status", target, "brn", $"[from] ability: {sourceEffect.Name}", $"[of] {source}");
                    }
                    else
                    {
                        battle.Add("-status", target, "brn");
                    }

                    return true; // Indicate successful application
                },
                OnResidualOrder = 10,
                OnResidual = (battle, target, source, sourceEffect) =>
                {
                    battle.Damage(target.BaseMaxHp / 16);
                },
                Fullname = string.Empty,
                Exists = true,
                Num = 0,
                Gen = 0,
                NoCopy = false,
                AffectsFainted = false,
                SourceEffect = string.Empty,
            },
            [new IdEntry("par")] = new Condition
            {
                Name = "par",
                EffectType = EffectType.Status,
                OnStart = (battle, target, source, sourceEffect) =>
                {
                    if (sourceEffect?.EffectType == EffectType.Ability)
                    {
                        battle.Add("-status", target, "par", $"[from] ability: {sourceEffect.Name}", $"[of] {source}");
                    }
                    else
                    {
                        battle.Add("-status", target, "par");
                    }
                    return true;
                },
                OnModifySpePriority = -101,
                OnModifySpe = (battle, spe, pokemon) =>
                {
                    // Paralysis occurs after all other Speed modifiers
                    spe = battle.FinalModify(spe);
                    if (!pokemon.HasAbility("quickfeet"))
                    {
                        spe = (int)Math.Floor(spe * 0.5); // Halve Speed if not Quick Feet
                    }
                    return spe;
                },
                OnBeforeMovePriority = 1,
                OnBeforeMove = (battle, source, target, move) =>
                {
                    if (battle.RandomChance(1, 4)) // 25% chance to be fully paralyzed
                    {
                        battle.Add("cant", target, "par");
                        return false;
                    }
                    return true;
                },
                Fullname = string.Empty,
                Exists = true,
                Num = 0,
                Gen = 0,
                NoCopy = false,
                AffectsFainted = false,
                SourceEffect = string.Empty,
            },
            [new IdEntry("slp")] = new Condition
            {
                Name = "slp",
                EffectType = EffectType.Status,
                OnStart = (battle, target, source, sourceEffect) =>
                {
                    if (sourceEffect?.EffectType == EffectType.Ability)
                    {
                        battle.Add("-status", target, "slp", $"[from] ability: {sourceEffect.Name}", $"[of] {source}");
                    }
                    else if (sourceEffect?.EffectType == EffectType.Move)
                    {
                        battle.Add("-status", target, "slp", $"[from] move: {sourceEffect.Name}");
                    }
                    else
                    {
                        battle.Add("-status", target, "slp");
                    }

                    // 1-3 turns (random between 2 and 4 inclusive)
                    int sleepTurns = battle.Random(2, 5);
                    battle.EffectState.ExtraData["startTime"] = sleepTurns;
                    battle.EffectState.ExtraData["time"] = sleepTurns;

                    // Remove Nightmare if present
                    if (target.RemoveVolatile("nightmare"))
                    {
                        battle.Add("-end", target, "Nightmare", "[silent]");
                    }
                    return true;
                },
                OnBeforeMovePriority = 10,
                OnBeforeMove = (battle, pokemon, target, move) =>
                {
                    // Decrement sleep counter (Early Bird halves sleep duration)
                    int decrement = pokemon.HasAbility("earlybird") ? 2 : 1;
                    if (pokemon.StatusState.ExtraData.TryGetValue("time", out var timeObj) &&
                                                                                timeObj is int time)
                    {
                        time -= decrement;
                        pokemon.StatusState.ExtraData["time"] = time;
                        if (time <= 0)
                        {
                            pokemon.CureStatus();
                            return true; // Allow move after waking up
                        }
                    }

                    battle.Add("cant", pokemon, "slp");
                    if (move?.SleepUsable == true)
                    {
                        return true; // Allow move like Sleep Talk
                    }
                    return false; // Prevent other moves
                },
                Fullname = string.Empty,
                Exists = true,
                Num = 0,
                Gen = 0,
                NoCopy = false,
                AffectsFainted = false,
                SourceEffect = string.Empty,
            },
            [new IdEntry("frz")] = new Condition
            {
                Name = "frz",
                EffectType = EffectType.Status,
                OnStart = (battle, target, source, sourceEffect) =>
                {
                    if (sourceEffect?.EffectType == EffectType.Ability)
                    {
                        battle.Add("-status", target, "frz", $"[from] ability: {sourceEffect.Name}", $"[of] {source}");
                    }
                    else
                    {
                        battle.Add("-status", target, "frz");
                    }
                    // Shaymin-Sky reverts to Shaymin when frozen
                    if (target.Species.Name == "Shaymin-Sky" && target.BaseSpecies.BaseSpecies == "Shaymin")
                    {
                        target.FormeChange("Shaymin", battle.Effect, true);
                    }
                    return true;
                },
                OnBeforeMovePriority = 10,
                OnBeforeMove = (battle, pokemon, target, move) =>
                {
                    if (move.Flags.Defrost == true)
                    {
                        return true; // Allow move, will cure after
                    }
                    if (battle.RandomChance(1, 5)) // 20% chance to thaw
                    {
                        pokemon.CureStatus();
                        return true;
                    }
                    battle.Add("cant", pokemon, "frz");
                    return false;
                },
                OnModifyMove = (battle, move, pokemon, target) =>
                {
                    if (move.Flags.Defrost == true)
                    {
                        battle.Add("-curestatus", pokemon, "frz", $"[from] move: {move.Name}");
                        pokemon.CureStatus();
                    }
                },
                OnAfterMoveSecondary = (battle, target, source, move) =>
                {
                    if (move.ThawsTarget == true)
                    {
                        target.CureStatus();
                    }
                },
                OnDamagingHit = (battle, damage, target, source, move) =>
                {
                    if (move.Type == PokemonType.Fire && move.Category != MoveCategory.Status)
                    {
                        target.CureStatus();
                    }
                },
                Fullname = string.Empty,
                Exists = true,
                Num = 0,
                Gen = 0,
                NoCopy = false,
                AffectsFainted = false,
                SourceEffect = string.Empty,
            },
            [new IdEntry("psn")] = new Condition()
            {
                Name = "psn",
                EffectType = EffectType.Status,
                OnStart = (battle, target, source, sourceEffect) =>
                {
                    if (sourceEffect?.EffectType == EffectType.Ability)
                    {
                        battle.Add("-status", target, "psn", $"[from] ability: {sourceEffect.Name}", $"[of] {source}");
                    }
                    else
                    {
                        battle.Add("-status", target, "psn");
                    }
                    return true;
                },
                OnResidualOrder = 9,
                OnResidual = (battle, target, source, sourceEffect) =>
                {
                    battle.Damage(target.BaseMaxHp / 8);
                },
                Fullname = string.Empty,
                Exists = true,
                Num = 0,
                Gen = 0,
                NoCopy = false,
                AffectsFainted = false,
                SourceEffect = string.Empty,
            },
            [new IdEntry("tox")] = new Condition()
            {
                Name = "tox",
                EffectType = EffectType.Status,
                OnStart = (battle, target, source, sourceEffect) =>
                {
                    battle.EffectState.ExtraData["stage"] = 0;
                    if (sourceEffect?.Id.ToString() == "toxicorb")
                    {
                        battle.Add("-status", target, "tox", "[from] item: Toxic Orb");
                    }
                    else if (sourceEffect?.EffectType == EffectType.Ability)
                    {
                        battle.Add("-status", target, "tox", $"[from] ability: {sourceEffect.Name}", $"[of] {source}");
                    }
                    else
                    {
                        battle.Add("-status", target, "tox");
                    }
                    return true;
                },
                OnSwitchIn = (battle, pokemon) =>
                {
                    battle.EffectState.ExtraData["stage"] = 0;
                },
                OnResidualOrder = 9,
                OnResidual = (battle, target, source, sourceEffect) =>
                {
                    // Increment stage up to 15
                    int stage = 0;
                    if (battle.EffectState.ExtraData.TryGetValue("stage", out var stageObj) && stageObj is int s)
                        stage = s;
                    if (stage < 15) stage++;
                    battle.EffectState.ExtraData["stage"] = stage;

                    int damage = Math.Max(1, target.BaseMaxHp / 16) * stage;
                    battle.Damage(damage);
                },
                Fullname = string.Empty,
                Exists = true,
                Num = 0,
                Gen = 0,
                NoCopy = false,
                AffectsFainted = false,
                SourceEffect = string.Empty,
            },
            [new IdEntry("confusion")] = new Condition()
            {
                Name = "confusion",   
                OnStart = (battle, target, source, sourceEffect) =>
                {
                    if (sourceEffect?.Id.ToString() == "lockedmove")
                    {
                        battle.Add("-start", target, "confusion", "[fatigue]");
                    }
                    else if (sourceEffect?.EffectType == EffectType.Ability)
                    {
                        battle.Add("-start", target, "confusion", $"[from] ability: {sourceEffect.Name}", $"[of] {source}");
                    }
                    else
                    {
                        battle.Add("-start", target, "confusion");
                    }
                    int min = sourceEffect?.Id.ToString() == "axekick" ? 3 : 2;
                    int duration = battle.Random(min, 6); // 2-5 or 3-5 turns
                    battle.EffectState.ExtraData["time"] = duration;
                    return true;
                },
                OnEnd = (battle, target) =>
                {
                    battle.Add("-end", target, "confusion");
                },
                OnBeforeMovePriority = 3,
                OnBeforeMove = (battle, pokemon, target, move) =>
                {
                    // Decrement confusion counter
                    if (pokemon.Volatiles.TryGetValue("confusion", out var confusion) &&
                        confusion.ExtraData.TryGetValue("time", out var timeObj) &&
                        timeObj is int time)
                    {
                        time--;
                        confusion.ExtraData["time"] = time;
                        if (time <= 0)
                        {
                            pokemon.RemoveVolatile("confusion");
                            return true; // Confusion ended, allow move
                        }
                    }
                    battle.Add("-activate", pokemon, "confusion");
                    if (!battle.RandomChance(33, 100))
                    {
                        return true; // Did not hurt itself, allow move
                    }
                    // Hurt itself in confusion
                    int damage = battle.Actions.GetConfusionDamage(pokemon, 40);
                    if (damage == 0)
                        throw new Exception("Confusion damage not dealt");
                    var confusedMove = new ActiveMove
                    {
                        EffectType = EffectType.Move,
                        Type = PokemonType.Unknown,
                        Name = "confused",
                        Fullname = string.Empty,
                        Exists = true,
                        Num = 0,
                        Gen = 0,
                        NoCopy = false,
                        AffectsFainted = false,
                        SourceEffect = string.Empty,
                        Accuracy = true,
                        Pp = 1,
                        Category = MoveCategory.Status,
                        Priority = 0,
                        Target = MoveTarget.Self,
                        Flags = new MoveFlags(),
                    };
                    battle.Damage(damage, pokemon, pokemon, confusedMove);
                    return false; // Prevent move
                },
                EffectType = EffectType.Status,
                Fullname = string.Empty,
                Exists = true,
                Num = 0,
                Gen = 0,
                NoCopy = false,
                AffectsFainted = false,
                SourceEffect = string.Empty,
            },
            [new IdEntry("flinch")] = new Condition
            {
                Name = "flinch",
                EffectType = EffectType.Status,
                Duration = 1,
                OnBeforeMovePriority = 8,
                OnBeforeMove = (battle, pokemon, target, move) =>
                {
                    battle.Add("cant", pokemon, "flinch");
                    battle.RunEvent("Flinch", pokemon);
                    return false; // Prevent the move
                },
                Fullname = string.Empty,
                Exists = true,
                Num = 0,
                Gen = 0,
                NoCopy = false,
                AffectsFainted = false,
                SourceEffect = string.Empty,
            },
            [new IdEntry("choicelock")] = new Condition
            {
                Name = "choicelock",
                EffectType = EffectType.Status,
                NoCopy = true,
                OnStart = (battle, pokemon, source, sourceEffect) =>
                {
                    var activeMove = battle.ActiveMove ?? throw new Exception("Battle.ActiveMove is null");
                    if (activeMove.Id.IsEmpty || activeMove.HasBounced == true ||
                                                        activeMove.SourceEffect == "snatch")
                    {
                        return false;
                    }
                    battle.EffectState.ExtraData["move"] = activeMove.Id;
                    return true;
                },
                OnBeforeMove = (battle, pokemon, target, move) =>
                {
                    var item = pokemon.GetItem();
                    if (item == null || (!item.IsChoice ?? false))
                    {
                        pokemon.RemoveVolatile("choicelock");
                        return true;
                    }
                    Id? lockedMove = battle.EffectState.ExtraData.TryGetValue("move", out var moveObj) 
                        ? moveObj as Id
                        : Id.Empty;
                    if (!pokemon.IgnoringItem() && !pokemon.Volatiles.ContainsKey("dynamax") &&
                        move.Id != lockedMove && move.Id != new Id("struggle"))
                    {
                        battle.AddMove("move", pokemon, move.Name);
                        battle.AttrLastMove("[still]");
                        battle.Debug("Disabled by Choice item lock");
                        battle.Add("-fail", pokemon);
                        return false;
                    }
                    return true;
                },
                OnDisableMove = (battle, pokemon) =>
                {
                    var item = pokemon.GetItem();
                    Id? lockedMove = battle.EffectState.ExtraData.TryGetValue("move", out var moveObj) ? moveObj as Id : null;
                    if (item == null || (!item.IsChoice ?? false) || pokemon.HasMove(lockedMove ?? Id.Empty) == null)
                    {
                        pokemon.RemoveVolatile("choicelock");
                        return;
                    }
                    if (pokemon.IgnoringItem() || pokemon.Volatiles.ContainsKey("dynamax"))
                    {
                        return;
                    }
                    foreach (var moveSlot in pokemon.MoveSlots)
                    {
                        if (moveSlot.Id != lockedMove)
                        {
                            string? sourceEffect = battle.EffectState.ExtraData.TryGetValue("sourceEffect", out var srcEff)
                            ? srcEff as string
                            : throw new Exception("sourceEffect not found in ExtraData");

                            Condition condition = new()
                            { 
                                Name = "choicelock",
                                EffectType = EffectType.Status,
                                Fullname = string.Empty,
                                Exists = true,
                                Num = 0,
                                Gen = 0,
                                NoCopy = true,
                                AffectsFainted = false,
                                SourceEffect = sourceEffect!
                            };

                            pokemon.DisableMove(moveSlot.Id, false, condition);
                        }
                    }
                },
                Fullname = string.Empty,
                Exists = true,
                Num = 0,
                Gen = 0,
                AffectsFainted = false,
                SourceEffect = string.Empty,
            },
            [new IdEntry("stall")] = new Condition
            {
                Name = "stall",
                EffectType = EffectType.Status,
                Duration = 2,
                CounterMax = 729,
                OnStart = (battle, pokemon, source, sourceEffect) =>
                {
                    battle.EffectState.ExtraData["counter"] = 3;
                    return true;
                },
                OnStallMove = (battle, pokemon) =>
                {
                    int counter = 1;
                    if (battle.EffectState.ExtraData.TryGetValue("counter", out var counterObj) && counterObj is int c)
                        counter = c;
                    battle.Debug($"Success chance: {Math.Round(100.0 / counter)}%");
                    bool success = battle.RandomChance(1, counter);
                    if (!success)
                    {
                        pokemon.RemoveVolatile("stall");
                    }
                    return success;
                },
                OnRestart = (battle, pokemon, source, sourceEffect) =>
                {
                    int counterMax = 729;
                    if (battle.EffectState.ExtraData.TryGetValue("counter", out var counterObj) && counterObj is int counter)
                    {
                        if (counter < counterMax)
                        {
                            counter *= 3;
                            battle.EffectState.ExtraData["counter"] = counter;
                        }
                    }
                    battle.EffectState.Duration = 2;
                    return true;
                },
                Fullname = string.Empty,
                Exists = true,
                Num = 0,
                Gen = 0,
                NoCopy = false,
                AffectsFainted = false,
                SourceEffect = string.Empty,
            },
        };
    }
}
