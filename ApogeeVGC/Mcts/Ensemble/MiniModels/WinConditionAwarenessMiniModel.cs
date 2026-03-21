using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Mcts.Ensemble.MiniModels;

/// <summary>
/// Adjusts action preferences based on whether we are ahead or behind.
/// When ahead: prefer safe plays (Protect, avoid trades, don't overcommit).
/// When behind: prefer aggressive plays (attack, tera, double-up for KOs).
/// </summary>
public sealed class WinConditionAwarenessMiniModel : IMiniModel
{
    public string Name => "WinConditionAwareness";

    public MiniModelScore[] Evaluate(
        Battle battle, SideId sideId,
        IReadOnlyList<MctsEdge> edges,
        OpponentPrediction? opponentPrediction,
        BattleInfoTracker? tracker = null)
    {
        Side ourSide = sideId == SideId.P1 ? battle.P1 : battle.P2;
        Side oppSide = sideId == SideId.P1 ? battle.P2 : battle.P1;

        float advantage = ComputeAdvantage(ourSide, oppSide);
        // advantage > 0 = we're ahead, < 0 = we're behind, ~0 = even

        float absAdv = MathF.Abs(advantage);
        if (absAdv < 0.1f)
        {
            // Game is roughly even — this model has little to say
            var neutral = new MiniModelScore[edges.Count];
            for (int i = 0; i < edges.Count; i++)
                neutral[i] = new MiniModelScore { Preference = 0.5f, Confidence = 0.1f };
            return neutral;
        }

        float confidence = MathF.Min(absAdv * 1.5f, 0.8f);
        var scores = new MiniModelScore[edges.Count];

        for (int i = 0; i < edges.Count; i++)
        {
            MctsEdge edge = edges[i];
            float pref;

            if (advantage > 0)
                pref = ScoreWhenAhead(battle, edge);
            else
                pref = ScoreWhenBehind(battle, edge);

            scores[i] = new MiniModelScore { Preference = pref, Confidence = confidence };
        }

        return scores;
    }

    private static float ComputeAdvantage(Side ourSide, Side oppSide)
    {
        // Count alive Pokemon (brought to battle, not full team)
        int ourAlive = CountAlive(ourSide);
        int oppAlive = CountAlive(oppSide);

        // HP fraction of alive Pokemon
        float ourHp = AverageHpFraction(ourSide);
        float oppHp = AverageHpFraction(oppSide);

        // Pokemon count advantage (normalized: +1 = up by 1 mon equivalent)
        float countAdv = (ourAlive - oppAlive) * 0.3f;

        // HP advantage
        float hpAdv = (ourHp - oppHp) * 0.4f;

        return countAdv + hpAdv;
    }

    private static float ScoreWhenAhead(Battle battle, MctsEdge edge)
    {
        float pref = 0.5f;
        LegalAction a = edge.ActionA;

        // Prefer Protect — preserve the lead
        if (ProtectDetector.IsProtectMove(a)) pref += 0.15f;

        // Slight preference for safe attacks (non-risky)
        if (a.ChoiceType == ChoiceType.Move && !ProtectDetector.IsProtectMove(a))
            pref += 0.05f;

        // Penalize unnecessary tera — don't waste resources when winning
        if (a.Terastallize.HasValue) pref -= 0.15f;

        if (edge.ActionB.HasValue)
        {
            LegalAction b = edge.ActionB.Value;
            if (ProtectDetector.IsProtectMove(b)) pref += 0.1f;
            if (b.Terastallize.HasValue) pref -= 0.1f;
        }

        return Math.Clamp(pref, 0f, 1f);
    }

    private static float ScoreWhenBehind(Battle battle, MctsEdge edge)
    {
        float pref = 0.5f;
        LegalAction a = edge.ActionA;

        // Prefer aggressive moves
        if (a.ChoiceType == ChoiceType.Move && !ProtectDetector.IsProtectMove(a))
        {
            if (battle.Library.Moves.TryGetValue(a.MoveId, out Move? move) &&
                move.Category != MoveCategory.Status && move.BasePower >= 60)
                pref += 0.15f;
        }

        // Tera is acceptable when behind — use resources to come back
        if (a.Terastallize.HasValue) pref += 0.05f;

        // Penalize Protect when behind — passive play bleeds the game
        if (ProtectDetector.IsProtectMove(a)) pref -= 0.15f;

        // Penalize switches when behind — losing tempo
        if (ProtectDetector.IsSwitchAction(a)) pref -= 0.1f;

        if (edge.ActionB.HasValue)
        {
            LegalAction b = edge.ActionB.Value;
            if (b.ChoiceType == ChoiceType.Move && !ProtectDetector.IsProtectMove(b))
            {
                if (battle.Library.Moves.TryGetValue(b.MoveId, out Move? move) &&
                    move.Category != MoveCategory.Status && move.BasePower >= 60)
                    pref += 0.1f;
            }
            if (ProtectDetector.IsProtectMove(b)) pref -= 0.1f;
        }

        return Math.Clamp(pref, 0f, 1f);
    }

    private static int CountAlive(Side side)
    {
        int count = 0;
        foreach (Pokemon pokemon in side.Pokemon)
            if (!pokemon.Fainted) count++;
        return count;
    }

    private static float AverageHpFraction(Side side)
    {
        float totalHp = 0f;
        int count = 0;
        foreach (Pokemon pokemon in side.Pokemon)
        {
            if (pokemon.Fainted) continue;
            totalHp += (float)pokemon.Hp / pokemon.MaxHp;
            count++;
        }
        return count > 0 ? totalHp / count : 0f;
    }
}
