using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ApogeeVGC.Data;
using ApogeeVGC.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace ApogeeVGC;

/// <summary>
/// Entry point for running the Pokemon Showdown WebSocket server.
/// </summary>
public class ServerProgram
{
    public static async Task RunServerAsync(string[] args)
    {
var builder = WebApplication.CreateBuilder(args);
     
        // Configure URLs
 builder.WebHost.UseUrls("http://localhost:8000");
        
        // Register services
        builder.Services.AddSingleton<Library>();
        builder.Services.AddSingleton<WebSocketServer>();
        builder.Services.AddCors(options =>
        {
        options.AddDefaultPolicy(policy =>
            {
  policy.AllowAnyOrigin()
              .AllowAnyMethod()
       .AllowAnyHeader();
      });
        });
    
        var app = builder.Build();
     
        // Enable CORS FIRST
        app.UseCors();
      
      // Serve static files from pokemon-showdown-client directory
        // Find the solution root by looking for the .sln file or going up from the project directory
        var currentDir = Directory.GetCurrentDirectory();
     Console.WriteLine($"Current directory: {currentDir}");
        
        // Try to find the solution root (where pokemon-showdown-client should be)
        string? solutionRoot = currentDir;
        while (solutionRoot != null && !Directory.Exists(Path.Combine(solutionRoot, "pokemon-showdown-client")))
        {
  var parent = Directory.GetParent(solutionRoot);
 solutionRoot = parent?.FullName;
        }
        
        if (solutionRoot == null || !Directory.Exists(Path.Combine(solutionRoot, "pokemon-showdown-client")))
     {
  Console.WriteLine($"? WARNING: Could not find pokemon-showdown-client directory");
  Console.WriteLine($"  Searched from: {currentDir}");
        }
        else
        {
            var fullClientPath = Path.Combine(solutionRoot, "pokemon-showdown-client");
            Console.WriteLine($"? Client directory found!");
        Console.WriteLine($"  Solution root: {solutionRoot}");
   Console.WriteLine($"  Serving static files from: {fullClientPath}");
  
         // Serve files directly (no RequestPath prefix)
 var fileProvider = new PhysicalFileProvider(fullClientPath);
   
            app.UseDefaultFiles(new DefaultFilesOptions
            {
    FileProvider = fileProvider,
   RequestPath = "",
         DefaultFileNames = new List<string> { "index.html" }
            });
       
         app.UseStaticFiles(new StaticFileOptions
            {
  FileProvider = fileProvider,
     RequestPath = "",
     ServeUnknownFileTypes = false
          });
            
      // Test if index.html exists
    var testFile = Path.Combine(fullClientPath, "play.pokemonshowdown.com", "index.html");
            if (File.Exists(testFile))
          {
     Console.WriteLine($"  ? index.html found - ready to serve!");
      }
    else
            {
 Console.WriteLine($"  ? index.html NOT found at {testFile}");
    }
        }
  
   // Enable WebSockets
  app.UseWebSockets();
        
   // Log ALL requests AFTER static files (so we only log dynamic requests)
 app.Use(async (context, next) =>
        {
     Console.WriteLine($"REQUEST: {context.Request.Method} {context.Request.Path}{context.Request.QueryString}");
  await next();
        });
        
        // WebSocket endpoint (Pokemon Showdown uses /showdown/websocket)
    app.Map("/showdown/websocket", async context =>
     {
 Console.WriteLine($"*** HIT /showdown/websocket endpoint ***");
     if (context.WebSockets.IsWebSocketRequest)
       {
      Console.WriteLine($"WebSocket request received from {context.Connection.RemoteIpAddress}");
   var webSocketServer = context.RequestServices.GetRequiredService<WebSocketServer>();
         var webSocket = await context.WebSockets.AcceptWebSocketAsync();
         await webSocketServer.HandleConnectionAsync(webSocket, context.RequestAborted);
          }
     else
  {
    Console.WriteLine("Not a WebSocket request!");
         context.Response.StatusCode = 426; // Upgrade Required
       await context.Response.WriteAsync("WebSocket connection required");
  }
        });

        // Also handle /websocket for fallback
        app.Map("/websocket", async context =>
        {
        Console.WriteLine($"*** HIT /websocket endpoint ***");
            if (context.WebSockets.IsWebSocketRequest)
            {
     Console.WriteLine($"WebSocket request received (fallback) from {context.Connection.RemoteIpAddress}");
       var webSocketServer = context.RequestServices.GetRequiredService<WebSocketServer>();
       var webSocket = await context.WebSockets.AcceptWebSocketAsync();
   await webSocketServer.HandleConnectionAsync(webSocket, context.RequestAborted);
      }
  else
            {
 Console.WriteLine("Not a WebSocket request!");
   context.Response.StatusCode = 426; // Upgrade Required
             await context.Response.WriteAsync("WebSocket connection required");
    }
      });

        // Server list endpoint - tells client where this server is
        app.MapGet("/~~{server}/action.php", (string server) =>
        {
    Console.WriteLine($"GET Action endpoint for server: {server}");
// Return empty JSON array (matches Pokemon Showdown behavior)
  return Results.Json(new object[] { });
        });
        
        // POST version for name changes and other actions
   app.MapPost("/~~{server}/action.php", async (HttpContext context, string server) =>
        {
     Console.WriteLine($"POST Action endpoint for server: {server}");
   
        // Read the form data
            var form = await context.Request.ReadFormAsync();
  var act = form["act"].ToString();
       
        Console.WriteLine($"  Action: {act}");
   
      if (act == "getassertion")
   {
                // Client requesting authentication assertion
                var userid = form["userid"].ToString();
     Console.WriteLine($"  Auth request for user: {userid}");
             
   // Return just the assertion string as plain text
   context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync(userid);
         return Results.Empty;
            }
            
 // For other actions, just acknowledge
    return Results.Json(new { actionsuccess = true });
  });

        // Also add explicit route for localhost
        app.MapPost("/~~localhost/action.php", async (HttpContext context) =>
        {
     Console.WriteLine($"POST ~~localhost/action.php endpoint hit");
   
    // Read the form data
            var form = await context.Request.ReadFormAsync();
     var act = form["act"].ToString();
        
            Console.WriteLine($"  Action: {act}");
   
            if (act == "getassertion")
  {
             // Client requesting authentication assertion
    var userid = form["userid"].ToString();
       Console.WriteLine($"  Auth request for user: {userid}");
   
// Return just the assertion string as plain text
             context.Response.ContentType = "text/plain";
     await context.Response.WriteAsync(userid);
     return Results.Empty;
        }
 
            // For other actions, just acknowledge
     return Results.Json(new { actionsuccess = true });
        });

        // Server action endpoint (Pokemon Showdown client polls this)
      app.MapGet("/action.php", () => 
        {
   Console.WriteLine("Action endpoint hit");
      return Results.Json(new object[] { });
        });
   
        app.MapPost("/action.php", async (HttpContext context) =>
        {
   Console.WriteLine("POST Action endpoint");
    var form = await context.Request.ReadFormAsync();
            var act = form["act"].ToString();
            Console.WriteLine($"  Action: {act}");
    
     if (act == "getassertion")
         {
    var userid = form["userid"].ToString();
           Console.WriteLine($"  Auth request for user: {userid}");
                
    // Return just the assertion string as plain text
        context.Response.ContentType = "text/plain";
     await context.Response.WriteAsync(userid);
    return Results.Empty;
            }
     
            return Results.Json(new { actionsuccess = true });
        });
    
        // Config endpoint for testclient
      app.MapGet("/config/{file}", (string file) =>
        {
            Console.WriteLine($"Config requested: {file}");
 return Results.Json(new object[] { });
        });

        // Server info endpoint (for client compatibility)
    app.MapGet("/crossdomain.php", async (HttpContext context) =>
      {
    Console.WriteLine("Crossdomain/serverlist endpoint hit");
  return Results.Text("*", "text/plain");
        });
        
        Console.WriteLine("==============================================");
        Console.WriteLine("Pokemon Showdown C# Server");
        Console.WriteLine("==============================================");
   Console.WriteLine($"Server URL: http://localhost:8000");
        Console.WriteLine();
        Console.WriteLine("To connect:");
        Console.WriteLine("1. Open browser: http://localhost:8000/play.pokemonshowdown.com/");
     Console.WriteLine("2. Open console (F12)");
        Console.WriteLine("3. Run: app.connect('localhost:8000')");
  Console.WriteLine();
        Console.WriteLine("Press Ctrl+C to stop");
        Console.WriteLine("==============================================");
 Console.WriteLine();
      
 await app.RunAsync();
    }
}
