# DisplayUi Guard Checklist

## Summary

| Metric | Count |
|--------|------:|
| Total call sites | 765 |
| Files affected | 58 |
| `Add()` | 703 |
| `AttrLastMove()` | 50 |
| `AddMove()` | 9 |
| `AddWithPerspective()` | 2 |
| `AddSplit()` | 1 |
| With string interpolation | 43 |

## Why this matters

Each unguarded call allocates **~10 objects / ~300+ bytes** at the call site even though
`Add()` returns early when `!DisplayUi`:

1. `params PartFuncUnion[]` array (~56 bytes)
2. Per argument: `Part` subclass record + `PartPartFuncUnion` wrapper (~56 bytes each)
3. String interpolation args create new `System.String` objects

At ~2,000 dynamic invocations per battle and 50K battles on 16 threads,
this accounts for ~12-18% of per-battle GC pressure.

## Guard pattern

### For standalone calls

```csharp
// BEFORE
Add("-damage", target, target.GetHealth, $"[dmg]{damageAmount}");

// AFTER
if (DisplayUi) // or battle.DisplayUi in Data files
{
    Add("-damage", target, target.GetHealth, $"[dmg]{damageAmount}");
}
```

### For methods that ONLY log

```csharp
// Add early return at method top
private void SomeLoggingMethod(...)
{
    if (!DisplayUi) return;
    // ... existing Add() calls ...
}
```

### Access pattern by file location

| Location | Access `DisplayUi` via |
|----------|------------------------|
| `Sim\BattleClasses\Battle.*.cs` | `DisplayUi` (same class) |
| `Sim\BattleClasses\BattleActions.*.cs` | `Battle.DisplayUi` |
| `Sim\PokemonClasses\*.cs` | `Battle.DisplayUi` |
| `Sim\SideClasses\*.cs` | `Battle.DisplayUi` |
| `Sim\FieldClasses\*.cs` | `Battle.DisplayUi` |
| `Data\Moves\*.cs` | `battle.DisplayUi` (lambda param) |
| `Data\Abilities\*.cs` | `battle.DisplayUi` (lambda param) |
| `Data\Items\*.cs` | `battle.DisplayUi` (lambda param) |
| `Data\Conditions\*.cs` | `battle.DisplayUi` (lambda param) |

### Rules

- Do NOT guard calls with side-effect arguments (mutations in args)
- `target.GetHealth` and `target.GetFullDetails` are pure `Func<>` property accesses - safe to skip
- If an `Add()` is the ONLY statement in an `if` branch, wrap the `Add()` only, not the branch condition
- Do NOT touch calls inside `Battle.Logging.cs` (already optimised)

---

## Call sites by file

### Sim files (22 files, 142 calls)

#### `ApogeeVGC\Sim\BattleClasses\Battle.Combat.cs` (13 calls)

- [ ] L278 `Add`
- [ ] L283 `Add`
- [ ] L288 `Add`
- [ ] L295 `Add`
- [ ] L522 `Add`
- [ ] L531 `Add`
- [ ] L535 `Add`
- [ ] L542 `Add`
- [ ] L547 `Add`
- [ ] L562 `Add`
- [ ] L569 `Add`
- [ ] L620 `Add`
- [ ] L621 `Add`

#### `ApogeeVGC\Sim\BattleClasses\Battle.EndlessBattle.cs` (4 calls, **2 interp**)

- [ ] L23 `Add` `[interp]`
- [ ] L41 `Add` `[interp]`
- [ ] L118 `Add`
- [ ] L129 `Add`

#### `ApogeeVGC\Sim\BattleClasses\Battle.Events.cs` (10 calls, **6 interp**)

- [ ] L33 `Add`
- [ ] L34 `Add` `[interp]`
- [ ] L35 `Add` `[interp]`
- [ ] L46 `Add`
- [ ] L47 `Add` `[interp]`
- [ ] L48 `Add` `[interp]`
- [ ] L188 `Add`
- [ ] L189 `Add`
- [ ] L190 `Add` `[interp]`
- [ ] L191 `Add` `[interp]`

#### `ApogeeVGC\Sim\BattleClasses\Battle.Fainting.cs` (9 calls)

- [ ] L94 `Add`
- [ ] L153 `Add`
- [ ] L235 `Add`
- [ ] L249 `Add`
- [ ] L272 `Add`
- [ ] L295 `Add`
- [ ] L354 `Add`
- [ ] L362 `Add`
- [ ] L367 `Add`

