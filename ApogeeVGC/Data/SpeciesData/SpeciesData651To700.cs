using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData651To700()
    {
        return new Dictionary<SpecieId, Species>
        {
            // 651 - Quilladin
            [SpecieId.Quilladin] = new()
            {
                Id = SpecieId.Quilladin,
                Num = 651,
                Name = "Quilladin",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 61,
                    Atk = 78,
                    Def = 95,
                    SpA = 56,
                    SpD = 58,
                    Spe = 57,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.Bulletproof,
                },
                HeightM = 0.7,
                WeightKg = 29,
                Color = "Green",
                Prevo = SpecieId.Chespin,
            },

            // 652 - Chesnaught
            [SpecieId.Chesnaught] = new()
            {
                Id = SpecieId.Chesnaught,
                Num = 652,
                Name = "Chesnaught",
                Types = [PokemonType.Grass, PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 88,
                    Atk = 107,
                    Def = 122,
                    SpA = 74,
                    SpD = 75,
                    Spe = 64,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.Bulletproof,
                },
                HeightM = 1.6,
                WeightKg = 90,
                Color = "Green",
                Prevo = SpecieId.Quilladin,
            },

            // 652 - Chesnaught-Mega
            [SpecieId.ChesnaughtMega] = new()
            {
                Id = SpecieId.ChesnaughtMega,
                Num = 652,
                Name = "Chesnaught-Mega",
                BaseSpecies = SpecieId.Chesnaught,
                Forme = FormeId.Mega,
                Types = [PokemonType.Grass, PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 88,
                    Atk = 137,
                    Def = 172,
                    SpA = 74,
                    SpD = 115,
                    Spe = 44,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.Bulletproof,
                },
                HeightM = 1.6,
                WeightKg = 90,
                Color = "Green",
                RequiredItem = ItemId.Chesnaughtite,
            },

            // 653 - Fennekin
            [SpecieId.Fennekin] = new()
            {
                Id = SpecieId.Fennekin,
                Num = 653,
                Name = "Fennekin",
                Types = [PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 45,
                    Def = 40,
                    SpA = 62,
                    SpD = 60,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Blaze,
                    Hidden = AbilityId.Magician,
                },
                HeightM = 0.4,
                WeightKg = 9.4,
                Color = "Red",
            },

            // 654 - Braixen
            [SpecieId.Braixen] = new()
            {
                Id = SpecieId.Braixen,
                Num = 654,
                Name = "Braixen",
                Types = [PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 59,
                    Atk = 59,
                    Def = 58,
                    SpA = 90,
                    SpD = 70,
                    Spe = 73,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Blaze,
                    Hidden = AbilityId.Magician,
                },
                HeightM = 1,
                WeightKg = 14.5,
                Color = "Red",
                Prevo = SpecieId.Fennekin,
            },

            // 655 - Delphox
            [SpecieId.Delphox] = new()
            {
                Id = SpecieId.Delphox,
                Num = 655,
                Name = "Delphox",
                Types = [PokemonType.Fire, PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 69,
                    Def = 72,
                    SpA = 114,
                    SpD = 100,
                    Spe = 104,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Blaze,
                    Hidden = AbilityId.Magician,
                },
                HeightM = 1.5,
                WeightKg = 39,
                Color = "Red",
                Prevo = SpecieId.Braixen,
            },

            // 655 - Delphox-Mega
            [SpecieId.DelphoxMega] = new()
            {
                Id = SpecieId.DelphoxMega,
                Num = 655,
                Name = "Delphox-Mega",
                BaseSpecies = SpecieId.Delphox,
                Forme = FormeId.Mega,
                Types = [PokemonType.Fire, PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 69,
                    Def = 72,
                    SpA = 159,
                    SpD = 125,
                    Spe = 134,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Blaze,
                    Hidden = AbilityId.Magician,
                },
                HeightM = 1.5,
                WeightKg = 39,
                Color = "Red",
                RequiredItem = ItemId.Delphoxite,
            },

            // 656 - Froakie
            [SpecieId.Froakie] = new()
            {
                Id = SpecieId.Froakie,
                Num = 656,
                Name = "Froakie",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 41,
                    Atk = 56,
                    Def = 40,
                    SpA = 62,
                    SpD = 44,
                    Spe = 71,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Torrent,
                    Hidden = AbilityId.Protean,
                },
                HeightM = 0.3,
                WeightKg = 7,
                Color = "Blue",
            },

            // 657 - Frogadier
            [SpecieId.Frogadier] = new()
            {
                Id = SpecieId.Frogadier,
                Num = 657,
                Name = "Frogadier",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 54,
                    Atk = 63,
                    Def = 52,
                    SpA = 83,
                    SpD = 56,
                    Spe = 97,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Torrent,
                    Hidden = AbilityId.Protean,
                },
                HeightM = 0.6,
                WeightKg = 10.9,
                Color = "Blue",
                Prevo = SpecieId.Froakie,
            },

            // 658 - Greninja
            [SpecieId.Greninja] = new()
            {
                Id = SpecieId.Greninja,
                Num = 658,
                Name = "Greninja",
                Types = [PokemonType.Water, PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 72,
                    Atk = 95,
                    Def = 67,
                    SpA = 103,
                    SpD = 71,
                    Spe = 122,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Torrent,
                    Hidden = AbilityId.Protean,
                },
                HeightM = 1.5,
                WeightKg = 40,
                Color = "Blue",
                Prevo = SpecieId.Frogadier,
            },

            // 658 - Greninja-Bond
            [SpecieId.GreninjaBond] = new()
            {
                Id = SpecieId.GreninjaBond,
                Num = 658,
                Name = "Greninja-Bond",
                BaseSpecies = SpecieId.Greninja,
                Forme = FormeId.Bond,
                Types = [PokemonType.Water, PokemonType.Dark],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 72,
                    Atk = 95,
                    Def = 67,
                    SpA = 103,
                    SpD = 71,
                    Spe = 122,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.BattleBond,
                },
                HeightM = 1.5,
                WeightKg = 40,
                Color = "Blue",
            },

            // 658 - Greninja-Ash
            [SpecieId.GreninjaAsh] = new()
            {
                Id = SpecieId.GreninjaAsh,
                Num = 658,
                Name = "Greninja-Ash",
                BaseSpecies = SpecieId.Greninja,
                Forme = FormeId.Ash,
                Types = [PokemonType.Water, PokemonType.Dark],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 72,
                    Atk = 145,
                    Def = 67,
                    SpA = 153,
                    SpD = 71,
                    Spe = 132,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.BattleBond,
                },
                HeightM = 1.5,
                WeightKg = 40,
                Color = "Blue",
                RequiredAbility = AbilityId.BattleBond,
                BattleOnly = FormeId.Bond,
            },

            // 658 - Greninja-Mega
            [SpecieId.GreninjaMega] = new()
            {
                Id = SpecieId.GreninjaMega,
                Num = 658,
                Name = "Greninja-Mega",
                BaseSpecies = SpecieId.Greninja,
                Forme = FormeId.Mega,
                Types = [PokemonType.Water, PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 72,
                    Atk = 125,
                    Def = 77,
                    SpA = 133,
                    SpD = 81,
                    Spe = 142,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Torrent,
                    Hidden = AbilityId.Protean,
                },
                HeightM = 1.5,
                WeightKg = 40,
                Color = "Blue",
                RequiredItem = ItemId.Greninjite,
            },

            // 659 - Bunnelby
            [SpecieId.Bunnelby] = new()
            {
                Id = SpecieId.Bunnelby,
                Num = 659,
                Name = "Bunnelby",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 38,
                    Atk = 36,
                    Def = 38,
                    SpA = 32,
                    SpD = 36,
                    Spe = 57,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pickup,
                    Slot1 = AbilityId.CheekPouch,
                    Hidden = AbilityId.HugePower,
                },
                HeightM = 0.4,
                WeightKg = 5,
                Color = "Brown",
            },

            // 660 - Diggersby
            [SpecieId.Diggersby] = new()
            {
                Id = SpecieId.Diggersby,
                Num = 660,
                Name = "Diggersby",
                Types = [PokemonType.Normal, PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 85,
                    Atk = 56,
                    Def = 77,
                    SpA = 50,
                    SpD = 77,
                    Spe = 78,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pickup,
                    Slot1 = AbilityId.CheekPouch,
                    Hidden = AbilityId.HugePower,
                },
                HeightM = 1,
                WeightKg = 42.4,
                Color = "Brown",
                Prevo = SpecieId.Bunnelby,
            },

            // 661 - Fletchling
            [SpecieId.Fletchling] = new()
            {
                Id = SpecieId.Fletchling,
                Num = 661,
                Name = "Fletchling",
                Types = [PokemonType.Normal, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 50,
                    Def = 43,
                    SpA = 40,
                    SpD = 38,
                    Spe = 62,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.BigPecks,
                    Hidden = AbilityId.GaleWings,
                },
                HeightM = 0.3,
                WeightKg = 1.7,
                Color = "Red",
            },

            // 662 - Fletchinder
            [SpecieId.Fletchinder] = new()
            {
                Id = SpecieId.Fletchinder,
                Num = 662,
                Name = "Fletchinder",
                Types = [PokemonType.Fire, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 62,
                    Atk = 73,
                    Def = 55,
                    SpA = 56,
                    SpD = 52,
                    Spe = 84,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.FlameBody,
                    Hidden = AbilityId.GaleWings,
                },
                HeightM = 0.7,
                WeightKg = 16,
                Color = "Red",
                Prevo = SpecieId.Fletchling,
            },

            // 663 - Talonflame
            [SpecieId.Talonflame] = new()
            {
                Id = SpecieId.Talonflame,
                Num = 663,
                Name = "Talonflame",
                Types = [PokemonType.Fire, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 78,
                    Atk = 81,
                    Def = 71,
                    SpA = 74,
                    SpD = 69,
                    Spe = 126,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.FlameBody,
                    Hidden = AbilityId.GaleWings,
                },
                HeightM = 1.2,
                WeightKg = 24.5,
                Color = "Red",
                Prevo = SpecieId.Fletchinder,
            },

            // 664 - Scatterbug
            [SpecieId.Scatterbug] = new()
            {
                Id = SpecieId.Scatterbug,
                Num = 664,
                Name = "Scatterbug",
                Types = [PokemonType.Bug],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 38,
                    Atk = 35,
                    Def = 40,
                    SpA = 27,
                    SpD = 25,
                    Spe = 35,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShieldDust,
                    Slot1 = AbilityId.CompoundEyes,
                    Hidden = AbilityId.FriendGuard,
                },
                HeightM = 0.3,
                WeightKg = 2.5,
                Color = "Black",
            },

            // 665 - Spewpa
            [SpecieId.Spewpa] = new()
            {
                Id = SpecieId.Spewpa,
                Num = 665,
                Name = "Spewpa",
                Types = [PokemonType.Bug],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 22,
                    Def = 60,
                    SpA = 27,
                    SpD = 30,
                    Spe = 29,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShedSkin,
                    Hidden = AbilityId.FriendGuard,
                },
                HeightM = 0.3,
                WeightKg = 8.4,
                Color = "Black",
                Prevo = SpecieId.Scatterbug,
            },

            // 666 - Vivillon
            [SpecieId.Vivillon] = new()
            {
                Id = SpecieId.Vivillon,
                Num = 666,
                Name = "Vivillon",
                Types = [PokemonType.Bug, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 52,
                    Def = 50,
                    SpA = 90,
                    SpD = 50,
                    Spe = 89,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShieldDust,
                    Slot1 = AbilityId.CompoundEyes,
                    Hidden = AbilityId.FriendGuard,
                },
                HeightM = 1.2,
                WeightKg = 17,
                Color = "Pink",
                Prevo = SpecieId.Spewpa,
            },

            // 666 - Vivillon-Fancy
            [SpecieId.VivillonFancy] = new()
            {
                Id = SpecieId.VivillonFancy,
                Num = 666,
                Name = "Vivillon-Fancy",
                BaseSpecies = SpecieId.Vivillon,
                Forme = FormeId.Fancy,
                Types = [PokemonType.Bug, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 52,
                    Def = 50,
                    SpA = 90,
                    SpD = 50,
                    Spe = 89,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShieldDust,
                    Slot1 = AbilityId.CompoundEyes,
                    Hidden = AbilityId.FriendGuard,
                },
                HeightM = 1.2,
                WeightKg = 17,
                Color = "Pink",
                Prevo = SpecieId.Spewpa,
            },

            // 666 - Vivillon-Pokeball
            [SpecieId.VivillonPokeball] = new()
            {
                Id = SpecieId.VivillonPokeball,
                Num = 666,
                Name = "Vivillon-Pokeball",
                BaseSpecies = SpecieId.Vivillon,
                Forme = FormeId.Pokeball,
                Types = [PokemonType.Bug, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 52,
                    Def = 50,
                    SpA = 90,
                    SpD = 50,
                    Spe = 89,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShieldDust,
                    Slot1 = AbilityId.CompoundEyes,
                    Hidden = AbilityId.FriendGuard,
                },
                HeightM = 1.2,
                WeightKg = 17,
                Color = "Red",
            },

            // 667 - Litleo
            [SpecieId.Litleo] = new()
            {
                Id = SpecieId.Litleo,
                Num = 667,
                Name = "Litleo",
                Types = [PokemonType.Fire, PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 62,
                    Atk = 50,
                    Def = 58,
                    SpA = 73,
                    SpD = 54,
                    Spe = 72,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Rivalry,
                    Slot1 = AbilityId.Unnerve,
                    Hidden = AbilityId.Moxie,
                },
                HeightM = 0.6,
                WeightKg = 13.5,
                Color = "Brown",
            },

            // 668 - Pyroar
            [SpecieId.Pyroar] = new()
            {
                Id = SpecieId.Pyroar,
                Num = 668,
                Name = "Pyroar",
                Types = [PokemonType.Fire, PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 86,
                    Atk = 68,
                    Def = 72,
                    SpA = 109,
                    SpD = 66,
                    Spe = 106,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Rivalry,
                    Slot1 = AbilityId.Unnerve,
                    Hidden = AbilityId.Moxie,
                },
                HeightM = 1.5,
                WeightKg = 81.5,
                Color = "Brown",
                Prevo = SpecieId.Litleo,
            },

            // 668 - Pyroar-Mega
            [SpecieId.PyroarMega] = new()
            {
                Id = SpecieId.PyroarMega,
                Num = 668,
                Name = "Pyroar-Mega",
                BaseSpecies = SpecieId.Pyroar,
                Forme = FormeId.Mega,
                Types = [PokemonType.Fire, PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 86,
                    Atk = 88,
                    Def = 92,
                    SpA = 129,
                    SpD = 86,
                    Spe = 126,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Rivalry,
                    Slot1 = AbilityId.Unnerve,
                    Hidden = AbilityId.Moxie,
                },
                HeightM = 1.5,
                WeightKg = 93.3,
                Color = "Brown",
                RequiredItem = ItemId.Pyroarite,
            },

            // 669 - Flabébé
            [SpecieId.Flabebe] = new()
            {
                Id = SpecieId.Flabebe,
                Num = 669,
                Name = "Flabébé",
                Types = [PokemonType.Fairy],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 44,
                    Atk = 38,
                    Def = 39,
                    SpA = 61,
                    SpD = 79,
                    Spe = 42,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.FlowerVeil,
                    Hidden = AbilityId.Symbiosis,
                },
                HeightM = 0.1,
                WeightKg = 0.1,
                Color = "White",
            },

            // 670 - Floette
            [SpecieId.Floette] = new()
            {
                Id = SpecieId.Floette,
                Num = 670,
                Name = "Floette",
                Types = [PokemonType.Fairy],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 54,
                    Atk = 45,
                    Def = 47,
                    SpA = 75,
                    SpD = 98,
                    Spe = 52,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.FlowerVeil,
                    Hidden = AbilityId.Symbiosis,
                },
                HeightM = 0.2,
                WeightKg = 0.9,
                Color = "White",
                Prevo = SpecieId.Flabebe,
            },

            // 670 - Floette-Eternal
            [SpecieId.FloetteEternal] = new()
            {
                Id = SpecieId.FloetteEternal,
                Num = 670,
                Name = "Floette-Eternal",
                BaseSpecies = SpecieId.Floette,
                Forme = FormeId.Eternal,
                Types = [PokemonType.Fairy],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 74,
                    Atk = 65,
                    Def = 67,
                    SpA = 125,
                    SpD = 128,
                    Spe = 92,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.FlowerVeil,
                    Hidden = AbilityId.Symbiosis,
                },
                HeightM = 0.2,
                WeightKg = 0.9,
                Color = "White",
            },

            // 670 - Floette-Mega
            [SpecieId.FloetteMega] = new()
            {
                Id = SpecieId.FloetteMega,
                Num = 670,
                Name = "Floette-Mega",
                BaseSpecies = SpecieId.Floette,
                Forme = FormeId.Mega,
                Types = [PokemonType.Fairy],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 74,
                    Atk = 85,
                    Def = 87,
                    SpA = 155,
                    SpD = 148,
                    Spe = 102,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.FlowerVeil,
                    Hidden = AbilityId.Symbiosis,
                },
                HeightM = 0.2,
                WeightKg = 100.8,
                Color = "White",
                RequiredItem = ItemId.Floettite,
                BattleOnly = FormeId.Eternal,
            },

            // 671 - Florges
            [SpecieId.Florges] = new()
            {
                Id = SpecieId.Florges,
                Num = 671,
                Name = "Florges",
                Types = [PokemonType.Fairy],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 78,
                    Atk = 65,
                    Def = 68,
                    SpA = 112,
                    SpD = 154,
                    Spe = 75,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.FlowerVeil,
                    Hidden = AbilityId.Symbiosis,
                },
                HeightM = 1.1,
                WeightKg = 10,
                Color = "White",
                Prevo = SpecieId.Floette,
            },

            // 672 - Skiddo
            [SpecieId.Skiddo] = new()
            {
                Id = SpecieId.Skiddo,
                Num = 672,
                Name = "Skiddo",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 66,
                    Atk = 65,
                    Def = 48,
                    SpA = 62,
                    SpD = 57,
                    Spe = 52,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SapSipper,
                    Hidden = AbilityId.GrassPelt,
                },
                HeightM = 0.9,
                WeightKg = 31,
                Color = "Brown",
            },

            // 673 - Gogoat
            [SpecieId.Gogoat] = new()
            {
                Id = SpecieId.Gogoat,
                Num = 673,
                Name = "Gogoat",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 123,
                    Atk = 100,
                    Def = 62,
                    SpA = 97,
                    SpD = 81,
                    Spe = 68,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SapSipper,
                    Hidden = AbilityId.GrassPelt,
                },
                HeightM = 1.7,
                WeightKg = 91,
                Color = "Brown",
                Prevo = SpecieId.Skiddo,
            },

            // 674 - Pancham
            [SpecieId.Pancham] = new()
            {
                Id = SpecieId.Pancham,
                Num = 674,
                Name = "Pancham",
                Types = [PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 67,
                    Atk = 82,
                    Def = 62,
                    SpA = 46,
                    SpD = 48,
                    Spe = 43,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.IronFist,
                    Slot1 = AbilityId.MoldBreaker,
                    Hidden = AbilityId.Scrappy,
                },
                HeightM = 0.6,
                WeightKg = 8,
                Color = "White",
            },

            // 675 - Pangoro
            [SpecieId.Pangoro] = new()
            {
                Id = SpecieId.Pangoro,
                Num = 675,
                Name = "Pangoro",
                Types = [PokemonType.Fighting, PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 124,
                    Def = 78,
                    SpA = 69,
                    SpD = 71,
                    Spe = 58,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.IronFist,
                    Slot1 = AbilityId.MoldBreaker,
                    Hidden = AbilityId.Scrappy,
                },
                HeightM = 2.1,
                WeightKg = 136,
                Color = "White",
                Prevo = SpecieId.Pancham,
            },
        };
    }
}
