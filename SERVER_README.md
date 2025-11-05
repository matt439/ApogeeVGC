# Pokemon Showdown C# Server

This document explains how to run the C# Pokemon Showdown simulator with the Pokemon Showdown client.

## Architecture

- **ApogeeVGC** - C# implementation of Pokemon Showdown simulator with WebSocket server
- **pokemon-showdown** - Original Node.js server (submodule, for reference)
- **pokemon-showdown-client** - Web-based client (submodule, served by C# server)

## Quick Start

### Start the C# Server

```powershell
cd C:\VSProjects\ApogeeVGC\ApogeeVGC
dotnet run server
```

The server will start on `http://localhost:8000` and serve:
- WebSocket endpoint: `ws://localhost:8000/showdown/websocket`
- Pokemon Showdown client: `http://localhost:8000/play.pokemonshowdown.com/`
- Action.php endpoint: `http://localhost:8000/action.php` (for authentication)

### Connect the Client

1. **Open your browser to:**
   ```
   http://localhost:8000/play.pokemonshowdown.com/
   ```

2. **Open browser console (F12)**

3. **Connect to the server by running this command in the console:**
   ```javascript
 Config.server = {id: 'localhost', host: 'localhost', port: 8000, httpport: 8000, altport: null, registered: false}; app.connect();
   ```

4. **You should see:**
   - Guest username in top-right corner (e.g., "Guest 1234567")
   - "Choose name" button is clickable
   - Battle formats are listed
   - Lobby room tab appears

### Change Your Username

Click "Choose name" in the top-right corner and enter your desired username. The C# server handles the authentication.

## What's Working

? **WebSocket Connection** - Full bidirectional communication  
? **User Authentication** - Guest login and username changes  
? **Protocol Implementation** - Proper Pokemon Showdown message format  
? **Static File Serving** - Client served directly from C# server  
? **Room Management** - Lobby room initialization  
? **Format Display** - Battle formats listed correctly  

## What's Not Yet Implemented

? **Battle Creation** - Starting battles from the client  
? **Battle Simulation** - Running battles through the WebSocket  
? **Team Validation** - Checking team legality  
? **Multiple Battles** - Handling concurrent battles  
? **Chat System** - Room chat functionality  

## Running in CLI Mode (Original Behavior)

To run the simulator in CLI mode with AI battles (no WebSocket server):

```powershell
cd C:\VSProjects\ApogeeVGC\ApogeeVGC

# Run a singles battle
dotnet run singles

# Run a doubles battle
dotnet run doubles

# Default (doubles)
dotnet run
```

## Project Structure

```
ApogeeVGC/
??? ApogeeVGC/
?   ??? Program.cs              # Entry point (CLI or server mode)
?   ??? Server/
?   ?   ??? ServerProgram.cs           # ASP.NET Core server with WebSocket + static files
?   ?   ??? WebSocketServer.cs         # Connection handler
?   ?   ??? WebSocketBattleHandler.cs  # Message bridge & protocol implementation
?   ??? Sim/   # Pokemon Showdown simulator logic
?   ?   ??? Core/Driver.cs      # Battle orchestration
?   ?   ??? ...
?   ??? Data/     # Pokemon data
??? pokemon-showdown/           # Node.js reference implementation (submodule)
??? pokemon-showdown-client/    # Web client (submodule, served by C# server)
```

## How It Works

```
Client (Browser)
    ? WebSocket
C# Server (Port 8000)
    ? Messages
WebSocketBattleHandler
    ? Commands
BattleStream ? ? Battle Simulator
    ? Output
WebSocketBattleHandler
    ? WebSocket
Client (Browser)
```

## Protocol

The server implements the Pokemon Showdown WebSocket protocol:

### Client ? Server
- `|/cmd rooms` - Request room list
- `|/autojoin` - Auto-join default rooms
- `|/trn <username>` - Change username
- `|/search <format>` - Search for a battle (not yet implemented)
- `|/utm <team>` - Send team data (not yet implemented)

### Server ? Client
- `>challstr|4|00000000` - Authentication challenge
- `|updateuser| <name>|<named>|<avatar>|{}` - User info update
- `|formats|...` - Available battle formats
- `>lobby\n|init|chat\n|title|Lobby\n|users|0` - Lobby room initialization

## Troubleshooting

### Client Can't Connect
- Ensure the C# server is running on port 8000
- Check that you ran `app.connect('localhost:8000')` in the browser console
- Verify no other process is using port 8000

### Authentication Errors
- Make sure you're using the correct connection command
- The server provides dummy authentication for local testing

### Static Files Not Loading
- Ensure `pokemon-showdown-client` submodule is initialized: `git submodule update --init`
- Check that the client files are built: `cd pokemon-showdown-client && npm run build-full`

## Next Steps

To fully integrate your C# battle simulator with the client:

1. **Implement battle request parsing** in `WebSocketBattleHandler.cs`
2. **Connect battle creation** to your existing `Driver.cs` logic
3. **Stream battle output** back through the WebSocket
4. **Handle player choices** (moves, switches) from the client
5. **Add battle room management** for concurrent battles

## References

- Pokemon Showdown Protocol: https://github.com/smogon/pokemon-showdown/blob/master/sim/SIM-PROTOCOL.md
- WebSocket API: https://developer.mozilla.org/en-US/docs/Web/API/WebSocket
- Your C# Simulator: `ApogeeVGC/Sim/` directory
