param(
    [switch]$DryRun,
    [switch]$Verbose
)

$ErrorActionPreference = 'Stop'
$root = "C:\VSProjects\ApogeeVGC"
$eventMethodsDir = "$root\ApogeeVGC\Sim\Events\Handlers\EventMethods"

# Find already-migrated files (have 'public static')
$migrated = Select-String -Path "$eventMethodsDir\*.cs" -Pattern "public static" |
    ForEach-Object { $_.Filename } | Sort-Object -Unique

# Get unmigrated files
$allFiles = Get-ChildItem "$eventMethodsDir\*.cs"
$unmigrated = $allFiles | Where-Object { $_.Name -notin $migrated }

Write-Host "Total files: $($allFiles.Count)"
Write-Host "Already migrated: $($migrated.Count)"
Write-Host "Need migration: $($unmigrated.Count)"

# ============================================================
# PARAMETER MAPPING RULES
# ============================================================
# Based on extensive analysis of already-migrated classes.
# Maps (delegate signature + event name) -> parameter extraction from EventContext.
#
# Key rules:
# - Position 0: Battle -> context.Battle
# - int/int? relay var -> context.GetRelayVar<IntRelayVar>().Value or TryGetRelayVar
# - SparseBoostsTable -> context.GetRelayVar<SparseBoostsTableRelayVar>().Table
# - PokemonType[] -> context.GetRelayVar<TypesRelayVar>() (need special handling)
# - SecondaryEffect[] -> context.GetRelayVar<SecondaryEffectArrayRelayVar>().Effects
# - PokemonTypeConditionIdUnion -> complex relay var extraction
# - Pokemon: target vs source determined by event semantics
# - ActiveMove/Move -> context.GetMove() or context.Move (if nullable)
# - IEffect -> context.GetSourceEffect<IEffect>()
# - Condition -> context.GetSourceEffect<Condition>()
# - Item -> (Item)context.Effect!
# - Ability -> (Ability)context.Effect! (not SourceEffect)
# - Side -> context.GetTargetSide()
# - PokemonType -> context.SourceType!.Value
# - object? -> context.SourcePokemon
# ============================================================

# Events where in the (Battle, int, Pokemon, Pokemon, ActiveMove) pattern,
# the first Pokemon is SOURCE (not target)
# Offensive stats, base power, damage modifiers, stab, crit ratio, priority, weather
$sourceFirstModifierEvents = @(
    'BasePower', 'ModifyAtk', 'ModifySpA', 'ModifyDamage',
    'ModifyDamagePhase1', 'ModifyDamagePhase2', 'ModifyStab',
    'ModifyCritRatio', 'ModifyPriority', 'WeatherModifyDamage'
)

# Events where in the (Battle, int, Pokemon, Pokemon, ActiveMove) pattern,
# the first Pokemon is TARGET
# Defensive stats, damage (raw), accuracy
$targetFirstModifierEvents = @(
    'ModifyDef', 'ModifySpD', 'Damage', 'DamagingHit',
    'Accuracy', 'ModifyAccuracy'
)

# For ModifyTarget: (Battle, Pokemon relayTarget, Pokemon source, Pokemon target, ActiveMove)
# This is a special case handled separately

function Get-EventIdFromName {
    param([string]$className)
    # Strip prefix (OnSource, OnFoe, OnAny, OnAlly) and suffix (EventInfo)
    $name = $className -replace '^On(Source|Foe|Any|Ally)', ''
    $name = $name -replace 'EventInfo$', ''
    return $name
}

function Get-BaseEventName {
    param([string]$className)
    # Get the event name without prefix
    $name = $className -replace 'EventInfo$', ''
    $name = $name -replace '^On(Source|Foe|Any)', 'On'
    return $name
}

