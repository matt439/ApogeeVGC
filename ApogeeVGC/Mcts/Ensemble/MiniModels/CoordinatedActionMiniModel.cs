using ApogeeVGC.Data;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Utils.Extensions;

namespace ApogeeVGC.Mcts.Ensemble.MiniModels;

/// <summary>
/// Scores joint actions that synergize between the two active slots.
/// Rewards coordinated plays: double-targeting, Fake Out + setup,
/// redirect + nuke, spread + single-target combos.
/// Penalizes anti-synergies: double switching, double Protect.
/// </summary>
public sealed class CoordinatedActionMiniModel : IMiniModel
{
    private static readonly TypeChart TypeChart = new();

    private static readonly HashSet<MoveId> RedirectMoves =
    [
        MoveId.FollowMe, MoveId.RagePowder,
    ];

    private static readonly HashSet<MoveId> FakeOutMoves =
    [
        MoveId.FakeOut,
    ];

    private static readonly HashSet<MoveId> SetupMoves =
    [
        MoveId.SwordsDance, MoveId.CalmMind, MoveId.NastyPlot,
        MoveId.DragonDance, MoveId.QuiverDance, MoveId.IronDefense,
        MoveId.Tailwind, MoveId.TrickRoom, MoveId.AuroraVeil,
        MoveId.LightScreen, MoveId.Reflect,
    ];

    public string Name => "CoordinatedAction";

    public MiniModelScore[] Evaluate(
        Battle battle, SideId sideId,
        IReadOnlyList<MctsEdge> edges,
        OpponentPrediction? opponentPrediction,
        BattleInfoTracker? tracker = null)
    {
        Side ourSide = sideId == SideId.P1 ? battle.P1 : battle.P2;
        Side oppSide = sideId == SideId.P1 ? battle.P2 : battle.P1;

        var scores = new MiniModelScore[edges.Count];
        float maxBonus = 0f;
        float[] bonuses = new float[edges.Count];

        for (int i = 0; i < edges.Count; i++)
        {
            MctsEdge edge = edges[i];
            if (!edge.ActionB.HasValue)
            {
                // Single-slot decision — no coordination possible
                scores[i] = new MiniModelScore { Preference = 0.5f, Confidence = 0.1f };
                continue;
            }

            float bonus = ScoreCoordination(battle, ourSide, oppSide, edge);
            bonuses[i] = bonus;
            if (MathF.Abs(bonus) > maxBonus) maxBonus = MathF.Abs(bonus);
        }

        if (maxBonus < 0.01f)
        {
            // No meaningful coordination detected
            for (int i = 0; i < edges.Count; i++)
                scores[i] = new MiniModelScore { Preference = 0.5f, Confidence = 0.1f };
            return scores;
        }

        for (int i = 0; i < edges.Count; i++)
        {
            if (!edges[i].ActionB.HasValue)
                continue;

            // Map [-maxBonus, +maxBonus] to [0, 1]
            float pref = 0.5f + 0.5f * bonuses[i] / maxBonus;
            scores[i] = new MiniModelScore { Preference = pref, Confidence = 0.7f };
        }

        return scores;
    }

    private float ScoreCoordination(
        Battle battle, Side ourSide, Side oppSide, MctsEdge edge)
    {
        LegalAction a = edge.ActionA;
        LegalAction b = edge.ActionB!.Value;
        float bonus = 0f;

        // ── Penalty: double switch (wastes tempo) ──
        if (ProtectDetector.IsSwitchAction(a) && ProtectDetector.IsSwitchAction(b))
            bonus -= 0.6f;

        // ── Penalty: double Protect (rarely correct) ──
        if (ProtectDetector.IsProtectMove(a) && ProtectDetector.IsProtectMove(b))
            bonus -= 0.4f;

        // ── Bonus: double-target same opponent for KO ──
        bonus += ScoreDoubleTarget(battle, ourSide, oppSide, a, b);

        // ── Bonus: Fake Out + power move / setup ──
        bonus += ScoreFakeOutCombo(battle, a, b);

        // ── Bonus: redirect + strong single-target move ──
        bonus += ScoreRedirectCombo(battle, ourSide, oppSide, a, b);

        // ── Bonus: spread + single-target (covers both opponents) ──
        bonus += ScoreSpreadCombo(battle, a, b);

        return bonus;
    }

