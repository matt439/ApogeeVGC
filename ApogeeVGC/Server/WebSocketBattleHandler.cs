using System.Net.WebSockets;
using System.Text;
using ApogeeVGC.Sim.Player;

namespace ApogeeVGC.Server;

/// <summary>
/// Bridges WebSocket messages between the client and the BattleStream.
/// </summary>
public class WebSocketBattleHandler
{
    private readonly WebSocket _webSocket;
    private readonly PlayerStreams _streams;
    private readonly CancellationToken _cancellationToken;
    private string _username = "Guest";
    
    public WebSocketBattleHandler(
        WebSocket webSocket,
        PlayerStreams streams,
        CancellationToken cancellationToken)
    {
        _webSocket = webSocket;
        _streams = streams;
        _cancellationToken = cancellationToken;
    }

    /// <summary>
    /// Runs the handler, managing bidirectional communication.
    /// </summary>
    public async Task RunAsync()
    {
        // Send initial connection messages
        await SendInitializationMessagesAsync();
  
        // Start task to send battle output to client
        Task sendTask = SendBattleOutputToClientAsync();
        
     // Start task to receive client input
        Task receiveTask = ReceiveClientInputAsync();
        
    // Wait for either task to complete
        await Task.WhenAny(sendTask, receiveTask);
        
    // Clean up
        _streams.Dispose();
    }

    /// <summary>
    /// Sends initialization messages to set up the client.
    /// </summary>
    private async Task SendInitializationMessagesAsync()
    {
        // Generate random guest number (6 digits like real showdown)
        var guestNumber = new Random().Next(1000000, 9999999);
        _username = $"Guest {guestNumber}"; // Space is OK!

        Console.WriteLine("=== SENDING INITIALIZATION SEQUENCE ===");
        
        // Send challstr first
        await SendToClientAsync(">challstr|4|00000000");
        await Task.Delay(100);
    
        // Send updateuser - unnamed guest (0), avatar 101, empty JSON object
 // IMPORTANT: Spaces after pipes match real Pokemon Showdown format!
        // Last field MUST be valid JSON (even if empty: {})
     await SendToClientAsync($"|updateuser| {_username}|0|101|{{}}");
    await Task.Delay(100);
        
        // Send formats
        await SendToClientAsync("|formats|,1|S/V Singles|[Gen 9] Random Battle,f|[Gen 9] Unrated Random Battle,b|[Gen 9] OU,e|[Gen 9] Ubers,e|[Gen 9] UU,e|[Gen 9] Custom Game,c|,1|S/V Doubles|[Gen 9] Random Doubles Battle,f,4|[Gen 9] Doubles OU,e,4|[Gen 9] VGC 2024 Reg G,c,4|[Gen 9] Doubles Custom Game,c,4");
        await Task.Delay(100);
      
        // Initialize lobby room
        await SendToClientAsync(">lobby\n|init|chat\n|title|Lobby\n|users|0");
await Task.Delay(100);
      
        Console.WriteLine("=== INITIALIZATION COMPLETE ===");
        Console.WriteLine($"Username set to: {_username}");
     Console.WriteLine("Client should now show 'Choose name' button");
    }

    /// <summary>
    /// Sends a message to the WebSocket client.
    /// </summary>
    private async Task SendToClientAsync(string message)
    {
        // Log the message (truncate if too long)
        var preview = message.Length > 200 ? message.Substring(0, 200) + "..." : message;
        Console.WriteLine($"SEND ({message.Length} bytes):\n{preview}\n");
 
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        await _webSocket.SendAsync(
            new ArraySegment<byte>(buffer),
            WebSocketMessageType.Text,
        endOfMessage: true,
            _cancellationToken);
    }

    /// <summary>
    /// Sends battle output from the omniscient stream to the WebSocket client.
    /// </summary>
    private async Task SendBattleOutputToClientAsync()
  {
        try
        {
   await foreach (string chunk in _streams.Omniscient.ReadAllAsync(_cancellationToken))
     {
            await SendToClientAsync(chunk);
    }
        }
        catch (OperationCanceledException)
        {
  Console.WriteLine("Send operation cancelled");
        }
        catch (WebSocketException ex)
        {
         Console.WriteLine($"WebSocket send error: {ex.Message}");
}
    }