#### `ApogeeVGC\Sim\BattleClasses\Battle.FieldControl.cs` (2 calls)

- [ ] L43 `Add`
- [ ] L47 `Add`

#### `ApogeeVGC\Sim\BattleClasses\Battle.Lifecycle.cs` (13 calls)

- [ ] L47 `Add`
- [ ] L50 `Add`
- [ ] L56 `Add`
- [ ] L221 `Add`
- [ ] L231 `Add`
- [ ] L317 `Add`
- [ ] L342 `Add`
- [ ] L344 `Add`
- [ ] L418 `Add`
- [ ] L424 `Add`
- [ ] L668 `Add`
- [ ] L700 `Add`
- [ ] L716 `Add`

#### `ApogeeVGC\Sim\BattleClasses\Battle.Requests.cs` (3 calls)

- [ ] L41 `Add`
- [ ] L151 `Add`
- [ ] L155 `Add`

#### `ApogeeVGC\Sim\BattleClasses\Battle.Validation.cs` (2 calls)

- [ ] L83 `Add`
- [ ] L157 `Add`

#### `ApogeeVGC\Sim\BattleClasses\BattleActions.Damage.cs` (3 calls)

- [ ] L405 `Add`
- [ ] L418 `Add`
- [ ] L429 `Add`

#### `ApogeeVGC\Sim\BattleClasses\BattleActions.HitSteps.cs` (19 calls, **2 interp**)

- [ ] L71 `AttrLastMove`
- [ ] L77 `Add`
- [ ] L134 `Add`
- [ ] L138 `Add`
- [ ] L142 `AttrLastMove`
- [ ] L219 `Add`
- [ ] L230 `Add`
- [ ] L257 `Add`
- [ ] L315 `Add`
- [ ] L457 `AttrLastMove`
- [ ] L462 `Add`
- [ ] L542 `Add`
- [ ] L546 `Add` `[interp]`
- [ ] L587 `AttrLastMove`
- [ ] L591 `Add` `[interp]`
- [ ] L612 `AddMove`
- [ ] L723 `AddMove`
- [ ] L952 `Add`
- [ ] L1018 `Add`

#### `ApogeeVGC\Sim\BattleClasses\BattleActions.MoveEffects.cs` (8 calls)

- [ ] L52 `Add`
- [ ] L53 `AttrLastMove`
- [ ] L72 `Add`
- [ ] L73 `AttrLastMove`
- [ ] L284 `Add`
- [ ] L285 `AttrLastMove`
- [ ] L446 `Add`
- [ ] L447 `AttrLastMove`

#### `ApogeeVGC\Sim\BattleClasses\BattleActions.MoveHit.cs` (15 calls, **1 interp**)

- [ ] L91 `Add`
- [ ] L92 `AttrLastMove`
- [ ] L156 `AttrLastMove` `[interp]`
- [ ] L213 `Add`
- [ ] L214 `AttrLastMove`
- [ ] L241 `Add`
- [ ] L242 `AttrLastMove`
- [ ] L280 `Add`
- [ ] L281 `AttrLastMove`
- [ ] L297 `Add`
- [ ] L298 `AttrLastMove`
- [ ] L314 `Add`
- [ ] L315 `AttrLastMove`
- [ ] L547 `Add`
- [ ] L548 `AttrLastMove`

#### `ApogeeVGC\Sim\BattleClasses\BattleActions.Moves.cs` (7 calls)

- [ ] L167 `Add`
- [ ] L254 `Add`
- [ ] L444 `AttrLastMove`
- [ ] L445 `Add`
- [ ] L514 `AddMove`
- [ ] L559 `AttrLastMove`
- [ ] L560 `Add`

#### `ApogeeVGC\Sim\BattleClasses\BattleActions.Switch.cs` (2 calls)

- [ ] L180 `AddWithPerspective`
- [ ] L185 `AddWithPerspective`

#### `ApogeeVGC\Sim\BattleClasses\BattleActions.Terastallization.cs` (1 calls)

- [ ] L36 `Add`

#### `ApogeeVGC\Sim\FieldClasses\Field.cs` (2 calls)

- [ ] L97 `Add`
- [ ] L102 `Add`

#### `ApogeeVGC\Sim\PokemonClasses\Pokemon.AbilityItem.cs` (6 calls, **1 interp**)

- [ ] L65 `Add`
- [ ] L161 `Add` `[interp]`
- [ ] L168 `Add`
- [ ] L173 `Add`
- [ ] L443 `Add`
- [ ] L448 `Add`