    private float ScoreDoubleTarget(
        Battle battle, Side ourSide, Side oppSide,
        LegalAction a, LegalAction b)
    {
        if (a.ChoiceType != ChoiceType.Move || b.ChoiceType != ChoiceType.Move)
            return 0f;

        // Both single-target moves hitting the same opponent
        if (!battle.Library.Moves.TryGetValue(a.MoveId, out Move? moveA)) return 0f;
        if (!battle.Library.Moves.TryGetValue(b.MoveId, out Move? moveB)) return 0f;

        if (moveA.Category == MoveCategory.Status || moveB.Category == MoveCategory.Status)
            return 0f;

        bool aSpread = moveA.Target is MoveTarget.AllAdjacentFoes or MoveTarget.AllAdjacent;
        bool bSpread = moveB.Target is MoveTarget.AllAdjacentFoes or MoveTarget.AllAdjacent;

        if (aSpread || bSpread) return 0f; // Not single-target pairing

        // Check if targeting the same slot (+1 = foe slot 1, -1 = foe slot 2 in VGC)
        if (a.TargetLoc == b.TargetLoc && a.TargetLoc != 0)
        {
            // Bonus scales with combined estimated damage vs target HP
            Pokemon? target = GetTargetPokemon(oppSide, a.TargetLoc);
            if (target == null || target.Fainted) return 0f;

            float dmgA = EstimateRawDamage(battle, ourSide, target, a, 0);
            float dmgB = EstimateRawDamage(battle, ourSide, target, b, 1);
            float combined = dmgA + dmgB;

            if (combined >= target.Hp)
                return 0.8f; // Guaranteed KO with double-up
            if (combined >= target.Hp * 0.7f)
                return 0.3f; // Near KO
        }

        return 0f;
    }

    private static float ScoreFakeOutCombo(Battle battle, LegalAction a, LegalAction b)
    {
        float bonus = 0f;

        // A = Fake Out, B = setup or power move
        if (a.ChoiceType == ChoiceType.Move && FakeOutMoves.Contains(a.MoveId))
        {
            if (b.ChoiceType == ChoiceType.Move)
            {
                if (SetupMoves.Contains(b.MoveId))
                    bonus += 0.5f; // Fake Out + setup is great
                else if (battle.Library.Moves.TryGetValue(b.MoveId, out Move? m) &&
                         m.Category != MoveCategory.Status && m.BasePower >= 80)
                    bonus += 0.3f; // Fake Out + strong attack
            }
        }

        // B = Fake Out, A = setup or power move
        if (b.ChoiceType == ChoiceType.Move && FakeOutMoves.Contains(b.MoveId))
        {
            if (a.ChoiceType == ChoiceType.Move)
            {
                if (SetupMoves.Contains(a.MoveId))
                    bonus += 0.5f;
                else if (battle.Library.Moves.TryGetValue(a.MoveId, out Move? m) &&
                         m.Category != MoveCategory.Status && m.BasePower >= 80)
                    bonus += 0.3f;
            }
        }

        return bonus;
    }