    /// <summary>
    /// Receives input from the WebSocket client and writes it to the omniscient stream.
    /// </summary>
    private async Task ReceiveClientInputAsync()
    {
     var buffer = new byte[4096];
        
        try
 {
        while (_webSocket.State == WebSocketState.Open)
    {
     WebSocketReceiveResult result = await _webSocket.ReceiveAsync(
     new ArraySegment<byte>(buffer),
        _cancellationToken);
        
            if (result.MessageType == WebSocketMessageType.Close)
  {
  Console.WriteLine("Client requested close");
        break;
        }
                
             if (result.MessageType == WebSocketMessageType.Text)
           {
     string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine($"RECV: {message}");
         
            // Handle client commands
           await HandleClientCommandAsync(message);
       }
            }
        }
   catch (OperationCanceledException)
        {
          Console.WriteLine("Receive operation cancelled");
        }
        catch (WebSocketException ex)
      {
            Console.WriteLine($"WebSocket receive error: {ex.Message}");
    }
    }

    /// <summary>
    /// Handles incoming client commands and sends appropriate responses.
    /// </summary>
    private async Task HandleClientCommandAsync(string message)
    {
        // Pokemon Showdown protocol: messages start with |/ for commands
        if (message.StartsWith("|/cmd rooms"))
        {
            // Send available rooms list (empty)
    await SendToClientAsync(">queryresponse|rooms|null");
        }
   else if (message.StartsWith("|/cmd roomlist"))
        {
            // Send room list
            await SendToClientAsync(">queryresponse|roomlist|null");
        }
      else if (message.StartsWith("|/cmd userdetails"))
        {
 // Send user details
            var parts = message.Split('|');
  if (parts.Length > 1)
            {
                var username = parts[1].Replace("/cmd userdetails ", "").Trim();
     await SendToClientAsync($">queryresponse|userdetails|{{\"userid\":\"{username.ToLower()}\",\"name\":\"{username}\",\"rooms\":false}}");
    }
        }
        else if (message.StartsWith("|/autojoin"))
        {
            // Client wants to auto-join rooms
       await SendToClientAsync(">noinit|autojoin\n|");
        }
      else if (message.StartsWith("|/updatesettings"))
      {
            // Settings updated - just acknowledge
       Console.WriteLine("Client settings updated");
  }
    else if (message.StartsWith("|/trn "))
        {
     // Client wants to set username
         // Format: |/trn username,challengekeyid,challenge
            var parts = message.Split(' ', 2);
         if (parts.Length > 1)
{
       // Extract just the username (before the first comma)
    var fullData = parts[1].Trim();
       var dataParts = fullData.Split(',');
                _username = dataParts[0]; // Just the username
           
           Console.WriteLine($"Username changed to: {_username}");
       // Send updateuser with named=1 (like real Pokemon Showdown)
                await SendToClientAsync($"|updateuser| {_username}|1|101|{{}}");
 }
        }
        else if (message.StartsWith("|/utm "))
 {
            // Client sent team - acknowledge
            Console.WriteLine("Team data received");
        }
        else if (message.StartsWith("|/challenge") || message.StartsWith("|/search"))
        {
   // Client wants to start a battle
Console.WriteLine("Battle request received");
            await SendToClientAsync(">popup|Battle system not yet implemented!\\n\\nYour C# simulator is working, but battle initiation from the client needs to be connected.\\n\\nTo test battles, use CLI mode:\\ndotnet run doubles");
        }
  else if (message.StartsWith("|/avatar"))
      {
     // Client changing avatar - acknowledge
      var parts = message.Split(' ', 2);
      if (parts.Length > 1)
   {
    Console.WriteLine($"Avatar changed to: {parts[1]}");
     }
        }
        else if (message.StartsWith("|/cmd "))
        {
   // Generic command - log it
            Console.WriteLine($"Command: {message}");
        }
  else
      {
            // For other messages, log them
            Console.WriteLine($"Unhandled: {message}");
        }
    }
}
