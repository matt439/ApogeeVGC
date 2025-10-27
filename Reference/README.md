# Pokemon Showdown Reference Code

This directory contains the official Pokemon Showdown repository as a git submodule for reference purposes.

## Current Version

Currently tracking: **commit 7b0158b4ab6c6d3fda3137ecf4d44756151f2428**

## Purpose

This submodule provides the TypeScript reference implementation for porting to C# in the ApogeeVGC project. The code here is read-only and used for:

- Understanding battle mechanics
- Comparing implementation approaches
- Verifying logic against the canonical source
- Tracking updates to Showdown's battle engine

## Key Files for Reference

The most relevant files for the VGC battle simulator are located in:

- `pokemon-showdown/sim/` - Core battle simulation engine
  - `battle.ts` - Main battle orchestration
  - `side.ts` - Side/player logic
  - `pokemon.ts` - Pokemon state and actions
  - `field.ts` - Field conditions and weather
- `pokemon-showdown/sim/dex*.ts` - Data definitions and lookups

## Updating the Reference

The submodule is pinned to a specific commit. To update to a newer version of Pokemon Showdown:

```sh
cd Reference/pokemon-showdown
git fetch origin
git checkout master  # or specific tag/commit
git pull
cd ../..
git add Reference/pokemon-showdown
git commit -m "Update Showdown reference to <version/commit>"
```

## Viewing Changes Between Versions

To see what changed in Showdown between updates:

```sh
cd Reference/pokemon-showdown
git log <old-commit>..<new-commit>
git diff <old-commit> <new-commit> -- sim/
```

## Important Notes

- **Do not modify files in this directory** - they are tracked by the submodule
- The submodule is for reference only and is not built or executed
- When cloning this repository, others will need to run:
  ```sh
  git submodule init
  git submodule update
  ```
- Or clone with: `git clone --recurse-submodules <repo-url>`

## Links

- [Pokemon Showdown Repository](https://github.com/smogon/pokemon-showdown)
- [Pokemon Showdown Documentation](https://github.com/smogon/pokemon-showdown/blob/master/README.md)
- [Showdown Simulator Documentation](https://github.com/smogon/pokemon-showdown/blob/master/sim/README.md)
