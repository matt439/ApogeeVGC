using System.Text;
using System.Text.RegularExpressions;

namespace EventHandlerUpdater;

/// <summary>
/// Automatically updates all EventHandlerInfo subclasses to support context-based handlers.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        string projectRoot = args.Length > 0 
            ? args[0] 
 : @"C:\VSProjects\ApogeeVGC\ApogeeVGC\Sim\Events\Handlers";
     
        Console.WriteLine($"Scanning: {projectRoot}");
      
        var files = Directory.GetFiles(projectRoot, "*EventInfo.cs", SearchOption.AllDirectories);
  
        Console.WriteLine($"Found {files.Length} event handler files");
        
        int updated = 0;
        int skipped = 0;
        int errors = 0;
     
 foreach (var file in files)
  {
      try
          {
 var result = UpdateEventHandlerFile(file);
            if (result == UpdateResult.Updated)
   {
              updated++;
         Console.WriteLine($"? {Path.GetFileName(file)}");
        }
   else if (result == UpdateResult.AlreadyUpdated)
                {
          skipped++;
         }
                else
     {
       errors++;
             Console.WriteLine($"? {Path.GetFileName(file)} - {result}");
          }
      }
            catch (Exception ex)
         {
          errors++;
           Console.WriteLine($"? {Path.GetFileName(file)} - ERROR: {ex.Message}");
          }
        }
  
  Console.WriteLine();
 Console.WriteLine($"Complete: {updated} updated, {skipped} skipped, {errors} errors");
  }
    
    enum UpdateResult
    {
        Updated,
        AlreadyUpdated,
    NoConstructorFound,
NoHandlerParameter,
     ParseError
    }
    
  static UpdateResult UpdateEventHandlerFile(string filePath)
    {
     var content = File.ReadAllText(filePath);
        
        // Check if already updated
      if (content.Contains("EventHandlerDelegate contextHandler"))
        {
       return UpdateResult.AlreadyUpdated;
  }
        
        // Extract class name
      var classMatch = Regex.Match(content, @"public sealed record (\w+EventInfo)\s*:");
        if (!classMatch.Success)
        {
            return UpdateResult.ParseError;
        }
        
      string className = classMatch.Groups[1].Value;
        
        // Extract EventId if present
        string? eventId = null;
        var eventIdMatch = Regex.Match(content, @"Id\s*=\s*EventId\.(\w+)");
        if (eventIdMatch.Success)
    {
    eventId = eventIdMatch.Groups[1].Value;
 }
        
        // Find existing constructor
  var constructorPattern = $@"public {className}\s*\(([^{{]+?)\)\s*{{";
      var constructorMatch = Regex.Match(content, constructorPattern, RegexOptions.Singleline);
        
        if (!constructorMatch.Success)
        {
      return UpdateResult.NoConstructorFound;
     }
        
        string constructorParams = constructorMatch.Groups[1].Value;
   
        // Parse parameters
        var parameters = ParseParameters(constructorParams);
  
        // Find handler parameter
        var handlerParam = parameters.FirstOrDefault(p => 
            p.Type.StartsWith("Action<") || p.Type.StartsWith("Func<"));
        
        if (handlerParam == null)
        {
   return UpdateResult.NoHandlerParameter;
        }
        
        // Get optional parameters (non-handler)
        var optionalParams = parameters.Where(p => p != handlerParam).ToList();
        
        // Build new content
        var newConstructors = BuildNewConstructors(className, eventId, handlerParam, optionalParams);
     
        // Find last closing brace and insert before it
        int lastBrace = content.LastIndexOf('}');
        if (lastBrace == -1)
        {
      return UpdateResult.ParseError;
      }
      
        var updatedContent = content.Substring(0, lastBrace) + newConstructors + "\n}";
      
     // Write back
      File.WriteAllText(filePath, updatedContent);
        
        return UpdateResult.Updated;
    }
    
    class Parameter
    {
        public string Type { get; set; } = "";
        public string Name { get; set; } = "";
        public bool HasDefault { get; set; }
     public string? DefaultValue { get; set; }
    }
    
    static List<Parameter> ParseParameters(string paramString)
    {
     var parameters = new List<Parameter>();
        
      // Split by comma, but respect angle brackets
        int depth = 0;
        int start = 0;

   for (int i = 0; i < paramString.Length; i++)
        {
            char c = paramString[i];
 
            if (c == '<') depth++;
            else if (c == '>') depth--;
            else if (c == ',' && depth == 0)
     {
    parameters.Add(ParseSingleParameter(paramString.Substring(start, i - start)));
     start = i + 1;
    }
    }
      
        // Don't forget the last parameter
        if (start < paramString.Length)
        {
            parameters.Add(ParseSingleParameter(paramString.Substring(start)));
  }
        
  return parameters;
    }
  
    static Parameter ParseSingleParameter(string param)
    {
        param = param.Trim();
        
      // Match: Type name = default
        var match = Regex.Match(param, @"([\w<>?\[\],\s]+?)\s+(\w+)(\s*=\s*(.+))?");
        
        if (match.Success)
        {
    return new Parameter
{
           Type = match.Groups[1].Value.Trim(),
         Name = match.Groups[2].Value,
                HasDefault = match.Groups[3].Success,
         DefaultValue = match.Groups[4].Success ? match.Groups[4].Value.Trim() : null
            };
        }
        
  return new Parameter { Type = param, Name = "unknown" };
    }
    
 static string BuildNewConstructors(
        string className,
        string? eventId,
        Parameter handlerParam,
        List<Parameter> optionalParams)
    {
        var sb = new StringBuilder();
        
// Context constructor
        sb.AppendLine();
        sb.AppendLine("    /// <summary>");
      sb.AppendLine("    /// Creates event handler using context-based pattern.");
        sb.AppendLine("    /// Context provides: Battle, TargetPokemon, SourcePokemon, Move, SourceEffect, etc.");
  sb.AppendLine("    /// </summary>");
   sb.Append($"    public {className}(\n    EventHandlerDelegate contextHandler");
        
  foreach (var param in optionalParams)
    {
   sb.Append($",\n {param.Type} {param.Name}");
         if (param.HasDefault)
    {
        sb.Append($" = {param.DefaultValue}");
         }
        }
        
sb.AppendLine(")");
        sb.AppendLine("    {");
   
 if (eventId != null)
        {
   sb.AppendLine($"        Id = EventId.{eventId};");
  }
      
        sb.AppendLine("  ContextHandler = contextHandler;");
        
    foreach (var param in optionalParams)
        {
 sb.AppendLine($"      {param.Name} = {param.Name};");
        }
 
        sb.AppendLine("    }");
        
        // Create method
        sb.AppendLine();
      sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Creates strongly-typed context-based handler.");
        sb.AppendLine("    /// Best of both worlds: strongly-typed parameters + context performance.");
        sb.AppendLine("    /// </summary>");
        sb.Append($"    public static {className} Create(\n        {handlerParam.Type} handler");
        
        foreach (var param in optionalParams)
        {
  sb.Append($",\n        {param.Type} {param.Name}");
        if (param.HasDefault)
            {
 sb.Append($" = {param.DefaultValue}");
}
    }
 
      sb.AppendLine(")");
        sb.AppendLine("    {");
        sb.AppendLine($"        return new {className}(");
        sb.AppendLine("   context => ConvertLegacyHandler(handler, context)");
        
    foreach (var param in optionalParams)
        {
       sb.AppendLine($"   , {param.Name}");
        }
        
sb.AppendLine("        );");
 sb.AppendLine("    }");
        
        return sb.ToString();
    }
}
