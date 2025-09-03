namespace ApogeeVGC.Sim;

/// <summary>
/// Base class for field elements like Weather, Terrain, and Pseudo-Weather.
/// </summary>
public class FieldElement
{
    public required string Name { get; init; }
    public required int BaseDuration
    {
        get;
        init
        {
            if (value < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "BaseDuration must be at least 1.");
            }
            field = value;
        }
    }
    public bool IsExtended { get; set; }
    public required int DurationExtension
    {
        get;
        init
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "DurationExtension cannot be negative.");
            }
            field = value;
        }
    }
    public int Duration => IsExtended ? BaseDuration + DurationExtension : BaseDuration;
    /// <summary>
    /// target, source, effect. If returns true, duration is extended.
    /// </summary>
    public Func<Pokemon?, Pokemon, IEffect, bool>? DurationCallback { get; init; }
    public int RemainingTurns => Duration - ElapsedTurns;
    public bool IsExpired => RemainingTurns <= 0;
    public bool PrintDebug { get; init; }
    protected int ElapsedTurns { get; set; }
    public Action<Pokemon[], BattleContext>? OnEnd { get; init; }
    public Action<Pokemon[], BattleContext>? OnStart { get; init; }
    public Action<Field, Pokemon[], BattleContext>? OnReapply { get; init; }
    public Action<Pokemon[], FieldElement, BattleContext>? OnIncrementTurnCounter { get; init; }
    public Action<Pokemon, BattleContext>? OnPokemonSwitchIn { get; init; }

    public void IncrementTurnCounter()
    {
        //if (RemainingTurns > 0)
        //{
        //    ElapsedTurns++;
        //}
        //else
        //{
        //    throw new InvalidOperationException("Cannot increment turn counter beyond duration.");
        //}
        ElapsedTurns++;
    }

    public void ResetTurnCounter()
    {
        ElapsedTurns = 0;
    }
}