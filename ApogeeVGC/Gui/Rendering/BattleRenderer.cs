using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ApogeeVGC.Gui.Rendering;

/// <summary>
/// Handles rendering of the battle scene including field, Pokémon, and UI
/// </summary>
public class BattleRenderer
{
    private readonly SpriteBatch _spriteBatch;
    private readonly SpriteFont _font;
    private readonly GraphicsDevice _graphicsDevice;
    
    // Layout constants
    private const int Padding = 20;
    private const int PokemonSpriteSize = 128;
    
    public BattleRenderer(SpriteBatch spriteBatch, SpriteFont font, GraphicsDevice graphicsDevice)
    {
        _spriteBatch = spriteBatch;
 _font = font;
        _graphicsDevice = graphicsDevice;
    }
    
    /// <summary>
    /// Render the entire battle scene
    /// </summary>
    public void Render(GameTime gameTime, BattleState? battleState)
    {
        if (battleState == null)
{
  RenderWaitingScreen();
 return;
        }
  
        RenderField(battleState);
        RenderPlayerPokemon(battleState);
        RenderOpponentPokemon(battleState);
        RenderUI(battleState);
 }
    
  private void RenderWaitingScreen()
    {
        var screenCenter = new Vector2(
   _graphicsDevice.Viewport.Width / 2f,
      _graphicsDevice.Viewport.Height / 2f);
      
        string message = "Waiting for battle to start...";
        var messageSize = _font.MeasureString(message);
        var position = screenCenter - messageSize / 2f;
        
        _spriteBatch.DrawString(_font, message, position, Color.White);
    }
    
    private void RenderField(BattleState battleState)
    {
        // TODO: Render weather, terrain, field effects
 // For now, just show field status text
        string fieldInfo = $"Turn: {battleState.Turn}";
        _spriteBatch.DrawString(_font, fieldInfo, new Vector2(Padding, Padding), Color.White);
    }
    
    private void RenderPlayerPokemon(BattleState battleState)
    {
        // TODO: Render player's active Pokémon sprites and info
     // Position in bottom-left area
    int yPosition = _graphicsDevice.Viewport.Height - PokemonSpriteSize - Padding;
        
        for (int i = 0; i < battleState.PlayerActivePokemon.Count; i++)
        {
            var pokemon = battleState.PlayerActivePokemon[i];
      int xPosition = Padding + (i * (PokemonSpriteSize + Padding));
            
            RenderPokemonInfo(pokemon, new Vector2(xPosition, yPosition), true);
        }
    }
    
    private void RenderOpponentPokemon(BattleState battleState)
    {
        // TODO: Render opponent's active Pokémon sprites and info
        // Position in top-right area
  int yPosition = Padding + 60;
        
        for (int i = 0; i < battleState.OpponentActivePokemon.Count; i++)
        {
      var pokemon = battleState.OpponentActivePokemon[i];
            int xPosition = _graphicsDevice.Viewport.Width - PokemonSpriteSize - Padding - (i * (PokemonSpriteSize + Padding));
            
    RenderPokemonInfo(pokemon, new Vector2(xPosition, yPosition), false);
     }
    }
    
    private void RenderPokemonInfo(PokemonDisplayInfo pokemon, Vector2 position, bool isPlayer)
  {
    // TODO: Draw actual sprite texture
    // For now, draw a placeholder rectangle and text
        var rect = new Rectangle((int)position.X, (int)position.Y, PokemonSpriteSize, PokemonSpriteSize);
        DrawRectangle(rect, isPlayer ? Color.Blue : Color.Red, 2);
        
 // Draw name and HP
        string info = $"{pokemon.Name}\nHP: {pokemon.CurrentHp}/{pokemon.MaxHp}";
        var textPosition = position + new Vector2(0, PokemonSpriteSize + 5);
      _spriteBatch.DrawString(_font, info, textPosition, Color.White);
    }
    
    private void RenderUI(BattleState battleState)
    {
        // TODO: Render action buttons, move selection, etc.
      // For now, show available actions
        string uiInfo = "Press ESC to exit";
        var uiPosition = new Vector2(
            _graphicsDevice.Viewport.Width / 2f - 100,
  _graphicsDevice.Viewport.Height - 40);
       
    _spriteBatch.DrawString(_font, uiInfo, uiPosition, Color.Yellow);
    }
    
    /// <summary>
    /// Helper to draw a rectangle outline
    /// </summary>
    private void DrawRectangle(Rectangle rect, Color color, int lineWidth)
    {
  // Create a 1x1 white texture if needed (lazy init in real implementation)
        var pixel = new Texture2D(_graphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });
        
     // Draw four lines
  _spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, rect.Width, lineWidth), color); // Top
      _spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Bottom - lineWidth, rect.Width, lineWidth), color); // Bottom
        _spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, lineWidth, rect.Height), color); // Left
        _spriteBatch.Draw(pixel, new Rectangle(rect.Right - lineWidth, rect.Y, lineWidth, rect.Height), color); // Right
    }
}

/// <summary>
/// Simplified battle state for rendering
/// </summary>
public class BattleState
{
    public int Turn { get; set; }
    public List<PokemonDisplayInfo> PlayerActivePokemon { get; set; } = new();
    public List<PokemonDisplayInfo> OpponentActivePokemon { get; set; } = new();
}

/// <summary>
/// Display information for a single Pokémon
/// </summary>
public class PokemonDisplayInfo
{
    public string Name { get; set; } = "";
    public int CurrentHp { get; set; }
  public int MaxHp { get; set; }
    public string Species { get; set; } = "";
    public int Level { get; set; } = 50;
}
