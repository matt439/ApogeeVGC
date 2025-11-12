#!/usr/bin/env pwsh
# Script to generate Pokemon (Ally-prefixed) EventHandlerInfo records

$OutputDirectory = "ApogeeVGC\Sim\Events\Handlers\PokemonEventMethods"

# All Ally-prefixed events from IPokemonEventMethods
# Many reuse handler types from EventMethods and MoveEventMethods
$AllyEvents = @{
    # Events that already have corresponding EventMethodsInfo (reuse signatures)
    "OnAllyDamagingHit" = @{
        Delegate = "Action<Battle, int, Pokemon, Pokemon, ActiveMove>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "DamagingHit"
        Prefix = "Ally"
        Description = "Triggered when an ally deals damaging hit"
    }
    "OnAllyAfterEachBoost" = @{
        Delegate = "Action<Battle, SparseBoostsTable, Pokemon, Pokemon>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(SparseBoostsTable)", "typeof(Pokemon)", "typeof(Pokemon)")
   EventId = "AfterEachBoost"
    Prefix = "Ally"
        Description = "Triggered after each boost to an ally"
    }
    "OnAllyAfterHit" = @{
      Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion>"
  Return = "BoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
   EventId = "AfterHit"
   Prefix = "Ally"
        Description = "Triggered after ally hits"
    }
    "OnAllyAfterSetStatus" = @{
        Delegate = "Action<Battle, Condition, Pokemon, Pokemon, IEffect>"
        Return = "void"
    Params = @("typeof(Battle)", "typeof(Condition)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)")
        EventId = "AfterSetStatus"
        Prefix = "Ally"
      Description = "Triggered after status is set on ally"
    }
    "OnAllyAfterSubDamage" = @{
        Delegate = "Action<Battle, int, Pokemon, Pokemon, ActiveMove>"
      Return = "void"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "AfterSubDamage"
        Prefix = "Ally"
Description = "Triggered after substitute damage to ally"
    }
    "OnAllyAfterSwitchInSelf" = @{
   Delegate = "Action<Battle, Pokemon>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(Pokemon)")
        EventId = "AfterSwitchInSelf"
    Prefix = "Ally"
        Description = "Triggered after ally switches in"
    }
    "OnAllyAfterUseItem" = @{
        Delegate = "Action<Battle, Item, Pokemon>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(Item)", "typeof(Pokemon)")
        EventId = "AfterUseItem"
        Prefix = "Ally"
     Description = "Triggered after ally uses item"
    }
    "OnAllyAfterBoost" = @{
     Delegate = "Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>"
    Return = "void"
        Params = @("typeof(Battle)", "typeof(SparseBoostsTable)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)")
        EventId = "AfterBoost"
 Prefix = "Ally"
     Description = "Triggered after ally is boosted"
    }
  "OnAllyAfterFaint" = @{
        Delegate = "Action<Battle, int, Pokemon, Pokemon, IEffect>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)")
        EventId = "AfterFaint"
     Prefix = "Ally"
        Description = "Triggered after ally faints"
    }
    "OnAllyAfterMoveSecondarySelf" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion>"
        Return = "BoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "AfterMoveSecondarySelf"
      Prefix = "Ally"
        Description = "Triggered after ally's move secondary effects on self"
    }
    "OnAllyAfterMoveSecondary" = @{
     Delegate = "Action<Battle, Pokemon, Pokemon, ActiveMove>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
    EventId = "AfterMoveSecondary"
        Prefix = "Ally"
        Description = "Triggered after ally's move secondary effects"
    }
    "OnAllyAfterMove" = @{
   Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion>"
        Return = "BoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
     EventId = "AfterMove"
 Prefix = "Ally"
        Description = "Triggered after ally's move"
 }
    "OnAllyAfterMoveSelf" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion>"
        Return = "BoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
    EventId = "AfterMoveSelf"
     Prefix = "Ally"
    Description = "Triggered after ally's move (self)"
 }
    "OnAllyAttract" = @{
        Delegate = "Action<Battle, Pokemon, Pokemon>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)")
   EventId = "Attract"
        Prefix = "Ally"
        Description = "Triggered when ally is attracted"
    }
    "OnAllyAccuracy" = @{
        Delegate = "Func<Battle, int, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>"
   Return = "IntBoolVoidUnion"
  Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "Accuracy"
        Prefix = "Ally"
   Description = "Triggered to modify ally's accuracy"
    }
    "OnAllyBasePower" = @{
   Delegate = "Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"
     Return = "DoubleVoidUnion"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
   EventId = "BasePower"
  Prefix = "Ally"
     Description = "Triggered to modify ally's base power"
    }
    "OnAllyBeforeFaint" = @{
        Delegate = "Action<Battle, Pokemon, IEffect>"
      Return = "void"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(IEffect)")
        EventId = "BeforeFaint"
   Prefix = "Ally"
     Description = "Triggered before ally faints"
    }
    "OnAllyBeforeMove" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion>"
        Return = "BoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "BeforeMove"
        Prefix = "Ally"
        Description = "Triggered before ally moves"
    }