#### `ApogeeVGC\Sim\PokemonClasses\Pokemon.Immunity.cs` (6 calls)

- [ ] L93 `Add`
- [ ] L98 `Add`
- [ ] L127 `Add`
- [ ] L146 `Add`
- [ ] L178 `Add`
- [ ] L198 `Add`

#### `ApogeeVGC\Sim\PokemonClasses\Pokemon.Status.cs` (6 calls)

- [ ] L60 `Add`
- [ ] L67 `Add`
- [ ] L68 `AttrLastMove`
- [ ] L93 `Add`
- [ ] L234 `Add`
- [ ] L297 `Add`

#### `ApogeeVGC\Sim\PokemonClasses\Pokemon.Transformation.cs` (9 calls)

- [ ] L125 `Add`
- [ ] L130 `Add`
- [ ] L252 `Add`
- [ ] L271 `Add`
- [ ] L275 `Add`
- [ ] L289 `Add`
- [ ] L294 `Add`
- [ ] L302 `Add`
- [ ] L306 `Add`

#### `ApogeeVGC\Sim\PokemonClasses\Pokemon.Types.cs` (1 calls)

- [ ] L210 `Add`

#### `ApogeeVGC\Sim\Utils\Unions\SpreadMoveDamage.cs` (1 calls)

- [ ] L16 `Add`

### Data files (35 files, 606 calls)

#### `ApogeeVGC\Data\Abilities\AbilitiesABC.cs` (23 calls, **1 interp**)

- [ ] L113 `Add`
- [ ] L268 `Add`
- [ ] L338 `AttrLastMove`
- [ ] L339 `Add`
- [ ] L375 `Add`
- [ ] L400 `Add`
- [ ] L401 `Add`
- [ ] L441 `Add`
- [ ] L442 `Add`
- [ ] L563 `Add`
- [ ] L596 `Add`
- [ ] L708 `Add`
- [ ] L756 `Add`
- [ ] L862 `Add`
- [ ] L877 `Add`
- [ ] L911 `Add`
- [ ] L946 `Add`
- [ ] L1100 `Add`
- [ ] L1117 `Add`
- [ ] L1157 `Add`
- [ ] L1158 `Add`
- [ ] L1190 `Add`
- [ ] L1280 `Add` `[interp]`

#### `ApogeeVGC\Data\Abilities\AbilitiesDEF.cs` (17 calls)

- [ ] L43 `AttrLastMove`
- [ ] L49 `Add`
- [ ] L86 `Add`
- [ ] L148 `AttrLastMove`
- [ ] L149 `Add`
- [ ] L312 `Add`
- [ ] L462 `Add`
- [ ] L518 `Add`
- [ ] L703 `Add`
- [ ] L715 `Add`
- [ ] L810 `Add`
- [ ] L997 `Add`
- [ ] L1025 `Add`
- [ ] L1043 `Add`
- [ ] L1166 `Add`
- [ ] L1205 `Add`
- [ ] L1266 `Add`

#### `ApogeeVGC\Data\Abilities\AbilitiesGHI.cs` (25 calls)

- [ ] L103 `Add`
- [ ] L120 `Add`
- [ ] L140 `AddMove`
- [ ] L141 `AttrLastMove`
- [ ] L143 `Add`
- [ ] L238 `Add`
- [ ] L329 `Add`
- [ ] L367 `Add`
- [ ] L383 `Add`
- [ ] L541 `Add`
- [ ] L561 `Add`
- [ ] L608 `Add`
- [ ] L619 `Add`
- [ ] L667 `Add`
- [ ] L707 `Add`
- [ ] L768 `Add`
- [ ] L769 `Add`
- [ ] L785 `Add`
- [ ] L795 `Add`
- [ ] L869 `Add`
- [ ] L885 `Add`
- [ ] L894 `Add`
- [ ] L903 `Add`
- [ ] L923 `Add`
- [ ] L929 `Add`

#### `ApogeeVGC\Data\Abilities\AbilitiesJKL.cs` (9 calls)

- [ ] L57 `Add`
- [ ] L96 `Add`
- [ ] L111 `Add`
- [ ] L157 `Add`
- [ ] L189 `Add`
- [ ] L218 `Add`
- [ ] L240 `Add`
- [ ] L249 `Add`
- [ ] L284 `Add`

#### `ApogeeVGC\Data\Abilities\AbilitiesMNO.cs` (28 calls, **1 interp**)

