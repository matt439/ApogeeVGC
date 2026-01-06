namespace ApogeeVGC.Sim.Utils.Unions;

/// <summary>
/// int | 'level' | false
/// </summary>
public abstract record MoveDamage
{
    public static implicit operator MoveDamage(int value) => new IntMoveDamage(value);
 public static MoveDamage FromLevel() => new LevelMoveDamage();
    public static MoveDamage FromFalse() => new FalseMoveDamage();
}

public record IntMoveDamage(int Value) : MoveDamage;
public record LevelMoveDamage : MoveDamage;
public record FalseMoveDamage : MoveDamage;
