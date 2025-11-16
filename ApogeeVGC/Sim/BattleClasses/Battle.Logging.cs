using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Utils.Extensions;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class Battle
{
    private void UpdateAllPlayersUi(
        BattlePerspectiveType battlePerspectiveType = BattlePerspectiveType.InBattle)
    {
        // Parse new log entries once
        var parsedMessages = ParseLogToMessages(SentLogPos, Log.Count);

        // Send the same messages to all players (each with their own perspective)
        foreach (Side side in Sides)
        {
            BattlePerspective perspective = GetPerspectiveForSide(side.Id, battlePerspectiveType);
            EmitUpdate(side.Id, perspective, parsedMessages);
        }

        // Update the sent log position to mark these entries as processed
        SentLogPos = Log.Count;
    }

    public void Add(params PartFuncUnion[] parts)
    {
        // Full observability mode: no secret/shared split needed
        // All information is visible to both sides
        
        var formattedParts = new List<string>();
        
        foreach (PartFuncUnion part in parts)
        {
            if (part is FuncPartFuncUnion funcPart)
            {
                // Execute the function to get content
                SideSecretSharedResult result = funcPart.Func();
                // In full observability mode, use the secret (full information) for everyone
                formattedParts.Add(result.Secret.ToString());
            }
            else if (part is PartPartFuncUnion directPart)
            {
                // Direct value
                formattedParts.Add(FormatPart(directPart.Part));
            }
        }
        
        // Add single message to log
        string message = $"|{string.Join("|", formattedParts)}";
        Log.Add(message);
    }

    public void AddMove(params StringNumberDelegateObjectUnion[] args)
    {
        // Track this line's position for later attribute additions
        LastMoveLine = Log.Count;

        // Format and add the move line to the log
        string message = $"|{string.Join("|", args.Select(FormatArg))}";
        Log.Add(message);
    }

    public void AttrLastMove(params StringNumberDelegateObjectUnion[] args)
    {
        // No last move to attribute to
        if (LastMoveLine < 0) return;

        // Special handling for animation lines with [still]
        if (Log[LastMoveLine].StartsWith("|-anim|"))
        {
            if (args.Any(arg => FormatArg(arg) == "[still]"))
            {
                // Remove the animation line entirely
                Log.RemoveAt(LastMoveLine);
                LastMoveLine = -1;
                return;
            }
        }
        else if (args.Any(arg => FormatArg(arg) == "[still]"))
        {
            // If no animation plays, hide the target (index 4) to prevent information leak
            string[] parts = Log[LastMoveLine].Split('|');
            if (parts.Length > 4)
            {
                parts[4] = string.Empty;
                Log[LastMoveLine] = string.Join("|", parts);
            }
        }

        // Append the attributes to the last move line
        string attributes = $"|{string.Join("|", args.Select(FormatArg))}";
        Log[LastMoveLine] += attributes;
    }


    /// <summary>
    /// Logs debug information to the console.
    /// Only outputs the message if debug mode is enabled.
    /// </summary>
    /// <param name="activity">The debug message to log</param>
    public void Debug(string activity)
    {
        if (DebugMode)
        {
            Console.WriteLine(activity);
        }
    }

    /// <summary>
    /// Extracts and returns all debug messages from the battle log.
    /// In the TypeScript version, this uses extractChannelMessages with channel -1.
    /// </summary>
    /// <returns>A string containing all debug messages, separated by newlines</returns>
    public string GetDebugLog()
    {
        // Extract debug channel messages (channel -1 in the original)
        // This would need the extractChannelMessages function implementation
        // For now, we'll filter for debug messages manually
        var debugMessages = Log
            .Where(line => line.StartsWith("|debug|"))
            .Select(line => line[7..]); // Remove "|debug|" prefix

        return string.Join("\n", debugMessages);
    }

    /// <summary>
    /// Logs a debug error message to the battle log.
    /// Unlike Debug(), this always logs regardless of debug mode setting.
    /// Used for important errors that should always be recorded.
    /// </summary>
    /// <param name="activity">The error message to log</param>
    public void DebugError(string activity)
    {
        Add("debug", activity);
    }

    public void Hint(string hint, bool once = false, Side? side = null)
    {
        // Create a unique key for this hint
        string hintKey = side != null ? $"{side.Id}|{hint}" : hint;

        // If this hint has already been shown and once=true, skip it
        if (Hints.Contains(hintKey)) return;

        // Send the hint to the appropriate recipient(s)
        if (side != null)
        {
            AddSplit(side.Id, ["-hint", hint]);
        }
        else
        {
            Add("-hint", hint);
        }

        // Mark this hint as shown if once=true
        if (once) Hints.Add(hintKey);
    }

    public void AddSplit(SideId side, Part[] secret, Part[]? shared = null)
    {
        // Add the split marker with the side ID (lowercase)
        Log.Add($"|split|{side.GetSideIdName()}");

        // Add the secret parts (visible only to the specified side)
        Add(secret.Select(p => (PartFuncUnion)p).ToArray());

        // Add the shared parts (visible to all sides) or empty line
        if (shared is { Length: > 0 })
        {
            Add(shared.Select(p => (PartFuncUnion)p).ToArray());
        }
        else
        {
            Log.Add(string.Empty);
        }
    }

    public void SendUpdates()
    {
        // Don't send if there are no new log entries
        if (SentLogPos >= Log.Count) return;

        // Send new log entries to clients
        var updates = Log.Skip(SentLogPos).ToList();

        // Send requests to players if not already sent
        if (!SentRequests)
        {
            foreach (Side side in Sides)
            {
                side.EmitRequest();
            }
            //SentRequests = true;
        }

        // Update the position marker
        SentLogPos = Log.Count;

        // Send end-of-battle summary if battle ended and not already sent
        if (!SentEnd && Ended)
        {
            // Build the battle log object
            var log = new Dictionary<string, object>
            {
                ["winner"] = Winner ?? string.Empty,
                ["seed"] = PrngSeed,
                ["turns"] = Turn,
                ["p1"] = Sides[0].Name,
                ["p2"] = Sides[1].Name,
                ["p1team"] = Sides[0].Team,
                ["p2team"] = Sides[1].Team,
                ["score"] = new List<int> { Sides[0].PokemonLeft, Sides[1].PokemonLeft },
                ["inputLog"] = InputLog,
            };

            // Add P3/P4 data only if they exist (for multi-battles)
            if (Sides.Count > 2)
            {
                log["p3"] = Sides[2].Name;
                log["p3team"] = Sides[2].Team;
                log["score"] = new List<int>
                {
                    Sides[0].PokemonLeft,
                    Sides[1].PokemonLeft,
                    Sides[2].PokemonLeft,
                };
            }

            if (Sides.Count > 3)
            {
                log["p4"] = Sides[3].Name;
                log["p4team"] = Sides[3].Team;
                log["score"] = new List<int>
                {
                    Sides[0].PokemonLeft,
                    Sides[1].PokemonLeft,
                    Sides[2].PokemonLeft,
                    Sides[3].PokemonLeft,
                };
            }

            // Serialize and send the end message
            string logJson = System.Text.Json.JsonSerializer.Serialize(log);
            Send(SendType.End, new List<string> { logJson });
            SentEnd = true;
        }
    }

    /// <summary>
    /// Formats a Part for output to the battle log.
    /// Converts various Part types to their string representations.
    /// </summary>
    private static string FormatPart(Part part)
    {
        return part switch
        {
            StringPart s => s.Value,
            IntPart i => i.Value.ToString(),
            DoublePart d => d.Value.ToString("F"),
            BoolPart b => b.Value.ToString().ToLowerInvariant(),
            PokemonPart p => p.Pokemon.ToString(),
            SidePart s => s.Side.Id.GetSideIdName(),
            MovePart m => m.Move.Name,
            EffectPart e => e.Effect.Name,
            UndefinedPart => "undefined",
            _ => string.Empty,
        };
    }

    /// <summary>
    /// Formats a StringNumberDelegateObjectUnion argument for output.
    /// Converts various union types to their string representations.
    /// </summary>
    private static string FormatArg(StringNumberDelegateObjectUnion arg)
    {
        return arg switch
        {
            StringStringNumberDelegateObjectUnion s => s.Value,
            IntStringNumberDelegateObjectUnion i => i.Value.ToString(),
            DoubleStringNumberDelegateObjectUnion d => d.Value.ToString("F"),
            DelegateStringNumberDelegateObjectUnion del => del.Delegate.Method.Name,
            ObjectStringNumberDelegateObjectUnion obj => obj.Object.ToString() ?? string.Empty,
            _ => string.Empty,
        };
    }

    /// <summary>
    /// Logs a damage message to the battle log based on the effect causing the damage.
    /// Handles special cases like partially trapped, powder, and confusion.
    /// </summary>
    /// <param name="target">The Pokemon that took damage</param>
    /// <param name="damageAmount">The actual amount of damage dealt</param>
    /// <param name="source">The Pokemon that caused the damage (optional)</param>
    /// <param name="effect">The effect that caused the damage (optional)</param>
    private void PrintDamageMessage(Pokemon target, int damageAmount, Pokemon? source,
        Condition? effect)
    {
        if (!DisplayUi) return;

        // Get the effect name, converting "tox" to "psn" for display
        string? effectName = effect?.FullName == "tox" ? "psn" : effect?.FullName;

        switch (effect?.Id)
        {
            case ConditionId.PartiallyTrapped:
                // Get the source effect from the volatile condition
                if (target.Volatiles.TryGetValue(ConditionId.PartiallyTrapped,
                        out EffectState? ptState) &&
                    ptState.SourceEffect != null)
                {
                    // Add to log - will be parsed to BattleMessage automatically
                    Add("-damage", target, target.GetHealth,
                        $"[from] {ptState.SourceEffect.FullName}", "[partiallytrapped]");
                }
                break;

            case ConditionId.Powder:
                // Silent damage
                Add("-damage", target, target.GetHealth, "[silent]");
                break;

            case ConditionId.Confusion:
                // Add to log - will be parsed to BattleMessage automatically
                Add("-damage", target, target.GetHealth, "[from] confusion");
                break;

            default:
                if (effect?.EffectType == EffectType.Move || string.IsNullOrEmpty(effectName))
                {
                    // Simple damage from a move or no effect
                    Add("-damage", target, target.GetHealth);
                }
                else if (source != null &&
                         (source != target || effect?.EffectType == EffectType.Ability))
                {
                    // Damage from effect with source
                    Add("-damage", target, target.GetHealth, $"[from] {effectName}",
                        $"[of] {source}");
                }
                else
                {
                    // Damage from effect without source
                    Add("-damage", target, target.GetHealth, $"[from] {effectName}");
                }
                break;
        }

        // Messages will be sent automatically when UpdateAllPlayersUi() is called
        // No need to flush here - batching improves performance
    }

    /// <summary>
    /// Logs a heal message to the battle log based on the effect causing the healing.
    /// </summary>
    private void PrintHealMessage(Pokemon target, int healAmount, Pokemon? source, Condition? effect)
    {
        if (!DisplayUi) return;

        // Get the health status for the log message
        var healthFunc = target.GetHealth;

        // Determine if this is a drain effect
      bool isDrain = effect?.Id == ConditionId.Drain;

        if (isDrain && source != null)
        {
       // Drain healing shows the source
            Add("-heal", target, healthFunc, $"[heal]{healAmount}", "[from] drain", $"[of] {source}");
   }
   else if (effect != null && effect.Id != ConditionId.None)
   {
            // Healing from a specific effect
            string effectName = effect.FullName == "tox" ? "psn" : effect.FullName;
   if (source != null && source != target)
      {
        Add("-heal", target, healthFunc, $"[heal]{healAmount}", $"[from] {effectName}", $"[of] {source}");
            }
    else
         {
Add("-heal", target, healthFunc, $"[heal]{healAmount}", $"[from] {effectName}");
 }
        }
        else
        {
  // Simple heal with no effect
 Add("-heal", target, healthFunc, $"[heal]{healAmount}");
     }
    }

    /// <summary>
    /// Formats a PartFuncUnion for output to the battle log.
    /// Extracts the Part from PartPartFuncUnion and formats it.
    /// </summary>
    private static string FormatPartFuncUnion(PartFuncUnion partFuncUnion)
    {
        return partFuncUnion switch
        {
            PartPartFuncUnion p => FormatPart(p.Part),
            FuncPartFuncUnion => throw new InvalidOperationException(
                "Cannot format a function PartFuncUnion directly. This should be handled in the Add method."),
            _ => string.Empty,
        };
    }

    /// <summary>
    /// Add a battle message to the pending message queue.
    /// Messages will be sent to players when FlushMessages is called.
    /// </summary>
    public void AddMessage(BattleMessage message)
    {
        PendingMessages.Add(message);
    }

    /// <summary>
    /// Flush all pending messages to players with GUI interfaces.
    /// This is typically called after a state update to send accumulated messages.
    /// NOTE: This only sends messages, NOT perspective updates. 
    /// Use UpdatePlayerUi() to send perspective updates.
    /// </summary>
    public void FlushMessages()
    {
        if (PendingMessages.Count == 0) return;

// Determine perspective type based on current request state
        BattlePerspectiveType perspectiveType = RequestState == RequestState.TeamPreview
            ? BattlePerspectiveType.TeamPreview
            : BattlePerspectiveType.InBattle;

        // Send messages via UpdateRequested event with correct perspective
        foreach (Side side in Sides)
        {
            BattlePerspective perspective = GetPerspectiveForSide(side.Id, perspectiveType);

            UpdateRequested?.Invoke(this, new BattleUpdateEventArgs
            {
                SideId = side.Id,
                Perspective = perspective,
                Messages = new List<BattleMessage>(PendingMessages)
            });
        }

        // Clear the pending messages
        PendingMessages.Clear();
    }

    /// <summary>
    /// Add a message and immediately flush it to players.
    /// Use this for messages that should be sent immediately rather than batched.
    /// </summary>
    public void AddAndFlushMessage(BattleMessage message)
    {
        AddMessage(message);
        FlushMessages();
    }

    /// <summary>
    /// Parses log entries from the battle log and converts them to BattleMessage objects.
    /// This allows console and GUI players to display human-readable messages.
    /// </summary>
    /// <param name="startIndex">The index in the Log to start parsing from</param>
    /// <param name="endIndex">The index in the Log to stop parsing at (exclusive)</param>
    /// <returns>A list of BattleMessage objects parsed from the log entries</returns>
    private List<BattleMessage> ParseLogToMessages(int startIndex, int endIndex)
    {
        var messages = new List<BattleMessage>();

        bool inSplitSection = false;
        bool parseSplitLine = false; // True when we're on the "shared" line of a split

        for (int i = startIndex; i < endIndex && i < Log.Count; i++)
        {
            string logEntry = Log[i];

            // Check for split marker
            if (logEntry.StartsWith("|split|"))
            {
                inSplitSection = true;
                parseSplitLine = false;
                continue;
            }

            // If we're in a split section
            if (inSplitSection)
            {
                if (string.IsNullOrEmpty(logEntry))
                {
                    // Empty line marks the end of the split section
                    inSplitSection = false;
                    parseSplitLine = false;
                    continue;
                }
                else if (!parseSplitLine)
                {
                    // First non-empty line after split is the "secret" line - skip it
                    parseSplitLine = true;
                    continue;
                }
                // If parseSplitLine is true, this is the "shared" line - parse it below
// After parsing this line, the next will be empty (end of split)
            }

            if (string.IsNullOrEmpty(logEntry)) continue;

            // Parse the log entry (format: |command|arg1|arg2|...)
            string[] parts = logEntry.Split('|');
            if (parts.Length < 2) continue; // Need at least || and command

            string command = parts[1]; // parts[0] is empty due to leading |

            try
            {
                BattleMessage? message = command switch
                {
                    "turn" when parts.Length > 2 && int.TryParse(parts[2], out int turnNum) =>
                        new TurnStartMessage { TurnNumber = turnNum },

                    "move" when parts.Length > 3 =>
                     new MoveUsedMessage
        {
          PokemonName = ExtractPokemonName(parts[2]),
              SideId = ExtractSideId(parts[2]),
                MoveName = parts[3]
       },

     "switch" or "drag" when parts.Length > 3 =>
     new SwitchMessage
             {
     TrainerName = ExtractTrainerName(parts[2]),
     PokemonName = ExtractPokemonName(parts[2])
    },

   "faint" when parts.Length > 2 =>
     new FaintMessage 
     { 
      PokemonName = ExtractPokemonName(parts[2]),
           SideId = ExtractSideId(parts[2])
  },

    "-damage" when parts.Length > 3 =>
    ParseDamageMessage(parts),

 "-heal" when parts.Length > 3 =>
  ParseHealMessage(parts),

        "-status" when parts.Length > 3 =>
     new StatusMessage
      {
    PokemonName = ExtractPokemonName(parts[2]),
SideId = ExtractSideId(parts[2]),
     StatusName = parts[3]
                },

          "-supereffective" =>
new EffectivenessMessage
 {
                Effectiveness = EffectivenessMessage.EffectivenessType.SuperEffective
             },

                    "-resisted" =>
         new EffectivenessMessage
  {
         Effectiveness = EffectivenessMessage.EffectivenessType.NotVeryEffective
     },

              "-immune" =>
             new EffectivenessMessage
   {
     Effectiveness = EffectivenessMessage.EffectivenessType.NoEffect
           },

          "-crit" =>
    new CriticalHitMessage(),

         "-miss" when parts.Length > 2 =>
     new MissMessage 
           { 
          PokemonName = ExtractPokemonName(parts[2]),
     SideId = ExtractSideId(parts[2])
      },

             "-fail" when parts.Length > 2 =>
              new MoveFailMessage { Reason = parts.Length > 3 ? parts[3] : "Unknown" },

      "-boost" or "-unboost" when parts.Length > 4 =>
     ParseStatChangeMessage(parts, command == "-boost"),

  "-weather" when parts.Length > 2 =>
            new WeatherMessage
{
                 WeatherName = parts[2],
       IsEnding = parts.Length > 3 && parts[3] == "[upkeep]"
 },

         "-ability" when parts.Length > 3 =>
      new AbilityMessage
  {
     PokemonName = ExtractPokemonName(parts[2]),
     SideId = ExtractSideId(parts[2]),
           AbilityName = parts[3],
 AdditionalInfo = parts.Length > 4 ? parts[4] : null
         },

         "-item" when parts.Length > 3 =>
      new ItemMessage
    {
     PokemonName = ExtractPokemonName(parts[2]),
  SideId = ExtractSideId(parts[2]),
         ItemName = parts[3]
        },

 _ => null
     };

                if (message != null)
                {
                    messages.Add(message);
                }
            }
            catch (Exception ex)
            {
                Debug($"Error parsing log entry '{logEntry}': {ex.Message}");
            }
        }

        return messages;
    }

    /// <summary>
    /// Extracts a Pokemon name from a log entry part (format: "p1a: PokemonName")
    /// </summary>
    private static string ExtractPokemonName(string part)
    {
        int colonIndex = part.IndexOf(':');
        if (colonIndex >= 0 && colonIndex < part.Length - 1)
        {
      return part.Substring(colonIndex + 2).Trim(); // +2 to skip ": "
        }

        return part.Trim();
    }

    /// <summary>
    /// Extracts the SideId from a Pokemon identifier in a log entry part (format: "p1a: PokemonName")
    /// Returns null if the side cannot be determined.
/// </summary>
    private static SideId? ExtractSideId(string part)
    {
        if (part.StartsWith("p1"))
        {
        return SideId.P1;
        }
        else if (part.StartsWith("p2"))
        {
      return SideId.P2;
        }
        return null;
  }

    /// <summary>
    /// Extracts a trainer name from a log entry part (format: "p1a: PokemonName")
  /// Returns "Player 1" or "Player 2" based on the side prefix
    /// </summary>
    private string ExtractTrainerName(string part)
    {
        if (part.StartsWith("p1"))
        {
          return Sides[0].Name;
        }
        else if (part.StartsWith("p2"))
      {
        return Sides[1].Name;
        }

        return "Unknown";
    }

    /// <summary>
    /// Parses a damage message from log parts
    /// </summary>
    private static BattleMessage? ParseDamageMessage(string[] parts)
    {
        if (parts.Length < 4) return null;

        string pokemonName = ExtractPokemonName(parts[2]);
        SideId? sideId = ExtractSideId(parts[2]);
  string healthStr = parts[3];

   // Parse health (format: "123/456" or "123/456 psn")
 string[] healthParts = healthStr.Split(' ');
        string[] hpParts = healthParts[0].Split('/');

    if (hpParts.Length != 2 ||
   !int.TryParse(hpParts[0], out int currentHp) ||
   !int.TryParse(hpParts[1], out int maxHp))
        {
            return new GenericMessage { Text = $"{pokemonName} took damage!" };
        }

 int damageAmount = maxHp - currentHp; // Approximate, we don't have previous HP

   // Extract effect name if present
        string? effectName = null;
string? sourceName = null;

     for (int i = 4; i < parts.Length; i++)
     {
            if (parts[i].StartsWith("[from]"))
            {
                effectName = parts[i].Substring(7).Trim(); // Remove "[from] "
  }
   else if (parts[i].StartsWith("[of]"))
   {
  sourceName = ExtractPokemonName(parts[i].Substring(5).Trim()); // Remove "[of] "
  }
        }

        return new DamageMessage
        {
   PokemonName = pokemonName,
        SideId = sideId,
       DamageAmount = damageAmount,
            RemainingHp = currentHp,
      MaxHp = maxHp,
   EffectName = effectName,
      SourcePokemonName = sourceName
 };
    }

    /// <summary>
    /// Parses a heal message from log parts
    /// </summary>
    private static BattleMessage? ParseHealMessage(string[] parts)
    {
if (parts.Length < 4) return null;

   string pokemonName = ExtractPokemonName(parts[2]);
SideId? sideId = ExtractSideId(parts[2]);
      string healthStr = parts[3];

     // Parse health (format: "123/456")
     string[] healthParts = healthStr.Split(' ');
        string[] hpParts = healthParts[0].Split('/');

   if (hpParts.Length != 2 ||
      !int.TryParse(hpParts[0], out int currentHp) ||
 !int.TryParse(hpParts[1], out int maxHp))
      {
    return new GenericMessage { Text = $"{pokemonName} restored HP!" };
        }

        // Extract heal amount from [heal]amount tag
    int healAmount = 0;
        for (int i = 4; i < parts.Length; i++)
     {
 if (parts[i].StartsWith("[heal]"))
    {
                string amountStr = parts[i].Substring(6); // Remove "[heal]" prefix
          if (int.TryParse(amountStr, out int amount))
      {
             healAmount = amount;
   }
 break;
        }
   }

     return new HealMessage
   {
        PokemonName = pokemonName,
      SideId = sideId,
            HealAmount = healAmount,
       CurrentHp = currentHp,
  MaxHp = maxHp
      };
    }

    /// <summary>
    /// Parses a stat change message from log parts
    /// </summary>
  private static BattleMessage? ParseStatChangeMessage(string[] parts, bool isBoost)
    {
      if (parts.Length < 5) return null;

        string pokemonName = ExtractPokemonName(parts[2]);
        SideId? sideId = ExtractSideId(parts[2]);
        string statName = parts[3];
     int stages = int.TryParse(parts[4], out int stageNum) ? stageNum : 1;

 if (!isBoost)
        {
   stages = -stages; // Unboost is negative
        }

      return new StatChangeMessage
        {
PokemonName = pokemonName,
        SideId = sideId,
     StatName = statName,
          Stages = stages
        };
    }
}