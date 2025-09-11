using ApogeeVGC.Sim.Choices;

namespace ApogeeVGC.Player;

public interface IPlayer
{
    PlayerId PlayerId { get; }

    BattleChoice GetNextChoice(BattleChoice[] availableChoices);
}


