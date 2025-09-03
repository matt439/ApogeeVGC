namespace ApogeeVGC.Sim;



public enum GenderId
{
    M,
    F,
    N,
    Empty,
}

public static class GenderIdExtensions
{
    public static string GenderIdString(this GenderId id)
    {
        return id switch
        {
            GenderId.M => "M",
            GenderId.F => "F",
            GenderId.N => string.Empty,
            GenderId.Empty => string.Empty,
            _ => throw new ArgumentOutOfRangeException(nameof(id), id, null)
        };
    }
}

public class EventInfo
{
    public int Generation { get; set; }
    public int? Level { get; set; }
    public bool? Shiny { get; set; } // true: always shiny, 1: sometimes shiny, false/null: never shiny
    public int? ShinySometimes { get; set; } // Use this if you need to distinguish '1'
    public GenderId? Gender { get; set; }
    public string? Nature { get; set; }
    public Dictionary<StatId, int>? Ivs { get; set; }
    public int? PerfectIvs { get; set; }
    public bool? IsHidden { get; set; }
    public List<AbilityId>? Abilities { get; set; }
    public int? MaxEggMoves { get; set; }
    public List<string>? Moves { get; set; }
    public string? Pokeball { get; set; }
    public string? From { get; set; }
    public bool? Japan { get; set; }
    public bool? EmeraldEventEgg { get; set; }
}

public record Learnset
{
    public Dictionary<string, List<MoveSource>>? LearnsetData { get; init; }
    public List<EventInfo>? EventData { get; init; }
    public bool? EventOnly { get; init; }
    public List<EventInfo>? Encounters { get; init; }
    public bool? Exists { get; init; }
}

public enum SpecieId
{
    CalyrexIce,
    Miraidon,
    Ursaluna,
    Volcarona,
    Grimmsnarl,
    IronHands,
}

public record SpeciesAbility
{
    public AbilityId Slot0 { get; init; }
    public AbilityId? Slot1 { get; init; }
    public AbilityId? Hidden { get; init; }
    public AbilityId? Special { get; init; }
}


public record Specie : IEffect
{
    public required SpecieId Id { get; init; }
    public EffectType EffectType => EffectType.Specie;
    public int Num { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? BaseSpecies { get; init; }
    public string? Forme { get; init; }
    public List<PokemonType> Types { get; init; } = [];
    public GenderId Gender { get; init; }
    public StatsTable BaseStats { get; init; } = new();
    public SpeciesAbility Abilities { get; init; } = new();
    public double Height { get; init; } // in meters
    public double Weight { get; init; } // in kilograms
    public string Color { get; init; } = string.Empty;
    public int Gen { get; init; }
    // Egg groups
    // Changes from
    // Gender ratio
    // Prevo
    // EvoType
    // EboCondition
    // Other forms
    // FormeOrder

    public Specie Copy()
    {
        return this with
        {
            Types = [..Types],
            BaseStats = BaseStats.Copy(),
            Abilities = new SpeciesAbility
            {
                Slot0 = Abilities.Slot0,
                Slot1 = Abilities.Slot1,
                Hidden = Abilities.Hidden,
                Special = Abilities.Special,
            },
        };
    }
}