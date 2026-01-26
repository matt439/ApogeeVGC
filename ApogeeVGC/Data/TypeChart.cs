using System.Collections.ObjectModel;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Types;
using ApogeeVGC.Sim.Utils.Extensions;

namespace ApogeeVGC.Data;

public enum MoveEffectiveness
{
    Normal = 0,
    SuperEffective2X = 1,
    NotVeryEffective05X = 2,
    Immune = 3,
    SuperEffective4X = 4,
    NotVeryEffective025X = 5,
}

public record TypeChart
{
    public IReadOnlyDictionary<PokemonType, TypeData> TypeData { get; }

    public TypeChart()
    {
        TypeData = new ReadOnlyDictionary<PokemonType, TypeData>(_typeData);
    }

    public MoveEffectiveness GetMoveEffectiveness(PokemonType pokemon, MoveType moveType)
    {
        var typeEffectiveness =  _typeData[pokemon].DamageTaken[moveType];
        return typeEffectiveness.ConvertToMoveEffectiveness();
    }

    public MoveEffectiveness GetMoveEffectiveness(IReadOnlyList<PokemonType> pokemon, MoveType moveType)
    {
        return pokemon.Count switch
        {
            0 => throw new ArgumentException("Pokemon type list cannot be empty.", nameof(pokemon)),
            1 => GetMoveEffectiveness(pokemon[0], moveType),
            > 2 => throw new ArgumentException("Pokemon type list can only contain up to two types.",
                nameof(pokemon)),
            _ => CombineTypeEffectivenesses(_typeData[pokemon[0]].DamageTaken[moveType],
                _typeData[pokemon[1]].DamageTaken[moveType]),
        };
    }

    public MoveEffectiveness GetMoveEffectiveness(PokemonType[] pokemon, MoveType moveType)
    {
        return GetMoveEffectiveness(pokemon.ToList(), moveType);
    }

    //public MoveEffectiveness GetSpecialEffectiveness(PokemonType pokemon, SpecialImmunityId immunity)
    //{
    //    TypeEffectiveness typeEffectiveness = _typeData[pokemon].SpecialDamageTaken[immunity];
    //    return typeEffectiveness.ConvertToMoveEffectiveness();
    //}

    private static MoveEffectiveness CombineTypeEffectivenesses(TypeEffectiveness effectiveness1,
        TypeEffectiveness effectiveness2)
    {
        if (effectiveness1 == TypeEffectiveness.Immune ||
            effectiveness2 == TypeEffectiveness.Immune)
            return MoveEffectiveness.Immune;

        if (effectiveness1 == TypeEffectiveness.Normal)
            return effectiveness2.ConvertToMoveEffectiveness();

        if (effectiveness2 == TypeEffectiveness.Normal)
            return effectiveness1.ConvertToMoveEffectiveness();

        switch (effectiveness1)
        {
            case TypeEffectiveness.SuperEffective:
                switch (effectiveness2)
                {
                    case TypeEffectiveness.SuperEffective:
                        return MoveEffectiveness.SuperEffective4X;
                    case TypeEffectiveness.NotVeryEffective:
                        return MoveEffectiveness.Normal;
                    case TypeEffectiveness.Normal:
                    case TypeEffectiveness.Immune:
                    default:
                        throw new InvalidOperationException("Invalid type effectiveness combination.");
                }
            case TypeEffectiveness.NotVeryEffective:
                switch (effectiveness2)
                {
                    case TypeEffectiveness.SuperEffective:
                        return MoveEffectiveness.Normal;
                    case TypeEffectiveness.NotVeryEffective:
                        return MoveEffectiveness.NotVeryEffective025X;
                    case TypeEffectiveness.Normal:
                    case TypeEffectiveness.Immune:
                    default:
                        throw new InvalidOperationException("Invalid type effectiveness combination.");
                }
            case TypeEffectiveness.Normal:
            case TypeEffectiveness.Immune:
            default:
                throw new InvalidOperationException("Invalid type effectiveness combination.");
        }
    }