- [ ] L93 `Add` `[interp]`
- [ ] L131 `Add`
- [ ] L149 `Add`
- [ ] L274 `Add`
- [ ] L284 `Add`
- [ ] L285 `Add`
- [ ] L304 `Add`
- [ ] L384 `Add`
- [ ] L410 `Add`
- [ ] L481 `Add`
- [ ] L571 `Add`
- [ ] L684 `Add`
- [ ] L702 `Add`
- [ ] L748 `Add`
- [ ] L764 `Add`
- [ ] L786 `Add`
- [ ] L811 `Add`
- [ ] L924 `Add`
- [ ] L926 `Add`
- [ ] L931 `Add`
- [ ] L942 `Add`
- [ ] L953 `Add`
- [ ] L1049 `Add`
- [ ] L1053 `Add`
- [ ] L1086 `Add`
- [ ] L1135 `Add`
- [ ] L1152 `Add`
- [ ] L1162 `Add`

#### `ApogeeVGC\Data\Abilities\AbilitiesPQR.cs` (20 calls)

- [ ] L62 `Add`
- [ ] L71 `Add`
- [ ] L86 `Add`
- [ ] L97 `Add`
- [ ] L114 `Add`
- [ ] L136 `Add`
- [ ] L166 `Add`
- [ ] L168 `Add`
- [ ] L197 `Add`
- [ ] L371 `Add`
- [ ] L547 `Add`
- [ ] L587 `Add`
- [ ] L666 `Add`
- [ ] L675 `Add`
- [ ] L739 `Add`
- [ ] L778 `AttrLastMove`
- [ ] L779 `Add`
- [ ] L799 `Add`
- [ ] L962 `Add`
- [ ] L1000 `Add`

#### `ApogeeVGC\Data\Abilities\AbilitiesSTU.cs` (37 calls, **2 interp**)

- [ ] L152 `Add`
- [ ] L265 `Add`
- [ ] L290 `Add`
- [ ] L426 `Add`
- [ ] L547 `Add`
- [ ] L559 `Add`
- [ ] L609 `Add`
- [ ] L620 `Add`
- [ ] L793 `Add`
- [ ] L808 `Add`
- [ ] L1069 `Add`
- [ ] L1089 `Add`
- [ ] L1116 `Add`
- [ ] L1154 `Add`
- [ ] L1165 `Add`
- [ ] L1182 `Add`
- [ ] L1205 `Add`
- [ ] L1210 `Add`
- [ ] L1230 `Add`
- [ ] L1232 `Add` `[interp]`
- [ ] L1241 `Add` `[interp]`
- [ ] L1323 `Add`
- [ ] L1343 `Add`
- [ ] L1381 `Add`
- [ ] L1425 `Add`
- [ ] L1441 `Add`
- [ ] L1458 `Add`
- [ ] L1506 `Add`
- [ ] L1548 `Add`
- [ ] L1575 `Add`
- [ ] L1631 `Add`
- [ ] L1663 `Add`
- [ ] L1672 `Add`
- [ ] L1829 `Add`
- [ ] L1858 `Add`
- [ ] L1955 `Add`
- [ ] L2051 `Add`

#### `ApogeeVGC\Data\Abilities\AbilitiesVWX.cs` (17 calls, **1 interp**)

- [ ] L32 `Add`
- [ ] L86 `Add`
- [ ] L95 `Add`
- [ ] L104 `Add`
- [ ] L124 `Add`
- [ ] L161 `Add` `[interp]`
- [ ] L167 `Add`
- [ ] L189 `Add`
- [ ] L251 `Add`
- [ ] L260 `Add`
- [ ] L291 `Add`
- [ ] L300 `Add`
- [ ] L333 `Add`
- [ ] L398 `Add`
- [ ] L421 `Add`
- [ ] L473 `Add`
- [ ] L526 `Add`

#### `ApogeeVGC\Data\Abilities\AbilitiesYZ.cs` (1 calls)

- [ ] L124 `Add`

#### `ApogeeVGC\Data\Conditions\ConditionsABC.cs` (31 calls, **1 interp**)

