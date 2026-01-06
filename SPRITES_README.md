# Adding Pokémon Sprites to ApogeeVGC

This guide explains how to add front and back Pokémon sprites to the MonoGame GUI.

## Quick Start

### Option 1: Download from Pokémon Showdown Server (Recommended)

1. **Run the PowerShell script:**
   ```powershell
   .\copy-sprites.ps1
   ```
   This downloads sprites directly from `https://play.pokemonshowdown.com/sprites/`

2. **Generate Content.mgcb entries:**
   ```powershell
   .\generate-sprite-content.ps1
   ```

3. **Add to Content.mgcb:**
   - Open `ApogeeVGC/Content/Content.mgcb` in a text editor
   - Append the contents of `sprite-content-entries.txt` to the end
   - Save the file

4. **Build the content project:**
   - Open `Content.mgcb` in **MGCB Editor** (or run `mgcb-editor-wpf Content.mgcb`)
   - Press F6 to build
   - Or run: `dotnet mgcb Content.mgcb`

### Option 2: Download from PokeAPI (Alternative Source)

If Showdown's server is unavailable, use PokeAPI instead:

```powershell
.\download-sprites-pokeapi.ps1
```

Then follow steps 2-4 from Option 1.

### Option 3: Manual Download

Download sprites manually from these sources:

- **Pokémon Showdown**: https://play.pokemonshowdown.com/sprites/
  - Front: `gen5ani/[name].png` or `dex/[name].png`
  - Back: `gen5ani-back/[name].png` or `gen5-back/[name].png`

- **PokeAPI GitHub**: https://github.com/PokeAPI/sprites
  - Front: `sprites/pokemon/[id].png`
  - Back: `sprites/pokemon/back/[id].png`

- **PokéSprite**: https://github.com/msikma/pokesprite
  - Comprehensive sprite collection

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
?   ?  ??? calyrexice.png
?   ?       ??? miraidon.png
?   ?       ??? ... (other Pokémon)
?   ??? Content.mgcb
??? Gui/
    ??? Rendering/
     ??? SpriteManager.cs
```

## Naming Convention

Sprite filenames must match the `SpecieId` enum values in **lowercase**:

| SpecieId | Filename | Showdown Name |
|----------|----------|---------------|
| `CalyrexIce` | `calyrexice.png` | `calyrex-ice.png` |
| `Miraidon` | `miraidon.png` | `miraidon.png` |
| `IronHands` | `ironhands.png` | `ironhands.png` |
| `ShayminSky` | `shayminsky.png` | `shaymin-sky.png` |

## Sprite Sources Explained

### Pokémon Showdown Sprite Server

Pokémon Showdown doesn't store sprites in their Git repository. Instead, they serve sprites dynamically from:
- **Base URL**: `https://play.pokemonshowdown.com/sprites/`
- **Gen 5 Animated Front**: `gen5ani/[name].png`
- **Gen 5 Animated Back**: `gen5ani-back/[name].png`
- **Static Dex Front**: `dex/[name].png`
- **Static Gen 5 Back**: `gen5-back/[name].png`

The `copy-sprites.ps1` script downloads directly from these URLs.

## Adding New Pokémon Sprites

1. **Update the sprite map** in `copy-sprites.ps1`:
   ```powershell
   $spriteMap = @{
       # ... existing entries ...
    "newpokemon" = "new-pokemon"  # SpecieId -> Showdown name
   }
   ```

2. **Run the download script:**
   ```powershell
   .\copy-sprites.ps1
   ```

3. **Regenerate Content.mgcb entries:**
   ```powershell
   .\generate-sprite-content.ps1
   ```

4. **Update SpriteManager.cs** to load the new species:
   ```csharp
   LoadSpriteForSpecies(content, SpecieId.NewPokemon);
   ```

## Sprite Requirements

- **Format**: PNG with transparency
- **Recommended size**: 96x96 pixels (Showdown sprites vary, will be scaled)
- **Transparent background** (or magenta #FF00FF for color key)
- **Front sprites**: Show Pokémon facing left (opponent's view)
- **Back sprites**: Show Pokémon facing right (player's view)

## Troubleshooting

### Download script fails with "Access Denied" or 404
- Check your internet connection
- Try the alternative PokeAPI script: `.\download-sprites-pokeapi.ps1`
- Some forms may not be available; the script will skip them

### "Content file not found" error at runtime
- Ensure sprites are in `Content/Sprites/Front/` and `Content/Sprites/Back/`
- Check that filenames match exactly (lowercase, no spaces)
- Verify Content.mgcb has been built successfully (check for .xnb files in Content/bin/)

### Sprites appear as magenta squares
- Check that the sprite files exist with correct names
- Ensure Content project has been built
- Verify the sprite is being loaded in `SpriteManager.LoadSprites()`
- Check the output window for ContentLoadException errors

### Build errors in Content.mgcb
- Open Content.mgcb in MGCB Editor to see specific errors
- Ensure all PNG files are valid images
- Check that file paths in Content.mgcb match actual file locations

## Performance Tips

- Use sprite atlases for better performance (combine multiple sprites into one texture)
- Consider using compressed texture formats for reduced memory usage
- Lazy-load sprites only when needed instead of loading all at once
- Cache frequently used sprites in memory

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

## Scripts Reference

| Script | Purpose |
|--------|---------|
| `copy-sprites.ps1` | Download sprites from Pokémon Showdown server |
| `download-sprites-pokeapi.ps1` | Download sprites from PokeAPI (alternative) |
| `generate-sprite-content.ps1` | Generate Content.mgcb entries for downloaded sprites |

## Future Enhancements

- **Animated sprites**: Use sprite sheets and animation system (Showdown's gen5ani sprites are animated GIFs)
- **Shiny variants**: Add alternate sprites for shiny Pokémon
- **Gender differences**: Load different sprites based on gender
- **Form variations**: Handle different forms (e.g., Alolan, Galarian, Tera)
- **Sprite caching**: Implement LRU cache for memory efficiency
- **Automatic updates**: Script to check for new/updated sprites from Showdown
