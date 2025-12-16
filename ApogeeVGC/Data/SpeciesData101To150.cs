using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData101To150()
    {
        return new Dictionary<SpecieId, Species>
        {
            [SpecieId.Electrode] = new()
            {
                Id = SpecieId.Electrode,
                Num = 101,
                Name = "Electrode",
                Types = [PokemonType.Electric],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 50,
                    Def = 70,
                    SpA = 80,
                    SpD = 80,
                    Spe = 150,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Soundproof,
                    Slot1 = AbilityId.Static,
                    Hidden = AbilityId.Aftermath,
                },
                HeightM = 1.2,
                WeightKg = 66.6,
                Color = "Red",
            },
            [SpecieId.ElectrodeHisui] = new()
            {
                Id = SpecieId.ElectrodeHisui,
                Num = 101,
                Name = "Electrode-Hisui",
                BaseSpecies = SpecieId.Electrode,
                Forme = FormeId.Hisui,
                Types = [PokemonType.Electric, PokemonType.Grass],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 50,
                    Def = 70,
                    SpA = 80,
                    SpD = 80,
                    Spe = 150,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Soundproof,
                    Slot1 = AbilityId.Static,
                    Hidden = AbilityId.Aftermath,
                },
                HeightM = 1.2,
                WeightKg = 71,
                Color = "Red",
            },
            [SpecieId.Exeggcute] = new()
            {
                Id = SpecieId.Exeggcute,
                Num = 102,
                Name = "Exeggcute",
                Types = [PokemonType.Grass, PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 40,
                    Def = 80,
                    SpA = 60,
                    SpD = 45,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Chlorophyll,
                    Hidden = AbilityId.Harvest,
                },
                HeightM = 0.4,
                WeightKg = 2.5,
                Color = "Pink",
            },
            [SpecieId.Exeggutor] = new()
            {
                Id = SpecieId.Exeggutor,
                Num = 103,
                Name = "Exeggutor",
                Types = [PokemonType.Grass, PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 95,
                    Def = 85,
                    SpA = 125,
                    SpD = 75,
                    Spe = 55,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Chlorophyll,
                    Hidden = AbilityId.Harvest,
                },
                HeightM = 2,
                WeightKg = 120,
                Color = "Yellow",
            },
            [SpecieId.ExeggutorAlola] = new()
            {
                Id = SpecieId.ExeggutorAlola,
                Num = 103,
                Name = "Exeggutor-Alola",
                BaseSpecies = SpecieId.Exeggutor,
                Forme = FormeId.Alola,
                Types = [PokemonType.Grass, PokemonType.Dragon],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 105,
                    Def = 85,
                    SpA = 125,
                    SpD = 75,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Frisk,
                    Hidden = AbilityId.Harvest,
                },
                HeightM = 10.9,
                WeightKg = 415.6,
                Color = "Yellow",
            },
            [SpecieId.Cubone] = new()
            {
                Id = SpecieId.Cubone,
                Num = 104,
                Name = "Cubone",
                Types = [PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 50,
                    Def = 95,
                    SpA = 40,
                    SpD = 50,
                    Spe = 35,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RockHead,
                    Slot1 = AbilityId.LightningRod,
                    Hidden = AbilityId.BattleArmor,
                },
                HeightM = 0.4,
                WeightKg = 6.5,
                Color = "Brown",
            },
            [SpecieId.Marowak] = new()
            {
                Id = SpecieId.Marowak,
                Num = 105,
                Name = "Marowak",
                Types = [PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 80,
                    Def = 110,
                    SpA = 50,
                    SpD = 80,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RockHead,
                    Slot1 = AbilityId.LightningRod,
                    Hidden = AbilityId.BattleArmor,
                },
                HeightM = 1,
                WeightKg = 45,
                Color = "Brown",
            },
            [SpecieId.MarowakAlola] = new()
            {
                Id = SpecieId.MarowakAlola,
                Num = 105,
                Name = "Marowak-Alola",
                BaseSpecies = SpecieId.Marowak,
                Forme = FormeId.Alola,
                Types = [PokemonType.Fire, PokemonType.Ghost],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 80,
                    Def = 110,
                    SpA = 50,
                    SpD = 80,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.CursedBody,
                    Slot1 = AbilityId.LightningRod,
                    Hidden = AbilityId.RockHead,
                },
                HeightM = 1,
                WeightKg = 34,
                Color = "Purple",
            },
            [SpecieId.MarowakAlolaTotem] = new()
            {
                Id = SpecieId.MarowakAlolaTotem,
                Num = 105,
                Name = "Marowak-Alola-Totem",
                BaseSpecies = SpecieId.Marowak,
                Forme = FormeId.AlolaTotem,
                Types = [PokemonType.Fire, PokemonType.Ghost],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 80,
                    Def = 110,
                    SpA = 50,
                    SpD = 80,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RockHead,
                },
                HeightM = 1.7,
                WeightKg = 98,
                Color = "Purple",
            },
            [SpecieId.Hitmonlee] = new()
            {
                Id = SpecieId.Hitmonlee,
                Num = 106,
                Name = "Hitmonlee",
                Types = [PokemonType.Fighting],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 120,
                    Def = 53,
                    SpA = 35,
                    SpD = 110,
                    Spe = 87,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Limber,
                    Slot1 = AbilityId.Reckless,
                    Hidden = AbilityId.Unnerve,
                },
                HeightM = 1.5,
                WeightKg = 49.8,
                Color = "Brown",
            },
            [SpecieId.Hitmonchan] = new()
            {
                Id = SpecieId.Hitmonchan,
                Num = 107,
                Name = "Hitmonchan",
                Types = [PokemonType.Fighting],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 105,
                    Def = 79,
                    SpA = 35,
                    SpD = 110,
                    Spe = 76,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.KeenEye,
                    Slot1 = AbilityId.HyperCutter,
                    Hidden = AbilityId.InnerFocus,
                },
                HeightM = 1.4,
                WeightKg = 50.2,
                Color = "Brown",
            },
            [SpecieId.Lickitung] = new()
            {
                Id = SpecieId.Lickitung,
                Num = 108,
                Name = "Lickitung",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 55,
                    Def = 75,
                    SpA = 60,
                    SpD = 75,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.OwnTempo,
                    Slot1 = AbilityId.Oblivious,
                    Hidden = AbilityId.CloudNine,
                },
                HeightM = 1.2,
                WeightKg = 65.5,
                Color = "Pink",
            },
            [SpecieId.Koffing] = new()
            {
                Id = SpecieId.Koffing,
                Num = 109,
                Name = "Koffing",
                Types = [PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 65,
                    Def = 95,
                    SpA = 60,
                    SpD = 45,
                    Spe = 35,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                    Slot1 = AbilityId.NeutralizingGas,
                    Hidden = AbilityId.Stench,
                },
                HeightM = 0.6,
                WeightKg = 1,
                Color = "Purple",
            },
            [SpecieId.Weezing] = new()
            {
                Id = SpecieId.Weezing,
                Num = 110,
                Name = "Weezing",
                Types = [PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 90,
                    Def = 120,
                    SpA = 85,
                    SpD = 70,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                    Slot1 = AbilityId.NeutralizingGas,
                    Hidden = AbilityId.Stench,
                },
                HeightM = 1.2,
                WeightKg = 9.5,
                Color = "Purple",
            },
            [SpecieId.WeezingGalar] = new()
            {
                Id = SpecieId.WeezingGalar,
                Num = 110,
                Name = "Weezing-Galar",
                BaseSpecies = SpecieId.Weezing,
                Forme = FormeId.Galar,
                Types = [PokemonType.Poison, PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 90,
                    Def = 120,
                    SpA = 85,
                    SpD = 70,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                    Slot1 = AbilityId.NeutralizingGas,
                    Hidden = AbilityId.MistySurge,
                },
                HeightM = 3,
                WeightKg = 16,
                Color = "Gray",
            },
            [SpecieId.Rhyhorn] = new()
            {
                Id = SpecieId.Rhyhorn,
                Num = 111,
                Name = "Rhyhorn",
                Types = [PokemonType.Ground, PokemonType.Rock],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 85,
                    Def = 95,
                    SpA = 30,
                    SpD = 30,
                    Spe = 25,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.LightningRod,
                    Slot1 = AbilityId.RockHead,
                    Hidden = AbilityId.Reckless,
                },
                HeightM = 1,
                WeightKg = 115,
                Color = "Gray",
            },
            [SpecieId.Rhydon] = new()
            {
                Id = SpecieId.Rhydon,
                Num = 112,
                Name = "Rhydon",
                Types = [PokemonType.Ground, PokemonType.Rock],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 105,
                    Atk = 130,
                    Def = 120,
                    SpA = 45,
                    SpD = 45,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.LightningRod,
                    Slot1 = AbilityId.RockHead,
                    Hidden = AbilityId.Reckless,
                },
                HeightM = 1.9,
                WeightKg = 120,
                Color = "Gray",
            },
            [SpecieId.Chansey] = new()
            {
                Id = SpecieId.Chansey,
                Num = 113,
                Name = "Chansey",
                Types = [PokemonType.Normal],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 250,
                    Atk = 5,
                    Def = 5,
                    SpA = 35,
                    SpD = 105,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.NaturalCure,
                    Slot1 = AbilityId.SereneGrace,
                    Hidden = AbilityId.Healer,
                },
                HeightM = 1.1,
                WeightKg = 34.6,
                Color = "Pink",
            },
            [SpecieId.Tangela] = new()
            {
                Id = SpecieId.Tangela,
                Num = 114,
                Name = "Tangela",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 55,
                    Def = 115,
                    SpA = 100,
                    SpD = 40,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Chlorophyll,
                    Slot1 = AbilityId.LeafGuard,
                    Hidden = AbilityId.Regenerator,
                },
                HeightM = 1,
                WeightKg = 35,
                Color = "Blue",
            },
            [SpecieId.Kangaskhan] = new()
            {
                Id = SpecieId.Kangaskhan,
                Num = 115,
                Name = "Kangaskhan",
                Types = [PokemonType.Normal],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 105,
                    Atk = 95,
                    Def = 80,
                    SpA = 40,
                    SpD = 80,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.EarlyBird,
                    Slot1 = AbilityId.Scrappy,
                    Hidden = AbilityId.InnerFocus,
                },
                HeightM = 2.2,
                WeightKg = 80,
                Color = "Brown",
            },
            [SpecieId.KangaskhanMega] = new()
            {
                Id = SpecieId.KangaskhanMega,
                Num = 115,
                Name = "Kangaskhan-Mega",
                BaseSpecies = SpecieId.Kangaskhan,
                Forme = FormeId.Mega,
                Types = [PokemonType.Normal],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 105,
                    Atk = 125,
                    Def = 100,
                    SpA = 60,
                    SpD = 100,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ParentalBond,
                },
                HeightM = 2.2,
                WeightKg = 100,
                Color = "Brown",
            },
            [SpecieId.Horsea] = new()
            {
                Id = SpecieId.Horsea,
                Num = 116,
                Name = "Horsea",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 30,
                    Atk = 40,
                    Def = 70,
                    SpA = 70,
                    SpD = 25,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SwiftSwim,
                    Slot1 = AbilityId.Sniper,
                    Hidden = AbilityId.Damp,
                },
                HeightM = 0.4,
                WeightKg = 8,
                Color = "Blue",
            },
            [SpecieId.Seadra] = new()
            {
                Id = SpecieId.Seadra,
                Num = 117,
                Name = "Seadra",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 55,
                    Atk = 65,
                    Def = 95,
                    SpA = 95,
                    SpD = 45,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.PoisonPoint,
                    Slot1 = AbilityId.Sniper,
                    Hidden = AbilityId.Damp,
                },
                HeightM = 1.2,
                WeightKg = 25,
                Color = "Blue",
            },
            [SpecieId.Goldeen] = new()
            {
                Id = SpecieId.Goldeen,
                Num = 118,
                Name = "Goldeen",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 67,
                    Def = 60,
                    SpA = 35,
                    SpD = 50,
                    Spe = 63,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SwiftSwim,
                    Slot1 = AbilityId.WaterVeil,
                    Hidden = AbilityId.LightningRod,
                },
                HeightM = 0.6,
                WeightKg = 15,
                Color = "Red",
            },
            [SpecieId.Seaking] = new()
            {
                Id = SpecieId.Seaking,
                Num = 119,
                Name = "Seaking",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 92,
                    Def = 65,
                    SpA = 65,
                    SpD = 80,
                    Spe = 68,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SwiftSwim,
                    Slot1 = AbilityId.WaterVeil,
                    Hidden = AbilityId.LightningRod,
                },
                HeightM = 1.3,
                WeightKg = 39,
                Color = "Red",
            },
            [SpecieId.Staryu] = new()
            {
                Id = SpecieId.Staryu,
                Num = 120,
                Name = "Staryu",
                Types = [PokemonType.Water],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 30,
                    Atk = 45,
                    Def = 55,
                    SpA = 70,
                    SpD = 55,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Illuminate,
                    Slot1 = AbilityId.NaturalCure,
                    Hidden = AbilityId.Analytic,
                },
                HeightM = 0.8,
                WeightKg = 34.5,
                Color = "Brown",
            },
            [SpecieId.Starmie] = new()
            {
                Id = SpecieId.Starmie,
                Num = 121,
                Name = "Starmie",
                Types = [PokemonType.Water, PokemonType.Psychic],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 75,
                    Def = 85,
                    SpA = 100,
                    SpD = 85,
                    Spe = 115,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Illuminate,
                    Slot1 = AbilityId.NaturalCure,
                    Hidden = AbilityId.Analytic,
                },
                HeightM = 1.1,
                WeightKg = 80,
                Color = "Purple",
            },
            [SpecieId.StarmieMega] = new()
            {
                Id = SpecieId.StarmieMega,
                Num = 121,
                Name = "Starmie-Mega",
                BaseSpecies = SpecieId.Starmie,
                Forme = FormeId.Mega,
                Types = [PokemonType.Water, PokemonType.Psychic],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 140,
                    Def = 105,
                    SpA = 130,
                    SpD = 105,
                    Spe = 120,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Illuminate,
                    Slot1 = AbilityId.NaturalCure,
                    Hidden = AbilityId.Analytic,
                },
                HeightM = 2.3,
                WeightKg = 80,
                Color = "Purple",
            },
            [SpecieId.MrMime] = new()
            {
                Id = SpecieId.MrMime,
                Num = 122,
                Name = "Mr. Mime",
                Types = [PokemonType.Psychic, PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 45,
                    Def = 65,
                    SpA = 100,
                    SpD = 120,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Soundproof,
                    Slot1 = AbilityId.Filter,
                    Hidden = AbilityId.Technician,
                },
                HeightM = 1.3,
                WeightKg = 54.5,
                Color = "Pink",
            },
            [SpecieId.MrMimeGalar] = new()
            {
                Id = SpecieId.MrMimeGalar,
                Num = 122,
                Name = "Mr. Mime-Galar",
                BaseSpecies = SpecieId.MrMime,
                Forme = FormeId.Galar,
                Types = [PokemonType.Ice, PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 65,
                    Def = 65,
                    SpA = 90,
                    SpD = 90,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.VitalSpirit,
                    Slot1 = AbilityId.ScreenCleaner,
                    Hidden = AbilityId.IceBody,
                },
                HeightM = 1.4,
                WeightKg = 56.8,
                Color = "White",
            },
            [SpecieId.Scyther] = new()
            {
                Id = SpecieId.Scyther,
                Num = 123,
                Name = "Scyther",
                Types = [PokemonType.Bug, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 110,
                    Def = 80,
                    SpA = 55,
                    SpD = 80,
                    Spe = 105,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Swarm,
                    Slot1 = AbilityId.Technician,
                    Hidden = AbilityId.Steadfast,
                },
                HeightM = 1.5,
                WeightKg = 56,
                Color = "Green",
            },
            [SpecieId.Jynx] = new()
            {
                Id = SpecieId.Jynx,
                Num = 124,
                Name = "Jynx",
                Types = [PokemonType.Ice, PokemonType.Psychic],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 50,
                    Def = 35,
                    SpA = 115,
                    SpD = 95,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Oblivious,
                    Slot1 = AbilityId.Forewarn,
                    Hidden = AbilityId.DrySkin,
                },
                HeightM = 1.4,
                WeightKg = 40.6,
                Color = "Red",
            },
            [SpecieId.Electabuzz] = new()
            {
                Id = SpecieId.Electabuzz,
                Num = 125,
                Name = "Electabuzz",
                Types = [PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 83,
                    Def = 57,
                    SpA = 95,
                    SpD = 85,
                    Spe = 105,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Static,
                    Hidden = AbilityId.VitalSpirit,
                },
                HeightM = 1.1,
                WeightKg = 30,
                Color = "Yellow",
            },
            [SpecieId.Magmar] = new()
            {
                Id = SpecieId.Magmar,
                Num = 126,
                Name = "Magmar",
                Types = [PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 95,
                    Def = 57,
                    SpA = 100,
                    SpD = 85,
                    Spe = 93,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.FlameBody,
                    Hidden = AbilityId.VitalSpirit,
                },
                HeightM = 1.3,
                WeightKg = 44.5,
                Color = "Red",
            },
            [SpecieId.Pinsir] = new()
            {
                Id = SpecieId.Pinsir,
                Num = 127,
                Name = "Pinsir",
                Types = [PokemonType.Bug],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 125,
                    Def = 100,
                    SpA = 55,
                    SpD = 70,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.HyperCutter,
                    Slot1 = AbilityId.MoldBreaker,
                    Hidden = AbilityId.Moxie,
                },
                HeightM = 1.5,
                WeightKg = 55,
                Color = "Brown",
            },
            [SpecieId.PinsirMega] = new()
            {
                Id = SpecieId.PinsirMega,
                Num = 127,
                Name = "Pinsir-Mega",
                BaseSpecies = SpecieId.Pinsir,
                Forme = FormeId.Mega,
                Types = [PokemonType.Bug, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 155,
                    Def = 120,
                    SpA = 65,
                    SpD = 90,
                    Spe = 105,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Aerilate,
                },
                HeightM = 1.7,
                WeightKg = 59,
                Color = "Brown",
            },
            [SpecieId.Tauros] = new()
            {
                Id = SpecieId.Tauros,
                Num = 128,
                Name = "Tauros",
                Types = [PokemonType.Normal],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 100,
                    Def = 95,
                    SpA = 40,
                    SpD = 70,
                    Spe = 110,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Intimidate,
                    Slot1 = AbilityId.AngerPoint,
                    Hidden = AbilityId.SheerForce,
                },
                HeightM = 1.4,
                WeightKg = 88.4,
                Color = "Brown",
            },
            [SpecieId.TaurosPaldeaCombat] = new()
            {
                Id = SpecieId.TaurosPaldeaCombat,
                Num = 128,
                Name = "Tauros-Paldea-Combat",
                BaseSpecies = SpecieId.Tauros,
                Forme = FormeId.PaldeaCombat,
                Types = [PokemonType.Fighting],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 110,
                    Def = 105,
                    SpA = 30,
                    SpD = 70,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Intimidate,
                    Slot1 = AbilityId.AngerPoint,
                    Hidden = AbilityId.CudChew,
                },
                HeightM = 1.4,
                WeightKg = 115,
                Color = "Black",
            },
            [SpecieId.TaurosPaldeaBlaze] = new()
            {
                Id = SpecieId.TaurosPaldeaBlaze,
                Num = 128,
                Name = "Tauros-Paldea-Blaze",
                BaseSpecies = SpecieId.Tauros,
                Forme = FormeId.PaldeaBlaze,
                Types = [PokemonType.Fighting, PokemonType.Fire],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 110,
                    Def = 105,
                    SpA = 30,
                    SpD = 70,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Intimidate,
                    Slot1 = AbilityId.AngerPoint,
                    Hidden = AbilityId.CudChew,
                },
                HeightM = 1.4,
                WeightKg = 85,
                Color = "Black",
            },
            [SpecieId.TaurosPaldeaAqua] = new()
            {
                Id = SpecieId.TaurosPaldeaAqua,
                Num = 128,
                Name = "Tauros-Paldea-Aqua",
                BaseSpecies = SpecieId.Tauros,
                Forme = FormeId.PaldeaAqua,
                Types = [PokemonType.Fighting, PokemonType.Water],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 110,
                    Def = 105,
                    SpA = 30,
                    SpD = 70,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Intimidate,
                    Slot1 = AbilityId.AngerPoint,
                    Hidden = AbilityId.CudChew,
                },
                HeightM = 1.4,
                WeightKg = 110,
                Color = "Black",
            },
            [SpecieId.Magikarp] = new()
            {
                Id = SpecieId.Magikarp,
                Num = 129,
                Name = "Magikarp",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 20,
                    Atk = 10,
                    Def = 55,
                    SpA = 15,
                    SpD = 20,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SwiftSwim,
                    Hidden = AbilityId.Rattled,
                },
                HeightM = 0.9,
                WeightKg = 10,
                Color = "Red",
            },
            [SpecieId.Gyarados] = new()
            {
                Id = SpecieId.Gyarados,
                Num = 130,
                Name = "Gyarados",
                Types = [PokemonType.Water, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 125,
                    Def = 79,
                    SpA = 60,
                    SpD = 100,
                    Spe = 81,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Intimidate,
                    Hidden = AbilityId.Moxie,
                },
                HeightM = 6.5,
                WeightKg = 235,
                Color = "Blue",
            },
            [SpecieId.GyaradosMega] = new()
            {
                Id = SpecieId.GyaradosMega,
                Num = 130,
                Name = "Gyarados-Mega",
                BaseSpecies = SpecieId.Gyarados,
                Forme = FormeId.Mega,
                Types = [PokemonType.Water, PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 155,
                    Def = 109,
                    SpA = 70,
                    SpD = 130,
                    Spe = 81,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.MoldBreaker,
                },
                HeightM = 6.5,
                WeightKg = 305,
                Color = "Blue",
            },
            [SpecieId.Lapras] = new()
            {
                Id = SpecieId.Lapras,
                Num = 131,
                Name = "Lapras",
                Types = [PokemonType.Water, PokemonType.Ice],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 130,
                    Atk = 85,
                    Def = 80,
                    SpA = 85,
                    SpD = 95,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.WaterAbsorb,
                    Slot1 = AbilityId.ShellArmor,
                    Hidden = AbilityId.Hydration,
                },
                HeightM = 2.5,
                WeightKg = 220,
                Color = "Blue",
            },
            [SpecieId.LaprasGmax] = new()
            {
                Id = SpecieId.LaprasGmax,
                Num = 131,
                Name = "Lapras-Gmax",
                BaseSpecies = SpecieId.Lapras,
                Forme = FormeId.Gmax,
                Types = [PokemonType.Water, PokemonType.Ice],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 130,
                    Atk = 85,
                    Def = 80,
                    SpA = 85,
                    SpD = 95,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.WaterAbsorb,
                    Slot1 = AbilityId.ShellArmor,
                    Hidden = AbilityId.Hydration,
                },
                HeightM = 24,
                WeightKg = 0,
                Color = "Blue",
            },
            [SpecieId.Ditto] = new()
            {
                Id = SpecieId.Ditto,
                Num = 132,
                Name = "Ditto",
                Types = [PokemonType.Normal],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 48,
                    Atk = 48,
                    Def = 48,
                    SpA = 48,
                    SpD = 48,
                    Spe = 48,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Limber,
                    Hidden = AbilityId.Imposter,
                },
                HeightM = 0.3,
                WeightKg = 4,
                Color = "Purple",
            },
            [SpecieId.Eevee] = new()
            {
                Id = SpecieId.Eevee,
                Num = 133,
                Name = "Eevee",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 55,
                    Atk = 55,
                    Def = 50,
                    SpA = 45,
                    SpD = 65,
                    Spe = 55,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RunAway,
                    Slot1 = AbilityId.Adaptability,
                    Hidden = AbilityId.Anticipation,
                },
                HeightM = 0.3,
                WeightKg = 6.5,
                Color = "Brown",
            },
            [SpecieId.EeveeStarter] = new()
            {
                Id = SpecieId.EeveeStarter,
                Num = 133,
                Name = "Eevee-Starter",
                BaseSpecies = SpecieId.Eevee,
                Forme = FormeId.Starter,
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 75,
                    Def = 70,
                    SpA = 65,
                    SpD = 85,
                    Spe = 75,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RunAway,
                    Slot1 = AbilityId.Adaptability,
                    Hidden = AbilityId.Anticipation,
                },
                HeightM = 0.3,
                WeightKg = 6.5,
                Color = "Brown",
            },
            [SpecieId.EeveeGmax] = new()
            {
                Id = SpecieId.EeveeGmax,
                Num = 133,
                Name = "Eevee-Gmax",
                BaseSpecies = SpecieId.Eevee,
                Forme = FormeId.Gmax,
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 55,
                    Atk = 55,
                    Def = 50,
                    SpA = 45,
                    SpD = 65,
                    Spe = 55,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RunAway,
                    Slot1 = AbilityId.Adaptability,
                    Hidden = AbilityId.Anticipation,
                },
                HeightM = 18,
                WeightKg = 0,
                Color = "Brown",
            },
            [SpecieId.Vaporeon] = new()
            {
                Id = SpecieId.Vaporeon,
                Num = 134,
                Name = "Vaporeon",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 130,
                    Atk = 65,
                    Def = 60,
                    SpA = 110,
                    SpD = 95,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.WaterAbsorb,
                    Hidden = AbilityId.Hydration,
                },
                HeightM = 1,
                WeightKg = 29,
                Color = "Blue",
            },
            [SpecieId.Jolteon] = new()
            {
                Id = SpecieId.Jolteon,
                Num = 135,
                Name = "Jolteon",
                Types = [PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 65,
                    Def = 60,
                    SpA = 110,
                    SpD = 95,
                    Spe = 130,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.VoltAbsorb,
                    Hidden = AbilityId.QuickFeet,
                },
                HeightM = 0.8,
                WeightKg = 24.5,
                Color = "Yellow",
            },
            [SpecieId.Flareon] = new()
            {
                Id = SpecieId.Flareon,
                Num = 136,
                Name = "Flareon",
                Types = [PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 130,
                    Def = 60,
                    SpA = 95,
                    SpD = 110,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.FlashFire,
                    Hidden = AbilityId.Guts,
                },
                HeightM = 0.9,
                WeightKg = 25,
                Color = "Red",
            },
            [SpecieId.Porygon] = new()
            {
                Id = SpecieId.Porygon,
                Num = 137,
                Name = "Porygon",
                Types = [PokemonType.Normal],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 60,
                    Def = 70,
                    SpA = 85,
                    SpD = 75,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Trace,
                    Slot1 = AbilityId.Download,
                    Hidden = AbilityId.Analytic,
                },
                HeightM = 0.8,
                WeightKg = 36.5,
                Color = "Pink",
            },
            [SpecieId.Omanyte] = new()
            {
                Id = SpecieId.Omanyte,
                Num = 138,
                Name = "Omanyte",
                Types = [PokemonType.Rock, PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 35,
                    Atk = 40,
                    Def = 100,
                    SpA = 90,
                    SpD = 55,
                    Spe = 35,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SwiftSwim,
                    Slot1 = AbilityId.ShellArmor,
                    Hidden = AbilityId.WeakArmor,
                },
                HeightM = 0.4,
                WeightKg = 7.5,
                Color = "Blue",
            },
            [SpecieId.Omastar] = new()
            {
                Id = SpecieId.Omastar,
                Num = 139,
                Name = "Omastar",
                Types = [PokemonType.Rock, PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 60,
                    Def = 125,
                    SpA = 115,
                    SpD = 70,
                    Spe = 55,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SwiftSwim,
                    Slot1 = AbilityId.ShellArmor,
                    Hidden = AbilityId.WeakArmor,
                },
                HeightM = 1,
                WeightKg = 35,
                Color = "Blue",
            },
            [SpecieId.Kabuto] = new()
            {
                Id = SpecieId.Kabuto,
                Num = 140,
                Name = "Kabuto",
                Types = [PokemonType.Rock, PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 30,
                    Atk = 80,
                    Def = 90,
                    SpA = 55,
                    SpD = 45,
                    Spe = 55,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SwiftSwim,
                    Slot1 = AbilityId.BattleArmor,
                    Hidden = AbilityId.WeakArmor,
                },
                HeightM = 0.5,
                WeightKg = 11.5,
                Color = "Brown",
            },
            [SpecieId.Kabutops] = new()
            {
                Id = SpecieId.Kabutops,
                Num = 141,
                Name = "Kabutops",
                Types = [PokemonType.Rock, PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 115,
                    Def = 105,
                    SpA = 65,
                    SpD = 70,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SwiftSwim,
                    Slot1 = AbilityId.BattleArmor,
                    Hidden = AbilityId.WeakArmor,
                },
                HeightM = 1.3,
                WeightKg = 40.5,
                Color = "Brown",
            },
            [SpecieId.Aerodactyl] = new()
            {
                Id = SpecieId.Aerodactyl,
                Num = 142,
                Name = "Aerodactyl",
                Types = [PokemonType.Rock, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 105,
                    Def = 65,
                    SpA = 60,
                    SpD = 75,
                    Spe = 130,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RockHead,
                    Slot1 = AbilityId.Pressure,
                    Hidden = AbilityId.Unnerve,
                },
                HeightM = 1.8,
                WeightKg = 59,
                Color = "Purple",
            },
            [SpecieId.AerodactylMega] = new()
            {
                Id = SpecieId.AerodactylMega,
                Num = 142,
                Name = "Aerodactyl-Mega",
                BaseSpecies = SpecieId.Aerodactyl,
                Forme = FormeId.Mega,
                Types = [PokemonType.Rock, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 135,
                    Def = 85,
                    SpA = 70,
                    SpD = 95,
                    Spe = 150,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ToughClaws,
                },
                HeightM = 2.1,
                WeightKg = 79,
                Color = "Purple",
            },
            [SpecieId.Snorlax] = new()
            {
                Id = SpecieId.Snorlax,
                Num = 143,
                Name = "Snorlax",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 160,
                    Atk = 110,
                    Def = 65,
                    SpA = 65,
                    SpD = 110,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Immunity,
                    Slot1 = AbilityId.ThickFat,
                    Hidden = AbilityId.Gluttony,
                },
                HeightM = 2.1,
                WeightKg = 460,
                Color = "Black",
            },
            [SpecieId.SnorlaxGmax] = new()
            {
                Id = SpecieId.SnorlaxGmax,
                Num = 143,
                Name = "Snorlax-Gmax",
                BaseSpecies = SpecieId.Snorlax,
                Forme = FormeId.Gmax,
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 160,
                    Atk = 110,
                    Def = 65,
                    SpA = 65,
                    SpD = 110,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Immunity,
                    Slot1 = AbilityId.ThickFat,
                    Hidden = AbilityId.Gluttony,
                },
                HeightM = 35,
                WeightKg = 0,
                Color = "Black",
            },
            [SpecieId.Articuno] = new()
            {
                Id = SpecieId.Articuno,
                Num = 144,
                Name = "Articuno",
                Types = [PokemonType.Ice, PokemonType.Flying],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 85,
                    Def = 100,
                    SpA = 95,
                    SpD = 125,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pressure,
                    Hidden = AbilityId.SnowCloak,
                },
                HeightM = 1.7,
                WeightKg = 55.4,
                Color = "Blue",
            },
            [SpecieId.ArticunoGalar] = new()
            {
                Id = SpecieId.ArticunoGalar,
                Num = 144,
                Name = "Articuno-Galar",
                BaseSpecies = SpecieId.Articuno,
                Forme = FormeId.Galar,
                Types = [PokemonType.Psychic, PokemonType.Flying],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 85,
                    Def = 85,
                    SpA = 125,
                    SpD = 100,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Competitive,
                },
                HeightM = 1.7,
                WeightKg = 50.9,
                Color = "Purple",
            },
            [SpecieId.Zapdos] = new()
            {
                Id = SpecieId.Zapdos,
                Num = 145,
                Name = "Zapdos",
                Types = [PokemonType.Electric, PokemonType.Flying],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 90,
                    Def = 85,
                    SpA = 125,
                    SpD = 90,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pressure,
                    Hidden = AbilityId.Static,
                },
                HeightM = 1.6,
                WeightKg = 52.6,
                Color = "Yellow",
            },
            [SpecieId.ZapdosGalar] = new()
            {
                Id = SpecieId.ZapdosGalar,
                Num = 145,
                Name = "Zapdos-Galar",
                BaseSpecies = SpecieId.Zapdos,
                Forme = FormeId.Galar,
                Types = [PokemonType.Fighting, PokemonType.Flying],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 125,
                    Def = 90,
                    SpA = 85,
                    SpD = 90,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Defiant,
                },
                HeightM = 1.6,
                WeightKg = 58.2,
                Color = "Yellow",
            },
            [SpecieId.Moltres] = new()
            {
                Id = SpecieId.Moltres,
                Num = 146,
                Name = "Moltres",
                Types = [PokemonType.Fire, PokemonType.Flying],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 100,
                    Def = 90,
                    SpA = 125,
                    SpD = 85,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pressure,
                    Hidden = AbilityId.FlameBody,
                },
                HeightM = 2,
                WeightKg = 60,
                Color = "Yellow",
            },
            [SpecieId.MoltresGalar] = new()
            {
                Id = SpecieId.MoltresGalar,
                Num = 146,
                Name = "Moltres-Galar",
                BaseSpecies = SpecieId.Moltres,
                Forme = FormeId.Galar,
                Types = [PokemonType.Dark, PokemonType.Flying],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 85,
                    Def = 90,
                    SpA = 100,
                    SpD = 125,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Berserk,
                },
                HeightM = 2,
                WeightKg = 66,
                Color = "Red",
            },
            [SpecieId.Dratini] = new()
            {
                Id = SpecieId.Dratini,
                Num = 147,
                Name = "Dratini",
                Types = [PokemonType.Dragon],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 41,
                    Atk = 64,
                    Def = 45,
                    SpA = 50,
                    SpD = 50,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShedSkin,
                    Hidden = AbilityId.MarvelScale,
                },
                HeightM = 1.8,
                WeightKg = 3.3,
                Color = "Blue",
            },
            [SpecieId.Dragonair] = new()
            {
                Id = SpecieId.Dragonair,
                Num = 148,
                Name = "Dragonair",
                Types = [PokemonType.Dragon],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 61,
                    Atk = 84,
                    Def = 65,
                    SpA = 70,
                    SpD = 70,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShedSkin,
                    Hidden = AbilityId.MarvelScale,
                },
                HeightM = 4,
                WeightKg = 16.5,
                Color = "Blue",
            },
            [SpecieId.Dragonite] = new()
            {
                Id = SpecieId.Dragonite,
                Num = 149,
                Name = "Dragonite",
                Types = [PokemonType.Dragon, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 91,
                    Atk = 134,
                    Def = 95,
                    SpA = 100,
                    SpD = 100,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.InnerFocus,
                    Hidden = AbilityId.Multiscale,
                },
                HeightM = 2.2,
                WeightKg = 210,
                Color = "Brown",
            },
            [SpecieId.DragoniteMega] = new()
            {
                Id = SpecieId.DragoniteMega,
                Num = 149,
                Name = "Dragonite-Mega",
                BaseSpecies = SpecieId.Dragonite,
                Forme = FormeId.Mega,
                Types = [PokemonType.Dragon, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 91,
                    Atk = 124,
                    Def = 115,
                    SpA = 145,
                    SpD = 125,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.InnerFocus,
                    Hidden = AbilityId.Multiscale,
                },
                HeightM = 2.2,
                WeightKg = 290,
                Color = "Brown",
            },
            [SpecieId.Mewtwo] = new()
            {
                Id = SpecieId.Mewtwo,
                Num = 150,
                Name = "Mewtwo",
                Types = [PokemonType.Psychic],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 106,
                    Atk = 110,
                    Def = 90,
                    SpA = 154,
                    SpD = 90,
                    Spe = 130,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pressure,
                    Hidden = AbilityId.Unnerve,
                },
                HeightM = 2,
                WeightKg = 122,
                Color = "Purple",
            },
            [SpecieId.MewtwoMegaX] = new()
            {
                Id = SpecieId.MewtwoMegaX,
                Num = 150,
                Name = "Mewtwo-Mega-X",
                BaseSpecies = SpecieId.Mewtwo,
                Forme = FormeId.MegaX,
                Types = [PokemonType.Psychic, PokemonType.Fighting],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 106,
                    Atk = 190,
                    Def = 100,
                    SpA = 154,
                    SpD = 100,
                    Spe = 130,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Steadfast,
                },
                HeightM = 2.3,
                WeightKg = 127,
                Color = "Purple",
            },
            [SpecieId.MewtwoMegaY] = new()
            {
                Id = SpecieId.MewtwoMegaY,
                Num = 150,
                Name = "Mewtwo-Mega-Y",
                BaseSpecies = SpecieId.Mewtwo,
                Forme = FormeId.MegaY,
                Types = [PokemonType.Psychic],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 106,
                    Atk = 150,
                    Def = 70,
                    SpA = 194,
                    SpD = 120,
                    Spe = 140,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Insomnia,
                },
                HeightM = 1.5,
                WeightKg = 33,
                Color = "Purple",
            },
        };
    }
}