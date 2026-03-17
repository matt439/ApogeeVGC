using ApogeeVGC.Mcts;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;

namespace ApogeeVGC.LiveAssist;

/// <summary>
/// Random player for Showdown: picks a uniformly random legal action per slot.
/// Useful as a baseline for thesis evaluation.
/// </summary>
public sealed class ShowdownPlayerRandom(int? seed = null) : IShowdownPlayer
{
    private readonly Random _rng = seed.HasValue ? new Random(seed.Value) : new Random();

    public string Name => "Random";

    public (LegalAction BestA, LegalAction? BestB) ChooseBattle(
        BattlePerspective perspective,
        LegalActionSet actions,
        bool[] maskA,
        bool[] maskB)
    {
        LegalAction bestA = actions.SlotA[_rng.Next(actions.SlotA.Count)];

        LegalAction? bestB = null;
        if (actions.SlotB.Count > 0)
        {
            // Filter out duplicate switch target
            var validB = actions.SlotB;
            if (bestA.ChoiceType == ChoiceType.Switch)
            {
                var filtered = new List<LegalAction>();
                foreach (LegalAction b in actions.SlotB)
                {
                    if (b.ChoiceType != ChoiceType.Switch || b.SwitchIndex != bestA.SwitchIndex)
                        filtered.Add(b);
                }
                if (filtered.Count > 0)
                    validB = filtered;
            }

            bestB = validB[_rng.Next(validB.Count)];
        }

        return (bestA, bestB);
    }
}
