using ApogeeVGC.Sim.Core;

namespace ApogeeVGC.Sim.BattleClasses.Messages;

/// <summary>
/// Base interface for all messages sent from players to the battle.
/// </summary>
public interface IPlayerMessage
{
    SideId SideId { get; }
}
