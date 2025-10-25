using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.PokemonClasses;

public partial class Pokemon
{
    public MoveSlot? GetMoveData(MoveId move)
    {
        return GetMoveData(Battle.Library.Moves[move]);
    }

    public MoveSlot? GetMoveData(Move move)
    {
        return MoveSlots.FirstOrDefault(moveSlot => moveSlot.Id == move.Id);
    }

    /// <summary>
    /// Gets or creates the move hit data for this Pokemon's slot.
    /// Tracks per-target information like critical hits and type effectiveness.
    /// </summary>
    public MoveHitResult GetMoveHitData(ActiveMove move)
    {
        // Lazy initialization of the moveHitData dictionary if it doesn't exist
        move.MoveHitData ??= new MoveHitData();

        // Get this Pokemon's slot identifier
        PokemonSlot slot = GetSlot();

        // Try to get existing hit data for this slot
        if (!move.MoveHitData.TryGetValue(slot, out MoveHitResult? hitResult))
        {
            // Create default hit data if it doesn't exist
            hitResult = new MoveHitResult
            {
                Crit = false,
                TypeMod = 0,
                ZBrokeProtect = false,
            };

            // Store it in the dictionary
            move.MoveHitData[slot] = hitResult;
        }

        return hitResult;
    }

    /// <summary>
    /// Gets the list of moves available to this Pokemon for the current turn.
    /// Handles locked moves, move modifications, PP depletion, and disabled moves.
    /// </summary>
    /// <param name="lockedMove">If specified, the Pokemon is locked into using this move</param>
    /// <param name="restrictData">If true, hide certain disabled move information</param>
    /// <returns>List of available moves with their current state</returns>
    public List<PokemonMoveData> GetMoves(MoveId? lockedMove = null, bool restrictData = false)
    {
        // Handle locked move cases
        if (lockedMove is not null)
        {
            Trapped = PokemonTrapped.True;

            // Special case: Recharge turn (after Hyper Beam, etc.)
            if (lockedMove == MoveId.Recharge)
            {
                return
                [
                    new PokemonMoveData
                    {
                        Move = Battle.Library.Moves[MoveId.Recharge],
                        Target = null,
                        Disabled = null,
                        DisabledSource = null,
                    },
                ];
            }

            // Find the locked move in move slots
            foreach (MoveSlot moveSlot in MoveSlots.Where(moveSlot => moveSlot.Id == lockedMove))
            {
                return
                [
                    new PokemonMoveData
                    {
                        Move = Battle.Library.Moves[moveSlot.Move],
                        Target = null,
                        Disabled = null,
                        DisabledSource = null,
                    },
                ];
            }

            // Fallback: lookup move by ID (shouldn't normally happen)
            return
            [
                new PokemonMoveData
                {
                    Move = Battle.Library.Moves[lockedMove.Value],
                    Target = null,
                    Disabled = null,
                    DisabledSource = null,
                },
            ];
        }

        // Build list of available moves
        var moves = new List<PokemonMoveData>();
        bool hasValidMove = false;

        foreach (MoveSlot moveSlot in MoveSlots)
        {
            MoveId moveName = moveSlot.Move;

            // Special move target modifications
            switch (moveSlot.Id)
            {
                case MoveId.PollenPuff:
                    // Heal Block prevents Pollen Puff from targeting allies
                    if (Volatiles.ContainsKey(ConditionId.HealBlock))
                    {
                    }
                    break;

                case MoveId.TeraStarStorm:
                    // Terapagos-Stellar gets spread targeting
                    if (Species.Id == SpecieId.TerapagosStellar)
                    {
                    }
                    break;
            }

            // Determine if move is disabled
            BoolHiddenUnion disabled = moveSlot.Disabled;

            // Skip Dynamax handling as requested

            // Check if move is out of PP (unless locked into partial trapping move)
            if (moveSlot.Pp <= 0 && !Volatiles.ContainsKey(ConditionId.PartialTrappingLock))
            {
                disabled = true;
            }

            // Handle hidden disabled state
            if (disabled is HiddenBoolHiddenUnion)
            {
                disabled = !restrictData;
            }

            // Track if we have at least one valid (non-disabled) move
            if (!disabled.IsTruthy())
            {
                hasValidMove = true;
            }

            // Convert disabled state to MoveIdBoolUnion
            MoveIdBoolUnion? disabledUnion = null;
            if (disabled.IsTruthy())
            {
                disabledUnion = disabled.IsTrue() ? true : null;
            }

            // Get the Move object from the library
            Move moveObject = Battle.Library.Moves[moveName];

            // Add move to list
            moves.Add(new PokemonMoveData
            {
                Move = moveObject,
                Target = null, // Target is not set in this context
                Disabled = disabledUnion,
                DisabledSource = moveSlot.DisabledSource,
            });
        }
        return hasValidMove ? moves : [];
    }

