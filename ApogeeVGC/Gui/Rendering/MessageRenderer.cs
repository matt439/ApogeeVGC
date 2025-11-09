using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ApogeeVGC.Sim.BattleClasses;
using System.Collections.Generic;
using XnaColor = Microsoft.Xna.Framework.Color;
using XnaVector2 = Microsoft.Xna.Framework.Vector2;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;

namespace ApogeeVGC.Gui.Rendering;

/// <summary>
/// Handles rendering of battle messages in a scrollable log window
/// </summary>
public class MessageRenderer
{
    private readonly SpriteBatch _spriteBatch;
    private readonly SpriteFont _font;
    private readonly GraphicsDevice _graphicsDevice;
    
    // Layout constants
    private const int Padding = 10;
    private const int LineHeight = 20;
    private const int MaxVisibleLines = 6; // Number of messages to show at once
    
    // Message box positioning and size
    private const int MessageBoxHeight = MaxVisibleLines * LineHeight + (Padding * 2);
    private const int MessageBoxWidth = 600;
    
    // Auto-scroll tracking
    private double _lastMessageTime;
    private const double AutoScrollDelay = 3.0; // Seconds to show each new message
    
    public MessageRenderer(SpriteBatch spriteBatch, SpriteFont font, GraphicsDevice graphicsDevice)
    {
     _spriteBatch = spriteBatch;
  _font = font;
_graphicsDevice = graphicsDevice;
        _lastMessageTime = 0;
    }
    
    /// <summary>
    /// Render the message log at the bottom center of the screen
    /// </summary>
    public void Render(GameTime gameTime, List<BattleMessage> messages)
    {
        if (messages.Count == 0) return;
    
        // Calculate message box position (bottom center of screen)
        int boxX = (_graphicsDevice.Viewport.Width - MessageBoxWidth) / 2;
        int boxY = _graphicsDevice.Viewport.Height - MessageBoxHeight - 20;
        
        var messageBox = new XnaRectangle(boxX, boxY, MessageBoxWidth, MessageBoxHeight);
  
     // Draw semi-transparent background
        DrawFilledRectangle(messageBox, XnaColor.Black * 0.7f);
        
        // Draw border
        DrawRectangleBorder(messageBox, XnaColor.White, 2);
    
    // Determine which messages to show (last MaxVisibleLines messages)
        int startIndex = Math.Max(0, messages.Count - MaxVisibleLines);
        int messagesToShow = Math.Min(MaxVisibleLines, messages.Count);
        
        // Render messages from bottom to top (newest at bottom)
        int yOffset = boxY + Padding;
   for (int i = startIndex; i < messages.Count; i++)
        {
         var message = messages[i];
    string displayText = message.ToDisplayText();
            
            // Truncate message if it's too long
        float maxWidth = MessageBoxWidth - (Padding * 2);
   if (_font.MeasureString(displayText).X > maxWidth)
            {
          while (_font.MeasureString(displayText + "...").X > maxWidth && displayText.Length > 3)
                {
        displayText = displayText.Substring(0, displayText.Length - 1);
         }
      displayText += "...";
      }
 
     // Determine message color based on type
            XnaColor messageColor = GetMessageColor(message);
      
      // Draw the message text
    var textPosition = new XnaVector2(boxX + Padding, yOffset);
        _spriteBatch.DrawString(_font, displayText, textPosition, messageColor);
    
      yOffset += LineHeight;
        }
  }
    
    /// <summary>
    /// Get the display color for a message based on its type
    /// </summary>
    private XnaColor GetMessageColor(BattleMessage message)
    {
        return message switch
   {
     MoveUsedMessage => XnaColor.White,
            EffectivenessMessage em => em.Effectiveness switch
    {
     EffectivenessMessage.EffectivenessType.SuperEffective => XnaColor.LimeGreen,
     EffectivenessMessage.EffectivenessType.NotVeryEffective => XnaColor.Orange,
             EffectivenessMessage.EffectivenessType.NoEffect => XnaColor.Gray,
              _ => XnaColor.White
            },
  DamageMessage => XnaColor.Red,
          FaintMessage => XnaColor.DarkRed,
         SwitchMessage => XnaColor.Cyan,
        CriticalHitMessage => XnaColor.Yellow,
    MissMessage => XnaColor.Gray,
StatusMessage => XnaColor.Purple,
    WeatherMessage => XnaColor.LightBlue,
  StatChangeMessage => XnaColor.Gold,
          HealMessage => XnaColor.LightGreen,
ItemMessage => XnaColor.Pink,
            AbilityMessage => XnaColor.Magenta,
            TurnStartMessage => XnaColor.Yellow,
    MoveFailMessage => XnaColor.DarkGray,
       GenericMessage => XnaColor.LightGray,
            _ => XnaColor.White
   };
    }
  
    /// <summary>
    /// Draw a filled rectangle
    /// </summary>
    private void DrawFilledRectangle(XnaRectangle rect, XnaColor color)
    {
        var pixel = new Texture2D(_graphicsDevice, 1, 1);
   pixel.SetData(new[] { XnaColor.White });
        _spriteBatch.Draw(pixel, rect, color);
        pixel.Dispose();
    }
    
    /// <summary>
    /// Draw a rectangle border
    /// </summary>
    private void DrawRectangleBorder(XnaRectangle rect, XnaColor color, int lineWidth)
    {
        var pixel = new Texture2D(_graphicsDevice, 1, 1);
        pixel.SetData(new[] { XnaColor.White });
        
      // Draw four lines
        _spriteBatch.Draw(pixel, new XnaRectangle(rect.X, rect.Y, rect.Width, lineWidth), color); // Top
        _spriteBatch.Draw(pixel, new XnaRectangle(rect.X, rect.Bottom - lineWidth, rect.Width, lineWidth), color); // Bottom
  _spriteBatch.Draw(pixel, new XnaRectangle(rect.X, rect.Y, lineWidth, rect.Height), color); // Left
        _spriteBatch.Draw(pixel, new XnaRectangle(rect.Right - lineWidth, rect.Y, lineWidth, rect.Height), color); // Right
        
        pixel.Dispose();
    }
}
