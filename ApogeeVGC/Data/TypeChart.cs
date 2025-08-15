using System.Collections.ObjectModel;
using ApogeeVGC.Sim;

namespace ApogeeVGC.Data;

public record TypeChart
{
    public IReadOnlyDictionary<PokemonType, TypeData> TypeData { get; }

    public TypeChart()
    {
        TypeData = new ReadOnlyDictionary<PokemonType, TypeData>(_typeData);
    }

    private readonly Dictionary<PokemonType, TypeData> _typeData = new()
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
            HpIvs = new Dictionary<StatId, int>
            {
                [StatId.Atk] = 30,
                [StatId.Def] = 30,
                [StatId.SpD] = 30,
            },
            HpDvs = new Dictionary<StatId, int>
            {
                [StatId.Atk] = 13,
                [StatId.Def] = 13,
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
            HpIvs = new Dictionary<StatId, int>
            {
                [StatId.Atk] = 30,
                [StatId.SpA] = 30,
                [StatId.Spe] = 30,
            },
            HpDvs = new Dictionary<StatId, int>
            {
                [StatId.Atk] = 14,
                [StatId.Def] = 12,
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
            HpIvs = new Dictionary<StatId, int>
            {
                [StatId.Atk] = 30,
                [StatId.Def] = 30,
                [StatId.SpA] = 30,
            },
            HpDvs = new Dictionary<StatId, int>
            {
                [StatId.Atk] = 14,
                [StatId.Def] = 13,
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
            HpIvs = new Dictionary<StatId, int>
            {
                [StatId.SpA] = 30,
            },
            HpDvs = new Dictionary<StatId, int>
            {
                [StatId.Atk] = 14,
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
            HpIvs = new Dictionary<StatId, int>
            {
                [StatId.Atk] = 30,
                [StatId.SpA] = 30,
            },
            HpDvs = new Dictionary<StatId, int>
            {
                [StatId.Atk] = 14,
                [StatId.Def] = 14,
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
            HpIvs = new Dictionary<StatId, int>
            {
                [StatId.Atk] = 30,
                [StatId.Def] = 30,
            },
            HpDvs = new Dictionary<StatId, int>
            {
                [StatId.Def] = 13,
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
            HpIvs = new Dictionary<StatId, int>
            {
                [StatId.Def] = 30,
                [StatId.SpA] = 30,
                [StatId.SpD] = 30,
                [StatId.Spe] = 30,
            },
            HpDvs = new Dictionary<StatId, int>
            {
                [StatId.Atk] = 12,
                [StatId.Def] = 12,
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
            HpIvs = new Dictionary<StatId, int>
            {
                [StatId.Def] = 30,
                [StatId.SpA] = 30,
                [StatId.SpD] = 30,
            },
            HpDvs = new Dictionary<StatId, int>
            {
                [StatId.Atk] = 12,
                [StatId.Def] = 14,
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
            HpIvs = new Dictionary<StatId, int>
            {
                [StatId.SpA] = 30,
                [StatId.SpD] = 30,
            },
            HpDvs = new Dictionary<StatId, int>
            {
                [StatId.Atk] = 12,
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
            HpIvs = new Dictionary<StatId, int>
            {
                [StatId.Hp] = 30,
                [StatId.Atk] = 30,
                [StatId.Def] = 30,
                [StatId.SpA] = 30,
                [StatId.SpD] = 30,
            },
            HpDvs = new Dictionary<StatId, int>
            {
                [StatId.Atk] = 12,
                [StatId.Def] = 13,
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
            HpIvs = new Dictionary<StatId, int>
            {
                [StatId.Atk] = 30,
                [StatId.Spe] = 30,
            },
            HpDvs = new Dictionary<StatId, int>
            {
                [StatId.Def] = 12,
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
            HpIvs = new Dictionary<StatId, int>
            {
                [StatId.Def] = 30,
                [StatId.SpD] = 30,
                [StatId.Spe] = 30,
            },
            HpDvs = new Dictionary<StatId, int>
            {
                [StatId.Atk] = 13,
                [StatId.Def] = 12,
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
            HpIvs = new Dictionary<StatId, int>
            {
                [StatId.Def] = 30,
                [StatId.SpD] = 30,
            },
            HpDvs = new Dictionary<StatId, int>
            {
                [StatId.Atk] = 13,
                [StatId.Def] = 14,
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
            HpIvs = new Dictionary<StatId, int>
            {
                [StatId.Atk] = 30,
            },
            HpDvs = new Dictionary<StatId, int>
            {
                [StatId.Def] = 14,
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
            HpIvs = new Dictionary<StatId, int>(),
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
            HpIvs = new Dictionary<StatId, int>
            {
                [StatId.SpD] = 30,
            },
            HpDvs = new Dictionary<StatId, int>
            {
                [StatId.Atk] = 13,
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