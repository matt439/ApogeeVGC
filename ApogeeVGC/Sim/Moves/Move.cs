using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Moves;

public record Move : IEffect
{
    public EffectType EffectType => EffectType.Move;
    public required MoveId Id { get; init; }
    public required int Num
    {
        get;
        init
        {
            if (Num < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Num), "Move number must be non-negative.");
            }
            field = value;
        }
    }
    public string Name { get; init; } = string.Empty;
    public required int Accuracy
    {
        get;
        init
        {
            if (value is < 1 or > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(Accuracy), "Accuracy must be between 1 and 100.");
            }
            field = value;
        }
    }
    public int BasePower
    {
        get;
        init
        {
            if (value < 0 )
            {
                throw new ArgumentOutOfRangeException(nameof(BasePower), "Base power must be non-negative.");
            }
            field = value;
        }
    }
    public required MoveCategory Category { get; init; }
    public required int BasePp
    {
        get;
        init
        {
            if (!(value == 1 || value % 5 == 0))
            {
                throw new ArgumentOutOfRangeException(nameof(BasePp), "PP must be 1 or a multiple of 5.");
            }
            if (value > 40)
            {
                throw new ArgumentOutOfRangeException(nameof(BasePp), "PP cannot exceed 40.");
            }
            field = value;
        }
    }
    public int PpUp
    {
        get;
        init
        {
            if (value is < 0 or > 3)
            {
                throw new ArgumentOutOfRangeException(nameof(PpUp), "PP Ups must be between 0 and 3.");
            }

            field = value;
        }
    } = 0;
    public int MaxPp => BasePp + (int)(0.2 * BasePp * PpUp);
    public int UsedPp
    {
        get;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(UsedPp), "Used PP cannot be negative.");
            }
            field = value;
        }
    } = 0;
    public int Pp
    {
        get
        {
            int pp = MaxPp - UsedPp;
            return pp > 0 ? pp : 0;
        }
    }
    public int Priority
    {
        get;
        init
        {
            if (value is > 5 or < -7)
            {
                throw new ArgumentOutOfRangeException(nameof(Priority), "Priority must be between -7 and 5.");
            }
            field = value;
        }
    } = 0;
    public MoveFlags Flags { get; init; } = new();
    public MoveTarget Target { get; init; }
    public MoveType Type { get; init; }
    public SecondaryEffect? Secondary { get; init; }
    public Condition? Condition { get; init; }
    public bool AlwaysHit { get; init; }
    public bool StallingMove { get; init; }


    public bool SelfSwitch { get; init; }
    public bool Infiltrates { get; init; }


    /// <summary>
    /// The recoil damage as a fraction of the damage dealt (e.g., 0.25 for 1/4 recoil).
    /// </summary>
    public double? Recoil { get; init; }
    public bool Disabled { get; set; }

    /// <summary>The MoveSlot this move occupies in a Pokemon's moveset.</summary>
    public MoveSlot MoveSlot { get; init; }

    public bool? PranksterBooster { get; set; }

    /// <summary>
    /// Creates a deep copy of this Move for simulation purposes.
    /// This method creates an independent copy with the same state while sharing immutable references.
    /// </summary>
    /// <returns>A new Move instance with copied state</returns>
    public Move Copy()
    {
        return this with 
        { 
            // Records have built-in copy semantics with 'with' expression
            // This creates a shallow copy which is appropriate since most properties
            // are either value types, immutable references, or function delegates
            // The mutable properties (PpUp, UsedPp) are copied correctly
        };
    }
}