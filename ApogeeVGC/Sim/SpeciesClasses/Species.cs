using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils;
using ApogeeVGC.Sim.Utils.Extensions;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.SpeciesClasses;

public record Species : IEffect, ICopyable<Species>
{
    public const double Epsilon = 0.0001;

    public SpecieId Id { get; init; }
    public EffectStateId EffectStateId => Id;
    public EffectType EffectType => EffectType.Specie;
    public int Num { get; init; }
    public required string Name { get; init; }
    public string FullName => $"{Name}{(Forme != BaseForme ? $"-{Forme}" : string.Empty)}";
    public SpecieId BaseSpecies { get; init; }
    public FormeId Forme { get; init; } = FormeId.None;
    public FormeId BaseForme { get; init; } = FormeId.None;
    public IReadOnlyList<FormeId>? CosmeticFormes { get; init; }
    public IReadOnlyList<FormeId>? OtherFormes { get; init; }
    public IReadOnlyList<FormeId>? FormeOrder { get; init; }
    public SpriteId SpriteId { get; init; }
    public required SpeciesAbility Abilities { get; init; }
    public required IReadOnlyList<PokemonType> Types { get; init; }

    public PokemonType? AddedType { get; init; }

    // null if no prevo
    public SpecieId? Prevo { get; init; }
    public IReadOnlyList<SpecieId> Evos { get; init; } = [];
    public EvoType? EvoType { get; init; }
    public string? EvoCondition { get; init; }
    public int? EvoItem { get; init; }
    public int? EvoMove { get; init; }
    public EvoRegion? EvoRegion { get; init; }
    public int? EvoLevel { get; init; }
    public bool Nfe { get; init; }
    public IReadOnlyList<EggGroup> EggGroups { get; init; } = [];
    public bool CanHatch { get; init; }
    public bool IsCosmeticForme { get; init; }
    public GenderId Gender { get; init; }

    public GenderRatio GenderRatio => new(Gender);
    public required StatsTable BaseStats { get; init; }
    public int? MaxHp { get; init; }
    public int Bst => BaseStats.BaseStatTotal;
    public double WeightKg { get; init; } // in kilograms

    /// <summary>
    /// Weight (in integer multiples of 0.1kg).
    /// </summary>
    public int WeightHg => (int)WeightKg * 10;
    public double HeightM { get; init; } // in meters
    public string Color { get; init; } = string.Empty;

    public IReadOnlyList<SpeciesTag> Tags { get; init; } = [];
    public SpecieUnreleasedHidden UnreleasedHidden { get; init; } = false;
    public bool MaleOnlyHidden { get; init; }
    public SpecieId? Mother { get; init; }
    public MoveType? RequiredTeraType { get; init; }
    public FormeId? BattleOnly { get; init; }
    public ItemId? RequiredItem { get; init; }
    public MoveId? RequiredMove { get; init; }
    public AbilityId? RequiredAbility { get; init; }

    public ConditionId Conditon { get; init; } = ConditionId.None;

    public IReadOnlyList<ItemId>? RequiredItems
    {
        get
        {
            if (field is { Count: > 0 })
            {
                return field;
            }
            if (RequiredItem != null)
            {
                return [RequiredItem.Value];
            }
            return null;
        }
        init;
    }

    public FormeId? ChangesFrom { get; init; }


    public Tier Tier
    {
        get;
        init
        {
            if (!value.IsSinglesOrOtherTier())
            {
                throw new ArgumentException("Tier must be a valid Singles tier.", nameof(Tier));
            }
            field = value;
        }
    }
    public Tier DoublesTier
    {
        get;
        init
        {
            if (!value.IsDoublesOrOtherTier())
            {
                throw new ArgumentException("Tier must be a valid Doubles tier.", nameof(Tier));
            }
            field = value;
        }
    }

    public Tier NatDexTier
    {
        get;
        init
        {
            if (!value.IsSinglesOrOtherTier())
            {
                throw new ArgumentException("Tier must be a valid Singles tier.", nameof(NatDexTier));
            }
            field = value;
        }
    }

