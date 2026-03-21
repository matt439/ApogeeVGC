using ApogeeVGC.Sim.Core;

namespace ApogeeVGC;

public class Program
{
    public static void Main(string[] args)
    {
        // Override args for profiling
        //args = ["--mode", "Evaluate", "--format", "gen9vgc2025regi", "--player1", "ensemble", "--player2", "random", "--battles", "5", "--time-per-move", "8000", "--rollout-depth", "0", "--threads", "16"];

        // Global exception handler to prevent silent crashes
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            Console.WriteLine($"\n[FATAL] Unhandled exception: {e.ExceptionObject}");
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        };

        Driver driver = new();
        // Parse --mode argument
        DriverMode mode = DriverMode.RndVsRndEvaluation;
        string? format = null;

        // Evaluate mode args
        string? player1 = null;
        string? player2 = null;
        int? battles = null;
        int? mctsIterations = null;
        int? threads = null;
        string? output = null;

        // Remote MCTS args
        string? workerHost = null;
        int? workerPort = null;
        int? rolloutDepth = null;
        int? timePerMoveMs = null;

        // First bare argument is the mode name
        if (args.Length > 0 && Enum.TryParse(args[0], true, out DriverMode parsedMode))
        {
            mode = parsedMode;
        }

        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == "--mode" && Enum.TryParse(args[i + 1], true, out DriverMode parsed))
            {
                mode = parsed;
            }
            else if (args[i] == "--format")
            {
                format = args[i + 1];
            }
            else if (args[i] == "--player1")
            {
                player1 = args[i + 1];
            }
            else if (args[i] == "--player2")
            {
                player2 = args[i + 1];
            }
            else if (args[i] == "--battles" && int.TryParse(args[i + 1], out int b))
            {
                battles = b;
            }
            else if (args[i] == "--mcts-iterations" && int.TryParse(args[i + 1], out int mi))
            {
                mctsIterations = mi;
            }
            else if (args[i] == "--threads" && int.TryParse(args[i + 1], out int t))
            {
                threads = t;
            }
            else if (args[i] == "--output")
            {
                output = args[i + 1];
            }
            else if (args[i] == "--worker-host")
            {
                workerHost = args[i + 1];
            }
            else if (args[i] == "--worker-port" && int.TryParse(args[i + 1], out int wp))
            {
                workerPort = wp;
            }
            else if (args[i] == "--port" && int.TryParse(args[i + 1], out int p))
            {
                workerPort = p;
            }
            else if (args[i] == "--rollout-depth" && int.TryParse(args[i + 1], out int rd))
            {
                rolloutDepth = rd;
            }
            else if (args[i] == "--time-per-move" && int.TryParse(args[i + 1], out int tpm))
            {
                timePerMoveMs = tpm;
            }
        }

        // Set remote MCTS args before starting
        Driver.SetRemoteArgs(workerHost, workerPort, rolloutDepth, timePerMoveMs);

        if (mode == DriverMode.Evaluate)
        {
            driver.StartEvaluation(format, player1, player2, battles, mctsIterations, threads, output);
        }
        else
        {
            driver.Start(mode, format);
        }
    }
}
