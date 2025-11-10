using ApogeeVGC.Player;
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
        foreach (Side side in Sides)
        {
            UpdatePlayerUi(side.Id, battlePerspectiveType);
        }

        // Flush messages after all players have been updated
        FlushMessages();
    }

    private void UpdatePlayerUi(SideId sideId,
        BattlePerspectiveType battlePerspectiveType = BattlePerspectiveType.InBattle)
    {
   // Create perspective with the correct type
      BattlePerspective perspective = GetPerspectiveForSide(sideId, battlePerspectiveType);
        
      // Emit update event with the perspective
    EmitUpdate(sideId, perspective, new List<BattleMessage>(PendingMessages));
 }
    public void Add(params PartFuncUnion[] parts)
    {
        // Check if any part is a function that generates side-specific content
        bool hasFunction = parts.Any(part => part is FuncPartFuncUnion);

        if (!hasFunction)
        {
            // Simple case: all parts are direct values
            // Extract Part from PartPartFuncUnion before formatting
            string message = $"|{string.Join("|", parts.Select(FormatPartFuncUnion))}";
            Log.Add(message);
            return;
        }

        // Complex case: some parts are functions
        SideId? side = null;
        var secret = new List<string>();
        var shared = new List<string>();

        foreach (PartFuncUnion part in parts)
        {
            if (part is FuncPartFuncUnion funcPart)
            {
                // Execute the function to get side-specific content
                SideSecretSharedResult result = funcPart.Func();

                // Validate that all functions use the same side
                if (side.HasValue && side.Value != result.Side)
                {
                    throw new InvalidOperationException("Multiple sides passed to add");
                }

                side = result.Side;
                secret.Add(result.Secret.ToString());
                shared.Add(result.Shared.ToString());
            }
            else if (part is PartPartFuncUnion directPart)
            {
                // Direct value: add to both secret and shared
                string formatted = FormatPart(directPart.Part);
                secret.Add(formatted);
                shared.Add(formatted);
            }
        }

        // Add the split message
        if (side.HasValue)
        {
            AddSplit(side.Value,
                secret.Select(Part (s) => new StringPart(s)).ToArray(),
                shared.Select(Part (s) => new StringPart(s)).ToArray());
        }
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
    /// Logs debug information to the battle log.
    /// Only adds the message if debug mode is enabled.
    /// </summary>
    /// <param name="activity">The debug message to log</param>
    public void Debug(string activity)
    {
        if (DebugMode)
        {
            //Add("debug", activity);
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
        Console.WriteLine($"[SendUpdates] SentLogPos={SentLogPos}, Log.Count={Log.Count}");
        // Don't send if there are no new log entries
        if (SentLogPos >= Log.Count) return;

        // Send new log entries to clients
        var updates = Log.Skip(SentLogPos).ToList();
        Console.WriteLine($"[SendUpdates] Sending {updates.Count} updates");
        Send(SendType.Update, updates);

        // Send requests to players if not already sent
        if (!SentRequests)
        {
            Console.WriteLine("[SendUpdates] Sending requests to players");
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
    private void PrintDamageMessage(Pokemon target, int damageAmount, Pokemon? source, Condition? effect)
    {
        if (!DisplayUi) return;

        // Get the effect name, converting "tox" to "psn" for display
        string? effectName = effect?.FullName == "tox" ? "psn" : effect?.FullName;

        // Create the base damage message
        DamageMessage CreateDamageMessage(string? effectNameOverride = null,
            string? sourceNameOverride = null, string? specialTag = null)
        {
            return new DamageMessage
            {
                PokemonName = target.Name,
                DamageAmount = damageAmount,
                RemainingHp = target.Hp,
                MaxHp = target.MaxHp,
                EffectName = effectNameOverride ?? effectName,
                SourcePokemonName = sourceNameOverride,
                SpecialTag = specialTag
            };
        }

        switch (effect?.Id)
        {
            case ConditionId.PartiallyTrapped:
                // Get the source effect from the volatile condition
                if (target.Volatiles.TryGetValue(ConditionId.PartiallyTrapped,
                        out EffectState? ptState) &&
                    ptState.SourceEffect != null)
                {
                    AddMessage(CreateDamageMessage(
                        effectNameOverride: ptState.SourceEffect.FullName,
                        specialTag: "[partiallytrapped]"));

                    // Still add to legacy log
                    Add("-damage", target, target.GetHealth,
                        $"[from] {ptState.SourceEffect.FullName}", "[partiallytrapped]");
                }

                break;

            case ConditionId.Powder:
                // Silent damage - create message but mark as silent
                AddMessage(CreateDamageMessage(specialTag: "[silent]"));

                // Still add to legacy log
                Add("-damage", target, target.GetHealth, "[silent]");
                break;

            case ConditionId.Confusion:
                AddMessage(CreateDamageMessage(effectNameOverride: "confusion"));

                // Still add to legacy log
                Add("-damage", target, target.GetHealth, "[from] confusion");
                break;

            default:
                if (effect?.EffectType == EffectType.Move || string.IsNullOrEmpty(effectName))
                {
                    // Simple damage from a move or no effect
                    AddMessage(CreateDamageMessage());
                    Add("-damage", target, target.GetHealth);
                }
                else if (source != null &&
                         (source != target || effect?.EffectType == EffectType.Ability))
                {
                    // Damage from effect with source
                    AddMessage(CreateDamageMessage(sourceNameOverride: source.Name));
                    Add("-damage", target, target.GetHealth, $"[from] {effectName}",
                        $"[of] {source}");
                }
                else
                {
                    // Damage from effect without source
                    AddMessage(CreateDamageMessage());
                    Add("-damage", target, target.GetHealth, $"[from] {effectName}");
                }

                break;
        }

        // Flush messages immediately so GUI players see damage updates
        FlushMessages();
    }

    /// <summary>
    /// Logs a heal message to the battle log based on the effect causing the healing.
    /// </summary>
    private void PrintHealMessage(Pokemon target, Pokemon? source, Condition? effect)
    {
        if (!DisplayUi) return;

        // Get the health status for the log message
        var healthFunc = target.GetHealth;

        // Determine if this is a drain effect
        bool isDrain = effect?.Id == ConditionId.Drain;

        if (isDrain && source != null)
        {
            // Drain healing shows the source
            Add("-heal", target, healthFunc, "[from] drain", $"[of] {source}");
        }
        else if (effect != null && effect.Id != ConditionId.None)
        {
            // Healing from a specific effect
            string effectName = effect.FullName == "tox" ? "psn" : effect.FullName;
            if (source != null && source != target)
            {
                Add("-heal", target, healthFunc, $"[from] {effectName}", $"[of] {source}");
            }
            else
            {
                Add("-heal", target, healthFunc, $"[from] {effectName}");
            }
        }
        else
        {
            // Simple heal with no effect
            Add("-heal", target, healthFunc);
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
}