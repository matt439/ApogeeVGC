# Sprite Download Guide - Quick Reference

## ? Fixed Issue

**Problem**: The original `copy-sprites.ps1` tried to copy from `pokemon-showdown\public\sprites`, but Pokémon Showdown doesn't store sprites in their Git repository.

**Solution**: Updated script now downloads sprites directly from Pokémon Showdown's web server at `https://play.pokemonshowdown.com/sprites/`

## ?? Quick Start (3 Steps)

### Step 1: Download Sprites
```powershell
.\copy-sprites.ps1
```

### Step 2: Generate Content Entries
```powershell
.\generate-sprite-content.ps1
```

### Step 3: Update Content.mgcb
Open `ApogeeVGC\Content\Content.mgcb` and append the contents of `sprite-content-entries.txt` to the end.

## ?? Available Scripts

| Script | What It Does |
|--------|--------------|
| **copy-sprites.ps1** | Downloads sprites from Pokémon Showdown server (recommended) |
| **download-sprites-pokeapi.ps1** | Alternative: Downloads from PokeAPI if Showdown is down |
| **generate-sprite-content.ps1** | Creates Content.mgcb entries for your downloaded sprites |
| **test-sprite-urls.ps1** | Tests if Showdown sprite URLs are accessible before downloading |

## ?? Testing Before Download

Run this to verify Showdown's server is accessible:
```powershell
.\test-sprite-urls.ps1
```

## ?? Where Sprites Come From

### Pokémon Showdown Server
- **Base URL**: `https://play.pokemonshowdown.com/sprites/`
- **Front sprites** (animated): `gen5ani/[pokemon-name].png`
- **Front sprites** (static fallback): `dex/[pokemon-name].png`
- **Back sprites** (animated): `gen5ani-back/[pokemon-name].png`
- **Back sprites** (static fallback): `gen5-back/[pokemon-name].png`

### Example URLs
- Bulbasaur front: `https://play.pokemonshowdown.com/sprites/gen5ani/bulbasaur.png`
- Bulbasaur back: `https://play.pokemonshowdown.com/sprites/gen5ani-back/bulbasaur.png`
- Calyrex-Ice front: `https://play.pokemonshowdown.com/sprites/gen5ani/calyrex-ice.png`

## ?? Current Pokémon in Script

The script currently downloads sprites for these Pokémon:
- Bulbasaur
- Calyrex (Ice Rider)
- Miraidon
- Ursaluna
- Volcarona
- Grimmsnarl
- Iron Hands
- Calyrex
- Shaymin (Sky)
- Shaymin
- Greninja (Bond)
- Rockruff (Dusk)
- Terapagos (Terastal & Stellar)
- Zacian (Normal & Crowned)
- Zamazenta (Normal & Crowned)
- Ogerpon (Normal & Teal)
- Eternatus (Eternamax)
- Morpeko
- Xerneas (Neutral & Active)

## ? Adding More Pokémon

Edit `copy-sprites.ps1` and add to the `$spriteMap`:

```powershell
$spriteMap = @{
    # Existing entries...
    "charizard" = "charizard"  # Simple name
    "meowscarada" = "meowscarada"  # Gen 9
    "landorustherian" = "landorus-therian"  # Forme with hyphen
}
```

**Naming Rules**:
- Left side: Your SpecieId in lowercase
- Right side: Showdown's name (usually lowercase with hyphens for forms)

## ?? Troubleshooting

### "Cannot download sprite"
- Check internet connection
- Try: `.\test-sprite-urls.ps1` to verify server is accessible
- Use alternative: `.\download-sprites-pokeapi.ps1`

### "Content not found" at runtime
- Did you run `generate-sprite-content.ps1`?
- Did you append `sprite-content-entries.txt` to `Content.mgcb`?
- Did you build the Content project?

### Build the Content Project
Using MGCB Editor:
```
1. Open Content.mgcb in MGCB Editor
2. Press F6 (or click Build toolbar button)
3. Check output for errors
```

Or command line:
```powershell
cd ApogeeVGC\Content
mgcb Content.mgcb
```

## ?? Expected Results

After running all scripts and building:

```
ApogeeVGC/Content/
??? Sprites/
?   ??? Front/
?   ?   ??? bulbasaur.png (downloaded)
?   ?   ??? miraidon.png (downloaded)
?   ?   ??? ... (26+ sprites)
?   ??? Back/
?       ??? bulbasaur.png (downloaded)
???? miraidon.png (downloaded)
?       ??? ... (26+ sprites)
??? bin/
    ??? Windows/
     ??? Content/
      ??? Sprites/
                ??? Front/
       ?   ??? *.xnb (compiled)
        ??? Back/
      ??? *.xnb (compiled)
```

## ?? What Happens in Game

Once sprites are loaded:
- **Team Preview**: Shows all 6 Pokémon with **front sprites**
- **In Battle**: 
  - Your active Pokémon = **back sprites** (bottom)
  - Opponent's active Pokémon = **front sprites** (top)
- **Missing sprites**: Show as **magenta squares**

## ?? Workflow Summary

```
1. .\copy-sprites.ps1
   ? Downloads PNG files from Showdown
   
2. .\generate-sprite-content.ps1
   ? Creates sprite-content-entries.txt
 
3. Append to Content.mgcb
 ? Tells MonoGame about the sprites
   
4. Build Content Project
   ? Compiles PNG ? XNB format
   
5. Run ApogeeVGC
   ? Sprites appear in GUI!
```

## ?? Still Having Issues?

Check `SPRITES_README.md` for detailed documentation and troubleshooting.
