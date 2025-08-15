using ApogeeVGC.Sim;

namespace ApogeeVGC.Data;

public static class TypeChart
{
    public static Dictionary<PokemonType, TypeData> TypeData { get; } = new()
    {
        [PokemonType.Bug] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, MoveEffectiveness>
            {
                [MoveType.Bug] = MoveEffectiveness.Normal,
                [MoveType.Dark] = MoveEffectiveness.Normal,
                [MoveType.Dragon] = MoveEffectiveness.Normal,
                [MoveType.Electric] = MoveEffectiveness.Normal,
                [MoveType.Fairy] = MoveEffectiveness.Normal,
                [MoveType.Fighting] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Fire] = MoveEffectiveness.SuperEffective,
                [MoveType.Flying] = MoveEffectiveness.SuperEffective,
                [MoveType.Ghost] = MoveEffectiveness.Normal,
                [MoveType.Grass] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Ground] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Ice] = MoveEffectiveness.Normal,
                [MoveType.Normal] = MoveEffectiveness.Normal,
                [MoveType.Poison] = MoveEffectiveness.Normal,
                [MoveType.Psychic] = MoveEffectiveness.Normal,
                [MoveType.Rock] = MoveEffectiveness.SuperEffective,
                [MoveType.Steel] = MoveEffectiveness.Normal,
                [MoveType.Stellar] = MoveEffectiveness.Normal,
                [MoveType.Water] = MoveEffectiveness.Normal,
            },
            HpIvs = new Dictionary<StatType, int>
            {
                [StatType.Atk] = 30,
                [StatType.Def] = 30,
                [StatType.SpD] = 30,
            },
            HpDvs = new Dictionary<StatType, int>
            {
                [StatType.Atk] = 13,
                [StatType.Def] = 13,
            },
        },
        [PokemonType.Fire] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, MoveEffectiveness>
            {
                [MoveType.Bug] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Dark] = MoveEffectiveness.Normal,
                [MoveType.Dragon] = MoveEffectiveness.Normal,
                [MoveType.Electric] = MoveEffectiveness.Normal,
                [MoveType.Fairy] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Fighting] = MoveEffectiveness.Normal,
                [MoveType.Fire] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Flying] = MoveEffectiveness.Normal,
                [MoveType.Ghost] = MoveEffectiveness.Normal,
                [MoveType.Grass] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Ground] = MoveEffectiveness.SuperEffective,
                [MoveType.Ice] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Normal] = MoveEffectiveness.Normal,
                [MoveType.Poison] = MoveEffectiveness.Normal,
                [MoveType.Psychic] = MoveEffectiveness.Normal,
                [MoveType.Rock] = MoveEffectiveness.SuperEffective,
                [MoveType.Steel] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Stellar] = MoveEffectiveness.Normal,
                [MoveType.Water] = MoveEffectiveness.SuperEffective,
            },
            HpIvs = new Dictionary<StatType, int>
            {
                [StatType.Atk] = 30,
                [StatType.SpA] = 30,
                [StatType.Spe] = 30,
            },
            HpDvs = new Dictionary<StatType, int>
            {
                [StatType.Atk] = 14,
                [StatType.Def] = 12,
            },
        },
        [PokemonType.Water] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, MoveEffectiveness>
            {
                [MoveType.Bug] = MoveEffectiveness.Normal,
                [MoveType.Dark] = MoveEffectiveness.Normal,
                [MoveType.Dragon] = MoveEffectiveness.Normal,
                [MoveType.Electric] = MoveEffectiveness.SuperEffective,
                [MoveType.Fairy] = MoveEffectiveness.Normal,
                [MoveType.Fighting] = MoveEffectiveness.Normal,
                [MoveType.Fire] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Flying] = MoveEffectiveness.Normal,
                [MoveType.Ghost] = MoveEffectiveness.Normal,
                [MoveType.Grass] = MoveEffectiveness.SuperEffective,
                [MoveType.Ground] = MoveEffectiveness.Normal,
                [MoveType.Ice] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Normal] = MoveEffectiveness.Normal,
                [MoveType.Poison] = MoveEffectiveness.Normal,
                [MoveType.Psychic] = MoveEffectiveness.Normal,
                [MoveType.Rock] = MoveEffectiveness.Normal,
                [MoveType.Steel] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Stellar] = MoveEffectiveness.Normal,
                [MoveType.Water] = MoveEffectiveness.NotVeryEffective,
            },
            HpIvs = new Dictionary<StatType, int>
            {
                [StatType.Atk] = 30,
                [StatType.Def] = 30,
                [StatType.SpA] = 30,
            },
            HpDvs = new Dictionary<StatType, int>
            {
                [StatType.Atk] = 14,
                [StatType.Def] = 13,
            },
        },
        [PokemonType.Electric] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, MoveEffectiveness>
            {
                [MoveType.Bug] = MoveEffectiveness.Normal,
                [MoveType.Dark] = MoveEffectiveness.Normal,
                [MoveType.Dragon] = MoveEffectiveness.Normal,
                [MoveType.Electric] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Fairy] = MoveEffectiveness.Normal,
                [MoveType.Fighting] = MoveEffectiveness.Normal,
                [MoveType.Fire] = MoveEffectiveness.Normal,
                [MoveType.Flying] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Ghost] = MoveEffectiveness.Normal,
                [MoveType.Grass] = MoveEffectiveness.Normal,
                [MoveType.Ground] = MoveEffectiveness.SuperEffective,
                [MoveType.Ice] = MoveEffectiveness.Normal,
                [MoveType.Normal] = MoveEffectiveness.Normal,
                [MoveType.Poison] = MoveEffectiveness.Normal,
                [MoveType.Psychic] = MoveEffectiveness.Normal,
                [MoveType.Rock] = MoveEffectiveness.Normal,
                [MoveType.Steel] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Stellar] = MoveEffectiveness.Normal,
                [MoveType.Water] = MoveEffectiveness.Normal,
            },
            HpIvs = new Dictionary<StatType, int>
            {
                [StatType.SpA] = 30,
            },
            HpDvs = new Dictionary<StatType, int>
            {
                [StatType.Atk] = 14,
            },
        },
        [PokemonType.Grass] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, MoveEffectiveness>
            {
                [MoveType.Bug] = MoveEffectiveness.SuperEffective,
                [MoveType.Dark] = MoveEffectiveness.Normal,
                [MoveType.Dragon] = MoveEffectiveness.Normal,
                [MoveType.Electric] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Fairy] = MoveEffectiveness.Normal,
                [MoveType.Fighting] = MoveEffectiveness.Normal,
                [MoveType.Fire] = MoveEffectiveness.SuperEffective,
                [MoveType.Flying] = MoveEffectiveness.SuperEffective,
                [MoveType.Ghost] = MoveEffectiveness.Normal,
                [MoveType.Grass] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Ground] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Ice] = MoveEffectiveness.SuperEffective,
                [MoveType.Normal] = MoveEffectiveness.Normal,
                [MoveType.Poison] = MoveEffectiveness.SuperEffective,
                [MoveType.Psychic] = MoveEffectiveness.Normal,
                [MoveType.Rock] = MoveEffectiveness.Normal,
                [MoveType.Steel] = MoveEffectiveness.Normal,
                [MoveType.Stellar] = MoveEffectiveness.Normal,
                [MoveType.Water] = MoveEffectiveness.NotVeryEffective,
            },
            HpIvs = new Dictionary<StatType, int>
            {
                [StatType.Atk] = 30,
                [StatType.SpA] = 30,
            },
            HpDvs = new Dictionary<StatType, int>
            {
                [StatType.Atk] = 14,
                [StatType.Def] = 14,
            },
        },
        [PokemonType.Ice] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, MoveEffectiveness>
            {
                [MoveType.Bug] = MoveEffectiveness.Normal,
                [MoveType.Dark] = MoveEffectiveness.Normal,
                [MoveType.Dragon] = MoveEffectiveness.Normal,
                [MoveType.Electric] = MoveEffectiveness.Normal,
                [MoveType.Fairy] = MoveEffectiveness.Normal,
                [MoveType.Fighting] = MoveEffectiveness.SuperEffective,
                [MoveType.Fire] = MoveEffectiveness.SuperEffective,
                [MoveType.Flying] = MoveEffectiveness.Normal,
                [MoveType.Ghost] = MoveEffectiveness.Normal,
                [MoveType.Grass] = MoveEffectiveness.Normal,
                [MoveType.Ground] = MoveEffectiveness.Normal,
                [MoveType.Ice] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Normal] = MoveEffectiveness.Normal,
                [MoveType.Poison] = MoveEffectiveness.Normal,
                [MoveType.Psychic] = MoveEffectiveness.Normal,
                [MoveType.Rock] = MoveEffectiveness.SuperEffective,
                [MoveType.Steel] = MoveEffectiveness.SuperEffective,
                [MoveType.Stellar] = MoveEffectiveness.Normal,
                [MoveType.Water] = MoveEffectiveness.Normal,
            },
            HpIvs = new Dictionary<StatType, int>
            {
                [StatType.Atk] = 30,
                [StatType.Def] = 30,
            },
            HpDvs = new Dictionary<StatType, int>
            {
                [StatType.Def] = 13,
            },
        },
        [PokemonType.Fighting] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, MoveEffectiveness>
            {
                [MoveType.Bug] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Dark] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Dragon] = MoveEffectiveness.Normal,
                [MoveType.Electric] = MoveEffectiveness.Normal,
                [MoveType.Fairy] = MoveEffectiveness.SuperEffective,
                [MoveType.Fighting] = MoveEffectiveness.Normal,
                [MoveType.Fire] = MoveEffectiveness.Normal,
                [MoveType.Flying] = MoveEffectiveness.SuperEffective,
                [MoveType.Ghost] = MoveEffectiveness.Normal,
                [MoveType.Grass] = MoveEffectiveness.Normal,
                [MoveType.Ground] = MoveEffectiveness.Normal,
                [MoveType.Ice] = MoveEffectiveness.Normal,
                [MoveType.Normal] = MoveEffectiveness.Normal,
                [MoveType.Poison] = MoveEffectiveness.Normal,
                [MoveType.Psychic] = MoveEffectiveness.SuperEffective,
                [MoveType.Rock] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Steel] = MoveEffectiveness.Normal,
                [MoveType.Stellar] = MoveEffectiveness.Normal,
                [MoveType.Water] = MoveEffectiveness.Normal,
            },
            HpIvs = new Dictionary<StatType, int>
            {
                [StatType.Def] = 30,
                [StatType.SpA] = 30,
                [StatType.SpD] = 30,
                [StatType.Spe] = 30,
            },
            HpDvs = new Dictionary<StatType, int>
            {
                [StatType.Atk] = 12,
                [StatType.Def] = 12,
            },
        },
        [PokemonType.Poison] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, MoveEffectiveness>
            {
                [MoveType.Bug] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Dark] = MoveEffectiveness.Normal,
                [MoveType.Dragon] = MoveEffectiveness.Normal,
                [MoveType.Electric] = MoveEffectiveness.Normal,
                [MoveType.Fairy] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Fighting] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Fire] = MoveEffectiveness.Normal,
                [MoveType.Flying] = MoveEffectiveness.Normal,
                [MoveType.Ghost] = MoveEffectiveness.Normal,
                [MoveType.Grass] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Ground] = MoveEffectiveness.SuperEffective,
                [MoveType.Ice] = MoveEffectiveness.Normal,
                [MoveType.Normal] = MoveEffectiveness.Normal,
                [MoveType.Poison] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Psychic] = MoveEffectiveness.SuperEffective,
                [MoveType.Rock] = MoveEffectiveness.Normal,
                [MoveType.Steel] = MoveEffectiveness.Normal,
                [MoveType.Stellar] = MoveEffectiveness.Normal,
                [MoveType.Water] = MoveEffectiveness.Normal,
            },
            HpIvs = new Dictionary<StatType, int>
            {
                [StatType.Def] = 30,
                [StatType.SpA] = 30,
                [StatType.SpD] = 30,
            },
            HpDvs = new Dictionary<StatType, int>
            {
                [StatType.Atk] = 12,
                [StatType.Def] = 14,
            },
        },
        [PokemonType.Ground] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, MoveEffectiveness>
            {
                [MoveType.Bug] = MoveEffectiveness.Normal,
                [MoveType.Dark] = MoveEffectiveness.Normal,
                [MoveType.Dragon] = MoveEffectiveness.Normal,
                [MoveType.Electric] = MoveEffectiveness.Immune,
                [MoveType.Fairy] = MoveEffectiveness.Normal,
                [MoveType.Fighting] = MoveEffectiveness.Normal,
                [MoveType.Fire] = MoveEffectiveness.Normal,
                [MoveType.Flying] = MoveEffectiveness.Normal,
                [MoveType.Ghost] = MoveEffectiveness.Normal,
                [MoveType.Grass] = MoveEffectiveness.SuperEffective,
                [MoveType.Ground] = MoveEffectiveness.Normal,
                [MoveType.Ice] = MoveEffectiveness.SuperEffective,
                [MoveType.Normal] = MoveEffectiveness.Normal,
                [MoveType.Poison] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Psychic] = MoveEffectiveness.Normal,
                [MoveType.Rock] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Steel] = MoveEffectiveness.Normal,
                [MoveType.Stellar] = MoveEffectiveness.Normal,
                [MoveType.Water] = MoveEffectiveness.SuperEffective,
            },
            HpIvs = new Dictionary<StatType, int>
            {
                [StatType.SpA] = 30,
                [StatType.SpD] = 30,
            },
            HpDvs = new Dictionary<StatType, int>
            {
                [StatType.Atk] = 12,
            },
        },
        [PokemonType.Flying] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, MoveEffectiveness>
            {
                [MoveType.Bug] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Dark] = MoveEffectiveness.Normal,
                [MoveType.Dragon] = MoveEffectiveness.Normal,
                [MoveType.Electric] = MoveEffectiveness.SuperEffective,
                [MoveType.Fairy] = MoveEffectiveness.Normal,
                [MoveType.Fighting] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Fire] = MoveEffectiveness.Normal,
                [MoveType.Flying] = MoveEffectiveness.Normal,
                [MoveType.Ghost] = MoveEffectiveness.Normal,
                [MoveType.Grass] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Ground] = MoveEffectiveness.Immune,
                [MoveType.Ice] = MoveEffectiveness.SuperEffective,
                [MoveType.Normal] = MoveEffectiveness.Normal,
                [MoveType.Poison] = MoveEffectiveness.Normal,
                [MoveType.Psychic] = MoveEffectiveness.Normal,
                [MoveType.Rock] = MoveEffectiveness.SuperEffective,
                [MoveType.Steel] = MoveEffectiveness.Normal,
                [MoveType.Stellar] = MoveEffectiveness.Normal,
                [MoveType.Water] = MoveEffectiveness.Normal,
            },
            HpIvs = new Dictionary<StatType, int>
            {
                [StatType.Hp] = 30,
                [StatType.Atk] = 30,
                [StatType.Def] = 30,
                [StatType.SpA] = 30,
                [StatType.SpD] = 30,
            },
            HpDvs = new Dictionary<StatType, int>
            {
                [StatType.Atk] = 12,
                [StatType.Def] = 13,
            },
        },
        [PokemonType.Psychic] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, MoveEffectiveness>
            {
                [MoveType.Bug] = MoveEffectiveness.SuperEffective,
                [MoveType.Dark] = MoveEffectiveness.SuperEffective,
                [MoveType.Dragon] = MoveEffectiveness.Normal,
                [MoveType.Electric] = MoveEffectiveness.Normal,
                [MoveType.Fairy] = MoveEffectiveness.Normal,
                [MoveType.Fighting] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Fire] = MoveEffectiveness.Normal,
                [MoveType.Flying] = MoveEffectiveness.Normal,
                [MoveType.Ghost] = MoveEffectiveness.SuperEffective,
                [MoveType.Grass] = MoveEffectiveness.Normal,
                [MoveType.Ground] = MoveEffectiveness.Normal,
                [MoveType.Ice] = MoveEffectiveness.Normal,
                [MoveType.Normal] = MoveEffectiveness.Normal,
                [MoveType.Poison] = MoveEffectiveness.Normal,
                [MoveType.Psychic] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Rock] = MoveEffectiveness.Normal,
                [MoveType.Steel] = MoveEffectiveness.Normal,
                [MoveType.Stellar] = MoveEffectiveness.Normal,
                [MoveType.Water] = MoveEffectiveness.Normal,
            },
            HpIvs = new Dictionary<StatType, int>
            {
                [StatType.Atk] = 30,
                [StatType.Spe] = 30,
            },
            HpDvs = new Dictionary<StatType, int>
            {
                [StatType.Def] = 12,
            },
        },
        [PokemonType.Rock] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, MoveEffectiveness>
            {
                [MoveType.Bug] = MoveEffectiveness.Normal,
                [MoveType.Dark] = MoveEffectiveness.Normal,
                [MoveType.Dragon] = MoveEffectiveness.Normal,
                [MoveType.Electric] = MoveEffectiveness.Normal,
                [MoveType.Fairy] = MoveEffectiveness.Normal,
                [MoveType.Fighting] = MoveEffectiveness.SuperEffective,
                [MoveType.Fire] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Flying] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Ghost] = MoveEffectiveness.Normal,
                [MoveType.Grass] = MoveEffectiveness.SuperEffective,
                [MoveType.Ground] = MoveEffectiveness.SuperEffective,
                [MoveType.Ice] = MoveEffectiveness.Normal,
                [MoveType.Normal] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Poison] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Psychic] = MoveEffectiveness.Normal,
                [MoveType.Rock] = MoveEffectiveness.Normal,
                [MoveType.Steel] = MoveEffectiveness.SuperEffective,
                [MoveType.Stellar] = MoveEffectiveness.Normal,
                [MoveType.Water] = MoveEffectiveness.SuperEffective,
            },
            HpIvs = new Dictionary<StatType, int>
            {
                [StatType.Def] = 30,
                [StatType.SpD] = 30,
                [StatType.Spe] = 30,
            },
            HpDvs = new Dictionary<StatType, int>
            {
                [StatType.Atk] = 13,
                [StatType.Def] = 12,
            },
        },
        [PokemonType.Ghost] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, MoveEffectiveness>
            {
                [MoveType.Bug] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Dark] = MoveEffectiveness.SuperEffective,
                [MoveType.Dragon] = MoveEffectiveness.Normal,
                [MoveType.Electric] = MoveEffectiveness.Normal,
                [MoveType.Fairy] = MoveEffectiveness.Normal,
                [MoveType.Fighting] = MoveEffectiveness.Immune,
                [MoveType.Fire] = MoveEffectiveness.Normal,
                [MoveType.Flying] = MoveEffectiveness.Normal,
                [MoveType.Ghost] = MoveEffectiveness.SuperEffective,
                [MoveType.Grass] = MoveEffectiveness.Normal,
                [MoveType.Ground] = MoveEffectiveness.Normal,
                [MoveType.Ice] = MoveEffectiveness.Normal,
                [MoveType.Normal] = MoveEffectiveness.Immune,
                [MoveType.Poison] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Psychic] = MoveEffectiveness.Normal,
                [MoveType.Rock] = MoveEffectiveness.Normal,
                [MoveType.Steel] = MoveEffectiveness.Normal,
                [MoveType.Stellar] = MoveEffectiveness.Normal,
                [MoveType.Water] = MoveEffectiveness.Normal,
            },
            HpIvs = new Dictionary<StatType, int>
            {
                [StatType.Def] = 30,
                [StatType.SpD] = 30,
            },
            HpDvs = new Dictionary<StatType, int>
            {
                [StatType.Atk] = 13,
                [StatType.Def] = 14,
            },
        },
        [PokemonType.Dragon] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, MoveEffectiveness>
            {
                [MoveType.Bug] = MoveEffectiveness.Normal,
                [MoveType.Dark] = MoveEffectiveness.Normal,
                [MoveType.Dragon] = MoveEffectiveness.SuperEffective,
                [MoveType.Electric] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Fairy] = MoveEffectiveness.SuperEffective,
                [MoveType.Fighting] = MoveEffectiveness.Normal,
                [MoveType.Fire] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Flying] = MoveEffectiveness.Normal,
                [MoveType.Ghost] = MoveEffectiveness.Normal,
                [MoveType.Grass] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Ground] = MoveEffectiveness.Normal,
                [MoveType.Ice] = MoveEffectiveness.SuperEffective,
                [MoveType.Normal] = MoveEffectiveness.Normal,
                [MoveType.Poison] = MoveEffectiveness.Normal,
                [MoveType.Psychic] = MoveEffectiveness.Normal,
                [MoveType.Rock] = MoveEffectiveness.Normal,
                [MoveType.Steel] = MoveEffectiveness.Normal,
                [MoveType.Stellar] = MoveEffectiveness.Normal,
                [MoveType.Water] = MoveEffectiveness.NotVeryEffective,
            },
            HpIvs = new Dictionary<StatType, int>
            {
                [StatType.Atk] = 30,
            },
            HpDvs = new Dictionary<StatType, int>
            {
                [StatType.Def] = 14,
            },
        },
        [PokemonType.Dark] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, MoveEffectiveness>
            {
                [MoveType.Bug] = MoveEffectiveness.SuperEffective,
                [MoveType.Dark] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Dragon] = MoveEffectiveness.Normal,
                [MoveType.Electric] = MoveEffectiveness.Normal,
                [MoveType.Fairy] = MoveEffectiveness.SuperEffective,
                [MoveType.Fighting] = MoveEffectiveness.SuperEffective,
                [MoveType.Fire] = MoveEffectiveness.Normal,
                [MoveType.Flying] = MoveEffectiveness.Normal,
                [MoveType.Ghost] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Grass] = MoveEffectiveness.Normal,
                [MoveType.Ground] = MoveEffectiveness.Normal,
                [MoveType.Ice] = MoveEffectiveness.Normal,
                [MoveType.Normal] = MoveEffectiveness.Normal,
                [MoveType.Poison] = MoveEffectiveness.Normal,
                [MoveType.Psychic] = MoveEffectiveness.Immune,
                [MoveType.Rock] = MoveEffectiveness.Normal,
                [MoveType.Steel] = MoveEffectiveness.Normal,
                [MoveType.Stellar] = MoveEffectiveness.Normal,
                [MoveType.Water] = MoveEffectiveness.Normal,
            },
            HpIvs = [],
        },
        [PokemonType.Steel] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, MoveEffectiveness>
            {
                [MoveType.Bug] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Dark] = MoveEffectiveness.Normal,
                [MoveType.Dragon] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Electric] = MoveEffectiveness.Normal,
                [MoveType.Fairy] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Fighting] = MoveEffectiveness.SuperEffective,
                [MoveType.Fire] = MoveEffectiveness.SuperEffective,
                [MoveType.Flying] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Ghost] = MoveEffectiveness.Normal,
                [MoveType.Grass] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Ground] = MoveEffectiveness.SuperEffective,
                [MoveType.Ice] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Normal] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Poison] = MoveEffectiveness.Immune,
                [MoveType.Psychic] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Rock] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Steel] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Stellar] = MoveEffectiveness.Normal,
                [MoveType.Water] = MoveEffectiveness.Normal,
            },
            HpIvs = new Dictionary<StatType, int>
            {
                [StatType.SpD] = 30,
            },
            HpDvs = new Dictionary<StatType, int>
            {
                [StatType.Atk] = 13,
            },
        },
        [PokemonType.Fairy] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, MoveEffectiveness>
            {
                [MoveType.Bug] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Dark] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Dragon] = MoveEffectiveness.Immune,
                [MoveType.Electric] = MoveEffectiveness.Normal,
                [MoveType.Fairy] = MoveEffectiveness.Normal,
                [MoveType.Fighting] = MoveEffectiveness.NotVeryEffective,
                [MoveType.Fire] = MoveEffectiveness.Normal,
                [MoveType.Flying] = MoveEffectiveness.Normal,
                [MoveType.Ghost] = MoveEffectiveness.Normal,
                [MoveType.Grass] = MoveEffectiveness.Normal,
                [MoveType.Ground] = MoveEffectiveness.Normal,
                [MoveType.Ice] = MoveEffectiveness.Normal,
                [MoveType.Normal] = MoveEffectiveness.Normal,
                [MoveType.Poison] = MoveEffectiveness.SuperEffective,
                [MoveType.Psychic] = MoveEffectiveness.Normal,
                [MoveType.Rock] = MoveEffectiveness.Normal,
                [MoveType.Steel] = MoveEffectiveness.SuperEffective,
                [MoveType.Stellar] = MoveEffectiveness.Normal,
                [MoveType.Water] = MoveEffectiveness.Normal,
            },
        },
        [PokemonType.Normal] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, MoveEffectiveness>
            {
                [MoveType.Bug] = MoveEffectiveness.Normal,
                [MoveType.Dark] = MoveEffectiveness.Normal,
                [MoveType.Dragon] = MoveEffectiveness.Normal,
                [MoveType.Electric] = MoveEffectiveness.Normal,
                [MoveType.Fairy] = MoveEffectiveness.Normal,
                [MoveType.Fighting] = MoveEffectiveness.SuperEffective,
                [MoveType.Fire] = MoveEffectiveness.Normal,
                [MoveType.Flying] = MoveEffectiveness.Normal,
                [MoveType.Ghost] = MoveEffectiveness.Immune,
                [MoveType.Grass] = MoveEffectiveness.Normal,
                [MoveType.Ground] = MoveEffectiveness.Normal,
                [MoveType.Ice] = MoveEffectiveness.Normal,
                [MoveType.Normal] = MoveEffectiveness.Normal,
                [MoveType.Poison] = MoveEffectiveness.Normal,
                [MoveType.Psychic] = MoveEffectiveness.Normal,
                [MoveType.Rock] = MoveEffectiveness.Normal,
                [MoveType.Steel] = MoveEffectiveness.Normal,
                [MoveType.Stellar] = MoveEffectiveness.Normal,
                [MoveType.Water] = MoveEffectiveness.Normal,
            },
        },
        [PokemonType.Unknown] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, MoveEffectiveness>
            {
                [MoveType.Bug] = MoveEffectiveness.Normal,
                [MoveType.Dark] = MoveEffectiveness.Normal,
                [MoveType.Dragon] = MoveEffectiveness.Normal,
                [MoveType.Electric] = MoveEffectiveness.Normal,
                [MoveType.Fairy] = MoveEffectiveness.Normal,
                [MoveType.Fighting] = MoveEffectiveness.Normal,
                [MoveType.Fire] = MoveEffectiveness.Normal,
                [MoveType.Flying] = MoveEffectiveness.Normal,
                [MoveType.Ghost] = MoveEffectiveness.Normal,
                [MoveType.Grass] = MoveEffectiveness.Normal,
                [MoveType.Ground] = MoveEffectiveness.Normal,
                [MoveType.Ice] = MoveEffectiveness.Normal,
                [MoveType.Normal] = MoveEffectiveness.Normal,
                [MoveType.Poison] = MoveEffectiveness.Normal,
                [MoveType.Psychic] = MoveEffectiveness.Normal,
                [MoveType.Rock] = MoveEffectiveness.Normal,
                [MoveType.Steel] = MoveEffectiveness.Normal,
                [MoveType.Stellar] = MoveEffectiveness.Normal,
                [MoveType.Water] = MoveEffectiveness.Normal,
            },
            IsNonstandard = Nonstandard.Past,
        },
    };
}