    private readonly Dictionary<PokemonType, TypeData> _typeData = new()
    {
        [PokemonType.Bug] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, TypeEffectiveness>
            {
                [MoveType.Bug] = TypeEffectiveness.Normal,
                [MoveType.Dark] = TypeEffectiveness.Normal,
                [MoveType.Dragon] = TypeEffectiveness.Normal,
                [MoveType.Electric] = TypeEffectiveness.Normal,
                [MoveType.Fairy] = TypeEffectiveness.Normal,
                [MoveType.Fighting] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Fire] = TypeEffectiveness.SuperEffective,
                [MoveType.Flying] = TypeEffectiveness.SuperEffective,
                [MoveType.Ghost] = TypeEffectiveness.Normal,
                [MoveType.Grass] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Ground] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Ice] = TypeEffectiveness.Normal,
                [MoveType.Normal] = TypeEffectiveness.Normal,
                [MoveType.Poison] = TypeEffectiveness.Normal,
                [MoveType.Psychic] = TypeEffectiveness.Normal,
                [MoveType.Rock] = TypeEffectiveness.SuperEffective,
                [MoveType.Steel] = TypeEffectiveness.Normal,
                [MoveType.Stellar] = TypeEffectiveness.Normal,
                [MoveType.Water] = TypeEffectiveness.Normal,

                [MoveType.Unknown] = TypeEffectiveness.Normal,
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
            //SpecialDamageTaken = new Dictionary<SpecialImmunityId, TypeEffectiveness>
            //{
            //    [SpecialImmunityId.Prankster] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Paralysis] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Burn ] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Trapped] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Powder] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Sandstorm] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Hail] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Freeze] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Poison] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Toxic] = TypeEffectiveness.Normal,
            //},
        },
        [PokemonType.Fire] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, TypeEffectiveness>
            {
                [MoveType.Bug] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Dark] = TypeEffectiveness.Normal,
                [MoveType.Dragon] = TypeEffectiveness.Normal,
                [MoveType.Electric] = TypeEffectiveness.Normal,
                [MoveType.Fairy] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Fighting] = TypeEffectiveness.Normal,
                [MoveType.Fire] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Flying] = TypeEffectiveness.Normal,
                [MoveType.Ghost] = TypeEffectiveness.Normal,
                [MoveType.Grass] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Ground] = TypeEffectiveness.SuperEffective,
                [MoveType.Ice] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Normal] = TypeEffectiveness.Normal,
                [MoveType.Poison] = TypeEffectiveness.Normal,
                [MoveType.Psychic] = TypeEffectiveness.Normal,
                [MoveType.Rock] = TypeEffectiveness.SuperEffective,
                [MoveType.Steel] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Stellar] = TypeEffectiveness.Normal,
                [MoveType.Water] = TypeEffectiveness.SuperEffective,

                [MoveType.Unknown] = TypeEffectiveness.Normal,
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
            //SpecialDamageTaken = new Dictionary<SpecialImmunityId, TypeEffectiveness>
            //{
            //    [SpecialImmunityId.Prankster] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Paralysis] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Burn] = TypeEffectiveness.Immune,
            //    [SpecialImmunityId.Trapped] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Powder] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Sandstorm] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Hail] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Freeze] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Poison] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Toxic] = TypeEffectiveness.Normal,
            //},
        },
        [PokemonType.Water] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, TypeEffectiveness>
            {
                [MoveType.Bug] = TypeEffectiveness.Normal,
                [MoveType.Dark] = TypeEffectiveness.Normal,
                [MoveType.Dragon] = TypeEffectiveness.Normal,
                [MoveType.Electric] = TypeEffectiveness.SuperEffective,
                [MoveType.Fairy] = TypeEffectiveness.Normal,
                [MoveType.Fighting] = TypeEffectiveness.Normal,
                [MoveType.Fire] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Flying] = TypeEffectiveness.Normal,
                [MoveType.Ghost] = TypeEffectiveness.Normal,
                [MoveType.Grass] = TypeEffectiveness.SuperEffective,
                [MoveType.Ground] = TypeEffectiveness.Normal,
                [MoveType.Ice] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Normal] = TypeEffectiveness.Normal,
                [MoveType.Poison] = TypeEffectiveness.Normal,
                [MoveType.Psychic] = TypeEffectiveness.Normal,
                [MoveType.Rock] = TypeEffectiveness.Normal,
                [MoveType.Steel] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Stellar] = TypeEffectiveness.Normal,
                [MoveType.Water] = TypeEffectiveness.NotVeryEffective,

                [MoveType.Unknown] = TypeEffectiveness.Normal,
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
            //SpecialDamageTaken = new Dictionary<SpecialImmunityId, TypeEffectiveness>
            //{
            //    [SpecialImmunityId.Prankster] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Paralysis] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Burn] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Trapped] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Powder] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Sandstorm] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Hail] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Freeze] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Poison] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Toxic] = TypeEffectiveness.Normal,
            //},
        },
        [PokemonType.Electric] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, TypeEffectiveness>
            {
                [MoveType.Bug] = TypeEffectiveness.Normal,
                [MoveType.Dark] = TypeEffectiveness.Normal,
                [MoveType.Dragon] = TypeEffectiveness.Normal,
                [MoveType.Electric] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Fairy] = TypeEffectiveness.Normal,
                [MoveType.Fighting] = TypeEffectiveness.Normal,
                [MoveType.Fire] = TypeEffectiveness.Normal,
                [MoveType.Flying] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Ghost] = TypeEffectiveness.Normal,
                [MoveType.Grass] = TypeEffectiveness.Normal,
                [MoveType.Ground] = TypeEffectiveness.SuperEffective,
                [MoveType.Ice] = TypeEffectiveness.Normal,
                [MoveType.Normal] = TypeEffectiveness.Normal,
                [MoveType.Poison] = TypeEffectiveness.Normal,
                [MoveType.Psychic] = TypeEffectiveness.Normal,
                [MoveType.Rock] = TypeEffectiveness.Normal,
                [MoveType.Steel] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Stellar] = TypeEffectiveness.Normal,
                [MoveType.Water] = TypeEffectiveness.Normal,

                [MoveType.Unknown] = TypeEffectiveness.Normal,
            },
            HpIvs = new Dictionary<StatId, int>
            {
                [StatId.SpA] = 30,
            },
            HpDvs = new Dictionary<StatId, int>
            {
                [StatId.Atk] = 14,
            },
            //SpecialDamageTaken = new Dictionary<SpecialImmunityId, TypeEffectiveness>
            //{
            //    [SpecialImmunityId.Prankster] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Paralysis] = TypeEffectiveness.Immune,
            //    [SpecialImmunityId.Burn] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Trapped] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Powder] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Sandstorm] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Hail] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Freeze] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Poison] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Toxic] = TypeEffectiveness.Normal,
            //},
        },
        [PokemonType.Grass] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, TypeEffectiveness>
            {
                [MoveType.Bug] = TypeEffectiveness.SuperEffective,
                [MoveType.Dark] = TypeEffectiveness.Normal,
                [MoveType.Dragon] = TypeEffectiveness.Normal,
                [MoveType.Electric] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Fairy] = TypeEffectiveness.Normal,
                [MoveType.Fighting] = TypeEffectiveness.Normal,
                [MoveType.Fire] = TypeEffectiveness.SuperEffective,
                [MoveType.Flying] = TypeEffectiveness.SuperEffective,
                [MoveType.Ghost] = TypeEffectiveness.Normal,
                [MoveType.Grass] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Ground] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Ice] = TypeEffectiveness.SuperEffective,
                [MoveType.Normal] = TypeEffectiveness.Normal,
                [MoveType.Poison] = TypeEffectiveness.SuperEffective,
                [MoveType.Psychic] = TypeEffectiveness.Normal,
                [MoveType.Rock] = TypeEffectiveness.Normal,
                [MoveType.Steel] = TypeEffectiveness.Normal,
                [MoveType.Stellar] = TypeEffectiveness.Normal,
                [MoveType.Water] = TypeEffectiveness.NotVeryEffective,

                [MoveType.Unknown] = TypeEffectiveness.Normal,
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
            //SpecialDamageTaken = new Dictionary<SpecialImmunityId, TypeEffectiveness>
            //{
            //    [SpecialImmunityId.Prankster] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Paralysis] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Burn] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Trapped] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Powder] = TypeEffectiveness.Immune,
            //    [SpecialImmunityId.Sandstorm] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Hail] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Freeze] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Poison] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Toxic] = TypeEffectiveness.Normal,
            //},
        },
        [PokemonType.Ice] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, TypeEffectiveness>
            {
                [MoveType.Bug] = TypeEffectiveness.Normal,
                [MoveType.Dark] = TypeEffectiveness.Normal,
                [MoveType.Dragon] = TypeEffectiveness.Normal,
                [MoveType.Electric] = TypeEffectiveness.Normal,
                [MoveType.Fairy] = TypeEffectiveness.Normal,
                [MoveType.Fighting] = TypeEffectiveness.SuperEffective,
                [MoveType.Fire] = TypeEffectiveness.SuperEffective,
                [MoveType.Flying] = TypeEffectiveness.Normal,
                [MoveType.Ghost] = TypeEffectiveness.Normal,
                [MoveType.Grass] = TypeEffectiveness.Normal,
                [MoveType.Ground] = TypeEffectiveness.Normal,
                [MoveType.Ice] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Normal] = TypeEffectiveness.Normal,
                [MoveType.Poison] = TypeEffectiveness.Normal,
                [MoveType.Psychic] = TypeEffectiveness.Normal,
                [MoveType.Rock] = TypeEffectiveness.SuperEffective,
                [MoveType.Steel] = TypeEffectiveness.SuperEffective,
                [MoveType.Stellar] = TypeEffectiveness.Normal,
                [MoveType.Water] = TypeEffectiveness.Normal,

                [MoveType.Unknown] = TypeEffectiveness.Normal,
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
            //SpecialDamageTaken = new Dictionary<SpecialImmunityId, TypeEffectiveness>
            //{
            //    [SpecialImmunityId.Prankster] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Paralysis] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Burn] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Trapped] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Powder] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Sandstorm] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Hail] = TypeEffectiveness.Immune,
            //    [SpecialImmunityId.Freeze] = TypeEffectiveness.Immune,
            //    [SpecialImmunityId.Poison] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Toxic] = TypeEffectiveness.Normal,
            //},
        },
        [PokemonType.Fighting] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, TypeEffectiveness>
            {
                [MoveType.Bug] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Dark] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Dragon] = TypeEffectiveness.Normal,
                [MoveType.Electric] = TypeEffectiveness.Normal,
                [MoveType.Fairy] = TypeEffectiveness.SuperEffective,
                [MoveType.Fighting] = TypeEffectiveness.Normal,
                [MoveType.Fire] = TypeEffectiveness.Normal,
                [MoveType.Flying] = TypeEffectiveness.SuperEffective,
                [MoveType.Ghost] = TypeEffectiveness.Normal,
                [MoveType.Grass] = TypeEffectiveness.Normal,
                [MoveType.Ground] = TypeEffectiveness.Normal,
                [MoveType.Ice] = TypeEffectiveness.Normal,
                [MoveType.Normal] = TypeEffectiveness.Normal,
                [MoveType.Poison] = TypeEffectiveness.Normal,
                [MoveType.Psychic] = TypeEffectiveness.SuperEffective,
                [MoveType.Rock] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Steel] = TypeEffectiveness.Normal,
                [MoveType.Stellar] = TypeEffectiveness.Normal,
                [MoveType.Water] = TypeEffectiveness.Normal,

                [MoveType.Unknown] = TypeEffectiveness.Normal,
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
            //SpecialDamageTaken = new Dictionary<SpecialImmunityId, TypeEffectiveness>
            //{
            //    [SpecialImmunityId.Prankster] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Paralysis] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Burn] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Trapped] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Powder] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Sandstorm] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Hail] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Freeze] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Poison] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Toxic] = TypeEffectiveness.Normal,
            //},
        },
        [PokemonType.Poison] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, TypeEffectiveness>
            {
                [MoveType.Bug] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Dark] = TypeEffectiveness.Normal,
                [MoveType.Dragon] = TypeEffectiveness.Normal,
                [MoveType.Electric] = TypeEffectiveness.Normal,
                [MoveType.Fairy] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Fighting] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Fire] = TypeEffectiveness.Normal,
                [MoveType.Flying] = TypeEffectiveness.Normal,
                [MoveType.Ghost] = TypeEffectiveness.Normal,
                [MoveType.Grass] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Ground] = TypeEffectiveness.SuperEffective,
                [MoveType.Ice] = TypeEffectiveness.Normal,
                [MoveType.Normal] = TypeEffectiveness.Normal,
                [MoveType.Poison] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Psychic] = TypeEffectiveness.SuperEffective,
                [MoveType.Rock] = TypeEffectiveness.Normal,
                [MoveType.Steel] = TypeEffectiveness.Normal,
                [MoveType.Stellar] = TypeEffectiveness.Normal,
                [MoveType.Water] = TypeEffectiveness.Normal,

                [MoveType.Unknown] = TypeEffectiveness.Normal,
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
            //SpecialDamageTaken = new Dictionary<SpecialImmunityId, TypeEffectiveness>
            //{
            //    [SpecialImmunityId.Prankster] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Paralysis] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Burn] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Trapped] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Powder] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Sandstorm] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Hail] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Freeze] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Poison] = TypeEffectiveness.Immune,
            //    [SpecialImmunityId.Toxic] = TypeEffectiveness.Immune,
            //},
        },
        [PokemonType.Ground] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, TypeEffectiveness>
            {
                [MoveType.Bug] = TypeEffectiveness.Normal,
                [MoveType.Dark] = TypeEffectiveness.Normal,
                [MoveType.Dragon] = TypeEffectiveness.Normal,
                [MoveType.Electric] = TypeEffectiveness.Immune,
                [MoveType.Fairy] = TypeEffectiveness.Normal,
                [MoveType.Fighting] = TypeEffectiveness.Normal,
                [MoveType.Fire] = TypeEffectiveness.Normal,
                [MoveType.Flying] = TypeEffectiveness.Normal,
                [MoveType.Ghost] = TypeEffectiveness.Normal,
                [MoveType.Grass] = TypeEffectiveness.SuperEffective,
                [MoveType.Ground] = TypeEffectiveness.Normal,
                [MoveType.Ice] = TypeEffectiveness.SuperEffective,
                [MoveType.Normal] = TypeEffectiveness.Normal,
                [MoveType.Poison] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Psychic] = TypeEffectiveness.Normal,
                [MoveType.Rock] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Steel] = TypeEffectiveness.Normal,
                [MoveType.Stellar] = TypeEffectiveness.Normal,
                [MoveType.Water] = TypeEffectiveness.SuperEffective,

                [MoveType.Unknown] = TypeEffectiveness.Normal,
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
            //SpecialDamageTaken = new Dictionary<SpecialImmunityId, TypeEffectiveness>
            //{
            //    [SpecialImmunityId.Prankster] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Paralysis] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Burn] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Trapped] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Powder] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Sandstorm] = TypeEffectiveness.Immune,
            //    [SpecialImmunityId.Hail] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Freeze] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Poison] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Toxic] = TypeEffectiveness.Normal,
            //},
        },
        [PokemonType.Flying] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, TypeEffectiveness>
            {
                [MoveType.Bug] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Dark] = TypeEffectiveness.Normal,
                [MoveType.Dragon] = TypeEffectiveness.Normal,
                [MoveType.Electric] = TypeEffectiveness.SuperEffective,
                [MoveType.Fairy] = TypeEffectiveness.Normal,
                [MoveType.Fighting] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Fire] = TypeEffectiveness.Normal,
                [MoveType.Flying] = TypeEffectiveness.Normal,
                [MoveType.Ghost] = TypeEffectiveness.Normal,
                [MoveType.Grass] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Ground] = TypeEffectiveness.Immune,
                [MoveType.Ice] = TypeEffectiveness.SuperEffective,
                [MoveType.Normal] = TypeEffectiveness.Normal,
                [MoveType.Poison] = TypeEffectiveness.Normal,
                [MoveType.Psychic] = TypeEffectiveness.Normal,
                [MoveType.Rock] = TypeEffectiveness.SuperEffective,
                [MoveType.Steel] = TypeEffectiveness.Normal,
                [MoveType.Stellar] = TypeEffectiveness.Normal,
                [MoveType.Water] = TypeEffectiveness.Normal,

                [MoveType.Unknown] = TypeEffectiveness.Normal,
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
            //SpecialDamageTaken = new Dictionary<SpecialImmunityId, TypeEffectiveness>
            //{
            //    [SpecialImmunityId.Prankster] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Paralysis] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Burn] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Trapped] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Powder] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Sandstorm] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Hail] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Freeze] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Poison] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Toxic] = TypeEffectiveness.Normal,
            //},
        },
        [PokemonType.Psychic] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, TypeEffectiveness>
            {
                [MoveType.Bug] = TypeEffectiveness.SuperEffective,
                [MoveType.Dark] = TypeEffectiveness.SuperEffective,
                [MoveType.Dragon] = TypeEffectiveness.Normal,
                [MoveType.Electric] = TypeEffectiveness.Normal,
                [MoveType.Fairy] = TypeEffectiveness.Normal,
                [MoveType.Fighting] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Fire] = TypeEffectiveness.Normal,
                [MoveType.Flying] = TypeEffectiveness.Normal,
                [MoveType.Ghost] = TypeEffectiveness.SuperEffective,
                [MoveType.Grass] = TypeEffectiveness.Normal,
                [MoveType.Ground] = TypeEffectiveness.Normal,
                [MoveType.Ice] = TypeEffectiveness.Normal,
                [MoveType.Normal] = TypeEffectiveness.Normal,
                [MoveType.Poison] = TypeEffectiveness.Normal,
                [MoveType.Psychic] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Rock] = TypeEffectiveness.Normal,
                [MoveType.Steel] = TypeEffectiveness.Normal,
                [MoveType.Stellar] = TypeEffectiveness.Normal,
                [MoveType.Water] = TypeEffectiveness.Normal,

                [MoveType.Unknown] = TypeEffectiveness.Normal,
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
            //SpecialDamageTaken = new Dictionary<SpecialImmunityId, TypeEffectiveness>
            //{
            //    [SpecialImmunityId.Prankster] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Paralysis] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Burn] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Trapped] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Powder] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Sandstorm] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Hail] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Freeze] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Poison] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Toxic] = TypeEffectiveness.Normal,
            //},
        },
        [PokemonType.Rock] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, TypeEffectiveness>
            {
                [MoveType.Bug] = TypeEffectiveness.Normal,
                [MoveType.Dark] = TypeEffectiveness.Normal,
                [MoveType.Dragon] = TypeEffectiveness.Normal,
                [MoveType.Electric] = TypeEffectiveness.Normal,
                [MoveType.Fairy] = TypeEffectiveness.Normal,
                [MoveType.Fighting] = TypeEffectiveness.SuperEffective,
                [MoveType.Fire] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Flying] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Ghost] = TypeEffectiveness.Normal,
                [MoveType.Grass] = TypeEffectiveness.SuperEffective,
                [MoveType.Ground] = TypeEffectiveness.SuperEffective,
                [MoveType.Ice] = TypeEffectiveness.Normal,
                [MoveType.Normal] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Poison] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Psychic] = TypeEffectiveness.Normal,
                [MoveType.Rock] = TypeEffectiveness.Normal,
                [MoveType.Steel] = TypeEffectiveness.SuperEffective,
                [MoveType.Stellar] = TypeEffectiveness.Normal,
                [MoveType.Water] = TypeEffectiveness.SuperEffective,

                [MoveType.Unknown] = TypeEffectiveness.Normal,
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
            //SpecialDamageTaken = new Dictionary<SpecialImmunityId, TypeEffectiveness>
            //{
            //    [SpecialImmunityId.Prankster] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Paralysis] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Burn] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Trapped] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Powder] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Sandstorm] = TypeEffectiveness.Immune,
            //    [SpecialImmunityId.Hail] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Freeze] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Poison] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Toxic] = TypeEffectiveness.Normal,
            //},
        },
        [PokemonType.Ghost] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, TypeEffectiveness>
            {
                [MoveType.Bug] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Dark] = TypeEffectiveness.SuperEffective,
                [MoveType.Dragon] = TypeEffectiveness.Normal,
                [MoveType.Electric] = TypeEffectiveness.Normal,
                [MoveType.Fairy] = TypeEffectiveness.Normal,
                [MoveType.Fighting] = TypeEffectiveness.Immune,
                [MoveType.Fire] = TypeEffectiveness.Normal,
                [MoveType.Flying] = TypeEffectiveness.Normal,
                [MoveType.Ghost] = TypeEffectiveness.SuperEffective,
                [MoveType.Grass] = TypeEffectiveness.Normal,
                [MoveType.Ground] = TypeEffectiveness.Normal,
                [MoveType.Ice] = TypeEffectiveness.Normal,
                [MoveType.Normal] = TypeEffectiveness.Immune,
                [MoveType.Poison] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Psychic] = TypeEffectiveness.Normal,
                [MoveType.Rock] = TypeEffectiveness.Normal,
                [MoveType.Steel] = TypeEffectiveness.Normal,
                [MoveType.Stellar] = TypeEffectiveness.Normal,
                [MoveType.Water] = TypeEffectiveness.Normal,

                [MoveType.Unknown] = TypeEffectiveness.Normal,
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
            //SpecialDamageTaken = new Dictionary<SpecialImmunityId, TypeEffectiveness>
            //{
            //    [SpecialImmunityId.Prankster] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Paralysis] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Burn] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Trapped] = TypeEffectiveness.Immune,
            //    [SpecialImmunityId.Powder] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Sandstorm] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Hail] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Freeze] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Poison] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Toxic] = TypeEffectiveness.Normal,
            //},
        },
        [PokemonType.Dragon] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, TypeEffectiveness>
            {
                [MoveType.Bug] = TypeEffectiveness.Normal,
                [MoveType.Dark] = TypeEffectiveness.Normal,
                [MoveType.Dragon] = TypeEffectiveness.SuperEffective,
                [MoveType.Electric] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Fairy] = TypeEffectiveness.SuperEffective,
                [MoveType.Fighting] = TypeEffectiveness.Normal,
                [MoveType.Fire] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Flying] = TypeEffectiveness.Normal,
                [MoveType.Ghost] = TypeEffectiveness.Normal,
                [MoveType.Grass] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Ground] = TypeEffectiveness.Normal,
                [MoveType.Ice] = TypeEffectiveness.SuperEffective,
                [MoveType.Normal] = TypeEffectiveness.Normal,
                [MoveType.Poison] = TypeEffectiveness.Normal,
                [MoveType.Psychic] = TypeEffectiveness.Normal,
                [MoveType.Rock] = TypeEffectiveness.Normal,
                [MoveType.Steel] = TypeEffectiveness.Normal,
                [MoveType.Stellar] = TypeEffectiveness.Normal,
                [MoveType.Water] = TypeEffectiveness.NotVeryEffective,

                [MoveType.Unknown] = TypeEffectiveness.Normal,
            },
            HpIvs = new Dictionary<StatId, int>
            {
                [StatId.Atk] = 30,
            },
            HpDvs = new Dictionary<StatId, int>
            {
                [StatId.Def] = 14,
            },
            //SpecialDamageTaken = new Dictionary<SpecialImmunityId, TypeEffectiveness>
            //{
            //    [SpecialImmunityId.Prankster] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Paralysis] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Burn] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Trapped] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Powder] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Sandstorm] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Hail] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Freeze] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Poison] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Toxic] = TypeEffectiveness.Normal,
            //},
        },
        [PokemonType.Dark] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, TypeEffectiveness>
            {
                [MoveType.Bug] = TypeEffectiveness.SuperEffective,
                [MoveType.Dark] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Dragon] = TypeEffectiveness.Normal,
                [MoveType.Electric] = TypeEffectiveness.Normal,
                [MoveType.Fairy] = TypeEffectiveness.SuperEffective,
                [MoveType.Fighting] = TypeEffectiveness.SuperEffective,
                [MoveType.Fire] = TypeEffectiveness.Normal,
                [MoveType.Flying] = TypeEffectiveness.Normal,
                [MoveType.Ghost] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Grass] = TypeEffectiveness.Normal,
                [MoveType.Ground] = TypeEffectiveness.Normal,
                [MoveType.Ice] = TypeEffectiveness.Normal,
                [MoveType.Normal] = TypeEffectiveness.Normal,
                [MoveType.Poison] = TypeEffectiveness.Normal,
                [MoveType.Psychic] = TypeEffectiveness.Immune,
                [MoveType.Rock] = TypeEffectiveness.Normal,
                [MoveType.Steel] = TypeEffectiveness.Normal,
                [MoveType.Stellar] = TypeEffectiveness.Normal,
                [MoveType.Water] = TypeEffectiveness.Normal,

                [MoveType.Unknown] = TypeEffectiveness.Normal,
            },
            HpIvs = new Dictionary<StatId, int>(),
            //SpecialDamageTaken = new Dictionary<SpecialImmunityId, TypeEffectiveness>
            //{
            //    [SpecialImmunityId.Prankster] = TypeEffectiveness.Immune,
            //    [SpecialImmunityId.Paralysis] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Burn] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Trapped] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Powder] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Sandstorm] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Hail] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Freeze] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Poison] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Toxic] = TypeEffectiveness.Normal,
            //},
        },
        [PokemonType.Steel] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, TypeEffectiveness>
            {
                [MoveType.Bug] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Dark] = TypeEffectiveness.Normal,
                [MoveType.Dragon] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Electric] = TypeEffectiveness.Normal,
                [MoveType.Fairy] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Fighting] = TypeEffectiveness.SuperEffective,
                [MoveType.Fire] = TypeEffectiveness.SuperEffective,
                [MoveType.Flying] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Ghost] = TypeEffectiveness.Normal,
                [MoveType.Grass] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Ground] = TypeEffectiveness.SuperEffective,
                [MoveType.Ice] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Normal] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Poison] = TypeEffectiveness.Immune,
                [MoveType.Psychic] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Rock] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Steel] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Stellar] = TypeEffectiveness.Normal,
                [MoveType.Water] = TypeEffectiveness.Normal,

                [MoveType.Unknown] = TypeEffectiveness.Normal,
            },
            HpIvs = new Dictionary<StatId, int>
            {
                [StatId.SpD] = 30,
            },
            HpDvs = new Dictionary<StatId, int>
            {
                [StatId.Atk] = 13,
            },
            //SpecialDamageTaken = new Dictionary<SpecialImmunityId, TypeEffectiveness>
            //{
            //    [SpecialImmunityId.Prankster] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Paralysis] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Burn] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Trapped] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Powder] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Sandstorm] = TypeEffectiveness.Immune,
            //    [SpecialImmunityId.Hail] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Freeze] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Poison] = TypeEffectiveness.Immune,
            //    [SpecialImmunityId.Toxic] = TypeEffectiveness.Immune,
            //},
        },
        [PokemonType.Fairy] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, TypeEffectiveness>
            {
                [MoveType.Bug] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Dark] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Dragon] = TypeEffectiveness.Immune,
                [MoveType.Electric] = TypeEffectiveness.Normal,
                [MoveType.Fairy] = TypeEffectiveness.Normal,
                [MoveType.Fighting] = TypeEffectiveness.NotVeryEffective,
                [MoveType.Fire] = TypeEffectiveness.Normal,
                [MoveType.Flying] = TypeEffectiveness.Normal,
                [MoveType.Ghost] = TypeEffectiveness.Normal,
                [MoveType.Grass] = TypeEffectiveness.Normal,
                [MoveType.Ground] = TypeEffectiveness.Normal,
                [MoveType.Ice] = TypeEffectiveness.Normal,
                [MoveType.Normal] = TypeEffectiveness.Normal,
                [MoveType.Poison] = TypeEffectiveness.SuperEffective,
                [MoveType.Psychic] = TypeEffectiveness.Normal,
                [MoveType.Rock] = TypeEffectiveness.Normal,
                [MoveType.Steel] = TypeEffectiveness.SuperEffective,
                [MoveType.Stellar] = TypeEffectiveness.Normal,
                [MoveType.Water] = TypeEffectiveness.Normal,

                [MoveType.Unknown] = TypeEffectiveness.Normal,
            },
            //SpecialDamageTaken = new Dictionary<SpecialImmunityId, TypeEffectiveness>
            //{
            //    [SpecialImmunityId.Prankster] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Paralysis] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Burn] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Trapped] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Powder] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Sandstorm] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Hail] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Freeze] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Poison] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Toxic] = TypeEffectiveness.Normal,
            //},
        },
        [PokemonType.Normal] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, TypeEffectiveness>
            {
                [MoveType.Bug] = TypeEffectiveness.Normal,
                [MoveType.Dark] = TypeEffectiveness.Normal,
                [MoveType.Dragon] = TypeEffectiveness.Normal,
                [MoveType.Electric] = TypeEffectiveness.Normal,
                [MoveType.Fairy] = TypeEffectiveness.Normal,
                [MoveType.Fighting] = TypeEffectiveness.SuperEffective,
                [MoveType.Fire] = TypeEffectiveness.Normal,
                [MoveType.Flying] = TypeEffectiveness.Normal,
                [MoveType.Ghost] = TypeEffectiveness.Immune,
                [MoveType.Grass] = TypeEffectiveness.Normal,
                [MoveType.Ground] = TypeEffectiveness.Normal,
                [MoveType.Ice] = TypeEffectiveness.Normal,
                [MoveType.Normal] = TypeEffectiveness.Normal,
                [MoveType.Poison] = TypeEffectiveness.Normal,
                [MoveType.Psychic] = TypeEffectiveness.Normal,
                [MoveType.Rock] = TypeEffectiveness.Normal,
                [MoveType.Steel] = TypeEffectiveness.Normal,
                [MoveType.Stellar] = TypeEffectiveness.Normal,
                [MoveType.Water] = TypeEffectiveness.Normal,

                [MoveType.Unknown] = TypeEffectiveness.Normal,
            },
            //SpecialDamageTaken = new Dictionary<SpecialImmunityId, TypeEffectiveness>
            //{
            //    [SpecialImmunityId.Prankster] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Paralysis] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Burn] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Trapped] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Powder] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Sandstorm] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Hail] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Freeze] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Poison] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Toxic] = TypeEffectiveness.Normal,
            //},
        },
        [PokemonType.Unknown] = new TypeData
        {
            DamageTaken = new Dictionary<MoveType, TypeEffectiveness>
            {
                [MoveType.Bug] = TypeEffectiveness.Normal,
                [MoveType.Dark] = TypeEffectiveness.Normal,
                [MoveType.Dragon] = TypeEffectiveness.Normal,
                [MoveType.Electric] = TypeEffectiveness.Normal,
                [MoveType.Fairy] = TypeEffectiveness.Normal,
                [MoveType.Fighting] = TypeEffectiveness.Normal,
                [MoveType.Fire] = TypeEffectiveness.Normal,
                [MoveType.Flying] = TypeEffectiveness.Normal,
                [MoveType.Ghost] = TypeEffectiveness.Normal,
                [MoveType.Grass] = TypeEffectiveness.Normal,
                [MoveType.Ground] = TypeEffectiveness.Normal,
                [MoveType.Ice] = TypeEffectiveness.Normal,
                [MoveType.Normal] = TypeEffectiveness.Normal,
                [MoveType.Poison] = TypeEffectiveness.Normal,
                [MoveType.Psychic] = TypeEffectiveness.Normal,
                [MoveType.Rock] = TypeEffectiveness.Normal,
                [MoveType.Steel] = TypeEffectiveness.Normal,
                [MoveType.Stellar] = TypeEffectiveness.Normal,
                [MoveType.Water] = TypeEffectiveness.Normal,

                [MoveType.Unknown] = TypeEffectiveness.Normal,
            },
            IsNonstandard = Nonstandard.Past,
            //SpecialDamageTaken = new Dictionary<SpecialImmunityId, TypeEffectiveness>
            //{
            //    [SpecialImmunityId.Prankster] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Paralysis] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Burn] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Trapped] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Powder] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Sandstorm] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Hail] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Freeze] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Poison] = TypeEffectiveness.Normal,
            //    [SpecialImmunityId.Toxic] = TypeEffectiveness.Normal,
            //},
        },
    };
}