function Get-PokemonOrder {
    param(
        [string]$eventName,
        [string]$sigComment,
        [string]$delegateType
    )
    
    # First, check if the signature comment has parameter names
    if ($sigComment -match '\(.*Pokemon\s+(\w+).*Pokemon[?\s]+(\w+)') {
        $first = $Matches[1].ToLower()
        $second = $Matches[2].ToLower()
        if ($first -eq 'source' -and ($second -eq 'target' -or $second -eq 'pokemon')) {
            return 'source_target'
        }
        if ($first -eq 'target' -and ($second -eq 'source' -or $second -eq 'pokemon')) {
            return 'target_source'
        }
        if ($first -eq 'pokemon' -and $second -eq 'source') {
            return 'target_source'
        }
        if ($first -eq 'pokemon' -and $second -eq 'target') {
            return 'source_target'
        }
        # relayTarget, pokemon, target pattern for ModifyTarget
        if ($first -eq 'relaytarget') {
            return 'relay_source_target'
        }
    }
    
    # For modifier patterns (int relay var + two Pokemons + move)
    if ($delegateType -match 'Battle,\s*int[?,]*\s*Pokemon') {
        foreach ($evt in $sourceFirstModifierEvents) {
            if ($eventName -match $evt) { return 'source_target' }
        }
        foreach ($evt in $targetFirstModifierEvents) {
            if ($eventName -match $evt) { return 'target_source' }
        }
    }
    
    # Default: target first, source second
    return 'target_source'
}

function Get-ParameterExtraction {
    param(
        [string]$paramType,
        [int]$position,
        [string]$pokemonRole,  # 'target', 'source', 'relay_target'
        [bool]$isNullable
    )
    
    switch -Regex ($paramType) {
        '^Battle$' { return 'context.Battle' }
        '^int\?$' { return 'context.TryGetRelayVar<IntRelayVar>()?.Value' }
        '^int$' {
            if ($position -eq 1) { return 'context.GetRelayVar<IntRelayVar>().Value' }
            return 'context.GetRelayVar<IntRelayVar>().Value'
        }
        '^decimal$' { return 'context.GetRelayVar<DecimalRelayVar>().Value' }
        '^SparseBoostsTable$' { return 'context.GetRelayVar<SparseBoostsTableRelayVar>().Table' }
        '^PokemonType\[\]$' { return 'context.GetRelayVar<TypesRelayVar>().Types.ToArray()' }
        '^SecondaryEffect\[\]$' { return 'context.GetRelayVar<SecondaryEffectArrayRelayVar>().Effects' }
        '^PokemonTypeConditionIdUnion$' { return 'COMPLEX_RELAY_VAR' }
        '^Pokemon\??$' {
            if ($pokemonRole -eq 'source') {
                if ($isNullable) { return 'context.SourcePokemon' }
                return 'context.GetSourcePokemon()'
            }
            if ($pokemonRole -eq 'relay_target') {
                # For ModifyTarget: the first Pokemon is the relay target (redirect target)
                if ($isNullable) { return 'context.TargetPokemon' }
                return 'context.GetTargetPokemon()'
            }
            # Default to target
            if ($isNullable) { return 'context.TargetPokemon' }
            return 'context.GetTargetPokemon()'
        }
        '^ActiveMove\??$' {
            if ($isNullable) { return 'context.Move' }
            return 'context.GetMove()'
        }
        '^Move\??$' {
            if ($isNullable) { return 'context.Move' }
            return 'context.GetMove()'
        }
        '^IEffect\??$' {
            if ($isNullable) { return 'context.TryGetSourceEffect<IEffect>()' }
            return 'context.GetSourceEffect<IEffect>()'
        }
        '^Condition$' { return 'context.GetSourceEffect<Condition>()' }
        '^Item$' { return '(Item)context.Effect!' }
        '^Ability$' { return '(Ability)context.Effect!' }
        '^Side$' { return 'context.GetTargetSide()' }
        '^PokemonType$' { return 'context.SourceType!.Value' }
        '^object\??$' { return 'context.SourcePokemon' }
        'List<SecondaryEffect>' { return 'new List<SecondaryEffect>(context.GetRelayVar<SecondaryEffectArrayRelayVar>().Effects)' }
        default { return "/* UNKNOWN: $paramType */" }
    }
}