"OnAllyBeforeSwitchIn" = @{
        Delegate = "Action<Battle, Pokemon>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(Pokemon)")
        EventId = "BeforeSwitchIn"
        Prefix = "Ally"
        Description = "Triggered before ally switches in"
    }
    "OnAllyBeforeSwitchOut" = @{
 Delegate = "Action<Battle, Pokemon>"
   Return = "void"
        Params = @("typeof(Battle)", "typeof(Pokemon)")
        EventId = "BeforeSwitchOut"
     Prefix = "Ally"
        Description = "Triggered before ally switches out"
    }
    "OnAllyTryBoost" = @{
        Delegate = "Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(SparseBoostsTable)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)")
        EventId = "TryBoost"
        Prefix = "Ally"
        Description = "Triggered when trying to boost ally"
    }
  "OnAllyChargeMove" = @{
  Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion>"
        Return = "BoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "ChargeMove"
        Prefix = "Ally"
        Description = "Triggered when ally charges move"
    }
    "OnAllyCriticalHit" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion>"
        Return = "BoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "CriticalHit"
        Prefix = "Ally"
        Description = "Triggered when ally gets critical hit"
    }
    "OnAllyDamage" = @{
        Delegate = "Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?>"
        Return = "IntBoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)")
   EventId = "Damage"
        Prefix = "Ally"
    Description = "Triggered to modify damage to ally"
    }
    "OnAllyDeductPp" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, IntVoidUnion>"
        Return = "IntVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)")
        EventId = "DeductPp"
     Prefix = "Ally"
        Description = "Triggered when deducting ally's PP"
    }
    "OnAllyDisableMove" = @{
        Delegate = "Action<Battle, Pokemon>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(Pokemon)")
        EventId = "DisableMove"
        Prefix = "Ally"
        Description = "Triggered to disable ally's move"
    }
    "OnAllyDragOut" = @{
        Delegate = "Action<Battle, Pokemon, Pokemon?, ActiveMove?>"
        Return = "void"
      Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
     EventId = "DragOut"
      Prefix = "Ally"
        Description = "Triggered when ally is dragged out"
    }
    "OnAllyEatItem" = @{
        Delegate = "Action<Battle, Item, Pokemon>"
  Return = "void"
        Params = @("typeof(Battle)", "typeof(Item)", "typeof(Pokemon)")
        EventId = "EatItem"
        Prefix = "Ally"
        Description = "Triggered when ally eats item"
    }
  "OnAllyEffectiveness" = @{
        Delegate = "Func<Battle, int, Pokemon?, PokemonType, ActiveMove, IntVoidUnion>"
 Return = "IntVoidUnion"
      Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(PokemonType)", "typeof(ActiveMove)")
    EventId = "Effectiveness"
Prefix = "Ally"
        Description = "Triggered to modify type effectiveness against ally"
    }
    "OnAllyFaint" = @{
        Delegate = "Action<Battle, Pokemon, Pokemon, IEffect>"
        Return = "void"
Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)")
        EventId = "Faint"
        Prefix = "Ally"
  Description = "Triggered when ally faints"
    }
    "OnAllyFlinch" = @{
 Delegate = "Func<Battle, Pokemon, BoolVoidUnion>"
     Return = "BoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)")
      EventId = "Flinch"
   Prefix = "Ally"
        Description = "Triggered when ally flinches"
    }
    "OnAllyHit" = @{
      Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?>"
        Return = "BoolEmptyVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "Hit"
  Prefix = "Ally"
      Description = "Triggered when ally is hit"
    }
    "OnAllyImmunity" = @{
        Delegate = "Action<Battle, PokemonType, Pokemon>"
        Return = "void"
     Params = @("typeof(Battle)", "typeof(PokemonType)", "typeof(Pokemon)")
        EventId = "Immunity"
        Prefix = "Ally"
        Description = "Triggered for ally immunity check"
    }
    "OnAllyLockMove" = @{
 Delegate = "Func<Battle, Pokemon, ActiveMove?>"
        Return = "ActiveMove"
        Params = @("typeof(Battle)", "typeof(Pokemon)")
        EventId = "LockMove"
    Prefix = "Ally"
      Description = "Triggered to lock ally's move"
    }
    "OnAllyMaybeTrapPokemon" = @{
        Delegate = "Action<Battle, Pokemon>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(Pokemon)")
        EventId = "MaybeTrapPokemon"
    Prefix = "Ally"
        Description = "Triggered to maybe trap ally"
 }
    "OnAllyModifyAccuracy" = @{
        Delegate = "Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"
        Return = "DoubleVoidUnion"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
 EventId = "ModifyAccuracy"
        Prefix = "Ally"
      Description = "Triggered to modify ally's accuracy"
    }
    "OnAllyModifyAtk" = @{
      Delegate = "Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"
 Return = "DoubleVoidUnion"
      Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
   EventId = "ModifyAtk"
     Prefix = "Ally"
        Description = "Triggered to modify ally's attack"
    }
    "OnAllyModifyBoost" = @{
     Delegate = "Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTableVoidUnion>"
    Return = "SparseBoostsTableVoidUnion"
   Params = @("typeof(Battle)", "typeof(SparseBoostsTable)", "typeof(Pokemon)")
        EventId = "ModifyBoost"
        Prefix = "Ally"
        Description = "Triggered to modify ally's boosts"
    }
    "OnAllyModifyCritRatio" = @{
        Delegate = "Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"
    Return = "DoubleVoidUnion"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "ModifyCritRatio"
      Prefix = "Ally"
        Description = "Triggered to modify ally's crit ratio"
    }
    "OnAllyModifyDamage" = @{
        Delegate = "Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"
  Return = "DoubleVoidUnion"
 Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
EventId = "ModifyDamage"
        Prefix = "Ally"
        Description = "Triggered to modify damage to ally"
    }
    "OnAllyModifyDef" = @{
        Delegate = "Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"
        Return = "DoubleVoidUnion"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "ModifyDef"
        Prefix = "Ally"
      Description = "Triggered to modify ally's defense"
    }
    "OnAllyModifyMove" = @{
        Delegate = "Action<Battle, ActiveMove, Pokemon, Pokemon?>"
      Return = "void"
   Params = @("typeof(Battle)", "typeof(ActiveMove)", "typeof(Pokemon)", "typeof(Pokemon)")
        EventId = "ModifyMove"
 Prefix = "Ally"
        Description = "Triggered to modify ally's move"
    }
    "OnAllyModifyPriority" = @{
        Delegate = "Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"
        Return = "DoubleVoidUnion"
   Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "ModifyPriority"
        Prefix = "Ally"
        Description = "Triggered to modify ally's move priority"
    }
 "OnAllyModifySecondaries" = @{
        Delegate = "Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(List<SecondaryEffect>)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "ModifySecondaries"
        Prefix = "Ally"
        Description = "Triggered to modify ally's move secondary effects"
    }
    "OnAllyModifySpA" = @{
 Delegate = "Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"
  Return = "DoubleVoidUnion"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "ModifySpA"
        Prefix = "Ally"
        Description = "Triggered to modify ally's special attack"
    }
 "OnAllyModifySpD" = @{
        Delegate = "Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"
        Return = "DoubleVoidUnion"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "ModifySpD"
  Prefix = "Ally"
        Description = "Triggered to modify ally's special defense"
    }
    "OnAllyModifySpe" = @{
   Delegate = "Func<Battle, int, Pokemon, IntVoidUnion>"
    Return = "IntVoidUnion"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)")
        EventId = "ModifySpe"
        Prefix = "Ally"
        Description = "Triggered to modify ally's speed"
    }
    "OnAllyModifyStab" = @{
        Delegate = "Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"
        Return = "DoubleVoidUnion"
     Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "ModifyStab"
        Prefix = "Ally"
        Description = "Triggered to modify ally's STAB"
    }
    "OnAllyModifyType" = @{
      Delegate = "Action<Battle, ActiveMove, Pokemon, Pokemon>"
        Return = "void"
   Params = @("typeof(Battle)", "typeof(ActiveMove)", "typeof(Pokemon)", "typeof(Pokemon)")
        EventId = "ModifyType"
      Prefix = "Ally"
Description = "Triggered to modify ally's move type"
    }
    "OnAllyModifyTarget" = @{
        Delegate = "Action<Battle, Pokemon, Pokemon, Pokemon, ActiveMove>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "ModifyTarget"
   Prefix = "Ally"
        Description = "Triggered to modify ally's move target"
    }
    "OnAllyModifyWeight" = @{
        Delegate = "Func<Battle, int, Pokemon, IntVoidUnion>"
        Return = "IntVoidUnion"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)")
        EventId = "ModifyWeight"