- [ ] L64 `Add`
- [ ] L121 `Add`
- [ ] L126 `Add`
- [ ] L131 `Add`
- [ ] L147 `Add`
- [ ] L151 `Add`
- [ ] L159 `Add`
- [ ] L212 `Add`
- [ ] L220 `Add`
- [ ] L231 `Add`
- [ ] L248 `Add`
- [ ] L284 `Add`
- [ ] L359 `Add`
- [ ] L363 `Add`
- [ ] L368 `Add`
- [ ] L386 `Add`
- [ ] L404 `Add`
- [ ] L436 `Add`
- [ ] L441 `Add`
- [ ] L450 `Add`
- [ ] L455 `Add`
- [ ] L497 `Add`
- [ ] L558 `AddMove`
- [ ] L560 `AttrLastMove`
- [ ] L562 `Add`
- [ ] L667 `Add`
- [ ] L670 `Add`
- [ ] L674 `Add`
- [ ] L693 `Add`
- [ ] L708 `Add`
- [ ] L787 `Add` `[interp]`

#### `ApogeeVGC\Data\Conditions\ConditionsDEF.cs` (44 calls, **2 interp**)

- [ ] L53 `Add`
- [ ] L66 `Add`
- [ ] L75 `Add`
- [ ] L85 `Add`
- [ ] L103 `Add`
- [ ] L104 `AttrLastMove`
- [ ] L129 `Add`
- [ ] L144 `Add`
- [ ] L154 `Add`
- [ ] L169 `Add`
- [ ] L182 `Add`
- [ ] L215 `Add`
- [ ] L233 `Add`
- [ ] L321 `Add`
- [ ] L326 `Add`
- [ ] L341 `Add`
- [ ] L350 `Add`
- [ ] L429 `Add`
- [ ] L436 `Add`
- [ ] L501 `Add`
- [ ] L517 `Add`
- [ ] L545 `Add`
- [ ] L550 `Add`
- [ ] L562 `Add`
- [ ] L622 `Add`
- [ ] L686 `Add`
- [ ] L701 `Add`
- [ ] L712 `Add`
- [ ] L740 `Add`
- [ ] L756 `Add`
- [ ] L775 `Add`
- [ ] L790 `Add`
- [ ] L823 `Add`
- [ ] L838 `Add`
- [ ] L860 `Add`
- [ ] L920 `Add`
- [ ] L924 `Add`
- [ ] L944 `Add`
- [ ] L979 `Add`
- [ ] L1027 `Add`
- [ ] L1031 `Add`
- [ ] L1060 `Add`
- [ ] L1071 `Add` `[interp]`
- [ ] L1185 `Add` `[interp]`

#### `ApogeeVGC\Data\Conditions\ConditionsGHI.cs` (21 calls, **2 interp**)

- [ ] L41 `Add`
- [ ] L97 `Add`
- [ ] L128 `Add`
- [ ] L145 `Add`
- [ ] L193 `Add`
- [ ] L198 `Add`
- [ ] L229 `Add`
- [ ] L244 `Add`
- [ ] L267 `Add`
- [ ] L295 `Add`
- [ ] L309 `Add`
- [ ] L328 `Add`
- [ ] L362 `Add`
- [ ] L380 `Add`
- [ ] L390 `Add`
- [ ] L411 `Add` `[interp]`
- [ ] L421 `Add` `[interp]`
- [ ] L443 `Add`
- [ ] L479 `Add`
- [ ] L498 `Add`
- [ ] L511 `Add`

#### `ApogeeVGC\Data\Conditions\ConditionsJKL.cs` (4 calls)

- [ ] L35 `Add`
- [ ] L111 `Add`
- [ ] L125 `Add`
- [ ] L255 `Add`

#### `ApogeeVGC\Data\Conditions\ConditionsMNO.cs` (17 calls, **1 interp**)

- [ ] L37 `Add` `[interp]`
- [ ] L61 `Add`
- [ ] L85 `Add`
- [ ] L108 `Add`
- [ ] L207 `Add`
- [ ] L358 `Add`
- [ ] L367 `Add`
- [ ] L381 `Add`
- [ ] L411 `Add`
- [ ] L418 `Add`
- [ ] L443 `Add`
- [ ] L474 `Add`
- [ ] L479 `Add`
- [ ] L491 `Add`
- [ ] L506 `Add`
- [ ] L518 `Add`
- [ ] L535 `Add`

#### `ApogeeVGC\Data\Conditions\ConditionsPQR.cs` (46 calls, **1 interp**)

