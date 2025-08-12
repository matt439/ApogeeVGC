using ApogeeVGC_CS.sim;

namespace ApogeeVGC_CS
{
    public class RandomPlayerAiOptions
    {
        public int? Move { get; init; }
        public int? Mega { get; init; }
        public PrngPrngSeedUnion? Seed { get; init; }
    }

    public class MoveOption
    {
        public int Slot { get; set; }
        public required string Move { get; set; }
        public required string Target { get; set; }
        public bool ZMove { get; set; }
    }

    public class ChooseMoveOption
    {
        public required string Choice { get; set; }
        public required MoveOption Move { get; set; }
    }

    public class SwitchOption
    {
        public int Slot { get; set; }
        public PokemonSwitchRequestData Pokemon { get; set; } = null!;
    }

    public class ZMoveCollectionData
    {
        public required string Move { get; set; }
        public required string Target { get; set; }
        public Dictionary<string, object> AdditionalData { get; set; } = new();
    }

    public class ZMoveCollection
    {
        private readonly List<ZMoveCollectionData> _moves = [];

        public ZMoveCollectionData this[int index] => _moves[index];
        public int Count => _moves.Count;

        public void Add(ZMoveCollectionData move) => _moves.Add(move);

        // Factory method to create from ExtraData
        public static ZMoveCollection FromExtraData(Dictionary<string, object> extraData)
        {
            var collection = new ZMoveCollection();

            // Parse the ExtraData structure and populate the collection
            // This depends on how the data is actually stored
            if (!extraData.TryGetValue("moves", out object? movesObj) ||
                movesObj is not List<Dictionary<string, object>> movesList) return collection;
            foreach (var moveDict in movesList)
            {
                collection.Add(new ZMoveCollectionData
                {
                    Move = moveDict.GetValueOrDefault("move")?.ToString() ?? "",
                    Target = moveDict.GetValueOrDefault("target")?.ToString() ?? "",
                    AdditionalData = moveDict
                });
            }

            return collection;
        }
    }


