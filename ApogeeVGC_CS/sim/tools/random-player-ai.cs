//using ApogeeVGC_CS.data;
//using ApogeeVGC_CS.lib;
//using ApogeeVGC_CS.sim;
//using System;
//using System.Diagnostics;
//using static System.Net.Mime.MediaTypeNames;
//using static System.Runtime.InteropServices.JavaScript.JSType;

//namespace ApogeeVGC_CS.sim.tools
//{
//    public class RandomPlayerAiOptions
//    {
//        public int? Move { get; init; }
//        public int? Mega { get; init; }
//        public Prng? Seed { get; init; }
//    }

//    public class MoveOption
//    {
//        public int Slot { get; set; }
//        public required string Move { get; set; }
//        public required string Target { get; set; }
//        public bool ZMove { get; set; }
//    }

//    public class ChooseMoveOption
//    {
//        public required string Choice { get; set; }
//        public required MoveOption Move { get; set; }
//    }

//    public class SwitchOption
//    {
//        public int Slot { get; set; }
//        public required PokemonSwitchRequestData Pokemon { get; set; }
//    }

//    // Interface to represent move data with common properties
//    public interface IMoveData
//    {
//        string Move { get; }
//        string? Target { get; }
//        bool? Disabled { get; }
//    }

//    public class RandomPlayerAi : BattlePlayer
//    {
//        public required int Move { get; init; }
//        public required int Mega { get; init; }
//        public required Prng Prng { get; init; }

//        public RandomPlayerAi(ObjectReadWriteStream<string> playerStream,
//            RandomPlayerAiOptions options, bool debug = false) : 
//            base(playerStream, debug)
//        {
//            Move = options.Move ?? 1;
//            Mega = options.Mega ?? 0;
//            Prng = Prng.Get(options.Seed);
//        }

//        public override void ReceiveError(Exception error)
//        {
//            // If we made an unavailable choice we will receive a followup request to
//            // allow us the opportunity to correct our decision.
//            if (error.Message.StartsWith("[Unavailable choice]")) return;
//            throw error;
//        }

//        public override void ReceiveRequest(IChoiceRequest request)
//        {
//            // Handle different request types
//            if (request is WaitRequest)
//            {
//                // wait request - do nothing
//                return;
//            }
//            else if (request is SwitchRequest switchRequest)
//            {
//                // switch request
//                var pokemon = switchRequest.Side.Pokemon;
//                var chosen = new List<int>();
//                var choices = new List<string>();

//                for (int i = 0; i < switchRequest.ForceSwitch.Count; i++)
//                {
//                    if (!switchRequest.ForceSwitch[i])
//                    {
//                        choices.Add("pass");
//                        continue;
//                    }

//                    var canSwitch = Range(1, 6).Where(j =>
//                        pokemon[j - 1] != null &&
//                        // not active
//                        j > switchRequest.ForceSwitch.Count &&
//                        // not chosen for a simultaneous switch
//                        !chosen.Contains(j) &&
//                        // not fainted or fainted and using Revival Blessing
//                        (!pokemon[j - 1].Condition.EndsWith(" fnt")) == (!pokemon[i].Reviving)
//                    ).ToList();

//                    if (canSwitch.Count == 0)
//                    {
//                        choices.Add("pass");
//                        continue;
//                    }

//                    var target = ChooseSwitch(
//                        null,
//                        canSwitch.Select(slot => new SwitchOption 
//                        { 
//                            Slot = slot, 
//                            Pokemon = pokemon[slot - 1] 
//                        }).ToList()
//                    );

//                    chosen.Add(target);
//                    choices.Add($"switch {target}");
//                }

//                Choose(string.Join(", ", choices));
//            }
//            else if (request is TeamPreviewRequest teamPreviewRequest)
//            {
//                Choose(ChooseTeamPreview(teamPreviewRequest.Side.Pokemon));
//            }
//            else if (request is MoveRequest moveRequest)
//            {
//                // move request
//                bool canMegaEvo = true, canUltraBurst = true, canZMove = true, 
//                     canDynamax = true, canTerastallize = true;
//                var pokemon = moveRequest.Side.Pokemon;
//                var chosen = new List<int>();
//                var choices = new List<string>();