Prefix = "Ally"
        Description = "Triggered to modify ally's weight"
    }
    "OnAllyMoveAborted" = @{
        Delegate = "Action<Battle, Pokemon, Pokemon, ActiveMove>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
    EventId = "MoveAborted"
  Prefix = "Ally"
   Description = "Triggered when ally's move is aborted"
    }
    "OnAllyNegateImmunity" = @{
        Delegate = "Func<Battle, Pokemon, PokemonType?, BoolVoidUnion>"
Return = "BoolVoidUnion"
 Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(PokemonType)")
        EventId = "NegateImmunity"
        Prefix = "Ally"
        Description = "Triggered to negate ally immunity"
    }
    "OnAllyOverrideAction" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, DelegateVoidUnion>"
        Return = "DelegateVoidUnion"
 Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "OverrideAction"
 Prefix = "Ally"
  Description = "Triggered to override ally action"
    }
    "OnAllyPrepareHit" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?>"
        Return = "BoolEmptyVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "PrepareHit"
        Prefix = "Ally"
        Description = "Triggered to prepare ally hit"
    }
    "OnAllyRedirectTarget" = @{
     Delegate = "Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, PokemonVoidUnion>"
        Return = "PokemonVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)", "typeof(ActiveMove)")
        EventId = "RedirectTarget"
  Prefix = "Ally"
    Description = "Triggered to redirect target from ally"
    }
    "OnAllyResidual" = @{
  Delegate = "Action<Battle, PokemonSideUnion, Pokemon, IEffect>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(PokemonSideUnion)", "typeof(Pokemon)", "typeof(IEffect)")
        EventId = "Residual"
        Prefix = "Ally"
      Description = "Triggered for ally residual effects"
    }
    "OnAllySetAbility" = @{
        Delegate = "Func<Battle, Ability, Pokemon, Pokemon, IEffect, BoolVoidUnion>"
        Return = "BoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(Ability)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)")
        EventId = "SetAbility"
        Prefix = "Ally"
     Description = "Triggered when setting ally ability"
    }
    "OnAllySetStatus" = @{
        Delegate = "Func<Battle, Condition, Pokemon, Pokemon, IEffect, PokemonVoidUnion?>"
        Return = "PokemonVoidUnion"
        Params = @("typeof(Battle)", "typeof(Condition)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)")
   EventId = "SetStatus"
        Prefix = "Ally"
        Description = "Triggered when setting ally status"
    }
    "OnAllySetWeather" = @{
     Delegate = "Func<Battle, Pokemon, Pokemon, Condition, PokemonVoidUnion>"
        Return = "PokemonVoidUnion"
 Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(Condition)")
     EventId = "SetWeather"
        Prefix = "Ally"
        Description = "Triggered when setting weather affecting ally"
    }
    "OnAllyStallMove" = @{
        Delegate = "Func<Battle, Pokemon, PokemonVoidUnion>"
        Return = "PokemonVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)")
        EventId = "StallMove"
        Prefix = "Ally"
        Description = "Triggered for ally stall move"
    }
    "OnAllySwitchOut" = @{
        Delegate = "Action<Battle, Pokemon>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(Pokemon)")
     EventId = "SwitchOut"
        Prefix = "Ally"
        Description = "Triggered when ally switches out"
 }
    "OnAllyTakeItem" = @{
  Delegate = "Func<Battle, Item, Pokemon, Pokemon, BoolVoidUnion>"
        Return = "BoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(Item)", "typeof(Pokemon)", "typeof(Pokemon)")
 EventId = "TakeItem"
        Prefix = "Ally"
        Description = "Triggered when taking ally's item"
    }
    "OnAllyTerrain" = @{
        Delegate = "Action<Battle, Pokemon>"
        Return = "void"
        Params = @("typeof(Battle)", "typeof(Pokemon)")
     EventId = "Terrain"
        Prefix = "Ally"
        Description = "Triggered for terrain affecting ally"
    }
    "OnAllyTrapPokemon" = @{
        Delegate = "Action<Battle, Pokemon>"
        Return = "void"
  Params = @("typeof(Battle)", "typeof(Pokemon)")
 EventId = "TrapPokemon"
    Prefix = "Ally"
        Description = "Triggered when trapping ally"
    }
 "OnAllyTryAddVolatile" = @{
        Delegate = "Func<Battle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?>"
        Return = "BoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(Condition)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)")
    EventId = "TryAddVolatile"
    Prefix = "Ally"
  Description = "Triggered when trying to add volatile to ally"
    }
    "OnAllyTryEatItem" = @{
  Delegate = "Func<Battle, Item, Pokemon, BoolVoidUnion>"
      Return = "BoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(Item)", "typeof(Pokemon)")
        EventId = "TryEatItem"
        Prefix = "Ally"
        Description = "Triggered when ally tries to eat item"
    }
    "OnAllyTryHeal" = @{
        Delegate = "Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?>"
        Return = "IntBoolVoidUnion"
      Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(IEffect)")
        EventId = "TryHeal"
        Prefix = "Ally"
        Description = "Triggered when trying to heal ally"
    }
    "OnAllyTryHit" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolIntEmptyVoidUnion?>"
        Return = "BoolIntEmptyVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "TryHit"
        Prefix = "Ally"
     Description = "Triggered when trying to hit ally"
    }
    "OnAllyTryHitField" = @{
      Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolIntEmptyVoidUnion?>"
        Return = "BoolIntEmptyVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "TryHitField"
  Prefix = "Ally"
        Description = "Triggered when trying to hit field affecting ally"
    }
    "OnAllyTryHitSide" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?>"
        Return = "BoolEmptyVoidUnion"
     Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "TryHitSide"
        Prefix = "Ally"
        Description = "Triggered when trying to hit side with ally"
 }
 "OnAllyInvulnerability" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolIntEmptyVoidUnion?>"
        Return = "BoolIntEmptyVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "Invulnerability"
    Prefix = "Ally"
 Description = "Triggered for ally invulnerability check"
    }
    "OnAllyTryMove" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?>"
        Return = "BoolEmptyVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "TryMove"
        Prefix = "Ally"
        Description = "Triggered when ally tries to move"
    }
    "OnAllyTryPrimaryHit" = @{
        Delegate = "Func<Battle, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>"
   Return = "IntBoolVoidUnion"
        Params = @("typeof(Battle)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
  EventId = "TryPrimaryHit"
 Prefix = "Ally"
 Description = "Triggered when trying primary hit on ally"
    }
    "OnAllyType" = @{
        Delegate = "Func<Battle, PokemonType[], Pokemon, TypesVoidUnion>"
  Return = "TypesVoidUnion"
    Params = @("typeof(Battle)", "typeof(PokemonType[])", "typeof(Pokemon)")
    EventId = "Type"
        Prefix = "Ally"
        Description = "Triggered to get ally's type"
    }
    "OnAllyWeatherModifyDamage" = @{
        Delegate = "Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"
    Return = "DoubleVoidUnion"
        Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "WeatherModifyDamage"
        Prefix = "Ally"
        Description = "Triggered when weather modifies damage to ally"
    }
    "OnAllyModifyDamagePhase1" = @{
    Delegate = "Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"
 Return = "DoubleVoidUnion"
     Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
    EventId = "ModifyDamagePhase1"
        Prefix = "Ally"
        Description = "Triggered to modify damage to ally (phase 1)"
 }
    "OnAllyModifyDamagePhase2" = @{
        Delegate = "Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>"
        Return = "DoubleVoidUnion"
 Params = @("typeof(Battle)", "typeof(int)", "typeof(Pokemon)", "typeof(Pokemon)", "typeof(ActiveMove)")
        EventId = "ModifyDamagePhase2"
        Prefix = "Ally"
        Description = "Triggered to modify damage to ally (phase 2)"
    }
}