    private static float ScoreRedirectCombo(
        Battle battle, Side ourSide, Side oppSide,
        LegalAction a, LegalAction b)
    {
        // A = redirect, B = strong single-target
        if (a.ChoiceType == ChoiceType.Move && RedirectMoves.Contains(a.MoveId) &&
            b.ChoiceType == ChoiceType.Move)
        {
            if (battle.Library.Moves.TryGetValue(b.MoveId, out Move? m) &&
                m.Category != MoveCategory.Status && m.BasePower >= 80 &&
                m.Target is not (MoveTarget.AllAdjacentFoes or MoveTarget.AllAdjacent))
                return 0.4f;
        }

        // B = redirect, A = strong single-target
        if (b.ChoiceType == ChoiceType.Move && RedirectMoves.Contains(b.MoveId) &&
            a.ChoiceType == ChoiceType.Move)
        {
            if (battle.Library.Moves.TryGetValue(a.MoveId, out Move? m) &&
                m.Category != MoveCategory.Status && m.BasePower >= 80 &&
                m.Target is not (MoveTarget.AllAdjacentFoes or MoveTarget.AllAdjacent))
                return 0.4f;
        }

        return 0f;
    }

    private static float ScoreSpreadCombo(Battle battle, LegalAction a, LegalAction b)
    {
        if (a.ChoiceType != ChoiceType.Move || b.ChoiceType != ChoiceType.Move)
            return 0f;

        if (!battle.Library.Moves.TryGetValue(a.MoveId, out Move? moveA)) return 0f;
        if (!battle.Library.Moves.TryGetValue(b.MoveId, out Move? moveB)) return 0f;

        bool aSpread = moveA.Target is MoveTarget.AllAdjacentFoes or MoveTarget.AllAdjacent;
        bool bSpread = moveB.Target is MoveTarget.AllAdjacentFoes or MoveTarget.AllAdjacent;

        // One spread + one single-target — good coverage
        if ((aSpread && !bSpread && moveB.Category != MoveCategory.Status) ||
            (!aSpread && bSpread && moveA.Category != MoveCategory.Status))
            return 0.15f;

        return 0f;
    }

    private static Pokemon? GetTargetPokemon(Side oppSide, int targetLoc)
    {
        // targetLoc: +1 = opponent slot 0, +2 = opponent slot 1
        int idx = targetLoc > 0 ? targetLoc - 1 : 0;
        return idx < oppSide.Active.Count ? oppSide.Active[idx] : null;
    }

    private float EstimateRawDamage(
        Battle battle, Side ourSide, Pokemon target,
        LegalAction action, int slotIndex)
    {
        Pokemon? attacker = slotIndex < ourSide.Active.Count ? ourSide.Active[slotIndex] : null;
        if (attacker == null || attacker.Fainted) return 0f;

        if (!battle.Library.Moves.TryGetValue(action.MoveId, out Move? move))
            return 0f;

        if (move.Category == MoveCategory.Status || move.BasePower <= 0)
            return 0f;

        float effectiveness = (float)TypeChart
            .GetMoveEffectiveness(target.Types, move.Type)
            .ToMultiplier();
        if (effectiveness == 0f) return 0f;

        float stab = 1f;
        if (move.Type is not (MoveType.Stellar or MoveType.Unknown))
            if (attacker.HasType(move.Type.ConvertToPokemonType()))
                stab = 1.5f;

        float atkStat = move.Category == MoveCategory.Physical
            ? attacker.StoredStats.Atk * (float)Sim.Stats.BoostsTable.CalculateRegularStatMultiplier(attacker.Boosts.Atk)
            : attacker.StoredStats.SpA * (float)Sim.Stats.BoostsTable.CalculateRegularStatMultiplier(attacker.Boosts.SpA);
        float defStat = move.Category == MoveCategory.Physical
            ? target.StoredStats.Def * (float)Sim.Stats.BoostsTable.CalculateRegularStatMultiplier(target.Boosts.Def)
            : target.StoredStats.SpD * (float)Sim.Stats.BoostsTable.CalculateRegularStatMultiplier(target.Boosts.SpD);

        if (defStat <= 0f) defStat = 1f;

        return ((2f * 50f / 5f + 2f) * move.BasePower * atkStat / defStat / 50f + 2f)
               * effectiveness * stab * 0.85f;
    }
}
