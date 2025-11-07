using ApogeeVGC.Data;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleActions(Battle battle)
{
    public Battle Battle { get; init; } = battle;
    public Library Library => Battle.Library;
    // public ModdedDex Dex => Battle.Dex;

    private readonly HashSet<MoveTarget> _choosableTargets =
    [
        MoveTarget.Normal,
        MoveTarget.Any,
        MoveTarget.AdjacentAlly,
        MoveTarget.AdjacentAllyOrSelf,
        MoveTarget.AdjacentFoe,
    ];




    #region Helpers

    // Priority mapping (lower number = higher priority)
    private static int GetBattleActionsPriority(BoolIntEmptyUndefinedUnion? value)
    {
        return value switch
        {
            UndefinedBoolIntEmptyUndefinedUnion => 0,        // undefined (highest)
            EmptyBoolIntEmptyUndefinedUnion => 1,            // empty string (NOT_FAILURE)
            null => 2,                                       // null
            BoolBoolIntEmptyUndefinedUnion => 3,             // boolean
            IntBoolIntEmptyUndefinedUnion => 4,              // number (lowest)
            _ => 5,
        };
    }

    private IntUndefinedFalseUnion ExecuteMoveHit(List<Pokemon> targets, Pokemon pokemon, ActiveMove move,
        HitEffect? moveData = null, bool isSecondary = false, bool isSelf = false)
    {
        (SpreadMoveDamage damage, _) = SpreadMoveHit(
            SpreadMoveTargets.FromPokemonList(targets),
            pokemon, move, moveData, isSecondary, isSelf);

        if (damage.Count == 0)
        {
            return IntUndefinedFalseUnion.FromFalse();
        }

        BoolIntUndefinedUnion retVal = damage[0];

        return retVal switch
        {
            BoolBoolIntUndefinedUnion { Value: true } => new Undefined(),
            IntBoolIntUndefinedUnion intVal => intVal.Value,
            UndefinedBoolIntUndefinedUnion => new Undefined(),
            _ => IntUndefinedFalseUnion.FromFalse()
        };
    }

    private void ClearActiveMove(bool failed)
    {
        Battle.ActiveMove = null;
        if (failed)
        {
            Battle.ActiveTarget = null;
        }
    }

    #endregion
}