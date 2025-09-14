using ApogeeVGC.Player;
using ApogeeVGC.Sim.Choices;

namespace ApogeeVGC.Sim.BattleClasses;

internal interface IBattleMctsOperations
{
    void ApplyChoiceSync(PlayerId playerId, BattleChoice choice);
    Random GetRandom();
    GameplayExecutionStage GetExecutionStage();
    void SetExecutionStage(GameplayExecutionStage stage);
}