- [ ] L38 `Add`
- [ ] L43 `Add`
- [ ] L66 `Add`
- [ ] L92 `Add`
- [ ] L112 `Add`
- [ ] L125 `Add`
- [ ] L166 `Add`
- [ ] L179 `Add` `[interp]`
- [ ] L213 `Add`
- [ ] L218 `Add`
- [ ] L249 `Add`
- [ ] L261 `Add`
- [ ] L267 `AttrLastMove`
- [ ] L284 `Add`
- [ ] L306 `Add`
- [ ] L336 `Add`
- [ ] L337 `AttrLastMove`
- [ ] L362 `Add`
- [ ] L371 `Add`
- [ ] L381 `Add`
- [ ] L397 `Add`
- [ ] L419 `Add`
- [ ] L451 `Add`
- [ ] L457 `Add`
- [ ] L463 `Add`
- [ ] L561 `Add`
- [ ] L584 `Add`
- [ ] L593 `Add`
- [ ] L642 `Add`
- [ ] L672 `Add`
- [ ] L677 `Add`
- [ ] L690 `Add`
- [ ] L708 `Add`
- [ ] L714 `Add`
- [ ] L720 `Add`
- [ ] L796 `Add`
- [ ] L811 `Add`
- [ ] L832 `Add`
- [ ] L874 `Add`
- [ ] L965 `Add`
- [ ] L971 `Add`
- [ ] L979 `Add`
- [ ] L989 `Add`
- [ ] L1050 `Add`
- [ ] L1064 `Add`
- [ ] L1162 `Add`

#### `ApogeeVGC\Data\Conditions\ConditionsSTU.cs` (73 calls, **3 interp**)

- [ ] L54 `Add`
- [ ] L80 `Add`
- [ ] L94 `Add`
- [ ] L106 `Add`
- [ ] L121 `Add`
- [ ] L138 `Add`
- [ ] L170 `Add`
- [ ] L175 `Add`
- [ ] L183 `Add`
- [ ] L197 `Add`
- [ ] L230 `Add`
- [ ] L250 `Add`
- [ ] L318 `Add`
- [ ] L322 `Add`
- [ ] L326 `Add`
- [ ] L354 `Add`
- [ ] L402 `Add`
- [ ] L416 `Add`
- [ ] L450 `Add`
- [ ] L455 `Add`
- [ ] L463 `Add`
- [ ] L476 `Add`
- [ ] L499 `Add`
- [ ] L509 `Add`
- [ ] L535 `Add`
- [ ] L555 `Add`
- [ ] L652 `Add`
- [ ] L685 `Add`
- [ ] L694 `Add`
- [ ] L717 `Add` `[interp]`
- [ ] L739 `Add` `[interp]`
- [ ] L769 `Add`
- [ ] L789 `Add`
- [ ] L796 `Add`
- [ ] L810 `Add`
- [ ] L838 `Add`
- [ ] L841 `AttrLastMove`
- [ ] L868 `Add`
- [ ] L880 `Add`
- [ ] L928 `Add`
- [ ] L971 `Add`
- [ ] L976 `Add`
- [ ] L990 `Add`
- [ ] L1000 `Add`
- [ ] L1020 `Add`
- [ ] L1041 `Add`
- [ ] L1057 `Add`
- [ ] L1078 `Add`
- [ ] L1097 `Add`
- [ ] L1144 `Add`
- [ ] L1157 `Add`
- [ ] L1176 `Add`
- [ ] L1195 `Add`
- [ ] L1218 `Add`
- [ ] L1233 `Add`
- [ ] L1246 `Add`
- [ ] L1262 `Add`
- [ ] L1271 `Add`
- [ ] L1296 `Add`
- [ ] L1299 `Add`
- [ ] L1304 `Add`
- [ ] L1334 `Add`
- [ ] L1344 `Add`
- [ ] L1357 `Add`
- [ ] L1390 `Add`
- [ ] L1414 `Add` `[interp]`
- [ ] L1438 `Add`
- [ ] L1531 `AttrLastMove`
- [ ] L1596 `Add`
- [ ] L1619 `Add`
- [ ] L1626 `Add`
- [ ] L1642 `Add`
- [ ] L1646 `Add`

#### `ApogeeVGC\Data\Conditions\ConditionsVWX.cs` (7 calls, **1 interp**)

- [ ] L39 `Add`
- [ ] L51 `Add`
- [ ] L96 `Add`
- [ ] L113 `Add`
- [ ] L168 `Add`
- [ ] L214 `Add` `[interp]`
- [ ] L231 `Add`

#### `ApogeeVGC\Data\Conditions\ConditionsYZ.cs` (2 calls, **1 interp**)

- [ ] L31 `Add` `[interp]`
- [ ] L41 `Add`

#### `ApogeeVGC\Data\Items\ItemsABC.cs` (13 calls)

