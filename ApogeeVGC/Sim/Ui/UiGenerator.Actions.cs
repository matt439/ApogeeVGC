using ApogeeVGC.Data;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using System.Text;

namespace ApogeeVGC.Sim.Ui;

public static partial class UiGenerator
{
    public static void PrintDamagingMoveAction(Pokemon attacker, Move move, int damage, Pokemon defender,
        MoveEffectiveness effectiveness, bool isCrit = false)
    {
        StringBuilder sb = new();
        sb.AppendLine($"{attacker.Name} used {move.Name} on {defender.Name}.");
        if (isCrit)
        {
            sb.AppendLine("Critical hit!");
        }

        switch (effectiveness)
        {
            case MoveEffectiveness.Normal:
                sb.AppendLine("It was a normal hit.");
                break;
            case MoveEffectiveness.SuperEffective2X:
                sb.AppendLine("It's super effective!");
                break;
            case MoveEffectiveness.SuperEffective4X:
                sb.AppendLine("It was extremely super effective!");
                break;
            case MoveEffectiveness.NotVeryEffective05X:
                sb.AppendLine("It's not very effective.");
                break;
            case MoveEffectiveness.NotVeryEffective025X:
                sb.AppendLine("It was extremely not very effective.");
                break;
            case MoveEffectiveness.Immune:
                sb.AppendLine("It had no effect.");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(effectiveness),
                    effectiveness, "Invalid move effectiveness.");
        }

        sb.AppendLine($"It dealt {damage} damage.");
        Console.WriteLine(sb.ToString());
    }

    public static void PrintMoveImmuneAction(Pokemon attacker, Move move, Pokemon defender)
    {
        StringBuilder sb = new();
        sb.AppendLine($"{attacker.Name} used {move.Name} on {defender.Name}.");
        sb.AppendLine($"{defender.Name} is immune!");
        Console.WriteLine(sb.ToString());
    }

    public static void PrintStatusMoveAction(Pokemon attackr, Move move)
    {
        StringBuilder sb = new();
        sb.AppendLine($"{attackr.Name} used {move.Name}.");
        Console.Write(sb.ToString());
    }

    public static void PrintStruggleAction(Pokemon attacker, int damage, int recoil, Pokemon defender)
    {
        StringBuilder sb = new();
        sb.AppendLine($"{attacker.Name} used Struggle on {defender.Name}.");
        sb.AppendLine("It dealt " + damage + " damage.");
        sb.AppendLine(attacker.Name + " took " + recoil + " recoil damage.");
        Console.WriteLine(sb.ToString());
    }

    public static void PrintMoveMissAction(Pokemon attacker, Move move, Pokemon defender)
    {
        StringBuilder sb = new();
        sb.AppendLine($"{attacker.Name} used {move.Name} on {defender.Name}.");
        sb.AppendLine("But it missed!");
        Console.WriteLine(sb.ToString());
    }

    public static void PrintMoveFailAction(Pokemon attacker, Move move)
    {
        StringBuilder sb = new();
        sb.AppendLine($"{attacker.Name} used {move.Name}.");
        sb.AppendLine("But it failed!");
        Console.WriteLine(sb.ToString());
    }

    public static void PrintMoveNoEffectAction(Pokemon attacker, Move move, Pokemon defender)
    {
        StringBuilder sb = new();
        sb.AppendLine($"{attacker.Name} used {move.Name} on {defender.Name}.");
        sb.AppendLine("But it had no effect!");
        Console.WriteLine(sb.ToString());
    }

    public static void PrintStallMoveProtection(Pokemon attacker, Move move, Pokemon defender)
    {
        StringBuilder sb = new();
        sb.AppendLine($"{attacker.Name} used {move.Name} on {defender.Name}.");
        sb.AppendLine($"{defender.Name} protected itself.");
        Console.WriteLine(sb.ToString());
    }

    public static void PrintFakeOutOnTryFail(Pokemon attacker, Pokemon defender)
    {
        StringBuilder sb = new();
        sb.AppendLine($"{attacker.Name} used Fake Out on {defender.Name}.");
        sb.AppendLine("But if failed becuase fake out must be first move used.");
        Console.WriteLine(sb.ToString());
    }

    public static void PrintRecoilDamageAction(Pokemon attacker, int recoilDamage)
    {
        Console.WriteLine($"{attacker.Name} is hurt by recoil and lost {recoilDamage} HP!\n");
    }

    public static void PrintDisabledMoveTry(Pokemon attacker, Move move)
    {
        StringBuilder sb = new();
        sb.AppendLine($"{attacker.Name} tried to use {move.Name}.");
        sb.AppendLine("But it is disabled!");
        Console.WriteLine(sb.ToString());
    }
}