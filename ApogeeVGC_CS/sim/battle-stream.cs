using System;
using System.Diagnostics;
using System.IO;

namespace ApogeeVGC_CS.sim
{
    public class BattleStreamOptions
    {
        public bool Debug { get; set; }
        public bool NoCatch { get; set; }
        public bool KeepAlive { get; set; }
        public object? Replay { get; set; } // bool or "spectator"
    }

    public class BattleStream // : ObjectReadWriteStream<string> // Implement as needed
    {
        public bool Debug { get; set; }
        public bool NoCatch { get; set; }
        public object? Replay { get; set; } // bool or "spectator"
        public bool KeepAlive { get; set; }
        public Battle? Battle { get; set; }

        public BattleStream(BattleStreamOptions? options = null)
        {
            Debug = options?.Debug ?? false;
            NoCatch = options?.NoCatch ?? false;
            Replay = options?.Replay ?? false;
            KeepAlive = options?.KeepAlive ?? false;
            Battle = null;
        }
        //TODO
    }
    public abstract class BattlePlayer
    {
        public Stream Stream { get; }
        public List<string> Log { get; }
        public bool Debug { get; }

        protected BattlePlayer(Stream playerStream, bool debug = false)
        {
            Stream = playerStream;
            Log = new List<string>();
            Debug = debug;
        }
        // TODO
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

        private void Listen()
        {
            // TODO: Implement message listening logic
        }
        // TODO
    }
}