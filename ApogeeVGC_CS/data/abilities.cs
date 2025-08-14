using ApogeeVGC_CS.sim;

namespace ApogeeVGC_CS.data
{
    /**
    Ratings and how they work:

    -1: Detrimental
          An ability that severely harms the user.
        ex. Defeatist, Slow Start

     0: Useless
          An ability with no overall benefit in a singles battle.
        ex. Color Change, Plus

     1: Ineffective
          An ability that has minimal effect or is only useful in niche situations.
        ex. Light Metal, Suction Cups

     2: Useful
          An ability that can be generally useful.
        ex. Flame Body, Overcoat

     3: Effective
          An ability with a strong effect on the user or foe.
        ex. Chlorophyll, Sturdy

     4: Very useful
          One of the more popular abilities. It requires minimal support to be effective.
        ex. Adaptability, Magic Bounce

     5: Essential
          The sort of ability that defines metagames.
        ex. Imposter, Shadow Tag
    */
    public static class Abilities
    {
        public static readonly AbilityData DefaultAbilityData = new()
        {
            Name = string.Empty,
            Fullname = string.Empty,
            EffectType = EffectType.Ability,
            Exists = true,
            Num = 0,
            Gen = 0,
            NoCopy = false,
            AffectsFainted = false,
            SourceEffect = string.Empty,
            Rating = 0,
            SuppressWeather = false,
            Flags = new AbilityFlags(),
        };

