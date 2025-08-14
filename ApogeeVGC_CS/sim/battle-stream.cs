using ApogeeVGC_CS.lib;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using static ApogeeVGC_CS.sim.BattleStreamUtils;

namespace ApogeeVGC_CS.sim
{
    // Helper struct to hold player streams
    public struct PlayerStreams
    {
        public ObjectReadWriteStream<string> Omniscient { get; init; }
        public ObjectReadStream<string> Spectator { get; init; }
        public ObjectReadWriteStream<string> Player1 { get; init; }
        public ObjectReadWriteStream<string> Player2 { get; init; }
        public ObjectReadWriteStream<string> Player3 { get; init; } // Optional, for 3-player battles
        public ObjectReadWriteStream<string> Player4 { get; init; } // Optional, for 4-player battles
    }

    public static class BattleStreamUtils
    {
        public static string[] SplitFirst(string str, string delimiter, int limit = 1)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(delimiter))
                return [str];

            var parts = new List<string>();
            string remaining = str;

            for (int i = 0; i < limit && !string.IsNullOrEmpty(remaining); i++)
            {
                int index = remaining.IndexOf(delimiter, StringComparison.Ordinal);
                if (index == -1)
                {
                    parts.Add(remaining);
                    remaining = string.Empty;
                }
                else
                {
                    parts.Add(remaining[..index]);
                    remaining = remaining[(index + delimiter.Length)..];
                }
            }

            if (!string.IsNullOrEmpty(remaining))
                parts.Add(remaining);

            // Ensure we always return exactly limit + 1 elements
            while (parts.Count < limit + 1)
                parts.Add(string.Empty);