//                for (int i = 0; i < moveRequest.Active.Count; i++)
//                {
//                    var active = moveRequest.Active[i];
                    
//                    if (pokemon[i].Condition.EndsWith(" fnt") || (pokemon[i].Commanding ?? false))
//                    {
//                        choices.Add("pass");
//                        continue;
//                    }

//                    canMegaEvo = canMegaEvo && (active.CanMegaEvo ?? false);
//                    canUltraBurst = canUltraBurst && (active.CanUltraBurst ?? false);
//                    canZMove = canZMove && (active.CanZMove != null);
//                    canDynamax = canDynamax && (active.CanDynamax != null);
//                    canTerastallize = canTerastallize && (active.CanTerastallize != null);

//                    // Determine whether we should change form if we do end up switching
//                    bool change = (canMegaEvo || canUltraBurst || canDynamax) && Prng.Random() < Mega;
                    
//                    // If we've already dynamaxed or if we're planning on potentially dynamaxing
//                    // we need to use the maxMoves instead of our regular moves
//                    bool useMaxMoves = (!(active.CanDynamax ?? false) && active.MaxMoves != null) || 
//                                       (change && canDynamax);

//                    var canMove = new List<MoveOption>();

//                    // Handle moves based on whether we're using max moves or regular moves
//                    if (useMaxMoves && active.MaxMoves?.MaxMoves != null)
//                    {
//                        var maxMoves = active.MaxMoves.MaxMoves;
//                        for (int j = 1; j <= maxMoves.Count; j++)
//                        {
//                            if (!(maxMoves[j - 1].Disabled ?? false))
//                            {
//                                canMove.Add(new MoveOption
//                                {
//                                    Slot = j,
//                                    Move = maxMoves[j - 1].Move,
//                                    Target = maxMoves[j - 1].Target ?? "",
//                                    ZMove = false
//                                });
//                            }
//                        }
//                    }
//                    else if (active.Moves != null)
//                    {
//                        var regularMoves = active.Moves;
//                        for (int j = 1; j <= regularMoves.Count; j++)
//                        {
//                            // not disabled
//                            if (!(regularMoves[j - 1].Disabled ?? false))
//                            {
//                                // NOTE: we don't actually check for whether we have PP or not because the
//                                // simulator will mark the move as disabled if there is zero PP and there are
//                                // situations where we actually need to use a move with 0 PP (Gen 1 Wrap).
//                                canMove.Add(new MoveOption
//                                {
//                                    Slot = j,
//                                    Move = regularMoves[j - 1].Move,
//                                    Target = regularMoves[j - 1].Target ?? "",
//                                    ZMove = false
//                                });
//                            }
//                        }
//                    }

//                    if (canZMove && active.CanZMove != null)
//                    {
//                        // Handle Z-move logic - simplified since we don't have the exact structure
//                        // In the original JS, this would iterate over active.canZMove array
//                        // For now, we'll handle it as a generic object and try to extract move data
//                        try
//                        {
//                            if (active.CanZMove is AnyObject zMoveObj)
//                            {
//                                // Try to interpret as array-like structure
//                                for (int j = 0; j < 10; j++) // Arbitrary limit to prevent infinite loop
//                                {
//                                    if (zMoveObj.TryGetValue(j.ToString(), out var moveValue) && moveValue != null)
//                                    {
//                                        if (moveValue is AnyObject moveData)
//                                        {
//                                            canMove.Add(new MoveOption
//                                            {
//                                                Slot = j + 1,
//                                                Move = moveData.GetValueOrDefault("move")?.ToString() ?? "",
//                                                Target = moveData.GetValueOrDefault("target")?.ToString() ?? "",
//                                                ZMove = true
//                                            });
//                                        }
//                                    }
//                                    else
//                                    {
//                                        break; // No more moves
//                                    }
//                                }
//                            }
//                        }
//                        catch
//                        {
//                            // If Z-move parsing fails, just continue without Z-moves
//                        }
//                    }

//                    // Filter out adjacentAlly moves if we have no allies left, unless they're our
//                    // only possible move options.
//                    bool hasAlly = pokemon.Count > 1 && !pokemon[i ^ 1].Condition.EndsWith(" fnt");
//                    var filtered = canMove.Where(m => m.Target != "adjacentAlly" || hasAlly).ToList();
//                    canMove = filtered.Count > 0 ? filtered : canMove;

