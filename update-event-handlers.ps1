# PowerShell Script to Update All Event Handler Classes
# This script adds context constructor and Create method to all EventHandlerInfo subclasses

param(
    [string]$ProjectRoot = "ApogeeVGC\Sim\Events\Handlers"
)

# Parameter mapping for common types
$parameterMappings = @{
    'Battle' = 'context.Battle'
    'Pokemon' = 'context.GetTargetPokemon()' # First Pokemon is typically target
  'ActiveMove' = 'context.GetMove()'
    'IEffect' = 'context.GetSourceEffect<IEffect>()'
    'Side' = 'context.GetTargetSide()'
    'Field' = 'context.Battle.Field'
    'int' = 'context.GetRelayVar<IntRelayVar>().Value'
    'decimal' = 'context.GetRelayVar<DecimalRelayVar>().Value'
    'bool' = 'context.GetRelayVar<BoolRelayVar>().Value'
}

function Get-ContextAccessor {
    param([string]$typeName, [int]$position, [string]$paramName)
    
    # Handle specific parameter names
    if ($paramName -match 'target') {
  if ($typeName -eq 'Pokemon') { return 'context.GetTargetPokemon()' }
        if ($typeName -eq 'Side') { return 'context.GetTargetSide()' }
    }
    
    if ($paramName -match 'source' -and $typeName -eq 'Pokemon') {
        return 'context.GetSourcePokemon()'
    }
    
    if ($paramName -match 'effect' -or $paramName -match 'sourceEffect') {
        return 'context.GetSourceEffect<IEffect>()'
    }
    
    if ($paramName -match 'move') {
        return 'context.GetMove()'
 }
    
    # Default mappings by type
    if ($parameterMappings.ContainsKey($typeName)) {
        return $parameterMappings[$typeName]
    }
    
    return $null
}

function Update-EventHandlerFile {
 param([string]$filePath)
    
    Write-Host "Processing: $filePath"
    
    $content = Get-Content $filePath -Raw
    
    # Check if already has ContextHandler constructor
    if ($content -match 'EventHandlerDelegate contextHandler') {
   Write-Host "  Already updated, skipping"
        return
    }
    
    # Extract class name
    if ($content -match 'public sealed record (\w+EventInfo)\s*:') {
        $className = $Matches[1]
    } else {
     Write-Host "  Could not find class name, skipping"
    return
    }
    
    # Extract EventId
    $eventId = $null
    if ($content -match 'Id\s*=\s*EventId\.(\w+)') {
        $eventId = $Matches[1]
    }
    
    # Extract existing constructor signature
    $constructorPattern = "public\s+$className\s*\(([^)]+)\)"
    if ($content -match $constructorPattern) {
     $constructorParams = $Matches[1]
        
        # Parse parameters
        $params = $constructorParams -split ',' | ForEach-Object {
  $param = $_.Trim()
       if ($param -match '([\w<>?]+)\s+(\w+)(\s*=\s*null)?') {
     @{
          Type = $Matches[1]
              Name = $Matches[2]
     HasDefault = $Matches[3] -ne $null
      }
}
        }
        
        # Find the handler parameter
        $handlerParam = $params | Where-Object { 
            $_.Type -match '^(Action|Func)' 
        } | Select-Object -First 1
        
        if (-not $handlerParam) {
   Write-Host "  No handler parameter found, skipping"
     return
        }
        
        # Extract handler type parameters
        if ($handlerParam.Type -match '(Action|Func)<(.+)>') {
            $delegateType = $Matches[1]
$typeParams = $Matches[2] -split ',' | ForEach-Object { $_.Trim() }
    
         # Build context accessor calls
        $contextAccessors = @()
            for ($i = 0; $i -lt $typeParams.Count; $i++) {
 $typeName = $typeParams[$i]
    $paramName = "param$i"
    $accessor = Get-ContextAccessor $typeName $i $paramName
                if ($accessor) {
           $contextAccessors += $accessor
           }
     }
            
      # Get all optional parameters (excluding handler)
$optionalParams = $params | Where-Object { 
  $_.Name -ne $handlerParam.Name 
            }
  
         # Build new constructors
         $contextConstructor = @"

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// Context provides: Battle, TargetPokemon, SourcePokemon, Move, SourceEffect, etc.
    /// </summary>
    public $className(
        EventHandlerDelegate contextHandler
"@
   
          foreach ($param in $optionalParams) {
          $defaultValue = if ($param.HasDefault) { " = null" } else { "" }
                $contextConstructor += ",`n      $($param.Type) $($param.Name)$defaultValue"
    }
     
      $contextConstructor += @"
)
{
"@
    
            if ($eventId) {
     $contextConstructor += "`n        Id = EventId.$eventId;"
   }
            
    $contextConstructor += @"

  ContextHandler = contextHandler;
"@
            
foreach ($param in $optionalParams) {
     $contextConstructor += "`n   $($param.Name) = $($param.Name);"
            }
          
       $contextConstructor += @"

    }
"@
 
  # Build Create method
            $createMethod = @"

    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static $className Create(
        $($handlerParam.Type) handler
"@
            
       foreach ($param in $optionalParams) {
     $defaultValue = if ($param.HasDefault) { " = null" } else { "" }
          $createMethod += ",`n        $($param.Type) $($param.Name)$defaultValue"
}
            
   $createMethod += @"
)
 {
        return new $className(
          context => 
      {
          var result = handler(
"@
  
         for ($i = 0; $i -lt $contextAccessors.Count; $i++) {
   $comma = if ($i -lt $contextAccessors.Count - 1) { "," } else { "" }
  $createMethod += "`n      $($contextAccessors[$i])$comma"
   }
            
 $createMethod += @"

                );
   // TODO: Convert return value to RelayVar if needed
       return null;
            }
"@
          
            foreach ($param in $optionalParams) {
     $createMethod += ",`n            $($param.Name)"
            }
            
   $createMethod += @"

        );
  }
"@
       
            # Insert before the final closing brace
   $lastBrace = $content.LastIndexOf('}')
            $newContent = $content.Substring(0, $lastBrace) + $contextConstructor + $createMethod + "`n}"
    
            # Write back
     Set-Content -Path $filePath -Value $newContent -NoNewline
        Write-Host "  Updated successfully"
        }
    } else {
    Write-Host "  Could not find constructor, skipping"
    }
}

# Find all EventHandlerInfo files
$files = Get-ChildItem -Path $ProjectRoot -Filter "*EventInfo.cs" -Recurse

Write-Host "Found $($files.Count) event handler files"
Write-Host ""

foreach ($file in $files) {
    try {
        Update-EventHandlerFile $file.FullName
    } catch {
        Write-Host "  ERROR: $_" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "Update complete!" -ForegroundColor Green
