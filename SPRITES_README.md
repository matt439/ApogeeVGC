# Adding Pokémon Sprites to ApogeeVGC

This guide explains how to add front and back Pokémon sprites to the MonoGame GUI.

## Quick Start

### Option 1: Copy from Pokémon Showdown (Recommended)

1. **Run the PowerShell script:**
   ```powershell
   .\copy-sprites.ps1
   ```

2. **Add sprites to Content Project:**
   - Open `ApogeeVGC/Content/Content.mgcb` in **MGCB Editor**
   - Right-click on "Content" ? "Add" ? "Existing Item"
   - Navigate to `Content/Sprites/Front/` and select all PNG files
   - Repeat for `Content/Sprites/Back/`
   - Save and build the content project

### Option 2: Download from External Sources

Download sprites from one of these sources:

- **PokeAPI Sprites**: https://github.com/PokeAPI/sprites
  - Front: `sprites/pokemon/[id].png`
  - Back: `sprites/pokemon/back/[id].png`

- **Pokémon Showdown**: https://play.pokemonshowdown.com/sprites/
  - Download the sprite pack from the GitHub repo

- **PokéSprite**: https://github.com/msikma/pokesprite

## File Structure

```
ApogeeVGC/
??? Content/
?   ??? Sprites/
?   ?   ??? Front/
?   ?   ?   ??? bulbasaur.png
?   ?   ?   ??? calyrexice.png
?   ?   ?   ??? miraidon.png
?   ?   ?   ??? ... (other Pokémon)
?   ?   ??? Back/
?   ?       ??? bulbasaur.png
?   ?       ??? calyrexice.png
?   ?       ??? miraidon.png
?   ?     ??? ... (other Pokémon)
?   ??? Content.mgcb
??? Gui/
    ??? Rendering/
        ??? SpriteManager.cs
```

## Naming Convention

Sprite filenames must match the `SpecieId` enum values in **lowercase**:

| SpecieId | Filename |
|----------|----------|
| `CalyrexIce` | `calyrexice.png` |
| `Miraidon` | `miraidon.png` |
| `IronHands` | `ironhands.png` |
| `ShayminSky` | `shayminsky.png` |

## Adding New Pokémon Sprites

1. **Add sprite files** to `Content/Sprites/Front/` and `Content/Sprites/Back/`
2. **Update Content.mgcb** using MGCB Editor or manually:
   ```
   #begin Sprites/Front/newpokemon.png
   /importer:TextureImporter
   /processor:TextureProcessor
   /processorParam:ColorKeyEnabled=True
   /processorParam:ColorKeyColor=255,0,255,255
   /processorParam:GenerateMipmaps=False
   /processorParam:PremultiplyAlpha=True
   /processorParam:ResizeToPowerOfTwo=False
   /processorParam:TextureFormat=Color
/build:Sprites/Front/newpokemon.png
   ```

3. **Update SpriteManager.cs** to load the new species:
   ```csharp
   LoadSpriteForSpecies(content, SpecieId.NewPokemon);
   ```

## Sprite Requirements

- **Format**: PNG with transparency
- **Recommended size**: 96x96 pixels (will be scaled if different)
- **Transparent background** (or magenta #FF00FF for color key)
- **Front sprites**: Show Pokémon facing left (opponent's view)
- **Back sprites**: Show Pokémon facing right (player's view)

## Using MGCB Editor

1. **Open MGCB Editor:**
   - Right-click `Content.mgcb` ? "Open With" ? "MGCB Editor"
   - Or run: `mgcb-editor Content.mgcb`

2. **Add Content:**
 - Click "Content" in tree view
   - Click "Add Existing Item" toolbar button
   - Select sprite files
   - Set processor to "Texture - MonoGame"

3. **Build:**
   - Press F6 or click "Build" toolbar button
   - Check for errors in output window

## Troubleshooting

### "Content file not found" error
- Ensure sprites are in `Content/Sprites/Front/` and `Content/Sprites/Back/`
- Check that filenames match exactly (lowercase, no spaces)
- Verify Content.mgcb has been built successfully

### Sprites appear as magenta squares
- Check that the sprite files exist with correct names
- Ensure Content project has been built
- Verify the sprite is being loaded in `SpriteManager.LoadSprites()`

### Sprites are too large/small
- Adjust `PokemonSpriteSize` constant in `BattleRenderer.cs`
- Or modify sprite rendering to scale: `spriteBatch.Draw(sprite, destRect, Color.White)`

## Performance Tips

- Use sprite atlases for better performance (combine multiple sprites into one texture)
- Consider using compressed texture formats for reduced memory usage
- Lazy-load sprites only when needed instead of loading all at once

## Advanced: Dynamic Sprite Loading

To load all sprites automatically without hardcoding each species:

```csharp
public void LoadSprites(ContentManager content, GraphicsDevice graphicsDevice)
{
    _missingSprite = CreateMissingTexture(graphicsDevice);

    // Load sprites for all SpecieId enum values
    foreach (SpecieId specieId in Enum.GetValues<SpecieId>())
    {
        LoadSpriteForSpecies(content, specieId);
    }
}
```

## Future Enhancements

- **Animated sprites**: Use sprite sheets and animation system
- **Shiny variants**: Add alternate sprites for shiny Pokémon
- **Gender differences**: Load different sprites based on gender
- **Form variations**: Handle different forms (e.g., Alolan, Galarian)
- **Sprite caching**: Implement LRU cache for memory efficiency