    public int Gen
    {
        get
        {
            if (field != 0 || Num < 1) return Gen;
            if (Num >= 906 || Forme.ToString().Contains("Paldea"))
            {
                return 9;
            }
            if (Num >= 810 || Forme.IsGen8Forme())
            {
                return 8;
            }
            if (Num >= 722 || Forme.IsGen7Forme())
            {
                return 7;
            }
            if (Forme.IsPrimalForme())
            {
                return 6;
                // Note: You may need to add IsPrimal and BattleOnly properties if they don't exist
                // IsPrimal = true;
                // BattleOnly = BaseSpecies;
            }
            if (Num >= 650 || Forme.IsMegaForme())
            {
                return 6;
            }
            return Num switch
            {
                >= 494 => 5,
                >= 387 => 4,
                >= 252 => 3,
                >= 152 => 2,
                _ => 1,
            };
        }
        init;
    }


    public Species Copy()
    {
        return this with
        {
            Types = [..Types],
            BaseStats = BaseStats.Copy(),
            Abilities = new SpeciesAbility
            {
                Slot0 = Abilities.Slot0,
                Slot1 = Abilities.Slot1,
                Hidden = Abilities.Hidden,
                Special = Abilities.Special,
            },
            CosmeticFormes = CosmeticFormes != null ? [..CosmeticFormes] : null,
            OtherFormes = OtherFormes != null ? [..OtherFormes] : null,
            FormeOrder = FormeOrder != null ? [..FormeOrder] : null,
            Evos = [..Evos],
            EggGroups = [..EggGroups],
            Tags = [..Tags],
        };
    }

    public EffectDelegate? GetDelegate(EventId id)
    {
        // Species doesn't have any event handlers, so always return null
        return null;
    }

    public int? GetPriority(EventId id) => null;
    public IntFalseUnion? GetOrder(EventId id) => null;
    public int? GetSubOrder(EventId id) => null;

    /// <summary>
    /// Gets event handler information for the specified event (TODO: implement fully).
    /// </summary>
    public EventHandlerInfo? GetEventHandlerInfo(EventId id)
    {
    // TODO: Implement using EventHandlerInfoBuilder similar to Ability class
        // For now, return null as this hasn't been migrated yet
        return null;
    }
}

public enum SpeciesAbilityType
{
    Slot0,
    Slot1,
    Hidden,
    Special,
}

public class SpeciesAbility
{
    public AbilityId Slot0 { get; set; }
    public AbilityId? Slot1 { get; set; }
    public AbilityId? Hidden { get; set; }
    public AbilityId? Special { get; set; }

    public AbilityId? GetAbility(SpeciesAbilityType type)
    {
        return type switch
        {
            SpeciesAbilityType.Slot0 => Slot0,
            SpeciesAbilityType.Slot1 => Slot1,
            SpeciesAbilityType.Hidden => Hidden,
            SpeciesAbilityType.Special => Special,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };
    }

    public void SetAbility(SpeciesAbilityType type, AbilityId? abilityId)
    {
        switch (type)
        {
            case SpeciesAbilityType.Slot0:
                if (abilityId == null)
                {
                    throw new ArgumentNullException(nameof(abilityId), "Slot0 ability cannot be null.");
                }
                Slot0 = abilityId.Value;
                break;
            case SpeciesAbilityType.Slot1:
                Slot1 = abilityId;
                break;
            case SpeciesAbilityType.Hidden:
                Hidden = abilityId;
                break;
            case SpeciesAbilityType.Special:
                Special = abilityId;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}


public record GenderRatio
{
    public double M
    {
        get;
        init
        {
            if (value is < 0 or > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            field = value;
        }
    }

    public double F
    {
        get;
        init
        {
            if (value is < 0 or > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            field = value;
        }
    }

    public GenderRatio(double m, double f)
    {
        if (m < 0.0 || m > 1.0 || f < 0.0 || f > 1.0 ||
            Math.Abs(m + f - 1.0) > Species.Epsilon && m != 0.0 && f != 0.0)
        {
            throw new ArgumentException();
        }
        M = m;
        F = f;
    }

    public GenderRatio(GenderId gender)
    {
        switch (gender)
        {
            case GenderId.M:
                M = 1.0;
                F = 0.0;
                break;
            case GenderId.F:
                M = 0.0;
                F = 1.0;
                break;
            case GenderId.N:
                M = 0.0;
                F = 0.0;
                break;
            case GenderId.Empty:
                M = 0.5;
                F = 0.5;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gender), gender, null);
        }
    }
}