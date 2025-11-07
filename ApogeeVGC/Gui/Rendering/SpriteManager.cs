using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ApogeeVGC.Sim.SpeciesClasses;

namespace ApogeeVGC.Gui.Rendering;

/// <summary>
/// Manages loading and retrieval of Pokémon sprites
/// </summary>
public class SpriteManager
{
    private readonly Dictionary<SpecieId, Texture2D> _frontSprites = new();
    private readonly Dictionary<SpecieId, Texture2D> _backSprites = new();
    private Texture2D? _missingSprite;

    /// <summary>
    /// Load all Pokémon sprites from content
    /// </summary>
    public void LoadSprites(ContentManager content, GraphicsDevice graphicsDevice)
    {
        // Create a simple missing sprite texture (magenta square)
        _missingSprite = CreateMissingTexture(graphicsDevice);

        // Load sprites for each species
        // You can expand this to load all sprites dynamically
        //LoadSpriteForSpecies(content, SpecieId.Bulbasaur);
        LoadSpriteForSpecies(content, SpecieId.CalyrexIce);
        LoadSpriteForSpecies(content, SpecieId.Miraidon);
        LoadSpriteForSpecies(content, SpecieId.Ursaluna);
        LoadSpriteForSpecies(content, SpecieId.Volcarona);
        LoadSpriteForSpecies(content, SpecieId.Grimmsnarl);
        LoadSpriteForSpecies(content, SpecieId.IronHands);
        // Add more species as needed...
    }

    /// <summary>
    /// Load sprites for a specific species
    /// </summary>
    private void LoadSpriteForSpecies(ContentManager content, SpecieId specieId)
    {
        string speciesName = GetSpriteFileName(specieId);

        try
        {
            // Load front sprite
            string frontPath = $"Sprites/Front/{speciesName}";
            _frontSprites[specieId] = content.Load<Texture2D>(frontPath);
        }
        catch (ContentLoadException)
        {
            // Sprite not found, will use missing texture
            _frontSprites[specieId] = _missingSprite!;
        }

        try
        {
            // Load back sprite
            string backPath = $"Sprites/Back/{speciesName}";
            _backSprites[specieId] = content.Load<Texture2D>(backPath);
        }
        catch (ContentLoadException)
        {
            // Sprite not found, will use missing texture
            _backSprites[specieId] = _missingSprite!;
        }
    }

    /// <summary>
    /// Get the front sprite for a species
    /// </summary>
    public Texture2D GetFrontSprite(SpecieId specieId)
    {
        // Try to get front sprite
        if (_frontSprites.TryGetValue(specieId, out Texture2D? frontSprite) && frontSprite != _missingSprite)
        {
            return frontSprite;
        }

        // Fallback to back sprite if front is missing
        if (_backSprites.TryGetValue(specieId, out Texture2D? backSprite) && backSprite != _missingSprite)
        {
            return backSprite;
        }

        // Finally use missing sprite placeholder
        return _missingSprite!;
    }

    /// <summary>
    /// Get the back sprite for a species
    /// </summary>
    public Texture2D GetBackSprite(SpecieId specieId)
    {
        // Try to get back sprite
        if (_backSprites.TryGetValue(specieId, out Texture2D? backSprite) && backSprite != _missingSprite)
        {
            return backSprite;
        }

        // Fallback to front sprite if back is missing
        if (_frontSprites.TryGetValue(specieId, out Texture2D? frontSprite) && frontSprite != _missingSprite)
        {
            return frontSprite;
        }

        // Finally use missing sprite placeholder
        return _missingSprite!;
    }

    /// <summary>
    /// Convert SpecieId to lowercase filename
    /// </summary>
    private static string GetSpriteFileName(SpecieId specieId)
    {
        // Convert enum to lowercase (e.g., "CalyrexIce" -> "calyrexice")
        return specieId.ToString().ToLowerInvariant();
    }

    /// <summary>
    /// Create a magenta placeholder texture for missing sprites
    /// </summary>
    private static Texture2D CreateMissingTexture(GraphicsDevice graphicsDevice)
    {
        var texture = new Texture2D(graphicsDevice, 96, 96);
        var data = new Microsoft.Xna.Framework.Color[96 * 96];
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = Microsoft.Xna.Framework.Color.Magenta;
        }

        texture.SetData(data);
        return texture;
    }

    /// <summary>
    /// Unload all sprite textures
    /// </summary>
    public void Unload()
    {
        foreach (Texture2D sprite in _frontSprites.Values)
        {
            sprite.Dispose();
        }

        foreach (Texture2D sprite in _backSprites.Values)
        {
            sprite.Dispose();
        }

        _missingSprite?.Dispose();

        _frontSprites.Clear();
        _backSprites.Clear();
    }
}