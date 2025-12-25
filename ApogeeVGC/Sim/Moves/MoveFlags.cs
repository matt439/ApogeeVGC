namespace ApogeeVGC.Sim.Moves;

public record MoveFlags
{
    public bool? AllyAnim { get; init; }
    public bool? BypassSub { get; init; }
    public bool? Bite { get; init; }
    public bool? Bullet { get; init; }
    public bool? CantUseTwice { get; init; }
    public bool? Charge { get; init; }
    public bool? Contact { get; set; }
    public bool? Dance { get; init; }
    public bool? Defrost { get; init; }
    public bool? Distance { get; init; }
    public bool? FailCopycat { get; init; }
    public bool? FailEncore { get; init; }
    public bool? FailInstruct { get; init; }
    public bool? FailMeFirst { get; init; }
    public bool? FailMimic { get; init; }
    public bool? FutureMove { get; init; }
    public bool? Gravity { get; init; }
    public bool? Heal { get; init; }
    public bool? Metronome { get; init; }
    public bool? Mirror { get; init; }
    public bool? MustPressure { get; init; }
    public bool? NoAssist { get; init; }
    public bool? NonSky { get; init; }
    public bool? NoParentalBond { get; init; }
    public bool? NoSketch { get; init; }
    public bool? NoSleepTalk { get; init; }
    public bool? PledgeCombo { get; init; }
    public bool? Powder { get; init; }
    public bool? Protect { get; init; }
    public bool? Pulse { get; init; }
    public bool? Punch { get; init; }
    public bool? Recharge { get; init; }
    public bool? Reflectable { get; init; }
    public bool? Slicing { get; init; }
    public bool? Snatch { get; init; }
    public bool? Sound { get; init; }
    public bool? Wind { get; init; }
}