    /// <summary>
    /// This refers to multi-turn moves like SolarBeam and Outrage and
    /// Sky Drop, which remove all choice (no dynamax, switching, etc).
    /// Don't use it for "soft locks" like Choice Band.
    /// </summary>
    public MoveId? GetLockedMove()
    {
        RelayVar? lockedMove = Battle.RunEvent(EventId.LockMove, this);

        // If event returns true, there's no locked move
        if (lockedMove is BoolRelayVar { Value: true })
        {
            return null;
        }

        // Otherwise, try to extract MoveId from the RelayVar
        if (lockedMove is MoveIdRelayVar moveIdRelayVar)
        {
            return moveIdRelayVar.MoveId;
        }

        // If RelayVar is null or another type, no locked move
        return null;
    }

    /// <summary>
    /// Deducts PP (Power Points) from a move when it is used.
    /// In Gen 1, PP can go negative. In Gen 2+, PP is clamped to 0.
    /// </summary>
    /// <param name="moveId">The move to deduct PP from</param>
    /// <param name="amount">Amount of PP to deduct (defaults to 1)</param>
    /// <param name="target">The target Pokemon (unused but kept for API compatibility)</param>
    /// <returns>The actual amount of PP deducted</returns>
    public int DeductPp(MoveId moveId, int? amount = null, PokemonFalseUnion? target = null)
    {
        Move move = Battle.Library.Moves[moveId];
        return DeductPp(move, amount, target);
    }

    /// <summary>
    /// Deducts PP (Power Points) from a move when it is used.
    /// In Gen 1, PP can go negative. In Gen 2+, PP is clamped to 0.
    /// </summary>
    /// <param name="move">The move to deduct PP from</param>
    /// <param name="amount">Amount of PP to deduct (defaults to 1)</param>
    /// <param name="target">The target Pokemon (unused but kept for API compatibility)</param>
    /// <returns>The actual amount of PP deducted</returns>
    public int DeductPp(Move move, int? amount = null, PokemonFalseUnion? target = null)
    {
        int gen = Battle.Gen;

        // Get the move data for this Pokemon
        MoveSlot? ppData = GetMoveData(move);
        if (ppData == null) return 0;

        // Mark move as used
        ppData.Used = true;

        // Gen 2+: If move has no PP left, can't deduct anything
        if (ppData.Pp <= 0 && gen > 1) return 0;

        // Default to deducting 1 PP
        int deductAmount = amount ?? 1;

        // Deduct the PP
        ppData.Pp -= deductAmount;

        // Gen 2+: Clamp PP to 0 and adjust return value if we went negative
        if (ppData.Pp < 0 && gen > 1)
        {
            deductAmount += ppData.Pp; // ppData.pp is negative, so this reduces deductAmount
            ppData.Pp = 0;
        }

        return deductAmount;
    }

    public void MoveUsed(ActiveMove move, int? targetLoc = null)
    {
        LastMove = move;
        if (Battle.Gen == 2) LastMoveEncore = move;
        LastMoveTargetLoc = targetLoc;
        MoveThisTurn = move.Id;
    }

    public bool HasMove(MoveId move)
    {
        return MoveSlots.Any(ms => ms.Id == move);
    }

    public void DisableMove(MoveId moveId, bool isHidden = false, IEffect? sourceEffect = null)
    {
        if (sourceEffect is not null && Battle.Event is not null)
        {
            sourceEffect = Battle.Event.Effect;
        }

        foreach (MoveSlot moveSlot in MoveSlots.Where(moveSlot =>
                     moveSlot.Id != moveId && moveSlot.Disabled != true))
        {
            moveSlot.Disabled = isHidden ? BoolHiddenUnion.FromHidden() : true;
            moveSlot.DisabledSource = sourceEffect ?? Battle.Library.Moves[moveSlot.Move].ToActiveMove();
        }
    }
}