    public class RandomPlayerAi(Stream playerStream, RandomPlayerAiOptions options, bool debug = false)
        : BattlePlayer(playerStream, debug)
    {
        protected int Move { get; init; } = options.Move ?? 1;
        protected int Mega { get; init; } = options.Mega ?? 0;
        protected Prng Prng { get; init; } = Prng.Get(options.Seed);

        public override void ReceiveError(Exception error)
        {
            // If we made an unavailable choice we will receive a followup request to
            // allow us the opportunity to correct our decision.
            if (error.Message.StartsWith("[Unavailable choice]")) return;
            throw error;
        }

        public override void ReceiveRequest(IChoiceRequest request)
        {
            if (request is WaitRequest)
            {
                // wait request - do nothing
                return;
            }
            else if (request is SwitchRequest switchRequest)
            {
                // handle switch request
                var pokemon = switchRequest.Side.Pokemon;
                var chosen = new List<int>();
                var choices = new List<string>();

                for (int i = 0; i < switchRequest.ForceSwitch.Count; i++)
                {
                    if (!switchRequest.ForceSwitch[i])
                    {
                        choices.Add("pass");
                        continue;
                    }

                    // Find valid switch options - Pokemon that are:
                    // - not null
                    // - not active (position > forceSwitch.Count)
                    // - not already chosen for another position
                    // - not fainted (or fainted only when using Revival Blessing)
                    var canSwitch = RandomPlayerAiUtils.Range(1, 6).Where(j =>
                        pokemon[j - 1] != null &&
                        j > switchRequest.ForceSwitch.Count &&
                        !chosen.Contains(j) &&
                        !pokemon[j - 1].Condition.EndsWith(" fnt") == !pokemon[i].Reviving
                    ).ToList();

                    if (canSwitch.Count == 0)
                    {
                        choices.Add("pass");
                        continue;
                    }

                    var target = ChooseSwitch(
                        canSwitch.Select(slot => new SwitchOption
                        {
                            Slot = slot,
                            Pokemon = pokemon[slot - 1]
                        }).ToList()
                    );

                    chosen.Add(target);
                    choices.Add($"switch {target}");
                }

                Choose(string.Join(", ", choices));
            }
            else if (request is TeamPreviewRequest teamPreviewRequest)
            {
                Choose(ChooseTeamPreview(teamPreviewRequest.Side.Pokemon));
            }
            else if (request is MoveRequest moveRequest)
            {
                // move request
                bool canMegaEvo = true, canUltraBurst = true, canZMove = true,
                     canDynamax = true, canTerastallize = true;
                var pokemon = moveRequest.Side.Pokemon;
                var chosen = new List<int>();
                var choices = new List<string>();

                for (int i = 0; i < moveRequest.Active.Count; i++)
                {
                    var active = moveRequest.Active[i];

                    // Skip fainted or commanding Pokemon
                    if (pokemon[i].Condition.EndsWith(" fnt") || (pokemon[i].Commanding ?? false))
                    {
                        choices.Add("pass");
                        continue;
                    }

                    // Update available special move options
                    canMegaEvo = canMegaEvo && (active.CanMegaEvo ?? false);
                    canUltraBurst = canUltraBurst && (active.CanUltraBurst ?? false);
                    canZMove = canZMove && active.CanZMove != null;
                    canDynamax = canDynamax && active.CanDynamax != null;
                    canTerastallize = canTerastallize && active.CanTerastallize != null;

                    // Determine if we should use special form-changing moves
                    bool change = (canMegaEvo || canUltraBurst || canDynamax) && Prng.Random() < Mega;

                    // Decide if we should use max moves
                    bool useMaxMoves = (!(active.CanDynamax ?? false) && active.MaxMoves != null) ||
                                       (change && canDynamax);

                    var canMove = new List<MoveOption>();
                    if (useMaxMoves && active.MaxMoves?.MaxMoves is List<DynamaxMoveData> maxMoves)
                    {
                        for (int j = 1; j <= maxMoves.Count; j++)
                        {
                            if (!maxMoves[j - 1].Disabled)
                            {
                                canMove.Add(new MoveOption
                                {
                                    Slot = j,
                                    Move = maxMoves[j - 1].Move,
                                    Target = maxMoves[j - 1].Target,
                                    ZMove = false
                                });
                            }
                        }
                    }
                    else if (active.Moves is List<PokemonMoveData> movesList)
                    {
                        for (int j = 1; j <= movesList.Count; j++)
                        {
                            if (!movesList[j - 1].Disabled)
                            {
                                canMove.Add(new MoveOption
                                {
                                    Slot = j,
                                    Move = movesList[j - 1].Move,
                                    Target = movesList[j - 1].Target,
                                    ZMove = false
                                });
                            }
                        }
                    }

                    // Add Z-move options if available
                    if (canZMove && active.CanZMove is not null)
                    {
                        Dictionary<string, object>? zMoveDict = null;
                        if (active.CanZMove is Dictionary<string, object> dict)
                        {
                            zMoveDict = dict;
                        }
                        else if (active.CanZMove is AnyObject anyObj)
                        {
                            zMoveDict = anyObj as Dictionary<string, object>;
                            if (zMoveDict == null && anyObj is not null)
                            {
                                zMoveDict = new Dictionary<string, object>(anyObj);
                            }
                        }
                        if (zMoveDict != null)
                        {
                            var zMoves = ZMoveCollection.FromExtraData(zMoveDict);
                            for (int j = 1; j <= zMoves.Count; j++)
                            {
                                if (zMoves[j - 1] != null)
                                {
                                    canMove.Add(new MoveOption
                                    {
                                        Slot = j,
                                        Move = zMoves[j - 1].Move,
                                        Target = zMoves[j - 1].Target,
                                        ZMove = true
                                    });
                                }
                            }
                        }
                    }

                    // Handle ally targeting logic in multi-battles
                    bool hasAlly = pokemon.Count > 1 && !pokemon[i ^ 1].Condition.EndsWith(" fnt");
                    var filtered = canMove.Where(m => m.Target != "adjacentAlly" || hasAlly).ToList();
                    canMove = filtered.Count > 0 ? filtered : canMove;

                    // Build move choices with proper targeting
                    var moves = new List<ChooseMoveOption>();
                    foreach (var m in canMove)
                    {
                        string moveChoice = $"move {m.Slot}";

                        if (moveRequest.Active.Count > 1)
                        {
                            // Add targeting for multi-battles
                            if (new[] { "normal", "any", "adjacentFoe" }.Contains(m.Target))
                            {
                                moveChoice += $" {1 + Prng.Random(2)}";
                            }
                            if (m.Target == "adjacentAlly")
                            {
                                moveChoice += $" -{(i ^ 1) + 1}";
                            }
                            if (m.Target == "adjacentAllyOrSelf")
                            {
                                if (hasAlly)
                                {
                                    moveChoice += $" -{1 + Prng.Random(2)}";
                                }
                                else
                                {
                                    moveChoice += $" -{i + 1}";
                                }
                            }
                        }

                        if (m.ZMove) moveChoice += " zmove";

                        moves.Add(new ChooseMoveOption
                        {
                            Choice = moveChoice,
                            Move = m
                        });
                    }

                    // Find valid switch targets
                    var canSwitch = RandomPlayerAiUtils.Range(1, 6).Where(j =>
                        pokemon[j - 1] != null &&
                        !pokemon[j - 1].Active &&
                        !chosen.Contains(j) &&
                        !pokemon[j - 1].Condition.EndsWith(" fnt")
                    ).ToList();

                    // Decide whether to switch or use move
                    var switches = (active.Trapped ?? false) ? [] : canSwitch;

                    if (switches.Count > 0 && (moves.Count == 0 || Prng.Random() > Move))
                    {
                        // Choose to switch
                        var switchOptions = canSwitch.Select(slot => new SwitchOption
                        {
                            Slot = slot,
                            Pokemon = pokemon[slot - 1]
                        }).ToList();

                        var target = ChooseSwitch(switchOptions);
                        chosen.Add(target);
                        choices.Add($"switch {target}");
                    }
                    else if (moves.Count > 0)
                    {
                        // Choose to use a move
                        var move = ChooseMove(moves);

                        // Handle special move mechanics
                        if (move.EndsWith(" zmove"))
                        {
                            canZMove = false;
                            choices.Add(move);
                        }
                        else if (change)
                        {
                            if (canTerastallize)
                            {
                                canTerastallize = false;
                                choices.Add($"{move} terastallize");
                            }
                            else if (canDynamax)
                            {
                                canDynamax = false;
                                choices.Add($"{move} dynamax");
                            }
                            else if (canMegaEvo)
                            {
                                canMegaEvo = false;
                                choices.Add($"{move} mega");
                            }
                            else
                            {
                                canUltraBurst = false;
                                choices.Add($"{move} ultra");
                            }
                        }
                        else
                        {
                            choices.Add(move);
                        }
                    }
                    else
                    {
                        // No valid moves or switches
                        throw new Exception(
                            $"{GetType().Name} unable to make choice {i}. " +
                            $"request = '{request.GetType()}', chosen = '{string.Join(",", chosen)}', " +
                            $"(mega = {canMegaEvo}, ultra = {canUltraBurst}, zmove = {canZMove}, " +
                            $"dynamax = '{canDynamax}', terastallize = {canTerastallize})"
                        );
                    }
                }

                Choose(string.Join(", ", choices));
            }
        }

        // You'll also need to add the Choose method to the base class or implement it:
        protected new virtual void Choose(string choice)
        {
            // Send the choice to the battle stream
            // Implementation depends on how BattlePlayer communicates with the battle
            // This might involve writing to the Stream property
            byte[] choiceBytes = System.Text.Encoding.UTF8.GetBytes(choice + "\n");
            Stream.Write(choiceBytes, 0, choiceBytes.Length);
            Stream.Flush();
        }

        protected virtual string ChooseTeamPreview(List<AnyObject> team)
        {
            return "default";
        }

        protected virtual string ChooseTeamPreview(List<PokemonSwitchRequestData> team)
        {
            return "default";
        }

        protected virtual string ChooseMove(List<ChooseMoveOption> moves)
        {
            return Prng.Sample(moves).Choice;
        }

        protected virtual int ChooseSwitch(List<SwitchOption> switches)
        {
            return Prng.Sample(switches).Slot;
        }
    }

    public static class RandomPlayerAiUtils
    {
        public static List<int> Range(int start, int? end = null, int step = 1)
        {
            if (end == null)
            {
                end = start;
                start = 0;
            }
            if (step <= 0)
            {
                throw new ArgumentException("Step must be greater than 0.");
            }
            var range = new List<int>();
            for (int i = start; i < end; i += step)
            {
                range.Add(i);
            }

            return range;
        }
    }
}
