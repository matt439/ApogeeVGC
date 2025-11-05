# MonoGame Content Directory

This directory contains all game assets (sprites, fonts, sounds, etc.) for the battle GUI.

## Structure

- **Fonts/** - SpriteFont definitions (.spritefont files)
- **Sprites/** - Pokémon sprites, UI elements, backgrounds
- **Audio/** - Sound effects and music (future)

## Adding Content

1. Add your asset files to the appropriate subfolder
2. Edit `Content.mgcb` using the MGCB Editor or manually
3. Assets will be compiled during build and loaded at runtime

## Getting Started

To create a basic font:
1. Create a `.spritefont` file in the Fonts folder
2. Add it to Content.mgcb
3. Load it in code: `Content.Load<SpriteFont>("Fonts/YourFont")`
