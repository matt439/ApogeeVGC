using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.PokemonClasses;

public partial class Pokemon
{
    public MoveTargets GetMoveTargets(ActiveMove move, Pokemon target)
    {
        List<Pokemon> targets = [];

        switch (move.Target)
        {
            case MoveTarget.All:
            case MoveTarget.FoeSide:
            case MoveTarget.AllySide:
            case MoveTarget.AllyTeam:
            {
                // Add allies if not a foe-only move
                if (move.Target != MoveTarget.FoeSide)
                {
                    targets.AddRange(AlliesAndSelf());
                }

                // Add foes if not an ally-only move
                if (move.Target != MoveTarget.AllySide && move.Target != MoveTarget.AllyTeam)
                {
                    targets.AddRange(Foes(all: true));
                }

                // Retarget if the original target isn't in the list
                if (targets.Count > 0 && !targets.Contains(target))
                {
                    Battle.RetargetLastMove(targets[^1]);
                }

                break;
            }

            case MoveTarget.AllAdjacent:
                targets.AddRange(AdjacentAllies());
                goto case MoveTarget.AllAdjacentFoes; // Fall through

            case MoveTarget.AllAdjacentFoes:
                targets.AddRange(AdjacentFoes());
                if (targets.Count > 0 && !targets.Contains(target))
                {
                    Battle.RetargetLastMove(targets[^1]);
                }

                break;

            case MoveTarget.Allies:
                targets = AlliesAndSelf();
                break;

            default:
            {
                Pokemon selectedTarget = target;

                // If targeted foe faints, retarget (except in free-for-all)
                if ((target.Fainted && !target.IsAlly(this)))
                {
                    Pokemon? possibleTarget = Battle.GetRandomTarget(this, move);
                    if (possibleTarget == null)
                    {
                        return new MoveTargets
                        {
                            Targets = [],
                            PressureTargets = [],
                        };
                    }

                    target = possibleTarget;
                }

                // Handle redirection for multi-Pokémon battles
                if (Battle.ActivePerHalf > 1 && move.TracksTarget != true)
                {
                    // Check if this is a charging turn (first turn of two-turn moves)
                    bool isCharging = move.Flags.Charge == true &&
                                      !Volatiles.ContainsKey(ConditionId.TwoTurnMove) &&
                                      // Solar Beam/Blade skip charging in sun
                                      !(move.Id is MoveId.SolarBeam or MoveId.SolarBlade &&
                                        EffectiveWeather() is ConditionId.SunnyDay
                                            or ConditionId.DesolateLand)
                                      // Electro Shot skips charging in rain
                                      && !(move.Id == MoveId.ElectroShot &&
                                           EffectiveWeather() is ConditionId.RainDance
                                               or ConditionId.PrimordialSea) &&
                                      // Power Herb allows skipping charge
                                      !HasItem(ItemId.PowerHerb);

                    // Apply redirection (Follow Me, Rage Powder, etc.)
                    if (!isCharging && !(target.BeingCalledBack || target.SwitchFlag.IsTrue()))
                    {
                        RelayVar? redirectResult = Battle.PriorityEvent(
                            EventId.RedirectTarget,
                            this,
                            this,
                            move,
                            target
                        );

                        if (redirectResult is PokemonRelayVar prv)
                        {
                            target = prv.Pokemon;
                        }
                    }
                }

                // Handle smart targeting (Dragon Darts)
                if (move.SmartTarget == true)
                {
                    targets = GetSmartTargets(target, move);
                    target = targets[0];
                }
                else
                {
                    targets.Add(target);
                }

                // Fail if target fainted (unless it's a future move like Future Sight)
                if (target.Fainted && move.Flags.FutureMove != true)
                {
                    return new MoveTargets
                    {
                        Targets = [],
                        PressureTargets = [],
                    };
                }

                // Update battle log if target changed
                if (selectedTarget != target)
                {
                    Battle.RetargetLastMove(target);
                }

                break;
            }
        }

        // Resolve apparent targets for Pressure ability
        var pressureTargets = targets;

        if (move.Target == MoveTarget.FoeSide)
        {
            // FoeSide moves don't trigger Pressure
            pressureTargets = [];
        }

        if (move.Flags.MustPressure == true)
        {
            // Some moves always trigger Pressure on all foes
            pressureTargets = Foes();
        }

        return new MoveTargets
        {
            Targets = targets,
            PressureTargets = pressureTargets,
        };
    }

