# Bulk Update Strategy for All Event Handler Classes

## Problem
~300 event handler classes need updating with context support to avoid mixing old/new patterns.

## Solution: Visual Studio Find & Replace with Regex

### Step 1: Backup Your Code
```bash
git add -A
git commit -m "Before event handler mass update"
```

### Step 2: Use Multi-File Find & Replace

Open Visual Studio ? Edit ? Find and Replace ? Replace in Files

**Scope:** `ApogeeVGC\Sim\Events\Handlers\**\*.cs`

### Find & Replace Pattern 1: Add Context Constructor

**Find (Regex):**
```regex
(public sealed record (\w+EventInfo) : EventHandlerInfo\s*\{[^}]+?public \2\([^)]+\)\s*\{[^}]+?\})
```

**Replace:**
```
$1

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// Context provides: Battle, TargetPokemon, SourcePokemon, Move, SourceEffect, etc.
    /// </summary>
  public $2(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.PLACEHOLDER;
  ContextHandler = contextHandler;
        Priority = priority;
  UsesSpeed = usesSpeed;
    }
```

Then manually fix:
- EventId (replace PLACEHOLDER)
- Optional parameters (match existing constructor)

### Step 3: Use Snippet for Create Method

Create a Visual Studio Code Snippet:

**File:** `EventHandlerCreate.snippet`
```xml
<?xml version="1.0" encoding="utf-8"?>
<CodeSnippets xmlns="http://schemas.microsoft.com/VisualStudio/2005/CodeSnippet">
  <CodeSnippet Format="1.0.0">
    <Header>
      <Title>EventHandler Create Method</Title>
      <Shortcut>ehcreate</Shortcut>
    </Header>
    <Snippet>
      <Code Language="CSharp">
        <![CDATA[
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static $ClassName$EventInfo Create(
   $HandlerType$ handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new $ClassName$EventInfo(
context => handler(
    $ContextAccessors$
            ),
            priority,
        usesSpeed
        );
    }
        ]]>
      </Code>
    </Snippet>
  </CodeSnippet>
</CodeSnippets>
```

## Alternative: PowerShell Batch Script

Run this in PowerShell from the solution root:

```powershell
# Get all event handler files
$files = Get-ChildItem "ApogeeVGC\Sim\Events\Handlers" -Filter "*EventInfo.cs" -Recurse

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    
    # Skip if already updated
    if ($content -match 'EventHandlerDelegate contextHandler') {
        continue
    }
    
    # Extract class name
    if ($content -match 'public sealed record (\w+EventInfo)') {
        $className = $Matches[1]
     
        # Add template at end (before final brace)
        $template = @"

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public $className(
        EventHandlerDelegate contextHandler,
        int? priority = null)
    {
   Id = EventId.TODO;
    ContextHandler = contextHandler;
 Priority = priority;
    }
    
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static $className Create(
    // TODO: Add parameters
   int? priority = null)
  {
      return new $className(
   context => {
           // TODO: Implement
     return null;
    },
   priority
        );
    }
"@
        
        $lastBrace = $content.LastIndexOf('}')
   $newContent = $content.Substring(0, $lastBrace) + $template + "`n}"
        
        Set-Content $file.FullName $newContent -NoNewline
  
Write-Host "Updated: $($file.Name)"
    }
}
```

Then manually fill in the TODOs.

## Recommended Approach: Systematic Manual Updates

### Phase 1: Update by Category

Update one file from each category as a reference, then copy the pattern:

1. **EventMethods** (largest category)
   - Do: OnBeforeMoveEventInfo ?
   - Template others from this

2. **PokemonEventMethods**
   - Do: OnAllyModifyAtkEventInfo
   - Template others from this

3. **FieldEventMethods**
 - Do: OnFieldStartEventInfo
   - Template others from this

4. **SideEventMethods**
   - Do: OnSideStartEventInfo
   - Template others from this

5. **AbilityEventMethods**
   - Do: OnStartEventInfo ?
   - Template others from this

6. **ItemSpecific**
   - Do: OnEatEventInfo
   - Template others from this

7. **MoveEventMethods**
   - Do: OnBasePowerEventInfo
   - Template others from this

8. **ConditionSpecific**
   - Do: OnStartEventInfo ?
   - Template others from this

### Phase 2: Copy-Paste Pattern

For each category:
1. Open reference file (already updated)
2. Copy context constructor + Create method
3. Open target file
4. Paste before final `}`
5. Adjust:
   - Class name (2-3 places)
   - EventId
   - Optional parameters
   - Context accessors in Create
6. Save
7. Repeat

**Time estimate:** ~2 minutes per file = ~10 hours total

### Phase 3: Validate with Build

```powershell
dotnet build
# Fix any compilation errors
```

## Fastest Approach: Use AI to Generate

Since you have GitHub Copilot:

1. Open any event handler file
2. Add comment above class:
```csharp
// TODO: Add context constructor and Create method following the pattern in OnBeforeMoveEventInfo
```

3. Hit Enter, let Copilot generate
4. Review and accept
5. Repeat for each file

**Time estimate:** ~30 seconds per file = ~2.5 hours total with Copilot

## My Recommendation

**Hybrid approach:**

1. I'll update 10-15 representative files (one from each pattern category)
2. You use those as templates with Copilot for the rest
3. Should take ~3-4 hours total with AI assistance

Want me to proceed with updating the representative files from each category?