//                    var moves = canMove.Select(m =>
//                    {
//                        var move = $"move {m.Slot}";
//                        // NOTE: We don't generate all possible targeting combinations.
//                        if (moveRequest.Active.Count > 1)
//                        {
//                            if (new[] { "normal", "any", "adjacentFoe" }.Contains(m.Target))
//                            {
//                                move += $" {1 + Prng.Random(2)}";
//                            }
//                            if (m.Target == "adjacentAlly")
//                            {
//                                move += $" -{(i ^ 1) + 1}";
//                            }
//                            if (m.Target == "adjacentAllyOrSelf")
//                            {
//                                if (hasAlly)
//                                {
//                                    move += $" -{1 + Prng.Random(2)}";
//                                }
//                                else
//                                {
//                                    move += $" -{i + 1}";
//                                }
//                            }
//                        }
//                        if (m.ZMove) move += " zmove";
//                        return new ChooseMoveOption { Choice = move, Move = m };
//                    }).ToList();

//                    var canSwitch = Range(1, 6).Where(j =>
//                        pokemon[j - 1] != null &&
//                        // not active
//                        !pokemon[j - 1].Active &&
//                        // not chosen for a simultaneous switch
//                        !chosen.Contains(j) &&
//                        // not fainted
//                        !pokemon[j - 1].Condition.EndsWith(" fnt")
//                    ).ToList();

//                    var switches = (active.Trapped ?? false) ? new List<int>() : canSwitch;

//                    if (switches.Count > 0 && (moves.Count == 0 || Prng.Random() > Move))
//                    {
//                        var target = ChooseSwitch(
//                            new AnyObject(active),
//                            canSwitch.Select(slot => new SwitchOption 
//                            { 
//                                Slot = slot, 
//                                Pokemon = pokemon[slot - 1] 
//                            }).ToList()
//                        );
//                        chosen.Add(target);
//                        choices.Add($"switch {target}");
//                    }
//                    else if (moves.Count > 0)
//                    {
//                        var move = ChooseMove(new AnyObject(active), moves);
//                        if (move.EndsWith(" zmove"))
//                        {
//                            canZMove = false;
//                            choices.Add(move);
//                        }
//                        else if (change)
//                        {
//                            if (canTerastallize)
//                            {
//                                canTerastallize = false;
//                                choices.Add($"{move} terastallize");
//                            }
//                            else if (canDynamax)
//                            {
//                                canDynamax = false;
//                                choices.Add($"{move} dynamax");
//                            }
//                            else if (canMegaEvo)
//                            {
//                                canMegaEvo = false;
//                                choices.Add($"{move} mega");
//                            }
//                            else
//                            {
//                                canUltraBurst = false;
//                                choices.Add($"{move} ultra");
//                            }
//                        }
//                        else
//                        {
//                            choices.Add(move);
//                        }
//                    }
//                    else
//                    {
//                        throw new Exception($"{GetType().Name} unable to make choice {i}. " +
//                            $"request = '{request.GetType()}', chosen = '{string.Join(",", chosen)}', " +
//                            $"(mega = {canMegaEvo}, ultra = {canUltraBurst}, zmove = {canZMove}, " +
//                            $"dynamax = '{canDynamax}', terastallize = {canTerastallize})");
//                    }
//                }

//                Choose(string.Join(", ", choices));
//            }
//        }

//        // Helper method to create range (equivalent to JavaScript range function)
//        private static List<int> Range(int start, int end)
//        {
//            return Enumerable.Range(start, end - start + 1).ToList();
//        }

//        // Virtual methods that can be overridden for custom AI behavior
//        protected virtual int ChooseSwitch(AnyObject? active, List<SwitchOption> switches)
//        {
//            return Prng.Sample(switches).Slot;
//        }

//        protected virtual string ChooseMove(AnyObject active, List<ChooseMoveOption> moves)
//        {
//            return Prng.Sample(moves).Choice;
//        }

//        protected virtual string ChooseTeamPreview(List<PokemonSwitchRequestData> team)
//        {
//            return "default";
//        }
//    }
//}
