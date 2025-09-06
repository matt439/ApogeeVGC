using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Extensions;

namespace ApogeeVGC.Sim.Choices;


public abstract record SlotChoice
{
    private SlotChoice() { }

    public sealed record MoveChoice : SlotChoice
    {
        // SideId, SlotId, and Trainer can be accessed via Attacker property
        public Pokemon Attacker { get; }
        // MoveSlot can be accessed via Move property
        public Move Move { get; }
        public bool IsTera { get; }
        public MoveNormalTarget MoveNormalTarget { get; }
        public IReadOnlyList<Pokemon> PossibleTargets { get; }

        public SideId SideId => Attacker.SideId;
        public SlotId SlotId => Attacker.SlotId;

        internal MoveChoice(Pokemon attacker, Move move, bool isTera, MoveNormalTarget moveNormalTarget,
            IReadOnlyList<Pokemon> possibleTargets)
        {
            Attacker = attacker ?? throw new ArgumentNullException(nameof(attacker));
            Move = move ?? throw new ArgumentNullException(nameof(move));
            PossibleTargets = possibleTargets ?? throw new ArgumentNullException(nameof(possibleTargets));

            // Check if move is contained in attacker's moves
            if (!attacker.Moves.Contains(move))
            {
                throw new ArgumentException("The specified move is not known by the attacker.");
            }
            if (attacker.IsFainted)
            {
                throw new ArgumentException("A fainted Pokémon cannot use moves.", nameof(attacker));
            }
            if (move.Target == MoveTarget.Normal && moveNormalTarget == MoveNormalTarget.None)
            {
                throw new ArgumentException("A normal target move must have a specified normal target.");
            }
            if (move.Target != MoveTarget.Normal && moveNormalTarget != MoveNormalTarget.None)
            {
                throw new ArgumentException("A non-normal target move cannot have a normal target specified.");
            }
            // Check for any duplicates in possibleTargets
            if (possibleTargets.Distinct().Count() != possibleTargets.Count)
            {
                throw new ArgumentException("Possible targets list contains duplicate Pokémon.");
            }
            // Check for fainted Pokémon in possibleTargets
            if (possibleTargets.Any(p => p.IsFainted))
            {
                throw new ArgumentException("Possible targets list contains fainted Pokémon.");
            }

            //(int minTargets, int maxTargets) = move.Target.GetPossibleTargetMinMax();

            //if (possibleTargets.Count < minTargets || possibleTargets.Count > maxTargets)
            //{
            //    throw new ArgumentException(
            //        $"The number of possible targets ({possibleTargets.Count}) is not valid for" +
            //        $"the move target type ({move.Target}). Must be between {minTargets} and {maxTargets}.");
            //}

            ValidatePossibleTargets(attacker, move, moveNormalTarget, possibleTargets);

            IsTera = isTera;
            MoveNormalTarget = moveNormalTarget;
        }