function Get-RequiredUsings {
    param($EventDef)
 
    $usings = @(
        "using ApogeeVGC.Sim.BattleClasses;",
        "using ApogeeVGC.Sim.PokemonClasses;"
    )
    
    if ($EventDef.Delegate -match "ActiveMove|Move|SecondaryEffect") { $usings += "using ApogeeVGC.Sim.Moves;" }
    if ($EventDef.Delegate -match "IEffect|Effect") { $usings += "using ApogeeVGC.Sim.Effects;" }
    if ($EventDef.Delegate -match "Item") { $usings += "using ApogeeVGC.Sim.Items;" }
    if ($EventDef.Delegate -match "Condition") { $usings += "using ApogeeVGC.Sim.Conditions;" }
    if ($EventDef.Delegate -match "Ability") { $usings += "using ApogeeVGC.Sim.Abilities;" }
    if ($EventDef.Delegate -match "SparseBoostsTable") { $usings += "using ApogeeVGC.Sim.Stats;" }
    if ($EventDef.Delegate -match "PokemonSideUnion") { $usings += "using ApogeeVGC.Sim.Utils.Unions;" }
    if ($EventDef.Delegate -match "Union" -or $EventDef.Return -match "Union") { $usings += "using ApogeeVGC.Sim.Utils.Unions;" }
    
    return $usings | Sort-Object -Unique
}