function Get-ReturnConversion {
    param([string]$returnType)
    
    switch ($returnType) {
        'void' { return 'action' }
        'DoubleVoidUnion' { return @"
                var result = CALL;
                return result switch
                {
                    DoubleDoubleVoidUnion d => new DecimalRelayVar((decimal)d.Value),
                    VoidDoubleVoidUnion => null,
                    _ => null
                };
"@ }
        'IntVoidUnion' { return @"
                var result = CALL;
                return result switch
                {
                    IntIntVoidUnion i => new IntRelayVar(i.Value),
                    VoidIntVoidUnion => null,
                    _ => null
                };
"@ }
        'BoolVoidUnion' { return @"
                var result = CALL;
                return result switch
                {
                    BoolBoolVoidUnion b => new BoolRelayVar(b.Value),
                    VoidBoolVoidUnion => null,
                    _ => null
                };
"@ }
        'BoolVoidUnion?' { return @"
                var result = CALL;
                if (result == null) return null;
                return result switch
                {
                    BoolBoolVoidUnion b => new BoolRelayVar(b.Value),
                    VoidBoolVoidUnion => null,
                    _ => null
                };
"@ }
        'BoolEmptyVoidUnion?' { return @"
                var result = CALL;
                if (result == null) return null;
                return result switch
                {
                    BoolBoolEmptyVoidUnion b => new BoolRelayVar(b.Value),
                    EmptyBoolEmptyVoidUnion => new BoolRelayVar(false),
                    VoidUnionBoolEmptyVoidUnion => null,
                    _ => null
                };
"@ }
        'BoolIntEmptyVoidUnion?' { return @"
                var result = CALL;
                if (result == null) return null;
                return result switch
                {
                    BoolBoolIntEmptyVoidUnion b => new BoolRelayVar(b.Value),
                    IntBoolIntEmptyVoidUnion i => new IntRelayVar(i.Value),
                    EmptyBoolIntEmptyVoidUnion => new BoolRelayVar(false),
                    VoidUnionBoolIntEmptyVoidUnion => null,
                    _ => null
                };
"@ }
        'IntBoolVoidUnion?' { return @"
                var result = CALL;
                if (result == null) return null;
                return result switch
                {
                    IntIntBoolVoidUnion i => new IntRelayVar(i.Value),
                    BoolIntBoolVoidUnion b => new BoolRelayVar(b.Value),
                    VoidIntBoolVoidUnion => null,
                    _ => null
                };
"@ }
        'DelegateVoidUnion' { return @"
                var result = CALL;
                return result switch
                {
                    DelegateDelegateVoidUnion d => throw new NotImplementedException("DelegateVoidUnion delegate return not supported in Create pattern"),
                    VoidDelegateVoidUnion => null,
                    _ => null
                };
"@ }
        'PokemonVoidUnion' { return @"
                var result = CALL;
                return result switch
                {
                    PokemonPokemonVoidUnion p => new PokemonRelayVar(p.Pokemon),
                    VoidPokemonVoidUnion => null,
                    _ => null
                };
"@ }
        'SparseBoostsTableVoidUnion' { return @"
                var result = CALL;
                return result switch
                {
                    SparseBoostsTableSparseBoostsTableVoidUnion s => new SparseBoostsTableRelayVar(s.Table),
                    VoidSparseBoostsTableVoidUnion => null,
                    _ => null
                };
"@ }
        'TypesVoidUnion' { return @"
                var result = CALL;
                return result switch
                {
                    TypesTypesVoidUnion t => new TypesRelayVar(t.Types),
                    VoidTypesVoidUnion => null,
                    _ => null
                };
"@ }
        'int' { return @"
                var result = CALL;
                return new IntRelayVar(result);
"@ }
        'bool?' { return @"
                var result = CALL;
                if (result == null) return null;
                return new BoolRelayVar(result.Value);
"@ }
        'VoidReturn?' { return @"
                CALL;
                return null;
"@ }
        'SecondaryEffect[]' { return @"
                var result = CALL;
                return new SecondaryEffectArrayRelayVar(result);
"@ }
        'BoolEmptyVoidUnion' { return @"
                var result = CALL;
                return result switch
                {
                    BoolBoolEmptyVoidUnion b => new BoolRelayVar(b.Value),
                    EmptyBoolEmptyVoidUnion => new BoolRelayVar(false),
                    VoidUnionBoolEmptyVoidUnion => null,
                    _ => null
                };
"@ }
        'BoolIntEmptyVoidUnion' { return @"
                var result = CALL;
                return result switch
                {
                    BoolBoolIntEmptyVoidUnion b => new BoolRelayVar(b.Value),
                    IntBoolIntEmptyVoidUnion i => new IntRelayVar(i.Value),
                    EmptyBoolIntEmptyVoidUnion => new BoolRelayVar(false),
                    VoidUnionBoolIntEmptyVoidUnion => null,
                    _ => null
                };
"@ }
        'IntBoolVoidUnion' { return @"
                var result = CALL;
                return result switch
                {
                    IntIntBoolVoidUnion i => new IntRelayVar(i.Value),
                    BoolIntBoolVoidUnion b => new BoolRelayVar(b.Value),
                    VoidIntBoolVoidUnion => null,
                    _ => null
                };
"@ }
        'ActiveMove' { return @"
                var result = CALL;
                return result switch
                {
                    null => null,
                    _ => new EffectRelayVar(result)
                };
"@ }
        'double' { return @"
                var result = CALL;
                return new DecimalRelayVar((decimal)result);
"@ }
        'VoidReturn' { return @"
                CALL;
                return null;
"@ }
        'MoveIdVoidUnion' { return @"
                var result = CALL;
                return result switch
                {
                    MoveIdMoveIdVoidUnion m => new MoveIdRelayVar(m.MoveId),
                    VoidMoveIdVoidUnion => null,
                    _ => null
                };
"@ }
        default { return "/* UNKNOWN RETURN: $returnType */" }
    }
}

function Parse-EventHandlerFile {
    param([string]$filePath)
    
    $content = Get-Content $filePath -Raw
    $fileName = Split-Path $filePath -Leaf
    
    $result = @{
        FilePath = $filePath
        FileName = $fileName
        Content = $content
    }
    
    # Extract class name
    if ($content -match 'public sealed record (\w+)\s*:\s*(UnionEventHandlerInfo<(\w+)>|EventHandlerInfo)') {
        $result.ClassName = $Matches[1]
        $result.IsUnion = $null -ne $Matches[3]
        $result.UnionType = $Matches[3]
    } else {
        Write-Warning "Could not parse class name from $fileName"
        return $null
    }
    
    # Extract EventId
    if ($content -match 'Id\s*=\s*EventId\.(\w+)') {
        $result.EventId = $Matches[1]
    }
    
    # Extract Prefix
    if ($content -match 'Prefix\s*=\s*EventPrefix\.(\w+)') {
        $result.Prefix = $Matches[1]
    }
    
    # Extract signature comment
    if ($content -match '///\s*Signature:\s*(.+)') {
        $result.SigComment = $Matches[1].Trim()
    } else {
        $result.SigComment = ''
    }
    
    # Determine if it's a union handler (takes union value, not handler)
    if ($result.IsUnion) {
        # Union handlers have: public ClassName(OnXxx unionValue, ...)
        if ($content -match "public\s+$([regex]::Escape($result.ClassName))\(\s*$([regex]::Escape($result.UnionType))\s+unionValue") {
            $result.IsUnionConstructor = $true
            # The delegate type is extracted from ExpectedParameterTypes + ExpectedReturnType
            # For union handlers, we use the same pattern as non-union but extract params differently
        }
    }
    
    # Extract delegate type from constructor
    if ($content -match 'public\s+\w+\(\s*((?:Func|Action)<[^>]+>)\s+handler') {
        $result.DelegateType = $Matches[1]
    } elseif ($content -match 'public\s+\w+\(\s*(\w+Handler)\s+handler') {
        $result.DelegateType = $Matches[1]
    } elseif ($result.IsUnion) {
        # For union types, we need to get the delegate from the ExpectedParameterTypes + ReturnType
        $result.DelegateType = 'UNION'
    }
    
    # Extract constructor parameters beyond handler (priority, order, subOrder, usesSpeed)
    $result.HasPriority = $content -match 'int\?\s+priority\s*='
    $result.HasOrder = $content -match 'int\?\s+order\s*='
    $result.HasSubOrder = $content -match 'int\?\s+subOrder\s*='
    $result.HasUsesSpeed = $content -match 'bool\s+usesSpeed\s*='
    
    # Extract ExpectedParameterTypes - use greedy match up to ];
    if ($content -match 'ExpectedParameterTypes\s*=\s*\[([\s\S]*?)\]\s*;') {
        $typesBlock = $Matches[1]
        $types = [regex]::Matches($typesBlock, 'typeof\(([^)]+)\)') | ForEach-Object { $_.Groups[1].Value }
        $result.ParamTypes = $types
    }
    
    # Extract ExpectedReturnType - handle array types and generics
    if ($content -match 'ExpectedReturnType\s*=\s*typeof\(([^)]+)\)') {
        $result.ReturnType = $Matches[1]
    }
    
    # Extract ParameterNullability
    if ($content -match 'ParameterNullability\s*=\s*\[([^\]]+)\]') {
        $nullBlock = $Matches[1]
        $result.Nullability = $nullBlock -split ',' | ForEach-Object { $_.Trim() -eq 'true' }
    }
    
    return $result
}

function Get-ParamTypeList {
    param([string[]]$types, [bool[]]$nullability)
    $result = @()
    for ($i = 0; $i -lt $types.Count; $i++) {
        $t = $types[$i]
        $nullable = if ($nullability -and $i -lt $nullability.Count) { $nullability[$i] } else { $false }
        # Add ? for nullable reference types
        if ($nullable -and $t -notmatch '\?' -and $t -ne 'int' -and $t -ne 'bool') {
            $result += "${t}?"
        } elseif ($nullable -and $t -eq 'int') {
            $result += "int?"
        } else {
            $result += $t
        }
    }
    return $result
}

function Build-ContextExtraction {
    param(
        [hashtable]$info
    )
    
    $eventName = $info.EventId
    $paramTypes = $info.ParamTypes
    $nullability = $info.Nullability
    $sigComment = $info.SigComment
    $className = $info.ClassName
    
    if (-not $paramTypes -or $paramTypes.Count -eq 0) {
        Write-Warning "No param types for $($info.FileName)"
        return $null
    }
    
    # Determine Pokemon ordering
    $delegType = if ($info.DelegateType) { $info.DelegateType } else { '' }
    $pokemonOrder = Get-PokemonOrder -eventName $eventName -sigComment $sigComment -delegateType $delegType
    
    $extractions = @()
    $pokemonCount = 0
    
    for ($i = 0; $i -lt $paramTypes.Count; $i++) {
        $pType = $paramTypes[$i]
        $isNullable = if ($nullability -and $i -lt $nullability.Count) { $nullability[$i] } else { $false }
        
        # Determine Pokemon role based on ordering
        $role = 'target'
        if ($pType -match '^Pokemon') {
            $pokemonCount++
            if ($pokemonOrder -eq 'source_target') {
                if ($pokemonCount -eq 1) { $role = 'source' }
                elseif ($pokemonCount -eq 2) { $role = 'target' }
                elseif ($pokemonCount -eq 3) { $role = 'source' }  # 3rd Pokemon case
            } elseif ($pokemonOrder -eq 'target_source') {
                if ($pokemonCount -eq 1) { $role = 'target' }
                elseif ($pokemonCount -eq 2) { $role = 'source' }
                elseif ($pokemonCount -eq 3) { $role = 'target' }
            } elseif ($pokemonOrder -eq 'relay_source_target') {
                # ModifyTarget pattern: relayTarget, source, target
                if ($pokemonCount -eq 1) { $role = 'relay_target' }
                elseif ($pokemonCount -eq 2) { $role = 'source' }
                elseif ($pokemonCount -eq 3) { $role = 'target' }
            }
        }
        
        $nullable_suffix = ''
        if ($isNullable -and $pType -match '^(Pokemon|ActiveMove|Move|IEffect)$') {
            $nullable_suffix = '?'
        } elseif ($isNullable -and $pType -eq 'int') {
            $pType = 'int?'
        }
        
        $extraction = Get-ParameterExtraction -paramType "$pType$nullable_suffix" -position $i -pokemonRole $role -isNullable $isNullable
        $extractions += $extraction
    }
    
    return $extractions
}

function Build-ConstructorParams {
    param([hashtable]$info)
    
    $params = @()
    $passThrough = @()
    
    if ($info.HasPriority) {
        $params += "int? priority = null"
        $passThrough += "priority"
    }
    if ($info.HasOrder) {
        $params += "int? order = null"
        $passThrough += "order"
    }
    if ($info.HasSubOrder) {
        $params += "int? subOrder = null"
        $passThrough += "subOrder"
    }
    if ($info.HasUsesSpeed) {
        $params += "bool usesSpeed = true"
        $passThrough += "usesSpeed"
    }
    
    return @{
        Declarations = $params
        PassThrough = $passThrough
    }
}

function Build-ContextConstructor {
    param([hashtable]$info)
    
    $ctorParams = Build-ConstructorParams -info $info
    $paramDecls = @("EventHandlerDelegate contextHandler") + $ctorParams.Declarations
    $paramStr = "`n        " + ($paramDecls -join ",`n        ")
    
    $bodyLines = @()
    $bodyLines += "        Id = EventId.$($info.EventId);"
    if ($info.Prefix) {
        $bodyLines += "        Prefix = EventPrefix.$($info.Prefix);"
    }
    $bodyLines += "        ContextHandler = contextHandler;"
    
    foreach ($pt in $ctorParams.PassThrough) {
        $capitalPt = $pt.Substring(0,1).ToUpper() + $pt.Substring(1)
        $bodyLines += "        $capitalPt = $pt;"
    }
    
    $body = $bodyLines -join "`n"
    
    return @"

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public $($info.ClassName)($paramStr)
    {
$body
    }
"@
}

function Build-CreateFactory {
    param([hashtable]$info)
    
    $extractions = Build-ContextExtraction -info $info
    if (-not $extractions) { return $null }
    
    $ctorParams = Build-ConstructorParams -info $info
    $returnType = $info.ReturnType
    if (-not $returnType) { $returnType = 'void' }
    
    # Build parameter list for the Create method
    $paramTypes = $info.ParamTypes
    $nullability = $info.Nullability
    
    # Build the delegate type for Create
    $isAction = $returnType -eq 'void'
    
    # Get parameter types with nullability
    $typedParams = Get-ParamTypeList -types $paramTypes -nullability $nullability
    
    if ($isAction) {
        $delegateStr = "Action<" + ($typedParams -join ", ") + ">"
    } else {
        # Keep the same delegate type as the legacy constructor
        $delegateStr = $info.DelegateType
        if (-not $delegateStr -or $delegateStr -eq 'UNION') {
            # Build from param types + return type
            $delegateStr = "Func<" + ($typedParams -join ", ") + ", $returnType>"
            # Handle nullable return
            if ($returnType -match '(BoolVoidUnion|BoolEmptyVoidUnion|BoolIntEmptyVoidUnion|IntBoolVoidUnion)\??$') {
                if ($info.Content -match 'ReturnTypeNullable\s*=\s*true' -or $info.DelegateType -match '\?>' ) {
                    $delegateStr = $delegateStr -replace '>$', '?>'
                }
            }
        }
    }
    
    $createParams = @($delegateStr + " handler") + $ctorParams.Declarations
    $createParamStr = "`n        " + ($createParams -join ",`n        ")
    
    $passArgs = $ctorParams.PassThrough -join ",`n            "
    
    # Build the handler call with parameter extractions
    $extractionStr = $extractions -join ",`n                "
    
    if ($isAction) {
        # Action: wrap in lambda returning null
        $lambdaBody = @"
            context =>
            {
                handler(
                    $extractionStr
                );
                return null;
            }
"@
    } else {
        # Func: need to convert return value
        $conversion = Get-ReturnConversion -returnType $returnType
        
        if ($conversion -eq 'action') {
            # Shouldn't happen for Func, but fallback
            $lambdaBody = @"
            context =>
            {
                handler(
                    $extractionStr
                );
                return null;
            }
"@
        } else {
            $handlerCall = "handler(`n                    $extractionStr`n                )"
            $convBody = $conversion -replace 'CALL', $handlerCall
            $lambdaBody = @"
            context =>
            {
$convBody
            }
"@
        }
    }
    
    # Build the full Create method
    $newCtorArgs = @($lambdaBody.TrimEnd())
    if ($ctorParams.PassThrough.Count -gt 0) {
        $newCtorArgs += $ctorParams.PassThrough
    }
    $newCtorArgsStr = $newCtorArgs -join ",`n            "
    
    return @"

    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static $($info.ClassName) Create($createParamStr)
    {
        return new $($info.ClassName)(
            $newCtorArgsStr
        );
    }
"@
}

# ============================================================
# MAIN MIGRATION LOOP
# ============================================================
$successCount = 0
$failCount = 0
$failedFiles = @()

foreach ($file in $unmigrated) {
    $info = Parse-EventHandlerFile -filePath $file.FullName
    
    if (-not $info) {
        Write-Warning "SKIP: Could not parse $($file.Name)"
        $failCount++
        $failedFiles += $file.Name
        continue
    }
    
    if ($Verbose) {
        Write-Host "Processing: $($info.ClassName) [EventId=$($info.EventId), Prefix=$($info.Prefix), Delegate=$($info.DelegateType), Return=$($info.ReturnType)]"
    }
    
    # Build context constructor
    $contextCtor = Build-ContextConstructor -info $info
    
    # Build Create factory
    $createFactory = Build-CreateFactory -info $info
    
    if (-not $createFactory) {
        Write-Warning "SKIP: Could not build Create factory for $($file.Name)"
        $failCount++
        $failedFiles += $file.Name
        continue
    }
    
    # Find insertion point: before the last closing brace of the class
    $content = $info.Content
    
    # Find the last } in the file (class closing brace)
    $lastBrace = $content.LastIndexOf('}')
    if ($lastBrace -lt 0) {
        Write-Warning "SKIP: Could not find closing brace in $($file.Name)"
        $failCount++
        $failedFiles += $file.Name
        continue
    }
    
    # Insert before the last closing brace
    $newContent = $content.Substring(0, $lastBrace) + $contextCtor + $createFactory + "`n" + $content.Substring($lastBrace)
    
    if ($DryRun) {
        Write-Host "DRY RUN: Would modify $($file.Name)"
        if ($Verbose) {
            Write-Host "--- Context Constructor ---"
            Write-Host $contextCtor
            Write-Host "--- Create Factory ---"
            Write-Host $createFactory
            Write-Host "---"
        }
    } else {
        Set-Content -Path $file.FullName -Value $newContent -NoNewline
    }
    
    $successCount++
}

Write-Host "`n=== MIGRATION COMPLETE ==="
Write-Host "Successfully processed: $successCount"
Write-Host "Failed/Skipped: $failCount"
if ($failedFiles.Count -gt 0) {
    Write-Host "Failed files:"
    $failedFiles | ForEach-Object { Write-Host "  - $_" }
}
