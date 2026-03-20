using ApogeeVGC.Data;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Extensions;

namespace ApogeeVGC.Mcts.Ensemble.MiniModels;

/// <summary>
/// Strongly prefers actions that can KO an opponent Pokemon.
/// Taking a KO is almost always the correct play in VGC.
/// High confidence when KOs are available, very low otherwise.
/// </summary>
public sealed class KOSeekingMiniModel : IMiniModel
{
    private static readonly TypeChart TypeChart = new();

    public string Name => "KOSeeking";

    public MiniModelScore[] Evaluate(
        Battle battle, SideId sideId,
        IReadOnlyList<MctsEdge> edges,
        OpponentPrediction? opponentPrediction,
        BattleInfoTracker? tracker = null)
    {
        Side ourSide = sideId == SideId.P1 ? battle.P1 : battle.P2;
        Side oppSide = sideId == SideId.P1 ? battle.P2 : battle.P1;

        var scores = new MiniModelScore[edges.Count];
        bool anyKO = false;

        for (int i = 0; i < edges.Count; i++)
        {
            MctsEdge edge = edges[i];
            int koCount = CountPotentialKOs(battle, ourSide, oppSide, edge);

            if (koCount > 0)
            {
                anyKO = true;
                // Double KO > single KO
                scores[i] = new MiniModelScore
                {
                    Preference = koCount >= 2 ? 1.0f : 0.85f,
                    Confidence = 0.95f,
                };
            }
            else
            {
                scores[i] = new MiniModelScore
                {
                    Preference = 0.3f,
                    Confidence = 0.1f, // Low confidence when no KOs available
                };
            }
        }

        // If no edge has a KO, lower all confidences
        if (!anyKO)
        {
            for (int i = 0; i < scores.Length; i++)
                scores[i] = new MiniModelScore { Preference = 0.5f, Confidence = 0.05f };
        }

        return scores;
    }

    private static int CountPotentialKOs(
        Battle battle, Side ourSide, Side oppSide, MctsEdge edge)
    {
        int kos = 0;
        kos += CanKO(battle, ourSide, oppSide, edge.ActionA, 0) ? 1 : 0;
        if (edge.ActionB.HasValue)
            kos += CanKO(battle, ourSide, oppSide, edge.ActionB.Value, 1) ? 1 : 0;
        return kos;
    }

    private static bool CanKO(
        Battle battle, Side ourSide, Side oppSide,
        LegalAction action, int slotIndex)
    {
        if (action.ChoiceType != ChoiceType.Move) return false;

        Pokemon? attacker = slotIndex < ourSide.Active.Count ? ourSide.Active[slotIndex] : null;
        if (attacker == null || attacker.Fainted) return false;

        if (!battle.Library.Moves.TryGetValue(action.MoveId, out Move? move))
            return false;

        if (move.Category == MoveCategory.Status || move.BasePower <= 0)
            return false;

        bool isSpread = move.Target is MoveTarget.AllAdjacentFoes or MoveTarget.AllAdjacent;
        float spreadPenalty = isSpread && CountAlive(oppSide) > 1 ? 0.75f : 1f;

        foreach (Pokemon? target in oppSide.Active)
        {
            if (target == null || target.Fainted) continue;

            float effectiveness = (float)TypeChart
                .GetMoveEffectiveness(target.Types, move.Type)
                .ToMultiplier();
            if (effectiveness == 0f) continue;

            float stab = 1f;
            if (move.Type is not (MoveType.Stellar or MoveType.Unknown))
                if (attacker.HasType(move.Type.ConvertToPokemonType()))
                    stab = 1.5f;

            // Rough damage estimate (level 50, neutral nature, 85 base stats)
            float atkStat = move.Category == MoveCategory.Physical
                ? attacker.StoredStats.Atk * (float)BoostsTable.CalculateRegularStatMultiplier(attacker.Boosts.Atk)
                : attacker.StoredStats.SpA * (float)BoostsTable.CalculateRegularStatMultiplier(attacker.Boosts.SpA);
            float defStat = move.Category == MoveCategory.Physical
                ? target.StoredStats.Def * (float)BoostsTable.CalculateRegularStatMultiplier(target.Boosts.Def)
                : target.StoredStats.SpD * (float)BoostsTable.CalculateRegularStatMultiplier(target.Boosts.SpD);

            if (defStat <= 0f) defStat = 1f;

            float damage = ((2f * 50f / 5f + 2f) * move.BasePower * atkStat / defStat / 50f + 2f)
                           * effectiveness * stab * spreadPenalty;

            // Use 85% roll (conservative estimate)
            damage *= 0.85f;

            if (damage >= target.Hp)
                return true;
        }

        return false;
    }

    private static int CountAlive(Side side)
    {
        int count = 0;
        foreach (Pokemon? p in side.Active)
            if (p != null && !p.Fainted) count++;
        return count;
    }
}
