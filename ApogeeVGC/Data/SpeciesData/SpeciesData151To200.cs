using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData151To200()
    {
        return new Dictionary<SpecieId, Species>
        {
            [SpecieId.Mew] = new()
            {
                Id = SpecieId.Mew,
                Num = 151,
                Name = "Mew",
                Types = [PokemonType.Psychic],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 100,
                    Atk = 100,
                    Def = 100,
                    SpA = 100,
                    SpD = 100,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Synchronize,
                },
                HeightM = 0.4,
                WeightKg = 4,
                Color = "Pink",
            },
            [SpecieId.Chikorita] = new()
            {
                Id = SpecieId.Chikorita,
                Num = 152,
                Name = "Chikorita",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 49,
                    Def = 65,
                    SpA = 49,
                    SpD = 65,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.LeafGuard,
                },
                HeightM = 0.9,
                WeightKg = 6.4,
                Color = "Green",
            },
            [SpecieId.Bayleef] = new()
            {
                Id = SpecieId.Bayleef,
                Num = 153,
                Name = "Bayleef",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 62,
                    Def = 80,
                    SpA = 63,
                    SpD = 80,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.LeafGuard,
                },
                HeightM = 1.2,
                WeightKg = 15.8,
                Color = "Green",
            },
            [SpecieId.Meganium] = new()
            {
                Id = SpecieId.Meganium,
                Num = 154,
                Name = "Meganium",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 82,
                    Def = 100,
                    SpA = 83,
                    SpD = 100,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.LeafGuard,
                },
                HeightM = 1.8,
                WeightKg = 100.5,
                Color = "Green",
            },
            [SpecieId.MeganiummMega] = new()
            {
                Id = SpecieId.MeganiummMega,
                Num = 154,
                Name = "Meganium-Mega",
                BaseSpecies = SpecieId.Meganium,
                Forme = FormeId.Mega,
                Types = [PokemonType.Grass, PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 92,
                    Def = 115,
                    SpA = 143,
                    SpD = 115,
                    Spe = 80,
                },
                                Abilities = new SpeciesAbility
                                {
                                    Slot0 = AbilityId.Overgrow,
                                    Hidden = AbilityId.LeafGuard,
                                },
                                HeightM = 2.4,
                                WeightKg = 201,
                                Color = "Green",
                            },
                            [SpecieId.Cyndaquil] = new()
                            {
                                Id = SpecieId.Cyndaquil,
                                Num = 155,
                                Name = "Cyndaquil",
                                Types = [PokemonType.Fire],
                                Gender = GenderId.Empty,
                                BaseStats = new StatsTable
                                {
                                    Hp = 39,
                                    Atk = 52,
                                    Def = 43,
                                    SpA = 60,
                                    SpD = 50,
                                    Spe = 65,
                                },
                                Abilities = new SpeciesAbility
                                {
                                    Slot0 = AbilityId.Blaze,
                                    Hidden = AbilityId.FlashFire,
                                },
                                HeightM = 0.5,
                                WeightKg = 7.9,
                                Color = "Yellow",
                            },
                            [SpecieId.Quilava] = new()
                            {
                                Id = SpecieId.Quilava,
                                Num = 156,
                                Name = "Quilava",
                                Types = [PokemonType.Fire],
                                Gender = GenderId.Empty,
                                BaseStats = new StatsTable
                                {
                                    Hp = 58,
                                    Atk = 64,
                                    Def = 58,
                                    SpA = 80,
                                    SpD = 65,
                                    Spe = 80,
                                },
                                Abilities = new SpeciesAbility
                                {
                                    Slot0 = AbilityId.Blaze,
                                    Hidden = AbilityId.FlashFire,
                                },
                                HeightM = 0.9,
                                WeightKg = 19,
                                Color = "Yellow",
                            },
                            [SpecieId.Typhlosion] = new()
                            {
                                Id = SpecieId.Typhlosion,
                                Num = 157,
                                Name = "Typhlosion",
                                Types = [PokemonType.Fire],
                                Gender = GenderId.Empty,
                                BaseStats = new StatsTable
                                {
                                    Hp = 78,
                                    Atk = 84,
                                    Def = 78,
                                    SpA = 109,
                                    SpD = 85,
                                    Spe = 100,
                                },
                                Abilities = new SpeciesAbility
                                {
                                    Slot0 = AbilityId.Blaze,
                                    Hidden = AbilityId.FlashFire,
                                },
                                HeightM = 1.7,
                                WeightKg = 79.5,
                                Color = "Yellow",
                            },
                            [SpecieId.TyphlosionHisui] = new()
                            {
                                Id = SpecieId.TyphlosionHisui,
                                Num = 157,
                                Name = "Typhlosion-Hisui",
                                BaseSpecies = SpecieId.Typhlosion,
                                Forme = FormeId.Hisui,
                                Types = [PokemonType.Fire, PokemonType.Ghost],
                                Gender = GenderId.Empty,
                                BaseStats = new StatsTable
                                {
                                    Hp = 73,
                                    Atk = 84,
                                    Def = 78,
                                    SpA = 119,
                                    SpD = 85,
                                    Spe = 95,
                                },
                                Abilities = new SpeciesAbility
                                {
                                    Slot0 = AbilityId.Blaze,
                                    Hidden = AbilityId.Frisk,
                                },
                                HeightM = 1.6,
                                WeightKg = 69.8,
                                Color = "Yellow",
                            },
                            [SpecieId.Totodile] = new()
                            {
                                Id = SpecieId.Totodile,
                                Num = 158,
                                Name = "Totodile",
                                Types = [PokemonType.Water],
                                Gender = GenderId.Empty,
                                BaseStats = new StatsTable
                                {
                                    Hp = 50,
                                    Atk = 65,
                                    Def = 64,
                                    SpA = 44,
                                    SpD = 48,
                                    Spe = 43,
                                },
                                Abilities = new SpeciesAbility
                                {
                                    Slot0 = AbilityId.Torrent,
                                    Hidden = AbilityId.SheerForce,
                                },
                                HeightM = 0.6,
                                WeightKg = 9.5,
                                Color = "Blue",
                            },
                            [SpecieId.Croconaw] = new()
                            {
                                Id = SpecieId.Croconaw,
                                Num = 159,
                                Name = "Croconaw",
                                Types = [PokemonType.Water],
                                Gender = GenderId.Empty,
                                BaseStats = new StatsTable
                                {
                                    Hp = 65,
                                    Atk = 80,
                                    Def = 80,
                                    SpA = 59,
                                    SpD = 63,
                                    Spe = 58,
                                },
                                Abilities = new SpeciesAbility
                                {
                                    Slot0 = AbilityId.Torrent,
                                    Hidden = AbilityId.SheerForce,
                                },
                                HeightM = 1.1,
                                WeightKg = 25,
                                Color = "Blue",
                            },
                            [SpecieId.Feraligatr] = new()
                            {
                                Id = SpecieId.Feraligatr,
                                Num = 160,
                                Name = "Feraligatr",
                                Types = [PokemonType.Water],
                                Gender = GenderId.Empty,
                                BaseStats = new StatsTable
                                {
                                    Hp = 85,
                                    Atk = 105,
                                    Def = 100,
                                    SpA = 79,
                                    SpD = 83,
                                    Spe = 78,
                                },
                                Abilities = new SpeciesAbility
                                {
                                    Slot0 = AbilityId.Torrent,
                                    Hidden = AbilityId.SheerForce,
                                },
                                HeightM = 2.3,
                                WeightKg = 88.8,
                                Color = "Blue",
                            },
                            [SpecieId.FeraligatrMega] = new()
                            {
                                Id = SpecieId.FeraligatrMega,
                                Num = 160,
                                Name = "Feraligatr-Mega",
                                BaseSpecies = SpecieId.Feraligatr,
                                Forme = FormeId.Mega,
                                Types = [PokemonType.Water, PokemonType.Dragon],
                                Gender = GenderId.Empty,
                                BaseStats = new StatsTable
                                {
                                    Hp = 85,
                                    Atk = 160,
                                    Def = 125,
                                    SpA = 89,
                                    SpD = 93,
                                    Spe = 78,
                                },
                                Abilities = new SpeciesAbility
                                {
                                    Slot0 = AbilityId.Torrent,
                                    Hidden = AbilityId.SheerForce,
                                },
                                HeightM = 2.3,
                                WeightKg = 108.8,
                                Color = "Blue",
                            },
                            [SpecieId.Sentret] = new()
                            {
                                Id = SpecieId.Sentret,
                                Num = 161,
                                Name = "Sentret",
                                Types = [PokemonType.Normal],
                                Gender = GenderId.Empty,
                                BaseStats = new StatsTable
                                {
                                    Hp = 35,
                                    Atk = 46,
                                    Def = 34,
                                    SpA = 35,
                                    SpD = 45,
                                    Spe = 20,
                                },
                                Abilities = new SpeciesAbility
                                {
                                    Slot0 = AbilityId.RunAway,
                                    Slot1 = AbilityId.KeenEye,
                                    Hidden = AbilityId.Frisk,
                                },
                                HeightM = 0.8,
                                WeightKg = 6,
                                Color = "Brown",
                            },
                            [SpecieId.Furret] = new()
                            {
                                Id = SpecieId.Furret,
                                Num = 162,
                                Name = "Furret",
                                Types = [PokemonType.Normal],
                                Gender = GenderId.Empty,
                                BaseStats = new StatsTable
                                {
                                    Hp = 85,
                                    Atk = 76,
                                    Def = 64,
                                    SpA = 45,
                                    SpD = 55,
                                    Spe = 90,
                                },
                                Abilities = new SpeciesAbility
                                {
                                    Slot0 = AbilityId.RunAway,
                                    Slot1 = AbilityId.KeenEye,
                                    Hidden = AbilityId.Frisk,
                                },
                                HeightM = 1.8,
                                WeightKg = 32.5,
                                Color = "Brown",
                            },
                        };
                    }
                }
