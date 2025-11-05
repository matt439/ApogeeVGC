using System.Net.WebSockets;
using System.Text;
using ApogeeVGC.Data;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Player;
using ApogeeVGC.Sim.Utils.Extensions;

namespace ApogeeVGC.Server;

/// <summary>
/// Handles WebSocket connections from Pokemon Showdown clients.
/// </summary>
public class WebSocketServer
{
    private readonly Library _library;
    private readonly BattleManager _battleManager;
  
    public WebSocketServer(Library library)
    {
        _library = library;
        _battleManager = new BattleManager(library);
    }

    /// <summary>
    /// Handles an incoming WebSocket connection.
    /// </summary>
    public async Task HandleConnectionAsync(WebSocket webSocket, CancellationToken cancellationToken)
    {
        Console.WriteLine("New WebSocket connection established");
        
        try
        {
            // Create battle streams for this connection
            PlayerStreams streams = BattleStreamExtensions.GetPlayerStreams(new BattleStream(_library));
       
            // Create handler for this connection
            var handler = new WebSocketBattleHandler(webSocket, streams, _battleManager, cancellationToken);
       
            // Start handling the connection
            await handler.RunAsync();
}
        catch (WebSocketException ex)
        {
            Console.WriteLine($"WebSocket error: {ex.Message}");
      }
    catch (Exception ex)
        {
            Console.WriteLine($"Connection error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
 }
  finally
        {
            if (webSocket.State == WebSocketState.Open)
            {
      await webSocket.CloseAsync(
           WebSocketCloseStatus.NormalClosure,
 "Connection closed",
          cancellationToken);
     }
     Console.WriteLine("WebSocket connection closed");
        }
    }
}
