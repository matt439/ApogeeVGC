# Working Without MGCB Editor - Command Line Guide

## Problem
The MGCB Editor GUI doesn't open from Visual Studio.

## Solution
Use the command-line `mgcb` tool instead. It works perfectly!

---

## ? Quick Reference Commands

### Build Content
```powershell
# From project root
.\build-content.ps1

# Or manually
cd ApogeeVGC\Content
mgcb Content.mgcb
```

### Clean and Rebuild
```powershell
.\build-content.ps1 -Rebuild
```

### Check Missing Sprites
```powershell
.\download-missing-sprites.ps1
```

---

## ?? Current Status

Based on the build output, you have:
- ? **36 content files** built successfully
- ? **12 front sprites**
- ? **23 back sprites**  
- ? **1 font**

### ?? Missing Sprites

**Missing FRONT sprites** (have back, need front):
- calyrex
- calyrexice
- eternatuseeternamax
- grimmsnarl
- terapagos
- terapagosstellar
- terapagosterastal
- ursaluna
- zacian
- zaciancrowned
- zamazenta
- zamazentacrowned

**Missing BACK sprites** (have front, need back):
- xerneasactive

---

## ?? Step-by-Step Workflow

### 1. Download Missing Sprites
```powershell
.\download-missing-sprites.ps1
```

This will:
- Check which sprites are missing
- Download them from Pokémon Showdown
- Save them to the correct folders

### 2. Regenerate Content.mgcb Entries
```powershell
.\generate-sprite-content.ps1
```

This creates `sprite-content-entries.txt` with all sprite entries.

### 3. Update Content.mgcb

**Option A: Manual (Safest)**
1. Open `ApogeeVGC\Content\Content.mgcb` in a text editor
2. Find the `#-------------------------------- Sprites -----------------------------------#` section
3. Replace everything below it (except the NOTE comment) with contents of `sprite-content-entries.txt`

**Option B: PowerShell (Automated)**
```powershell
# Backup first!
Copy-Item "ApogeeVGC\Content\Content.mgcb" "ApogeeVGC\Content\Content.mgcb.backup"

# Then update (script to be created if needed)
```

### 4. Build Content
```powershell
.\build-content.ps1
```

### 5. Run Your Game!
The sprites will now load properly.

---

## ??? Manual MGCB Commands

If you prefer to work directly with mgcb:

### Build
```powershell
cd ApogeeVGC\Content
mgcb Content.mgcb
```

### Clean
```powershell
mgcb Content.mgcb /clean
```

### Rebuild (clean + build)
```powershell
mgcb Content.mgcb /clean
mgcb Content.mgcb
```

### Get Help
```powershell
mgcb /?
```

---

## ?? Understanding the Output

After building, you'll see files in:
```
ApogeeVGC/Content/bin/Windows/
??? Fonts/
?   ??? DefaultFont.xnb
??? Sprites/
    ??? Front/
    ?   ??? bulbasaur.xnb
    ?   ??? miraidon.xnb
    ?   ??? ... (12 files)
    ??? Back/
        ??? bulbasaur.xnb
        ??? calyrexice.xnb
        ??? ... (23 files)
```

The `.xnb` files are compiled binary versions of your PNG sprites that MonoGame uses at runtime.

---

## ?? Troubleshooting

### "Command 'mgcb' not found"
Install the MGCB tool:
```powershell
dotnet tool install -g dotnet-mgcb
```

### Build fails with "File not found"
- Check that PNG files exist in `Content/Sprites/Front/` and `Content/Sprites/Back/`
- Verify paths in Content.mgcb match actual file locations
- Ensure file names are lowercase

### Sprites don't load at runtime
1. Verify Content project built successfully (check for .xnb files)
2. Ensure `Content.RootDirectory = "Content"` in BattleGame.cs
3. Check sprite names match `SpecieId` enum (lowercase)
4. Verify SpriteManager is loading the sprites

### Content.mgcb syntax errors
- Check for typos in file paths
- Ensure each `#begin` has matching `/build` and `/importer` lines
- Use `generate-sprite-content.ps1` to create correct entries

---

## ?? Adding New Sprites Later

1. **Download the sprite PNG files**
2. **Place in** `Content/Sprites/Front/` or `/Back/`
3. **Run** `.\generate-sprite-content.ps1`
4. **Update** `Content.mgcb` with new entries from `sprite-content-entries.txt`
5. **Build** with `.\build-content.ps1`

---

## ?? Tips

### Faster Builds
The `mgcb` tool only rebuilds changed files (incremental builds), so subsequent builds are faster.

### Check What Changed
```powershell
git status ApogeeVGC/Content/Sprites/
```

### See All Built Content
```powershell
Get-ChildItem "ApogeeVGC\Content\bin\Windows" -Recurse -Filter "*.xnb"
```

### Verify Sprite Count
```powershell
$front = (Get-ChildItem "ApogeeVGC\Content\bin\Windows\Sprites\Front\*.xnb").Count
$back = (Get-ChildItem "ApogeeVGC\Content\bin\Windows\Sprites\Back\*.xnb").Count
Write-Host "Front: $front, Back: $back"
```

---

## ?? Scripts Reference

| Script | Purpose |
|--------|---------|
| `build-content.ps1` | Build or rebuild MonoGame content |
| `download-missing-sprites.ps1` | Download sprites that are missing |
| `copy-sprites.ps1` | Download all sprites fresh from Showdown |
| `generate-sprite-content.ps1` | Generate Content.mgcb entries |

---

## ? Your Content is Already Built!

The good news: Your content built successfully with 36 files. You just need to:
1. Download the 12 missing front sprites + 1 back sprite
2. Update Content.mgcb
3. Rebuild
4. Run the game!

```powershell
# Quick fix
.\download-missing-sprites.ps1
.\generate-sprite-content.ps1
# Update Content.mgcb manually
.\build-content.ps1
```

That's it! No MGCB Editor GUI needed. ??
