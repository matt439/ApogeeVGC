using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

/// <summary>
/// Script to generate remaining EventHandlerInfo records for all prefix variants.
/// Run this to complete the remaining ~200 records automatically.
/// </summary>
public class EventHandlerInfoGenerator
{
    private static readonly string OutputDirectory = @"ApogeeVGC\Sim\Events\Handlers\EventMethods";
    
  // Base events that need prefix variants created
    private static readonly Dictionary<string, EventSignature> BaseEvents = new()
    {
        // Action<Battle, Pokemon> - Simple 2-param events
      ["EmergencyExit"] = new("Action<Battle, Pokemon>", "void", 
            ["typeof(Battle)", "typeof(Pokemon)"]),
        ["BeforeSwitchIn"] = new("Action<Battle, Pokemon>", "void",
     ["typeof(Battle)", "typeof(Pokemon)"]),
        ["BeforeTurn"] = new("Action<Battle, Pokemon>", "void",
            ["typeof(Battle)", "typeof(Pokemon)"]),
        ["Update"] = new("Action<Battle, Pokemon>", "void",
         ["typeof(Battle)", "typeof(Pokemon)"]),
   
  // Action<Battle, Pokemon, Pokemon> - 3-param events
      ["Attract"] = new("Action<Battle, Pokemon, Pokemon>", "void",
   ["typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)"]),
   
        // Action<Battle, Pokemon, Pokemon, IEffect> - 4-param with effect
        ["BeforeFaint"] = new("Action<Battle, Pokemon, IEffect>", "void",
    ["typeof(Battle)", "typeof(Pokemon)", "typeof(IEffect)"]),
          
        // Action with Item
        ["AfterUseItem"] = new("Action<Battle, Item, Pokemon>", "void",
   ["typeof(Battle)", "typeof(Item)", "typeof(Pokemon)"]),
        ["AfterTakeItem"] = new("Action<Battle, Item, Pokemon>", "void",
  ["typeof(Battle)", "typeof(Item)", "typeof(Pokemon)"]),
        ["EatItem"] = new("Action<Battle, Item, Pokemon>", "void",
["typeof(Battle)", "typeof(Item)", "typeof(Pokemon)"]),
         
        // Move-based events with BoolVoidUnion
        ["ChargeMove"] = new("Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion>", "BoolVoidUnion",
  ["typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)"]),
            
        // Modifier events
    ["ModifyDef"] = new("Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>", "DoubleVoidUnion",
["typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)"]),
        ["ModifyCritRatio"] = new("Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>", "DoubleVoidUnion",
            ["typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)"]),
 ["ModifyPriority"] = new("Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>", "DoubleVoidUnion",
     ["typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)"]),
        ["ModifyStab"] = new("Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>", "DoubleVoidUnion",
  ["typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)"]),
          
        // Other events
   ["ModifySecondaries"] = new("Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>", "void",
     ["typeof(Battle)", "typeof(List<SecondaryEffect>)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)"]),
        ["ModifySpe"] = new("Func<Battle, int, Pokemon, IntVoidUnion>", "IntVoidUnion",
            ["typeof(Battle)", "typeof(int)", "typeof(Pokemon)"]),
    ["ModifyWeight"] = new("Func<Battle, int, Pokemon, IntVoidUnion>", "IntVoidUnion",
     ["typeof(Battle)", "typeof(int)", "typeof(Pokemon)"]),
        ["MoveAborted"] = new("Action<Battle, Pokemon, Pokemon, ActiveMove>", "void",
            ["typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)"]),
    ["DisableMove"] = new("Action<Battle, Pokemon>", "void",
        ["typeof(Battle)", "typeof(Pokemon)"]),
      ["DeductPp"] = new("Func<Battle, Pokemon, Pokemon, int>", "int",
            ["typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)"]),
     ["DragOut"] = new("Action<Battle, Pokemon, Pokemon?, ActiveMove?>", "void",
            ["typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)"]),
   ["EntryHazard"] = new("Action<Battle, Pokemon>", "void",
      ["typeof(Battle)", "typeof(Pokemon)"]),
["TrapPokemon"] = new("Action<Battle, Pokemon>", "void",
          ["typeof(Battle)", "typeof(Pokemon)"]),
        ["MaybeTrapPokemon"] = new("Action<Battle, Pokemon, Pokemon?>", "void",
            ["typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)"]),
 ["ChangeBoost"] = new("Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>", "void",
            ["typeof(Battle)", "typeof(SparseBoostsTable)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)"]),
        ["AfterMove"] = new("Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion>", "BoolVoidUnion",
    ["typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)"]),
        ["AfterMoveSelf"] = new("Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion>", "BoolVoidUnion",
            ["typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)"]),
        ["AfterMoveSecondary"] = new("Action<Battle, Pokemon, Pokemon, ActiveMove>", "void",
        ["typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)"]),
      ["AfterMoveSecondarySelf"] = new("Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion>", "BoolVoidUnion",
   ["typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)"]),
        ["AfterFaint"] = new("Action<Battle, int, Pokemon, Pokemon, IEffect>", "void",
["typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)"]),
  ["AfterEachBoost"] = new("Action<Battle, SparseBoostsTable, Pokemon, Pokemon>", "void",
            ["typeof(Battle)", "typeof(SparseBoostsTable)", "typeof(Pokemon)", "typeof(Pokemon)"]),
 ["AfterSubDamage"] = new("Action<Battle, int, Pokemon, Pokemon, ActiveMove>", "void",
     ["typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)"]),
     ["AfterSwitchInSelf"] = new("Action<Battle, Pokemon>", "void",
["typeof(Battle)", "typeof(Pokemon)"]),
        ["TryBoost"] = new("Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>", "void",
     ["typeof(Battle)", "typeof(SparseBoostsTable)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)"]),
        ["Immunity"] = new("Action<Battle, PokemonType, Pokemon>", "void",
     ["typeof(Battle)", "typeof(PokemonType)", "typeof(Pokemon)"]),
      ["CriticalHit"] = new("Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion>", "BoolVoidUnion",
  ["typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)"]),
        ["Flinch"] = new("Func<Battle, Pokemon, BoolVoidUnion>", "BoolVoidUnion",
          ["typeof(Battle)", "typeof(Pokemon)"]),
    ["LockMove"] = new("Func<Battle, Pokemon, ActiveMove?>", "ActiveMove",
            ["typeof(Battle)", "typeof(Pokemon)"]),
     ["FractionalPriority"] = new("Func<Battle, int, Pokemon, ActiveMove, double>", "double",
 ["typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(ActiveMove)"]),
    ["OverrideAction"] = new("Func<Battle, Pokemon, Pokemon, ActiveMove, DelegateVoidUnion>", "DelegateVoidUnion",
            ["typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)"]),
        ["RedirectTarget"] = new("Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, PokemonVoidUnion>", "PokemonVoidUnion",
["typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)", "typeof(ActiveMove)"]),
        ["TryHitField"] = new("Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?>", "BoolEmptyVoidUnion",
      ["typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)"]),
        ["TryHitSide"] = new("Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?>", "BoolEmptyVoidUnion",
            ["typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)"]),
        ["TryMove"] = new("Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?>", "BoolEmptyVoidUnion",
            ["typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)"]),
      ["TryPrimaryHit"] = new("Func<Battle, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>", "IntBoolVoidUnion",
   ["typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)"]),
        ["ModifyMove"] = new("Action<Battle, ActiveMove, Pokemon, Pokemon?>", "void",
["typeof(Battle)", "typeof(ActiveMove)", "typeof(Pokemon)", "typeof(Pokemon)"]),
        ["ModifyType"] = new("Action<Battle, ActiveMove, Pokemon, Pokemon>", "void",
        ["typeof(Battle)", "typeof(ActiveMove)", "typeof(Pokemon)", "typeof(Pokemon)"]),
        ["ModifyTarget"] = new("Action<Battle, Pokemon, Pokemon, Pokemon, ActiveMove>", "void",
 ["typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)"]),
        ["Effectiveness"] = new("Func<Battle, int, Pokemon?, PokemonType, ActiveMove, IntVoidUnion>", "IntVoidUnion",
            ["typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(PokemonType)", "typeof(ActiveMove)"]),
        ["NegateImmunity"] = new("Func<Battle, Pokemon, PokemonType?, BoolVoidUnion>", "BoolVoidUnion",
  ["typeof(Battle)", "typeof(Pokemon)", "typeof(PokemonType)"]),
        ["TryEatItem"] = new("Func<Battle, Item, Pokemon, BoolVoidUnion>", "BoolVoidUnion",
     ["typeof(Battle)", "typeof(Item)", "typeof(Pokemon)"]),
        ["TryHeal"] = new("Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?>", "IntBoolVoidUnion",
        ["typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)"]),
        ["TakeItem"] = new("Func<Battle, Item, Pokemon, Pokemon, BoolVoidUnion>", "BoolVoidUnion",
            ["typeof(Battle)", "typeof(Item)", "typeof(Pokemon)", "typeof(Pokemon)"]),
     ["TryAddVolatile"] = new("Func<Battle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?>", "BoolVoidUnion",
     ["typeof(Battle)", "typeof(Condition)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)"]),
        ["Type"] = new("Func<Battle, PokemonType[], Pokemon, TypesVoidUnion>", "TypesVoidUnion",
 ["typeof(Battle)", "typeof(PokemonType[])", "typeof(Pokemon)"]),
        ["UseItem"] = new("Action<Battle, Item, Pokemon>", "void",
     ["typeof(Battle)", "typeof(Item)", "typeof(Pokemon)"]),
        ["Weather"] = new("Action<Battle, Pokemon, object?, Condition>", "void",
            ["typeof(Battle)", "typeof(Pokemon)", "typeof(object)", "typeof(Condition)"]),
        ["WeatherChange"] = new("Action<Battle, Pokemon, Pokemon, IEffect>", "void",
            ["typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)"]),
        ["TerrainChange"] = new("Action<Battle, Pokemon, Pokemon, IEffect>", "void",
            ["typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)"]),
        ["PseudoWeatherChange"] = new("Action<Battle, Pokemon, Pokemon, Condition>", "void",
     ["typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(Condition)"]),
        ["SetAbility"] = new("Action<Battle, Ability, Pokemon, Pokemon, IEffect>", "void",
       ["typeof(Battle)", "typeof(Ability)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)"]),
 ["SetWeather"] = new("Func<Battle, Pokemon, Pokemon, Condition, BoolVoidUnion>", "BoolVoidUnion",
          ["typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(Condition)"]),
        ["SideConditionStart"] = new("Action<Battle, Side, Pokemon, Condition>", "void",
       ["typeof(Battle)", "typeof(Side)", "typeof(Pokemon)", "typeof(Condition)"]),
        ["StallMove"] = new("Func<Battle, Pokemon, BoolVoidUnion>", "BoolVoidUnion",
    ["typeof(Battle)", "typeof(Pokemon)"]),
        ["SwitchIn"] = new("Action<Battle, Pokemon>", "void",
   ["typeof(Battle)", "typeof(Pokemon)"]),
   ["Swap"] = new("Action<Battle, Pokemon, Pokemon>", "void",
         ["typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)"]),
        ["WeatherModifyDamage"] = new("Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>", "DoubleVoidUnion",
            ["typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)"]),
        ["ModifyDamagePhase1"] = new("Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>", "DoubleVoidUnion",
            ["typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)"]),
    ["ModifyDamagePhase2"] = new("Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>", "DoubleVoidUnion",
            ["typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)"]),
    };
    