- [ ] L41 `Add`
- [ ] L44 `Add`
- [ ] L200 `Add`
- [ ] L206 `Add`
- [ ] L219 `Add`
- [ ] L334 `Add`
- [ ] L615 `Add`
- [ ] L693 `Add`
- [ ] L822 `Add`
- [ ] L903 `Add`
- [ ] L948 `Add`
- [ ] L980 `Add`
- [ ] L1068 `Add`

#### `ApogeeVGC\Data\Items\ItemsDEF.cs` (1 calls)

- [ ] L664 `Add`

#### `ApogeeVGC\Data\Items\ItemsGHI.cs` (1 calls)

- [ ] L208 `Add`

#### `ApogeeVGC\Data\Items\ItemsJKL.cs` (3 calls)

- [ ] L73 `Add`
- [ ] L105 `Add`
- [ ] L286 `Add`

#### `ApogeeVGC\Data\Items\ItemsMNO.cs` (3 calls)

- [ ] L199 `Add`
- [ ] L229 `Add`
- [ ] L604 `Add`

#### `ApogeeVGC\Data\Items\ItemsPQR.cs` (7 calls)

- [ ] L47 `Add`
- [ ] L80 `Add`
- [ ] L314 `AttrLastMove`
- [ ] L315 `AddMove`
- [ ] L495 `Add`
- [ ] L679 `Add`
- [ ] L768 `Add`

#### `ApogeeVGC\Data\Items\ItemsSTU.cs` (3 calls)

- [ ] L55 `Add`
- [ ] L186 `Add`
- [ ] L650 `Add`

#### `ApogeeVGC\Data\Items\ItemsVWX.cs` (3 calls)

- [ ] L46 `Add`
- [ ] L150 `Add`
- [ ] L171 `Add`

#### `ApogeeVGC\Data\Items\ItemsYZ.cs` (1 calls)

- [ ] L40 `Add`

#### `ApogeeVGC\Data\Moves\MovesABC.cs` (16 calls)

- [ ] L235 `Add`
- [ ] L364 `Add`
- [ ] L365 `AttrLastMove`
- [ ] L707 `AttrLastMove`
- [ ] L708 `Add`
- [ ] L900 `AttrLastMove`
- [ ] L901 `Add`
- [ ] L1429 `Add`
- [ ] L1596 `Add`
- [ ] L1834 `Add`
- [ ] L2049 `Add`
- [ ] L2251 `Add`
- [ ] L2310 `Add`
- [ ] L2510 `Add`
- [ ] L2511 `Add`
- [ ] L2556 `Add`

#### `ApogeeVGC\Data\Moves\MovesDEF.cs` (25 calls)

- [ ] L90 `Add`
- [ ] L221 `Add`
- [ ] L237 `Add`
- [ ] L351 `Add`
- [ ] L511 `Add`
- [ ] L573 `Add`
- [ ] L576 `AttrLastMove`
- [ ] L608 `Add`
- [ ] L686 `Add`
- [ ] L705 `Add`
- [ ] L706 `AttrLastMove`
- [ ] L1221 `Add`
- [ ] L1337 `Add`
- [ ] L1345 `AttrLastMove`
- [ ] L1346 `AddMove`
- [ ] L1918 `AttrLastMove`
- [ ] L1921 `Add`
- [ ] L2126 `Add`
- [ ] L2156 `Add`
- [ ] L2636 `Add`
- [ ] L2688 `Add`
- [ ] L2789 `Add`
- [ ] L2860 `Add`
- [ ] L2940 `Add`
- [ ] L3160 `Add`

#### `ApogeeVGC\Data\Moves\MovesGHI.cs` (22 calls, **3 interp**)

- [ ] L55 `Add`
- [ ] L292 `Add`
- [ ] L336 `Add`
- [ ] L556 `Add` `[interp]`
- [ ] L591 `Add`
- [ ] L736 `Add`
- [ ] L810 `Add`
- [ ] L904 `Add`
- [ ] L918 `Add`
- [ ] L924 `Add`
- [ ] L959 `AttrLastMove`
- [ ] L960 `Add`
- [ ] L1011 `Add`
- [ ] L1062 `Add`
- [ ] L1534 `AttrLastMove`
- [ ] L1535 `Add`
- [ ] L1539 `AttrLastMove`
- [ ] L1540 `Add`
- [ ] L1648 `Add`
- [ ] L1862 `Add`
- [ ] L1986 `Add` `[interp]`
- [ ] L2080 `AttrLastMove` `[interp]`

