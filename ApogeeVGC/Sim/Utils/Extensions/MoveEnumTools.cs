//using ApogeeVGC.Sim.Core;
//using ApogeeVGC.Sim.Moves;
//using ApogeeVGC.Sim.PokemonClasses;

//namespace ApogeeVGC.Sim.Utils.Extensions;

//public static class MoveEnumTools
//{
//    public static MoveNormalTarget CalculateMoveNormalTarget(Pokemon attacker, Move move, Pokemon defender)
//    {
//        if (move.Target != MoveTarget.Normal)
//        {
//            throw new ArgumentException("Move target must be Normal for this calculation.");
//        }
//        if (defender.IsFainted)
//        {
//            throw new ArgumentException("Defender cannot be fainted.");
//        }

//        bool isAlly = attacker.Trainer == defender.Trainer;
//        if (isAlly)
//        {
//            return defender.SlotId == SlotId.Slot1 ? MoveNormalTarget.AllySlot1 : MoveNormalTarget.AllySlot2;
//        }

//        return defender.SlotId == SlotId.Slot1 ? MoveNormalTarget.FoeSlot1 : MoveNormalTarget.FoeSlot2;
//    }
//}