        public static AbilityDataTable AbilitiesData { get; } = new()
        {
            [new IdEntry("asoneglastrier")] = new AbilityData
            {
                OnSwitchInPriority = 1,

                OnStart = (battle, pokemon) =>
                {
                    if (pokemon.AbilityState.ExtraData?.ContainsKey("unnerved") == true &&
                        (bool)pokemon.AbilityState.ExtraData["unnerved"]) return;

                    battle.Add("-ability", pokemon, "As One");
                    battle.Add("-ability", pokemon, "Unnerve");

                    pokemon.AbilityState.ExtraData ??= [];
                    pokemon.AbilityState.ExtraData["unnerved"] = true;
                },

                OnEnd = (battle, pokemon) =>
                {
                    if (pokemon.AbilityState.ExtraData?.ContainsKey("unnerved") == true)
                    {
                        pokemon.AbilityState.ExtraData["unnerved"] = false;
                    }
                },

                OnFoeTryEatItem = (battle, item, pokemon) =>
                {
                    bool unnerved = pokemon.AbilityState.ExtraData?.ContainsKey("unnerved") == true &&
                                    (bool)pokemon.AbilityState.ExtraData["unnerved"];
                    return !unnerved;
                },

                OnSourceAfterFaint = (battle, length, target, source, effect) =>
                {
                    if (effect?.EffectType != EffectType.Move) return;
                    SparseBoostsTable boosts = new SparseBoostsTable
                    {
                        [BoostId.Atk] = length
                    };

                    battle.Boost(boosts, source, source,
                        battle.Dex.Abilities.Get("chillingneigh"));
                },
                Name = "As One (Glastrier)",
                Fullname = string.Empty,
                EffectType = EffectType.Condition, //EffectType.Ability,
                Exists = true,
                Num = 266,
                Gen = 0,
                NoCopy = false,
                AffectsFainted = false,
                SourceEffect = string.Empty,
                Rating = 3.5,
                SuppressWeather = false,
                Flags = new AbilityFlags()
                {
                    FailRolePlay = true,
                    NoReceiver = true,
                    NoEntrain = true,
                    NoTrace = true,
                    FailSkillSwap = true,
                    CantSuppress = true
                }
            },
            [new IdEntry("hadronengine")] = new AbilityData
            {
                OnStart = (battle, pokemon) =>
                {
                    if (!battle.Field.SetTerrain("electricterrain") && battle.Field.IsTerrain("electricterrain"))
                    {
                        battle.Add("-activate",
                            pokemon,
                            "ability: Hadron Engine");
                    }
                },
                OnModifySpAPriority = 5,
                OnModifySpA = (battle, spa, attacker, defender, move) =>
                {
                    if (!battle.Field.IsTerrain("electricterrain"))
                        return spa; // Return original value if terrain not active

                    battle.Debug("Hadron Engine boost");
                    battle.ChainModify([
                        5461,
                        4096
                    ]);
                    return 0; // Return 0 to indicate the SPA boost has been applied
                },
                Flags = new AbilityFlags(),
                Name = "Hadron Engine",
                Rating = 4.5,
                Num = 289,
                Fullname = string.Empty,
                EffectType = EffectType.Condition, //EffectType.Ability,
                Exists = true,
                Gen = 0,
                NoCopy = false,
                AffectsFainted = false,
                SourceEffect = string.Empty,
                SuppressWeather = false,
            },
            [new IdEntry("guts")] = new AbilityData
            {
                OnModifyAtkPriority = 5,
                OnModifyAtk = (battle, atk, pokemon, defender, move) =>
                {
                    if (!pokemon.Status.IsEmpty)
                    {
                        battle.ChainModify(3,
                            2); // 1.5x boost (3/2 = 1.5)
                    }

                    return atk;
                },
                Flags = new AbilityFlags(),
                Name = "Guts",
                Rating = 3.5,
                Num = 62,
                Fullname = string.Empty,
                EffectType = EffectType.Condition,
                Exists = true,
                Gen = 0,
                NoCopy = false,
                AffectsFainted = false,
                SourceEffect = string.Empty,
                SuppressWeather = false,
            },
            [new IdEntry("flamebody")] = new AbilityData
            {
                OnDamagingHit = (battle, damage, target, source, move) =>
                {
                    if (!battle.CheckMoveMakesContact(move,
                            source,
                            target))
                        return;
                    if (battle.RandomChance(3,
                            10)) // 30% chance (3 out of 10)
                    {
                        source.TrySetStatus("brn",
                            target);
                    }
                },
                Flags = new AbilityFlags(),
                Name = "Flame Body",
                Rating = 2,
                Num = 49,
                Fullname = string.Empty,
                EffectType = EffectType.Condition,
                Exists = true,
                Gen = 0,
                NoCopy = false,
                AffectsFainted = false,
                SourceEffect = string.Empty,
                SuppressWeather = false,
            },
            [new IdEntry("prankster")] = new AbilityData
            {
                OnModifyPriority = (battle, priority, pokemon, target, move) =>
                {
                    if (move?.Category != MoveCategory.Status)
                        return priority;
                    move.PranksterBoosted = true;
                    return priority + 1;
                },
                Flags = new AbilityFlags(),
                Name = "Prankster",
                Rating = 4,
                Num = 158,
                Fullname = string.Empty,
                EffectType = EffectType.Condition,
                Exists = true,
                Gen = 0,
                NoCopy = false,
                AffectsFainted = false,
                SourceEffect = string.Empty,
                SuppressWeather = false,
            },
            [new IdEntry("quarkdrive")] = new AbilityData
            {
                OnSwitchInPriority = -2,
                OnStart = (battle, pokemon) =>
                {
                    battle.SingleEvent("TerrainChange", battle.Effect, battle.EffectState, pokemon);
                },
                OnTerrainChange = (battle, target, source, effect) =>
                {
                    if (battle.Field.IsTerrain("electricterrain"))
                    {
                        target.AddVolatile("quarkdrive");
                    }
                    else
                    {
                        IEffect? quarkDriveVolatile = target.GetVolatile("quarkdrive");
                        if (quarkDriveVolatile?.ExtraData.ContainsKey("fromBooster") != true ||
                            !(bool)quarkDriveVolatile.ExtraData["fromBooster"])
                        {
                            target.RemoveVolatile("quarkdrive");
                        }
                    }
                },
                OnEnd = (battle, pokemon) =>
                {
                    pokemon.RemoveVolatile("quarkdrive");
                    battle.Add("-end", pokemon, "Quark Drive", "[silent]");
                },
                Condition = new Condition
                {
                    Name = "Quark Drive",
                    Fullname = "ability: Quark Drive",
                    EffectType = EffectType.Condition,
                    Exists = true,
                    Num = 282,
                    Gen = 0,
                    AffectsFainted = false,
                    SourceEffect = string.Empty,
                    NoCopy = true,
                    OnStart = (battle, pokemon, source, effect) =>
                    {
                        IEffect? effectState = pokemon.GetVolatile("quarkdrive");
                        if (effectState == null) return null;

                        if (effect?.Name == "Booster Energy")
                        {
                            effectState.ExtraData ??= new Dictionary<string, object>();
                            effectState.ExtraData["fromBooster"] = true;
                            battle.Add("-activate", pokemon, "ability: Quark Drive", "[fromitem]");
                        }
                        else
                        {
                            battle.Add("-activate", pokemon, "ability: Quark Drive");
                        }

                        StatIdExceptHp bestStat = pokemon.GetBestStat(false, true);
                        effectState.ExtraData ??= new Dictionary<string, object>();
                        effectState.ExtraData["bestStat"] = bestStat.ToString().ToLower();
                        battle.Add("-start", pokemon, $"quarkdrive{bestStat.ToString().ToLower()}");
                        return null;
                    },
                    OnModifyAtkPriority = 5,
                    OnModifyAtk = (battle, atk, pokemon, defender, move) =>
                    {
                        IEffect? effectState = pokemon.GetVolatile("quarkdrive");
                        string? bestStat = effectState?.ExtraData?.GetValueOrDefault("bestStat") as string;

                        if (bestStat != "atk" || pokemon.IgnoringAbility())
                            return atk;

                        battle.Debug("Quark Drive atk boost");
                        battle.ChainModify(new int[] { 5325, 4096 });
                        return atk;
                    },
                    OnModifyDefPriority = 6,
                    OnModifyDef = (battle, def, pokemon, attacker, move) =>
                    {
                        IEffect? effectState = pokemon.GetVolatile("quarkdrive");
                        string? bestStat = effectState?.ExtraData?.GetValueOrDefault("bestStat") as string;

                        if (bestStat != "def" || pokemon.IgnoringAbility())
                            return def;

                        battle.Debug("Quark Drive def boost");
                        battle.ChainModify(new int[] { 5325, 4096 });
                        return def;
                    },
                    OnModifySpAPriority = 5,
                    OnModifySpA = (battle, spa, pokemon, defender, move) =>
                    {
                        IEffect? effectState = pokemon.GetVolatile("quarkdrive");
                        string? bestStat = effectState?.ExtraData?.GetValueOrDefault("bestStat") as string;

                        if (bestStat != "spa" || pokemon.IgnoringAbility())
                            return spa;

                        battle.Debug("Quark Drive spa boost");
                        battle.ChainModify(new int[] { 5325, 4096 });
                        return spa;
                    },
                    OnModifySpDPriority = 6,
                    OnModifySpD = (battle, spd, pokemon, attacker, move) =>
                    {
                        IEffect? effectState = pokemon.GetVolatile("quarkdrive");
                        string? bestStat = effectState?.ExtraData?.GetValueOrDefault("bestStat") as string;

                        if (bestStat != "spd" || pokemon.IgnoringAbility())
                            return spd;

                        battle.Debug("Quark Drive spd boost");
                        battle.ChainModify(new int[] { 5325, 4096 });
                        return spd;
                    },
                    OnModifySpe = (battle, spe, pokemon) =>
                    {
                        IEffect? effectState = pokemon.GetVolatile("quarkdrive");
                        string? bestStat = effectState?.ExtraData?.GetValueOrDefault("bestStat") as string;

                        if (bestStat != "spe" || pokemon.IgnoringAbility())
                            return spe;

                        battle.Debug("Quark Drive spe boost");
                        battle.ChainModify(3, 2); // 1.5x boost (3/2 = 1.5)
                        return spe;
                    },
                    OnEnd = (battle, pokemon) =>
                    {
                        battle.Add("-end", pokemon, "Quark Drive");
                    },
                },
                Name = "Quark Drive",
                Rating = 3,
                Num = 282,
                Fullname = string.Empty,
                EffectType = EffectType.Condition,
                Exists = true,
                Gen = 0,
                NoCopy = false,
                AffectsFainted = false,
                SourceEffect = string.Empty,
                SuppressWeather = false,
                Flags = new AbilityFlags()
                {
                    FailRolePlay = true,
                    NoReceiver = true,
                    NoEntrain = true,
                    NoTrace = true,
                    FailSkillSwap = true,
                    NoTransform = true
                }
            },
        };
    }
}