#### `ApogeeVGC\Data\Moves\MovesJKL.cs` (4 calls, **1 interp**)

- [ ] L162 `Add`
- [ ] L548 `Add` `[interp]`
- [ ] L710 `AttrLastMove`
- [ ] L711 `Add`

#### `ApogeeVGC\Data\Moves\MovesMNO.cs` (10 calls)

- [ ] L96 `Add`
- [ ] L220 `Add`
- [ ] L580 `Add`
- [ ] L771 `Add`
- [ ] L1029 `Add`
- [ ] L1075 `Add`
- [ ] L1110 `Add`
- [ ] L1125 `Add`
- [ ] L1147 `Add`
- [ ] L1162 `Add`

#### `ApogeeVGC\Data\Moves\MovesPQR.cs` (24 calls, **1 interp**)

- [ ] L49 `Add`
- [ ] L52 `Add`
- [ ] L221 `Add`
- [ ] L239 `Add`
- [ ] L332 `Add`
- [ ] L471 `Add`
- [ ] L667 `AttrLastMove`
- [ ] L668 `Add`
- [ ] L697 `AttrLastMove`
- [ ] L698 `Add`
- [ ] L748 `Add`
- [ ] L896 `Add` `[interp]`
- [ ] L932 `Add`
- [ ] L1424 `Add`
- [ ] L1570 `Add`
- [ ] L1811 `Add`
- [ ] L1826 `Add`
- [ ] L1849 `Add`
- [ ] L1862 `Add`
- [ ] L1989 `Add`
- [ ] L2069 `Add`
- [ ] L2157 `Add`
- [ ] L2164 `Add`
- [ ] L2171 `Add`

#### `ApogeeVGC\Data\Moves\MovesSTU.cs` (46 calls, **3 interp**)

- [ ] L548 `Add`
- [ ] L623 `Add`
- [ ] L629 `Add`
- [ ] L635 `Add`
- [ ] L700 `AttrLastMove`
- [ ] L850 `Add`
- [ ] L998 `Add`
- [ ] L1069 `Add`
- [ ] L1074 `Add`
- [ ] L1142 `Add`
- [ ] L1582 `Add`
- [ ] L1590 `Add`
- [ ] L1648 `Add`
- [ ] L1653 `AttrLastMove`
- [ ] L1654 `AddMove`
- [ ] L1720 `Add`
- [ ] L1725 `AttrLastMove`
- [ ] L1726 `AddMove`
- [ ] L1885 `Add` `[interp]`
- [ ] L2103 `Add`
- [ ] L2129 `Add`
- [ ] L2137 `Add`
- [ ] L2666 `Add`
- [ ] L2780 `Add`
- [ ] L2786 `Add`
- [ ] L3198 `Add` `[interp]`
- [ ] L3202 `Add`
- [ ] L3207 `Add`
- [ ] L3214 `Add`
- [ ] L3219 `Add`
- [ ] L3280 `Add`
- [ ] L3595 `Add`
- [ ] L3614 `Add`
- [ ] L3622 `Add`
- [ ] L3623 `AttrLastMove`
- [ ] L3747 `AttrLastMove`
- [ ] L3908 `Add`
- [ ] L3911 `Add`
- [ ] L4273 `Add`
- [ ] L4283 `Add`
- [ ] L4325 `Add`
- [ ] L4605 `Add` `[interp]`
- [ ] L4609 `Add`
- [ ] L4613 `Add`
- [ ] L4620 `Add`
- [ ] L4624 `Add`

#### `ApogeeVGC\Data\Moves\MovesVWX.cs` (2 calls)

- [ ] L254 `Add`
- [ ] L284 `Add`

### Already optimised (skip)

#### ~~`ApogeeVGC\Sim\BattleClasses\Battle.Logging.cs`~~ (17 calls - already guarded internally)

---

## Progress tracker

| Category | Files | Calls | Done |
|----------|------:|------:|-----:|
| Sim\BattleClasses | 13 | ~85 | 0 |
| Sim\PokemonClasses | 5 | ~27 | 0 |
| Sim\Other (Field, Side, Utils) | 4 | ~30 | 0 |
| Data\Moves | 10 | ~116 | 0 |
| Data\Abilities | 10 | ~155 | 0 |
| Data\Items | 9 | ~60 | 0 |
| Data\Conditions | 6 | ~276 | 0 |
| **Total** | **57** | **~749** | **0** |

*(Battle.Logging.cs excluded - 16 calls already guarded internally)*