    private static readonly string[] Prefixes = ["Foe", "Source", "Ally", "Any"];
    
    public static void Main(string[] args)
{
   Console.WriteLine("EventHandlerInfo Generator");
        Console.WriteLine("=========================\n");
 
        int created = 0;
        int skipped = 0;
    
        foreach (var prefix in Prefixes)
      {
  foreach (var (eventName, signature) in BaseEvents)
    {
 string fileName = $"On{prefix}{eventName}EventInfo.cs";
     string filePath = Path.Combine(OutputDirectory, fileName);
            
 if (File.Exists(filePath))
          {
         Console.WriteLine($"??  Skipping {fileName} (already exists)");
          skipped++;
   continue;
                }
           
   string content = GenerateEventHandlerInfo(prefix, eventName, signature);
         
      try
           {
        File.WriteAllText(filePath, content);
 Console.WriteLine($"? Created {fileName}");
        created++;
         }
             catch (Exception ex)
 {
         Console.WriteLine($"? Error creating {fileName}: {ex.Message}");
        }
 }
}
        
        Console.WriteLine($"\n?? Summary:");
        Console.WriteLine($" Created: {created}");
        Console.WriteLine($"   Skipped: {skipped}");
        Console.WriteLine($"   Total:   {created + skipped}");
    Console.WriteLine($"\n? Generation complete!");
    }
    
