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
/// Prefers Protect or switch when our active Pokemon are in KO range.
/// Confidence scales with how threatened we are.
/// Uses opponent prediction model to estimate attack probability.
/// </summary>
public sealed class KOAvoidanceMiniModel : IMiniModel
{
    private static readonly TypeChart TypeChart = new();

    public string Name => "KOAvoidance";

    public MiniModelScore[] Evaluate(
        Battle battle, SideId sideId,
        IReadOnlyList<MctsEdge> edges,
        OpponentPrediction? opponentPrediction)
    {
        Side ourSide = sideId == SideId.P1 ? battle.P1 : battle.P2;
        Side oppSide = sideId == SideId.P1 ? battle.P2 : battle.P1;

        // Check which of our active Pokemon are in KO range
        bool[] threatened = new bool[ourSide.Active.Count];
        float maxThreat = 0f;

        for (int i = 0; i < ourSide.Active.Count; i++)
        {
            Pokemon? ours = ourSide.Active[i];
            if (ours == null || ours.Fainted) continue;

            float threat = EstimateThreatLevel(battle, oppSide, ours);
            if (threat >= 0.8f) // Can be KO'd
            {
                threatened[i] = true;
                maxThreat = MathF.Max(maxThreat, threat);
            }
        }

        if (maxThreat < 0.8f)
        {
            // Not threatened — this model is irrelevant
            var lowScores = new MiniModelScore[edges.Count];
            for (int i = 0; i < edges.Count; i++)
                lowScores[i] = new MiniModelScore { Preference = 0.5f, Confidence = 0.05f };
            return lowScores;
        }

        // Score edges: prefer Protect/switch for threatened Pokemon
        var scores = new MiniModelScore[edges.Count];
        float confidence = MathF.Min(maxThreat, 1f) * 0.8f;

        for (int i = 0; i < edges.Count; i++)
        {
            MctsEdge edge = edges[i];
            float pref = 0.3f; // Default: neutral

            // Check if action protects threatened Pokemon
            if (threatened.Length > 0 && threatened[0])
                pref += ScoreDefensiveAction(edge.ActionA);

            if (edge.ActionB.HasValue && threatened.Length > 1 && threatened[1])
                pref += ScoreDefensiveAction(edge.ActionB.Value);

            scores[i] = new MiniModelScore
            {
                Preference = MathF.Min(pref, 1f),
                Confidence = confidence,
            };
        }

        return scores;
    }

    private static float ScoreDefensiveAction(LegalAction action)
    {
        if (action.ChoiceType is ChoiceType.Switch or ChoiceType.InstaSwitch)
            return 0.3f; // Switching avoids the KO

        if (action.ChoiceType == ChoiceType.Move)
        {
            // Check if it's a Protect-type move
            if (action.MoveId is MoveId.Protect or MoveId.Detect
                or MoveId.BanefulBunker
                or MoveId.SpikyShield or MoveId.SilkTrap
                or MoveId.BurningBulwark)
                return 0.35f;
        }

        return 0f;
    }

    /// <summary>
    /// Estimate how threatened a Pokemon is (0 = safe, 1+ = in KO range).
    /// Returns max(estimated damage / HP) across all opponent active moves.
    /// </summary>
    private static float EstimateThreatLevel(
        Battle battle, Side oppSide, Pokemon target)
    {
        float maxRatio = 0f;

        foreach (Pokemon? attacker in oppSide.Active)
        {
            if (attacker == null || attacker.Fainted) continue;

            foreach (MoveSlot moveSlot in attacker.MoveSlots)
            {
                if (!battle.Library.Moves.TryGetValue(moveSlot.Id, out Move? move))
                    continue;

                if (move.Category == MoveCategory.Status || move.BasePower <= 0)
                    continue;

                float effectiveness = (float)TypeChart
                    .GetMoveEffectiveness(target.Types, move.Type)
                    .ToMultiplier();
                if (effectiveness == 0f) continue;

                float stab = 1f;
                if (move.Type is not (MoveType.Stellar or MoveType.Unknown))
                    if (attacker.HasType(move.Type.ConvertToPokemonType()))
                        stab = 1.5f;

                float atkStat = move.Category == MoveCategory.Physical
                    ? attacker.StoredStats.Atk * (float)BoostsTable.CalculateRegularStatMultiplier(attacker.Boosts.Atk)
                    : attacker.StoredStats.SpA * (float)BoostsTable.CalculateRegularStatMultiplier(attacker.Boosts.SpA);
                float defStat = move.Category == MoveCategory.Physical
                    ? target.StoredStats.Def * (float)BoostsTable.CalculateRegularStatMultiplier(target.Boosts.Def)
                    : target.StoredStats.SpD * (float)BoostsTable.CalculateRegularStatMultiplier(target.Boosts.SpD);

                if (defStat <= 0f) defStat = 1f;

                float damage = ((2f * 50f / 5f + 2f) * move.BasePower * atkStat / defStat / 50f + 2f)
                               * effectiveness * stab;
                damage *= 0.85f; // Conservative roll

                float ratio = damage / MathF.Max(target.Hp, 1f);
                if (ratio > maxRatio) maxRatio = ratio;
            }
        }

        return maxRatio;
    }
}
