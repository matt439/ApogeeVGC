using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Utils;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Player;

public class PlayerRandom(SideId sideId, PlayerOptions options, IBattleController battleController)
    : IPlayer
{
    public SideId SideId { get; } = sideId;
    public PlayerOptions Options { get; } = options;
    public PlayerUiType UiType => PlayerUiType.None;
    public IBattleController BattleController { get; } = battleController;
    public bool PrintDebug { get; } = options.PrintDebug;

    private readonly Prng _random = options.Seed is null ? new Prng(null) : new Prng(options.Seed);

    // Synchronous version for MCTS and fast simulations
    public Choice GetChoiceSync(IChoiceRequest choiceRequest, BattleRequestType requestType,
        BattlePerspective perspective)
    {
        if (PrintDebug)
        {
            Console.WriteLine($"[PlayerRandom.GetChoiceSync] Called for {SideId}");
        }

        // Use the same logic as GetNextChoiceFromAll
        return GetNextChoiceFromAll(choiceRequest);
    }

    // Fast sync version for MCTS rollouts (IPlayer)
    public Choice GetNextChoiceSync(IChoiceRequest choice, BattlePerspective perspective)
    {
        return GetNextChoiceFromAll(choice);
    }

    // Simplified async version (IPlayer)
    public Task<Choice> GetNextChoiceAsync(IChoiceRequest choiceRequest,
        BattlePerspective perspective,
        CancellationToken cancellationToken)
    {
        Choice choice = GetNextChoiceFromAll(choiceRequest);
        ChoiceSubmitted?.Invoke(this, choice);
        return Task.FromResult(choice);
    }

    // Full async version for backward compatibility (IPlayer)
    public Task<Choice> GetNextChoiceAsync(IChoiceRequest choiceRequest,
        BattleRequestType requestType,
        BattlePerspective perspective, CancellationToken cancellationToken)
    {
        Choice choice = GetNextChoiceFromAll(choiceRequest);
        ChoiceRequested?.Invoke(this, new ChoiceRequestEventArgs
        {
            Choice = choiceRequest,
            TimeLimit = TimeSpan.FromSeconds(45),
            RequestTime = DateTime.UtcNow,
        });
        ChoiceSubmitted?.Invoke(this, choice);
        return Task.FromResult(choice);
    }

    public void UpdateUi(BattlePerspective perspective)
    {
        // Random player doesn't have a UI to update
    }

    public void UpdateEvents(IEnumerable<BattleEvent> events)
    {
        // Random player doesn't need to receive events
    }

    // Events from interfaces
    public event EventHandler<ChoiceRequestEventArgs>? ChoiceRequested;
    public event EventHandler<Choice>? ChoiceSubmitted;

    // Timeout methods from IPlayer
    public Task NotifyTimeoutWarningAsync(TimeSpan remainingTime)
    {
        // Random player doesn't need timeout warnings since it's automated
        return Task.CompletedTask;
    }

    public Task NotifyChoiceTimeoutAsync()
    {
        // Random player doesn't need timeout notifications since it's automated
        return Task.CompletedTask;
    }

    private Choice GetNextChoiceFromAll(IChoiceRequest request)
    {
        if (PrintDebug)
        {
            Console.WriteLine($"[PlayerRandom] GetNextChoiceFromAll called for {SideId}");
        }

        // Validate request is not null
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request), "Choice request cannot be null");
        }

        // Dispatch to appropriate handler based on request type
        return request switch
        {
            TeamPreviewRequest tpr => GetRandomTeamPreviewChoice(tpr),
            MoveRequest mr => GetRandomMoveChoice(mr),
            SwitchRequest sr => GetRandomSwitchChoice(sr),
            _ => throw new NotImplementedException(
                $"Request type {request.GetType().Name} not implemented")
        };
    }

    private Choice GetRandomTeamPreviewChoice(TeamPreviewRequest request)
    {
        if (PrintDebug)
        {
            Console.WriteLine($"[PlayerRandom] Generating random team preview choice for {SideId}");
        }

        var pokemon = request.Side.Pokemon;

        // Generate a random order for the team
        var order = Enumerable.Range(0, pokemon.Count).ToList();

        // Shuffle using Fisher-Yates algorithm
        for (int i = order.Count - 1; i > 0; i--)
        {
            int j = _random.Random(0, i + 1);
            (order[i], order[j]) = (order[j], order[i]);
        }

        if (PrintDebug)
        {
            Console.WriteLine(
                $"[PlayerRandom] Selected order: {string.Join(",", order.Select(i => i + 1))}");
        }

        // Build actions based on the randomly selected order
        var actions = order.Select((originalPokemonIndex, newPosition) => new ChosenAction
        {
            Choice = ChoiceType.Team,
            Pokemon = null,
            MoveId = MoveId.None,
            Index = newPosition,
            TargetLoc = originalPokemonIndex,
            Priority = -newPosition,
        }).ToList();

        return new Choice
        {
            Actions = actions,
        };
    }

    private Choice GetRandomMoveChoice(MoveRequest request)
    {
        if (PrintDebug)
        {
            Console.WriteLine($"[PlayerRandom] Generating random move choice for {SideId}");
        }

        if (request == null)
        {
            throw new ArgumentNullException(nameof(request), "MoveRequest cannot be null");
        }

        if (request.Active == null)
        {
            throw new InvalidOperationException("MoveRequest.Active cannot be null");
        }

        if (request.Active.Count == 0)
        {
            throw new InvalidOperationException("No active Pokemon to make a move choice");
        }

        if (request.Side == null)
        {
            throw new InvalidOperationException("MoveRequest.Side cannot be null");
        }


        var actions = new List<ChosenAction>();

        // Handle each active Pokemon
        for (int pokemonIndex = 0; pokemonIndex < request.Active.Count; pokemonIndex++)
        {
            PokemonMoveRequestData? pokemonRequest = request.Active[pokemonIndex];

            // Null entries represent fainted Pokemon or empty slots - add a pass action
            if (pokemonRequest == null)
            {
                actions.Add(new ChosenAction
                {
                    Choice = ChoiceType.Pass,
                    Pokemon = null,
                    MoveId = MoveId.None,
                });
                continue;
            }

            // Build list of all available choices (moves with and without tera, plus switch option)
            var availableChoices = new List<(bool isMove, int moveIndex, bool useTera)>();

            // Check if terastallization is available
            MoveType? teraType = pokemonRequest.CanTerastallize switch
            {
                MoveTypeMoveTypeFalseUnion mtfu => mtfu.MoveType,
                _ => null,
            };

            // Add moves to available choices
            for (int i = 0; i < pokemonRequest.Moves.Count; i++)
            {
                PokemonMoveData move = pokemonRequest.Moves[i];
                bool disabled = IsDisabled(move.Disabled);

                if (!disabled)
                {
                    // Add regular move option
                    availableChoices.Add((true, i, false));

                    // Add tera variant if available
                    if (teraType.HasValue)
                    {
                        availableChoices.Add((true, i, true));
                    }
                }
            }

            // Add switch option only if:
            // 1. The Pokemon is not trapped
            // 2. There are bench Pokemon available to switch in
            bool isTrapped = pokemonRequest.Trapped == true;
            var availableSwitches = request.Side.Pokemon
                .Select((p, index) => new { PokemonData = p, Index = index })
                .Where(x => !x.PokemonData.Active && !IsPokemonFainted(x.PokemonData))
                .ToList();

            bool canSwitch = !isTrapped && availableSwitches.Count > 0;
            if (canSwitch)
            {
                availableChoices.Add((false, -1, false)); // Switch option
            }

            // If no choices available (all moves disabled and can't switch), 
            // use the first move anyway - the battle engine will force Struggle
            if (availableChoices.Count == 0)
            {
                if (pokemonRequest.Moves.Count > 0)
                {
                    // Use the first move (likely Struggle or will be converted to Struggle)
                    availableChoices.Add((true, 0, false));
                }
                else
                {
                    // No moves at all - this shouldn't happen, but add a pass just in case
                    actions.Add(new ChosenAction
                    {
                        Choice = ChoiceType.Pass,
                        Pokemon = null,
                        MoveId = MoveId.None,
                    });
                    continue;
                }
            }

            int randomIndex = _random.Random(0, availableChoices.Count);
            (bool isMove, int moveIndex, bool useTera) = availableChoices[randomIndex];

            if (isMove)
            {
                // Selected a move
                PokemonMoveData selectedMove = pokemonRequest.Moves[moveIndex];

                if (PrintDebug)
                {
                    string teraStr = useTera ? $" with Tera ({teraType})" : "";
                    Console.WriteLine(
                        $"[PlayerRandom] Pokemon {pokemonIndex + 1} selected move: {selectedMove.Move.Name}{teraStr}");
                }

                // Random target location (0 for auto-targeting, will be resolved by battle engine)
                int targetLoc = GetRandomTargetLocation(selectedMove.Move.Target);

                actions.Add(new ChosenAction
                {
                    Choice = ChoiceType.Move,
                    Pokemon = null,
                    MoveId = selectedMove.Id,
                    TargetLoc = targetLoc,
                    Terastallize = useTera ? teraType : null
                });
            }
            else
            {
                // Selected switch
                if (PrintDebug)
                {
                    Console.WriteLine($"[PlayerRandom] Pokemon {pokemonIndex + 1} selected switch option");
                }

                // Pick a random Pokemon to switch in
                int randomSwitchIndex = _random.Random(0, availableSwitches.Count);
                var selectedSwitch = availableSwitches[randomSwitchIndex];

                actions.Add(new ChosenAction
                {
                    Choice = ChoiceType.Switch,
                    Pokemon = null,
                    MoveId = MoveId.None,
                    Index = selectedSwitch.Index,
                });
            }
        }

        return new Choice
        {
            Actions = actions
        };
    }

    /// <summary>
    /// Gets a random target location for a move.
    /// Returns 0 for most moves (auto-targeting), or a specific slot for targeting moves in doubles.
    /// </summary>
    private int GetRandomTargetLocation(MoveTarget targetType)
    {
        // In singles, always use auto-targeting
        // In doubles, moves that require explicit targeting (Normal, Any, AdjacentFoe, etc.)
        // need a non-zero target location
        
        // Check if this move type requires explicit targeting
        var requiresExplicitTarget = targetType is MoveTarget.Normal 
            or MoveTarget.Any 
            or MoveTarget.AdjacentAlly 
            or MoveTarget.AdjacentAllyOrSelf 
            or MoveTarget.AdjacentFoe;

        if (!requiresExplicitTarget)
        {
            // Moves like AllAdjacent, AllAdjacentFoes, etc. use auto-targeting (0)
            return 0;
        }

        // For moves requiring explicit targets, pick a random opponent slot (1 or 2)
        // In doubles: 1 = left opponent, 2 = right opponent
        // Random player will pick between the two opponent slots
        return _random.Random(1, 3); // Returns 1 or 2
    }

    private bool IsDisabled(MoveIdBoolUnion disabled)
    {
        return disabled switch
        {
            BoolMoveIdBoolUnion boolUnion => boolUnion.Value,
            _ => false
        };
    }

    private bool IsPokemonFainted(PokemonSwitchRequestData pokemon)
    {
        // Check if the Pokemon is fainted (Condition == Fainted)
        // or being revived by Revival Blessing (Reviving flag)
        return pokemon.Condition == ConditionId.Fainted || pokemon.Reviving;
    }

    private Choice GetRandomSwitchChoice(SwitchRequest request)
    {
        if (PrintDebug)
        {
            Console.WriteLine($"[PlayerRandom] Generating random switch choice for {SideId}");
        }

        // Build list of available Pokemon (excluding active and fainted)
        var availablePokemonWithIndex = request.Side.Pokemon
            .Select((p, index) => new { PokemonData = p, OriginalIndex = index })
            .Where(x => !x.PokemonData.Active && !IsPokemonFainted(x.PokemonData))
            .ToList();

        if (availablePokemonWithIndex.Count == 0)
        {
            throw new InvalidOperationException("No Pokemon available to switch");
        }

        // Pick a random Pokemon
        int randomIndex = _random.Random(0, availablePokemonWithIndex.Count);
        var selectedItem = availablePokemonWithIndex[randomIndex];

        if (PrintDebug)
        {
            Console.WriteLine(
                $"[PlayerRandom] Selected switch to: {selectedItem.PokemonData.Details}");
        }

        return new Choice
        {
            Actions = new List<ChosenAction>
            {
                new()
                {
                    Choice = ChoiceType.Switch,
                    Pokemon = null,
                    MoveId = MoveId.None,
                    Index = selectedItem.OriginalIndex,
                },
            },
        };
    }
}