            return parts.ToArray();
        }

        /// <summary>
        /// Splits a BattleStream into omniscient, spectator, p1, p2, p3 and p4
        /// streams, for ease of consumption.
        /// </summary>
        public static PlayerStreams GetPlayerStreams(BattleStream stream)
        {
            var streams = new PlayerStreams
            {
                Omniscient = new ObjectReadWriteStream<string>(new ObjectReadWriteStreamOptions<string>
                {
                    Write = async (writeStream, data) => stream.Write(data),
                    WriteEnd = () => Task.CompletedTask
                }),
                Spectator = new ObjectReadStream<string>([], new Queue<Exception>(),
                    Task.CompletedTask),
                Player1 = new ObjectReadWriteStream<string>(new ObjectReadWriteStreamOptions<string>
                {
                    Write = async (writeStream, data) => stream.Write(Regex.Replace(data,
                        @"(^|\n)", "$1>p1 "))
                }),
                Player2 = new ObjectReadWriteStream<string>(new ObjectReadWriteStreamOptions<string>
                {
                    Write = async (writeStream, data) => stream.Write(Regex.Replace(data,
                        @"(^|\n)", "$1>p2 "))
                }),
                Player3 = new ObjectReadWriteStream<string>(new ObjectReadWriteStreamOptions<string>
                {
                    Write = async (writeStream, data) => stream.Write(Regex.Replace(data,
                        @"(^|\n)", "$1>p3 "))
                }),
                Player4 = new ObjectReadWriteStream<string>(new ObjectReadWriteStreamOptions<string>
                {
                    Write = async (writeStream, data) => stream.Write(Regex.Replace(data,
                        @"(^|\n)", "$1>p4 "))
                })
            };

            // Start async processing task
            _ = Task.Run(async () =>
            {
                try
                {
                    await foreach (string chunk in stream.ReadAllChunksAsync())
                    {
                        string[] parts = SplitFirst(chunk, "\n");
                        string type = parts[0];
                        string data = parts.Length > 1 ? parts[1] : string.Empty;

                        switch (type)
                        {
                            case "update":
                                var channelMessages =
                                    BattleUtils.ExtractChannelMessages(data, [-1, 0, 1, 2, 3, 4]);

                                if (channelMessages.TryGetValue(-1, out var omniscientMsgs))
                                    streams.Omniscient.Push(string.Join('\n', omniscientMsgs));

                                if (channelMessages.TryGetValue(0, out var spectatorMsgs))
                                    streams.Spectator.Push(string.Join('\n', spectatorMsgs));

                                if (channelMessages.TryGetValue(1, out var p1Msgs))
                                    streams.Player1.Push(string.Join('\n', p1Msgs));

                                if (channelMessages.TryGetValue(2, out var p2Msgs))
                                    streams.Player2.Push(string.Join('\n', p2Msgs));

                                if (channelMessages.TryGetValue(3, out var p3Msgs))
                                    streams.Player3.Push(string.Join('\n', p3Msgs));

                                if (channelMessages.TryGetValue(4, out var p4Msgs))
                                    streams.Player4.Push(string.Join('\n', p4Msgs));
                                break;

                            case "sideupdate":
                                string[] sideParts = SplitFirst(data, "\n");
                                string side = sideParts[0];
                                string sideData = sideParts.Length > 1 ? sideParts[1] : string.Empty;

                                var targetStream = side switch
                                {
                                    "p1" => streams.Player1,
                                    "p2" => streams.Player2,
                                    "p3" => streams.Player3,
                                    "p4" => streams.Player4,
                                    _ => null
                                };
                                targetStream?.Push(sideData);
                                break;

                            case "end":
                                // ignore
                                break;
                        }
                    }

                    // Signal end to all streams
                    streams.Omniscient.PushEnd();
                    streams.Spectator.PushEnd();
                    streams.Player1.PushEnd();
                    streams.Player2.PushEnd();
                    streams.Player3.PushEnd();
                    streams.Player4.PushEnd();
                }
                catch (Exception err)
                {
                    // Signal error to all streams
                    streams.Omniscient.PushError(err, true);
                    streams.Spectator.PushError(err, true);
                    streams.Player1.PushError(err, true);
                    streams.Player2.PushError(err, true);
                    streams.Player3.PushError(err, true);
                    streams.Player4.PushError(err, true);
                }
            });

            return streams;
        }
    }

    // Extension method for BattleStream to support async enumeration
    public static class BattleStreamExtensions
    {
        public static async IAsyncEnumerable<string> ReadAllChunksAsync(this BattleStream stream)
        {
            // Read from the base ObjectReadWriteStream implementation
            await foreach (string chunk in ((ObjectReadWriteStream<string>)stream))
            {
                yield return chunk;
            }
        }

        public static async IAsyncEnumerable<string> ReadAllChunksAsync(this ObjectReadWriteStream<string> stream)
        {
            // Read from the stream using the async enumerable interface
            await foreach (string chunk in stream)
            {
                yield return chunk;
            }
        }

        public static async IAsyncEnumerable<string> ReadAllChunksAsync(this ObjectReadStream<string> stream)
        {
            // Read from the stream using the async enumerable interface
            await foreach (string chunk in stream)
            {
                yield return chunk;
            }
        }
    }

    public class BattleStreamOptions
    {
        public bool? Debug { get; init; }
        public bool? NoCatch { get; init; }
        public bool? KeepAlive { get; init; }
        public BattleStreamReplay? Replay { get; init; }
    }

    public class BattleStream(BattleStreamOptions? options = null) : ObjectReadWriteStream<string>
    {
        public bool Debug { get; init; } = options?.Debug ?? false;
        public bool NoCatch { get; init; } = options?.NoCatch ?? false;
        public BattleStreamReplay Replay { get; init; } =
            options?.Replay ?? new BoolBattleStreamReplay(false);
        public bool KeepAlive { get; init; } = options?.KeepAlive ?? false;
        public Battle? Battle { get; set; }

        /// <summary>
        /// Main write method that processes chunks with error handling and battle updates.
        /// Equivalent to _write(chunk: string) in TypeScript version.
        /// </summary>
        /// <param name="chunk">The chunk of data to process</param>
        public void Write(string chunk)
        {
            if (string.IsNullOrEmpty(chunk)) return;

            if (NoCatch)
            {
                WriteLines(chunk);
            }
            else
            {
                try
                {
                    WriteLines(chunk);
                }
                catch (Exception err)
                {
                    PushError(err, true);
                    return;
                }
            }
            Battle?.SendUpdates();
        }

        private void WriteLines(string chunk)
        {
            foreach (string line in chunk.Split('\n'))
            {
                if (!line.StartsWith('>')) continue;
                string[] parts = SplitFirst(line[1..], " ");
                string type = parts[0];
                string message = parts.Length > 1 ? parts[1] : string.Empty;
                WriteLine(type, message);
            }
        }

        /// <summary>
        /// Equivalent to this.pushMessage(t, data) in TypeScript version.
        /// Handles message routing and formatting for battle communication.
        /// Implements replay functionality to filter messages based on replay mode.
        /// </summary>
        private void PushMessage(string type, string data)
        {
            if (string.IsNullOrEmpty(data)) return;

            if (Replay is BoolBattleStreamReplay { Value: false }) return;
            if (type != "update") return;
            // Check if replay mode is 'spectator' or omniscient
            if (IsSpectatorReplay())
            {
                // Spectator mode: extract channel 0 messages
                var channelMessages = BattleUtils.ExtractChannelMessages(data, [0]);
                if (channelMessages.TryGetValue(0, out var spectatorMsgs))
                {
                    Push(string.Join('\n', spectatorMsgs));
                }
            }
            else
            {
                // Omniscient mode: extract channel -1 messages (full view)
                var channelMessages = BattleUtils.ExtractChannelMessages(data, [-1]);
                if (channelMessages.TryGetValue(-1, out var omniscientMsgs))
                {
                    Push(string.Join('\n', omniscientMsgs));
                }
            }
        }

        private void WriteLine(string type, string message)
        {
            switch (type)
            {
                case "start":
                    try
                    {
                        var options = JsonSerializer.Deserialize<Dictionary<string, object>>(message);

                        if (options != null)
                        {
                            void SendCallback(string t, StrListStrUnion data)
                            {
                                string processedData;

                                // Handle Array.isArray(data) equivalent in C#
                                if (data is IEnumerable<string> arrayData)
                                {
                                    // If data is an array, join with "\n" like data.join("\n")
                                    processedData = string.Join("\n", arrayData);
                                }
                                else
                                {
                                    // Otherwise convert to string
                                    processedData = data?.ToString() ?? string.Empty;
                                }

                                // Call this.pushMessage(t, data) equivalent
                                PushMessage(t, processedData);

                                // If t === 'end' && !this.keepAlive then this.pushEnd()
                                if (t == "end" && !KeepAlive)
                                {
                                    PushEnd();
                                }
                            }

                            // Determine debug flag: if (this.debug) options.debug = true
                            bool debugFlag = Debug;
                            if (options.TryGetValue("debug", out object? debugValue))
                            {
                                if (debugValue is bool debugBool)
                                {
                                    debugFlag = debugBool;
                                }
                                else if (bool.TryParse(debugValue.ToString(), out bool parsedDebug))
                                {
                                    debugFlag = parsedDebug;
                                }
                            }
                            if (Debug) debugFlag = true; // Override if stream debug is enabled
                            
                            // Create BattleOptions with all properties set in object initializer
                            BattleOptions battleOptions = CreateBattleOptionsFromDictionary(options,
                                SendCallback, debugFlag);
                            
                            // Create the battle: this.battle = new Battle(options)
                            Battle = new Battle(battleOptions, Dex.GetRequiredDex(DexConstants.BaseMod.ToString()));

                            Push("info: Battle created successfully");
                            Push($"start: {System.Text.Json.JsonSerializer.Serialize(options)}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Push($"error: Failed to create battle: {ex.Message}");
                    }
                    break;
                    
                case "player":
                    string[] parts = SplitFirst(message, " ");
                    string slot = parts[0];
                    string optionsText = parts.Length > 1 ? parts[1] : string.Empty;
                    
                    // Convert slot string to SideId enum
                    SideId sideId = slot switch
                    {
                        "p1" => SideId.P1, 
                        "p2" => SideId.P2, 
                        "p3" => SideId.P3, 
                        "p4" => SideId.P4,
                        _ => throw new ArgumentException($"Invalid slot: {slot}")
                    };
                    
                    try
                    {
                        var playerOptionsDict = JsonSerializer.Deserialize<Dictionary<string, object>>(optionsText);
                        if (playerOptionsDict != null)
                        {
                            // Convert Dictionary to PlayerOptions type
                            PlayerOptions playerOptions = ConvertToPlayerOptions(playerOptionsDict);
                            
                            Battle?.SetPlayer(sideId, playerOptions);
                            
                            Push($"info: Player {sideId} setup completed");
                            Push($"player: {sideId} {JsonSerializer.Serialize(playerOptionsDict)}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Push($"error: Failed to parse player options: {ex.Message}");
                    }
                    break;
                
                case "p1": case "p2": case "p3": case "p4":
                    SideId playerId = type switch
                    {
                        "p1" => SideId.P1, "p2" => SideId.P2, "p3" => SideId.P3, "p4" => SideId.P4,
                        _ => throw new ArgumentException($"Invalid type: {type}")
                    };
                    if (message == "undo")
                    {
                        Battle?.UndoChoice(playerId);
                        Push($"info: Undo choice for {playerId}");
                        Push($"undo: {playerId}");
                    }
                    else
                    {
                        Battle?.Choose(playerId, message);
                        Push($"info: Choice for {playerId}: ${message}");
                        Push($"choice: {playerId} {message}");
                    }
                    break;
                    
                case "forcewin": case "forcetie":
                    if (type == "forcewin")
                    {
                        SideId winSide = message switch
                        {
                            "p1" => SideId.P1, "p2" => SideId.P2, "p3" => SideId.P3, "p4" => SideId.P4,
                            _ => throw new ArgumentException($"Invalid side: {message}")
                        };
                        Battle?.ForceWin(winSide);
                        Push($"info: Force win for {winSide}");
                        Push($"win: {winSide}");
                        if (!string.IsNullOrEmpty(message))
                        {
                            Battle?.InputLog.Add($"> forcewin {message}");
                        }
                    }
                    else
                    {
                        Battle?.ForceWin(); // null means tie
                        Push("info: Force tie");
                        Push("tie");
                        Battle?.InputLog.Add("> forcetie");
                    }
                    break;
                    
                case "forcelose":
                    SideId loseSide = message switch
                    {
                        "p1" => SideId.P1, "p2" => SideId.P2, "p3" => SideId.P3, "p4" => SideId.P4,
                        _ => throw new ArgumentException($"Invalid side: {message}")
                    }; 
                    Battle?.ForceWin(GetOppositeSide(loseSide)); // Force the opposite side to win
                    Push($"info: Force lose for {loseSide}");
                    Push($"lose: {loseSide}");
                    Battle?.InputLog.Add($"> forcelose {message}");
                    break;
                    
                case "reseed":
                    try
                    {
                        string? seed = JsonSerializer.Deserialize<string>(message);
                        if (!string.IsNullOrEmpty(seed))
                        {
                            Battle?.Prng.SetSeed(seed);
                            Push($"info: RNG reseed requested: {seed}");
                            Push($"reseed: {seed}");
                            Battle?.InputLog.Add($"> reseed {seed}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Push($"error: Failed to parse seed: {ex.Message}");
                    }
                    break;
                    
                case "tiebreak":
                    Battle?.Tiebreak();
                    Push("info: Tiebreak requested");
                    Push("tiebreak");
                    break;
                    
                case "chat-inputlogonly":
                    Battle?.InputLog.Add($"> chat {message}");
                    break;
                    
                case "chat":
                    Battle?.InputLog.Add($"> chat {message}");
                    Battle?.Add("chat", message);
                    break;
                    
                case "eval":
                    Battle battle = Battle!;
                    battle.InputLog.Add($">{type} {message}");
                    message = message.Replace("\f ", "\n");
                    battle.Add("", ">>> " + message.Replace("\n ", "\n||"));
                    
                    try
                    {
                        Side? p1 = battle.Sides.Length > 0 ? battle.Sides[0] : null;
                        Side? p2 = battle.Sides.Length > 1 ? battle.Sides[1] : null;
                        Side? p3 = battle.Sides.Length > 2 ? battle.Sides[2] : null;
                        Side? p4 = battle.Sides.Length > 3 ? battle.Sides[3] : null;
                        Pokemon? p1active = p1?.Active?.FirstOrDefault();
                        Pokemon? p2active = p2?.Active?.FirstOrDefault();
                        Pokemon? p3active = p3?.Active?.FirstOrDefault();
                        Pokemon? p4active = p4?.Active?.FirstOrDefault();
                        
                        var toId = new Func<string, Id>(input => new Id(input));
                        
                        var player = new Func<string, Side?>(input =>
                        {
                            Id inputId = toId(input);
                            
                            if (Regex.IsMatch(inputId.ToString(), @"^p[1-9]$"))
                            {
                                if (int.TryParse(inputId.ToString()[1..], out int sideNum) && sideNum > 0 && sideNum <= battle.Sides.Length)
                                {
                                    return battle.Sides[sideNum - 1];
                                }
                            }
                            
                            if (Regex.IsMatch(inputId.ToString(), @"^[1-9]$"))
                            {
                                if (int.TryParse(inputId.ToString(), out int directNum) && directNum > 0 && directNum <= battle.Sides.Length)
                                {
                                    return battle.Sides[directNum - 1];
                                }
                            }

                            return battle.Sides.FirstOrDefault(side =>
                                toId(side.Name).ToString() == inputId.ToString());
                        });
                        
                        var pokemon = new Func<object, string, Pokemon?>((_side, input) =>
                        {
                            Side? side = _side switch
                            {
                                string sideStr => player(sideStr),
                                Side sideObj => sideObj,
                                _ => null
                            };
                            
                            if (side == null) return null;
                            
                            Id inputId = toId(input);
                            
                            if (!Regex.IsMatch(inputId.ToString(), @"^[1-9]$"))
                                return side.Pokemon.FirstOrDefault(p =>
                                    p.BaseSpecies.Id.ToString() == inputId.ToString() ||
                                    p.Species.Id.ToString() == inputId.ToString());
                            if (int.TryParse(inputId.ToString(), out int pokemonNum) && pokemonNum > 0 && pokemonNum <= side.Pokemon.Count)
                            {
                                return side.Pokemon[pokemonNum - 1];
                            }

                            return side.Pokemon.FirstOrDefault(p => 
                                p.BaseSpecies.Id.ToString() == inputId.ToString() || 
                                p.Species.Id.ToString() == inputId.ToString());
                        });

                        // C# cannot do JavaScript's eval() - we create a scripting-like environment
                        // For demonstration, we'll provide some common eval scenarios
                        string result = ProcessEvalMessage(message, battle, p1, p2, p3, p4, p1active, p2active, p3active, p4active, toId, player, pokemon);

                        result = VisualizeResult(result);
                        result = result.Replace("\n ", "\n||");
                        battle.Add("", "<<< " + result);
                    }
                    catch (Exception e)
                    {
                        battle.Add("", "<<< error: " + e.Message);
                    }
                    break;

                case "requestlog":
                    Push($"requesteddata\n{string.Join('\n', Battle!.InputLog)}");
                    break;
                    
                case "requestexport":
                    Push($"requesteddata\nSEED_PLACEHOLDER\n{string.Join('\n', Battle!.InputLog)}");
                    break;
                    
                case "requestteam":
                    if (!string.IsNullOrEmpty(message))
                    {
                        message = message.Trim();
                        
                        if (int.TryParse(message[1..], out int slotNum))
                        {
                            slotNum -= 1; // Convert to 0-based (matching - 1 from TypeScript)
                            
                            if (slotNum < 0 || slotNum >= Battle!.Sides.Length)
                            {
                                throw new Exception($"Team requested for slot {message}, but that slot does not exist.");
                            }
                            
                            Side side = Battle.Sides[slotNum];
                            string team = Teams.Pack(side.Team);
                            Push($"requesteddata\n{team}");
                        }
                        else
                        {
                            throw new Exception($"Team requested for slot {message}, but that slot does not exist.");
                        }
                    }
                    break;
                    
                case "show-openteamsheets":
                    Battle?.ShowOpenTeamSheets();
                    Push("info: Show open team sheets requested");
                    Push("openteamsheets");
                    break;
                    
                case "version": case "version-origin":
                    // These are ignored in the original implementation
                    break;
                    
                default:
                    throw new Exception($"Unrecognized command \"> {type} {message}\"");
            }
        }

        // Helper methods for the WriteLine implementation
        private static BattleOptions CreateBattleOptionsFromDictionary(Dictionary<string, object> options, 
            Action<string, StrListStrUnion> sendCallback, bool debugFlag)
        {
            string formatId = "gen9ou"; // Default format
            PrngSeed? prngSeed = null;
            BoolStringUnion rated = false;
            
            // Extract formatId
            if (options.TryGetValue("formatId", out object? formatValue))
            {
                formatId = formatValue.ToString() ?? "gen9ou";
            }
            else if (options.TryGetValue("formatid", out object? formatIdValue))
            {
                formatId = formatIdValue.ToString() ?? "gen9ou";
            }
            
            // Extract seed
            if (options.TryGetValue("seed", out object? seedValue))
            {
                if (seedValue is string seedStr)
                {
                    prngSeed = new PrngSeed(seedStr);
                }
                else if (seedValue is JsonElement { ValueKind: JsonValueKind.Array } jsonElement)
                {
                    // Convert JsonElement array to string representation
                    var seedArray = new List<int>();
                    foreach (JsonElement element in jsonElement.EnumerateArray())
                    {
                        if (element.TryGetInt32(out int intValue))
                        {
                            seedArray.Add(intValue);
                        }
                    }
                    prngSeed = new PrngSeed(string.Join(",", seedArray));
                }
                else
                {
                    prngSeed = new PrngSeed(seedValue.ToString() ?? "default");
                }
            }
            
            // Extract rated
            if (options.TryGetValue("rated", out object? ratedValue))
            {
                if (ratedValue is bool ratedBool)
                {
                    rated = ratedBool;
                }
                else if (ratedValue is string ratedStr)
                {
                    rated = ratedStr;
                }
                else if (bool.TryParse(ratedValue.ToString(), out bool parsedRated))
                {
                    rated = parsedRated;
                }
            }
            
            // Create BattleOptions with object initializer to handle init-only properties
            return new BattleOptions
            {
                FormatId = new Id(formatId),
                Debug = debugFlag,
                PrngSeed = prngSeed,
                Rated = rated,
                Send = sendCallback
            };
        }
        
        /// <summary>
        /// Determines if the current replay mode is spectator mode.
        /// This method checks the BattleStreamReplay configuration to see if it's set to spectator view.
        /// </summary>
        private bool IsSpectatorReplay()
        {
            return Replay switch
            {
                // Check if it's a SpectatorBattleStreamReplay (equivalent to this.replay === 'spectator')
                SpectatorBattleStreamReplay => true,
                
                // BoolBattleStreamReplay with false means no replay mode (not spectator)
                // BoolBattleStreamReplay with true means omniscient replay mode (not spectator)
                BoolBattleStreamReplay => false,
                
                // Default: not spectator mode
                _ => false
            };
        }

        private static SideId GetOppositeSide(SideId side)
        {
            return side switch
            {
                SideId.P1 => SideId.P2,
                SideId.P2 => SideId.P1,
                SideId.P3 => SideId.P4,
                SideId.P4 => SideId.P3,
                _ => throw new ArgumentException($"Invalid side: {side}")
            };
        }

        private static string VisualizeResult(object? result)
        {
            // Simple visualization - you might want to implement Utils.visualize equivalent
            if (result == null) return "null";
            if (result is string str) return str;
            return result.ToString() ?? "undefined";
        }

        private static PlayerOptions ConvertToPlayerOptions(Dictionary<string, object> optionsDict)
        {
            PlayerOptions playerOptions = new PlayerOptions();
            
            if (optionsDict.TryGetValue("name", out object? nameValue))
                playerOptions.Name = nameValue?.ToString();
            
            if (optionsDict.TryGetValue("avatar", out object? avatarValue))
                playerOptions.Avatar = avatarValue?.ToString();
            
            if (optionsDict.TryGetValue("rating", out object? ratingValue) && int.TryParse(ratingValue?.ToString(), out int rating))
                playerOptions.Rating = rating;
            
            if (optionsDict.TryGetValue("team", out object? teamValue))
                playerOptions.TeamString = teamValue?.ToString();
            else if (optionsDict.TryGetValue("teamString", out object? teamStringValue))
                playerOptions.TeamString = teamStringValue?.ToString();
            
            if (optionsDict.TryGetValue("seed", out object? seedValue))
                playerOptions.Seed = new PrngSeed(seedValue?.ToString() ?? "default");
            
            return playerOptions;
        }

        /// <summary>
        /// Processes eval messages in a controlled environment, simulating JavaScript eval functionality.
        /// Since C# doesn't have native eval, this provides common eval scenarios and introspection.
        /// </summary>
        private static string ProcessEvalMessage(string message, Battle battle, Side? p1, Side? p2, Side? p3, Side? p4,
            Pokemon? p1active, Pokemon? p2active, Pokemon? p3active, Pokemon? p4active,
            Func<string, Id> toID, Func<string, Side?> player, Func<object, string, Pokemon?> pokemon)
        {
            // Handle common eval patterns that would be used in battle debugging
            
            // Simple property access patterns
            if (message.Trim() == "battle.turn")
                return battle.Turn.ToString();
            if (message.Trim() == "battle.weather")
                return battle.Field.Weather?.ToString() ?? "none";
            if (message.Trim() == "battle.terrain")
                return battle.Field.Terrain?.ToString() ?? "none";
            
            // Side information
            if (message.Trim() == "p1?.name")
                return p1?.Name ?? "null";
            if (message.Trim() == "p2?.name")
                return p2?.Name ?? "null";
            if (message.Trim() == "p1active?.species")
                return p1active?.Species.Name ?? "null";
            if (message.Trim() == "p2active?.species")
                return p2active?.Species.Name ?? "null";
            if (message.Trim() == "p1active?.hp")
                return p1active?.Hp.ToString() ?? "null";
            if (message.Trim() == "p2active?.hp")
                return p2active?.Hp.ToString() ?? "null";
            
            // Handle function calls like player("p1") or pokemon("p1", "pikachu")
            if (message.Contains("player(") && message.Contains(")"))
            {
                Match match = System.Text.RegularExpressions.Regex.Match(message, @"player\([""']([^""']+)[""']\)");
                if (match.Success)
                {
                    Side? result = player(match.Groups[1].Value);
                    return result?.Name ?? "null";
                }
            }
            
            if (message.Contains("pokemon(") && message.Contains(")"))
            {
                Match match = System.Text.RegularExpressions.Regex.Match(message, @"pokemon\([""']([^""']+)[""'],\s*[""']([^""']+)[""']\)");
                if (match.Success)
                {
                    Pokemon? result = pokemon(match.Groups[1].Value, match.Groups[2].Value);
                    return result?.Species.Name ?? "null";
                }
            }
            
            // Handle simple arithmetic or string operations
            if (System.Text.RegularExpressions.Regex.IsMatch(message, @"^\d+\s*[\+\-\*\/]\s*\d+$"))
            {
                try
                {
                    // Simple arithmetic evaluation (very limited for security)
                    string[] parts = System.Text.RegularExpressions.Regex.Split(message.Trim(), @"([\+\-\*\/])");
                    if (parts.Length == 3 && 
                        int.TryParse(parts[0].Trim(), out int left) && 
                        int.TryParse(parts[2].Trim(), out int right))
                    {
                        return parts[1].Trim() switch
                        {
                            "+" => (left + right).ToString(),
                            "-" => (left - right).ToString(),
                            "*" => (left * right).ToString(),
                            "/" => right != 0 ? (left / right).ToString() : "Division by zero",
                            _ => "Unsupported operation"
                        };
                    }
                }
                catch
                {
                    return "Arithmetic error";
                }
            }
            
            // Return information about available variables and functions
            if (message.Trim().ToLower() == "help" || message.Trim() == "?")
            {
                return @"Available in eval context:
- battle (Battle object)
- p1, p2, p3, p4 (Side objects)
- p1active, p2active, p3active, p4active (Pokemon objects)
- player(name) function - find side by name
- pokemon(side, name) function - find pokemon
- toID(string) function - convert to ID
Examples: battle.turn, p1?.name, player('p1'), pokemon('p1', 'pikachu')";
            }
            
            // For complex expressions, return a helpful message
            return $"C# eval simulation: '{message}' - Use 'help' for available commands. Full JavaScript eval requires Roslyn scripting.";
        }
    }

    public abstract class BattlePlayer(object playerStream, bool debug = false)
    {
        public object Stream { get; } = playerStream; // Changed to object to accept any stream type
        public List<string> Log { get; } = [];
        public bool Debug { get; } = debug;
        private CancellationTokenSource? _cancellationTokenSource;

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            try
            {
                await foreach (string chunk in ReadStreamChunksAsync(_cancellationTokenSource.Token))
                {
                    Receive(chunk);
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when cancelled
            }
            catch (Exception ex)
            {
                ReceiveError(ex);
            }
        }

        private async IAsyncEnumerable<string> ReadStreamChunksAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            switch (Stream)
            {
                case ObjectReadWriteStream<string> readWriteStream:
                    await foreach (string chunk in readWriteStream.ReadAllChunksAsync()
                        .WithCancellation(cancellationToken))
                    {
                        yield return chunk;
                    }
                    break;

                case ObjectReadStream<string> readStream:
                    await foreach (string chunk in readStream.ReadAllChunksAsync()
                        .WithCancellation(cancellationToken))
                    {
                        yield return chunk;
                    }
                    break;

                case System.IO.Stream netStream:
                {
                    // Fallback for standard .NET streams
                    using StreamReader reader = new StreamReader(netStream, leaveOpen: true);
                    string? line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        yield return line;
                    }
                    break;
                }
                
                default:
                    throw new NotSupportedException($"Unsupported stream type: {Stream.GetType()}");
            }
        }

        public void Receive(string chunk)
        {
            if (string.IsNullOrEmpty(chunk)) return;

            foreach (string line in chunk.Split('\n', StringSplitOptions.RemoveEmptyEntries))
            {
                ReceiveLine(line);
            }
        }

        public void ReceiveLine(string line)
        {
            if (Debug) Console.WriteLine(line);

            if (!line.StartsWith('|')) return;

            string[] parts = SplitFirst(line[1..], "|");
            string cmd = parts[0];
            string rest = parts.Length > 1 ? parts[1] : string.Empty;

            switch (cmd)
            {
                case "request":
                    HandleRequestCommand(rest);
                    return;

                case "error":
                    ReceiveError(new Exception(rest));
                    return;
            }

            Log.Add(line);
        }

        private void HandleRequestCommand(string jsonData)
        {
            try
            {
                // Use more sophisticated JSON parsing based on your IChoiceRequest implementation
                IChoiceRequest? request = ParseChoiceRequest(jsonData);
                if (request != null)
                {
                    ReceiveRequest(request);
                }
            }
            catch (Exception ex)
            {
                ReceiveError(new Exception($"Failed to parse request: {ex.Message}", ex));
            }
        }

        protected virtual IChoiceRequest? ParseChoiceRequest(string jsonData)
        {
            // This would need to be implemented based on your specific IChoiceRequest types
            // You might need a custom JsonConverter or factory method
            JsonSerializerOptions options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return System.Text.Json.JsonSerializer.Deserialize<IChoiceRequest>(jsonData, options);
        }

        public abstract void ReceiveRequest(IChoiceRequest request);

        public virtual void ReceiveError(Exception error)
        {
            throw error;
        }

        public async Task ChooseAsync(string choice)
        {
            byte[] choiceBytes = System.Text.Encoding.UTF8.GetBytes(choice + "\n");
            
            switch (Stream)
            {
                case ObjectReadWriteStream<string> readWriteStream:
                    await readWriteStream.WriteAsync(choice);
                    break;
                case Stream netStream:
                    await netStream.WriteAsync(choiceBytes, 0, choiceBytes.Length);
                    await netStream.FlushAsync();
                    break;
                default:
                    throw new NotSupportedException($"Unsupported stream type for writing: {Stream.GetType()}");
            }
        }

        public void Choose(string choice)
        {
            byte[] choiceBytes = System.Text.Encoding.UTF8.GetBytes(choice + "\n");
            
            switch (Stream)
            {
                case ObjectReadWriteStream<string> readWriteStream:
                    // For synchronous operation, we need to block on the async method
                    Task.Run(async () => await readWriteStream.WriteAsync(choice)).Wait();
                    break;
                case System.IO.Stream netStream:
                    netStream.Write(choiceBytes, 0, choiceBytes.Length);
                    netStream.Flush();
                    break;
                default:
                    throw new NotSupportedException($"Unsupported stream type for writing: {Stream.GetType()}");
            }
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
        }

        // Replace the old synchronous Start method with proper async implementation
        public async Task Start()
        {
            await foreach (string chunk in ReadStreamChunksAsync())
            {
                Receive(chunk);
            }
        }
    }

    public class BattleTextStream // : Stream or custom ReadWriteStream<string>
    {
        public BattleStream BattleStream { get; }
        public string CurrentMessage { get; set; }

        public BattleTextStream(BattleStreamOptions? options = null)
        {
            BattleStream = new BattleStream(options);
            CurrentMessage = string.Empty;
            Listen();
        }

        private Delegate Listen()
        {
            throw new NotImplementedException("Listen method is not implemented yet.");
        }
        
        private void Write(string message)
        {
            throw new NotImplementedException("Write method is not implemented yet.");
        }

        private Delegate WriteEnd()
        {
            throw new NotImplementedException("WriteEnd method is not implemented yet.");
        }
    }
}