        private static void ValidatePossibleTargets(Pokemon attacker, Move move, MoveNormalTarget moveNormalTarget,
            IReadOnlyList<Pokemon> possibleTargets)
        {
            SideId sideAttacker = attacker.SideId;
            SideId sideFoe = sideAttacker.GetOppositeSide();
            SlotId slotAttacker = attacker.SlotId;
            SlotId slotAlly = slotAttacker.GetAllySlot();

            switch (move.Target)
            {
                case MoveTarget.AdjacentAlly:
                    // Must be 1 possible target
                    if (possibleTargets.Count != 1)
                    {
                        throw new InvalidOperationException("AdjacentAlly move must have exactly one" +
                                                            "possible target.");
                    }
                    // Must be on the same side as the attacker
                    if (possibleTargets[0].SideId != sideAttacker)
                    {
                        throw new InvalidOperationException("AdjacentAlly move target must be on the same side" +
                                                            "as the attacker.");
                    }
                    // Must be in the adjacent slot to the attacker
                    if (possibleTargets[0].SlotId != slotAlly)
                    {
                        throw new InvalidOperationException("AdjacentAlly move target must be in the adjacent slot" +
                                                            "to the attacker.");
                    }
                    break;
                case MoveTarget.AdjacentAllyOrSelf:
                case MoveTarget.AdjacentFoe:
                case MoveTarget.All:
                case MoveTarget.AllAdjacent:
                    throw new NotImplementedException();
                case MoveTarget.AllAdjacentFoes:
                    // Must be 1 or 2 possible targets
                    if (possibleTargets.Count is < 1 or > 2)
                    {
                        throw new InvalidOperationException("AllAdjacentFoes move must have one or two" +
                                                            "possible targets.");
                    }
                    // Must be on the opposite side as the attacker
                    if (possibleTargets.Any(p => p.SideId != sideFoe))
                    {
                        throw new InvalidOperationException("AllAdjacentFoes move targets must be on the" +
                                                            "opposite side as the attacker.");
                    }
                    break;
                case MoveTarget.Allies:
                case MoveTarget.AllySide:
                case MoveTarget.AllyTeam:
                case MoveTarget.Any:
                case MoveTarget.FoeSide:
                    throw new NotImplementedException();
                case MoveTarget.Normal:
                    // Must be exactly 1 possible target
                    if (possibleTargets.Count != 1)
                    {
                        throw new InvalidOperationException("Normal target move must have exactly one" +
                                                            "possible target.");
                    }
                    // Validate based on MoveNormalTarget
                    switch (moveNormalTarget)
                    {
                        case MoveNormalTarget.FoeSlot1:
                            // Must be on the opposite side as the attacker
                            if (possibleTargets[0].SideId != sideFoe)
                            {
                                throw new InvalidOperationException("FoeSlot1 move target must be on the" +
                                                                    "opposite side as the attacker.");
                            }
                            // Must be in slot 1
                            if (possibleTargets[0].SlotId != SlotId.Slot1)
                            {
                                throw new InvalidOperationException("FoeSlot1 move target must be in slot 1.");
                            }
                            break;
                        case MoveNormalTarget.FoeSlot2:
                            // Must be on the opposite side as the attacker
                            if (possibleTargets[0].SideId != sideFoe)
                            {
                                throw new InvalidOperationException("FoeSlot2 move target must be on the" +
                                                                    "opposite side as the attacker.");
                            }
                            // Must be in slot 2
                            if (possibleTargets[0].SlotId != SlotId.Slot2)
                            {
                                throw new InvalidOperationException("FoeSlot2 move target must be in slot 2.");
                            }
                            break;
                        case MoveNormalTarget.AllySlot1:
                            // If attacker is in slot 1, cannot target self
                            if (slotAttacker == SlotId.Slot1)
                            {
                                throw new InvalidOperationException("AllySlot1 move target cannot be the attacker.");
                            }
                            // Must be on the same side as the attacker
                            if (possibleTargets[0].SideId != sideAttacker)
                            {
                                throw new InvalidOperationException("AllySlot1 move target must be on the same" +
                                                                    "side as the attacker.");
                            }
                            // Must be in slot 1
                            if (possibleTargets[0].SlotId != SlotId.Slot1)
                            {
                                throw new InvalidOperationException("AllySlot1 move target must be in slot 1.");
                            }
                            break;
                        case MoveNormalTarget.AllySlot2:
                            // If attacker is in slot 2, cannot target self
                            if (slotAttacker == SlotId.Slot2)
                            {
                                throw new InvalidOperationException("AllySlot2 move target cannot be the attacker.");
                            }
                            // Must be on the same side as the attacker
                            if (possibleTargets[0].SideId != sideAttacker)
                            {
                                throw new InvalidOperationException("AllySlot2 move target must be on the same" +
                                                                    "side as the attacker.");
                            }
                            // Must be in slot 2
                            if (possibleTargets[0].SlotId != SlotId.Slot2)
                            {
                                throw new InvalidOperationException("AllySlot2 move target must be in slot 2.");
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(moveNormalTarget), moveNormalTarget, null);
                    }
                    break;
                case MoveTarget.RandomNormal:
                case MoveTarget.Scripted:
                    throw new NotImplementedException();
                case MoveTarget.Self:
                    // Must be exactly 1 possible target
                    if (possibleTargets.Count != 1)
                    {
                        throw new InvalidOperationException("Self target move must have exactly one possible target.");
                    }
                    // Must be the attacker
                    if (possibleTargets[0] != attacker)
                    {
                        throw new InvalidOperationException("Self target move target must be the attacker.");
                    }
                    break;
                case MoveTarget.None:
                    throw new NotImplementedException();
                case MoveTarget.Field:
                    // Must be 0 possible targets
                    if (possibleTargets.Count != 0)
                    {
                        throw new InvalidOperationException("Field target move must have no possible targets.");
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public sealed record SwitchChoice : SlotChoice
    {
        // SideId can be accessed via SwitchOutPokemon or SwitchInPokemon properties
        // SlotId can be accessed via SwitchOutPokemon or SwitchInPokemon properties
        public Pokemon SwitchOutPokemon { get; }
        public Pokemon SwitchInPokemon { get; }
        public SideId SideId => SwitchOutPokemon.SideId;
        public SlotId SwitchOutSlot => SwitchOutPokemon.SlotId;
        public SlotId SwitchInSlot => SwitchInPokemon.SlotId;

        internal SwitchChoice(Pokemon switchOutPokemon, Pokemon switchInPokemon)
        {
            SwitchOutPokemon = switchOutPokemon;
            SwitchInPokemon = switchInPokemon;
        }
    }

    public static MoveChoice CreateMove(Pokemon pokemon, Move move, bool isTera, 
        MoveNormalTarget moveNormalTarget = MoveNormalTarget.None, IReadOnlyList<Pokemon>? possibleTargets = null)
    {
        return new MoveChoice(pokemon, move, isTera, moveNormalTarget, possibleTargets ?? []);
    }

    public static SwitchChoice CreateSwitch(Pokemon switchOutPokemon, Pokemon switchInPokemon)
    {
        if (switchOutPokemon == switchInPokemon)
        {
            throw new ArgumentException("Cannot switch a Pokemon with itself.");
        }
        if (switchOutPokemon.SideId != switchInPokemon.SideId)
        {
            throw new ArgumentException("Cannot switch between different sides.");
        }
        if (switchOutPokemon.SlotId is SlotId.Slot3 or SlotId.Slot4)
        {
            throw new ArgumentException("Cannot switch out a Pokemon from a bench slot.");
        }
        if (switchInPokemon.SlotId is SlotId.Slot1 or SlotId.Slot2)
        {
            throw new ArgumentException("Only bench Pokemon can be switched in.");
        }
        if (switchInPokemon.IsFainted)
        {
            throw new ArgumentException("Cannot switch in a fainted Pokémon.");
        }
        return new SwitchChoice(switchOutPokemon, switchInPokemon);
    }

    /// <summary>
    /// Gets whether this choice represents a move action.
    /// </summary>
    public bool IsMoveChoice => this is MoveChoice;

    /// <summary>
    /// Gets whether this choice represents a switch action.
    /// </summary>
    public bool IsSwitchChoice => this is SwitchChoice;
}


public enum Choice
{
    Move1,
    Move2,
    Move3,
    Move4,

    Move1WithTera,
    Move2WithTera,
    Move3WithTera,
    Move4WithTera,

    Switch1,
    Switch2,
    Switch3,
    Switch4,
    Switch5,
    Switch6,

    Select1,
    Select2,
    Select3,
    Select4,
    Select5,
    Select6,

    TargetSlot1,
    TargetSlot2,
    TargetSlot3,
    TargetSlot4,

    TargetSide1,
    TargetSide2,

    Struggle,

    Quit,
    None,
    Invalid,
}