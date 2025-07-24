using System;
using System.Diagnostics;
using System.IO;

namespace ApogeeVGC_CS.sim
{
    // Helper struct to hold player streams
    public struct PlayerStreams
    {
        public Stream Omniscient { get; set; } // Placeholder for omniscient stream
        public Stream Spectator { get; set; } // Placeholder for spectator stream
        public Stream Player1 { get; set; }
        public Stream Player2 { get; set; }
        public Stream Player3 { get; set; } // Optional, for 3-player battles
        public Stream Player4 { get; set; } // Optional, for 4-player battles
    }
    
    // Helper class to manage battle-queue functions
    static public class BattleStreamUtils
    {
        public static string[] SplitFirst(string str, string delimiter, int limit = 1)
        {
            throw new NotImplementedException("SplitFirst method is not implemented yet.");
        }

        public static PlayerStreams GetPlayerStreams(BattleStream stream)
        {
            throw new Exception("GetPlayerStreams method is not implemented yet.");
        }
    }


    public class BattleStreamOptions
    {
        public bool? Debug { get; set; }
        public bool? NoCatch { get; set; }
        public bool? KeepAlive { get; set; }
        public object? Replay { get; set; } // bool or "spectator"
    }

    public class BattleStream(BattleStreamOptions? options = null) // : ObjectReadWriteStream<string> // Implement as needed
    {
        public bool Debug { get; set; } = options?.Debug ?? false;
        public bool NoCatch { get; set; } = options?.NoCatch ?? false;
        public object Replay { get; set; } = options?.Replay ?? false;
        public bool KeepAlive { get; set; } = options?.KeepAlive ?? false;
        public Battle? Battle { get; set; } = null;

        private void Write(string chunk)
        {
            throw new NotImplementedException("Write method is not implemented yet.");
        }

        private void WriteLines(string chunk)
        {
            throw new NotImplementedException("WriteLines method is not implemented yet.");
        }

        public void PushMessage(string type, string data)
        {
            throw new NotImplementedException();
        }

        private void WriteLine(string type, string message)
        {
            throw new NotImplementedException("WriteLine method is not implemented yet.");
        }

        private void WriteEnd()
        {
            throw new NotImplementedException("WriteEnd method is not implemented yet.");
        }

        private void Destroy()
        {
            throw new NotImplementedException("Destroy method is not implemented yet.");
        }
    }


    public abstract class BattlePlayer(Stream playerStream, bool debug = false)
    {
        public Stream Stream { get; } = playerStream;
        public List<string> Log { get; } = [];
        public bool Debug { get; } = debug;

        public Delegate Start()
        {
            throw new NotImplementedException("Start method is not implemented yet.");
        }

        public void Receive(string chunk)
        {
            throw new NotImplementedException("Receive method is not implemented yet.");
        }

        public void ReceiveLine(string line)
        {
            throw new NotImplementedException("ReceiveLine method is not implemented yet.");
        }

        public void ReceiveRequest(string choice)
        {
            throw new NotImplementedException("ReceiveRequest method is not implemented yet.");
        }

        public void ReceiveError(string error)
        {
            throw new NotImplementedException("ReceiveError method is not implemented yet.");
        }

        public void Choose(string choice)
        {
            throw new NotImplementedException("Choose method is not implemented yet.");
        }
    }

    public class BattleTextStream // : Stream or custom ReadWriteStream<string>
    {
        public BattleStream BattleStream { get; }
        public string CurrentMessage { get; set; }

        public BattleTextStream(BattleStreamOptions? options = null)
        {
            BattleStream = new BattleStream(options);
            CurrentMessage = string.Empty;
            Listen();
        }

        private Delegate Listen()
        {
            throw new NotImplementedException("Listen method is not implemented yet.");
        }
        
        private void Write(string message)
        {
            throw new NotImplementedException("Write method is not implemented yet.");
        }

        private Delegate WriteEnd()
        {
            throw new NotImplementedException("WriteEnd method is not implemented yet.");
        }
    }
}