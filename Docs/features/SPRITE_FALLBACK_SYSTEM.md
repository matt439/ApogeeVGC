# Sprite Fallback System

## Overview

The `SpriteManager` now implements an intelligent sprite fallback system to handle missing sprites gracefully.

## Fallback Order

### For Front Sprites (`GetFrontSprite`)
1. **First**: Try to load the front sprite
2. **Fallback**: Use the back sprite if front is missing
3. **Final**: Show magenta placeholder if both are missing

### For Back Sprites (`GetBackSprite`)
1. **First**: Try to load the back sprite
2. **Fallback**: Use the front sprite if back is missing
3. **Final**: Show magenta placeholder if both are missing

## Benefits

### ? Better User Experience
- Pokémon are always visible, even if one sprite type is missing
- Reduces visual clutter from magenta placeholder squares
- Makes partial sprite sets more usable

### ? Development Flexibility
- Can download just front OR back sprites initially
- Sprites work immediately without waiting for full set
- Easier to test with incomplete sprite collections

### ? Graceful Degradation
- Front sprite missing? Show back sprite instead
- Back sprite missing? Show front sprite instead
- Both missing? Show clear visual indicator (magenta)

## Example Scenarios

### Scenario 1: Missing Front Sprites
You currently have 12 front sprites missing (calyrex, calyrexice, etc.). 

**Before this change:**
- Missing front sprites ? Magenta squares for opponents
- User sees 12 magenta squares in team preview

**After this change:**
- Missing front sprites ? Back sprites shown instead
- User sees actual Pokémon sprites (just facing the wrong way)
- Still playable and recognizable

### Scenario 2: Missing xerneasactive Back Sprite
You have `xerneasactive` front sprite but no back sprite.

**Before this change:**
- Missing back sprite ? Magenta square for player's Pokémon

**After this change:**
- Missing back sprite ? Front sprite shown instead
- User sees Xerneas (just facing forward instead of backward)

## How It Works

```csharp
public Texture2D GetFrontSprite(SpecieId specieId)
{
    // 1. Try front sprite
    if (_frontSprites.TryGetValue(specieId, out var frontSprite) 
        && frontSprite != _missingSprite)
    {
        return frontSprite;  // ? Found front sprite
    }

    // 2. Fallback to back sprite
    if (_backSprites.TryGetValue(specieId, out var backSprite) 
        && backSprite != _missingSprite)
    {
     return backSprite;  // ? Using back sprite as fallback
    }

    // 3. Show placeholder
    return _missingSprite!;  // ? Both missing
}
```

## Visual Impact

### Team Preview (All 6 Pokémon)
- **Opponent's team** uses front sprites (or back as fallback)
- Much better than seeing 12+ magenta squares

### In Battle (Active Pokémon)
- **Player's Pokémon** (bottom): Uses back sprites (or front as fallback)
- **Opponent's Pokémon** (top): Uses front sprites (or back as fallback)

## Current Sprite Status

Based on your current setup:

**Front sprites (12):**
- bulbasaur, ironhands, miraidon, morpeko
- ogerpon, ogerpontealtera, shaymin, shayminsky
- volcarona, xerneas, xerneasactive, xerneasneutral

**Back sprites (23):**
- All of the above PLUS:
- calyrex, calyrexice, eternatuseeternamax
- grimmsnarl, terapagos, terapagosstellar
- terapagosterastal, ursaluna, zacian
- zaciancrowned, zamazenta, zamazentacrowned

**What happens now:**
- The 12 missing front sprites will show their back sprites instead
- Only truly missing Pokémon (not in either folder) will show magenta
- Much more usable immediately!

## Future Enhancements

### Potential Improvements
1. **Flip sprites horizontally**: Could mirror the fallback sprite
   ```csharp
   return FlipHorizontally(backSprite);
   ```

2. **Visual indicator**: Tint fallback sprites slightly
   ```csharp
   return TintSprite(backSprite, Color.Yellow);
   ```

3. **Logging**: Warn about fallbacks in debug mode
   ```csharp
   Console.WriteLine($"Warning: Using back sprite for front view of {specieId}");
   ```

4. **Lazy loading**: Only load sprites when first requested
   - Reduces initial load time
   - Loads sprites on-demand

## Testing

To verify the fallback system works:

1. Delete a front sprite from `Content/Sprites/Front/`
2. Run the game
3. That Pokémon should show its back sprite instead of magenta
4. Verify in both team preview and battle views

## Notes

- The fallback check uses `!= _missingSprite` to avoid fallback loops
- Both dictionaries are checked independently
- The system is transparent to the rest of the rendering code
- No changes needed in `BattleRenderer` or `BattleGame`

## Impact on Your Current Setup

With your current sprites (12 front, 23 back), you will see:
- ? All 23 Pokémon visible in some form
- ? 12 Pokémon showing "wrong" facing (back sprite for front view)
- ? Only completely missing Pokémon show as magenta

This is a **huge improvement** over having 12 magenta squares!

You can gradually download the missing front sprites using:
```powershell
.\download-missing-sprites.ps1
```

And they'll automatically be used once Content is rebuilt.
