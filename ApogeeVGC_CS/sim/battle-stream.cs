using System.Net.Mail;

namespace ApogeeVGC_CS.sim
{
    // Helper struct to hold player streams
    public struct PlayerStreams
    {
        public Stream Omniscient { get; init; } // Placeholder for omniscient stream
        public Stream Spectator { get; init; } // Placeholder for spectator stream
        public Stream Player1 { get; init; }
        public Stream Player2 { get; init; }
        public Stream Player3 { get; init; } // Optional, for 3-player battles
        public Stream Player4 { get; init; } // Optional, for 4-player battles
    }

    // Helper class to manage battle-queue functions
    public static class BattleStreamUtils
    {
        /**
         * Like string.split(delimiter), but only recognizes the first `limit`
         * delimiters (default 1).
         *
         * `"1 2 3 4".split(" ", 2) => ["1", "2"]`
         *
         * `Utils.splitFirst("1 2 3 4", " ", 1) => ["1", "2 3 4"]`
         *
         * Returns an array of length exactly limit + 1.
         */
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
        public bool? Debug { get; init; }
        public bool? NoCatch { get; init; }
        public bool? KeepAlive { get; init; }
        public BattleStreamReplay? Replay { get; init; }

    }

    public class BattleStream(BattleStreamOptions? options = null)
    {
        public bool Debug { get; init; } = options?.Debug ?? false;
        public bool NoCatch { get; init; } = options?.NoCatch ?? false;
        public BattleStreamReplay Replay { get; init; } =
            options?.Replay ?? new BoolBattleStreamReplay(false);
        public bool KeepAlive { get; init; } = options?.KeepAlive ?? false;
        public Battle? Battle { get; init; } = null;

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