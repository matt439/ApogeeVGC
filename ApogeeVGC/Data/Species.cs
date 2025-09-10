﻿using System.Collections.ObjectModel;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data;

public record Species
{
    public IReadOnlyDictionary<SpecieId, Specie> SpeciesData { get; }

    public Species()
    {
        SpeciesData = new ReadOnlyDictionary<SpecieId, Specie>(_species);
    }

    private readonly Dictionary<SpecieId, Specie> _species = new()
    {
        [SpecieId.Bulbasaur] = new Specie
        {
            Id = SpecieId.Bulbasaur,
            Num = 1,
            Name = "Bulbasaur",
            Types = [PokemonType.Grass, PokemonType.Poison],
            Gender = GenderId.M,
            BaseStats = new StatsTable
            {
                Hp = 45,
                Atk = 49,
                Def = 49,
                SpA = 65,
                SpD = 65,
                Spe = 45,
            },
            Abilities = new SpeciesAbility
            {
                Slot0 = AbilityId.FlameBody,
                Hidden = AbilityId.Guts,
            },
            Height = 0.7,
            Weight = 6.9,
            Color = "Green",
        },
        [SpecieId.CalyrexIce] = new Specie
        {
            Id = SpecieId.CalyrexIce,
            Num = 898,
            Name = "Calyrex-Ice",
            BaseSpecies = "Calyrex",
            Forme = "Ice",
            Types = [PokemonType.Psychic, PokemonType.Ice],
            Gender = GenderId.N,
            BaseStats = new StatsTable()
            {
                Hp = 100,
                Atk = 165,
                Def = 150,
                SpA = 85,
                SpD = 130,
                Spe = 50,
            },
            Abilities = new SpeciesAbility { Slot0 = AbilityId.AsOneGlastrier },
            Height = 2.4,
            Weight = 809.1,
            Color = "White",
        },
        [SpecieId.Miraidon] = new Specie
        {
            Id = SpecieId.Miraidon,
            Num = 1008,
            Name = "Miraidon",
            Types = [PokemonType.Electric, PokemonType.Dragon],
            Gender = GenderId.N,
            BaseStats = new StatsTable
            {
                Hp = 100,
                Atk = 85,
                Def = 100,
                SpA = 135,
                SpD = 115,
                Spe = 135,
            },
            Abilities = new SpeciesAbility { Slot0 = AbilityId.HadronEngine },
            Height = 3.5,
            Weight = 240,
            Color = "Purple",
        },
        [SpecieId.Ursaluna] = new Specie
        {
            Id = SpecieId.Ursaluna,
            Num = 901,
            Name = "Ursaluna",
            Types = [PokemonType.Ground, PokemonType.Normal],
            Gender = GenderId.M, // Default, or use GenderId.N if genderless
            BaseStats = new StatsTable
            {
                Hp = 130,
                Atk = 140,
                Def = 105,
                SpA = 45,
                SpD = 80,
                Spe = 50,
            },
            Abilities = new SpeciesAbility
            {
                Slot0 = AbilityId.Guts,
                //Slot1 = "Bulletproof",
                //Hidden = "Unnerve"
            },
            Height = 2.4,
            Weight = 290,
            Color = "Brown",
        },
        [SpecieId.Volcarona] = new Specie
        {
            Id = SpecieId.Volcarona,
            Num = 637,
            Name = "Volcarona",
            Types = [PokemonType.Bug, PokemonType.Fire],
            Gender = GenderId.M, // or GenderId.N if genderless, adjust as needed
            BaseStats = new StatsTable
            {
                Hp = 85,
                Atk = 60,
                Def = 65,
                SpA = 135,
                SpD = 105,
                Spe = 100,
            },
            Abilities = new SpeciesAbility
            {
                Slot0 = AbilityId.FlameBody,
                // Hidden = "Swarm"
            },
            Height = 1.6,
            Weight = 46,
            Color = "White",
        },
        [SpecieId.Grimmsnarl] = new Specie
        {
            Id = SpecieId.Grimmsnarl,
            Num = 861,
            Name = "Grimmsnarl",
            Types = [PokemonType.Dark, PokemonType.Fairy],
            Gender = GenderId.M,
            BaseStats = new StatsTable
            {
                Hp = 95,
                Atk = 120,
                Def = 65,
                SpA = 95,
                SpD = 75,
                Spe = 60,
            },
            Abilities = new SpeciesAbility
            {
                Slot0 = AbilityId.Prankster,
                //Slot1 = "Frisk",
                //Hidden = "Pickpocket"
            },
            Height = 1.5,
            Weight = 61,
            Color = "Purple",
        },
        [SpecieId.IronHands] = new Specie
        {
            Id = SpecieId.IronHands,
            Num = 992,
            Name = "Iron Hands",
            Types = [PokemonType.Fighting, PokemonType.Electric],
            Gender = GenderId.N,
            BaseStats = new StatsTable
            {
                Hp = 154,
                Atk = 140,
                Def = 108,
                SpA = 50,
                SpD = 68,
                Spe = 50,
            },
            Abilities = new SpeciesAbility
            {
                Slot0 = AbilityId.QuarkDrive,
            },
            Height = 1.8,
            Weight = 380.7,
            Color = "Gray",
        },
    };
}