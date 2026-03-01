using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData0051to0100()
    {
        return new Dictionary<SpecieId, Species>
        {
            [SpecieId.Dugtrio] = new()
            {
                Id = SpecieId.Dugtrio,
                Num = 51,
                Name = "Dugtrio",
                Types = [PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 35,
                    Atk = 100,
                    Def = 50,
                    SpA = 50,
                    SpD = 70,
                    Spe = 120,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SandVeil,
                    Slot1 = AbilityId.ArenaTrap,
                    Hidden = AbilityId.SandForce,
                },
                HeightM = 0.7,
                WeightKg = 33.3,
                Color = "Brown",
            },
            [SpecieId.DugtrioAlola] = new()
            {
                Id = SpecieId.DugtrioAlola,
                Num = 51,
                Name = "Dugtrio-Alola",
                BaseSpecies = SpecieId.Dugtrio,
                Forme = FormeId.Alola,
                Types = [PokemonType.Ground, PokemonType.Steel],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 35,
                    Atk = 100,
                    Def = 60,
                    SpA = 50,
                    SpD = 70,
                    Spe = 110,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SandVeil,
                    Slot1 = AbilityId.TanglingHair,
                    Hidden = AbilityId.SandForce,
                },
                HeightM = 0.7,
                WeightKg = 66.6,
                Color = "Brown",
            },
            [SpecieId.Meowth] = new()
            {
                Id = SpecieId.Meowth,
                Num = 52,
                Name = "Meowth",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 45,
                    Def = 35,
                    SpA = 40,
                    SpD = 40,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pickup,
                    Slot1 = AbilityId.Technician,
                    Hidden = AbilityId.Unnerve,
                },
                HeightM = 0.4,
                WeightKg = 4.2,
                Color = "Yellow",
            },
            [SpecieId.MeowthAlola] = new()
            {
                Id = SpecieId.MeowthAlola,
                Num = 52,
                Name = "Meowth-Alola",
                BaseSpecies = SpecieId.Meowth,
                Forme = FormeId.Alola,
                Types = [PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 35,
                    Def = 35,
                    SpA = 50,
                    SpD = 40,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pickup,
                    Slot1 = AbilityId.Technician,
                    Hidden = AbilityId.Rattled,
                },
                HeightM = 0.4,
                WeightKg = 4.2,
                Color = "Blue",
            },
            [SpecieId.MeowthGalar] = new()
            {
                Id = SpecieId.MeowthGalar,
                Num = 52,
                Name = "Meowth-Galar",
                BaseSpecies = SpecieId.Meowth,
                Forme = FormeId.Galar,
                Types = [PokemonType.Steel],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 65,
                    Def = 55,
                    SpA = 40,
                    SpD = 40,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pickup,
                    Slot1 = AbilityId.ToughClaws,
                    Hidden = AbilityId.Unnerve,
                },
                HeightM = 0.4,
                WeightKg = 7.5,
                    Color = "Brown",
                },
                [SpecieId.Persian] = new()
            {
                Id = SpecieId.Persian,
                Num = 53,
                Name = "Persian",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 70,
                    Def = 60,
                    SpA = 65,
                    SpD = 65,
                    Spe = 115,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Limber,
                    Slot1 = AbilityId.Technician,
                    Hidden = AbilityId.Unnerve,
                },
                HeightM = 1,
                WeightKg = 32,
                Color = "Yellow",
            },
            [SpecieId.PersianAlola] = new()
            {
                Id = SpecieId.PersianAlola,
                Num = 53,
                Name = "Persian-Alola",
                BaseSpecies = SpecieId.Persian,
                Forme = FormeId.Alola,
                Types = [PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 60,
                    Def = 60,
                    SpA = 75,
                    SpD = 65,
                    Spe = 115,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.FurCoat,
                    Slot1 = AbilityId.Technician,
                    Hidden = AbilityId.Rattled,
                },
                HeightM = 1.1,
                WeightKg = 33,
                Color = "Blue",
            },
            [SpecieId.Psyduck] = new()
            {
                Id = SpecieId.Psyduck,
                Num = 54,
                Name = "Psyduck",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 52,
                    Def = 48,
                    SpA = 65,
                    SpD = 50,
                    Spe = 55,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Damp,
                    Slot1 = AbilityId.CloudNine,
                    Hidden = AbilityId.SwiftSwim,
                },
                HeightM = 0.8,
                WeightKg = 19.6,
                Color = "Yellow",
            },
            [SpecieId.Golduck] = new()
            {
                Id = SpecieId.Golduck,
                Num = 55,
                Name = "Golduck",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 82,
                    Def = 78,
                    SpA = 95,
                    SpD = 80,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Damp,
                    Slot1 = AbilityId.CloudNine,
                    Hidden = AbilityId.SwiftSwim,
                },
                HeightM = 1.7,
                WeightKg = 76.6,
                Color = "Blue",
            },
            [SpecieId.Mankey] = new()
            {
                Id = SpecieId.Mankey,
                Num = 56,
                Name = "Mankey",
                Types = [PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 80,
                    Def = 35,
                    SpA = 35,
                    SpD = 45,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.VitalSpirit,
                    Slot1 = AbilityId.AngerPoint,
                    Hidden = AbilityId.Defiant,
                },
                HeightM = 0.5,
                WeightKg = 28,
                Color = "Brown",
            },
            [SpecieId.Primeape] = new()
            {
                Id = SpecieId.Primeape,
                Num = 57,
                Name = "Primeape",
                Types = [PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 105,
                    Def = 60,
                    SpA = 60,
                    SpD = 70,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.VitalSpirit,
                    Slot1 = AbilityId.AngerPoint,
                    Hidden = AbilityId.Defiant,
                },
                HeightM = 1,
                WeightKg = 32,
                Color = "Brown",
            },
            [SpecieId.Growlithe] = new()
            {
                Id = SpecieId.Growlithe,
                Num = 58,
                Name = "Growlithe",
                Types = [PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 55,
                    Atk = 70,
                    Def = 45,
                    SpA = 70,
                    SpD = 50,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Intimidate,
                    Slot1 = AbilityId.FlashFire,
                    Hidden = AbilityId.Justified,
                },
                HeightM = 0.7,
                WeightKg = 19,
                Color = "Brown",
            },
            [SpecieId.GrowlitheHisui] = new()
            {
                Id = SpecieId.GrowlitheHisui,
                Num = 58,
                Name = "Growlithe-Hisui",
                BaseSpecies = SpecieId.Growlithe,
                Forme = FormeId.Hisui,
                Types = [PokemonType.Fire, PokemonType.Rock],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 75,
                    Def = 45,
                    SpA = 65,
                    SpD = 50,
                    Spe = 55,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Intimidate,
                    Slot1 = AbilityId.FlashFire,
                    Hidden = AbilityId.RockHead,
                },
                HeightM = 0.8,
                WeightKg = 22.7,
                Color = "Brown",
            },
            [SpecieId.Arcanine] = new()
            {
                Id = SpecieId.Arcanine,
                Num = 59,
                Name = "Arcanine",
                Types = [PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 110,
                    Def = 80,
                    SpA = 100,
                    SpD = 80,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Intimidate,
                    Slot1 = AbilityId.FlashFire,
                    Hidden = AbilityId.Justified,
                },
                HeightM = 1.9,
                WeightKg = 155,
                Color = "Brown",
            },
            [SpecieId.ArcanineHisui] = new()
            {
                Id = SpecieId.ArcanineHisui,
                Num = 59,
                Name = "Arcanine-Hisui",
                BaseSpecies = SpecieId.Arcanine,
                Forme = FormeId.Hisui,
                Types = [PokemonType.Fire, PokemonType.Rock],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 115,
                    Def = 80,
                    SpA = 95,
                    SpD = 80,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Intimidate,
                    Slot1 = AbilityId.FlashFire,
                    Hidden = AbilityId.RockHead,
                },
                HeightM = 2,
                WeightKg = 168,
                Color = "Brown",
            },
            [SpecieId.Poliwag] = new()
            {
                Id = SpecieId.Poliwag,
                Num = 60,
                Name = "Poliwag",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 50,
                    Def = 40,
                    SpA = 40,
                    SpD = 40,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.WaterAbsorb,
                    Slot1 = AbilityId.Damp,
                    Hidden = AbilityId.SwiftSwim,
                },
                HeightM = 0.6,
                WeightKg = 12.4,
                Color = "Blue",
            },
            [SpecieId.Poliwhirl] = new()
            {
                Id = SpecieId.Poliwhirl,
                Num = 61,
                Name = "Poliwhirl",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 65,
                    Def = 65,
                    SpA = 50,
                    SpD = 50,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.WaterAbsorb,
                    Slot1 = AbilityId.Damp,
                    Hidden = AbilityId.SwiftSwim,
                },
                HeightM = 1,
                WeightKg = 20,
                Color = "Blue",
            },
            [SpecieId.Poliwrath] = new()
            {
                Id = SpecieId.Poliwrath,
                Num = 62,
                Name = "Poliwrath",
                Types = [PokemonType.Water, PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 95,
                    Def = 95,
                    SpA = 70,
                    SpD = 90,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.WaterAbsorb,
                    Slot1 = AbilityId.Damp,
                    Hidden = AbilityId.SwiftSwim,
                },
                HeightM = 1.3,
                WeightKg = 54,
                Color = "Blue",
            },
            [SpecieId.Abra] = new()
            {
                Id = SpecieId.Abra,
                Num = 63,
                Name = "Abra",
                Types = [PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 25,
                    Atk = 20,
                    Def = 15,
                    SpA = 105,
                    SpD = 55,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Synchronize,
                    Slot1 = AbilityId.InnerFocus,
                    Hidden = AbilityId.MagicGuard,
                },
                HeightM = 0.9,
                WeightKg = 19.5,
                Color = "Brown",
            },
            [SpecieId.Kadabra] = new()
            {
                Id = SpecieId.Kadabra,
                Num = 64,
                Name = "Kadabra",
                Types = [PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 35,
                    Def = 30,
                    SpA = 120,
                    SpD = 70,
                    Spe = 105,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Synchronize,
                    Slot1 = AbilityId.InnerFocus,
                    Hidden = AbilityId.MagicGuard,
                },
                HeightM = 1.3,
                WeightKg = 56.5,
                Color = "Brown",
            },
            [SpecieId.Alakazam] = new()
            {
                Id = SpecieId.Alakazam,
                Num = 65,
                Name = "Alakazam",
                Types = [PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 55,
                    Atk = 50,
                    Def = 45,
                    SpA = 135,
                    SpD = 95,
                    Spe = 120,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Synchronize,
                    Slot1 = AbilityId.InnerFocus,
                    Hidden = AbilityId.MagicGuard,
                },
                HeightM = 1.5,
                WeightKg = 48,
                Color = "Brown",
                OtherFormes = [FormeId.Mega],
            },
            [SpecieId.AlakazamMega] = new()
            {
                Id = SpecieId.AlakazamMega,
                Num = 65,
                Name = "Alakazam-Mega",
                BaseSpecies = SpecieId.Alakazam,
                Forme = FormeId.Mega,
                Types = [PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 55,
                    Atk = 50,
                    Def = 65,
                    SpA = 175,
                    SpD = 105,
                    Spe = 150,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Trace,
                },
                HeightM = 1.2,
                WeightKg = 48,
                Color = "Brown",
                RequiredItem = ItemId.Alakazite,
            },
            [SpecieId.Machop] = new()
            {
                Id = SpecieId.Machop,
                Num = 66,
                Name = "Machop",
                Types = [PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 80,
                    Def = 50,
                    SpA = 35,
                    SpD = 35,
                    Spe = 35,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Guts,
                    Slot1 = AbilityId.NoGuard,
                    Hidden = AbilityId.Steadfast,
                },
                HeightM = 0.8,
                WeightKg = 19.5,
                Color = "Gray",
            },
            [SpecieId.Machoke] = new()
            {
                Id = SpecieId.Machoke,
                Num = 67,
                Name = "Machoke",
                Types = [PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 100,
                    Def = 70,
                    SpA = 50,
                    SpD = 60,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Guts,
                    Slot1 = AbilityId.NoGuard,
                    Hidden = AbilityId.Steadfast,
                },
                HeightM = 1.5,
                WeightKg = 70.5,
                Color = "Gray",
            },
            [SpecieId.Machamp] = new()
            {
                Id = SpecieId.Machamp,
                Num = 68,
                Name = "Machamp",
                Types = [PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 130,
                    Def = 80,
                    SpA = 65,
                    SpD = 85,
                    Spe = 55,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Guts,
                    Slot1 = AbilityId.NoGuard,
                    Hidden = AbilityId.Steadfast,
                },
                HeightM = 1.6,
                WeightKg = 130,
                Color = "Gray",
            },
            [SpecieId.Bellsprout] = new()
            {
                Id = SpecieId.Bellsprout,
                Num = 69,
                Name = "Bellsprout",
                Types = [PokemonType.Grass, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 75,
                    Def = 35,
                    SpA = 70,
                    SpD = 30,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Chlorophyll,
                    Hidden = AbilityId.Gluttony,
                },
                HeightM = 0.7,
                WeightKg = 4,
                Color = "Green",
            },
            [SpecieId.Weepinbell] = new()
            {
                Id = SpecieId.Weepinbell,
                Num = 70,
                Name = "Weepinbell",
                Types = [PokemonType.Grass, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 90,
                    Def = 50,
                    SpA = 85,
                    SpD = 45,
                    Spe = 55,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Chlorophyll,
                    Hidden = AbilityId.Gluttony,
                },
                HeightM = 1,
                WeightKg = 6.4,
                Color = "Green",
            },
            [SpecieId.Victreebel] = new()
            {
                Id = SpecieId.Victreebel,
                Num = 71,
                Name = "Victreebel",
                Types = [PokemonType.Grass, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 105,
                    Def = 65,
                    SpA = 100,
                    SpD = 70,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Chlorophyll,
                    Hidden = AbilityId.Gluttony,
                },
                HeightM = 1.7,
                WeightKg = 15.5,
                Color = "Green",
                OtherFormes = [FormeId.Mega],
            },
            [SpecieId.VictreebelMega] = new()
            {
                Id = SpecieId.VictreebelMega,
                Num = 71,
                Name = "Victreebel-Mega",
                BaseSpecies = SpecieId.Victreebel,
                Forme = FormeId.Mega,
                Types = [PokemonType.Grass, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 125,
                    Def = 85,
                    SpA = 135,
                    SpD = 95,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Chlorophyll,
                    Hidden = AbilityId.Gluttony,
                },
                HeightM = 4.5,
                WeightKg = 125.5,
                Color = "Green",
                RequiredItem = ItemId.Victrebelite,
            },
            [SpecieId.Tentacool] = new()
            {
                Id = SpecieId.Tentacool,
                Num = 72,
                Name = "Tentacool",
                Types = [PokemonType.Water, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 40,
                    Def = 35,
                    SpA = 50,
                    SpD = 100,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ClearBody,
                    Slot1 = AbilityId.LiquidOoze,
                    Hidden = AbilityId.RainDish,
                },
                HeightM = 0.9,
                WeightKg = 45.5,
                Color = "Blue",
            },
            [SpecieId.Tentacruel] = new()
            {
                Id = SpecieId.Tentacruel,
                Num = 73,
                Name = "Tentacruel",
                Types = [PokemonType.Water, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 70,
                    Def = 65,
                    SpA = 80,
                    SpD = 120,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ClearBody,
                    Slot1 = AbilityId.LiquidOoze,
                    Hidden = AbilityId.RainDish,
                },
                HeightM = 1.6,
                WeightKg = 55,
                Color = "Blue",
            },
            [SpecieId.Geodude] = new()
            {
                Id = SpecieId.Geodude,
                Num = 74,
                Name = "Geodude",
                Types = [PokemonType.Rock, PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 80,
                    Def = 100,
                    SpA = 30,
                    SpD = 30,
                    Spe = 20,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RockHead,
                    Slot1 = AbilityId.Sturdy,
                    Hidden = AbilityId.SandVeil,
                },
                HeightM = 0.4,
                WeightKg = 20,
                Color = "Brown",
            },
            [SpecieId.Geodudealola] = new()
            {
                Id = SpecieId.Geodudealola,
                Num = 74,
                Name = "Geodude-Alola",
                BaseSpecies = SpecieId.Geodude,
                Forme = FormeId.Alola,
                Types = [PokemonType.Rock, PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 80,
                    Def = 100,
                    SpA = 30,
                    SpD = 30,
                    Spe = 20,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.MagnetPull,
                    Slot1 = AbilityId.Sturdy,
                    Hidden = AbilityId.Galvanize,
                },
                HeightM = 0.4,
                WeightKg = 20.3,
                Color = "Gray",
            },
            [SpecieId.Graveler] = new()
            {
                Id = SpecieId.Graveler,
                Num = 75,
                Name = "Graveler",
                Types = [PokemonType.Rock, PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 55,
                    Atk = 95,
                    Def = 115,
                    SpA = 45,
                    SpD = 45,
                    Spe = 35,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RockHead,
                    Slot1 = AbilityId.Sturdy,
                    Hidden = AbilityId.SandVeil,
                },
                HeightM = 1,
                WeightKg = 105,
                Color = "Brown",
            },
            [SpecieId.GravelerAlola] = new()
            {
                Id = SpecieId.GravelerAlola,
                Num = 75,
                Name = "Graveler-Alola",
                BaseSpecies = SpecieId.Graveler,
                Forme = FormeId.Alola,
                Types = [PokemonType.Rock, PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 55,
                    Atk = 95,
                    Def = 115,
                    SpA = 45,
                    SpD = 45,
                    Spe = 35,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.MagnetPull,
                    Slot1 = AbilityId.Sturdy,
                    Hidden = AbilityId.Galvanize,
                },
                HeightM = 1,
                WeightKg = 110,
                Color = "Gray",
            },
            [SpecieId.Golem] = new()
            {
                Id = SpecieId.Golem,
                Num = 76,
                Name = "Golem",
                Types = [PokemonType.Rock, PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 120,
                    Def = 130,
                    SpA = 55,
                    SpD = 65,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RockHead,
                    Slot1 = AbilityId.Sturdy,
                    Hidden = AbilityId.SandVeil,
                },
                HeightM = 1.4,
                WeightKg = 300,
                Color = "Brown",
            },
            [SpecieId.GolemAlola] = new()
            {
                Id = SpecieId.GolemAlola,
                Num = 76,
                Name = "Golem-Alola",
                BaseSpecies = SpecieId.Golem,
                Forme = FormeId.Alola,
                Types = [PokemonType.Rock, PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 120,
                    Def = 130,
                    SpA = 55,
                    SpD = 65,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.MagnetPull,
                    Slot1 = AbilityId.Sturdy,
                    Hidden = AbilityId.Galvanize,
                },
                HeightM = 1.7,
                WeightKg = 316,
                Color = "Gray",
            },
            [SpecieId.Ponyta] = new()
            {
                Id = SpecieId.Ponyta,
                Num = 77,
                Name = "Ponyta",
                Types = [PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 85,
                    Def = 55,
                    SpA = 65,
                    SpD = 65,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RunAway,
                    Slot1 = AbilityId.FlashFire,
                    Hidden = AbilityId.FlameBody,
                },
                HeightM = 1,
                WeightKg = 30,
                Color = "Yellow",
            },
            [SpecieId.PonytaGalar] = new()
            {
                Id = SpecieId.PonytaGalar,
                Num = 77,
                Name = "Ponyta-Galar",
                BaseSpecies = SpecieId.Ponyta,
                Forme = FormeId.Galar,
                Types = [PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 85,
                    Def = 55,
                    SpA = 65,
                    SpD = 65,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RunAway,
                    Slot1 = AbilityId.PastelVeil,
                    Hidden = AbilityId.Anticipation,
                },
                HeightM = 0.8,
                WeightKg = 24,
                Color = "White",
            },
            [SpecieId.Rapidash] = new()
            {
                Id = SpecieId.Rapidash,
                Num = 78,
                Name = "Rapidash",
                Types = [PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 100,
                    Def = 70,
                    SpA = 80,
                    SpD = 80,
                    Spe = 105,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RunAway,
                    Slot1 = AbilityId.FlashFire,
                    Hidden = AbilityId.FlameBody,
                },
                HeightM = 1.7,
                WeightKg = 95,
                Color = "Yellow",
            },
            [SpecieId.RapidashGalar] = new()
            {
                Id = SpecieId.RapidashGalar,
                Num = 78,
                Name = "Rapidash-Galar",
                BaseSpecies = SpecieId.Rapidash,
                Forme = FormeId.Galar,
                Types = [PokemonType.Psychic, PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 100,
                    Def = 70,
                    SpA = 80,
                    SpD = 80,
                    Spe = 105,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RunAway,
                    Slot1 = AbilityId.PastelVeil,
                    Hidden = AbilityId.Anticipation,
                },
                HeightM = 1.7,
                WeightKg = 80,
                Color = "White",
            },
            [SpecieId.Slowpoke] = new()
            {
                Id = SpecieId.Slowpoke,
                Num = 79,
                Name = "Slowpoke",
                Types = [PokemonType.Water, PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 65,
                    Def = 65,
                    SpA = 40,
                    SpD = 40,
                    Spe = 15,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Oblivious,
                    Slot1 = AbilityId.OwnTempo,
                    Hidden = AbilityId.Regenerator,
                },
                HeightM = 1.2,
                WeightKg = 36,
                Color = "Pink",
            },
            [SpecieId.SlowpokeGalar] = new()
            {
                Id = SpecieId.SlowpokeGalar,
                Num = 79,
                Name = "Slowpoke-Galar",
                BaseSpecies = SpecieId.Slowpoke,
                Forme = FormeId.Galar,
                Types = [PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 65,
                    Def = 65,
                    SpA = 40,
                    SpD = 40,
                    Spe = 15,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Gluttony,
                    Slot1 = AbilityId.OwnTempo,
                    Hidden = AbilityId.Regenerator,
                },
                HeightM = 1.2,
                WeightKg = 36,
                Color = "Pink",
            },
            [SpecieId.Slowbro] = new()
            {
                Id = SpecieId.Slowbro,
                Num = 80,
                Name = "Slowbro",
                Types = [PokemonType.Water, PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 75,
                    Def = 110,
                    SpA = 100,
                    SpD = 80,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Oblivious,
                    Slot1 = AbilityId.OwnTempo,
                    Hidden = AbilityId.Regenerator,
                },
                HeightM = 1.6,
                WeightKg = 78.5,
                Color = "Pink",
                OtherFormes = [FormeId.Mega],
            },
            [SpecieId.SlowbroMega] = new()
            {
                Id = SpecieId.SlowbroMega,
                Num = 80,
                Name = "Slowbro-Mega",
                BaseSpecies = SpecieId.Slowbro,
                Forme = FormeId.Mega,
                Types = [PokemonType.Water, PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 75,
                    Def = 180,
                    SpA = 130,
                    SpD = 80,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShellArmor,
                },
                HeightM = 2,
                WeightKg = 120,
                Color = "Pink",
                RequiredItem = ItemId.Slowbronite,
            },
            [SpecieId.SlowbroGalar] = new()
            {
                Id = SpecieId.SlowbroGalar,
                Num = 80,
                Name = "Slowbro-Galar",
                BaseSpecies = SpecieId.Slowbro,
                Forme = FormeId.Galar,
                Types = [PokemonType.Poison, PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 100,
                    Def = 95,
                    SpA = 100,
                    SpD = 70,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.QuickDraw,
                    Slot1 = AbilityId.OwnTempo,
                    Hidden = AbilityId.Regenerator,
                },
                HeightM = 1.6,
                WeightKg = 70.5,
                Color = "Pink",
            },
            [SpecieId.Magnemite] = new()
            {
                Id = SpecieId.Magnemite,
                Num = 81,
                Name = "Magnemite",
                Types = [PokemonType.Electric, PokemonType.Steel],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 25,
                    Atk = 35,
                    Def = 70,
                    SpA = 95,
                    SpD = 55,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.MagnetPull,
                    Slot1 = AbilityId.Sturdy,
                    Hidden = AbilityId.Analytic,
                },
                HeightM = 0.3,
                WeightKg = 6,
                Color = "Gray",
            },
            [SpecieId.Magneton] = new()
            {
                Id = SpecieId.Magneton,
                Num = 82,
                Name = "Magneton",
                Types = [PokemonType.Electric, PokemonType.Steel],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 60,
                    Def = 95,
                    SpA = 120,
                    SpD = 70,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.MagnetPull,
                    Slot1 = AbilityId.Sturdy,
                    Hidden = AbilityId.Analytic,
                },
                HeightM = 1,
                WeightKg = 60,
                Color = "Gray",
            },
            [SpecieId.Farfetchd] = new()
            {
                Id = SpecieId.Farfetchd,
                Num = 83,
                Name = "Farfetch\u2019d",
                Types = [PokemonType.Normal, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 52,
                    Atk = 90,
                    Def = 55,
                    SpA = 58,
                    SpD = 62,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.KeenEye,
                    Slot1 = AbilityId.InnerFocus,
                    Hidden = AbilityId.Defiant,
                },
                HeightM = 0.8,
                WeightKg = 15,
                Color = "Brown",
            },
            [SpecieId.FarfetchdGalar] = new()
            {
                Id = SpecieId.FarfetchdGalar,
                Num = 83,
                Name = "Farfetch\u2019d-Galar",
                BaseSpecies = SpecieId.Farfetchd,
                Forme = FormeId.Galar,
                Types = [PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 52,
                    Atk = 95,
                    Def = 55,
                    SpA = 58,
                    SpD = 62,
                    Spe = 55,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Steadfast,
                    Hidden = AbilityId.Scrappy,
                },
                HeightM = 0.8,
                WeightKg = 42,
                Color = "Brown",
            },
            [SpecieId.Doduo] = new()
            {
                Id = SpecieId.Doduo,
                Num = 84,
                Name = "Doduo",
                Types = [PokemonType.Normal, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 35,
                    Atk = 85,
                    Def = 45,
                    SpA = 35,
                    SpD = 35,
                    Spe = 75,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RunAway,
                    Slot1 = AbilityId.EarlyBird,
                    Hidden = AbilityId.TangledFeet,
                },
                HeightM = 1.4,
                WeightKg = 39.2,
                Color = "Brown",
            },
            [SpecieId.Dodrio] = new()
            {
                Id = SpecieId.Dodrio,
                Num = 85,
                Name = "Dodrio",
                Types = [PokemonType.Normal, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 110,
                    Def = 70,
                    SpA = 60,
                    SpD = 60,
                    Spe = 110,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RunAway,
                    Slot1 = AbilityId.EarlyBird,
                    Hidden = AbilityId.TangledFeet,
                },
                HeightM = 1.8,
                WeightKg = 85.2,
                Color = "Brown",
            },
            [SpecieId.Seel] = new()
            {
                Id = SpecieId.Seel,
                Num = 86,
                Name = "Seel",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 45,
                    Def = 55,
                    SpA = 45,
                    SpD = 70,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ThickFat,
                    Slot1 = AbilityId.Hydration,
                    Hidden = AbilityId.IceBody,
                },
                HeightM = 1.1,
                WeightKg = 90,
                Color = "White",
            },
            [SpecieId.Dewgong] = new()
            {
                Id = SpecieId.Dewgong,
                Num = 87,
                Name = "Dewgong",
                Types = [PokemonType.Water, PokemonType.Ice],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 70,
                    Def = 80,
                    SpA = 70,
                    SpD = 95,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ThickFat,
                    Slot1 = AbilityId.Hydration,
                    Hidden = AbilityId.IceBody,
                },
                HeightM = 1.7,
                WeightKg = 120,
                Color = "White",
            },
            [SpecieId.Grimer] = new()
            {
                Id = SpecieId.Grimer,
                Num = 88,
                Name = "Grimer",
                Types = [PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 80,
                    Def = 50,
                    SpA = 40,
                    SpD = 50,
                    Spe = 25,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Stench,
                    Slot1 = AbilityId.StickyHold,
                    Hidden = AbilityId.PoisonTouch,
                },
                HeightM = 0.9,
                WeightKg = 30,
                Color = "Purple",
            },
            [SpecieId.GrimerAlola] = new()
            {
                Id = SpecieId.GrimerAlola,
                Num = 88,
                Name = "Grimer-Alola",
                BaseSpecies = SpecieId.Grimer,
                Forme = FormeId.Alola,
                Types = [PokemonType.Poison, PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 80,
                    Def = 50,
                    SpA = 40,
                    SpD = 50,
                    Spe = 25,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.PoisonTouch,
                    Slot1 = AbilityId.Gluttony,
                    Hidden = AbilityId.PowerOfAlchemy,
                },
                HeightM = 0.7,
                WeightKg = 42,
                Color = "Green",
            },
            [SpecieId.Muk] = new()
            {
                Id = SpecieId.Muk,
                Num = 89,
                Name = "Muk",
                Types = [PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 105,
                    Atk = 105,
                    Def = 75,
                    SpA = 65,
                    SpD = 100,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Stench,
                    Slot1 = AbilityId.StickyHold,
                    Hidden = AbilityId.PoisonTouch,
                },
                HeightM = 1.2,
                WeightKg = 30,
                Color = "Purple",
            },
            [SpecieId.MukAlola] = new()
            {
                Id = SpecieId.MukAlola,
                Num = 89,
                Name = "Muk-Alola",
                BaseSpecies = SpecieId.Muk,
                Forme = FormeId.Alola,
                Types = [PokemonType.Poison, PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 105,
                    Atk = 105,
                    Def = 75,
                    SpA = 65,
                    SpD = 100,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.PoisonTouch,
                    Slot1 = AbilityId.Gluttony,
                    Hidden = AbilityId.PowerOfAlchemy,
                },
                HeightM = 1,
                WeightKg = 52,
                Color = "Green",
            },
            [SpecieId.Shellder] = new()
            {
                Id = SpecieId.Shellder,
                Num = 90,
                Name = "Shellder",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 30,
                    Atk = 65,
                    Def = 100,
                    SpA = 45,
                    SpD = 25,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShellArmor,
                    Slot1 = AbilityId.SkillLink,
                    Hidden = AbilityId.Overcoat,
                },
                HeightM = 0.3,
                WeightKg = 4,
                Color = "Purple",
            },
            [SpecieId.Cloyster] = new()
            {
                Id = SpecieId.Cloyster,
                Num = 91,
                Name = "Cloyster",
                Types = [PokemonType.Water, PokemonType.Ice],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 95,
                    Def = 180,
                    SpA = 85,
                    SpD = 45,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShellArmor,
                    Slot1 = AbilityId.SkillLink,
                    Hidden = AbilityId.Overcoat,
                },
                HeightM = 1.5,
                WeightKg = 132.5,
                Color = "Purple",
            },
            [SpecieId.Gastly] = new()
            {
                Id = SpecieId.Gastly,
                Num = 92,
                Name = "Gastly",
                Types = [PokemonType.Ghost, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 30,
                    Atk = 35,
                    Def = 30,
                    SpA = 100,
                    SpD = 35,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                },
                HeightM = 1.3,
                WeightKg = 0.1,
                Color = "Purple",
            },
            [SpecieId.Haunter] = new()
            {
                Id = SpecieId.Haunter,
                Num = 93,
                Name = "Haunter",
                Types = [PokemonType.Ghost, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 50,
                    Def = 45,
                    SpA = 115,
                    SpD = 55,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                },
                HeightM = 1.6,
                WeightKg = 0.1,
                Color = "Purple",
            },
            [SpecieId.Gengar] = new()
            {
                Id = SpecieId.Gengar,
                Num = 94,
                Name = "Gengar",
                Types = [PokemonType.Ghost, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 65,
                    Def = 60,
                    SpA = 130,
                    SpD = 75,
                    Spe = 110,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.CursedBody,
                },
                HeightM = 1.5,
                WeightKg = 40.5,
                Color = "Purple",
                OtherFormes = [FormeId.Mega],
            },
            [SpecieId.GengarMega] = new()
            {
                Id = SpecieId.GengarMega,
                Num = 94,
                Name = "Gengar-Mega",
                BaseSpecies = SpecieId.Gengar,
                Forme = FormeId.Mega,
                Types = [PokemonType.Ghost, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 65,
                    Def = 80,
                    SpA = 170,
                    SpD = 95,
                    Spe = 130,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShadowTag,
                },
                HeightM = 1.4,
                WeightKg = 40.5,
                Color = "Purple",
                RequiredItem = ItemId.Gengarite,
            },
            [SpecieId.Onix] = new()
            {
                Id = SpecieId.Onix,
                Num = 95,
                Name = "Onix",
                Types = [PokemonType.Rock, PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 35,
                    Atk = 45,
                    Def = 160,
                    SpA = 30,
                    SpD = 45,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RockHead,
                    Slot1 = AbilityId.Sturdy,
                    Hidden = AbilityId.WeakArmor,
                },
                HeightM = 8.8,
                WeightKg = 210,
                Color = "Gray",
            },
            [SpecieId.Drowzee] = new()
            {
                Id = SpecieId.Drowzee,
                Num = 96,
                Name = "Drowzee",
                Types = [PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 48,
                    Def = 45,
                    SpA = 43,
                    SpD = 90,
                    Spe = 42,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Insomnia,
                    Slot1 = AbilityId.Forewarn,
                    Hidden = AbilityId.InnerFocus,
                },
                HeightM = 1,
                WeightKg = 32.4,
                Color = "Yellow",
            },
            [SpecieId.Hypno] = new()
            {
                Id = SpecieId.Hypno,
                Num = 97,
                Name = "Hypno",
                Types = [PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 85,
                    Atk = 73,
                    Def = 70,
                    SpA = 73,
                    SpD = 115,
                    Spe = 67,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Insomnia,
                    Slot1 = AbilityId.Forewarn,
                    Hidden = AbilityId.InnerFocus,
                },
                HeightM = 1.6,
                WeightKg = 75.6,
                Color = "Yellow",
            },
            [SpecieId.Krabby] = new()
            {
                Id = SpecieId.Krabby,
                Num = 98,
                Name = "Krabby",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 30,
                    Atk = 105,
                    Def = 90,
                    SpA = 25,
                    SpD = 25,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.HyperCutter,
                    Slot1 = AbilityId.ShellArmor,
                    Hidden = AbilityId.SheerForce,
                },
                HeightM = 0.4,
                WeightKg = 6.5,
                Color = "Red",
            },
            [SpecieId.Kingler] = new()
            {
                Id = SpecieId.Kingler,
                Num = 99,
                Name = "Kingler",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 55,
                    Atk = 130,
                    Def = 115,
                    SpA = 50,
                    SpD = 50,
                    Spe = 75,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.HyperCutter,
                    Slot1 = AbilityId.ShellArmor,
                    Hidden = AbilityId.SheerForce,
                },
                HeightM = 1.3,
                WeightKg = 60,
                Color = "Red",
            },
            [SpecieId.Voltorb] = new()
            {
                Id = SpecieId.Voltorb,
                Num = 100,
                Name = "Voltorb",
                Types = [PokemonType.Electric],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 30,
                    Def = 50,
                    SpA = 55,
                    SpD = 55,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Soundproof,
                    Slot1 = AbilityId.Static,
                    Hidden = AbilityId.Aftermath,
                },
                HeightM = 0.5,
                WeightKg = 10.4,
                Color = "Red",
            },
            [SpecieId.VoltorbHisui] = new()
            {
                Id = SpecieId.VoltorbHisui,
                Num = 100,
                Name = "Voltorb-Hisui",
                BaseSpecies = SpecieId.Voltorb,
                Forme = FormeId.Hisui,
                Types = [PokemonType.Electric, PokemonType.Grass],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 30,
                    Def = 50,
                    SpA = 55,
                    SpD = 55,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Soundproof,
                    Slot1 = AbilityId.Static,
                    Hidden = AbilityId.Aftermath,
                },
                HeightM = 0.5,
                WeightKg = 13,
                Color = "Red",
            },
        };
    }
}