# BattleGame Lifecycle: Before and After Fix

## BEFORE (Broken - Race Condition)

```
Main Thread    Background Thread
???????????????????????????????       ?????????????????????????
new BattleGame()
  ?
  ?? Constructor
  ?    _choiceInputManager = null
  ?
game.StartBattle(...)
  ?  
  ???????????????????????????????????> BattleRunner.StartBattle()
                 ?
game.Run()         ?? Task.Run(async () => {
  ?             ?  await Battle.StartAsync()
  ?? Initialize()       ?      ?
  ?          ?      ?? RunPickTeam()
  ?? LoadContent()         ?      ?   MakeRequest(TeamPreview)
  ?    _choiceInputManager =   ?      ?   
  ?      new ChoiceInputManager()       ?   ?? RequestAndWaitForChoicesAsync()
  ?     ?           ?
  ?            ?         ?? PlayerController.RequestChoiceAsync()
  ?        ?           ?
  ?          ?  ?? GuiWindow.RequestChoiceAsync()
  ?                 ?    ?
  ?       ?     ? CRASH!
  ?       ?              _choiceInputManager is NULL
  ?? Update()               
  ?? Draw()
```

**Problem**: Battle thread reaches `RequestChoiceAsync()` before `LoadContent()` initializes `_choiceInputManager`.

---

## AFTER (Fixed - Deferred Start)

```
Main Thread         Background Thread
???????????????????????????????  ?????????????????????????
new BattleGame()
  ?
  ?? Constructor
  ?    _choiceInputManager = null
  ?    _shouldStartBattle = false
  ?
game.StartBattle(...)
  ?
  ?? Check: _choiceInputManager == null?
  ?    YES ? Queue battle parameters
  ?      _pendingLibrary = library
  ?      _pendingBattleOptions = options
  ?      _pendingPlayerController = controller
  ?      _shouldStartBattle = TRUE
  ?
  ?? Return (battle NOT started yet)
  
game.Run()
  ?
  ?? Initialize()
  ?
  ?? LoadContent()
  ?    ?? _choiceInputManager =
  ?  ?    new ChoiceInputManager() ?
  ?    ?
  ?    ?? Check: _shouldStartBattle?
  ?    ?    YES ? StartBattleInternal(...)
  ?    ??
  ?    ?   ?????????????????????> BattleRunner.StartBattle()
  ?    ?          ?
  ?    ?                    ?? Task.Run(async () => {
  ?    ?          ?    await Battle.StartAsync()
  ?    ?? _shouldStartBattle = false    ?      ?
  ?  ?      ?? RunPickTeam()
  ?? Update()        ?      ?   MakeRequest(TeamPreview)
  ?    _choiceInputManager.Update()        ?      ?
  ?  ?      ?? RequestAndWaitForChoicesAsync()
  ?? Draw()         ?           ?
  ? _choiceInputManager.Render()   ?         ?? PlayerController.RequestChoiceAsync()
  ?       ?           ?
  ?             ?    ?? GuiWindow.RequestChoiceAsync()
  ?      ?         ?
  ?    ?         ? SUCCESS!
?       ?         _choiceInputManager is initialized
  ?             ?
  ?? Update()           ?
  ?User clicks button  ?
  ?    _choiceInputManager.SubmitChoice()  ?
  ?      TaskCompletionSource.SetResult()???
  ?        
  ?? Update()   Battle continues...
  ?? Draw()
```

**Solution**: Battle doesn't actually start until after `LoadContent()` completes, ensuring all resources are ready.

---

## Key Points

### Deferred Start Mechanism

1. **Early Call**: `StartBattle()` called before `LoadContent()`
   - Parameters are saved
   - `_shouldStartBattle` flag is set
   - Method returns immediately (no battle started)

2. **Content Loading**: `LoadContent()` executes
   - `_choiceInputManager` is initialized
   - Checks `_shouldStartBattle` flag
   - If true, calls `StartBattleInternal()` to actually start the battle

3. **Safe Execution**: Battle thread runs
   - `_choiceInputManager` is guaranteed to be initialized
   - No null reference exceptions

### Benefits

? **No Race Condition**: Battle only starts after content is loaded  
? **No Blocking**: No spin-waits or thread blocking  
? **User-Friendly**: Works with natural usage pattern  
? **Thread-Safe**: Clear happens-before relationship  

### Edge Cases Handled

- **Multiple StartBattle() calls**: Second call while battle running is ignored
- **Content already loaded**: If `_choiceInputManager` exists, starts immediately
- **Late StartBattle() call**: If called after `LoadContent()`, works normally