    /// <summary>
    /// Get targets for Dragon Darts - determines if the move should hit a target and its adjacent ally.
    /// </summary>
    public List<Pokemon> GetSmartTargets(Pokemon target, ActiveMove move)
    {
        // Get the first adjacent ally of the target
        Pokemon? target2 = target.AdjacentAllies().FirstOrDefault();

        // If the adjacent ally doesn't exist, is the user, or is fainted
        if (target2 == null || target2 == this || target2.Hp <= 0)
        {
            move.SmartTarget = false;
            return [target];
        }

        // If the primary target is fainted
        if (target.Hp <= 0)
        {
            move.SmartTarget = false;
            return [target2];
        }

        // Return both targets (primary target and its adjacent ally)
        return [target, target2];
    }

    public Pokemon GetAtLoc(int targetLoc)
    {
        // Determine which side based on targetLoc sign
        Side side = Battle.Sides[targetLoc < 0 ? Side.N % 2 : (Side.N + 1) % 2];

        // Use absolute value for position calculation
        targetLoc = Math.Abs(targetLoc);

        // Handle wrap-around for multi-battle formats (e.g., if position exceeds active Pokemon count)
        if (targetLoc > side.Active.Count)
        {
            targetLoc -= side.Active.Count;
            side = Battle.Sides[side.N + 2];
        }

        // Return the Pokemon at the calculated position (adjust for 0-based indexing)
        return side.GetActiveAt(targetLoc - 1);
    }

    /// <summary>
    /// Returns a relative location: 1-3, positive for foe, and negative for ally.
    /// Use <see cref="GetAtLoc"/> to reverse this operation.
    /// </summary>
    /// <param name="target">The target Pokémon to get the location of</param>
    /// <returns>Relative location as an integer (negative for allies, positive for foes)</returns>
    public int GetLocOf(Pokemon target)
    {
        // Calculate position offset based on which half of the field the target is on
        int positionOffset = (int)Math.Floor(target.Side.N / 2.0) * target.Side.Active.Count;

        // Calculate 1-indexed position
        int position = target.Position + positionOffset + 1;

        // Check if both Pokemon are on the same half of the field
        bool sameHalf = (Side.N % 2) == (target.Side.N % 2);

        // Return negative for allies, positive for foes
        return sameHalf ? -position : position;
    }

    public List<Pokemon> AlliesAndSelf()
    {
        return Side.Allies();
    }

    public List<Pokemon> Allies()
    {
        var allies = new List<Pokemon>(Side.Active.Count);
        foreach (var p in Side.Active)
        {
            if (p != null && p.Hp > 0 && p != this)
                allies.Add(p);
        }
        return allies;
    }

    public List<Pokemon> AdjacentAllies()
    {
        var allies = new List<Pokemon>(Side.Active.Count);
        foreach (var p in Side.Active)
        {
            if (p != null && p.Hp > 0 && IsAdjacent(p))
                allies.Add(p);
        }
        return allies;
    }

    public List<Pokemon> Foes(bool all = false)
    {
        return Side.Foes(all);
    }

    public List<Pokemon> AdjacentFoes()
    {
        if (Battle.ActivePerHalf <= 2)
            return Side.Foes();

        var foes = new List<Pokemon>(Side.Foe.Active.Count);
        foreach (var p in Side.Foe.Active)
        {
            if (p != null && p.Hp > 0 && IsAdjacent(p))
                foes.Add(p);
        }
        return foes;
    }

    public bool IsAlly(Pokemon? pokemon = null)
    {
        if (pokemon == null) return false;
        return Side == pokemon.Side;
    }

    public bool IsAdjacent(Pokemon pokemon2)
    {
        if (Fainted || pokemon2.Fainted) return false;
        if (Battle.ActivePerHalf <= 2) return this != pokemon2;
        if (Side == pokemon2.Side) return Math.Abs(Position - pokemon2.Position) == 1;
        return Math.Abs(Position + pokemon2.Position + 1 - Side.Active.Count) <= 1;
    }
}