    private static string GenerateEventHandlerInfo(string prefix, string eventName, EventSignature signature)
    {
        string className = $"On{prefix}{eventName}EventInfo";
        string description = GetEventDescription(prefix, eventName);
        
     var usings = GetRequiredUsings(signature);
        
     var sb = new StringBuilder();
   
        // Usings
        foreach (var u in usings.OrderBy(x => x))
        {
            sb.AppendLine(u);
        }
        sb.AppendLine();
sb.AppendLine("namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;");
   sb.AppendLine();
        
        // XML Comment
        sb.AppendLine("/// <summary>");
        sb.AppendLine($"/// Event handler info for On{prefix}{eventName} event.");
     sb.AppendLine($"/// {description}");
        sb.AppendLine($"/// Signature: {signature.DelegateType}");
        sb.AppendLine("/// </summary>");
        
        // Class declaration
        sb.AppendLine($"public sealed record {className} : EventHandlerInfo");
        sb.AppendLine("{");
  
        // Constructor
   sb.AppendLine($"    public {className}(");
    sb.AppendLine($"        {signature.DelegateType} handler,");
        sb.AppendLine("        int? priority = null,");
      sb.AppendLine("        bool usesSpeed = true)");
        sb.AppendLine("    {");
        sb.AppendLine($"      Id = EventId.{eventName};");
        sb.AppendLine($" Prefix = EventPrefix.{prefix};");
        sb.AppendLine("      Handler = handler;");
        sb.AppendLine("     Priority = priority;");
        sb.AppendLine("        UsesSpeed = usesSpeed;");
     
      // ExpectedParameterTypes
        sb.Append("        ExpectedParameterTypes = [");
        sb.Append(string.Join(", ", signature.ParameterTypes));
    sb.AppendLine("];");
  
        // ExpectedReturnType
        sb.AppendLine($"        ExpectedReturnType = typeof({signature.ReturnType});");
        
    sb.AppendLine("  }");
        sb.AppendLine("}");
        
        return sb.ToString();
    }
    
