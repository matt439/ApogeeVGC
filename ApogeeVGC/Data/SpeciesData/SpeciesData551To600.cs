using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData551To600()
    {
        return new Dictionary<SpecieId, Species>
        {
            [SpecieId.Cottonee] = new()
            {
                Id = SpecieId.Cottonee,
                Num = 546,
                Name = "Cottonee",
                Types = [PokemonType.Grass, PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 27,
                    Def = 60,
                    SpA = 37,
                    SpD = 50,
                    Spe = 66,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Prankster,
                    Slot1 = AbilityId.Infiltrator,
                    Hidden = AbilityId.Chlorophyll,
                },
                HeightM = 0.3,
                WeightKg = 0.6,
                Color = "Green",
            },
            [SpecieId.Whimsicott] = new()
            {
                Id = SpecieId.Whimsicott,
                Num = 547,
                Name = "Whimsicott",
                Types = [PokemonType.Grass, PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 67,
                    Def = 85,
                    SpA = 77,
                    SpD = 75,
                    Spe = 116,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Prankster,
                    Slot1 = AbilityId.Infiltrator,
                    Hidden = AbilityId.Chlorophyll,
                },
                HeightM = 0.7,
                WeightKg = 6.6,
                Color = "Green",
                Prevo = SpecieId.Cottonee,
            },
            [SpecieId.Petilil] = new()
            {
                Id = SpecieId.Petilil,
                Num = 548,
                Name = "Petilil",
                Types = [PokemonType.Grass],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 35,
                    Def = 50,
                    SpA = 70,
                    SpD = 50,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Chlorophyll,
                    Slot1 = AbilityId.OwnTempo,
                    Hidden = AbilityId.LeafGuard,
                },
                HeightM = 0.5,
                WeightKg = 6.6,
                Color = "Green",
            },
            [SpecieId.Lilligant] = new()
            {
                Id = SpecieId.Lilligant,
                Num = 549,
                Name = "Lilligant",
                Types = [PokemonType.Grass],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 60,
                    Def = 75,
                    SpA = 110,
                    SpD = 75,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Chlorophyll,
                    Slot1 = AbilityId.OwnTempo,
                    Hidden = AbilityId.LeafGuard,
                },
                HeightM = 1.1,
                WeightKg = 16.3,
                Color = "Green",
                Prevo = SpecieId.Petilil,
            },
            [SpecieId.LilligantHisui] = new()
            {
                Id = SpecieId.LilligantHisui,
                Num = 549,
                Name = "Lilligant-Hisui",
                Types = [PokemonType.Grass, PokemonType.Fighting],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 105,
                    Def = 75,
                    SpA = 50,
                    SpD = 75,
                    Spe = 105,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Chlorophyll,
                    Slot1 = AbilityId.Hustle,
                    Hidden = AbilityId.LeafGuard,
                },
                HeightM = 1.2,
                WeightKg = 19.2,
                Color = "Green",
                BaseSpecies = SpecieId.Lilligant,
                Forme = FormeId.Hisui,
                Prevo = SpecieId.Petilil,
            },
            [SpecieId.Basculin] = new()
            {
                Id = SpecieId.Basculin,
                Num = 550,
                Name = "Basculin",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 92,
                    Def = 65,
                    SpA = 80,
                    SpD = 55,
                    Spe = 98,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Reckless,
                    Slot1 = AbilityId.Adaptability,
                    Hidden = AbilityId.MoldBreaker,
                },
                HeightM = 1,
                WeightKg = 18,
                Color = "Green",
            },
            [SpecieId.BasculinBlueStriped] = new()
            {
                Id = SpecieId.BasculinBlueStriped,
                Num = 550,
                Name = "Basculin-Blue-Striped",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 92,
                    Def = 65,
                    SpA = 80,
                    SpD = 55,
                    Spe = 98,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RockHead,
                    Slot1 = AbilityId.Adaptability,
                    Hidden = AbilityId.MoldBreaker,
                },
                HeightM = 1,
                WeightKg = 18,
                Color = "Green",
                BaseSpecies = SpecieId.Basculin,
                Forme = FormeId.BlueStriped,
            },
            [SpecieId.BasculinWhiteStriped] = new()
            {
                Id = SpecieId.BasculinWhiteStriped,
                Num = 550,
                Name = "Basculin-White-Striped",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 92,
                    Def = 65,
                    SpA = 80,
                    SpD = 55,
                    Spe = 98,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Rattled,
                    Slot1 = AbilityId.Adaptability,
                    Hidden = AbilityId.MoldBreaker,
                },
                HeightM = 1,
                WeightKg = 18,
                Color = "Green",
                BaseSpecies = SpecieId.Basculin,
                Forme = FormeId.WhiteStriped,
            },
            [SpecieId.Sandile] = new()
            {
                Id = SpecieId.Sandile,
                Num = 551,
                Name = "Sandile",
                Types = [PokemonType.Ground, PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 72,
                    Def = 35,
                    SpA = 35,
                    SpD = 35,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Intimidate,
                    Slot1 = AbilityId.Moxie,
                    Hidden = AbilityId.AngerPoint,
                },
                HeightM = 0.7,
                WeightKg = 15.2,
                Color = "Brown",
            },
            [SpecieId.Krokorok] = new()
            {
                Id = SpecieId.Krokorok,
                Num = 552,
                Name = "Krokorok",
                Types = [PokemonType.Ground, PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 82,
                    Def = 45,
                    SpA = 45,
                    SpD = 45,
                    Spe = 74,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Intimidate,
                    Slot1 = AbilityId.Moxie,
                    Hidden = AbilityId.AngerPoint,
                },
                HeightM = 1,
                WeightKg = 33.4,
                Color = "Brown",
                Prevo = SpecieId.Sandile,
            },
            [SpecieId.Krookodile] = new()
            {
                Id = SpecieId.Krookodile,
                Num = 553,
                Name = "Krookodile",
                Types = [PokemonType.Ground, PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 117,
                    Def = 80,
                    SpA = 65,
                    SpD = 70,
                    Spe = 92,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Intimidate,
                    Slot1 = AbilityId.Moxie,
                    Hidden = AbilityId.AngerPoint,
                },
                HeightM = 1.5,
                WeightKg = 96.3,
                Color = "Red",
                Prevo = SpecieId.Krokorok,
            },
            [SpecieId.Darumaka] = new()
            {
                Id = SpecieId.Darumaka,
                Num = 554,
                Name = "Darumaka",
                Types = [PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 90,
                    Def = 45,
                    SpA = 15,
                    SpD = 45,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Hustle,
                    Hidden = AbilityId.InnerFocus,
                },
                HeightM = 0.6,
                WeightKg = 37.5,
                Color = "Red",
            },
            [SpecieId.DarumakaGalar] = new()
            {
                Id = SpecieId.DarumakaGalar,
                Num = 554,
                Name = "Darumaka-Galar",
                Types = [PokemonType.Ice],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 90,
                    Def = 45,
                    SpA = 15,
                    SpD = 45,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Hustle,
                    Hidden = AbilityId.InnerFocus,
                },
                HeightM = 0.7,
                WeightKg = 40,
                Color = "White",
                BaseSpecies = SpecieId.Darumaka,
                Forme = FormeId.Galar,
            },
            [SpecieId.Darmanitan] = new()
            {
                Id = SpecieId.Darmanitan,
                Num = 555,
                Name = "Darmanitan",
                Types = [PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 105,
                    Atk = 140,
                    Def = 55,
                    SpA = 30,
                    SpD = 55,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SheerForce,
                    Hidden = AbilityId.ZenMode,
                },
                HeightM = 1.3,
                WeightKg = 92.9,
                Color = "Red",
                Prevo = SpecieId.Darumaka,
            },
            [SpecieId.DarmanitanZen] = new()
            {
                Id = SpecieId.DarmanitanZen,
                Num = 555,
                Name = "Darmanitan-Zen",
                Types = [PokemonType.Fire, PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 105,
                    Atk = 30,
                    Def = 105,
                    SpA = 140,
                    SpD = 105,
                    Spe = 55,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ZenMode,
                },
                HeightM = 1.3,
                WeightKg = 92.9,
                Color = "Blue",
                BaseSpecies = SpecieId.Darmanitan,
                Forme = FormeId.Zen,
                ChangesFrom = FormeId.Standard,
            },
            [SpecieId.DarmanitanGalar] = new()
            {
                Id = SpecieId.DarmanitanGalar,
                Num = 555,
                Name = "Darmanitan-Galar",
                Types = [PokemonType.Ice],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 105,
                    Atk = 140,
                    Def = 55,
                    SpA = 30,
                    SpD = 55,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.GorillaTactics,
                    Hidden = AbilityId.ZenMode,
                },
                HeightM = 1.7,
                WeightKg = 120,
                Color = "White",
                BaseSpecies = SpecieId.Darmanitan,
                Forme = FormeId.Galar,
                Prevo = SpecieId.DarumakaGalar,
            },
            [SpecieId.DarmanitanGalarZen] = new()
            {
                Id = SpecieId.DarmanitanGalarZen,
                Num = 555,
                Name = "Darmanitan-Galar-Zen",
                Types = [PokemonType.Ice, PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 105,
                    Atk = 160,
                    Def = 55,
                    SpA = 30,
                    SpD = 55,
                    Spe = 135,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ZenMode,
                },
                HeightM = 1.7,
                WeightKg = 120,
                Color = "White",
                BaseSpecies = SpecieId.Darmanitan,
                Forme = FormeId.GalarZen,
                ChangesFrom = FormeId.Galar,
            },
            [SpecieId.Maractus] = new()
            {
                Id = SpecieId.Maractus,
                Num = 556,
                Name = "Maractus",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 86,
                    Def = 67,
                    SpA = 106,
                    SpD = 67,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.WaterAbsorb,
                    Slot1 = AbilityId.Chlorophyll,
                    Hidden = AbilityId.StormDrain,
                },
                HeightM = 1,
                WeightKg = 28,
                Color = "Green",
            },
            [SpecieId.Dwebble] = new()
            {
                Id = SpecieId.Dwebble,
                Num = 557,
                Name = "Dwebble",
                Types = [PokemonType.Bug, PokemonType.Rock],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 65,
                    Def = 85,
                    SpA = 35,
                    SpD = 35,
                    Spe = 55,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Sturdy,
                    Slot1 = AbilityId.ShellArmor,
                    Hidden = AbilityId.WeakArmor,
                },
                HeightM = 0.3,
                WeightKg = 14.5,
                Color = "Red",
            },
            [SpecieId.Crustle] = new()
            {
                Id = SpecieId.Crustle,
                Num = 558,
                Name = "Crustle",
                Types = [PokemonType.Bug, PokemonType.Rock],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 105,
                    Def = 125,
                    SpA = 65,
                    SpD = 75,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Sturdy,
                    Slot1 = AbilityId.ShellArmor,
                    Hidden = AbilityId.WeakArmor,
                },
                HeightM = 1.4,
                WeightKg = 200,
                Color = "Red",
                Prevo = SpecieId.Dwebble,
            },
            [SpecieId.Scraggy] = new()
            {
                Id = SpecieId.Scraggy,
                Num = 559,
                Name = "Scraggy",
                Types = [PokemonType.Dark, PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 75,
                    Def = 70,
                    SpA = 35,
                    SpD = 70,
                    Spe = 48,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShedSkin,
                    Slot1 = AbilityId.Moxie,
                    Hidden = AbilityId.Intimidate,
                },
                HeightM = 0.6,
                WeightKg = 11.8,
                Color = "Yellow",
            },
            [SpecieId.Scrafty] = new()
            {
                Id = SpecieId.Scrafty,
                Num = 560,
                Name = "Scrafty",
                Types = [PokemonType.Dark, PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 90,
                    Def = 115,
                    SpA = 45,
                    SpD = 115,
                    Spe = 58,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShedSkin,
                    Slot1 = AbilityId.Moxie,
                    Hidden = AbilityId.Intimidate,
                },
                HeightM = 1.1,
                WeightKg = 30,
                Color = "Red",
                Prevo = SpecieId.Scraggy,
            },
            [SpecieId.ScraftyMega] = new()
            {
                Id = SpecieId.ScraftyMega,
                Num = 560,
                Name = "Scrafty-Mega",
                Types = [PokemonType.Dark, PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 130,
                    Def = 135,
                    SpA = 55,
                    SpD = 135,
                    Spe = 68,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShedSkin,
                    Slot1 = AbilityId.Moxie,
                    Hidden = AbilityId.Intimidate,
                },
                HeightM = 1.1,
                WeightKg = 31,
                Color = "Red",
                BaseSpecies = SpecieId.Scrafty,
                Forme = FormeId.Mega,
            },
            [SpecieId.Sigilyph] = new()
            {
                Id = SpecieId.Sigilyph,
                Num = 561,
                Name = "Sigilyph",
                Types = [PokemonType.Psychic, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 72,
                    Atk = 58,
                    Def = 80,
                    SpA = 103,
                    SpD = 80,
                    Spe = 97,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.WonderSkin,
                    Slot1 = AbilityId.MagicGuard,
                    Hidden = AbilityId.TintedLens,
                },
                HeightM = 1.4,
                WeightKg = 14,
                Color = "Black",
            },
            [SpecieId.Yamask] = new()
            {
                Id = SpecieId.Yamask,
                Num = 562,
                Name = "Yamask",
                Types = [PokemonType.Ghost],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 38,
                    Atk = 30,
                    Def = 85,
                    SpA = 55,
                    SpD = 65,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Mummy,
                },
                HeightM = 0.5,
                WeightKg = 1.5,
                Color = "Black",
            },
            [SpecieId.YamaskGalar] = new()
            {
                Id = SpecieId.YamaskGalar,
                Num = 562,
                Name = "Yamask-Galar",
                Types = [PokemonType.Ground, PokemonType.Ghost],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 38,
                    Atk = 55,
                    Def = 85,
                    SpA = 30,
                    SpD = 65,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.WanderingSpirit,
                },
                HeightM = 0.5,
                WeightKg = 1.5,
                Color = "Black",
                BaseSpecies = SpecieId.Yamask,
                Forme = FormeId.Galar,
            },
            [SpecieId.Cofagrigus] = new()
            {
                Id = SpecieId.Cofagrigus,
                Num = 563,
                Name = "Cofagrigus",
                Types = [PokemonType.Ghost],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 58,
                    Atk = 50,
                    Def = 145,
                    SpA = 95,
                    SpD = 105,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Mummy,
                },
                HeightM = 1.7,
                WeightKg = 76.5,
                Color = "Yellow",
                Prevo = SpecieId.Yamask,
            },
            [SpecieId.Tirtouga] = new()
            {
                Id = SpecieId.Tirtouga,
                Num = 564,
                Name = "Tirtouga",
                Types = [PokemonType.Water, PokemonType.Rock],
                Gender = GenderId.M875F125,
                BaseStats = new StatsTable
                {
                    Hp = 54,
                    Atk = 78,
                    Def = 103,
                    SpA = 53,
                    SpD = 45,
                    Spe = 22,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SolidRock,
                    Slot1 = AbilityId.Sturdy,
                    Hidden = AbilityId.SwiftSwim,
                },
                HeightM = 0.7,
                WeightKg = 16.5,
                Color = "Blue",
            },
            [SpecieId.Carracosta] = new()
            {
                Id = SpecieId.Carracosta,
                Num = 565,
                Name = "Carracosta",
                Types = [PokemonType.Water, PokemonType.Rock],
                Gender = GenderId.M875F125,
                BaseStats = new StatsTable
                {
                    Hp = 74,
                    Atk = 108,
                    Def = 133,
                    SpA = 83,
                    SpD = 65,
                    Spe = 32,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SolidRock,
                    Slot1 = AbilityId.Sturdy,
                    Hidden = AbilityId.SwiftSwim,
                },
                HeightM = 1.2,
                WeightKg = 81,
                Color = "Blue",
                Prevo = SpecieId.Tirtouga,
            },
            [SpecieId.Archen] = new()
            {
                Id = SpecieId.Archen,
                Num = 566,
                Name = "Archen",
                Types = [PokemonType.Rock, PokemonType.Flying],
                Gender = GenderId.M875F125,
                BaseStats = new StatsTable
                {
                    Hp = 55,
                    Atk = 112,
                    Def = 45,
                    SpA = 74,
                    SpD = 45,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Defeatist,
                },
                HeightM = 0.5,
                WeightKg = 9.5,
                Color = "Yellow",
            },
            [SpecieId.Archeops] = new()
            {
                Id = SpecieId.Archeops,
                Num = 567,
                Name = "Archeops",
                Types = [PokemonType.Rock, PokemonType.Flying],
                Gender = GenderId.M875F125,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 140,
                    Def = 65,
                    SpA = 112,
                    SpD = 65,
                    Spe = 110,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Defeatist,
                },
                HeightM = 1.4,
                WeightKg = 32,
                Color = "Yellow",
                Prevo = SpecieId.Archen,
            },
            [SpecieId.Trubbish] = new()
            {
                Id = SpecieId.Trubbish,
                Num = 568,
                Name = "Trubbish",
                Types = [PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 50,
                    Def = 62,
                    SpA = 40,
                    SpD = 62,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Stench,
                    Slot1 = AbilityId.StickyHold,
                    Hidden = AbilityId.Aftermath,
                },
                HeightM = 0.6,
                WeightKg = 31,
                Color = "Green",
            },
            [SpecieId.Garbodor] = new()
            {
                Id = SpecieId.Garbodor,
                Num = 569,
                Name = "Garbodor",
                Types = [PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 95,
                    Def = 82,
                    SpA = 60,
                    SpD = 82,
                    Spe = 75,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Stench,
                    Slot1 = AbilityId.WeakArmor,
                    Hidden = AbilityId.Aftermath,
                },
                HeightM = 1.9,
                WeightKg = 107.3,
                Color = "Green",
                Prevo = SpecieId.Trubbish,
            },
            [SpecieId.GarbodorGmax] = new()
            {
                Id = SpecieId.GarbodorGmax,
                Num = 569,
                Name = "Garbodor-Gmax",
                Types = [PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 95,
                    Def = 82,
                    SpA = 60,
                    SpD = 82,
                    Spe = 75,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Stench,
                    Slot1 = AbilityId.WeakArmor,
                    Hidden = AbilityId.Aftermath,
                },
                HeightM = 21,
                WeightKg = 0,
                Color = "Green",
                BaseSpecies = SpecieId.Garbodor,
                Forme = FormeId.Gmax,
                ChangesFrom = FormeId.Standard,
            },
            [SpecieId.Zorua] = new()
            {
                Id = SpecieId.Zorua,
                Num = 570,
                Name = "Zorua",
                Types = [PokemonType.Dark],
                Gender = GenderId.M875F125,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 65,
                    Def = 40,
                    SpA = 80,
                    SpD = 40,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Illusion,
                },
                HeightM = 0.7,
                WeightKg = 12.5,
                Color = "Gray",
            },
            [SpecieId.ZoruaHisui] = new()
            {
                Id = SpecieId.ZoruaHisui,
                Num = 570,
                Name = "Zorua-Hisui",
                Types = [PokemonType.Normal, PokemonType.Ghost],
                Gender = GenderId.M875F125,
                BaseStats = new StatsTable
                {
                    Hp = 35,
                    Atk = 60,
                    Def = 40,
                    SpA = 85,
                    SpD = 40,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Illusion,
                },
                HeightM = 0.7,
                WeightKg = 12.5,
                Color = "Gray",
                BaseSpecies = SpecieId.Zorua,
                Forme = FormeId.Hisui,
            },
            [SpecieId.Zoroark] = new()
            {
                Id = SpecieId.Zoroark,
                Num = 571,
                Name = "Zoroark",
                Types = [PokemonType.Dark],
                Gender = GenderId.M875F125,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 105,
                    Def = 60,
                    SpA = 120,
                    SpD = 60,
                    Spe = 105,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Illusion,
                },
                HeightM = 1.6,
                WeightKg = 81.1,
                Color = "Gray",
                Prevo = SpecieId.Zorua,
            },
            [SpecieId.ZoroarkHisui] = new()
            {
                Id = SpecieId.ZoroarkHisui,
                Num = 571,
                Name = "Zoroark-Hisui",
                Types = [PokemonType.Normal, PokemonType.Ghost],
                Gender = GenderId.M875F125,
                BaseStats = new StatsTable
                {
                    Hp = 55,
                    Atk = 100,
                    Def = 60,
                    SpA = 125,
                    SpD = 60,
                    Spe = 110,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Illusion,
                },
                HeightM = 1.6,
                WeightKg = 73,
                Color = "Gray",
                BaseSpecies = SpecieId.Zoroark,
                Forme = FormeId.Hisui,
                Prevo = SpecieId.ZoruaHisui,
            },
            [SpecieId.Minccino] = new()
            {
                Id = SpecieId.Minccino,
                Num = 572,
                Name = "Minccino",
                Types = [PokemonType.Normal],
                Gender = GenderId.M25F75,
                BaseStats = new StatsTable
                {
                    Hp = 55,
                    Atk = 50,
                    Def = 40,
                    SpA = 40,
                    SpD = 40,
                    Spe = 75,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.CuteCharm,
                    Slot1 = AbilityId.Technician,
                    Hidden = AbilityId.SkillLink,
                },
                HeightM = 0.4,
                WeightKg = 5.8,
                Color = "Gray",
            },
            [SpecieId.Cinccino] = new()
            {
                Id = SpecieId.Cinccino,
                Num = 573,
                Name = "Cinccino",
                Types = [PokemonType.Normal],
                Gender = GenderId.M25F75,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 95,
                    Def = 60,
                    SpA = 65,
                    SpD = 60,
                    Spe = 115,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.CuteCharm,
                    Slot1 = AbilityId.Technician,
                    Hidden = AbilityId.SkillLink,
                },
                HeightM = 0.5,
                WeightKg = 7.5,
                Color = "Gray",
                Prevo = SpecieId.Minccino,
            },
                        [SpecieId.Gothita] = new()
                        {
                            Id = SpecieId.Gothita,
                            Num = 574,
                            Name = "Gothita",
                            Types = [PokemonType.Psychic],
                            Gender = GenderId.M25F75,
                            BaseStats = new StatsTable
                            {
                                Hp = 45,
                                Atk = 30,
                                Def = 50,
                                SpA = 55,
                                SpD = 65,
                                Spe = 45,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Frisk,
                                Slot1 = AbilityId.Competitive,
                                Hidden = AbilityId.ShadowTag,
                            },
                            HeightM = 0.4,
                            WeightKg = 5.8,
                            Color = "Purple",
                        },
                        [SpecieId.Gothorita] = new()
                        {
                            Id = SpecieId.Gothorita,
                            Num = 575,
                            Name = "Gothorita",
                            Types = [PokemonType.Psychic],
                            Gender = GenderId.M25F75,
                            BaseStats = new StatsTable
                            {
                                Hp = 60,
                                Atk = 45,
                                Def = 70,
                                SpA = 75,
                                SpD = 85,
                                Spe = 55,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Frisk,
                                Slot1 = AbilityId.Competitive,
                                Hidden = AbilityId.ShadowTag,
                            },
                            HeightM = 0.7,
                            WeightKg = 18,
                            Color = "Purple",
                            Prevo = SpecieId.Gothita,
                        },
                        [SpecieId.Gothitelle] = new()
                        {
                            Id = SpecieId.Gothitelle,
                            Num = 576,
                            Name = "Gothitelle",
                            Types = [PokemonType.Psychic],
                            Gender = GenderId.M25F75,
                            BaseStats = new StatsTable
                            {
                                Hp = 70,
                                Atk = 55,
                                Def = 95,
                                SpA = 95,
                                SpD = 110,
                                Spe = 65,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Frisk,
                                Slot1 = AbilityId.Competitive,
                                Hidden = AbilityId.ShadowTag,
                            },
                            HeightM = 1.5,
                            WeightKg = 44,
                            Color = "Purple",
                            Prevo = SpecieId.Gothorita,
                        },
                        [SpecieId.Solosis] = new()
                        {
                            Id = SpecieId.Solosis,
                            Num = 577,
                            Name = "Solosis",
                            Types = [PokemonType.Psychic],
                            Gender = GenderId.Empty,
                            BaseStats = new StatsTable
                            {
                                Hp = 45,
                                Atk = 30,
                                Def = 40,
                                SpA = 105,
                                SpD = 50,
                                Spe = 20,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Overcoat,
                                Slot1 = AbilityId.MagicGuard,
                                Hidden = AbilityId.Regenerator,
                            },
                            HeightM = 0.3,
                            WeightKg = 1,
                            Color = "Green",
                        },
                        [SpecieId.Duosion] = new()
                        {
                            Id = SpecieId.Duosion,
                            Num = 578,
                            Name = "Duosion",
                            Types = [PokemonType.Psychic],
                            Gender = GenderId.Empty,
                            BaseStats = new StatsTable
                            {
                                Hp = 65,
                                Atk = 40,
                                Def = 50,
                                SpA = 125,
                                SpD = 60,
                                Spe = 30,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Overcoat,
                                Slot1 = AbilityId.MagicGuard,
                                Hidden = AbilityId.Regenerator,
                            },
                            HeightM = 0.6,
                            WeightKg = 8,
                            Color = "Green",
                            Prevo = SpecieId.Solosis,
                        },
                        [SpecieId.Reuniclus] = new()
                        {
                            Id = SpecieId.Reuniclus,
                            Num = 579,
                            Name = "Reuniclus",
                            Types = [PokemonType.Psychic],
                            Gender = GenderId.Empty,
                            BaseStats = new StatsTable
                            {
                                Hp = 110,
                                Atk = 65,
                                Def = 75,
                                SpA = 125,
                                SpD = 85,
                                Spe = 30,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Overcoat,
                                Slot1 = AbilityId.MagicGuard,
                                Hidden = AbilityId.Regenerator,
                            },
                            HeightM = 1,
                            WeightKg = 20.1,
                            Color = "Green",
                            Prevo = SpecieId.Duosion,
                        },
                        [SpecieId.Ducklett] = new()
                        {
                            Id = SpecieId.Ducklett,
                            Num = 580,
                            Name = "Ducklett",
                            Types = [PokemonType.Water, PokemonType.Flying],
                            Gender = GenderId.Empty,
                            BaseStats = new StatsTable
                            {
                                Hp = 62,
                                Atk = 44,
                                Def = 50,
                                SpA = 44,
                                SpD = 50,
                                Spe = 55,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.KeenEye,
                                Slot1 = AbilityId.BigPecks,
                                Hidden = AbilityId.Hydration,
                            },
                            HeightM = 0.5,
                            WeightKg = 5.5,
                            Color = "Blue",
                        },
                        [SpecieId.Swanna] = new()
                        {
                            Id = SpecieId.Swanna,
                            Num = 581,
                            Name = "Swanna",
                            Types = [PokemonType.Water, PokemonType.Flying],
                            Gender = GenderId.Empty,
                            BaseStats = new StatsTable
                            {
                                Hp = 75,
                                Atk = 87,
                                Def = 63,
                                SpA = 87,
                                SpD = 63,
                                Spe = 98,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.KeenEye,
                                Slot1 = AbilityId.BigPecks,
                                Hidden = AbilityId.Hydration,
                            },
                            HeightM = 1.3,
                            WeightKg = 24.2,
                            Color = "White",
                            Prevo = SpecieId.Ducklett,
                        },
                        [SpecieId.Vanillite] = new()
                        {
                            Id = SpecieId.Vanillite,
                            Num = 582,
                            Name = "Vanillite",
                            Types = [PokemonType.Ice],
                            Gender = GenderId.Empty,
                            BaseStats = new StatsTable
                            {
                                Hp = 36,
                                Atk = 50,
                                Def = 50,
                                SpA = 65,
                                SpD = 60,
                                Spe = 44,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.IceBody,
                                Slot1 = AbilityId.SnowCloak,
                                Hidden = AbilityId.WeakArmor,
                            },
                            HeightM = 0.4,
                            WeightKg = 5.7,
                            Color = "White",
                        },
                        [SpecieId.Vanillish] = new()
                        {
                            Id = SpecieId.Vanillish,
                            Num = 583,
                            Name = "Vanillish",
                            Types = [PokemonType.Ice],
                            Gender = GenderId.Empty,
                            BaseStats = new StatsTable
                            {
                                Hp = 51,
                                Atk = 65,
                                Def = 65,
                                SpA = 80,
                                SpD = 75,
                                Spe = 59,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.IceBody,
                                Slot1 = AbilityId.SnowCloak,
                                Hidden = AbilityId.WeakArmor,
                            },
                            HeightM = 1.1,
                            WeightKg = 41,
                            Color = "White",
                            Prevo = SpecieId.Vanillite,
                        },
                        [SpecieId.Vanilluxe] = new()
                        {
                            Id = SpecieId.Vanilluxe,
                            Num = 584,
                            Name = "Vanilluxe",
                            Types = [PokemonType.Ice],
                            Gender = GenderId.Empty,
                            BaseStats = new StatsTable
                            {
                                Hp = 71,
                                Atk = 95,
                                Def = 85,
                                SpA = 110,
                                SpD = 95,
                                Spe = 79,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.IceBody,
                                Slot1 = AbilityId.SnowWarning,
                                Hidden = AbilityId.WeakArmor,
                            },
                            HeightM = 1.3,
                            WeightKg = 57.5,
                            Color = "White",
                            Prevo = SpecieId.Vanillish,
                        },
                        [SpecieId.Deerling] = new()
                        {
                            Id = SpecieId.Deerling,
                            Num = 585,
                            Name = "Deerling",
                            Types = [PokemonType.Normal, PokemonType.Grass],
                            Gender = GenderId.Empty,
                            BaseStats = new StatsTable
                            {
                                Hp = 60,
                                Atk = 60,
                                Def = 50,
                                SpA = 40,
                                SpD = 50,
                                Spe = 75,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Chlorophyll,
                                Slot1 = AbilityId.SapSipper,
                                Hidden = AbilityId.SereneGrace,
                            },
                            HeightM = 0.6,
                            WeightKg = 19.5,
                            Color = "Pink",
                        },
                        [SpecieId.Sawsbuck] = new()
                        {
                            Id = SpecieId.Sawsbuck,
                            Num = 586,
                            Name = "Sawsbuck",
                            Types = [PokemonType.Normal, PokemonType.Grass],
                            Gender = GenderId.Empty,
                            BaseStats = new StatsTable
                            {
                                Hp = 80,
                                Atk = 100,
                                Def = 70,
                                SpA = 60,
                                SpD = 70,
                                Spe = 95,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Chlorophyll,
                                Slot1 = AbilityId.SapSipper,
                                Hidden = AbilityId.SereneGrace,
                            },
                            HeightM = 1.9,
                            WeightKg = 92.5,
                            Color = "Brown",
                            Prevo = SpecieId.Deerling,
                        },
                        [SpecieId.Emolga] = new()
                        {
                            Id = SpecieId.Emolga,
                            Num = 587,
                            Name = "Emolga",
                            Types = [PokemonType.Electric, PokemonType.Flying],
                            Gender = GenderId.Empty,
                            BaseStats = new StatsTable
                            {
                                Hp = 55,
                                Atk = 75,
                                Def = 60,
                                SpA = 75,
                                SpD = 60,
                                Spe = 103,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Static,
                                Hidden = AbilityId.MotorDrive,
                            },
                            HeightM = 0.4,
                            WeightKg = 5,
                            Color = "White",
                        },
                    };
                }
            }