function Generate-EventHandlerInfo {
    param($EventName, $EventDef)
    
    $className = "${EventName}EventInfo"
    $usings = Get-RequiredUsings $EventDef
    $paramsStr = $EventDef.Params -join ", "
    
    $content = @"
$($usings -join "`n")

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for $EventName event (pokemon/ally-specific).
/// $($EventDef.Description).
/// Signature: $($EventDef.Delegate)
/// </summary>
public sealed record ${className} : EventHandlerInfo
{
    public ${className}(
    $($EventDef.Delegate) handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.$($EventDef.EventId);
  Prefix = EventPrefix.$($EventDef.Prefix);
  Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [$paramsStr];
        ExpectedReturnType = typeof($($EventDef.Return));
  }
}
"@
    
    return $content
}

Write-Host "Generating Pokemon (Ally) EventHandlerInfo records..." -ForegroundColor Cyan
Write-Host ""

$created = 0
$skipped = 0

foreach ($entry in $AllyEvents.GetEnumerator()) {
    $eventName = $entry.Key
    $eventDef = $entry.Value
    $fileName = "${eventName}EventInfo.cs"
    $filePath = Join-Path $OutputDirectory $fileName
    
    if (Test-Path $filePath) {
        Write-Host "??  Skipping $fileName (already exists)" -ForegroundColor Gray
        $skipped++
  continue
    }
    
    try {
        $content = Generate-EventHandlerInfo $eventName $eventDef
        [System.IO.File]::WriteAllText($filePath, $content)
      Write-Host "? Created $fileName" -ForegroundColor Green
        $created++
    }
    catch {
Write-Host "? Error creating ${fileName}: $_" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "?? Summary:" -ForegroundColor Cyan
Write-Host "   Created: $created" -ForegroundColor Green
Write-Host "   Skipped: $skipped" -ForegroundColor Yellow
Write-Host "   Total:   $($created + $skipped)" -ForegroundColor White
Write-Host ""

if ($created -gt 0) {
    Write-Host "? Generation complete!" -ForegroundColor Green
}