    private static string GetEventDescription(string prefix, string eventName)
    {
    return eventName switch
        {
  "DamagingHit" => $"Triggered after a damaging hit on {GetTargetDescription(prefix)}.",
            "AfterHit" => $"Triggered after a move hits {GetTargetDescription(prefix)}.",
   "BeforeMove" => $"Triggered before {GetTargetDescription(prefix)} uses a move.",
   "SetStatus" => $"Triggered when attempting to set status on {GetTargetDescription(prefix)}.",
            "AfterSetStatus" => $"Triggered after status is set on {GetTargetDescription(prefix)}.",
  _ => $"Triggered when the {eventName} event occurs for {GetTargetDescription(prefix)}."
};
    }
    
    private static string GetTargetDescription(string prefix) => prefix switch
    {
        "Foe" => "a foe",
        "Source" => "this Pokemon as source",
        "Ally" => "an ally",
        "Any" => "any Pokemon",
        _ => "a Pokemon"
    };
    
    private static HashSet<string> GetRequiredUsings(EventSignature signature)
    {
  var usings = new HashSet<string>
     {
            "using ApogeeVGC.Sim.BattleClasses;",
 "using ApogeeVGC.Sim.PokemonClasses;"
      };
        
    if (signature.DelegateType.Contains("ActiveMove") || signature.DelegateType.Contains("Move"))
    usings.Add("using ApogeeVGC.Sim.Moves;");
        
    if (signature.DelegateType.Contains("IEffect") || signature.DelegateType.Contains("Effect"))
   usings.Add("using ApogeeVGC.Sim.Effects;");
  
        if (signature.DelegateType.Contains("Item"))
   usings.Add("using ApogeeVGC.Sim.Items;");
        
  if (signature.DelegateType.Contains("Condition"))
            usings.Add("using ApogeeVGC.Sim.Conditions;");
     
        if (signature.DelegateType.Contains("Ability"))
 usings.Add("using ApogeeVGC.Sim.Abilities;");
        
        if (signature.DelegateType.Contains("Side"))
usings.Add("using ApogeeVGC.Sim.SideClasses;");
        
  if (signature.DelegateType.Contains("SparseBoostsTable"))
         usings.Add("using ApogeeVGC.Sim.Stats;");
        
        if (signature.DelegateType.Contains("Union") || 
     signature.ReturnType.Contains("Union"))
        usings.Add("using ApogeeVGC.Sim.Utils.Unions;");
        
        return usings;
    }
    
  private record EventSignature(
  string DelegateType,
        string ReturnType,
        string[] ParameterTypes);
}
