using ApogeeVGC.Data;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.BattleClasses;

public class BattleActions(IBattle battle)
{
    public IBattle Battle { get; init; } = battle;
    public Library Library => Battle.Library;

    #region Switch

    public bool SwitchIn(Pokemon pokemon, int pos, IEffect? sourceEffect, bool isDrag = false)
    {
        throw new NotImplementedException();
    }

    public bool DragIn(Side side, int pos)
    {
        throw new NotImplementedException();
    }

    public bool RunSwitch(Pokemon pokemon)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Moves

    public record RunMoveOptions
    {
        public IEffect? SourceEffect { get; init; }
        public bool? ExternalMove { get; init; }
        public Pokemon? OriginalTarget { get; init; }
    }

    public void RunMove(MoveId moveId, Pokemon pokemon, int targetLoc, RunMoveOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public void RunMove(Move move, Pokemon pokemon, int targetLoc, RunMoveOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public record UseMoveOptions
    {
        public Pokemon? Target { get; init; }
        public IEffect? SourceEffect { get; init; }
    }

    public void UseMove(MoveId moveId, Pokemon pokemon, UseMoveOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public void UseMove(Move move, Pokemon pokemon, UseMoveOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public void UseMoveInner(MoveId moveId, Pokemon pokemon, UseMoveOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public void UseMoveInner(Move move, Pokemon pokemon, UseMoveOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public bool TrySpreadMoveHit(List<Pokemon> targets, Pokemon pokemon, ActiveMove move, bool notActive = false)
    {
        throw new NotImplementedException();
    }

    public List<RelayVar> HitStepInvulnerabilityEvent(List<Pokemon> targets, Pokemon pokemon, ActiveMove move)
    {
        throw new NotImplementedException();
    }

    public List<RelayVar> HitStepTryEvent(List<Pokemon> targets, Pokemon pokemon, ActiveMove move)
    {
        throw new NotImplementedException();
    }

    public List<bool> HitStepTypeImmunity(List<Pokemon> targets, Pokemon pokemon, ActiveMove move)
    {
        throw new NotImplementedException();
    }

    public List<bool> HitStepTryImmunity(List<Pokemon> targets, Pokemon pokemon, ActiveMove move)
    {
        throw new NotImplementedException();
    }

    public List<bool> HitStepAccuracy(List<Pokemon> targets, Pokemon pokemon, ActiveMove move)
    {
        throw new NotImplementedException();
    }

    public Undefined HitStepBreakProtect(List<Pokemon> targets, Pokemon pokemon, ActiveMove move)
    {
        throw new NotImplementedException();
    }

    public Undefined HitStepStealProtect(List<Pokemon> targets, Pokemon pokemon, ActiveMove move)
    {
        throw new NotImplementedException();
    }

    public Undefined AfterMoveSecondaryEvent(List<Pokemon> targets, Pokemon pokemon, ActiveMove move)
    {
        throw new NotImplementedException();
    }

    public IntUndefinedFalseUnion TryMoveHit(Pokemon target, Pokemon pokemon, ActiveMove move)
    {
        throw new NotImplementedException();
    }

    public IntUndefinedFalseUnion TryMoveHit(List<Pokemon> targets, Pokemon pokemon, ActiveMove move)
    {
        throw new NotImplementedException();
    }

    public List<BoolIntUndefinedUnion> HitStepMoveHitLoop(List<Pokemon> targets, Pokemon pokemon, ActiveMove move)
    {
        throw new NotImplementedException();
    }

    public (SpreadMoveDamage, SpreadMoveTargets) SpreadMoveHit(SpreadMoveTargets targets, Pokemon pokemon,
        ActiveMove move, HitEffect? hitEffect = null, bool isSecondary = false, bool isSelf = false)
    {
        throw new NotImplementedException();
    }

    public SpreadMoveDamage TryPrimaryHitEvent(SpreadMoveDamage damage, SpreadMoveTargets targets,
        Pokemon pokemon, ActiveMove move, ActiveMove moveData, bool isSecondary = false)
    {
        throw new NotImplementedException();
    }

    public SpreadMoveDamage GetSpreadDamage(SpreadMoveDamage damage, SpreadMoveTargets targets, Pokemon source,
        ActiveMove move, ActiveMove moveData, bool isSecondary = false, bool isSelf = false)
    {
        throw new NotImplementedException();
    }

    public SpreadMoveDamage RunMoveEffects(SpreadMoveDamage damage, SpreadMoveTargets targets,
        Pokemon source, ActiveMove move, ActiveMove moveData, bool isSecondary = false, bool isSelf = false)
    {
        throw new NotImplementedException();
    }

    public void SelfDrops(SpreadMoveTargets targets, Pokemon source, ActiveMove move, ActiveMove moveData,
        bool isSecondary = false)
    {
        throw new NotImplementedException();
    }

    public void Secondaries(SpreadMoveTargets targets, Pokemon source, ActiveMove move, ActiveMove moveData,
        bool isSelf = false)
    {
        throw new NotImplementedException();
    }

    public SpreadMoveDamage ForceSwitch(SpreadMoveDamage damage, SpreadMoveTargets targets, Pokemon source,
        ActiveMove move)
    {
        throw new NotImplementedException();
    }

    public IntUndefinedFalseUnion MoveHit(Pokemon? target, Pokemon pokemon, ActiveMove move,
        HitEffect? moveData, bool isSecondary = false, bool isSelf = false)
    {
        throw new NotImplementedException();
    }

    public IntUndefinedFalseUnion MoveHit(List<Pokemon?> targets, Pokemon pokemon, ActiveMove move,
        HitEffect? moveData, bool isSecondary = false, bool isSelf = false)
    {
        throw new NotImplementedException();
    }

    public int CalcRecoilDamage(int damageDealt, Move move, Pokemon pokemon)
    {
        throw new NotImplementedException();
    }

    public bool TargetTypeChoices(PokemonType type)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Combines two move result values based on priority.
    /// Used to aggregate results across multiple targets.
    /// Priority order (highest to lowest): undefined, string (success), null, boolean, number.
    /// When both values are numbers, they are summed.
    /// </summary>
    /// <param name="left">First result value</param>
    /// <param name="right">Second result value</param>
    /// <returns>Combined result with the higher priority, or sum if both are numbers</returns>
    public static BoolIntUndefinedUnion CombineResults(
        BoolIntUndefinedUnion? left,
        BoolIntUndefinedUnion? right)
    {
        // Handle null inputs
        if (left == null && right == null) return BoolIntUndefinedUnion.FromUndefined();
        if (left == null) return right!;
        if (right == null) return left;

        int leftPriority = GetPriority(left);
        int rightPriority = GetPriority(right);

        // If left has higher priority, return it
        if (leftPriority < rightPriority)
        {
            return left;
        }

        // If left is truthy and right is falsy (but not 0)
        if (left.IsTruthy() && !right.IsTruthy() && !right.IsTruthy())
        {
            return left;
        }

        // If both are numbers, sum them
        if (left is IntBoolIntUndefinedUnion leftInt &&
            right is IntBoolIntUndefinedUnion rightInt)
        {
            return BoolIntUndefinedUnion.FromInt(leftInt.Value + rightInt.Value);
        }

        // Otherwise return right
        return right;

        // Priority mapping (lower number = higher priority)
        int GetPriority(BoolIntUndefinedUnion value)
        {
            return value switch
            {
                UndefinedBoolIntUndefinedUnion => 0,        // undefined (highest)
                // string/"NOT_FAILURE" case not in our union, would be priority 1
                // null case not in our union, would be priority 2
                BoolBoolIntUndefinedUnion => 3,              // boolean
                IntBoolIntUndefinedUnion => 4,               // number (lowest)
                _ => 5,
            };
        }
    }

    public IntUndefinedFalseUnion? GetDamage(Pokemon source, Pokemon target, ActiveMove move,
        bool suppressMessages = false)
    {
        throw new NotImplementedException();
    }

    public IntUndefinedFalseUnion? GetDamage(Pokemon source, Pokemon target, MoveId move,
        bool suppressMessages = false)
    {
        throw new NotImplementedException();
    }

    public IntUndefinedFalseUnion? GetDamage(Pokemon source, Pokemon target, int move,
        bool suppressMessages = false)
    {
        throw new NotImplementedException();
    }

    public int ModifyDamage(int baseDamage, Pokemon pokemon, Pokemon target, ActiveMove move,
        bool suppressMessages = false)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// Calculates confusion self-hit damage.
    /// 
    /// Confusion damage is unique - most typical modifiers that get run when calculating
    /// damage (e.g. Huge Power, Life Orb, critical hits) don't apply. It also uses a 16-bit
    /// context for its damage, unlike the regular damage formula (though this only comes up
    /// for base damage).
    /// </summary>
    /// <param name="pokemon">The confused Pokémon hitting itself</param>
    /// <param name="basePower">Base power of the confusion damage (typically 40)</param>
    /// <returns>The calculated damage amount (minimum 1)</returns>
    public int GetConfusionDamage(Pokemon pokemon, int basePower)
    {
        // Get the Pokémon's attack and defense stats with current boosts applied
        int attack = pokemon.CalculateStat(StatIdExceptHp.Atk, pokemon.Boosts.GetBoost(BoostId.Atk));
        int defense = pokemon.CalculateStat(StatIdExceptHp.Def, pokemon.Boosts.GetBoost(BoostId.Def));
        int level = pokemon.Level;

        // Calculate base damage using the standard Pokémon damage formula
        // Formula: ((2 * level / 5 + 2) * basePower * attack / defense) / 50 + 2
        // Each step is truncated to match game behavior
        int baseDamage = Battle.Trunc(
            Battle.Trunc(
                Battle.Trunc(
                    Battle.Trunc(2 * level / 5 + 2) * basePower * attack
                ) / defense
            ) / 50
        ) + 2;

        // Apply 16-bit truncation for confusion damage
        // This only matters for extremely high damage values (Eternatus-Eternamax level stats)
        int damage = Battle.Trunc(baseDamage, 16);

        // Apply random damage variance (85-100% of calculated damage)
        damage = Battle.Randomizer(damage);

        // Ensure at least 1 damage is dealt
        return Math.Max(1, damage);
    }

    #endregion

    #region Terastallization

    public MoveType? CanTerastallize(IBattle battle, Pokemon pokemon)
    {
        if (Battle.Gen != 9)
        {
            return null;
        }
        return pokemon.TeraType;
    }

    public void Terastallize(Pokemon pokemon)
    {
        throw new NotImplementedException();
    }

    #endregion
}